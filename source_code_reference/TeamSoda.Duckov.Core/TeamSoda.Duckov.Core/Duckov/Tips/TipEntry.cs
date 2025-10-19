using System;
using SodaCraft.Localizations;
using UnityEngine;

namespace Duckov.Tips
{
	// Token: 0x02000244 RID: 580
	[Serializable]
	internal struct TipEntry
	{
		// Token: 0x17000329 RID: 809
		// (get) Token: 0x06001209 RID: 4617 RVA: 0x00044C27 File Offset: 0x00042E27
		public string TipID
		{
			get
			{
				return this.tipID;
			}
		}

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x0600120A RID: 4618 RVA: 0x00044C2F File Offset: 0x00042E2F
		// (set) Token: 0x0600120B RID: 4619 RVA: 0x00044C41 File Offset: 0x00042E41
		[LocalizationKey("Default")]
		public string DescriptionKey
		{
			get
			{
				return "Tips_" + this.tipID;
			}
			set
			{
			}
		}

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x0600120C RID: 4620 RVA: 0x00044C43 File Offset: 0x00042E43
		public string Description
		{
			get
			{
				return this.DescriptionKey.ToPlainText();
			}
		}

		// Token: 0x04000DE0 RID: 3552
		[SerializeField]
		private string tipID;
	}
}
