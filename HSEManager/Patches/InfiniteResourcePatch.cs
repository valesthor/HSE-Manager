using HarmonyLib;

namespace HSEManager.Patches
{
    // Infinite Stamina/Eitr: instead of touching every vanilla action that
    // consumes these resources (run, jump, dodge, attacks, spells, etc.), we
    // intercept the single low-level RPC methods that actually subtract the
    // resource, and skip the vanilla method entirely (return false) when the
    // corresponding infinite option is on.
    //
    // This means every vanilla action still runs its full normal logic (skill
    // checks, cooldowns, animations, XP gain, HaveStamina()/HaveEitr() checks,
    // etc.) exactly as before — only the actual subtraction never happens.
    //
    // Side effect confirmed acceptable: RPC_UseStamina/RPC_UseEitr also reset
    // the regen delay timer on every normal call. Skipping the method means
    // that reset never happens either, which in practice just means the bar
    // never even enters its "waiting to regen" state while infinite is on —
    // no negative effect, no extra code needed to compensate.
    //
    // Only intercepts on this client's own Player (m_localPlayer) — a remote
    // player's resource consumption is not something this client should ever
    // alter.
    [HarmonyPatch(typeof(Player), "RPC_UseStamina")]
    public static class Player_RPC_UseStamina_Infinite_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Player __instance)
        {
            if (__instance != Player.m_localPlayer) return true;
            return !Plugin.InfiniteStamina.Value;
        }
    }

    [HarmonyPatch(typeof(Player), "RPC_UseEitr")]
    public static class Player_RPC_UseEitr_Infinite_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Player __instance)
        {
            if (__instance != Player.m_localPlayer) return true;
            return !Plugin.InfiniteEitr.Value;
        }
    }
}