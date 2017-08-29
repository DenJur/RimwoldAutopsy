using System.Collections.Generic;

namespace Autopsy
{
    internal class Constants
    {
        public const string AutopsyBasic = "AutopsyBasic";
        public const string AutopsyAdvanced = "AutopsyAdvanced";
        public const string AutopsyAnimal = "AutopsyAnimal";
        public const string AutopsyGlitterworld = "AutopsyGlitterworld";

        public static readonly Dictionary<string, RecipeInfo> RecipeDictionary = new Dictionary<string, RecipeInfo>
        {
            {AutopsyBasic, new RecipeInfo(0.4f, 7500, -1f)},
            {AutopsyAdvanced, new RecipeInfo(0.8f, 15000, 0.6f)},
            {AutopsyAnimal, new RecipeInfo(-1f, 15000, 0.6f)},
            {AutopsyGlitterworld, new RecipeInfo(0.95f, 30000, 0.8f)}
        };
    }
}