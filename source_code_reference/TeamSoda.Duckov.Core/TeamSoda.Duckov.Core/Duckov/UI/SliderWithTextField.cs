using System;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x02000387 RID: 903
	public class SliderWithTextField : MonoBehaviour
	{
		// Token: 0x17000606 RID: 1542
		// (get) Token: 0x06001F5D RID: 8029 RVA: 0x0006DBCC File Offset: 0x0006BDCC
		// (set) Token: 0x06001F5E RID: 8030 RVA: 0x0006DBD4 File Offset: 0x0006BDD4
		[LocalizationKey("Default")]
		public string LabelKey
		{
			get
			{
				return this._labelKey;
			}
			set
			{
			}
		}

		// Token: 0x17000607 RID: 1543
		// (get) Token: 0x06001F5F RID: 8031 RVA: 0x0006DBD6 File Offset: 0x0006BDD6
		// (set) Token: 0x06001F60 RID: 8032 RVA: 0x0006DBDE File Offset: 0x0006BDDE
		public float Value
		{
			get
			{
				return this.GetValue();
			}
			set
			{
				this.SetValue(value);
			}
		}

		// Token: 0x06001F61 RID: 8033 RVA: 0x0006DBE7 File Offset: 0x0006BDE7
		public void SetValueWithoutNotify(float value)
		{
			this.value = value;
			this.RefreshValues();
		}

		// Token: 0x06001F62 RID: 8034 RVA: 0x0006DBF6 File Offset: 0x0006BDF6
		public void SetValue(float value)
		{
			this.SetValueWithoutNotify(value);
			Action<float> action = this.onValueChanged;
			if (action == null)
			{
				return;
			}
			action(value);
		}

		// Token: 0x06001F63 RID: 8035 RVA: 0x0006DC10 File Offset: 0x0006BE10
		public float GetValue()
		{
			return this.value;
		}

		// Token: 0x06001F64 RID: 8036 RVA: 0x0006DC18 File Offset: 0x0006BE18
		private void Awake()
		{
			this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderValueChanged));
			this.valueField.onEndEdit.AddListener(new UnityAction<string>(this.OnFieldEndEdit));
			this.RefreshLable();
			LocalizationManager.OnSetLanguage += this.OnLanguageChanged;
		}

		// Token: 0x06001F65 RID: 8037 RVA: 0x0006DC74 File Offset: 0x0006BE74
		private void OnDestroy()
		{
			LocalizationManager.OnSetLanguage -= this.OnLanguageChanged;
		}

		// Token: 0x06001F66 RID: 8038 RVA: 0x0006DC87 File Offset: 0x0006BE87
		private void OnLanguageChanged(SystemLanguage language)
		{
			this.RefreshLable();
		}

		// Token: 0x06001F67 RID: 8039 RVA: 0x0006DC8F File Offset: 0x0006BE8F
		private void RefreshLable()
		{
			if (this.label)
			{
				this.label.text = this.LabelKey.ToPlainText();
			}
		}

		// Token: 0x06001F68 RID: 8040 RVA: 0x0006DCB4 File Offset: 0x0006BEB4
		private void OnFieldEndEdit(string arg0)
		{
			float num;
			if (float.TryParse(arg0, out num))
			{
				if (this.isPercentage)
				{
					num /= 100f;
				}
				num = Mathf.Clamp(num, this.slider.minValue, this.slider.maxValue);
				this.Value = num;
			}
			this.RefreshValues();
		}

		// Token: 0x06001F69 RID: 8041 RVA: 0x0006DD05 File Offset: 0x0006BF05
		private void OnEnable()
		{
			this.RefreshValues();
		}

		// Token: 0x06001F6A RID: 8042 RVA: 0x0006DD0D File Offset: 0x0006BF0D
		private void OnSliderValueChanged(float value)
		{
			this.Value = value;
			this.RefreshValues();
		}

		// Token: 0x06001F6B RID: 8043 RVA: 0x0006DD1C File Offset: 0x0006BF1C
		private void RefreshValues()
		{
			this.valueField.SetTextWithoutNotify(this.Value.ToString(this.valueFormat));
			this.slider.SetValueWithoutNotify(this.Value);
		}

		// Token: 0x06001F6C RID: 8044 RVA: 0x0006DD59 File Offset: 0x0006BF59
		private void OnValidate()
		{
			this.RefreshLable();
		}

		// Token: 0x0400156E RID: 5486
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x0400156F RID: 5487
		[SerializeField]
		private Slider slider;

		// Token: 0x04001570 RID: 5488
		[SerializeField]
		private TMP_InputField valueField;

		// Token: 0x04001571 RID: 5489
		[SerializeField]
		private string valueFormat = "0";

		// Token: 0x04001572 RID: 5490
		[SerializeField]
		private bool isPercentage;

		// Token: 0x04001573 RID: 5491
		[SerializeField]
		private string _labelKey = "?";

		// Token: 0x04001574 RID: 5492
		[SerializeField]
		private float value;

		// Token: 0x04001575 RID: 5493
		public Action<float> onValueChanged;
	}
}
