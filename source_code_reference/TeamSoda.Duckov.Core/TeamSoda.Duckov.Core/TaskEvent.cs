using System;

// Token: 0x0200011F RID: 287
public class TaskEvent
{
	// Token: 0x14000046 RID: 70
	// (add) Token: 0x0600097C RID: 2428 RVA: 0x00029584 File Offset: 0x00027784
	// (remove) Token: 0x0600097D RID: 2429 RVA: 0x000295B8 File Offset: 0x000277B8
	public static event Action<string> OnTaskEvent;

	// Token: 0x0600097E RID: 2430 RVA: 0x000295EB File Offset: 0x000277EB
	public static void EmitTaskEvent(string taskEventKey)
	{
		Action<string> onTaskEvent = TaskEvent.OnTaskEvent;
		if (onTaskEvent == null)
		{
			return;
		}
		onTaskEvent(taskEventKey);
	}
}
