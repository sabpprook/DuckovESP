using System;
using Duckov.PerkTrees;
using Duckov.Utilities;
using ItemStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x020003B9 RID: 953
	public class RequireItemEntry : MonoBehaviour, IPoolable
	{
		// Token: 0x06002294 RID: 8852 RVA: 0x00078F13 File Offset: 0x00077113
		public void NotifyPooled()
		{
		}

		// Token: 0x06002295 RID: 8853 RVA: 0x00078F15 File Offset: 0x00077115
		public void NotifyReleased()
		{
		}

		// Token: 0x06002296 RID: 8854 RVA: 0x00078F18 File Offset: 0x00077118
		public void Setup(PerkRequirement.RequireItemEntry target)
		{
			int id = target.id;
			ItemMetaData metaData = ItemAssetsCollection.GetMetaData(id);
			this.icon.sprite = metaData.icon;
			string displayName = metaData.DisplayName;
			int itemCount = ItemUtilities.GetItemCount(id);
			this.text.text = string.Format(this.textFormat, displayName, target.amount, itemCount);
		}

		// Token: 0x04001792 RID: 6034
		[SerializeField]
		private Image icon;

		// Token: 0x04001793 RID: 6035
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x04001794 RID: 6036
		[SerializeField]
		private string textFormat = "{0} x{1}";
	}
}
