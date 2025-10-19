using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Duckov.Tasks
{
	// Token: 0x02000370 RID: 880
	public class ParallelTask : MonoBehaviour, ITaskBehaviour
	{
		// Token: 0x06001E67 RID: 7783 RVA: 0x0006B09C File Offset: 0x0006929C
		private void Start()
		{
			if (this.beginOnStart)
			{
				this.Begin();
			}
		}

		// Token: 0x06001E68 RID: 7784 RVA: 0x0006B0AC File Offset: 0x000692AC
		private async UniTask MainTask()
		{
			foreach (MonoBehaviour monoBehaviour in this.tasks)
			{
				if (!(monoBehaviour == null))
				{
					ITaskBehaviour taskBehaviour = monoBehaviour as ITaskBehaviour;
					if (taskBehaviour != null)
					{
						taskBehaviour.Begin();
					}
				}
			}
			bool anyTaskPending = false;
			while (anyTaskPending)
			{
				if (this == null)
				{
					return;
				}
				anyTaskPending = false;
				foreach (MonoBehaviour monoBehaviour2 in this.tasks)
				{
					if (!(monoBehaviour2 == null))
					{
						ITaskBehaviour taskBehaviour2 = monoBehaviour2 as ITaskBehaviour;
						if (taskBehaviour2 != null && taskBehaviour2.IsPending())
						{
							anyTaskPending = true;
							break;
						}
					}
				}
				if (!anyTaskPending)
				{
					break;
				}
				await UniTask.Yield();
			}
			this.running = false;
			this.complete = true;
			UnityEvent unityEvent = this.onComplete;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
		}

		// Token: 0x06001E69 RID: 7785 RVA: 0x0006B0EF File Offset: 0x000692EF
		public void Begin()
		{
			if (this.running)
			{
				return;
			}
			this.running = true;
			this.complete = false;
			UnityEvent unityEvent = this.onBegin;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.MainTask().Forget();
		}

		// Token: 0x06001E6A RID: 7786 RVA: 0x0006B124 File Offset: 0x00069324
		public bool IsComplete()
		{
			return this.complete;
		}

		// Token: 0x06001E6B RID: 7787 RVA: 0x0006B12C File Offset: 0x0006932C
		public bool IsPending()
		{
			return this.running;
		}

		// Token: 0x040014CD RID: 5325
		[SerializeField]
		private bool beginOnStart;

		// Token: 0x040014CE RID: 5326
		[SerializeField]
		private List<MonoBehaviour> tasks;

		// Token: 0x040014CF RID: 5327
		[SerializeField]
		private UnityEvent onBegin;

		// Token: 0x040014D0 RID: 5328
		[SerializeField]
		private UnityEvent onComplete;

		// Token: 0x040014D1 RID: 5329
		private bool running;

		// Token: 0x040014D2 RID: 5330
		private bool complete;
	}
}
