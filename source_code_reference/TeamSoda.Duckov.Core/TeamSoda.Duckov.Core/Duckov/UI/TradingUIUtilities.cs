using System;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x020003AC RID: 940
	public static class TradingUIUtilities
	{
		// Token: 0x17000675 RID: 1653
		// (get) Token: 0x060021BE RID: 8638 RVA: 0x0007589A File Offset: 0x00073A9A
		// (set) Token: 0x060021BF RID: 8639 RVA: 0x000758A6 File Offset: 0x00073AA6
		public static IMerchant ActiveMerchant
		{
			get
			{
				return TradingUIUtilities.activeMerchant as IMerchant;
			}
			set
			{
				TradingUIUtilities.activeMerchant = value as global::UnityEngine.Object;
				Action<IMerchant> onActiveMerchantChanged = TradingUIUtilities.OnActiveMerchantChanged;
				if (onActiveMerchantChanged == null)
				{
					return;
				}
				onActiveMerchantChanged(value);
			}
		}

		// Token: 0x140000E7 RID: 231
		// (add) Token: 0x060021C0 RID: 8640 RVA: 0x000758C4 File Offset: 0x00073AC4
		// (remove) Token: 0x060021C1 RID: 8641 RVA: 0x000758F8 File Offset: 0x00073AF8
		public static event Action<IMerchant> OnActiveMerchantChanged;

		// Token: 0x040016D4 RID: 5844
		private static global::UnityEngine.Object activeMerchant;
	}
}
