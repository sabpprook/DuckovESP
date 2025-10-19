using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001EB RID: 491
public class ColorPunch : MonoBehaviour
{
	// Token: 0x06000E6D RID: 3693 RVA: 0x0003A05F File Offset: 0x0003825F
	private void Awake()
	{
		if (this.graphic == null)
		{
			this.graphic = base.GetComponent<Graphic>();
		}
		this.resetColor = this.graphic.color;
	}

	// Token: 0x06000E6E RID: 3694 RVA: 0x0003A08C File Offset: 0x0003828C
	public void Punch()
	{
		this.DoTask().Forget();
	}

	// Token: 0x06000E6F RID: 3695 RVA: 0x0003A099 File Offset: 0x00038299
	private int NewToken()
	{
		this.activeToken = global::UnityEngine.Random.Range(1, int.MaxValue);
		return this.activeToken;
	}

	// Token: 0x06000E70 RID: 3696 RVA: 0x0003A0B4 File Offset: 0x000382B4
	private async UniTask DoTask()
	{
		int token = this.NewToken();
		float time = 0f;
		if (this.duration > 0f)
		{
			while (time < this.duration)
			{
				time += Time.unscaledDeltaTime;
				float num = time / this.duration;
				this.graphic.color = this.gradient.Evaluate(num) * this.tint;
				await UniTask.NextFrame();
				if (token != this.activeToken)
				{
					return;
				}
			}
		}
		this.graphic.color = this.resetColor;
	}

	// Token: 0x04000BF8 RID: 3064
	[SerializeField]
	private Graphic graphic;

	// Token: 0x04000BF9 RID: 3065
	[SerializeField]
	private float duration;

	// Token: 0x04000BFA RID: 3066
	[SerializeField]
	private Gradient gradient;

	// Token: 0x04000BFB RID: 3067
	[SerializeField]
	private Color tint = Color.white;

	// Token: 0x04000BFC RID: 3068
	private Color resetColor;

	// Token: 0x04000BFD RID: 3069
	private int activeToken;
}
