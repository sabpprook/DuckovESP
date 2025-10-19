using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x020002A7 RID: 679
	public class StaminaDisplay : MonoBehaviour
	{
		// Token: 0x06001615 RID: 5653 RVA: 0x000517CC File Offset: 0x0004F9CC
		private void FixedUpdate()
		{
			this.Refresh();
		}

		// Token: 0x06001616 RID: 5654 RVA: 0x000517D4 File Offset: 0x0004F9D4
		private void Refresh()
		{
			if (this.master == null)
			{
				return;
			}
			GoldMinerRunData run = this.master.run;
			if (run == null)
			{
				return;
			}
			float stamina = run.stamina;
			float value = run.maxStamina.Value;
			float value2 = run.extraStamina.Value;
			if (stamina > 0f)
			{
				float num = stamina / value;
				this.fill.fillAmount = num;
				this.fill.color = this.normalColor.Evaluate(num);
				this.text.text = string.Format("{0:0.0}", stamina);
				return;
			}
			float num2 = value2 + stamina;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			float num3 = num2 / value2;
			this.fill.fillAmount = num3;
			this.fill.color = this.extraColor;
			this.text.text = string.Format("{0:0.00}", num2);
		}

		// Token: 0x04001064 RID: 4196
		[SerializeField]
		private GoldMiner master;

		// Token: 0x04001065 RID: 4197
		[SerializeField]
		private Image fill;

		// Token: 0x04001066 RID: 4198
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x04001067 RID: 4199
		[SerializeField]
		private Gradient normalColor;

		// Token: 0x04001068 RID: 4200
		[SerializeField]
		private Color extraColor = Color.red;
	}
}
