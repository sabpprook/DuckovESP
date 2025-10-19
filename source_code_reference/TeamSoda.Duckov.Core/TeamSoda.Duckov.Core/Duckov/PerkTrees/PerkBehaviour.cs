using System;
using UnityEngine;

namespace Duckov.PerkTrees
{
	// Token: 0x02000249 RID: 585
	[RequireComponent(typeof(Perk))]
	public abstract class PerkBehaviour : MonoBehaviour
	{
		// Token: 0x1700033E RID: 830
		// (get) Token: 0x0600123F RID: 4671 RVA: 0x000455AA File Offset: 0x000437AA
		protected Perk Master
		{
			get
			{
				return this.master;
			}
		}

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x06001240 RID: 4672 RVA: 0x000455B2 File Offset: 0x000437B2
		private bool Unlocked
		{
			get
			{
				return !(this.master == null) && this.master.Unlocked;
			}
		}

		// Token: 0x17000340 RID: 832
		// (get) Token: 0x06001241 RID: 4673 RVA: 0x000455CF File Offset: 0x000437CF
		public virtual string Description
		{
			get
			{
				return "";
			}
		}

		// Token: 0x06001242 RID: 4674 RVA: 0x000455D8 File Offset: 0x000437D8
		private void Awake()
		{
			if (this.master == null)
			{
				this.master = base.GetComponent<Perk>();
			}
			this.master.onUnlockStateChanged += this.OnMasterUnlockStateChanged;
			LevelManager.OnLevelInitialized += this.OnLevelInitialized;
			if (LevelManager.LevelInited)
			{
				this.OnLevelInitialized();
			}
			this.OnAwake();
		}

		// Token: 0x06001243 RID: 4675 RVA: 0x0004563A File Offset: 0x0004383A
		private void OnLevelInitialized()
		{
			this.NotifyUnlockStateChanged(this.Unlocked);
		}

		// Token: 0x06001244 RID: 4676 RVA: 0x00045648 File Offset: 0x00043848
		private void OnDestroy()
		{
			this.OnOnDestroy();
			if (this.master == null)
			{
				return;
			}
			this.master.onUnlockStateChanged -= this.OnMasterUnlockStateChanged;
			LevelManager.OnLevelInitialized -= this.OnLevelInitialized;
		}

		// Token: 0x06001245 RID: 4677 RVA: 0x00045687 File Offset: 0x00043887
		private void OnValidate()
		{
			if (this.master == null)
			{
				this.master = base.GetComponent<Perk>();
			}
		}

		// Token: 0x06001246 RID: 4678 RVA: 0x000456A3 File Offset: 0x000438A3
		private void OnMasterUnlockStateChanged(Perk perk, bool unlocked)
		{
			if (perk != this.master)
			{
				Debug.LogError("Perk对象不匹配");
			}
			this.NotifyUnlockStateChanged(unlocked);
		}

		// Token: 0x06001247 RID: 4679 RVA: 0x000456C4 File Offset: 0x000438C4
		private void NotifyUnlockStateChanged(bool unlocked)
		{
			this.OnUnlockStateChanged(unlocked);
			if (unlocked)
			{
				this.OnUnlocked();
				return;
			}
			this.OnLocked();
		}

		// Token: 0x06001248 RID: 4680 RVA: 0x000456DD File Offset: 0x000438DD
		protected virtual void OnUnlockStateChanged(bool unlocked)
		{
		}

		// Token: 0x06001249 RID: 4681 RVA: 0x000456DF File Offset: 0x000438DF
		protected virtual void OnUnlocked()
		{
		}

		// Token: 0x0600124A RID: 4682 RVA: 0x000456E1 File Offset: 0x000438E1
		protected virtual void OnLocked()
		{
		}

		// Token: 0x0600124B RID: 4683 RVA: 0x000456E3 File Offset: 0x000438E3
		protected virtual void OnAwake()
		{
		}

		// Token: 0x0600124C RID: 4684 RVA: 0x000456E5 File Offset: 0x000438E5
		protected virtual void OnOnDestroy()
		{
		}

		// Token: 0x04000DFF RID: 3583
		private Perk master;
	}
}
