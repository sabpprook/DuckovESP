using System;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x020002A3 RID: 675
	public class NavGroupLinearController : MiniGameBehaviour
	{
		// Token: 0x060015F4 RID: 5620 RVA: 0x000510C8 File Offset: 0x0004F2C8
		private void Awake()
		{
			GoldMiner goldMiner = this.master;
			goldMiner.onLevelBegin = (Action<GoldMiner>)Delegate.Combine(goldMiner.onLevelBegin, new Action<GoldMiner>(this.OnLevelBegin));
			NavGroup.OnNavGroupChanged = (Action)Delegate.Combine(NavGroup.OnNavGroupChanged, new Action(this.OnNavGroupChanged));
		}

		// Token: 0x060015F5 RID: 5621 RVA: 0x0005111C File Offset: 0x0004F31C
		private void OnLevelBegin(GoldMiner miner)
		{
			if (this.setActiveWhenLevelBegin)
			{
				this.navGroup.SetAsActiveNavGroup();
			}
		}

		// Token: 0x060015F6 RID: 5622 RVA: 0x00051131 File Offset: 0x0004F331
		private void OnNavGroupChanged()
		{
			this.changeLock = true;
		}

		// Token: 0x0400104C RID: 4172
		[SerializeField]
		private GoldMiner master;

		// Token: 0x0400104D RID: 4173
		[SerializeField]
		private NavGroup navGroup;

		// Token: 0x0400104E RID: 4174
		[SerializeField]
		private NavGroup otherNavGroup;

		// Token: 0x0400104F RID: 4175
		[SerializeField]
		private bool setActiveWhenLevelBegin;

		// Token: 0x04001050 RID: 4176
		private bool changeLock;
	}
}
