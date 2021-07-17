using System.Collections.Generic;
using System.Linq;
using Autopsy.Util;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Autopsy
{
    [HarmonyPatch(typeof(GenRecipe), nameof(GenRecipe.MakeRecipeProducts))]
    [HarmonyPatch(MethodType.Normal)]
    public static class MakeRecipeProductsPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref IEnumerable<Thing> __result, RecipeDef recipeDef, Pawn worker,
            List<Thing> ingredients)
        {
            if (Constants.HumanRecipeDefs.Contains(recipeDef))
            {
                RecipeInfo recipeSettings = null;
                float skillChance = worker.GetStatValue(StatDefOf.MedicalSurgerySuccessChance);
                if (recipeDef.Equals(AutopsyRecipeDefs.AutopsyBasic))
                {
                    recipeSettings = new RecipeInfo(
                        Mod.BasicAutopsyOrganMaxChance.Value,
                        Mod.BasicAutopsyCorpseAge.Value * 2500,
                        Mod.BasicAutopsyBionicMaxChance.Value,
                        Mod.BasicAutopsyMaxNumberOfOrgans.Value,
                        Mod.BasicAutopsyFrozenDecay.Value);
                    skillChance *= Mod.BasicAutopsyMedicalSkillScaling.Value;
                }
                else if (recipeDef.Equals(AutopsyRecipeDefs.AutopsyAdvanced))
                {
                    recipeSettings = new RecipeInfo(
                        Mod.AdvancedAutopsyOrganMaxChance.Value,
                        Mod.AdvancedAutopsyCorpseAge.Value * 2500,
                        Mod.AdvancedAutopsyBionicMaxChance.Value,
                        Mod.AdvancedAutopsyMaxNumberOfOrgans.Value,
                        Mod.AdvancedAutopsyFrozenDecay.Value);
                    skillChance *= Mod.AdvancedAutopsyMedicalSkillScaling.Value;
                }
                else if (recipeDef.Equals(AutopsyRecipeDefs.AutopsyGlitterworld))
                {
                    recipeSettings = new RecipeInfo(
                        Mod.GlitterAutopsyOrganMaxChance.Value,
                        Mod.GlitterAutopsyCorpseAge.Value * 2500,
                        Mod.GlitterAutopsyBionicMaxChance.Value,
                        Mod.GlitterAutopsyMaxNumberOfOrgans.Value,
                        Mod.GlitterAutopsyFrozenDecay.Value);
                    skillChance *= Mod.GlitterAutopsyMedicalSkillScaling.Value;
                }
                else if (recipeDef.Equals(AutopsyRecipeDefs.AutopsyAnimal))
                {
                    recipeSettings = new RecipeInfo(
                        0f,
                        0,
                        Mod.AnimalAutopsyBionicMaxChance.Value,
                        Mod.AnimalAutopsyMaxNumberOfOrgans.Value,
                        0);
                    skillChance *= Mod.AnimalAutopsyMedicalSkillScaling.Value;
                }

                if (recipeSettings == null) return;
                List<Thing> result = __result as List<Thing> ?? __result.ToList();
                foreach (Corpse corpse in ingredients.OfType<Corpse>())
                    result.AddRange(
                        NewMedicalRecipesUtility.TraverseBody(recipeSettings, corpse, skillChance));

                if (recipeDef.Equals(AutopsyRecipeDefs.AutopsyBasic))
                {
                    worker.needs?.mood?.thoughts?.memories?.TryGainMemory(AutopsyRecipeDefs.HarvestedHumanlikeCorpse, null);
                    foreach (Pawn pawn in worker.Map.mapPawns.SpawnedPawnsInFaction(worker.Faction))
                        if (pawn != worker)
                            pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(
                                AutopsyRecipeDefs.KnowHarvestedHumanlikeCorpse, null);
                }

                __result = result;
            }
        }
    }
}