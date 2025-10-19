using System;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using UnityEngine;

namespace Duckov.PerkTrees
{
	// Token: 0x02000251 RID: 593
	public class PerkRelationNode : PerkRelationNodeBase
	{
		// Token: 0x06001286 RID: 4742 RVA: 0x00045E88 File Offset: 0x00044088
		internal void SetDirty()
		{
			this.dirty = true;
		}

		// Token: 0x06001287 RID: 4743 RVA: 0x00045E94 File Offset: 0x00044094
		public override void OnDestroy()
		{
			if (this.relatedNode == null)
			{
				return;
			}
			IEnumerable<Node> enumerable = base.graph.allNodes.Where((Node e) => (e as PerkRelationNode).relatedNode == this.relatedNode);
			if (enumerable.Count<Node>() <= 2)
			{
				foreach (Node node in enumerable)
				{
					PerkRelationNode perkRelationNode = node as PerkRelationNode;
					if (perkRelationNode != null)
					{
						perkRelationNode.isDuplicate = false;
						perkRelationNode.SetDirty();
					}
				}
			}
		}

		// Token: 0x06001288 RID: 4744 RVA: 0x00045F20 File Offset: 0x00044120
		internal void NotifyIncomingStateChanged()
		{
			this.relatedNode.NotifyParentStateChanged();
		}

		// Token: 0x04000E0E RID: 3598
		public Perk relatedNode;

		// Token: 0x04000E0F RID: 3599
		public Vector2 cachedPosition;

		// Token: 0x04000E10 RID: 3600
		private bool dirty = true;

		// Token: 0x04000E11 RID: 3601
		internal bool isDuplicate;

		// Token: 0x04000E12 RID: 3602
		internal bool isInvalid;
	}
}
