using System;
using Duckov.MiniGames.GoldMiner.UI;
using TMPro;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x02000298 RID: 664
	public class BombDisplay : MonoBehaviour
	{
		// Token: 0x060015AE RID: 5550 RVA: 0x000504F8 File Offset: 0x0004E6F8
		private void Awake()
		{
			NavEntry navEntry = this.navEntry;
			navEntry.onInteract = (Action<NavEntry>)Delegate.Combine(navEntry.onInteract, new Action<NavEntry>(this.OnInteract));
			GoldMiner goldMiner = this.master;
			goldMiner.onEarlyLevelPlayTick = (Action<GoldMiner>)Delegate.Combine(goldMiner.onEarlyLevelPlayTick, new Action<GoldMiner>(this.OnEarlyLevelPlayTick));
		}

		// Token: 0x060015AF RID: 5551 RVA: 0x00050554 File Offset: 0x0004E754
		private void OnEarlyLevelPlayTick(GoldMiner miner)
		{
			if (this.master == null)
			{
				return;
			}
			if (this.master.run == null)
			{
				return;
			}
			this.amountText.text = string.Format("{0}", this.master.run.bomb);
		}

		// Token: 0x060015B0 RID: 5552 RVA: 0x000505A8 File Offset: 0x0004E7A8
		private void OnInteract(NavEntry entry)
		{
			this.master.UseBomb();
		}

		// Token: 0x0400100C RID: 4108
		[SerializeField]
		private GoldMiner master;

		// Token: 0x0400100D RID: 4109
		[SerializeField]
		private TextMeshProUGUI amountText;

		// Token: 0x0400100E RID: 4110
		[SerializeField]
		private NavEntry navEntry;
	}
}
