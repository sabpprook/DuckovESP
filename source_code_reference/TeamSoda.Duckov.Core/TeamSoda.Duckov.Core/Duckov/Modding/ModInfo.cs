using System;
using System.IO;
using UnityEngine;

namespace Duckov.Modding
{
	// Token: 0x02000269 RID: 617
	[Serializable]
	public struct ModInfo
	{
		// Token: 0x1700037D RID: 893
		// (get) Token: 0x0600133A RID: 4922 RVA: 0x00047BD7 File Offset: 0x00045DD7
		public string dllPath
		{
			get
			{
				return Path.Combine(this.path, this.name + ".dll");
			}
		}

		// Token: 0x04000E4C RID: 3660
		public string path;

		// Token: 0x04000E4D RID: 3661
		public string name;

		// Token: 0x04000E4E RID: 3662
		public string displayName;

		// Token: 0x04000E4F RID: 3663
		public string description;

		// Token: 0x04000E50 RID: 3664
		public Texture2D preview;

		// Token: 0x04000E51 RID: 3665
		public bool dllFound;

		// Token: 0x04000E52 RID: 3666
		public bool isSteamItem;

		// Token: 0x04000E53 RID: 3667
		public ulong publishedFileId;
	}
}
