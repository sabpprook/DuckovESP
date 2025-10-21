using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.UI.Animations
{
	// Token: 0x020003DC RID: 988
	public class ButtonAnimation : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x060023CB RID: 9163 RVA: 0x0007CD5C File Offset: 0x0007AF5C
		private void Awake()
		{
			this.SetAll(false);
			if (this.hoveringIndicator)
			{
				this.hoveringIndicator.SetActive(false);
			}
		}

		// Token: 0x060023CC RID: 9164 RVA: 0x0007CD7E File Offset: 0x0007AF7E
		private void OnEnable()
		{
			this.SetAll(false);
		}

		// Token: 0x060023CD RID: 9165 RVA: 0x0007CD87 File Offset: 0x0007AF87
		private void OnDisable()
		{
			if (this.hoveringIndicator)
			{
				this.hoveringIndicator.SetActive(false);
			}
		}

		// Token: 0x060023CE RID: 9166 RVA: 0x0007CDA4 File Offset: 0x0007AFA4
		private void SetAll(bool value)
		{
			foreach (ToggleAnimation toggleAnimation in this.toggles)
			{
				if (!(toggleAnimation == null))
				{
					toggleAnimation.SetToggle(value);
				}
			}
		}

		// Token: 0x060023CF RID: 9167 RVA: 0x0007CE00 File Offset: 0x0007B000
		public void OnPointerDown(PointerEventData eventData)
		{
			this.SetAll(true);
			if (!this.mute)
			{
				AudioManager.Post("UI/click");
			}
		}

		// Token: 0x060023D0 RID: 9168 RVA: 0x0007CE1C File Offset: 0x0007B01C
		public void OnPointerUp(PointerEventData eventData)
		{
			this.SetAll(false);
		}

		// Token: 0x060023D1 RID: 9169 RVA: 0x0007CE25 File Offset: 0x0007B025
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.hoveringIndicator)
			{
				this.hoveringIndicator.SetActive(true);
			}
			if (!this.mute)
			{
				AudioManager.Post("UI/hover");
			}
		}

		// Token: 0x060023D2 RID: 9170 RVA: 0x0007CE53 File Offset: 0x0007B053
		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.hoveringIndicator)
			{
				this.hoveringIndicator.SetActive(false);
			}
		}

		// Token: 0x04001850 RID: 6224
		[SerializeField]
		private GameObject hoveringIndicator;

		// Token: 0x04001851 RID: 6225
		[SerializeField]
		private List<ToggleAnimation> toggles = new List<ToggleAnimation>();

		// Token: 0x04001852 RID: 6226
		[SerializeField]
		private bool mute;
	}
}
