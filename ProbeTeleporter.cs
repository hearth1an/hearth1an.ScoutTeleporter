using NewHorizons.Utility;
using OWML.Common;
using OWML.ModHelper;
using ProbeTeleporter.Utilities.ModAPIs;
using UnityEngine.InputSystem;

namespace ProbeTeleporter
{
    public class ProbeTeleporter : ModBehaviour
    {
        public static INewHorizons newHorizonsAPI;
        public static ProbeTeleporter Instance;

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            var newHorizonsAPI = ModHelper.Interaction.GetModApi<INewHorizons>("xen.NewHorizons");
            newHorizonsAPI.LoadConfigs(this);
            newHorizonsAPI.GetStarSystemLoadedEvent().AddListener(OnStarSystemLoaded);
            ModHelper.Console.WriteLine($"{nameof(ProbeTeleporter)} is loaded!", MessageType.Success);            
        }

        private void OnStarSystemLoaded(string systemName)
        {
            ModHelper.Console.WriteLine("LOADED SYSTEM " + systemName);

            if (systemName == "SolarSystem")
            {
                SpawnOnStart();
            }
        }

        public void SpawnOnStart()
        {
            SearchUtilities.Find("Probe_Body/ProbeGravity/Props_NOM_GravityCrystal").transform.localScale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
            SearchUtilities.Find("Probe_Body/ProbeGravity/Props_NOM_GravityCrystal_Base").transform.localScale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);

            var WH = SearchUtilities.Find("Probe_Body/WhiteHole");
            var BH = SearchUtilities.Find("Player_Body/BlackHole");

            WH.SetActive(false);
            BH.SetActive(false);

            BH.transform.name = "BHTeleport";
            BH.transform.parent = SearchUtilities.Find("Probe_Body").transform.parent;

            ModHelper.Console.WriteLine("Parenting done!", MessageType.Success);
        }

        public void EnableWh()
        {
            var WH = SearchUtilities.Find("Probe_Body/WhiteHole");
            WH.SetActive(true);
            Invoke("DisableWh", 0.5f);
        }
        public void DisableWh()
        {
            var WH = SearchUtilities.Find("Probe_Body/WhiteHole");
            WH.SetActive(false);
        }

        public void EnableBh()
        {
            var BH = SearchUtilities.Find("BHTeleport");
            BH.transform.position = Locator.GetPlayerBody().transform.position;
            BH.SetActive(true);
            Invoke("DisableBh", 0.5f);
        }
        public void DisableBh()
        {
            var BH = SearchUtilities.Find("BHTeleport");
            BH.SetActive(false);
        }

        public void Update()
        {
            if (Locator.GetProbe().IsAnchored() && Keyboard.current[Key.T].wasReleasedThisFrame)
            {
                //Locator.GetPlayerBody().transform.position = Locator.GetProbe().transform.position;                
                Invoke("EnableWh", 0.2f);
                Invoke("EnableBh", 0.3f);
                ModHelper.Console.WriteLine("Teleported!", MessageType.Success);
            }
            if (Locator.GetProbe().IsLaunched() && Keyboard.current[Key.T].wasReleasedThisFrame)
            {
                //Locator.GetPlayerBody().transform.position = Locator.GetProbe().transform.position;                
                Invoke("EnableWh", 0.2f);
                Invoke("EnableBh", 0.3f);
                ModHelper.Console.WriteLine("Teleported!", MessageType.Success);
            }

        }
    }
}