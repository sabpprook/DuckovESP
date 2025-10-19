using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x020003BC RID: 956
	public class CloseViewOnPointerClick : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x060022BC RID: 8892 RVA: 0x00079B70 File Offset: 0x00077D70
		private void OnValidate()
		{
			if (this.view == null)
			{
				this.view = base.GetComponent<View>();
			}
			if (this.graphic == null)
			{
				this.graphic = base.GetComponent<Graphic>();
			}
		}

		// Token: 0x060022BD RID: 8893 RVA: 0x00079BA8 File Offset: 0x00077DA8
		private void Awake()
		{
			if (this.view == null)
			{
				this.view = base.GetComponent<View>();
			}
			if (this.graphic == null)
			{
				this.graphic = base.GetComponent<Graphic>();
			}
			ManagedUIElement.onOpen += this.OnViewOpen;
			ManagedUIElement.onClose += this.OnViewClose;
		}

		// Token: 0x060022BE RID: 8894 RVA: 0x00079C0B File Offset: 0x00077E0B
		private void OnDestroy()
		{
			ManagedUIElement.onOpen -= this.OnViewOpen;
			ManagedUIElement.onClose -= this.OnViewClose;
		}

		// Token: 0x060022BF RID: 8895 RVA: 0x00079C2F File Offset: 0x00077E2F
		private void OnViewClose(ManagedUIElement element)
		{
			if (element != this.view)
			{
				return;
			}
			if (this.graphic == null)
			{
				return;
			}
			this.graphic.enabled = false;
		}

		// Token: 0x060022C0 RID: 8896 RVA: 0x00079C5B File Offset: 0x00077E5B
		private void OnViewOpen(ManagedUIElement element)
		{
			if (element != this.view)
			{
				return;
			}
			if (this.graphic == null)
			{
				return;
			}
			this.graphic.enabled = true;
		}

		// Token: 0x060022C1 RID: 8897 RVA: 0x00079C88 File Offset: 0x00077E88
		public void OnPointerClick(PointerEventData eventData)
		{
		}

		// Token: 0x040017A6 RID: 6054
		private const bool FunctionEnabled = false;

		// Token: 0x040017A7 RID: 6055
		[SerializeField]
		private View view;

		// Token: 0x040017A8 RID: 6056
		[SerializeField]
		private Graphic graphic;
	}
}
