using System;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000C3 RID: 195
public class EvacuationCountdownUI : MonoBehaviour
{
	// Token: 0x1700012D RID: 301
	// (get) Token: 0x0600062D RID: 1581 RVA: 0x0001BD7E File Offset: 0x00019F7E
	public static EvacuationCountdownUI Instance
	{
		get
		{
			return EvacuationCountdownUI._instance;
		}
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x0001BD85 File Offset: 0x00019F85
	private void Awake()
	{
		if (EvacuationCountdownUI._instance == null)
		{
			EvacuationCountdownUI._instance = this;
		}
		if (EvacuationCountdownUI._instance != this)
		{
			Debug.LogWarning("Multiple Evacuation Countdown UI detected");
		}
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x0001BDB4 File Offset: 0x00019FB4
	private string ToDigitString(float number)
	{
		int num = (int)number;
		int num2 = Mathf.Min(999, Mathf.RoundToInt((number - (float)num) * 1000f));
		int num3 = num / 60;
		num -= num3 * 60;
		return string.Format(this.digitFormat, num3, num, num2);
	}

	// Token: 0x06000630 RID: 1584 RVA: 0x0001BE07 File Offset: 0x0001A007
	private void Update()
	{
		if (this.target == null && this.fadeGroup.IsShown)
		{
			this.Hide().Forget();
		}
		this.Refresh();
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x0001BE38 File Offset: 0x0001A038
	private void Refresh()
	{
		if (this.target == null)
		{
			return;
		}
		this.progressFill.fillAmount = this.target.Progress;
		this.countdownDigit.text = this.ToDigitString(this.target.RemainingTime);
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x0001BE88 File Offset: 0x0001A088
	private async UniTask Hide()
	{
		this.target = null;
		await this.fadeGroup.HideAndReturnTask();
	}

	// Token: 0x06000633 RID: 1587 RVA: 0x0001BECC File Offset: 0x0001A0CC
	private async UniTask Show(CountDownArea target)
	{
		this.target = target;
		if (!(this.target == null))
		{
			await this.fadeGroup.ShowAndReturnTask();
		}
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x0001BF17 File Offset: 0x0001A117
	public static void Request(CountDownArea target)
	{
		if (EvacuationCountdownUI.Instance == null)
		{
			return;
		}
		EvacuationCountdownUI.Instance.Show(target).Forget();
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x0001BF37 File Offset: 0x0001A137
	public static void Release(CountDownArea target)
	{
		if (EvacuationCountdownUI.Instance == null)
		{
			return;
		}
		if (EvacuationCountdownUI.Instance.target == target)
		{
			EvacuationCountdownUI.Instance.Hide().Forget();
		}
	}

	// Token: 0x040005EA RID: 1514
	private static EvacuationCountdownUI _instance;

	// Token: 0x040005EB RID: 1515
	[SerializeField]
	private FadeGroup fadeGroup;

	// Token: 0x040005EC RID: 1516
	[SerializeField]
	private Image progressFill;

	// Token: 0x040005ED RID: 1517
	[SerializeField]
	private TextMeshProUGUI countdownDigit;

	// Token: 0x040005EE RID: 1518
	[SerializeField]
	private string digitFormat = "{0:00}:{1:00}<sub>.{2:000}</sub>";

	// Token: 0x040005EF RID: 1519
	private CountDownArea target;
}
