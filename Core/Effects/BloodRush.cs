using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;

namespace SkillTree.Core.Effects
{
    public class BloodRush : Effect
    {
        public static bool IsBloodRushActive { get; private set; } = false;

        public override void ApplyToNPC(NPC npc)
        {
            MelonLogger.Warning("Blood Rush has no effect on NPCs");
        }

        public override void ClearFromNPC(NPC npc)
        {
            MelonLogger.Warning("Blood Rush has no effect on NPCs");
        }

        public override void ApplyToPlayer(Player player)
        {
            IsBloodRushActive = true;
            MelonLogger.Msg($"Blood Rush effect applied");
        }

        public override void ClearFromPlayer(Player player)
        {
            IsBloodRushActive = false;
            MelonLogger.Msg($"Blood Rush effect removed");
        }
    }
}
