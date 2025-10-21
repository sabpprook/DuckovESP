using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Duckov.Tasks
{
	// Token: 0x0200036E RID: 878
	[Obsolete]
	public class EndingFlow : MonoBehaviour
	{
		// Token: 0x06001E5C RID: 7772 RVA: 0x0006AF41 File Offset: 0x00069141
		private void Start()
		{
			this.Task().Forget();
		}

		// Token: 0x06001E5D RID: 7773 RVA: 0x0006AF50 File Offset: 0x00069150
		private async UniTask Task()
		{
			this.onBegin.Invoke();
			Debug.Log("Ending begin!");
			foreach (MonoBehaviour monoBehaviour in this.taskBehaviours)
			{
				await this.WaitForTaskBehaviour(monoBehaviour);
			}
			List<MonoBehaviour>.Enumerator enumerator = default(List<MonoBehaviour>.Enumerator);
			Debug.Log("Ending end!");
			this.onEnd.Invoke();
		}

		// Token: 0x06001E5E RID: 7774 RVA: 0x0006AF94 File Offset: 0x00069194
		private async UniTask WaitForTaskBehaviour(MonoBehaviour mono)
		{
			ITaskBehaviour task = mono as ITaskBehaviour;
			if (task != null)
			{
				task.Begin();
				while (task.IsPending())
				{
					await UniTask.Yield();
				}
			}
		}

		// Token: 0x040014C0 RID: 5312
		[SerializeField]
		private List<MonoBehaviour> taskBehaviours = new List<MonoBehaviour>();

		// Token: 0x040014C1 RID: 5313
		[SerializeField]
		private UnityEvent onBegin;

		// Token: 0x040014C2 RID: 5314
		[SerializeField]
		private UnityEvent onEnd;
	}
}
