using System;
using UnityEngine;

// Token: 0x020000CF RID: 207
public class TimeOfDayAlertTriggerProxy : MonoBehaviour
{
	// Token: 0x06000664 RID: 1636 RVA: 0x0001CEB7 File Offset: 0x0001B0B7
	public void OnEnter()
	{
		TimeOfDayAlert.EnterAlertTrigger();
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x0001CEBE File Offset: 0x0001B0BE
	public void OnLeave()
	{
		TimeOfDayAlert.LeaveAlertTrigger();
	}
}
