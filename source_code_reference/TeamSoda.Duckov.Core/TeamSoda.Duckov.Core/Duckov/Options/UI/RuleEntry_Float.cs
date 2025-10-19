using System;
using System.Reflection;
using Duckov.Rules;
using Duckov.UI;
using UnityEngine;

namespace Duckov.Options.UI
{
	// Token: 0x0200025F RID: 607
	public class RuleEntry_Float : MonoBehaviour
	{
		// Token: 0x060012E1 RID: 4833 RVA: 0x00046BFC File Offset: 0x00044DFC
		private void Awake()
		{
			SliderWithTextField sliderWithTextField = this.slider;
			sliderWithTextField.onValueChanged = (Action<float>)Delegate.Combine(sliderWithTextField.onValueChanged, new Action<float>(this.OnValueChanged));
			GameRulesManager.OnRuleChanged += this.OnRuleChanged;
			Type typeFromHandle = typeof(Ruleset);
			this.field = typeFromHandle.GetField(this.fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			this.RefreshValue();
		}

		// Token: 0x060012E2 RID: 4834 RVA: 0x00046C66 File Offset: 0x00044E66
		private void OnRuleChanged()
		{
			this.RefreshValue();
		}

		// Token: 0x060012E3 RID: 4835 RVA: 0x00046C70 File Offset: 0x00044E70
		private void OnValueChanged(float value)
		{
			if (GameRulesManager.SelectedRuleIndex != RuleIndex.Custom)
			{
				this.RefreshValue();
				return;
			}
			Ruleset ruleset = GameRulesManager.Current;
			this.SetValue(ruleset, value);
			GameRulesManager.NotifyRuleChanged();
		}

		// Token: 0x060012E4 RID: 4836 RVA: 0x00046CA0 File Offset: 0x00044EA0
		public void RefreshValue()
		{
			float value = this.GetValue(GameRulesManager.Current);
			this.slider.SetValueWithoutNotify(value);
		}

		// Token: 0x060012E5 RID: 4837 RVA: 0x00046CC5 File Offset: 0x00044EC5
		protected void SetValue(Ruleset ruleset, float value)
		{
			this.field.SetValue(ruleset, value);
		}

		// Token: 0x060012E6 RID: 4838 RVA: 0x00046CD9 File Offset: 0x00044ED9
		protected float GetValue(Ruleset ruleset)
		{
			return (float)this.field.GetValue(ruleset);
		}

		// Token: 0x04000E2D RID: 3629
		[SerializeField]
		private SliderWithTextField slider;

		// Token: 0x04000E2E RID: 3630
		[SerializeField]
		private string fieldName = "damageFactor_ToPlayer";

		// Token: 0x04000E2F RID: 3631
		private FieldInfo field;
	}
}
