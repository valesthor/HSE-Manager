using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Utils;

namespace HSEManager
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGUID = "valesthor.hsemanager";
        public const string PluginName = "Health, Stamina & Eitr - Manager";
        public const string PluginVersion = "0.1.0";

        internal static ManualLogSource Log;

        // Gameplay configs (server-authoritative via Jotunn IsAdminOnly when connected
        // to a server; local value applies in single-player, since the local player
        // is always considered admin there).
        public static ConfigEntry<float> BaseHealth;
        public static ConfigEntry<float> BaseStamina;
        public static ConfigEntry<float> BaseEitr;
        public static ConfigEntry<float> HealthRegenPerSecond;
        public static ConfigEntry<float> StaminaRegenPerSecond;
        public static ConfigEntry<float> EitrRegenPerSecond;
        public static ConfigEntry<bool> InfiniteStamina;
        public static ConfigEntry<bool> InfiniteEitr;

        // HUD configs (always local/client-only, never synced from server).
        public static ConfigEntry<bool> FixedHealthBar;
        public static ConfigEntry<bool> FixedStaminaBar;
        public static ConfigEntry<bool> FixedEitrBar;
        public static ConfigEntry<bool> AlwaysShowStaminaBar;
        public static ConfigEntry<bool> AlwaysShowEitrBar;

        private Harmony _harmony;

        private void Awake()
        {
            Log = Logger;

            // Server-authoritative attribute, shared by every gameplay config below.
            ConfigurationManagerAttributes isAdminOnly = new ConfigurationManagerAttributes { IsAdminOnly = true };

            // ---------- 01. Health ----------
            BaseHealth = Config.Bind(
                "01. Health",
                "BaseHealth",
                25f,
                new ConfigDescription(
                    "Base health value for the local player, before food bonuses are added. Vanilla default is 25. Server-authoritative: locked to the server's value while connected to a multiplayer server.",
                    null,
                    isAdminOnly)
            );

            // ---------- 02. Stamina ----------
            BaseStamina = Config.Bind(
                "02. Stamina",
                "BaseStamina",
                75f,
                new ConfigDescription(
                    "Base stamina value for the local player, before food bonuses are added. Vanilla default is 75. Server-authoritative: locked to the server's value while connected to a multiplayer server.",
                    null,
                    isAdminOnly)
            );

            InfiniteStamina = Config.Bind(
                "02. Stamina",
                "InfiniteStamina",
                false,
                new ConfigDescription(
                    "If enabled, stamina is never actually consumed. All vanilla actions and their calculations still run normally. Default: disabled. Server-authoritative: locked to the server's value while connected to a multiplayer server.",
                    null,
                    isAdminOnly)
            );

            // ---------- 03. Eitr ----------
            BaseEitr = Config.Bind(
                "03. Eitr",
                "BaseEitr",
                0f,
                new ConfigDescription(
                    "Base Eitr value for the local player, before food bonuses are added. Vanilla default is 0 (Eitr normally comes entirely from food). Server-authoritative: locked to the server's value while connected to a multiplayer server.",
                    null,
                    isAdminOnly)
            );

            InfiniteEitr = Config.Bind(
                "03. Eitr",
                "InfiniteEitr",
                false,
                new ConfigDescription(
                    "If enabled, Eitr is never actually consumed. All vanilla spells and their calculations still run normally. Default: disabled. Server-authoritative: locked to the server's value while connected to a multiplayer server.",
                    null,
                    isAdminOnly)
            );

            // ---------- 04. Regeneration ----------
            HealthRegenPerSecond = Config.Bind(
                "04. Regeneration",
                "HealthRegenPerSecond",
                0f,
                new ConfigDescription(
                    "Extra health regeneration per second, on top of vanilla regen. Works independently of food and Comfort. 0 = disabled (default). Server-authoritative: locked to the server's value while connected to a multiplayer server.",
                    null,
                    isAdminOnly)
            );

            StaminaRegenPerSecond = Config.Bind(
                "04. Regeneration",
                "StaminaRegenPerSecond",
                0f,
                new ConfigDescription(
                    "Extra stamina regeneration per second, on top of vanilla regen. Works independently of food and Comfort. 0 = disabled (default). Server-authoritative: locked to the server's value while connected to a multiplayer server.",
                    null,
                    isAdminOnly)
            );

            EitrRegenPerSecond = Config.Bind(
                "04. Regeneration",
                "EitrRegenPerSecond",
                0f,
                new ConfigDescription(
                    "Extra Eitr regeneration per second, on top of vanilla regen. Works independently of food and Comfort. 0 = disabled (default). Server-authoritative: locked to the server's value while connected to a multiplayer server.",
                    null,
                    isAdminOnly)
            );

            // ---------- 05. HUD (always local — never synced) ----------
            FixedHealthBar = Config.Bind(
                "05. HUD",
                "FixedHealthBar",
                false,
                "If enabled, the health bar keeps a fixed visual width (as if max health were 100), regardless of actual max health. Values and fill percentage are unaffected. Default: disabled."
            );

            FixedStaminaBar = Config.Bind(
                "05. HUD",
                "FixedStaminaBar",
                false,
                "If enabled, the stamina bar keeps a fixed visual width (as if max stamina were 100), regardless of actual max stamina. Values and fill percentage are unaffected. Default: disabled."
            );

            AlwaysShowStaminaBar = Config.Bind(
                "05. HUD",
                "AlwaysShowStaminaBar",
                false,
                "If enabled, the stamina bar stays visible at all times, even when full and unused. Has no effect if Infinite Stamina is on (bar already stays hidden in that case). Default: disabled."
            );

            FixedEitrBar = Config.Bind(
                "05. HUD",
                "FixedEitrBar",
                false,
                "If enabled, the Eitr bar keeps a fixed visual width (as if max Eitr were 100), regardless of actual max Eitr. Values and fill percentage are unaffected. Default: disabled."
            );

            AlwaysShowEitrBar = Config.Bind(
                "05. HUD",
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