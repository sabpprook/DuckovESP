using System;
using Duckov.Utilities;
using ItemStatsSystem;
using SodaCraft.Localizations;
using UnityEngine;

// Token: 0x020001EF RID: 495
public class InventoryFilterProvider : MonoBehaviour
{
	// Token: 0x04000C09 RID: 3081
	public InventoryFilterProvider.FilterEntry[] entries;

	// Token: 0x020004D9 RID: 1241
	[Serializable]
	public struct FilterEntry
	{
		// Token: 0x17000748 RID: 1864
		// (get) Token: 0x0600270C RID: 9996 RVA: 0x0008DB56 File Offset: 0x0008BD56
		public string DisplayName
		{
			get
			{
				return this.name.ToPlainText();
			}
		}

		// Token: 0x0600270D RID: 9997 RVA: 0x0008DB64 File Offset: 0x0008BD64
		private bool FilterFunction(Item item)
		{
			if (item == null)
			{
				return false;
			}
			if (this.requireTags.Length == 0)
			{
				return true;
			}
			foreach (Tag tag in this.requireTags)
			{
				if (!(tag == null) && item.Tags.Contains(tag))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600270E RID: 9998 RVA: 0x0008DBBA File Offset: 0x0008BDBA
		public Func<Item, bool> GetFunction()
		{
			if (this.requireTags.Length == 0)
			{
				return null;
			}
			return new Func<Item, bool>(this.FilterFunction);
		}

		// Token: 0x04001CFA RID: 7418
		[LocalizationKey("Default")]
		public string name;

		// Token: 0x04001CFB RID: 7419
		public Sprite icon;

		// Token: 0x04001CFC RID: 7420
		public Tag[] requireTags;
	}
}
