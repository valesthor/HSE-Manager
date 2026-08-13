using HarmonyLib;
using System;

namespace HSEManager.Patches
{
    // Adds configurable, food/Comfort-independent regeneration on top of vanilla
    // regen, by piggybacking on the existing Player.UpdateStats tick instead of
    // creating a separate timer/loop (see project philosophy, section 22).
    //
    // NOTE: Player declares two private overloads named "UpdateStats" — a
    // parameterless one (frame-driven playtime/stat tracking, unrelated to
    // Health/Stamina/Eitr) and UpdateStats(float dt) (fixed-tick regen logic,
    // called from FixedUpdate). We must target the (float) overload explicitly
    // via argumentTypes, or Harmony throws AmbiguousMatchException and aborts
    // patching the whole assembly (confirmed via real build/log, 2026-08-13).
    //
    // Only applies to Player.m_localPlayer — the character this client actually
    // owns and controls. Remote Player instances that also run UpdateStats on
    // this client are intentionally skipped: this client should never apply
    // healing/stamina/eitr changes to a character it doesn't own.
    [HarmonyPatch(typeof(Player), "UpdateStats", new Type[] { typeof(float) })]
    public static class Player_UpdateStats_ExtraRegen_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance, float dt)
        {
            if (__instance != Player.m_localPlayer) return;

            float healthRegen = Plugin.HealthRegenPerSecond.Value;
            if (healthRegen > 0f)
            {
                __instance.Heal(healthRegen * dt, showText: false);
            }

            float staminaRegen = Plugin.StaminaRegenPerSecond.Value;
            if (staminaRegen > 0f)
            {
                __instance.AddStamina(staminaRegen * dt);
            }

            float eitrRegen = Plugin.EitrRegenPerSecond.Value;
            if (eitrRegen > 0f)
            {
                __instance.AddEitr(eitrRegen * dt);
            }
        }
    }
}