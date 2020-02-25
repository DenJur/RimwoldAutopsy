using System;
using HugsLib.Settings;
using UnityEngine;
using Verse;

/**
 * Based on code by rheirman at: https://github.com/rheirman/RunAndGun
 */
namespace Autopsy.Util
{
    internal class SettingUIUtil
    {
        private static readonly Color SelectedOptionColor = new Color(0.5f, 1f, 0.5f, 1f);

        public static bool CustomDrawer_Tabs(Rect rect, SettingHandle<string> selected, string[] defaultValues)
        {
            int labelWidth = 140;
            int offset = -297;
            bool change = false;

            foreach (string tab in defaultValues)
            {
                Rect buttonRect = new Rect(rect)
                {
                    width = labelWidth
                };
                buttonRect.position = new Vector2(buttonRect.position.x + offset, buttonRect.position.y);
                Color activeColor = GUI.color;
                bool isSelected = tab == selected.Value;
                if (isSelected)
                    GUI.color = SelectedOptionColor;
                bool clicked = Widgets.ButtonText(buttonRect, tab);
                if (isSelected)
                    GUI.color = activeColor;

                if (clicked)
                {
                    selected.Value = selected.Value != tab ? tab : "none";
                    change = true;
                }

                offset += labelWidth;
            }

            return change;
        }

        public static bool CustomDrawer_Filter(Rect rect, SettingHandle<float> slider, float defMin, float defMax)
        {
            int labelWidth = 50;

            Rect sliderPortion = new Rect(rect);
            sliderPortion.width = sliderPortion.width - labelWidth;

            Rect labelPortion = new Rect(rect)
            {
                width = labelWidth,
                position = new Vector2(sliderPortion.position.x + sliderPortion.width + 5f,
                    sliderPortion.position.y + 4f)
            };

            sliderPortion = sliderPortion.ContractedBy(2f);

            Widgets.Label(labelPortion, Mathf.Round(slider.Value * 100f).ToString("F0") + "%");

            float val = Widgets.HorizontalSlider(sliderPortion, slider.Value, defMin, defMax, true);
            bool change = Math.Abs(slider.Value - val) > float.Epsilon;

            slider.Value = val;
            return change;
        }
    }
}