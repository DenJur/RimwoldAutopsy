using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Autopsy
{
    public static class NewMedicaRecipesUtility
    {
        public static bool IsCleanAndDroppable(Pawn pawn, BodyPartRecord part)
        {
            return !pawn.RaceProps.Animal && !pawn.RaceProps.IsMechanoid && part.def.spawnThingOnRemoved != null &&
                   !pawn.health.hediffSet.hediffs.Any(x => x.Part == part && !x.IsOld());
        }

        public static IEnumerable<Thing> TraverseBody(RecipeInfo recipeInfo, Corpse corpse, float skillChance)
        {
            BodyPartRecord core = corpse.InnerPawn.RaceProps.body.corePart;
            List<BodyPartRecord> queue = new List<BodyPartRecord> {core};
            HediffSet hediffSet = corpse.InnerPawn.health.hediffSet;
            List<Thing> results = new List<Thing>();
            while (queue.Count > 0)
            {
                BodyPartRecord part = queue.First();
                queue.Remove(part);
                //Drop parts and bionics that are higher onthe body tree.
                if (TryGetParts(corpse, recipeInfo, part, skillChance, ref results) && core != part)
                    continue;
                queue.AddRange(part.parts.Where(x => !hediffSet.PartIsMissing(x)));
            }

            if (results.Count > recipeInfo.PartNumber)
            {
                var random = new Random();
               return results.OrderBy(i=>random.Next()).Take(recipeInfo.PartNumber);
            }

            return results;
        }

        public static bool TryGetParts(Corpse corpse, RecipeInfo recipeInfo, BodyPartRecord part, float skillChance,
            ref List<Thing> result)
        {
            if (IsCleanAndDroppable(corpse.InnerPawn, part))
            {
                CompRottable rot = corpse.TryGetComp<CompRottable>();
                if ((rot == null
                        ? corpse.Age <= recipeInfo.CorpseValidAge
                        : rot.RotProgress+((corpse.Age-rot.RotProgress)*recipeInfo.FrozenDecay) <= recipeInfo.CorpseValidAge) &&
                    Rand.Chance(Math.Min(skillChance, recipeInfo.NaturalChance)))
                    result.Add(ThingMaker.MakeThing(part.def.spawnThingOnRemoved));
                return true;
            }

            if (corpse.InnerPawn.health.hediffSet.hediffs.Any(x =>
                x.Part == part && x.def.spawnThingOnRemoved != null &&
                (x is Hediff_Implant || x is Hediff_AddedPart)))
            {
                result.AddRange(from hediff in corpse.InnerPawn.health.hediffSet.hediffs
                    where hediff.Part == part && hediff.def.spawnThingOnRemoved != null &&
                          (hediff is Hediff_Implant || hediff is Hediff_AddedPart) &&
                          Rand.Chance(Math.Min(skillChance, recipeInfo.BionicChance))
                    select ThingMaker.MakeThing(hediff.def.spawnThingOnRemoved));
                return true;
            }

            return false;
        }
    }
}