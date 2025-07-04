using System.Reflection;
using HarmonyLib;
using NewHorizons;
using OWML.Common;
using OWML.ModHelper;
using System.Linq;
using NewHorizons.External;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using NewHorizons.External.SerializableData;

namespace ModJam5
{
    public class ModJam5 : ModBehaviour
    {
        public static string SystemName = "Jam5";

        public static ModJam5 Instance { get; private set; }
        public INewHorizons NewHorizons;

        public bool AllowSpawnOverride { get; private set; }
        public bool ShowAllowedVolume { get; private set; }

        private List<GameObject> allowedVolumeObjects = new();

        public void Awake()
        {
            Instance = this;
            // You won't be able to access OWML's mod helper in Awake.
            // So you probably don't want to do anything here.
            // Use Start() instead.
        }

        public override void Configure(IModConfig config)
        {
            base.Configure(config);

            AllowSpawnOverride = config.GetSettingsValue<bool>("allowSpawnOverride");
            ShowAllowedVolume = config.GetSettingsValue<bool>("showAllowedVolume");

            foreach (var sphere in allowedVolumeObjects)
            {
                if (sphere != null)
                {
                    sphere.SetActive(ShowAllowedVolume);
                }
            }
        }

        public void FixCompatIssues()
        {
            var jamEntries = NewHorizons.GetInstalledAddons()
                .Select(ModHelper.Interaction.TryGetMod)
                .Where(addon => addon.GetDependencies().Select(x => x.ModHelper.Manifest.UniqueName).Contains(ModHelper.Manifest.UniqueName))
                .Append(this)
                .ToArray();

            ModHelper.Console.WriteLine($"Found {jamEntries.Length} jam entries");

            // Moves the planets
            MiniSolarSystemOrganizer.Apply(Main.BodyDict[SystemName], jamEntries);

            // Make sure all ship log entries don't overlap
            ShipLogPacking.Apply(jamEntries);

            // Make sure that the root mod for the system remains us
            Main.SystemDict[SystemName].Mod = this;

            ModHelper.Console.WriteLine($"Finished packing jam entry ship logs");
        }

        public void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"My mod {nameof(ModJam5)} is loaded!", MessageType.Success);

            // Get the New Horizons API and load configs
            NewHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            NewHorizons.LoadConfigs(this);

            new Harmony("xen-42.ModJam5").PatchAll(Assembly.GetExecutingAssembly());

            // Example of accessing game code.
            OnCompleteSceneLoad(OWScene.TitleScreen, OWScene.TitleScreen); // We start on title screen
            LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;

            NewHorizons.GetStarSystemLoadedEvent().AddListener(OnStarSystemLoaded);
            NewHorizons.RegisterCustomBuilder(CustomBuilder);

            ModHelper.Events.Unity.FireOnNextUpdate(FixCompatIssues);
        }

        public void OnCompleteSceneLoad(OWScene previousScene, OWScene newScene)
        {
            allowedVolumeObjects.Clear();

            if (newScene != OWScene.SolarSystem) return;
            ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
        }

        public void OnStarSystemLoaded(string system)
        {
            if (system != SystemName)
            {
                return;
            }

            ModHelper.Console.WriteLine("Loaded into Jam 5 system!", MessageType.Success);
        }

        public void CustomBuilder(GameObject planet, string extrasConfig)
        {
            if (string.IsNullOrEmpty(extrasConfig))
            {
                return;
            }
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(extrasConfig);
                if (dict["isCenterOfMiniSystem"] is bool isCenter && isCenter)
                {
                    // Todo: big sphere
                    GameObject sphereGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphereGO.transform.parent = planet.transform;
                    sphereGO.transform.localPosition = Vector3.zero;
                    sphereGO.transform.localScale = Vector3.one * MiniSolarSystemOrganizer.MINI_SYSTEM_RADIUS;
                    sphereGO.GetComponent<Collider>().enabled = false;
                    sphereGO.GetComponent<MeshRenderer>().material.color = UnityEngine.Color.white;
                    var mesh = sphereGO.GetComponent<MeshFilter>().mesh;
                    var normals = mesh.normals;
                    for (var i = 0; i < normals.Length; i++) normals[i] = -normals[i];
                    mesh.normals = normals;
                    sphereGO.GetComponent<MeshFilter>().mesh = mesh;
                    for (var subMesh = 0; subMesh < mesh.subMeshCount; subMesh++)
                    {
                        var triangles = mesh.GetTriangles(subMesh);
                        for (var i = 0; i < triangles.Length; i += 3)
                        {
                            var temp = triangles[i];
                            triangles[i] = triangles[i + 1];
                            triangles[i + 1] = temp;
                        }
                        mesh.SetTriangles(triangles, subMesh);
                    }
                    sphereGO.SetActive(ShowAllowedVolume);
                    allowedVolumeObjects.Add(sphereGO);
                }
            }
            catch
            {

            }
        }

        public static void Log(string message)
        {
            Instance.ModHelper.Console.WriteLine(message, MessageType.Info);
        }

        public static void LogError(string message)
        {
            Instance.ModHelper.Console.WriteLine(message, MessageType.Error);
        }

        public static void LogDebug(string message)
        {
            // TODO: if debug
            Instance.ModHelper.Console.WriteLine("DEBUG: " + message, MessageType.Info);
        }

        private static bool GetBool(string name, JObject extras)
        {
            return extras.GetValue(name)?.ToObject<bool>() is bool result && result;
        }

        public static bool IsPlatform(NewHorizonsBody body)
        {
            if (body.Config.extras is JObject extras)
            {
                if (GetBool("isPlatform", extras))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsCenterOfMiniSystem(NewHorizonsBody body)
        {
            if (body.Config.extras is JObject extras)
            {
                if (GetBool("isCenterOfMiniSystem", extras))
                {
                    return true;
                }
            }
            return false;
        }
    }

}
