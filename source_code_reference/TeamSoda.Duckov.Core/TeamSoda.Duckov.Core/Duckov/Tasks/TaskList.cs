using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Duckov.Tasks
{
	// Token: 0x0200036F RID: 879
	public class TaskList : MonoBehaviour, ITaskBehaviour
	{
		// Token: 0x06001E60 RID: 7776 RVA: 0x0006AFEA File Offset: 0x000691EA
		private void Start()
		{
			if (this.beginOnStart)
			{
				this.Begin();
			}
		}

		// Token: 0x06001E61 RID: 7777 RVA: 0x0006AFFC File Offset: 0x000691FC
		private async UniTask MainTask()
		{
			for (int i = 0; i < this.tasks.Count; i++)
			{
				this.currentTaskIndex = i;
				MonoBehaviour monoBehaviour = this.tasks[this.currentTaskIndex];
				if (!(monoBehaviour == null))
				{
					ITaskBehaviour taskBehaviour = monoBehaviour as ITaskBehaviour;
					if (taskBehaviour != null)
					{
						this.currentTask = taskBehaviour;
						this.currentTask.Begin();
						while (this.currentTask != null && this.currentTask.IsPending() && !this.currentTask.IsComplete())
						{
							if (this == null)
							{
								return;
							}
							if (this.skip)
							{
								this.currentTask.Skip();
							}
							await UniTask.Yield();
						}
					}
				}
			}
			this.complete = true;
			this.running = false;
			UnityEvent unityEvent = this.onComplete;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
		}

		// Token: 0x06001E62 RID: 7778 RVA: 0x0006B03F File Offset: 0x0006923F
		public void Begin()
		{
			if (this.running)
			{
				return;
			}
			this.skip = false;
			this.running = true;
			this.complete = false;
			UnityEvent unityEvent = this.onBegin;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.MainTask().Forget();
		}

		// Token: 0x06001E63 RID: 7779 RVA: 0x0006B07B File Offset: 0x0006927B
		public bool IsComplete()
		{
			return this.complete;
		}

		// Token: 0x06001E64 RID: 7780 RVA: 0x0006B083 File Offset: 0x00069283
		public bool IsPending()
		{
			return this.running;
		}

		// Token: 0x06001E65 RID: 7781 RVA: 0x0006B08B File Offset: 0x0006928B
		public void Skip()
		{
			this.skip = true;
		}

		// Token: 0x040014C3 RID: 5315
		[SerializeField]
		private bool beginOnStart;

		// Token: 0x040014C4 RID: 5316
		[SerializeField]
		private List<MonoBehaviour> tasks;

		// Token: 0x040014C5 RID: 5317
		[SerializeField]
		private UnityEvent onBegin;

		// Token: 0x040014C6 RID: 5318
		[SerializeField]
		private UnityEvent onComplete;

		// Token: 0x040014C7 RID: 5319
		[SerializeField]
		private bool listenToSkipSignal;

		// Token: 0x040014C8 RID: 5320
		private bool running;

		// Token: 0x040014C9 RID: 5321
		private bool complete;

		// Token: 0x040014CA RID: 5322
		private int currentTaskIndex;

		// Token: 0x040014CB RID: 5323
		private ITaskBehaviour currentTask;

		// Token: 0x040014CC RID: 5324
		private bool skip;
	}
}
