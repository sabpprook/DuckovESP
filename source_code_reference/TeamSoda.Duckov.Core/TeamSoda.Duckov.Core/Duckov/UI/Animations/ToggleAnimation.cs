using System;
using UnityEngine;

namespace Duckov.UI.Animations
{
	// Token: 0x020003E0 RID: 992
	public class ToggleAnimation : MonoBehaviour
	{
		// Token: 0x140000F0 RID: 240
		// (add) Token: 0x060023DF RID: 9183 RVA: 0x0007D0C0 File Offset: 0x0007B2C0
		// (remove) Token: 0x060023E0 RID: 9184 RVA: 0x0007D0F8 File Offset: 0x0007B2F8
		public event Action<ToggleAnimation, bool> onSetToggle;

		// Token: 0x170006D0 RID: 1744
		// (get) Token: 0x060023E1 RID: 9185 RVA: 0x0007D12D File Offset: 0x0007B32D
		// (set) Token: 0x060023E2 RID: 9186 RVA: 0x0007D135 File Offset: 0x0007B335
		public bool Status
		{
			get
			{
				return this.status;
			}
			protected set
			{
				this.SetToggle(value);
			}
		}

		// Token: 0x060023E3 RID: 9187 RVA: 0x0007D13E File Offset: 0x0007B33E
		public void SetToggle(bool value)
		{
			this.status = value;
			if (!Application.isPlaying)
			{
				return;
			}
			this.OnSetToggle(this.Status);
			Action<ToggleAnimation, bool> action = this.onSetToggle;
			if (action == null)
			{
				return;
			}
			action(this, value);
		}

		// Token: 0x060023E4 RID: 9188 RVA: 0x0007D16D File Offset: 0x0007B36D
		protected virtual void OnSetToggle(bool value)
		{
		}

		// Token: 0x04001864 RID: 6244
		[SerializeField]
		[HideInInspector]
		private bool status;
	}
}
