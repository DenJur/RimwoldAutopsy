using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Autopsy.Filters
{
    public abstract class CorpseFilter : SpecialThingFilterWorker
    {
        private readonly bool animal;
        private readonly bool harvestable;

        protected CorpseFilter(bool harvestable, bool animal)
        {
            this.harvestable = harvestable;
            this.animal = animal;
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

            var race = corpse.InnerPawn.RaceProps;

            if (animal)
                return race.Animal && !race.Humanlike && CanHarvest(corpse) == harvestable;

            return race.Humanlike && !race.Animal && CanHarvest(corpse) == harvestable;
        }

        private bool CanHarvest(Corpse corpse)
        {
            var maxage = Mathf.Max(Mod.BasicAutopsyCorpseAge.Value, Mod.AdvancedAutopsyCorpseAge.Value,
                Mod.GlitterAutopsyCorpseAge.Value);
            var decay = Mathf.Min(Mod.BasicAutopsyFrozenDecay.Value, Mod.AdvancedAutopsyFrozenDecay.Value,
                Mod.GlitterAutopsyFrozenDecay.Value);
            var rot = corpse.TryGetComp<CompRottable>();
            var notRotten = rot == null
                ? corpse.Age <= maxage * 2500
                : rot.RotProgress + (corpse.Age - rot.RotProgress) * decay <=
                  maxage * 2500;

            var pawn = corpse.InnerPawn;
            var core = pawn.RaceProps.body.corePart;
            var queue = new List<BodyPartRecord> {core};
            var hediffSet = pawn.health.hediffSet;
            while (queue.Count > 0)
            {
                var part = queue.First();
                queue.Remove(part);
                if (CanGetPart(pawn, part, notRotten) && core != part)
                    return true;
                queue.AddRange(part.parts.Where(x => !hediffSet.PartIsMissing(x)));
            }

            return false;
        }

        public bool CanGetPart(Pawn pawn, BodyPartRecord part, bool notRotten)
        {
            if (!animal && notRotten)
                if (NewMedicaRecipesUtility.IsCleanAndDroppable(pawn, part))
                    return true;

            return pawn.health.hediffSet.hediffs.Any(x =>
                part.Equals(x.Part) && x.def.spawnThingOnRemoved != null &&
                (x is Hediff_Implant || x is Hediff_AddedPart));
        }
    }

    public class PossibleToHarvest : CorpseFilter
    {
        public PossibleToHarvest() : base(true, false)
        {
        }
    }

    public class PossibleToHarvestAnimal : CorpseFilter
    {
        public PossibleToHarvestAnimal() : base(true, true)
        {
        }
    }

    public class ImpossibleToHarvest : CorpseFilter
    {
        public ImpossibleToHarvest() : base(false, false)
        {
        }
    }

    public class ImpossibleToHarvestAnimal : CorpseFilter
    {
        public ImpossibleToHarvestAnimal() : base(false, true)
        {
        }
    }
}