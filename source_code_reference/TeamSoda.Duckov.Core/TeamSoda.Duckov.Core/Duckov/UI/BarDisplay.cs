using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x02000389 RID: 905
	public class BarDisplay : MonoBehaviour
	{
		// Token: 0x06001F79 RID: 8057 RVA: 0x0006E0C0 File Offset: 0x0006C2C0
		private void Awake()
		{
			this.fill.fillAmount = 0f;
			this.ApplyLook();
		}

		// Token: 0x06001F7A RID: 8058 RVA: 0x0006E0D8 File Offset: 0x0006C2D8
		public void Setup(string labelText, Color color, float current, float max, string format = "0.#", float min = 0f)
		{
			this.SetupLook(labelText, color);
			this.SetValue(current, max, format, min);
		}

		// Token: 0x06001F7B RID: 8059 RVA: 0x0006E0EF File Offset: 0x0006C2EF
		public void Setup(string labelText, Color color, int current, int max, int min = 0)
		{
			this.SetupLook(labelText, color);
			this.SetValue(current, max, min);
		}

		// Token: 0x06001F7C RID: 8060 RVA: 0x0006E104 File Offset: 0x0006C304
		public void SetupLook(string labelText, Color color)
		{
			this.labelText = labelText;
			this.color = color;
			this.ApplyLook();
		}

		// Token: 0x06001F7D RID: 8061 RVA: 0x0006E11A File Offset: 0x0006C31A
		private void ApplyLook()
		{
			this.text_Label.text = this.labelText.ToPlainText();
			this.fill.color = this.color;
		}

		// Token: 0x06001F7E RID: 8062 RVA: 0x0006E144 File Offset: 0x0006C344
		public void SetValue(float current, float max, string format = "0.#", float min = 0f)
		{
			this.text_Current.text = current.ToString(format);
			this.text_Max.text = max.ToString(format);
			float num = max - min;
			float num2 = 1f;
			if (num > 0f)
			{
				num2 = (current - min) / num;
			}
			this.fill.DOKill(false);
			this.fill.DOFillAmount(num2, this.animateDuration).SetEase(Ease.OutCubic);
		}

		// Token: 0x06001F7F RID: 8063 RVA: 0x0006E1B8 File Offset: 0x0006C3B8
		public void SetValue(int current, int max, int min = 0)
		{
			this.text_Current.text = current.ToString();
			this.text_Max.text = max.ToString();
			int num = max - min;
			float num2 = 1f;
			if (num > 0)
			{
				num2 = (float)(current - min) / (float)num;
			}
			this.fill.DOKill(false);
			this.fill.DOFillAmount(num2, this.animateDuration).SetEase(Ease.OutCubic);
		}

		// Token: 0x04001586 RID: 5510
		[SerializeField]
		private string labelText;

		// Token: 0x04001587 RID: 5511
		[SerializeField]
		private Color color = Color.red;

		// Token: 0x04001588 RID: 5512
		[SerializeField]
		private float animateDuration = 0.25f;

		// Token: 0x04001589 RID: 5513
		[SerializeField]
		private TextMeshProUGUI text_Label;

		// Token: 0x0400158A RID: 5514
		[SerializeField]
		private TextMeshProUGUI text_Current;

		// Token: 0x0400158B RID: 5515
		[SerializeField]
		private TextMeshProUGUI text_Max;

		// Token: 0x0400158C RID: 5516
		[SerializeField]
		private Image fill;
	}
}
