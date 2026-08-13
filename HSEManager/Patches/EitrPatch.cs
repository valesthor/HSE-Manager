using HarmonyLib;

namespace HSEManager.Patches
{
    // Unlike Health and Stamina, there is no public "m_baseEitr" field on Player —
    // vanilla Eitr always starts at 0 and comes entirely from food (see
    // Player.GetTotalFoodValue: "eitr = 0f;"). So instead of overwriting a field,
    // we patch GetTotalFoodValue directly and add our configured base value to
    // whatever the vanilla food calculation already produced.
    //
    // GetTotalFoodValue is private, so we target it by name via HarmonyPatch
    // with explicit parameter types (out float, out float, out float).
    [HarmonyPatch(typeof(Player), "GetTotalFoodValue")]
    public static class Player_GetTotalFoodValue_BaseEitr_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref float eitr)
        {
            eitr += Plugin.BaseEitr.Value;
        }
    }
}