using HarmonyLib;

namespace HSEManager.Patches
{
    // Overrides the player's base health (before food bonuses) right after the
    // vanilla Awake has finished setting up the Player object.
    //
    // We intentionally do NOT touch GetTotalFoodValue, SetMaxHealth, or the food
    // system itself — m_baseHP is a public field the vanilla food calculation
    // already reads from every ~1s (see Player.GetTotalFoodValue), so overwriting
    // it here is enough for the change to take effect on its own.
    //
    // KNOWN LIMITATION (temporary, step 2 of the roadmap): this currently applies
    // to every Player instance that Awakes on this client, including remote
    // players in multiplayer. Server-authoritative restriction comes later
    // (roadmap step 10). For now, test in single-player only.
    [HarmonyPatch(typeof(Player), "Awake")]
    public static class Player_Awake_BaseHealth_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            __instance.m_baseHP = Plugin.BaseHealth.Value;

            Plugin.Log.LogInfo(
                $"[HSE] Applied BaseHealth={Plugin.BaseHealth.Value} to Player instance."
            );
        }
    }
}