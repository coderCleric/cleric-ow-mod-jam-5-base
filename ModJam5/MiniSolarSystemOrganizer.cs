using NewHorizons.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModJam5;

internal static class MiniSolarSystemOrganizer
{
    public static void Apply(IEnumerable<NewHorizonsBody> bodies)
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

        var staticBodies = bodies.Where(x => (x.Config.Orbit.isStatic || x.Config.Orbit.staticPosition != null) && x.Config.Bramble?.dimension == null && !x.Config.Base.centerOfSolarSystem);
        var brambleDimensions = bodies.Where(x => x.Config.Bramble?.dimension != null);
        var regularPlanets = bodies.Where(x => (!x.Config.Orbit.isStatic && x.Config.Orbit.staticPosition == null) && x.Config.Bramble?.dimension == null);

        // TODO: Organize planets around the center 

        //HandleStaticBodies(staticBodies);
        //HandleBrambleDimensions(brambleDimensions);
        //HandleRegularPlanets(regularPlanets);
    }
}
