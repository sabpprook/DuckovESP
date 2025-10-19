using System;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x0200028F RID: 655
	[Serializable]
	public class ShopEntity
	{
		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x06001567 RID: 5479 RVA: 0x0004F4C0 File Offset: 0x0004D6C0
		public string ID
		{
			get
			{
				if (!this.artifact)
				{
					return null;
				}
				return this.artifact.ID;
			}
		}

		// Token: 0x04000FD5 RID: 4053
		public GoldMinerArtifact artifact;

		// Token: 0x04000FD6 RID: 4054
		public bool locked;

		// Token: 0x04000FD7 RID: 4055
		public bool sold;

		// Token: 0x04000FD8 RID: 4056
		public float priceFactor = 1f;
	}
}
