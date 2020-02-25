using System.Linq;
using Verse;

namespace Autopsy.Patches
{
    internal class AutopsyWorker : RecipeWorker
    {
        public override void ConsumeIngredient(Thing ingredient, RecipeDef recipeDef, Map map)
        {
            if (Constants.HumanRecipeDefs.Contains(recipeDef) && ingredient is Corpse)
                return;

            base.ConsumeIngredient(ingredient, recipeDef, map);
        }
    }
}