using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Autopsy.Filters
{
    public abstract class FilterCorpse : SpecialThingFilterWorker
    {
        private readonly bool harvestable;
        private readonly bool animal;

        protected FilterCorpse(bool harvestable, bool animal)
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

            RaceProperties race = corpse.InnerPawn.RaceProps;

            if (animal)
                return (race.Animal && !race.Humanlike) && (CanHarvest(corpse) == harvestable);

            return (race.Humanlike && !race.Animal) && (CanHarvest(corpse) == harvestable);
        }

        private bool CanHarvest(Corpse corpse)
        {
            int maxage = Mathf.Max(Mod.BasicAutopsyCorpseAge.Value, Mod.AdvancedAutopsyCorpseAge.Value,
                Mod.GlitterAutopsyCorpseAge.Value);
            float decay = Mathf.Min(Mod.BasicAutopsyFrozenDecay.Value, Mod.AdvancedAutopsyFrozenDecay.Value,
                Mod.GlitterAutopsyFrozenDecay.Value);
            CompRottable rot = corpse.TryGetComp<CompRottable>();
            bool notRotten = rot == null
                ? corpse.Age <= maxage * 2500
                : rot.RotProgress + ((corpse.Age - rot.RotProgress) * decay) <=
                  maxage * 2500;

            Pawn pawn = corpse.InnerPawn;
            BodyPartRecord core = pawn.RaceProps.body.corePart;
            List<BodyPartRecord> queue = new List<BodyPartRecord> {core};
            HediffSet hediffSet = pawn.health.hediffSet;
            while (queue.Count > 0)
            {
                BodyPartRecord part = queue.First();
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
                x.Part == part && x.def.spawnThingOnRemoved != null &&
                (x is Hediff_Implant || x is Hediff_AddedPart));
        }
    }

    public class PossibleToHarvest : FilterCorpse
    {
        public PossibleToHarvest() : base(true, false)
        {
        }
    }

    public class PossibleToHarvestAnimal : FilterCorpse
    {
        public PossibleToHarvestAnimal() : base(true, true)
        {
        }
    }

    public class ImpossibleToHarvest : FilterCorpse
    {
        public ImpossibleToHarvest() : base(false, false)
        {
        }
    }

    public class ImpossibleToHarvestAnimal : FilterCorpse
    {
        public ImpossibleToHarvestAnimal() : base(false, true)
        {
        }
    }
}