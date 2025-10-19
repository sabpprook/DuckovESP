using System;
using UnityEngine;

namespace Duckov.PerkTrees
{
	// Token: 0x0200024E RID: 590
	public class PerkLevelLineNode : PerkRelationNodeBase
	{
		// Token: 0x1700034D RID: 845
		// (get) Token: 0x06001276 RID: 4726 RVA: 0x00045D33 File Offset: 0x00043F33
		public string DisplayName
		{
			get
			{
				return this.name;
			}
		}

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x06001277 RID: 4727 RVA: 0x00045D3B File Offset: 0x00043F3B
		public override int maxInConnections
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x06001278 RID: 4728 RVA: 0x00045D3E File Offset: 0x00043F3E
		public override int maxOutConnections
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x04000E0D RID: 3597
		public Vector2 cachedPosition;
	}
}
