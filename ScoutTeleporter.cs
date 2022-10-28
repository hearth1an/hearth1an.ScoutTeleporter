using NewHorizons.Utility;
using OWML.Common;
using OWML.ModHelper;
using ScoutTeleporter.Utilities.ModAPIs;
using NewHorizons.Handlers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ScoutTeleporter
{
    public class ScoutTeleporter : ModBehaviour
    {
        public static INewHorizons newHorizonsAPI;
        public static ScoutTeleporter Instance;
        public static float reset;
        public ScreenPrompt _teleportPromt;
        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            var newHorizonsAPI = ModHelper.Interaction.GetModApi<INewHorizons>("xen.NewHorizons");
            newHorizonsAPI.LoadConfigs(this);
            newHorizonsAPI.GetStarSystemLoadedEvent().AddListener(OnStarSystemLoaded);
            ModHelper.Console.WriteLine($"{nameof(ScoutTeleporter)} is loaded!", MessageType.Success);            
        }
        public void UpdatePromtVisibility()
        {            
            if (Locator.GetProbe().IsAnchored() && reset == 0f)
            {
                _teleportPromt.VisibilityControllerSetVisibility(_teleportPromt, true);
            }
            if (Locator.GetProbe().IsLaunched() && reset == 0f)
            {
                _teleportPromt.VisibilityControllerSetVisibility(_teleportPromt, true);
            }
            if (reset != 0f)
            {
                _teleportPromt.VisibilityControllerSetVisibility(_teleportPromt, false);
            }
            if (Locator.GetProbe().enabled == false)
            {
                _teleportPromt.VisibilityControllerSetVisibility(_teleportPromt, false);
            }
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

            ScoutTeleporter.Instance.ModHelper.Events.Unity.FireOnNextUpdate(() =>
            {
                _teleportPromt = new ScreenPrompt(TranslationHandler.GetTranslation("TELEPORT_PROMT", TranslationHandler.TextType.UI) + " <CMD>", ImageUtilities.GetButtonSprite(KeyCode.T));
                Locator.GetPromptManager().AddScreenPrompt(_teleportPromt, PromptPosition.UpperRight, false);
                reset = 0;
            });
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
            var probeBody = Locator.GetProbe().GetComponent<OWRigidbody>();
            var playerBody = Locator.GetPlayerBody();

            playerBody._currentAngularVelocity = probeBody._currentAngularVelocity;
            playerBody._currentAccel = probeBody._currentAccel;
            playerBody._currentVelocity = new UnityEngine.Vector3 (0f, 0f, 0f);
            playerBody._currentVelocity = probeBody._currentVelocity;                      

            var BH = SearchUtilities.Find("BHTeleport");
            BH.transform.position = Locator.GetPlayerBody().transform.position;
            BH.SetActive(true);

            reset = 1f;

            Invoke("DisableBh", 0.5f);
        }
        public void DisableBh()
        {
            var BH = SearchUtilities.Find("BHTeleport");
            BH.SetActive(false);
            reset = 1f;
        }
        public void ResetTimer()
        {
            reset = 0f;
            Locator.GetPlayerAudioController().PlayEnterLaunchCodes();
        }
        public void Update()
        {
            if (reset == 0f && Locator.GetProbe().IsAnchored() && Locator.GetPlayerBody().GetComponent<PlayerBody>()._activeRigidbody == Locator.GetPlayerBody().GetComponent<Rigidbody>() && Keyboard.current[Key.T].wasReleasedThisFrame)
            {
                Invoke("EnableWh", 0.2f);
                Invoke("EnableBh", 0.3f);
                Invoke("ResetTimer", 5f);
                ModHelper.Console.WriteLine("Teleported!", MessageType.Success);

            }
            if (reset == 0f && Locator.GetProbe().IsLaunched() && Locator.GetPlayerBody().GetComponent<PlayerBody>()._activeRigidbody == Locator.GetPlayerBody().GetComponent<Rigidbody>() && Keyboard.current[Key.T].wasReleasedThisFrame)
            {
                Invoke("EnableWh", 0.2f);
                Invoke("EnableBh", 0.3f);
                Invoke("ResetTimer", 5f);
                ModHelper.Console.WriteLine("Teleported!", MessageType.Success);
            }
            UpdatePromtVisibility();
        }
    }
}