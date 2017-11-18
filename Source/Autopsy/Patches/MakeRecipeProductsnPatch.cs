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
            if (Constants.RecipeDictionary.ContainsKey(recipeDef.defName))
            {
                List<Thing> result = __result as List<Thing> ?? __result.ToList();
                float skillChance = 1.5f;
                skillChance *= worker.GetStatValue(StatDefOf.MedicalSurgerySuccessChance);
                skillChance *= recipeDef.surgerySuccessChanceFactor;
                foreach (Corpse corpse in ingredients.OfType<Corpse>())
                    result.AddRange(
                        NewMedicaRecipesUtility.TraverseBody(Constants.RecipeDictionary.GetValueSafe(recipeDef.defName),
                            corpse, skillChance));
                __result = result;
            }
        }
    }
}