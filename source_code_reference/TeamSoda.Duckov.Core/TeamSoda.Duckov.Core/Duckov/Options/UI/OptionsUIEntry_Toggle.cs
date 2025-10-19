using System;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.Options.UI
{
	// Token: 0x0200025E RID: 606
	public class OptionsUIEntry_Toggle : MonoBehaviour
	{
		// Token: 0x1700036B RID: 875
		// (get) Token: 0x060012D8 RID: 4824 RVA: 0x00046B1C File Offset: 0x00044D1C
		// (set) Token: 0x060012D9 RID: 4825 RVA: 0x00046B2E File Offset: 0x00044D2E
		[LocalizationKey("Default")]
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

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x060012DA RID: 4826 RVA: 0x00046B30 File Offset: 0x00044D30
		// (set) Token: 0x060012DB RID: 4827 RVA: 0x00046B43 File Offset: 0x00044D43
		public bool Value
		{
			get
			{
				return OptionsManager.Load<bool>(this.key, this.defaultValue);
			}
			set
			{
				OptionsManager.Save<bool>(this.key, value);
			}
		}

		// Token: 0x1700036D RID: 877
		// (get) Token: 0x060012DC RID: 4828 RVA: 0x00046B51 File Offset: 0x00044D51
		private int SliderValue
		{
			get
			{
				if (!this.Value)
				{
					return 0;
				}
				return 1;
			}
		}

		// Token: 0x060012DD RID: 4829 RVA: 0x00046B60 File Offset: 0x00044D60
		private void Awake()
		{
			this.toggle.wholeNumbers = true;
			this.toggle.minValue = 0f;
			this.toggle.maxValue = 1f;
			this.toggle.onValueChanged.AddListener(new UnityAction<float>(this.OnToggleValueChanged));
			this.label.text = this.labelKey.ToPlainText();
		}

		// Token: 0x060012DE RID: 4830 RVA: 0x00046BCB File Offset: 0x00044DCB
		private void OnEnable()
		{
			this.toggle.SetValueWithoutNotify((float)this.SliderValue);
		}

		// Token: 0x060012DF RID: 4831 RVA: 0x00046BDF File Offset: 0x00044DDF
		private void OnToggleValueChanged(float value)
		{
			this.Value = value > 0f;
		}

		// Token: 0x04000E29 RID: 3625
		[SerializeField]
		private string key;

		// Token: 0x04000E2A RID: 3626
		[SerializeField]
		private bool defaultValue;

		// Token: 0x04000E2B RID: 3627
		[Space]
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x04000E2C RID: 3628
		[SerializeField]
		private Slider toggle;
	}
}
