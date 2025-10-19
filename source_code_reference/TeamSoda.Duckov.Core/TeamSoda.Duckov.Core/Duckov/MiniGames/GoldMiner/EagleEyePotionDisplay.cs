using System;
using Duckov.MiniGames.GoldMiner.UI;
using TMPro;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x02000299 RID: 665
	public class EagleEyePotionDisplay : MonoBehaviour
	{
		// Token: 0x060015B2 RID: 5554 RVA: 0x000505C0 File Offset: 0x0004E7C0
		private void Awake()
		{
			NavEntry navEntry = this.navEntry;
			navEntry.onInteract = (Action<NavEntry>)Delegate.Combine(navEntry.onInteract, new Action<NavEntry>(this.OnInteract));
			GoldMiner goldMiner = this.master;
			goldMiner.onEarlyLevelPlayTick = (Action<GoldMiner>)Delegate.Combine(goldMiner.onEarlyLevelPlayTick, new Action<GoldMiner>(this.OnEarlyLevelPlayTick));
		}

		// Token: 0x060015B3 RID: 5555 RVA: 0x0005061C File Offset: 0x0004E81C
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
			this.amountText.text = string.Format("{0}", this.master.run.eagleEyePotion);
		}

		// Token: 0x060015B4 RID: 5556 RVA: 0x00050670 File Offset: 0x0004E870
		private void OnInteract(NavEntry entry)
		{
			this.master.UseEagleEyePotion();
		}

		// Token: 0x0400100F RID: 4111
		[SerializeField]
		private GoldMiner master;

		// Token: 0x04001010 RID: 4112
		[SerializeField]
		private TextMeshProUGUI amountText;

		// Token: 0x04001011 RID: 4113
		[SerializeField]
		private NavEntry navEntry;
	}
}
