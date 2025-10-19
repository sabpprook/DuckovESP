using System;
using SodaCraft.Localizations;
using UnityEngine;

namespace Duckov.NoteIndexs
{
	// Token: 0x02000263 RID: 611
	[Serializable]
	public class Note
	{
		// Token: 0x17000373 RID: 883
		// (get) Token: 0x06001305 RID: 4869 RVA: 0x000470F6 File Offset: 0x000452F6
		// (set) Token: 0x06001306 RID: 4870 RVA: 0x0004710D File Offset: 0x0004530D
		[LocalizationKey("Default")]
		public string titleKey
		{
			get
			{
				return "Note_" + this.key + "_Title";
			}
			set
			{
			}
		}

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x06001307 RID: 4871 RVA: 0x0004710F File Offset: 0x0004530F
		// (set) Token: 0x06001308 RID: 4872 RVA: 0x00047126 File Offset: 0x00045326
		[LocalizationKey("Default")]
		public string contentKey
		{
			get
			{
				return "Note_" + this.key + "_Content";
			}
			set
			{
			}
		}

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x06001309 RID: 4873 RVA: 0x00047128 File Offset: 0x00045328
		public string Title
		{
			get
			{
				return this.titleKey.ToPlainText();
			}
		}

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x0600130A RID: 4874 RVA: 0x00047135 File Offset: 0x00045335
		private Sprite previewSprite
		{
			get
			{
				return this.image;
			}
		}

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x0600130B RID: 4875 RVA: 0x0004713D File Offset: 0x0004533D
		public string Content
		{
			get
			{
				return this.contentKey.ToPlainText();
			}
		}

		// Token: 0x04000E3C RID: 3644
		[SerializeField]
		public string key;

		// Token: 0x04000E3D RID: 3645
		[SerializeField]
		public Sprite image;
	}
}
