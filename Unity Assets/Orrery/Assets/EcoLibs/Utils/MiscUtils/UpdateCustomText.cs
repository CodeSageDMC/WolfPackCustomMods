// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.UI
{
    using Eco.Shared.Items;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary> Set a text based on a numeric value. Just as the <seealso cref="UpdateText"/> class it can be used to display the numeric value of a slider.
    /// When using this component it is possible to define a condition for that numeric value and when the condition is met instead of displaying the number
    /// it will display a Custom Text. The condition is defined on the Editor using Comparison Type and Threshold (see the example for more information).
    /// When a Slider is a assigned on the editor it will subscribe to the OnValueChanged, update the number and evaluate the condition based on the slider value.
    /// </summary>
    /// <example> Parameters: customText = "Unlimited"; threshold = 10; comparisonType = ComparisonType.GreaterThan; format = "{0:0.0}";
    /// - The method UpdateFromFloatCustomText(5f) is called the text will display 5.0
    /// - The method UpdateFromFloatCustomText(15f) is called the text will display "Unlimited"
    /// </example> 
    public class UpdateCustomText : UpdateText
    {
        [SerializeField] string customText = "";
        [SerializeField] float threshold = 1f;

        [Tooltip("Optional parameter. Use this only if the value to display is based on a slider"), SerializeField]
        Slider slider = null;

        [SerializeField, Tooltip("Select a condition to evaluate the threshold")]
        ComparisonType comparisonType = ComparisonType.GreaterThan;

        void Start()
        {
            if (this.slider != null)
            {
                this.slider.onValueChanged.AddListener(this.slider.wholeNumbers ? this.UpdateFromIntCustomText : this.UpdateFromFloatCustomText);
                if(this.UseCustomText(this.slider.value)) this.text.text = this.customText;
            }
        }

        void OnEnable()
        {
            //If this is linked to a slider the initial value may be the one that triggers a custom text and should be updated once it is enabled
            if(this.slider != null && this.UseCustomText(this.slider.value)) this.text.text = this.customText;
        }

        /// <summary> Updates text from a value and uses a custom Text when the condition is met </summary>
        public void UpdateFromIntCustomText(float value)
        {
            value = (int)(value * this.scalar);
            if (this.valueIsPercent) value = (int)(value * 100.0f);
            this.UpdateTextFromValue(value);
        }

        /// <summary> Updates text from a value and uses a custom Text when the condition is met </summary>
        public void UpdateFromFloatCustomText(float value)
        {
            value *= this.scalar;
            if (this.valueIsPercent) value = value * 100.0f;
            this.UpdateTextFromValue(value);
        }

        void UpdateTextFromValue(float value) 
        {
            if (this.UseCustomText(value)) this.text.text = this.customText;
            else this.text.text = string.Format(this.format, value);
        }

        bool UseCustomText(float value) 
        {
            bool useCustomText = false;
            switch (this.comparisonType)
            {
                case ComparisonType.GreaterThan          : useCustomText = value >  this.threshold; break;
                case ComparisonType.GreaterThanOrEqualTo : useCustomText = value >= this.threshold; break;
                case ComparisonType.LessThan             : useCustomText = value <  this.threshold; break;
                case ComparisonType.LessThanOrEqualTo    : useCustomText = value <= this.threshold; break;
                case ComparisonType.EqualTo              : useCustomText = value == this.threshold; break;
                case ComparisonType.NotEqualTo           : useCustomText = value != this.threshold; break;
            }
            return useCustomText;
        }
    }
}
