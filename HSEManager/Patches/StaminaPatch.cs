using HarmonyLib;

namespace HSEManager.Patches
{
    // Overrides the player's base stamina (before food bonuses), same pattern
    // as Player_Awake_BaseHealth_Patch. m_baseStamina is a public field the
    // vanilla food calculation already reads from every ~1s (see
    // Player.GetTotalFoodValue), so overwriting it here is enough.
    //
    // KNOWN LIMITATION (temporary, step 3 of the roadmap): applies to every
    // Player instance that Awakes on this client, including remote players in
    // multiplayer. Server-authoritative restriction comes later (roadmap step 10).
    [HarmonyPatch(typeof(Player), "Awake")]
    public static class Player_Awake_BaseStamina_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            __instance.m_baseStamina = Plugin.BaseStamina.Value;

            Plugin.Log.LogInfo(
                $"[HSE] Applied BaseStamina={Plugin.BaseStamina.Value} to Player instance."
            );
        }
    }
}