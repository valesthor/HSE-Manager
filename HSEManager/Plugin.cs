using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace HSEManager
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGUID = "valesthor.hsemanager";
        public const string PluginName = "Health, Stamina & Eitr - Manager";
        public const string PluginVersion = "0.1.0";

        internal static ManualLogSource Log;

        // Local (client-side) config. Server authority will be added later.
        public static ConfigEntry<float> BaseHealth;
        public static ConfigEntry<float> BaseStamina;
        public static ConfigEntry<float> BaseEitr;
        public static ConfigEntry<float> HealthRegenPerSecond;
        public static ConfigEntry<float> StaminaRegenPerSecond;
        public static ConfigEntry<float> EitrRegenPerSecond;

        private Harmony _harmony;

        private void Awake()
        {
            Log = Logger;

            BaseHealth = Config.Bind(
                "Health",
                "BaseHealth",
                25f,
                "Base health value for the local player, before food bonuses are added. Vanilla default is 25."
            );

            BaseStamina = Config.Bind(
                "Stamina",
                "BaseStamina",
                75f,
                "Base stamina value for the local player, before food bonuses are added. Vanilla default is 75."
            );

            BaseEitr = Config.Bind(
                "Eitr",
                "BaseEitr",
                0f,
                "Base Eitr value for the local player, before food bonuses are added. Vanilla default is 0 (Eitr normally comes entirely from food)."
            );

            HealthRegenPerSecond = Config.Bind(
                "Regeneration",
                "HealthRegenPerSecond",
                0f,
                "Extra health regeneration per second, on top of vanilla regen. Works independently of food and Comfort. 0 = disabled (default)."
            );

            StaminaRegenPerSecond = Config.Bind(
                "Regeneration",
                "StaminaRegenPerSecond",
                0f,
                "Extra stamina regeneration per second, on top of vanilla regen. Works independently of food and Comfort. 0 = disabled (default)."
            );

            EitrRegenPerSecond = Config.Bind(
                "Regeneration",
                "EitrRegenPerSecond",
                0f,
                "Extra Eitr regeneration per second, on top of vanilla regen. Works independently of food and Comfort. 0 = disabled (default)."
            );

            _harmony = new Harmony(PluginGUID);
            _harmony.PatchAll();

            Log.LogInfo($"{PluginName} v{PluginVersion} loaded. BaseHealth = {BaseHealth.Value}, BaseStamina = {BaseStamina.Value}, BaseEitr = {BaseEitr.Value} HealthRegenPerSecond = {HealthRegenPerSecond.Value}, StaminaRegenPerSecond = {StaminaRegenPerSecond.Value}, EitrRegenPerSecond = {EitrRegenPerSecond.Value}");
        }
    }
}