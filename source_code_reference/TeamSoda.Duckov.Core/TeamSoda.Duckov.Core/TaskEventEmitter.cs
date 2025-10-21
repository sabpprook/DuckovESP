using System;
using UnityEngine;

// Token: 0x02000120 RID: 288
public class TaskEventEmitter : MonoBehaviour
{
	// Token: 0x06000980 RID: 2432 RVA: 0x00029605 File Offset: 0x00027805
	public void SetKey(string key)
	{
		this.eventKey = key;
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x0002960E File Offset: 0x0002780E
	private void Awake()
	{
		if (this.emitOnAwake)
		{
			this.EmitEvent();
		}
	}

	// Token: 0x06000982 RID: 2434 RVA: 0x0002961E File Offset: 0x0002781E
	public void EmitEvent()
	{
		Debug.Log("TaskEvent:" + this.eventKey);
		TaskEvent.EmitTaskEvent(this.eventKey);
	}

	// Token: 0x04000862 RID: 2146
	[SerializeField]
	private string eventKey;

	// Token: 0x04000863 RID: 2147
	[SerializeField]
	private bool emitOnAwake;
}
