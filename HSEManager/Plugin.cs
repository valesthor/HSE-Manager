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
        public static ConfigEntry<bool> InfiniteStamina;
        public static ConfigEntry<bool> InfiniteEitr;
        public static ConfigEntry<bool> FixedHealthBar;
        public static ConfigEntry<bool> FixedStaminaBar;
        public static ConfigEntry<bool> FixedEitrBar;
        public static ConfigEntry<bool> AlwaysShowStaminaBar;
        public static ConfigEntry<bool> AlwaysShowEitrBar;

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

            InfiniteStamina = Config.Bind(
                "Stamina",
                "InfiniteStamina",
                false,
                "If enabled, stamina is never actually consumed. All vanilla actions and their calculations still run normally. Default: disabled."
            );

            InfiniteEitr = Config.Bind(
                "Eitr",
                "InfiniteEitr",
                false,
                "If enabled, Eitr is never actually consumed. All vanilla spells and their calculations still run normally. Default: disabled."
            );

            FixedHealthBar = Config.Bind(
                "HUD",
                "FixedHealthBar",
                false,
                "If enabled, the health bar keeps a fixed visual width (as if max health were 100), regardless of actual max health. Values and fill percentage are unaffected. Default: disabled."
            );

            FixedStaminaBar = Config.Bind(
                "HUD",
                "FixedStaminaBar",
                false,
                "If enabled, the stamina bar keeps a fixed visual width (as if max stamina were 100), regardless of actual max stamina. Values and fill percentage are unaffected. Default: disabled."
            );

            FixedEitrBar = Config.Bind(
                "HUD",
                "FixedEitrBar",
                false,
                "If enabled, the Eitr bar keeps a fixed visual width (as if max Eitr were 100), regardless of actual max Eitr. Values and fill percentage are unaffected. Default: disabled."
            );

            AlwaysShowStaminaBar = Config.Bind(
                "HUD",
                "AlwaysShowStaminaBar",
                false,
                "If enabled, the stamina bar stays visible at all times, even when full and unused. Has no effect if Infinite Stamina is on (bar already stays hidden in that case). Default: disabled."
            );

            AlwaysShowEitrBar = Config.Bind(
                "HUD",
                "AlwaysShowEitrBar",
                false,
                "If enabled, the Eitr bar stays visible at all times, even when full and unused. Has no effect if Infinite Eitr is on (bar already stays hidden in that case). Default: disabled."
            );

            _harmony = new Harmony(PluginGUID);
            _harmony.PatchAll();

            Log.LogInfo($"{PluginName} v{PluginVersion} loaded. BaseHealth = {BaseHealth.Value}, BaseStamina = {BaseStamina.Value}, BaseEitr = {BaseEitr.Value} HealthRegenPerSecond = {HealthRegenPerSecond.Value}, StaminaRegenPerSecond = {StaminaRegenPerSecond.Value}, EitrRegenPerSecond = {EitrRegenPerSecond.Value} InfiniteStamina = {InfiniteStamina.Value}, InfiniteEitr = {InfiniteEitr.Value} FixedHealthBar = {FixedHealthBar.Value}, FixedStaminaBar = {FixedStaminaBar.Value}, FixedEitrBar = {FixedEitrBar.Value} AlwaysShowStaminaBar = {AlwaysShowStaminaBar.Value}, AlwaysShowEitrBar = {AlwaysShowEitrBar.Value}");
        }
    }
}