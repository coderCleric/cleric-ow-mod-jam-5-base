using System.Reflection;
using HarmonyLib;
using NewHorizons;
using OWML.Common;
using OWML.ModHelper;
using System.Linq;

namespace ModJam5
{
    public class ModJam5 : ModBehaviour
    {
        public static string SystemName = "Jam5";

        public static ModJam5 Instance { get; private set; }
        public INewHorizons NewHorizons;

        public bool AllowSpawnOverride { get; private set; }


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
        }

        public void FixCompatIssues()
        {
            return;
            var jamEntries = NewHorizons.GetInstalledAddons()
                .Select(ModHelper.Interaction.TryGetMod)
                .Where(addon => addon.GetDependencies().Select(x => x.ModHelper.Manifest.UniqueName).Contains(ModHelper.Manifest.UniqueName))
                .Append(this)
                .ToArray();

            ModHelper.Console.WriteLine($"Found {jamEntries.Length} jam entries");

            // Moves the planets
            MiniSolarSystemOrganizer.Apply(Main.BodyDict[SystemName]);

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
        }

        public void OnCompleteSceneLoad(OWScene previousScene, OWScene newScene)
        {
            if (newScene != OWScene.SolarSystem) return;
            ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
        }
    }

}
