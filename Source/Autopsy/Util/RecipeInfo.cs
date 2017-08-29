namespace Autopsy
{
    public class RecipeInfo
    {
        public readonly float BionicChance;

        public readonly int CorpseValidAge;

        public readonly float NaturalChance;

        public RecipeInfo(float naturalChance, int corpseValidAge, float bionicChance)
        {
            NaturalChance = naturalChance;
            CorpseValidAge = corpseValidAge;
            BionicChance = bionicChance;
        }
    }
}