using System;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.Options.UI
{
	// Token: 0x0200025D RID: 605
	public class OptionsUIEntry_Slider : MonoBehaviour
	{
		// Token: 0x17000369 RID: 873
		// (get) Token: 0x060012CA RID: 4810 RVA: 0x0004699A File Offset: 0x00044B9A
		// (set) Token: 0x060012CB RID: 4811 RVA: 0x000469AC File Offset: 0x00044BAC
		[LocalizationKey("Options")]
		private string labelKey
		{
			get
			{
				return "Options_" + this.key;
			}
			set
			{
			}
		}

		// Token: 0x1700036A RID: 874
		// (get) Token: 0x060012CC RID: 4812 RVA: 0x000469AE File Offset: 0x00044BAE
		// (set) Token: 0x060012CD RID: 4813 RVA: 0x000469C1 File Offset: 0x00044BC1
		public float Value
		{
			get
			{
				return OptionsManager.Load<float>(this.key, this.defaultValue);
			}
			set
			{
				OptionsManager.Save<float>(this.key, value);
			}
		}

		// Token: 0x060012CE RID: 4814 RVA: 0x000469D0 File Offset: 0x00044BD0
		private void Awake()
		{
			this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderValueChanged));
			this.valueField.onEndEdit.AddListener(new UnityAction<string>(this.OnFieldEndEdit));
			this.RefreshLable();
			LocalizationManager.OnSetLanguage += this.OnLanguageChanged;
		}

		// Token: 0x060012CF RID: 4815 RVA: 0x00046A2C File Offset: 0x00044C2C
		private void OnDestroy()
		{
			LocalizationManager.OnSetLanguage -= this.OnLanguageChanged;
		}

		// Token: 0x060012D0 RID: 4816 RVA: 0x00046A3F File Offset: 0x00044C3F
		private void OnLanguageChanged(SystemLanguage language)
		{
			this.RefreshLable();
		}

		// Token: 0x060012D1 RID: 4817 RVA: 0x00046A47 File Offset: 0x00044C47
		private void RefreshLable()
		{
			if (this.label)
			{
				this.label.text = this.labelKey.ToPlainText();
			}
		}

		// Token: 0x060012D2 RID: 4818 RVA: 0x00046A6C File Offset: 0x00044C6C
		private void OnFieldEndEdit(string arg0)
		{
			float num;
			if (float.TryParse(arg0, out num))
			{
				num = Mathf.Clamp(num, this.slider.minValue, this.slider.maxValue);
				this.Value = num;
			}
			this.RefreshValues();
		}

		// Token: 0x060012D3 RID: 4819 RVA: 0x00046AAD File Offset: 0x00044CAD
		private void OnEnable()
		{
			this.RefreshValues();
		}

		// Token: 0x060012D4 RID: 4820 RVA: 0x00046AB5 File Offset: 0x00044CB5
		private void OnSliderValueChanged(float value)
		{
			this.Value = value;
			this.RefreshValues();
		}

		// Token: 0x060012D5 RID: 4821 RVA: 0x00046AC4 File Offset: 0x00044CC4
		private void RefreshValues()
		{
			this.valueField.SetTextWithoutNotify(this.Value.ToString(this.valueFormat));
			this.slider.SetValueWithoutNotify(this.Value);
		}

		// Token: 0x060012D6 RID: 4822 RVA: 0x00046B01 File Offset: 0x00044D01
		private void OnValidate()
		{
			this.RefreshLable();
		}

		// Token: 0x04000E23 RID: 3619
		[SerializeField]
		private string key;

		// Token: 0x04000E24 RID: 3620
		[Space]
		[SerializeField]
		private float defaultValue;

		// Token: 0x04000E25 RID: 3621
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x04000E26 RID: 3622
		[SerializeField]
		private Slider slider;

		// Token: 0x04000E27 RID: 3623
		[SerializeField]
		private TMP_InputField valueField;

		// Token: 0x04000E28 RID: 3624
		[SerializeField]
		private string valueFormat = "0";
	}
}
