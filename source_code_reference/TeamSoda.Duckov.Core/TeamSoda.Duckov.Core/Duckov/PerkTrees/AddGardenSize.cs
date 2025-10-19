using System;
using Duckov.Crops;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.PerkTrees
{
	// Token: 0x0200024A RID: 586
	public class AddGardenSize : PerkBehaviour, IGardenSizeAdder
	{
		// Token: 0x17000341 RID: 833
		// (get) Token: 0x0600124E RID: 4686 RVA: 0x000456EF File Offset: 0x000438EF
		public override string Description
		{
			get
			{
				return this.descriptionFormatKey.ToPlainText().Format(new
				{
					addX = this.add.x,
					addY = this.add.y
				});
			}
		}

		// Token: 0x0600124F RID: 4687 RVA: 0x0004571C File Offset: 0x0004391C
		protected override void OnUnlocked()
		{
			Garden.Register(this);
		}

		// Token: 0x06001250 RID: 4688 RVA: 0x00045724 File Offset: 0x00043924
		protected override void OnOnDestroy()
		{
			Garden.Unregister(this);
		}

		// Token: 0x06001251 RID: 4689 RVA: 0x0004572C File Offset: 0x0004392C
		public Vector2Int GetValue(string gardenID)
		{
			if (gardenID != this.gardenID)
			{
				return default(Vector2Int);
			}
			return this.add;
		}

		// Token: 0x04000E00 RID: 3584
		[LocalizationKey("Default")]
		[SerializeField]
		private string descriptionFormatKey = "PerkBehaviour_AddGardenSize";

		// Token: 0x04000E01 RID: 3585
		[SerializeField]
		private string gardenID = "Default";

		// Token: 0x04000E02 RID: 3586
		[SerializeField]
		private Vector2Int add;
	}
}
