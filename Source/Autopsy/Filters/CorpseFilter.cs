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
        private readonly bool animalOrHuman;

        /**
         * true - animal, false - human
         */
        protected FilterCorpse(bool harvestable, bool animalOrHuman)
        {
            this.harvestable = harvestable;
            this.animalOrHuman = animalOrHuman;
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

            if (animalOrHuman)
                return race.Animal && CanHarvest(corpse) == harvestable;

            return race.Humanlike && CanHarvest(corpse) == harvestable;
        }

        private bool CanHarvest(Corpse corpse)
        {
            if (!animalOrHuman)
            {
                IEnumerable<BodyPartRecord> parts = corpse.InnerPawn.health.hediffSet.GetNotMissingParts();
                int maxage = Mathf.Max(Mod.BasicAutopsyCorpseAge.Value, Mod.AdvancedAutopsyCorpseAge.Value,
                    Mod.GlitterAutopsyCorpseAge.Value);
                float decay = Mathf.Min(Mod.BasicAutopsyFrozenDecay.Value, Mod.AdvancedAutopsyFrozenDecay.Value,
                    Mod.GlitterAutopsyFrozenDecay.Value);
                CompRottable rot = corpse.TryGetComp<CompRottable>();

                if (rot == null
                    ? corpse.Age <= maxage * 2500
                    : rot.RotProgress + ((corpse.Age - rot.RotProgress) * decay) <=
                      maxage * 2500)
                    if (parts.Any(part => NewMedicaRecipesUtility.IsCleanAndDroppable(corpse.InnerPawn, part)))
                        return true;
            }

            if (corpse.InnerPawn.health.hediffSet.hediffs.Any(x =>
                x.def.spawnThingOnRemoved != null &&
                (x is Hediff_Implant || x is Hediff_AddedPart)))
                return true;

            return false;
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