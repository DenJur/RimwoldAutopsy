using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Random = System.Random;

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
            var core = corpse.InnerPawn.RaceProps.body.corePart;
            var queue = new List<BodyPartRecord> {core};
            var hediffSet = corpse.InnerPawn.health.hediffSet;
            var results = new List<Thing>();
            var damagedParts = new List<BodyPartRecord>();
            while (queue.Count > 0)
            {
                var part = queue.First();
                queue.Remove(part);
                //Drop parts and bionics that are higher on the body tree.
                if (TryGetParts(corpse, recipeInfo, part, skillChance, ref results, ref damagedParts) && core != part)
                    continue;
                queue.AddRange(part.parts.Where(x => !hediffSet.PartIsMissing(x)));
            }

            if (results.Count > recipeInfo.PartNumber)
            {
                var random = new Random();
                return results.OrderBy(i => random.Next()).Take(recipeInfo.PartNumber);
            }

            foreach (var part in damagedParts) DamageHarvested(corpse.InnerPawn, part);

            return results;
        }

        public static bool TryGetParts(Corpse corpse, RecipeInfo recipeInfo, BodyPartRecord part, float skillChance,
            ref List<Thing> result, ref List<BodyPartRecord> damagedParts)
        {
            if (IsCleanAndDroppable(corpse.InnerPawn, part))
            {
                damagedParts.Add(part);
                var rot = corpse.TryGetComp<CompRottable>();
                if (rot == null
                    ? corpse.Age <= recipeInfo.CorpseValidAge
                    : rot.RotProgress + (corpse.Age - rot.RotProgress) * recipeInfo.FrozenDecay <=
                      recipeInfo.CorpseValidAge)
                {
                    if (Rand.Chance(Math.Min(skillChance, recipeInfo.NaturalChance)))
                        result.Add(ThingMaker.MakeThing(part.def.spawnThingOnRemoved));
                    return true;
                }
            }

            List<Hediff> bion = corpse.InnerPawn.health.hediffSet.hediffs.Where(x =>
                part.Equals(x.Part) && x.def.spawnThingOnRemoved != null &&
                (x is Hediff_Implant || x is Hediff_AddedPart)).ToList();

            if (bion.Count > 0)
            {
                result.AddRange(bion.Where(x => Rand.Chance(Math.Min(skillChance, recipeInfo.BionicChance)))
                    .Select(x => ThingMaker.MakeThing(x.def.spawnThingOnRemoved)));
                damagedParts.Add(part);
                return true;
            }

            return false;
        }

        public static void DamageHarvested(Pawn p, BodyPartRecord part)
        {
            if (part == null) return;
            var targets = p.health.hediffSet.GetNotMissingParts();

            targets = targets.Where(pa => part.parent == pa);

            var bodyPartRecords = targets.ToList();

            var partHealth = p.health.hediffSet.GetPartHealth(part);
            if (partHealth >= float.Epsilon) DamagePart(p, Mathf.CeilToInt(partHealth), part);

            var start = DateTime.Now;

            var totalSharedDamage = Rand.Range(5, 10);

            while (totalSharedDamage > 0 && bodyPartRecords.Count != 0)
            {
                if ((DateTime.Now - start).TotalSeconds > 1)
                    return;

                BodyPartRecord bodyPartRecord;

                if (!bodyPartRecords.TryRandomElementByWeight(x => x.coverageAbs, out bodyPartRecord)) return;

                partHealth = p.health.hediffSet.GetPartHealth(bodyPartRecord);

                if (partHealth < float.Epsilon)
                {
                    bodyPartRecords.Remove(bodyPartRecord);
                    continue;
                }

                var num = Mathf.Max(3, GenMath.RoundRandom(partHealth * Rand.Range(0.5f, 1f)));

                DamagePart(p, num, bodyPartRecord);

                totalSharedDamage -= num;
            }
        }

        public static void DamagePart(Pawn p, int damage, BodyPartRecord part)
        {
            var def = Rand.Element(DamageDefOf.Cut, DamageDefOf.Stab);

            var hediffDefFromDamage = HealthUtility.GetHediffDefFromDamage(def, p, part);

            var injury = (Hediff_Injury) HediffMaker.MakeHediff(hediffDefFromDamage, p, part);
            injury.Severity = damage;

            p.health.AddHediff(injury, part, new DamageInfo(def, damage, -1f, null, part));
            GenLeaving.DropFilthDueToDamage(p, damage);
        }
    }
}