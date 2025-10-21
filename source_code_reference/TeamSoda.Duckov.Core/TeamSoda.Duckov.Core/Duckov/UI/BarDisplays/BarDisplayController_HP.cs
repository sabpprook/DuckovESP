using System;
using UnityEngine.Events;

namespace Duckov.UI.BarDisplays
{
	// Token: 0x020003CA RID: 970
	public class BarDisplayController_HP : BarDisplayController
	{
		// Token: 0x170006AF RID: 1711
		// (get) Token: 0x06002339 RID: 9017 RVA: 0x0007B49A File Offset: 0x0007969A
		protected override float Current
		{
			get
			{
				if (this.Target == null)
				{
					return 0f;
				}
				return this.Target.Health.CurrentHealth;
			}
		}

		// Token: 0x170006B0 RID: 1712
		// (get) Token: 0x0600233A RID: 9018 RVA: 0x0007B4C0 File Offset: 0x000796C0
		protected override float Max
		{
			get
			{
				if (this.Target == null)
				{
					return 0f;
				}
				return this.Target.Health.MaxHealth;
			}
		}

		// Token: 0x0600233B RID: 9019 RVA: 0x0007B4E6 File Offset: 0x000796E6
		private void OnEnable()
		{
			base.Refresh();
			this.RegisterEvents();
		}

		// Token: 0x0600233C RID: 9020 RVA: 0x0007B4F4 File Offset: 0x000796F4
		private void OnDisable()
		{
			this.UnregisterEvents();
		}

		// Token: 0x0600233D RID: 9021 RVA: 0x0007B4FC File Offset: 0x000796FC
		private void RegisterEvents()
		{
			if (this.Target == null)
			{
				return;
			}
			this.Target.Health.OnHealthChange.AddListener(new UnityAction<Health>(this.OnHealthChange));
		}

		// Token: 0x0600233E RID: 9022 RVA: 0x0007B52E File Offset: 0x0007972E
		private void UnregisterEvents()
		{
			if (this.Target == null)
			{
				return;
			}
			this.Target.Health.OnHealthChange.RemoveListener(new UnityAction<Health>(this.OnHealthChange));
		}

		// Token: 0x170006B1 RID: 1713
		// (get) Token: 0x0600233F RID: 9023 RVA: 0x0007B560 File Offset: 0x00079760
		private CharacterMainControl Target
		{
			get
			{
				if (this._target == null)
				{
					this._target = CharacterMainControl.Main;
				}
				return this._target;
			}
		}

		// Token: 0x06002340 RID: 9024 RVA: 0x0007B581 File Offset: 0x00079781
		private void OnHealthChange(Health health)
		{
			base.Refresh();
		}

		// Token: 0x040017F9 RID: 6137
		private CharacterMainControl _target;
	}
}
