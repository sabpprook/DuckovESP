using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Duckov;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001F5 RID: 501
public class ExpDisplay : MonoBehaviour
{
	// Token: 0x06000EAA RID: 3754 RVA: 0x0003A8CC File Offset: 0x00038ACC
	private void Refresh()
	{
		EXPManager instance = EXPManager.Instance;
		if (instance == null)
		{
			return;
		}
		int num = instance.LevelFromExp(this.displayExp);
		if (this.displayingLevel != num)
		{
			this.displayingLevel = num;
			this.OnDisplayingLevelChanged();
		}
		ValueTuple<long, long> levelExpRange = this.GetLevelExpRange(num);
		long num2 = levelExpRange.Item2 - levelExpRange.Item1;
		this.txtLevel.text = num.ToString();
		this.txtCurrentExp.text = this.displayExp.ToString();
		string text;
		if (levelExpRange.Item2 == 9223372036854775807L)
		{
			text = "∞";
		}
		else
		{
			text = levelExpRange.Item2.ToString();
		}
		this.txtMaxExp.text = text;
		float num3 = (float)((double)(this.displayExp - levelExpRange.Item1) / (double)num2);
		this.expBarFill.fillAmount = num3;
	}

	// Token: 0x06000EAB RID: 3755 RVA: 0x0003A9A0 File Offset: 0x00038BA0
	private void OnDisplayingLevelChanged()
	{
	}

	// Token: 0x06000EAC RID: 3756 RVA: 0x0003A9A4 File Offset: 0x00038BA4
	[return: TupleElementNames(new string[] { "from", "to" })]
	private ValueTuple<long, long> GetLevelExpRange(int level)
	{
		ValueTuple<long, long> valueTuple;
		if (this.cachedLevelExpRange.TryGetValue(level, out valueTuple))
		{
			return valueTuple;
		}
		EXPManager instance = EXPManager.Instance;
		if (instance == null)
		{
			return new ValueTuple<long, long>(0L, 0L);
		}
		ValueTuple<long, long> levelExpRange = instance.GetLevelExpRange(level);
		this.cachedLevelExpRange[level] = levelExpRange;
		return levelExpRange;
	}

	// Token: 0x06000EAD RID: 3757 RVA: 0x0003A9F2 File Offset: 0x00038BF2
	private void SnapToCurrent()
	{
		this.displayExp = EXPManager.EXP;
		this.Refresh();
	}

	// Token: 0x06000EAE RID: 3758 RVA: 0x0003AA08 File Offset: 0x00038C08
	private async UniTask Animate(long targetExp, float duration, AnimationCurve curve)
	{
		int token = global::UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		this.currentToken = token;
		if (duration > 0f)
		{
			float time = 0f;
			long from = this.displayExp;
			while (time < duration)
			{
				if (this.currentToken != token)
				{
					return;
				}
				float num = curve.Evaluate(time / duration);
				this.displayExp = this.LongLerp(from, targetExp, num);
				time += Time.deltaTime;
				this.Refresh();
				await UniTask.WaitForEndOfFrame(this);
			}
		}
		this.displayExp = targetExp;
		this.Refresh();
	}

	// Token: 0x06000EAF RID: 3759 RVA: 0x0003AA64 File Offset: 0x00038C64
	private long LongLerp(long a, long b, float t)
	{
		long num = b - a;
		return a + (long)(t * (float)num);
	}

	// Token: 0x06000EB0 RID: 3760 RVA: 0x0003AA7C File Offset: 0x00038C7C
	private void OnEnable()
	{
		if (this.snapToCurrentOnEnable)
		{
			this.SnapToCurrent();
		}
		this.RegisterEvents();
	}

	// Token: 0x06000EB1 RID: 3761 RVA: 0x0003AA92 File Offset: 0x00038C92
	private void OnDisable()
	{
		this.UnregisterEvents();
	}

	// Token: 0x06000EB2 RID: 3762 RVA: 0x0003AA9A File Offset: 0x00038C9A
	private void RegisterEvents()
	{
		EXPManager.onExpChanged = (Action<long>)Delegate.Combine(EXPManager.onExpChanged, new Action<long>(this.OnExpChanged));
	}

	// Token: 0x06000EB3 RID: 3763 RVA: 0x0003AABC File Offset: 0x00038CBC
	private void UnregisterEvents()
	{
		EXPManager.onExpChanged = (Action<long>)Delegate.Remove(EXPManager.onExpChanged, new Action<long>(this.OnExpChanged));
	}

	// Token: 0x06000EB4 RID: 3764 RVA: 0x0003AADE File Offset: 0x00038CDE
	private void OnExpChanged(long exp)
	{
		this.Animate(exp, this.animationDuration, this.animationCurve).Forget();
	}

	// Token: 0x04000C1E RID: 3102
	[SerializeField]
	private TextMeshProUGUI txtLevel;

	// Token: 0x04000C1F RID: 3103
	[SerializeField]
	private TextMeshProUGUI txtCurrentExp;

	// Token: 0x04000C20 RID: 3104
	[SerializeField]
	private TextMeshProUGUI txtMaxExp;

	// Token: 0x04000C21 RID: 3105
	[SerializeField]
	private Image expBarFill;

	// Token: 0x04000C22 RID: 3106
	[SerializeField]
	private bool snapToCurrentOnEnable;

	// Token: 0x04000C23 RID: 3107
	[SerializeField]
	private float animationDuration = 0.1f;

	// Token: 0x04000C24 RID: 3108
	[SerializeField]
	private AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000C25 RID: 3109
	[SerializeField]
	private long displayExp;

	// Token: 0x04000C26 RID: 3110
	private int displayingLevel = -1;

	// Token: 0x04000C27 RID: 3111
	[TupleElementNames(new string[] { "from", "to" })]
	private Dictionary<int, ValueTuple<long, long>> cachedLevelExpRange = new Dictionary<int, ValueTuple<long, long>>();

	// Token: 0x04000C28 RID: 3112
	private int currentToken;
}
