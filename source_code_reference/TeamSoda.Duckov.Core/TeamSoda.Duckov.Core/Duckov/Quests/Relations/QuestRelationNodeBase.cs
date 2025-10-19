using System;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace Duckov.Quests.Relations
{
	// Token: 0x0200035B RID: 859
	public class QuestRelationNodeBase : Node
	{
		// Token: 0x170005D1 RID: 1489
		// (get) Token: 0x06001E0C RID: 7692 RVA: 0x0006A0EA File Offset: 0x000682EA
		public override int maxInConnections
		{
			get
			{
				return 64;
			}
		}

		// Token: 0x170005D2 RID: 1490
		// (get) Token: 0x06001E0D RID: 7693 RVA: 0x0006A0EE File Offset: 0x000682EE
		public override int maxOutConnections
		{
			get
			{
				return 64;
			}
		}

		// Token: 0x170005D3 RID: 1491
		// (get) Token: 0x06001E0E RID: 7694 RVA: 0x0006A0F2 File Offset: 0x000682F2
		public override Type outConnectionType
		{
			get
			{
				return typeof(QuestRelationConnection);
			}
		}

		// Token: 0x170005D4 RID: 1492
		// (get) Token: 0x06001E0F RID: 7695 RVA: 0x0006A0FE File Offset: 0x000682FE
		public override bool allowAsPrime
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170005D5 RID: 1493
		// (get) Token: 0x06001E10 RID: 7696 RVA: 0x0006A101 File Offset: 0x00068301
		public override bool canSelfConnect
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170005D6 RID: 1494
		// (get) Token: 0x06001E11 RID: 7697 RVA: 0x0006A104 File Offset: 0x00068304
		public override Alignment2x2 commentsAlignment
		{
			get
			{
				return Alignment2x2.Default;
			}
		}

		// Token: 0x170005D7 RID: 1495
		// (get) Token: 0x06001E12 RID: 7698 RVA: 0x0006A107 File Offset: 0x00068307
		public override Alignment2x2 iconAlignment
		{
			get
			{
				return Alignment2x2.Default;
			}
		}
	}
}
