namespace Autopsy
{
    public class RecipeInfo
    {
        public readonly float BionicChance;

        public readonly int CorpseValidAge;

        public readonly float NaturalChance;

        public readonly int PartNumber;

        public readonly float FrozenDecay;

        public RecipeInfo(float naturalChance, int corpseValidAge, float bionicChance, int partNumber,
            float frozenDecay)
        {
            NaturalChance = naturalChance;
            CorpseValidAge = corpseValidAge;
            BionicChance = bionicChance;
            PartNumber = partNumber;
            FrozenDecay = frozenDecay;
        }
    }
}