using System;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace Duckov.PerkTrees
{
	// Token: 0x02000252 RID: 594
	public class PerkRelationNodeBase : Node
	{
		// Token: 0x17000357 RID: 855
		// (get) Token: 0x0600128B RID: 4747 RVA: 0x00045F54 File Offset: 0x00044154
		public override int maxInConnections
		{
			get
			{
				return 16;
			}
		}

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x0600128C RID: 4748 RVA: 0x00045F58 File Offset: 0x00044158
		public override int maxOutConnections
		{
			get
			{
				return 16;
			}
		}

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x0600128D RID: 4749 RVA: 0x00045F5C File Offset: 0x0004415C
		public override Type outConnectionType
		{
			get
			{
				return typeof(PerkRelationConnection);
			}
		}

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x0600128E RID: 4750 RVA: 0x00045F68 File Offset: 0x00044168
		public override bool allowAsPrime
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x0600128F RID: 4751 RVA: 0x00045F6B File Offset: 0x0004416B
		public override bool canSelfConnect
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x06001290 RID: 4752 RVA: 0x00045F6E File Offset: 0x0004416E
		public override Alignment2x2 commentsAlignment
		{
			get
			{
				return Alignment2x2.Default;
			}
		}

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x06001291 RID: 4753 RVA: 0x00045F71 File Offset: 0x00044171
		public override Alignment2x2 iconAlignment
		{
			get
			{
				return Alignment2x2.Default;
			}
		}
	}
}
