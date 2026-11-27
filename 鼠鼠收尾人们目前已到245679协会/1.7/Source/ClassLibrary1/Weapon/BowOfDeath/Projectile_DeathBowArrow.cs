using RimWorld;
using UnityEngine;
using Verse;

namespace XIYUNTE
{
    // 死之弓箭矢:命中血肉单位后挂载"箭矢"Hediff,每个单位最多 4 层。
    // 效果挂在弹丸命中上,命中判定失败(打空或打在掩体上)不会触发。
    public class Projectile_DeathBowArrow : Bullet
    {
        private const int MaxArrowStacks = 4;

        private bool applyStuckArrow;

        public override void Launch(Thing launcher, Vector3 origin, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, bool preventFriendlyFire = false, Thing equipment = null, ThingDef targetCoverDef = null)
        {
            base.Launch(launcher, origin, usedTarget, intendedTarget, hitFlags, preventFriendlyFire, equipment, targetCoverDef);
            applyStuckArrow = equipment is Thing_DeathBow && launcher is Pawn pawn && pawn.CurJobDef != JobDefOf.Hunt;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref applyStuckArrow, "deathBowApplyStuckArrow");
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing, blockedByShield);
            Pawn pawn = hitThing as Pawn;
            if (!applyStuckArrow || pawn == null || pawn.Dead || pawn.health?.hediffSet == null || pawn.RaceProps == null || !pawn.RaceProps.IsFlesh)
            {
                return;
            }
            HediffDef arrowDef = DeathBowDefOf.BowOfDeath_StuckArrow;
            if (BowOfDeathArrowRecovery.CountStacks(pawn) >= MaxArrowStacks)
            {
                return;
            }
            pawn.health.AddHediff(arrowDef, pawn.RaceProps.body.corePart);
        }
    }
}
