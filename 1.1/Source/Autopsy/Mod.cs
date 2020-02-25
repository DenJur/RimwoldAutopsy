using System.Collections.Generic;
using Autopsy.Util;
using HugsLib;
using HugsLib.Settings;
using Verse;

namespace Autopsy
{
    public class Mod : ModBase
    {
        internal static SettingHandle<int> BasicAutopsyMaxNumberOfOrgans;
        internal static SettingHandle<float> BasicAutopsyOrganMaxChance;
        internal static SettingHandle<int> BasicAutopsyCorpseAge;
        internal static SettingHandle<float> BasicAutopsyBionicMaxChance;
        internal static SettingHandle<float> BasicAutopsyMedicalSkillScaling;
        internal static SettingHandle<float> BasicAutopsyFrozenDecay;

        internal static SettingHandle<int> AdvancedAutopsyMaxNumberOfOrgans;
        internal static SettingHandle<float> AdvancedAutopsyOrganMaxChance;
        internal static SettingHandle<int> AdvancedAutopsyCorpseAge;
        internal static SettingHandle<float> AdvancedAutopsyBionicMaxChance;
        internal static SettingHandle<float> AdvancedAutopsyMedicalSkillScaling;
        internal static SettingHandle<float> AdvancedAutopsyFrozenDecay;

        internal static SettingHandle<int> GlitterAutopsyMaxNumberOfOrgans;
        internal static SettingHandle<float> GlitterAutopsyOrganMaxChance;
        internal static SettingHandle<int> GlitterAutopsyCorpseAge;
        internal static SettingHandle<float> GlitterAutopsyBionicMaxChance;
        internal static SettingHandle<float> GlitterAutopsyMedicalSkillScaling;
        internal static SettingHandle<float> GlitterAutopsyFrozenDecay;

        internal static SettingHandle<int> AnimalAutopsyMaxNumberOfOrgans;
        internal static SettingHandle<float> AnimalAutopsyBionicMaxChance;
        internal static SettingHandle<float> AnimalAutopsyMedicalSkillScaling;

        private static SettingHandle<string> _tabsHandler;

        private readonly SettingHandle.ValueIsValid _positiveInt = value =>
        {
            int i;
            if (int.TryParse(value, out i)) return i >= 0;

            return false;
        };

        private readonly List<string> _tabNames = new List<string>
        {
            "autopsyBasicTab".Translate(),
            "autopsyAdvancedTab".Translate(),
            "autopsyGlitterTab".Translate()
        };

        public override string ModIdentifier => "Autopsy";

        public override void DefsLoaded()
        {
            _tabsHandler = Settings.GetHandle<string>("tabs", null, null);

            //Basic
            BasicAutopsyOrganMaxChance =
                Settings.GetHandle("basicAutopsyOrganChance", "organChanceTitle".Translate(),
                    "organChanceDescription".Translate(), 0.4f);
            BasicAutopsyOrganMaxChance.CustomDrawer =
                rect => SettingUIUtil.CustomDrawer_Filter(rect, BasicAutopsyOrganMaxChance, 0, 1.0f);
            BasicAutopsyOrganMaxChance.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[0];
            BasicAutopsyBionicMaxChance =
                Settings.GetHandle("basicAutopsyBionicChance", "bionicChanceTitle".Translate(),
                    "bionicChanceDescription".Translate(), 0f);
            BasicAutopsyBionicMaxChance.CustomDrawer =
                rect => SettingUIUtil.CustomDrawer_Filter(rect, BasicAutopsyBionicMaxChance, 0, 1.0f);
            BasicAutopsyBionicMaxChance.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[0];
            BasicAutopsyCorpseAge =
                Settings.GetHandle("basicAutopsyCorpseAge", "corpseAgeTitle".Translate(),
                    "corpseAgeDescription".Translate(), 3);
            BasicAutopsyCorpseAge.Validator = _positiveInt;
            BasicAutopsyCorpseAge.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[0];
            BasicAutopsyMaxNumberOfOrgans =
                Settings.GetHandle("basicAutopsyNumberParts", "numberPartsTitle".Translate(),
                    "numberPartsDescription".Translate(), 99);
            BasicAutopsyMaxNumberOfOrgans.Validator = _positiveInt;
            BasicAutopsyMaxNumberOfOrgans.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[0];
            BasicAutopsyMedicalSkillScaling =
                Settings.GetHandle("basicAutopsySkill", "skillScalingTitle".Translate(),
                    "skillScalingDescription".Translate(), 1f);
            BasicAutopsyMedicalSkillScaling.CustomDrawer =
                rect => SettingUIUtil.CustomDrawer_Filter(rect, BasicAutopsyMedicalSkillScaling, 0, 10.0f);
            BasicAutopsyMedicalSkillScaling.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[0];
            BasicAutopsyFrozenDecay =
                Settings.GetHandle("basicAutopsyDecay", "frozenDecayTitle".Translate(),
                    "frozenDecayDescription".Translate(), 0f);
            BasicAutopsyFrozenDecay.CustomDrawer =
                rect => SettingUIUtil.CustomDrawer_Filter(rect, BasicAutopsyFrozenDecay, 0, 1.0f);
            BasicAutopsyFrozenDecay.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[0];

            //Advanced
            AdvancedAutopsyOrganMaxChance =
                Settings.GetHandle("advancedAutopsyOrganChance", "organChanceTitle".Translate(),
                    "organChanceDescription".Translate(), 0.8f);
            AdvancedAutopsyOrganMaxChance.CustomDrawer =
                rect => SettingUIUtil.CustomDrawer_Filter(rect, AdvancedAutopsyOrganMaxChance, 0, 1.0f);
            AdvancedAutopsyOrganMaxChance.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[1];
            AdvancedAutopsyBionicMaxChance =
                Settings.GetHandle("advancedAutopsyBionicChance", "bionicChanceTitle".Translate(),
                    "bionicChanceDescription".Translate(), 0.4f);
            AdvancedAutopsyBionicMaxChance.CustomDrawer =
                rect => SettingUIUtil.CustomDrawer_Filter(rect, AdvancedAutopsyBionicMaxChance, 0, 1.0f);
            AdvancedAutopsyBionicMaxChance.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[1];
            AdvancedAutopsyCorpseAge =
                Settings.GetHandle("advancedAutopsyCorpseAge", "corpseAgeTitle".Translate(),
                    "corpseAgeDescription".Translate(), 6);
            AdvancedAutopsyCorpseAge.Validator = _positiveInt;
            AdvancedAutopsyCorpseAge.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[1];
            AdvancedAutopsyMaxNumberOfOrgans =
                Settings.GetHandle("advancedAutopsyNumberParts", "numberPartsTitle".Translate(),
                    "numberPartsDescription".Translate(), 99);
            AdvancedAutopsyMaxNumberOfOrgans.Validator = _positiveInt;
            AdvancedAutopsyMaxNumberOfOrgans.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[1];
            AdvancedAutopsyMedicalSkillScaling =
                Settings.GetHandle("advancedAutopsySkill", "skillScalingTitle".Translate(),
                    "skillScalingDescription".Translate(), 1f);
            AdvancedAutopsyMedicalSkillScaling.CustomDrawer =
                rect => SettingUIUtil.CustomDrawer_Filter(rect, AdvancedAutopsyMedicalSkillScaling, 0, 10.0f);
            AdvancedAutopsyMedicalSkillScaling.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[1];
            AdvancedAutopsyFrozenDecay =
                Settings.GetHandle("advancedAutopsyDecay", "frozenDecayTitle".Translate(),
                    "frozenDecayDescription".Translate(), 0f);
            AdvancedAutopsyFrozenDecay.CustomDrawer =
                rect => SettingUIUtil.CustomDrawer_Filter(rect, AdvancedAutopsyFrozenDecay, 0, 1.0f);
            AdvancedAutopsyFrozenDecay.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[1];

            //Glitter
            GlitterAutopsyOrganMaxChance =
                Settings.GetHandle("glitterAutopsyOrganChance", "organChanceTitle".Translate(),
                    "organChanceDescription".Translate(), 0.95f);
            GlitterAutopsyOrganMaxChance.CustomDrawer =
                rect => SettingUIUtil.CustomDrawer_Filter(rect, GlitterAutopsyOrganMaxChance, 0, 1.0f);
            GlitterAutopsyOrganMaxChance.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[2];
            GlitterAutopsyBionicMaxChance =
                Settings.GetHandle("glitterAutopsyBionicChance", "bionicChanceTitle".Translate(),
                    "bionicChanceDescription".Translate(), 0.8f);
            GlitterAutopsyBionicMaxChance.CustomDrawer =
                rect => SettingUIUtil.CustomDrawer_Filter(rect, GlitterAutopsyBionicMaxChance, 0, 1.0f);
            GlitterAutopsyBionicMaxChance.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[2];
            GlitterAutopsyCorpseAge =
                Settings.GetHandle("glitterAutopsyCorpseAge", "corpseAgeTitle".Translate(),
                    "corpseAgeDescription".Translate(), 12);
            GlitterAutopsyCorpseAge.Validator = _positiveInt;
            GlitterAutopsyCorpseAge.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[2];
            GlitterAutopsyMaxNumberOfOrgans =
                Settings.GetHandle("glitterAutopsyNumberParts", "numberPartsTitle".Translate(),
                    "numberPartsDescription".Translate(), 99);
            GlitterAutopsyMaxNumberOfOrgans.Validator = _positiveInt;
            GlitterAutopsyMaxNumberOfOrgans.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[2];
            GlitterAutopsyMedicalSkillScaling =
                Settings.GetHandle("glitterAutopsySkill", "skillScalingTitle".Translate(),
                    "skillScalingDescription".Translate(), 1f);
            GlitterAutopsyMedicalSkillScaling.CustomDrawer =
                rect => SettingUIUtil.CustomDrawer_Filter(rect, GlitterAutopsyMedicalSkillScaling, 0, 10.0f);
            GlitterAutopsyMedicalSkillScaling.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[2];
            GlitterAutopsyFrozenDecay =
                Settings.GetHandle("glitterAutopsyDecay", "frozenDecayTitle".Translate(),
                    "frozenDecayDescription".Translate(), 0f);
            GlitterAutopsyFrozenDecay.CustomDrawer =
                rect => SettingUIUtil.CustomDrawer_Filter(rect, GlitterAutopsyFrozenDecay, 0, 1.0f);
            GlitterAutopsyFrozenDecay.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[2];


            _tabNames.Add("autopsyAnimalTab".Translate());
            AnimalAutopsyBionicMaxChance =
                Settings.GetHandle("animalAutopsyBionicChance", "bionicChanceTitle".Translate(),
                    "bionicChanceDescription".Translate(), 0.6f);
            AnimalAutopsyBionicMaxChance.CustomDrawer =
                rect => SettingUIUtil.CustomDrawer_Filter(rect, AnimalAutopsyBionicMaxChance, 0, 1.0f);
            AnimalAutopsyBionicMaxChance.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[3];
            AnimalAutopsyMaxNumberOfOrgans =
                Settings.GetHandle("animalAutopsyNumberParts", "numberPartsTitle".Translate(),
                    "numberPartsDescription".Translate(), 99);
            AnimalAutopsyMaxNumberOfOrgans.Validator = _positiveInt;
            AnimalAutopsyMaxNumberOfOrgans.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[3];
            AnimalAutopsyMedicalSkillScaling =
                Settings.GetHandle("animalAutopsySkill", "skillScalingTitle".Translate(),
                    "skillScalingDescription".Translate(), 0.8f);
            AnimalAutopsyMedicalSkillScaling.CustomDrawer =
                rect => SettingUIUtil.CustomDrawer_Filter(rect, AnimalAutopsyMedicalSkillScaling, 0, 10.0f);
            AnimalAutopsyMedicalSkillScaling.VisibilityPredicate = () => _tabsHandler.Value == _tabNames[3];

            _tabsHandler.CustomDrawer =
                rect => SettingUIUtil.CustomDrawer_Tabs(rect, _tabsHandler, _tabNames.ToArray());
        }
    }
}