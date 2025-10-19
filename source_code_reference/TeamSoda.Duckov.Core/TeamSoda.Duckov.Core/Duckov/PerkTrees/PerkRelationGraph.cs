using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace Duckov.PerkTrees
{
	// Token: 0x02000250 RID: 592
	public class PerkRelationGraph : Graph
	{
		// Token: 0x17000350 RID: 848
		// (get) Token: 0x0600127B RID: 4731 RVA: 0x00045D51 File Offset: 0x00043F51
		public override Type baseNodeType
		{
			get
			{
				return typeof(PerkRelationNodeBase);
			}
		}

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x0600127C RID: 4732 RVA: 0x00045D5D File Offset: 0x00043F5D
		public override bool requiresAgent
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x0600127D RID: 4733 RVA: 0x00045D60 File Offset: 0x00043F60
		public override bool requiresPrimeNode
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x0600127E RID: 4734 RVA: 0x00045D63 File Offset: 0x00043F63
		public override bool isTree
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x0600127F RID: 4735 RVA: 0x00045D66 File Offset: 0x00043F66
		public override PlanarDirection flowDirection
		{
			get
			{
				return PlanarDirection.Vertical;
			}
		}

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x06001280 RID: 4736 RVA: 0x00045D69 File Offset: 0x00043F69
		public override bool allowBlackboardOverrides
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000356 RID: 854
		// (get) Token: 0x06001281 RID: 4737 RVA: 0x00045D6C File Offset: 0x00043F6C
		public override bool canAcceptVariableDrops
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001282 RID: 4738 RVA: 0x00045D70 File Offset: 0x00043F70
		public PerkRelationNode GetRelatedNode(Perk perk)
		{
			return base.allNodes.Find(delegate(Node node)
			{
				if (node == null)
				{
					return false;
				}
				PerkRelationNode perkRelationNode = node as PerkRelationNode;
				return perkRelationNode != null && perkRelationNode.relatedNode == perk;
			}) as PerkRelationNode;
		}

		// Token: 0x06001283 RID: 4739 RVA: 0x00045DA8 File Offset: 0x00043FA8
		public List<PerkRelationNode> GetIncomingNodes(PerkRelationNode skillTreeNode)
		{
			List<PerkRelationNode> list = new List<PerkRelationNode>();
			foreach (Connection connection in skillTreeNode.inConnections)
			{
				if (connection != null)
				{
					PerkRelationNode perkRelationNode = connection.sourceNode as PerkRelationNode;
					if (perkRelationNode != null)
					{
						list.Add(perkRelationNode);
					}
				}
			}
			return list;
		}

		// Token: 0x06001284 RID: 4740 RVA: 0x00045E14 File Offset: 0x00044014
		public List<PerkRelationNode> GetOutgoingNodes(PerkRelationNode skillTreeNode)
		{
			List<PerkRelationNode> list = new List<PerkRelationNode>();
			foreach (Connection connection in skillTreeNode.outConnections)
			{
				if (connection != null)
				{
					PerkRelationNode perkRelationNode = connection.targetNode as PerkRelationNode;
					if (perkRelationNode != null)
					{
						list.Add(perkRelationNode);
					}
				}
			}
			return list;
		}
	}
}
