using NewHorizons.External;
using Newtonsoft.Json.Linq;
using OWML.Common;
using OWML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Analytics;

namespace ModJam5;

internal static class MiniSolarSystemOrganizer
{
    public const float BRAMBLE_PLANE_DISTANCE = 20000f;
    public const float MINI_SYSTEM_RADIUS = 2500f;
    public const float MINI_SYSTEM_DISTANCE = 10000f;

    public static void Apply(IEnumerable<NewHorizonsBody> bodies, IModBehaviour[] jamEntries)
    {
        // Only allow one spawn point for now ig and let them override the sun one immediately
        // Only include spawns that set both because idk why don't you want a ship bro?
        var bodiesWithSpawns = bodies.Where(x => x.Config.Spawn?.playerSpawn != null && x.Config.Spawn?.shipSpawn != null);
        if (bodiesWithSpawns.Count() > 1)
        {
            var foundSpawnFlag = false;
            foreach (var body in bodiesWithSpawns)
            {
                var isDefaultSpawn = body.Config.name == "Hub";
                var isValidSpawn = (isDefaultSpawn && !ModJam5.Instance.AllowSpawnOverride) || (!isDefaultSpawn && ModJam5.Instance.AllowSpawnOverride);
                var keepSpawn = isValidSpawn && !foundSpawnFlag;
                if (keepSpawn)
                {
                    foundSpawnFlag = true;
                }
                else
                {
                    body.Config.Spawn = null;
                }
            }
        }

        foreach (var body in bodies)
        {
            var hadShipLog = body.Config.ShipLog?.mapMode != null;

            // Force all bodies to have shiplogs so we can remove them if need be
            body.Config.ShipLog ??= new();
            body.Config.ShipLog.mapMode ??= new();

            // Force all planets to be automatic placement
            var mapMode = body.Config.ShipLog.mapMode;

            mapMode.manualPosition = null;
            mapMode.manualNavigationPosition = null;

            // If it's a moon with no ship logs, just get rid of it
            var isMoon = !string.IsNullOrEmpty(body.Config.Orbit.primaryBody) && body.Config.Orbit.primaryBody != "Jam 3 Sun";
            var shouldRemove = isMoon && !hadShipLog;
            if (shouldRemove)
            {
                mapMode.remove = true;
            }
            else if (isMoon)
            {
                mapMode.scale = 0.5f;
            }
        }

        var centers = bodies.Where(x => x.Config.Base.centerOfSolarSystem && x.Config.name != "Central Station");
        var staticBodies = bodies.Where(x => (x.Config.Orbit.isStatic || x.Config.Orbit.staticPosition != null) && x.Config.Bramble?.dimension == null)
                .Where(x => !x.Config.Base.centerOfSolarSystem).Where(x => !centers.Any(y => y.Config.name == x.Config.name));
        var brambleDimensions = bodies.Where(x => x.Config.Bramble?.dimension != null);

        // Verify mods are all valid
        var angularPosition = new Dictionary<string, float>();
        for (int i = 0; i < jamEntries.Length; i++)
        {
            var mod = jamEntries[i];
            angularPosition[mod.ModHelper.Manifest.UniqueName] = 2f * Mathf.PI * (float)i / (float)jamEntries.Length;
            if (!centers.Any(x => x.Mod.ModHelper.Manifest.UniqueName == mod.ModHelper.Manifest.UniqueName))
            {
                ModJam5.LogError($"INVALID JAM ENTRY {mod.ModHelper.Manifest.UniqueName} HAS NO CENTER");
            }
        }

        var centerBodyNames = new List<string>();

        foreach (var center in centers)
        {
            ModJam5.LogDebug($"Fixing mod center {center.Config.name}");

            center.Config.Orbit ??= new();
            center.Config.Base.centerOfSolarSystem = false;
            center.Config.Orbit.isStatic = true;
            var angle = angularPosition[center.Mod.ModHelper.Manifest.UniqueName];
            center.Config.Orbit.staticPosition = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward * MINI_SYSTEM_DISTANCE;
            center.Config.Orbit.primaryBody = "Central Station";
            var dict = new Dictionary<string, object>();
            if (center.Config.extras is JObject jObject)
            {
                dict = jObject.ToObject<Dictionary<string, object>>();
            }
            dict["isCenterOfMiniSystem"] = true;
            center.Config.extras = JObject.FromObject(dict);

            centerBodyNames.Add(center.Config.name.Trim().ToLowerInvariant());
        }

        foreach (var staticBody in staticBodies)
        {
            ModJam5.LogDebug($"Fixing static body position {staticBody.Config.name}");

            if (centerBodyNames.Contains(staticBody.Config.name.Trim().ToLowerInvariant()))
            {
                // No idea why this happens
                continue;
            }

            staticBody.Config.Orbit ??= new();
            staticBody.Config.Orbit.staticPosition ??= Vector3.zero;
            if (((Vector3)staticBody.Config.Orbit.staticPosition).magnitude > MINI_SYSTEM_RADIUS)
            {
                ModJam5.LogError($"INVALID JAM ENTRY {staticBody.Config.name} IS OUTSIDE MAXIMUM RADIUS {MINI_SYSTEM_RADIUS}");
            }
            var angle = angularPosition[staticBody.Mod.ModHelper.Manifest.UniqueName];
            staticBody.Config.Orbit.staticPosition += Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward * MINI_SYSTEM_DISTANCE;
        }

        HandleBrambleDimensions(brambleDimensions);
    }

    private static void HandleBrambleDimensions(IEnumerable<NewHorizonsBody> brambleDimensions)
    {
        ModJam5.LogDebug($"Handling {brambleDimensions.Count()} hidden dimensions");

        var brambleDimensionRects = new List<Rect>();

        foreach (var body in brambleDimensions)
        {
            // Take radius with padding
            // Have to add a lot of padding to include the repel volume around the dimension (about 3.2x the radius)
            var radius = body.Config.Bramble.dimension.radius * 4f;
            brambleDimensionRects.Add(new Rect(-radius, -radius, radius * 2f, radius * 2f));
        }

        var packedRectPositions = RectPacking.Apply(brambleDimensionRects.ToArray());

        for (int i = 0; i < brambleDimensions.Count(); i++)
        {
            var packedRect = packedRectPositions[i];
            var body = brambleDimensions.ElementAt(i);

            var center = packedRect.center;
            body.Config.Orbit.staticPosition = new Vector3(center.x, -BRAMBLE_PLANE_DISTANCE, center.y);
        }
    }
}
