using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Duckov.PerkTrees;
using Duckov.UI.Animations;
using Duckov.Utilities;
using NodeCanvas.Framework;
using TMPro;
using UI_Spline_Renderer;
using UnityEngine;
using UnityEngine.Splines;

namespace Duckov.UI
{
	// Token: 0x020003BA RID: 954
	public class PerkTreeView : View, ISingleSelectionMenu<PerkEntry>
	{
		// Token: 0x17000697 RID: 1687
		// (get) Token: 0x06002298 RID: 8856 RVA: 0x00078F8C File Offset: 0x0007718C
		public static PerkTreeView Instance
		{
			get
			{
				return View.GetViewInstance<PerkTreeView>();
			}
		}

		// Token: 0x17000698 RID: 1688
		// (get) Token: 0x06002299 RID: 8857 RVA: 0x00078F94 File Offset: 0x00077194
		private PrefabPool<PerkEntry> PerkEntryPool
		{
			get
			{
				if (this._perkEntryPool == null)
				{
					this._perkEntryPool = new PrefabPool<PerkEntry>(this.perkEntryPrefab, this.contentParent, null, null, null, true, 10, 10000, null);
				}
				return this._perkEntryPool;
			}
		}

		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x0600229A RID: 8858 RVA: 0x00078FD4 File Offset: 0x000771D4
		private PrefabPool<PerkLineEntry> PerkLinePool
		{
			get
			{
				if (this._perkLinePool == null)
				{
					this._perkLinePool = new PrefabPool<PerkLineEntry>(this.perkLinePrefab, this.contentParent, null, null, null, true, 10, 10000, null);
				}
				return this._perkLinePool;
			}
		}

		// Token: 0x1700069A RID: 1690
		// (get) Token: 0x0600229B RID: 8859 RVA: 0x00079012 File Offset: 0x00077212
		protected override bool ShowOpenCloseButtons
		{
			get
			{
				return false;
			}
		}

		// Token: 0x140000E9 RID: 233
		// (add) Token: 0x0600229C RID: 8860 RVA: 0x00079018 File Offset: 0x00077218
		// (remove) Token: 0x0600229D RID: 8861 RVA: 0x00079050 File Offset: 0x00077250
		internal event Action<PerkEntry> onSelectionChanged;

		// Token: 0x0600229E RID: 8862 RVA: 0x00079088 File Offset: 0x00077288
		private void PopulatePerks()
		{
			this.contentParent.ForceUpdateRectTransforms();
			this.PerkEntryPool.ReleaseAll();
			this.PerkLinePool.ReleaseAll();
			bool isDemo = GameMetaData.Instance.IsDemo;
			foreach (Perk perk in this.target.Perks)
			{
				if ((!isDemo || !perk.LockInDemo) && this.target.RelationGraphOwner.GetRelatedNode(perk) != null)
				{
					this.PerkEntryPool.Get(this.contentParent).Setup(this, perk);
				}
			}
			foreach (PerkLevelLineNode perkLevelLineNode in this.target.RelationGraphOwner.graph.GetAllNodesOfType<PerkLevelLineNode>())
			{
				this.PerkLinePool.Get(this.contentParent).Setup(this, perkLevelLineNode);
			}
			this.FitChildren();
			this.RefreshConnections();
		}

		// Token: 0x0600229F RID: 8863 RVA: 0x000791A0 File Offset: 0x000773A0
		private void RefreshConnections()
		{
			bool isDemo = GameMetaData.Instance.IsDemo;
			this.activeConnectionsRenderer.enabled = false;
			this.inactiveConnectionsRenderer.enabled = false;
			SplineContainer splineContainer = this.activeConnectionsRenderer.splineContainer;
			SplineContainer splineContainer2 = this.inactiveConnectionsRenderer.splineContainer;
			PerkTreeView.<RefreshConnections>g__ClearSplines|27_0(splineContainer);
			PerkTreeView.<RefreshConnections>g__ClearSplines|27_0(splineContainer2);
			PerkTreeView.<>c__DisplayClass27_0 CS$<>8__locals1;
			CS$<>8__locals1.horizontal = this.target.Horizontal;
			CS$<>8__locals1.splineTangentVector = (CS$<>8__locals1.horizontal ? Vector3.left : Vector3.up) * this.splineTangent;
			foreach (Perk perk in this.target.Perks)
			{
				if (!isDemo || !perk.LockInDemo)
				{
					PerkRelationNode relatedNode = this.target.RelationGraphOwner.GetRelatedNode(perk);
					PerkEntry perkEntry = this.GetPerkEntry(perk);
					if (!(perkEntry == null) && relatedNode != null)
					{
						SplineContainer splineContainer3 = (perk.Unlocked ? splineContainer : splineContainer2);
						foreach (Connection connection in relatedNode.outConnections)
						{
							PerkRelationNode perkRelationNode = connection.targetNode as PerkRelationNode;
							Perk relatedNode2 = perkRelationNode.relatedNode;
							if (relatedNode2 == null)
							{
								Debug.Log(string.Concat(new string[] { "Target Perk is Null (Connection from ", relatedNode.name, " to ", perkRelationNode.name, ")" }));
							}
							else if (!isDemo || !relatedNode2.LockInDemo)
							{
								PerkEntry perkEntry2 = this.GetPerkEntry(relatedNode2);
								if (perkEntry2 == null)
								{
									Debug.Log(string.Concat(new string[] { "Target Perk Entry is Null (Connection from ", relatedNode.name, " to ", perkRelationNode.name, ")" }));
								}
								else
								{
									PerkTreeView.<RefreshConnections>g__AddConnection|27_1(splineContainer3, perkEntry.transform.localPosition, perkEntry2.transform.localPosition, ref CS$<>8__locals1);
								}
							}
						}
					}
				}
			}
			this.activeConnectionsRenderer.enabled = true;
			this.inactiveConnectionsRenderer.enabled = true;
		}

		// Token: 0x060022A0 RID: 8864 RVA: 0x00079424 File Offset: 0x00077624
		private PerkEntry GetPerkEntry(Perk ofPerk)
		{
			return this.PerkEntryPool.ActiveEntries.FirstOrDefault((PerkEntry e) => e != null && e.Target == ofPerk);
		}

		// Token: 0x060022A1 RID: 8865 RVA: 0x0007945C File Offset: 0x0007765C
		private void FitChildren()
		{
			this.contentParent.ForceUpdateRectTransforms();
			ReadOnlyCollection<PerkEntry> activeEntries = this.PerkEntryPool.ActiveEntries;
			float num2;
			float num = (num2 = float.MaxValue);
			float num4;
			float num3 = (num4 = float.MinValue);
			foreach (PerkEntry perkEntry in activeEntries)
			{
				RectTransform rectTransform = perkEntry.RectTransform;
				rectTransform.anchorMin = Vector2.zero;
				rectTransform.anchorMax = Vector2.zero;
				Vector2 layoutPosition = perkEntry.GetLayoutPosition();
				layoutPosition.y *= -1f;
				Vector2 vector = layoutPosition * this.layoutFactor;
				rectTransform.anchoredPosition = vector;
				if (vector.x < num2)
				{
					num2 = vector.x;
				}
				if (vector.y < num)
				{
					num = vector.y;
				}
				if (vector.x > num4)
				{
					num4 = vector.x;
				}
				if (vector.y > num3)
				{
					num3 = vector.y;
				}
			}
			float num5 = num4 - num2;
			float num6 = num3 - num;
			Vector2 vector2 = -new Vector2(num2, num);
			RectTransform rectTransform2 = this.contentParent;
			Vector2 sizeDelta = rectTransform2.sizeDelta;
			sizeDelta.y = num6 + this.padding.y * 2f;
			rectTransform2.sizeDelta = sizeDelta;
			foreach (PerkEntry perkEntry2 in activeEntries)
			{
				RectTransform rectTransform3 = perkEntry2.RectTransform;
				Vector2 vector3 = rectTransform3.anchoredPosition + vector2;
				if (num5 == 0f)
				{
					vector3.x = (rectTransform2.rect.width - this.padding.x * 2f) / 2f;
				}
				else
				{
					float num7 = (rectTransform2.rect.width - this.padding.x * 2f) / num5;
					vector3.x *= num7;
				}
				vector3 += this.padding;
				rectTransform3.anchoredPosition = vector3;
			}
			foreach (PerkLineEntry perkLineEntry in this.PerkLinePool.ActiveEntries)
			{
				RectTransform rectTransform4 = perkLineEntry.RectTransform;
				Vector2 layoutPosition2 = perkLineEntry.GetLayoutPosition();
				layoutPosition2.y *= -1f;
				Vector2 vector4 = layoutPosition2 * this.layoutFactor;
				vector4 += this.padding;
				vector4.x = rectTransform4.anchoredPosition.x;
				rectTransform4.anchoredPosition = vector4;
				rectTransform4.SetAsFirstSibling();
			}
			this.contentParent.anchoredPosition = Vector2.zero;
		}

		// Token: 0x060022A2 RID: 8866 RVA: 0x0007973C File Offset: 0x0007793C
		protected override void OnOpen()
		{
			base.OnOpen();
			this.fadeGroup.Show();
		}

		// Token: 0x060022A3 RID: 8867 RVA: 0x0007974F File Offset: 0x0007794F
		protected override void OnClose()
		{
			base.OnClose();
			this.fadeGroup.Hide();
		}

		// Token: 0x060022A4 RID: 8868 RVA: 0x00079762 File Offset: 0x00077962
		public PerkEntry GetSelection()
		{
			return this.selectedPerkEntry;
		}

		// Token: 0x060022A5 RID: 8869 RVA: 0x0007976A File Offset: 0x0007796A
		public bool SetSelection(PerkEntry selection)
		{
			this.selectedPerkEntry = selection;
			this.OnSelectionChanged();
			return true;
		}

		// Token: 0x060022A6 RID: 8870 RVA: 0x0007977A File Offset: 0x0007797A
		private void OnSelectionChanged()
		{
			Action<PerkEntry> action = this.onSelectionChanged;
			if (action != null)
			{
				action(this.selectedPerkEntry);
			}
			this.RefreshDetails();
		}

		// Token: 0x060022A7 RID: 8871 RVA: 0x00079799 File Offset: 0x00077999
		private void RefreshDetails()
		{
			PerkDetails perkDetails = this.details;
			PerkEntry perkEntry = this.selectedPerkEntry;
			perkDetails.Setup((perkEntry != null) ? perkEntry.Target : null, true);
		}

		// Token: 0x060022A8 RID: 8872 RVA: 0x000797B9 File Offset: 0x000779B9
		private void Show_Local(PerkTree target)
		{
			this.UnregisterEvents();
			this.SetSelection(null);
			this.target = target;
			this.title.text = target.DisplayName;
			this.ShowTask().Forget();
			this.RegisterEvents();
		}

		// Token: 0x060022A9 RID: 8873 RVA: 0x000797F2 File Offset: 0x000779F2
		public static void Show(PerkTree target)
		{
			if (PerkTreeView.Instance == null)
			{
				return;
			}
			PerkTreeView.Instance.Show_Local(target);
		}

		// Token: 0x060022AA RID: 8874 RVA: 0x0007980D File Offset: 0x00077A0D
		private void RegisterEvents()
		{
			if (this.target != null)
			{
				this.target.onPerkTreeStatusChanged += this.Refresh;
			}
		}

		// Token: 0x060022AB RID: 8875 RVA: 0x00079834 File Offset: 0x00077A34
		private void UnregisterEvents()
		{
			if (this.target != null)
			{
				this.target.onPerkTreeStatusChanged -= this.Refresh;
			}
		}

		// Token: 0x060022AC RID: 8876 RVA: 0x0007985B File Offset: 0x00077A5B
		private void Refresh(PerkTree tree)
		{
			this.RefreshConnections();
		}

		// Token: 0x060022AD RID: 8877 RVA: 0x00079864 File Offset: 0x00077A64
		private async UniTask ShowTask()
		{
			if (this.target == null)
			{
				base.Close();
			}
			else
			{
				base.Open(null);
				await UniTask.WaitForEndOfFrame(this);
				await UniTask.WaitForEndOfFrame(this);
				await UniTask.WaitForEndOfFrame(this);
				this.PopulatePerks();
			}
		}

		// Token: 0x060022AE RID: 8878 RVA: 0x000798A7 File Offset: 0x00077AA7
		public void Hide()
		{
			base.Close();
		}

		// Token: 0x060022AF RID: 8879 RVA: 0x000798AF File Offset: 0x00077AAF
		protected override void Awake()
		{
			base.Awake();
		}

		// Token: 0x060022B1 RID: 8881 RVA: 0x000798E0 File Offset: 0x00077AE0
		[CompilerGenerated]
		internal static void <RefreshConnections>g__ClearSplines|27_0(SplineContainer splineContainer)
		{
			while (splineContainer.Splines.Count > 0)
			{
				splineContainer.RemoveSplineAt(0);
			}
		}

		// Token: 0x060022B2 RID: 8882 RVA: 0x000798FC File Offset: 0x00077AFC
		[CompilerGenerated]
		internal static void <RefreshConnections>g__AddConnection|27_1(SplineContainer container, Vector2 from, Vector2 to, ref PerkTreeView.<>c__DisplayClass27_0 A_3)
		{
			if (A_3.horizontal)
			{
				container.AddSpline(new Spline(new BezierKnot[]
				{
					new BezierKnot(from, A_3.splineTangentVector, -A_3.splineTangentVector),
					new BezierKnot(from - A_3.splineTangentVector, A_3.splineTangentVector, -A_3.splineTangentVector),
					new BezierKnot(new Vector3(from.x, to.y) - 2f * A_3.splineTangentVector, A_3.splineTangentVector, -A_3.splineTangentVector),
					new BezierKnot(to, A_3.splineTangentVector, -A_3.splineTangentVector)
				}, false));
				return;
			}
			container.AddSpline(new Spline(new BezierKnot[]
			{
				new BezierKnot(from, A_3.splineTangentVector, -A_3.splineTangentVector),
				new BezierKnot(from - A_3.splineTangentVector, A_3.splineTangentVector, -A_3.splineTangentVector),
				new BezierKnot(new Vector3(to.x, from.y) - 2f * A_3.splineTangentVector, A_3.splineTangentVector, -A_3.splineTangentVector),
				new BezierKnot(to, A_3.splineTangentVector, -A_3.splineTangentVector)
			}, false));
		}

		// Token: 0x04001795 RID: 6037
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x04001796 RID: 6038
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x04001797 RID: 6039
		[SerializeField]
		private RectTransform contentParent;

		// Token: 0x04001798 RID: 6040
		[SerializeField]
		private PerkDetails details;

		// Token: 0x04001799 RID: 6041
		[SerializeField]
		private PerkEntry perkEntryPrefab;

		// Token: 0x0400179A RID: 6042
		[SerializeField]
		private PerkLineEntry perkLinePrefab;

		// Token: 0x0400179B RID: 6043
		[SerializeField]
		private UISplineRenderer activeConnectionsRenderer;

		// Token: 0x0400179C RID: 6044
		[SerializeField]
		private UISplineRenderer inactiveConnectionsRenderer;

		// Token: 0x0400179D RID: 6045
		[SerializeField]
		private float splineTangent = 100f;

		// Token: 0x0400179E RID: 6046
		[SerializeField]
		private PerkTree target;

		// Token: 0x0400179F RID: 6047
		private PrefabPool<PerkEntry> _perkEntryPool;

		// Token: 0x040017A0 RID: 6048
		private PrefabPool<PerkLineEntry> _perkLinePool;

		// Token: 0x040017A1 RID: 6049
		private PerkEntry selectedPerkEntry;

		// Token: 0x040017A2 RID: 6050
		[SerializeField]
		private float layoutFactor = 10f;

		// Token: 0x040017A3 RID: 6051
		[SerializeField]
		private Vector2 padding = Vector2.one;
	}
}
