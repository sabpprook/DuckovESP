using System;
using System.Text;
using Duckov.Utilities;

namespace ItemStatsSystem
{
	// Token: 0x02000006 RID: 6
	[Serializable]
	public struct ItemFilter
	{
		// Token: 0x0600001F RID: 31 RVA: 0x0000293D File Offset: 0x00000B3D
		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002950 File Offset: 0x00000B50
		public override string ToString()
		{
			ItemFilter.sb.Clear();
			ItemFilter.sb.AppendLine("R");
			if (this.requireTags != null)
			{
				foreach (Tag tag in this.requireTags)
				{
					if (!(tag == null))
					{
						ItemFilter.sb.AppendLine(tag.name);
					}
				}
			}
			ItemFilter.sb.AppendLine("E");
			if (this.excludeTags != null)
			{
				foreach (Tag tag2 in this.excludeTags)
				{
					if (!(tag2 == null))
					{
						ItemFilter.sb.AppendLine(tag2.name);
					}
				}
			}
			ItemFilter.sb.AppendLine("MinQ");
			ItemFilter.sb.AppendLine(this.minQuality.ToString());
			ItemFilter.sb.AppendLine("MaxQ");
			ItemFilter.sb.AppendLine(this.maxQuality.ToString());
			ItemFilter.sb.AppendLine("CALIBER");
			ItemFilter.sb.AppendLine(this.caliber);
			return ItemFilter.sb.ToString();
		}

		// Token: 0x0400000E RID: 14
		public Tag[] requireTags;

		// Token: 0x0400000F RID: 15
		public Tag[] excludeTags;

		// Token: 0x04000010 RID: 16
		public int minQuality;

		// Token: 0x04000011 RID: 17
		public int maxQuality;

		// Token: 0x04000012 RID: 18
		public string caliber;

		// Token: 0x04000013 RID: 19
		private static StringBuilder sb = new StringBuilder();
	}
}
