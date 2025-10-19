using System;
using Duckov.MiniGames.GoldMiner.UI;
using TMPro;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x020002A8 RID: 680
	public class StrengthPotionDisplay : MonoBehaviour
	{
		// Token: 0x06001618 RID: 5656 RVA: 0x000518D8 File Offset: 0x0004FAD8
		private void Awake()
		{
			NavEntry navEntry = this.navEntry;
			navEntry.onInteract = (Action<NavEntry>)Delegate.Combine(navEntry.onInteract, new Action<NavEntry>(this.OnInteract));
			GoldMiner goldMiner = this.master;
			goldMiner.onEarlyLevelPlayTick = (Action<GoldMiner>)Delegate.Combine(goldMiner.onEarlyLevelPlayTick, new Action<GoldMiner>(this.OnEarlyLevelPlayTick));
		}

		// Token: 0x06001619 RID: 5657 RVA: 0x00051934 File Offset: 0x0004FB34
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
			this.amountText.text = string.Format("{0}", this.master.run.strengthPotion);
		}

		// Token: 0x0600161A RID: 5658 RVA: 0x00051988 File Offset: 0x0004FB88
		private void OnInteract(NavEntry entry)
		{
			this.master.UseStrengthPotion();
		}

		// Token: 0x04001069 RID: 4201
		[SerializeField]
		private GoldMiner master;

		// Token: 0x0400106A RID: 4202
		[SerializeField]
		private TextMeshProUGUI amountText;

		// Token: 0x0400106B RID: 4203
		[SerializeField]
		private NavEntry navEntry;
	}
}
