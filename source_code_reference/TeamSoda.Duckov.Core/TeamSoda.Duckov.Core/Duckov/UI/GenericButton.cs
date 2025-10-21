using System;
using System.Collections.Generic;
using Duckov.UI.Animations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Duckov.UI
{
	// Token: 0x0200038A RID: 906
	public class GenericButton : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler
	{
		// Token: 0x06001F81 RID: 8065 RVA: 0x0006E244 File Offset: 0x0006C444
		public void OnPointerClick(PointerEventData eventData)
		{
			UnityEvent unityEvent = this.onPointerClick;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x06001F82 RID: 8066 RVA: 0x0006E258 File Offset: 0x0006C458
		public void OnPointerDown(PointerEventData eventData)
		{
			foreach (ToggleAnimation toggleAnimation in this.toggleAnimations)
			{
				toggleAnimation.SetToggle(true);
			}
			UnityEvent unityEvent = this.onPointerDown;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x06001F83 RID: 8067 RVA: 0x0006E2BC File Offset: 0x0006C4BC
		public void OnPointerUp(PointerEventData eventData)
		{
			foreach (ToggleAnimation toggleAnimation in this.toggleAnimations)
			{
				toggleAnimation.SetToggle(false);
			}
			UnityEvent unityEvent = this.onPointerUp;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x0400158D RID: 5517
		public List<ToggleAnimation> toggleAnimations = new List<ToggleAnimation>();

		// Token: 0x0400158E RID: 5518
		public UnityEvent onPointerClick;

		// Token: 0x0400158F RID: 5519
		public UnityEvent onPointerDown;

		// Token: 0x04001590 RID: 5520
		public UnityEvent onPointerUp;
	}
}
