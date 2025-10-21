using System;
using Duckov.Crops;
using SodaCraft.Localizations;
using UnityEngine;

namespace Duckov.PerkTrees
{
	// Token: 0x0200024C RID: 588
	public class GardenAutoWater : PerkBehaviour, IGardenAutoWaterProvider
	{
		// Token: 0x17000344 RID: 836
		// (get) Token: 0x0600125A RID: 4698 RVA: 0x000457F6 File Offset: 0x000439F6
		public override string Description
		{
			get
			{
				return this.descriptionKey.ToPlainText();
			}
		}

		// Token: 0x0600125B RID: 4699 RVA: 0x00045803 File Offset: 0x00043A03
		protected override void OnUnlocked()
		{
			Garden.Register(this);
		}

		// Token: 0x0600125C RID: 4700 RVA: 0x0004580B File Offset: 0x00043A0B
		protected override void OnOnDestroy()
		{
			Garden.Unregister(this);
		}

		// Token: 0x0600125D RID: 4701 RVA: 0x00045813 File Offset: 0x00043A13
		public bool TakeEffect(string gardenID)
		{
			return gardenID == this.gardenID;
		}

		// Token: 0x04000E04 RID: 3588
		[SerializeField]
		[LocalizationKey("Default")]
		private string descriptionKey = "PerkBehaviour_GardenAutoWater";

		// Token: 0x04000E05 RID: 3589
		[SerializeField]
		private string gardenID = "Default";
	}
}
