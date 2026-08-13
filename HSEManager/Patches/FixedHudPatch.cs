using HarmonyLib;

namespace HSEManager.Patches
{
    // Fixed-width bars: vanilla scales each bar's pixel width directly from
    // its max value (confirmed via decompiled Hud: maxX / 25f * 32f, identical
    // formula for Health, Stamina, and Eitr). Rather than reimplementing that
    // scaling logic, we intercept the single low-level size setters and
    // override just the "size" argument to the width vanilla would have used
    // for a max value of exactly 100 (100 / 25f * 32f = 128f) — everything
    // else (actual values, fill percentage, colors) is untouched.
    //
    // SetHealthBarSize/SetStaminaBarSize/SetEitrBarSize are each declared only
    // once in Hud (confirmed via decompile) — no ambiguous-overload risk here.
    public static class FixedHudPatch
    {
        // Visual width vanilla uses for a max value of 100, on the 25-base scale.
        private const float ReferenceBarSize = 100f / 25f * 32f;

        [HarmonyPatch(typeof(Hud), "SetHealthBarSize")]
        public static class SetHealthBarSize_Patch
        {
            [HarmonyPrefix]
            public static void Prefix(ref float size)
            {
                if (Plugin.FixedHealthBar.Value)
                {
                    size = ReferenceBarSize;
                }
            }
        }

        [HarmonyPatch(typeof(Hud), "SetStaminaBarSize")]
        public static class SetStaminaBarSize_Patch
        {
            [HarmonyPrefix]
            public static void Prefix(ref float size)
            {
                if (Plugin.FixedStaminaBar.Value)
                {
                    size = ReferenceBarSize;
                }
            }
        }

        [HarmonyPatch(typeof(Hud), "SetEitrBarSize")]
        public static class SetEitrBarSize_Patch
        {
            [HarmonyPrefix]
            public static void Prefix(ref float size)
            {
                if (Plugin.FixedEitrBar.Value)
                {
                    size = ReferenceBarSize;
                }
            }
        }
    }

    // Always Show: forces the stamina/Eitr bar to stay visible, overriding
    // vanilla's auto-hide-when-full behavior. Runs as a Postfix on the same
    // per-frame Update tick vanilla already uses (no new loop), simply
    // overwriting the "Visible" bool vanilla just set, only when the option
    // is on. m_staminaAnimator / m_eitrAnimator are public fields on Hud, so
    // no reflection is needed to reach them.
    //
    // UpdateStamina(Player, float) and UpdateEitr(Player, float) are each
    // declared only once in Hud (confirmed via decompile) — no ambiguous-
    // overload risk here either.
    [HarmonyPatch(typeof(Hud), "UpdateStamina")]
    public static class Hud_UpdateStamina_AlwaysShow_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Hud __instance)
        {
            if (Plugin.AlwaysShowStaminaBar.Value)
            {
                __instance.m_staminaAnimator.SetBool("Visible", true);
            }
        }
    }

    [HarmonyPatch(typeof(Hud), "UpdateEitr")]
    public static class Hud_UpdateEitr_AlwaysShow_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Hud __instance)
        {
            if (Plugin.AlwaysShowEitrBar.Value)
            {
                __instance.m_eitrAnimator.SetBool("Visible", true);
            }
        }
    }
}