using System;
using UnityEngine;

namespace Duckov.UI.Animations
{
	// Token: 0x020003E1 RID: 993
	public class ToggleComponent : MonoBehaviour
	{
		// Token: 0x170006D1 RID: 1745
		// (get) Token: 0x060023E6 RID: 9190 RVA: 0x0007D177 File Offset: 0x0007B377
		private bool Status
		{
			get
			{
				return this.master && this.master.Status;
			}
		}

		// Token: 0x060023E7 RID: 9191 RVA: 0x0007D193 File Offset: 0x0007B393
		private void Awake()
		{
			if (this.master == null)
			{
				this.master = base.GetComponent<ToggleAnimation>();
			}
			this.master.onSetToggle += this.OnSetToggle;
		}

		// Token: 0x060023E8 RID: 9192 RVA: 0x0007D1C7 File Offset: 0x0007B3C7
		private void OnDestroy()
		{
			if (this.master == null)
			{
				return;
			}
			this.master.onSetToggle -= this.OnSetToggle;
		}

		// Token: 0x060023E9 RID: 9193 RVA: 0x0007D1F0 File Offset: 0x0007B3F0
		protected virtual void OnSetToggle(ToggleAnimation master, bool value)
		{
		}

		// Token: 0x04001865 RID: 6245
		[SerializeField]
		private ToggleAnimation master;
	}
}
