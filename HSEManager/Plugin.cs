using BepInEx;
using BepInEx.Logging;

namespace HSEManager
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGUID = "valesthor.hsemanager";
        public const string PluginName = "Health, Stamina & Eitr - Manager";
        public const string PluginVersion = "0.1.0";

        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo($"{PluginName} v{PluginVersion} loaded.");
        }
    }
}