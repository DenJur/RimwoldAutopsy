using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Autopsy.Filters
{
    public abstract class CorpseBionicFilter : SpecialThingFilterWorker
    {
        private readonly bool harvestable;

        protected CorpseBionicFilter(bool harvestable)
        {
            this.harvestable = harvestable;
        }

        public override bool Matches(Thing t)
        {
            return DoesMatch(t as Corpse);
        }

        public override bool AlwaysMatches(ThingDef def)
        {
            return false;
        }

        public override bool CanEverMatch(ThingDef def)
        {
            return def.IsWithinCategory(ThingCategoryDefOf.Corpses);
        }

        protected virtual bool DoesMatch(Corpse corpse)
        {
            if (corpse == null)
                return false;

            RaceProperties race = corpse.InnerPawn.RaceProps;
            return race.Humanlike && !race.Animal && HasBionic(corpse) == harvestable;
        }

        private bool HasBionic(Corpse corpse)
        {
            Pawn pawn = corpse.InnerPawn;
            BodyPartRecord core = pawn.RaceProps.body.corePart;
            List<BodyPartRecord> queue = new List<BodyPartRecord> {core};
            HediffSet hediffSet = pawn.health.hediffSet;
            while (queue.Count > 0)
            {
                BodyPartRecord part = queue.First();
                queue.Remove(part);
                if (CanGetPart(pawn, part))
                    return true;
                queue.AddRange(part.parts.Where(x => !hediffSet.PartIsMissing(x)));
            }

            return false;
        }

        public bool CanGetPart(Pawn pawn, BodyPartRecord part)
        {
            return pawn.health.hediffSet.hediffs.Any(x =>
                part.Equals(x.Part) && x.def.spawnThingOnRemoved != null &&
                (x is Hediff_Implant || x is Hediff_AddedPart));
        }
    }

    public class HasBionics : CorpseBionicFilter
    {
        public HasBionics() : base(true)
        {
        }
    }

    public class DoesntHaveBionics : CorpseBionicFilter
    {
        public DoesntHaveBionics() : base(false)
        {
        }
    }
}