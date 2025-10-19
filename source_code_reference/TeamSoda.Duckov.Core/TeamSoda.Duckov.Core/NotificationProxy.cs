using System;
using Duckov.UI;
using SodaCraft.Localizations;
using UnityEngine;

// Token: 0x020000C4 RID: 196
public class NotificationProxy : MonoBehaviour
{
	// Token: 0x06000637 RID: 1591 RVA: 0x0001BF7B File Offset: 0x0001A17B
	public void Notify()
	{
		NotificationText.Push(this.notification.ToPlainText());
	}

	// Token: 0x040005F0 RID: 1520
	[LocalizationKey("Default")]
	public string notification;
}
