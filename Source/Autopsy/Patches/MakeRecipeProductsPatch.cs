using System.Collections.Generic;
using System.Linq;
using Harmony;
using RimWorld;
using Verse;

namespace Autopsy
{
    [HarmonyPatch(typeof(GenRecipe), nameof(GenRecipe.MakeRecipeProducts))]
    public static class MakeRecipeProductsnPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref IEnumerable<Thing> __result, RecipeDef recipeDef, Pawn worker,
            List<Thing> ingredients)
        {
            RecipeInfo recipeSettings = null;
            float skillChance = worker.GetStatValue(StatDefOf.MedicalSurgerySuccessChance);
            switch (recipeDef.defName)
            {
                case Constants.AutopsyBasic:
                    recipeSettings = new RecipeInfo(
                        Mod.BasicAutopsyOrganMaxChance.Value,
                        Mod.BasicAutopsyCorpseAge.Value * 2500,
                        Mod.BasicAutopsyBionicMaxChance.Value,
                        Mod.BasicAutopsyMaxNumberOfOrgans.Value, 
                        Mod.BasicAutopsyFrozenDecay.Value);
                    skillChance *= Mod.BasicAutopsyMedicalSkillScaling.Value;
                    break;
                case Constants.AutopsyAdvanced:
                    recipeSettings = new RecipeInfo(
                        Mod.AdvancedAutopsyOrganMaxChance.Value,
                        Mod.AdvancedAutopsyCorpseAge.Value * 2500,
                        Mod.AdvancedAutopsyBionicMaxChance.Value,
                        Mod.AdvancedAutopsyMaxNumberOfOrgans.Value, 
                        Mod.AdvancedAutopsyFrozenDecay.Value);
                    skillChance *= Mod.AdvancedAutopsyMedicalSkillScaling.Value;
                    break;
                case Constants.AutopsyGlitterworld:
                    recipeSettings = new RecipeInfo(
                        Mod.GlitterAutopsyOrganMaxChance.Value,
                        Mod.GlitterAutopsyCorpseAge.Value * 2500,
                        Mod.GlitterAutopsyBionicMaxChance.Value,
                        Mod.GlitterAutopsyMaxNumberOfOrgans.Value, 
                        Mod.GlitterAutopsyFrozenDecay.Value);
                    skillChance *= Mod.GlitterAutopsyMedicalSkillScaling.Value;
                    break;
                case Constants.AutopsyAnimal:
                    recipeSettings = new RecipeInfo(
                        0f,
                        0,
                        Mod.AnimalAutopsyBionicMaxChance.Value,
                        Mod.AnimalAutopsyMaxNumberOfOrgans.Value, 
                        0);
                    skillChance *= Mod.AnimalAutopsyMedicalSkillScaling.Value;
                    break;
            }

            if (recipeSettings == null) return;
            List<Thing> result = __result as List<Thing> ?? __result.ToList();
            foreach (Corpse corpse in ingredients.OfType<Corpse>())
                result.AddRange(
                    NewMedicaRecipesUtility.TraverseBody(recipeSettings, corpse, skillChance));
            __result = result;
        }
    }
}