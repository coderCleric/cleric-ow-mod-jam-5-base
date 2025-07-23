using OWML.ModHelper;
using System.Linq;
using UnityEngine;

namespace ModJam5;

public class StarshipCommunityHelper : MonoBehaviour
{
    public Material porcelain, silver, black;
    public Texture2D nomaiSuit, ember, emberEmission, ash;

    public void Awake()
    {
        ModJam5.Instance.NewHorizons.GetStarSystemLoadedEvent().AddListener(OnStarSystemLoaded);

        nomaiSuit = ModJam5.Instance.ModHelper.Assets.GetTexture("planets/assets/Character_NOM_Nomai_v2_d 1.png");
        ember = ModJam5.Instance.ModHelper.Assets.GetTexture("planets/assets/Props_HEA_CampfireEmbers_d.png");
        emberEmission = ModJam5.Instance.ModHelper.Assets.GetTexture("planets/assets/Props_HEA_CampfireEmbers_e.png");
        ash = ModJam5.Instance.ModHelper.Assets.GetTexture("planets/assets/Props_HEA_CampfireAsh_e.png");
    }

    private Material GetReplacementMaterial(Material material)
    {
        if (material.name.Contains("Structure_NOM_Whiteboard_mat") ||
            material.name.Contains("Structure_NOM_SandStone_mat") ||
            material.name.Contains("Structure_NOM_SandStone_Dark_mat") ||
            material.name.Contains("ObservatoryInterior_HEA_VillagePlanks_mat") ||
            material.name.Contains("Props_NOM_SmallTractorBeam_mat")
            )
        {
            return porcelain;
        }
        else if (material.name.Contains("Structure_NOM_PropTile_Color_mat") ||
            material.name.Contains("Structure_NOM_SandStone_Darker_mat") ||
            material.name.Contains("Structure_NOM_WarpReceiver_mat")
            )
        {
            return black;
        }
        else if (material.name.Contains("Structure_NOM_CopperOld_mat") ||
            material.name.Contains("Structure_NOM_TrimPattern_mat") ||
            material.name.Contains("Structure_NOM_CopperOld_Dark_mat") ||
            material.name.Contains("ObservatoryInterior_HEA_VillageMetal_mat")
            )
        {
            return silver;
        }
        else if (material.name.Contains("Props_NOM_Scroll_mat") ||
            material.name.Contains("Props_NOM_Mask_Trim_mat") ||
            material.name.Contains("Structure_NOM_Airlock_mat")
            )
        {
            material.color = new Color(0.05f, 0.05f, 0.05f);
        }
        else if (material.name.Contains("Character_NOM_Nomai_v2_mat"))
        {
            material.mainTexture = nomaiSuit;
        }
        else if (material.name.Contains("Props_HEA_Lightbulb_mat"))
        {
            material.SetColor("_EmissionColor", new Color(0.6f, 0.7f, 0.8f));
        }

        return material;
    }

    public void OnStarSystemLoaded(string name)
    {
        if (name == ModJam5.SystemName)
        {
            porcelain = Resources.FindObjectsOfTypeAll<Material>().First(x => x.name.Contains("Structure_NOM_PorcelainClean_mat"));
            silver = Resources.FindObjectsOfTypeAll<Material>().First(x => x.name.Contains("Structure_NOM_Silver_mat"));
            black = Resources.FindObjectsOfTypeAll<Material>().First(x => x.name.Contains("Structure_NOM_SilverPorcelain_mat"));

            var starship = ModJam5.Instance.NewHorizons.GetPlanet("Starship Community");
            ReplaceMaterials(starship);

            //var platform = ModJam5.Instance.NewHorizons.GetPlanet("Example Platform");
            //ReplaceMaterials(platform);

            var centralStation = ModJam5.Instance.NewHorizons.GetPlanet("Central Station");
            ReplaceMaterials(centralStation);

            var beam = centralStation.transform.Find("Sector/TractorBeam").GetComponentInChildren<TractorBeamFluid>();
            beam.transform.localPosition = new Vector3(0, 12.8f, 0);
            beam.transform.localRotation = Quaternion.Euler(0, 180, 180);
            beam.transform.Find("BeamParticles").gameObject.SetActive(false);
            beam.transform.Find("BeamRings").gameObject.SetActive(false);
            beam.transform.Find("BeamParticlesReverse").gameObject.SetActive(true);
            beam.transform.Find("BeamRingsReverse").gameObject.SetActive(true);

            // Add previews
            /*
            PlacePicture("EchoHike", new Vector3(-15.5f, 6.5f, 8f), new Vector3(0.92f, -0.37f, 0f));
            PlacePicture("Axiom", new Vector3(-6.5f, 15.5f, 8f), new Vector3(0.37f, -0.92f, 0f));
            PlacePicture("Callis", new Vector3(6.5f, 15.5f, 8f), new Vector3(-0.37f, -0.92f, 0f));
            PlacePicture("Finis", new Vector3(15.5f, 6.5f, 8f), new Vector3(-0.92f, -0.37f, 0f));
            PlacePicture("JamHub", new Vector3(15.5f, -6.5f, 8f), new Vector3(-0.92f, 0.37f, 0f));
            PlacePicture("Symbiosis", new Vector3(6.5f, -15.5f, 8f), new Vector3(-0.37f, 0.92f, 0f));
            PlacePicture("BandTogether", new Vector3(-15.5f, -6.5f, 8f), new Vector3(0.92f, 0.37f, 0f));

            // Winner previews
            PlacePicture("SolarRangers", new Vector3(-25f, 6.8f, 1f), new Vector3(0.75f, -0.15f, 0f));
            PlacePicture("Reflections", new Vector3(6.8f, 25f, 1f), new Vector3(-0.15f, -0.75f, 0f));
            PlacePicture("Magistarium", new Vector3(25f, -6.8f, 1f), new Vector3(-0.75f, 0.15f, 0f));

            try
            {
                SpawnCompletionItems();
            }
            catch { }
            */
        }
    }

    public void ReplaceMaterials(GameObject go)
    {
        // Replace materials on the starship community
        foreach (var renderer in go.GetComponentsInChildren<Renderer>())
        {
            renderer.materials = renderer.materials.Select(GetReplacementMaterial).ToArray();
        }

        // Replace campfires
        foreach (var campfire in go.GetComponentsInChildren<Campfire>())
        {
            var emberMaterial = campfire.transform.parent.Find("Props_HEA_Campfire/Campfire_Embers").GetComponent<MeshRenderer>().material;
            emberMaterial.SetTexture("_MainTex", ember);
            emberMaterial.SetTexture("_EmissionMap", emberEmission);

            var ashMaterial = campfire.transform.parent.Find("Props_HEA_Campfire/Campfire_Ash").GetComponent<MeshRenderer>().material;
            ashMaterial.SetTexture("_EmissionMap", ash);

            foreach (var light in campfire._lightController.lights)
            {
                light.gameObject.GetComponent<Light>().color = new Color(0f, 0f, 0f);
            }

            campfire._flames.material.color = new Color(0f, 0.2f, 1f);
        }
    }
}
