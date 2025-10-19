using System;
using UnityEngine.Events;

namespace Duckov.MiniGames
{
	// Token: 0x0200027B RID: 635
	public class VirtualCursorTarget : MiniGameBehaviour
	{
		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x06001423 RID: 5155 RVA: 0x0004ACA9 File Offset: 0x00048EA9
		public bool IsHovering
		{
			get
			{
				return VirtualCursor.IsHovering(this);
			}
		}

		// Token: 0x06001424 RID: 5156 RVA: 0x0004ACB1 File Offset: 0x00048EB1
		public void OnCursorEnter()
		{
			UnityEvent unityEvent = this.onEnter;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x06001425 RID: 5157 RVA: 0x0004ACC3 File Offset: 0x00048EC3
		public void OnCursorExit()
		{
			UnityEvent unityEvent = this.onExit;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x06001426 RID: 5158 RVA: 0x0004ACD5 File Offset: 0x00048ED5
		public void OnClick()
		{
			UnityEvent unityEvent = this.onClick;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x04000ED1 RID: 3793
		public UnityEvent onEnter;

		// Token: 0x04000ED2 RID: 3794
		public UnityEvent onExit;

		// Token: 0x04000ED3 RID: 3795
		public UnityEvent onClick;
	}
}
