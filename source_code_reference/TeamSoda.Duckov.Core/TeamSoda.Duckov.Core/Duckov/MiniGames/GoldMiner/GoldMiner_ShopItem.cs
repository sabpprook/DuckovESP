using System;
using SodaCraft.Localizations;
using UnityEngine;
using UnityEngine.Events;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x02000292 RID: 658
	public class GoldMiner_ShopItem : MonoBehaviour
	{
		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x06001597 RID: 5527 RVA: 0x000500B6 File Offset: 0x0004E2B6
		public Sprite Icon
		{
			get
			{
				return this.icon;
			}
		}

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x06001598 RID: 5528 RVA: 0x000500BE File Offset: 0x0004E2BE
		public string DisplayNameKey
		{
			get
			{
				return this.displayNameKey;
			}
		}

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x06001599 RID: 5529 RVA: 0x000500C6 File Offset: 0x0004E2C6
		public string DisplayName
		{
			get
			{
				return this.displayNameKey.ToPlainText();
			}
		}

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x0600159A RID: 5530 RVA: 0x000500D3 File Offset: 0x0004E2D3
		public int BasePrice
		{
			get
			{
				return this.basePrice;
			}
		}

		// Token: 0x0600159B RID: 5531 RVA: 0x000500DB File Offset: 0x0004E2DB
		public void OnBought(GoldMiner target)
		{
			UnityEvent<GoldMiner> unityEvent = this.onBought;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(target);
		}

		// Token: 0x04000FFB RID: 4091
		[SerializeField]
		private Sprite icon;

		// Token: 0x04000FFC RID: 4092
		[LocalizationKey("Default")]
		[SerializeField]
		private string displayNameKey;

		// Token: 0x04000FFD RID: 4093
		[SerializeField]
		private int basePrice;

		// Token: 0x04000FFE RID: 4094
		public UnityEvent<GoldMiner> onBought;
	}
}
