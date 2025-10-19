using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001F6 RID: 502
public class HealthBar_DamageBar : MonoBehaviour
{
	// Token: 0x06000EB6 RID: 3766 RVA: 0x0003AB47 File Offset: 0x00038D47
	private void Awake()
	{
		if (this.rectTransform == null)
		{
			this.rectTransform = base.transform as RectTransform;
		}
		if (this.image == null)
		{
			this.image = base.GetComponent<Image>();
		}
	}

	// Token: 0x06000EB7 RID: 3767 RVA: 0x0003AB84 File Offset: 0x00038D84
	public async UniTask Animate(float damageBarPostion, float damageBarWidth, Action onComplete)
	{
		base.gameObject.SetActive(true);
		this.rectTransform.anchoredPosition = new Vector2(damageBarPostion, 0f);
		this.rectTransform.sizeDelta = new Vector2(damageBarWidth, 0f);
		float time = 0f;
		while (time < this.duration)
		{
			if (this.rectTransform == null)
			{
				return;
			}
			time += Time.deltaTime;
			float num = time / this.duration;
			float num2 = this.curve.Evaluate(num) * this.targetSizeDelta;
			this.rectTransform.sizeDelta = new Vector2(damageBarWidth, num2);
			Color color = this.colorOverTime.Evaluate(num);
			this.image.color = color;
			await UniTask.NextFrame();
		}
		if (onComplete != null)
		{
			onComplete();
		}
	}

	// Token: 0x04000C29 RID: 3113
	[SerializeField]
	internal RectTransform rectTransform;

	// Token: 0x04000C2A RID: 3114
	[SerializeField]
	internal Image image;

	// Token: 0x04000C2B RID: 3115
	[SerializeField]
	private float duration;

	// Token: 0x04000C2C RID: 3116
	[SerializeField]
	private float targetSizeDelta = 4f;

	// Token: 0x04000C2D RID: 3117
	[SerializeField]
	private AnimationCurve curve;

	// Token: 0x04000C2E RID: 3118
	[SerializeField]
	private Gradient colorOverTime;
}
