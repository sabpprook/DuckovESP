using System;
using System.Collections.Generic;
using NodeCanvas.Framework;

namespace Duckov.PerkTrees
{
	// Token: 0x02000253 RID: 595
	public class PerkTreeRelationGraphOwner : GraphOwner<PerkRelationGraph>
	{
		// Token: 0x1700035E RID: 862
		// (get) Token: 0x06001293 RID: 4755 RVA: 0x00045F7C File Offset: 0x0004417C
		public PerkRelationGraph RelationGraph
		{
			get
			{
				if (this._relationGraph == null)
				{
					this._relationGraph = this.graph as PerkRelationGraph;
				}
				return this._relationGraph;
			}
		}

		// Token: 0x06001294 RID: 4756 RVA: 0x00045FA4 File Offset: 0x000441A4
		public List<Perk> GetRequiredNodes(Perk node)
		{
			PerkRelationNode relatedNode = this.RelationGraph.GetRelatedNode(node);
			if (relatedNode == null)
			{
				return null;
			}
			List<PerkRelationNode> incomingNodes = this.RelationGraph.GetIncomingNodes(relatedNode);
			if (incomingNodes == null)
			{
				return null;
			}
			if (incomingNodes.Count < 1)
			{
				return null;
			}
			List<Perk> list = new List<Perk>();
			foreach (PerkRelationNode perkRelationNode in incomingNodes)
			{
				Perk relatedNode2 = perkRelationNode.relatedNode;
				if (!(relatedNode2 == null))
				{
					list.Add(relatedNode2);
				}
			}
			return list;
		}

		// Token: 0x06001295 RID: 4757 RVA: 0x0004603C File Offset: 0x0004423C
		internal PerkRelationNode GetRelatedNode(Perk perk)
		{
			if (this.RelationGraph == null)
			{
				return null;
			}
			return this.RelationGraph.GetRelatedNode(perk);
		}

		// Token: 0x04000E13 RID: 3603
		private PerkRelationGraph _relationGraph;
	}
}
