using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace LCExtendedHUD {
    [BepInPlugin(ExtendedHUD.GUID, ExtendedHUD.PLUGIN_NAME, ExtendedHUD.PLUGIN_VERSION)]
    public class ExtendedHUD : BaseUnityPlugin {

        public const string GUID = "com.github.elishmel.lcextendedhud";
        public const string PLUGIN_NAME = "LC Extended HUD";
        public const string PLUGIN_VERSION = "1.0";

        internal static ManualLogSource Log;

        private void Awake() {
            ExtendedHUD.Log = base.Logger;
            
            // Plugin startup logic
            ExtendedHUD.Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            new Harmony(ExtendedHUD.GUID).PatchAll(Assembly.GetExecutingAssembly());
        }
    }


}
