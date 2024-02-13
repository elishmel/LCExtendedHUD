using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace LCExtendedHUD {
    [BepInPlugin(ExtendedHUD.PLUGIN_GUID, ExtendedHUD.PLUGIN_NAME, ExtendedHUD.PLUGIN_VERSION)]
    public class ExtendedHUD : BaseUnityPlugin {

        public const string PLUGIN_GUID = "com.github.elishmel.lcextendedhud";
        public const string PLUGIN_NAME = "LC Extended HUD";
        public const string PLUGIN_VERSION = "1.1";

        internal static ManualLogSource Log;

        private void Awake() {
            ExtendedHUD.Log = base.Logger;
            
            // Plugin startup logic
            ExtendedHUD.Log.LogInfo($"Plugin {ExtendedHUD.PLUGIN_GUID} is loaded!");

            new Harmony(ExtendedHUD.PLUGIN_GUID).PatchAll(Assembly.GetExecutingAssembly());
        }
    }


}
