using System;
using Duckov.MiniGames.GoldMiner.UI;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x0200029C RID: 668
	public class GoldMinerShopUIContinueBtn : MonoBehaviour
	{
		// Token: 0x060015C2 RID: 5570 RVA: 0x00050864 File Offset: 0x0004EA64
		private void Awake()
		{
			if (!this.navEntry)
			{
				this.navEntry = base.GetComponent<NavEntry>();
			}
			NavEntry navEntry = this.navEntry;
			navEntry.onInteract = (Action<NavEntry>)Delegate.Combine(navEntry.onInteract, new Action<NavEntry>(this.OnInteract));
		}

		// Token: 0x060015C3 RID: 5571 RVA: 0x000508B1 File Offset: 0x0004EAB1
		private void OnInteract(NavEntry entry)
		{
			this.shop.Continue();
		}

		// Token: 0x0400101C RID: 4124
		[SerializeField]
		private GoldMinerShop shop;

		// Token: 0x0400101D RID: 4125
		[SerializeField]
		private NavEntry navEntry;
	}
}
