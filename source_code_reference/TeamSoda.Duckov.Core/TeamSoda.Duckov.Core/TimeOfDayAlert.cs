using System;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;

// Token: 0x020000CE RID: 206
public class TimeOfDayAlert : MonoBehaviour
{
	// Token: 0x14000027 RID: 39
	// (add) Token: 0x0600065B RID: 1627 RVA: 0x0001CC74 File Offset: 0x0001AE74
	// (remove) Token: 0x0600065C RID: 1628 RVA: 0x0001CCA8 File Offset: 0x0001AEA8
	public static event Action OnAlertTriggeredEvent;

	// Token: 0x0600065D RID: 1629 RVA: 0x0001CCDB File Offset: 0x0001AEDB
	private void Awake()
	{
		this.canvasGroup.alpha = 0f;
		TimeOfDayAlert.OnAlertTriggeredEvent += this.OnAlertTriggered;
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x0001CCFE File Offset: 0x0001AEFE
	private void OnDestroy()
	{
		TimeOfDayAlert.OnAlertTriggeredEvent -= this.OnAlertTriggered;
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x0001CD14 File Offset: 0x0001AF14
	private void Update()
	{
		if (!LevelManager.LevelInited)
		{
			return;
		}
		if (!LevelManager.Instance.IsBaseLevel)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (this.timer > 0f)
		{
			this.timer -= Time.deltaTime;
		}
		if (this.timer <= 0f && this.canvasGroup.alpha > 0f)
		{
			this.canvasGroup.alpha = Mathf.MoveTowards(this.canvasGroup.alpha, 0f, 0.4f * Time.unscaledDeltaTime);
		}
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x0001CDAC File Offset: 0x0001AFAC
	private void OnAlertTriggered()
	{
		bool flag = false;
		float time = TimeOfDayController.Instance.Time;
		if (TimeOfDayController.Instance.AtNight)
		{
			flag = true;
			Debug.Log(string.Format("At Night,time:{0}", time));
			this.text.text = this.inNightKey.ToPlainText();
		}
		else if (TimeOfDayController.Instance.nightStart - time < 4f)
		{
			flag = true;
			Debug.Log(string.Format("Near Night,time:{0},night start:{1}", time, TimeOfDayController.Instance.nightStart));
			this.text.text = this.nearNightKey.ToPlainText();
		}
		if (!flag)
		{
			return;
		}
		this.canvasGroup.alpha = 1f;
		this.timer = this.stayTime;
		this.blinkPunch.Punch();
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x0001CE7B File Offset: 0x0001B07B
	public static void EnterAlertTrigger()
	{
		Action onAlertTriggeredEvent = TimeOfDayAlert.OnAlertTriggeredEvent;
		if (onAlertTriggeredEvent == null)
		{
			return;
		}
		onAlertTriggeredEvent();
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x0001CE8C File Offset: 0x0001B08C
	public static void LeaveAlertTrigger()
	{
	}

	// Token: 0x04000623 RID: 1571
	[SerializeField]
	private CanvasGroup canvasGroup;

	// Token: 0x04000624 RID: 1572
	[SerializeField]
	public TextMeshProUGUI text;

	// Token: 0x04000625 RID: 1573
	[SerializeField]
	private ColorPunch blinkPunch;

	// Token: 0x04000627 RID: 1575
	[LocalizationKey("Default")]
	public string nearNightKey = "TODAlert_NearNight";

	// Token: 0x04000628 RID: 1576
	[LocalizationKey("Default")]
	public string inNightKey = "TODAlert_InNight";

	// Token: 0x04000629 RID: 1577
	private float stayTime = 5f;

	// Token: 0x0400062A RID: 1578
	private float timer;
}
