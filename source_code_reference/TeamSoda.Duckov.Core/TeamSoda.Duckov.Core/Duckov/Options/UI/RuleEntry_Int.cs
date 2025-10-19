using System;
using System.Reflection;
using Duckov.Rules;
using Duckov.UI;
using UnityEngine;

namespace Duckov.Options.UI
{
	// Token: 0x02000260 RID: 608
	public class RuleEntry_Int : MonoBehaviour
	{
		// Token: 0x060012E8 RID: 4840 RVA: 0x00046D00 File Offset: 0x00044F00
		private void Awake()
		{
			SliderWithTextField sliderWithTextField = this.slider;
			sliderWithTextField.onValueChanged = (Action<float>)Delegate.Combine(sliderWithTextField.onValueChanged, new Action<float>(this.OnValueChanged));
			GameRulesManager.OnRuleChanged += this.OnRuleChanged;
			Type typeFromHandle = typeof(Ruleset);
			this.field = typeFromHandle.GetField(this.fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			this.RefreshValue();
		}

		// Token: 0x060012E9 RID: 4841 RVA: 0x00046D6A File Offset: 0x00044F6A
		private void OnRuleChanged()
		{
			this.RefreshValue();
		}

		// Token: 0x060012EA RID: 4842 RVA: 0x00046D74 File Offset: 0x00044F74
		private void OnValueChanged(float value)
		{
			if (GameRulesManager.SelectedRuleIndex != RuleIndex.Custom)
			{
				this.RefreshValue();
				return;
			}
			Ruleset ruleset = GameRulesManager.Current;
			this.SetValue(ruleset, (int)value);
			GameRulesManager.NotifyRuleChanged();
		}

		// Token: 0x060012EB RID: 4843 RVA: 0x00046DA4 File Offset: 0x00044FA4
		public void RefreshValue()
		{
			float num = (float)this.GetValue(GameRulesManager.Current);
			this.slider.SetValueWithoutNotify(num);
		}

		// Token: 0x060012EC RID: 4844 RVA: 0x00046DCA File Offset: 0x00044FCA
		protected void SetValue(Ruleset ruleset, int value)
		{
			this.field.SetValue(ruleset, value);
		}

		// Token: 0x060012ED RID: 4845 RVA: 0x00046DDE File Offset: 0x00044FDE
		protected int GetValue(Ruleset ruleset)
		{
			return (int)this.field.GetValue(ruleset);
		}

		// Token: 0x04000E30 RID: 3632
		[SerializeField]
		private SliderWithTextField slider;

		// Token: 0x04000E31 RID: 3633
		[SerializeField]
		private string fieldName = "damageFactor_ToPlayer";

		// Token: 0x04000E32 RID: 3634
		private FieldInfo field;
	}
}
