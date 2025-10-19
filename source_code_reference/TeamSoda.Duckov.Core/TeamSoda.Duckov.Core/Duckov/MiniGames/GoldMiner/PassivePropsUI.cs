using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Duckov.MiniGames.GoldMiner.UI;
using Duckov.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x020002A5 RID: 677
	public class PassivePropsUI : MiniGameBehaviour
	{
		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x060015FF RID: 5631 RVA: 0x000511D0 File Offset: 0x0004F3D0
		private PrefabPool<PassivePropDisplay> Pool
		{
			get
			{
				if (this._pool == null)
				{
					this._pool = new PrefabPool<PassivePropDisplay>(this.entryTemplate, null, new Action<PassivePropDisplay>(this.OnGetEntry), new Action<PassivePropDisplay>(this.OnReleaseEntry), null, true, 10, 10000, null);
				}
				return this._pool;
			}
		}

		// Token: 0x06001600 RID: 5632 RVA: 0x0005121F File Offset: 0x0004F41F
		private void OnReleaseEntry(PassivePropDisplay display)
		{
			this.navGroup.Remove(display.NavEntry);
		}

		// Token: 0x06001601 RID: 5633 RVA: 0x00051232 File Offset: 0x0004F432
		private void OnGetEntry(PassivePropDisplay display)
		{
			this.navGroup.Add(display.NavEntry);
		}

		// Token: 0x06001602 RID: 5634 RVA: 0x00051248 File Offset: 0x0004F448
		private void Awake()
		{
			GoldMiner goldMiner = this.master;
			goldMiner.onLevelBegin = (Action<GoldMiner>)Delegate.Combine(goldMiner.onLevelBegin, new Action<GoldMiner>(this.OnLevelBegin));
			GoldMiner goldMiner2 = this.master;
			goldMiner2.onArtifactChange = (Action<GoldMiner>)Delegate.Combine(goldMiner2.onArtifactChange, new Action<GoldMiner>(this.OnArtifactChanged));
			GoldMiner goldMiner3 = this.master;
			goldMiner3.onEarlyLevelPlayTick = (Action<GoldMiner>)Delegate.Combine(goldMiner3.onEarlyLevelPlayTick, new Action<GoldMiner>(this.OnEarlyTick));
			NavGroup.OnNavGroupChanged = (Action)Delegate.Combine(NavGroup.OnNavGroupChanged, new Action(this.OnNavGroupChanged));
		}

		// Token: 0x06001603 RID: 5635 RVA: 0x000512EA File Offset: 0x0004F4EA
		private void OnDestroy()
		{
			NavGroup.OnNavGroupChanged = (Action)Delegate.Remove(NavGroup.OnNavGroupChanged, new Action(this.OnNavGroupChanged));
		}

		// Token: 0x06001604 RID: 5636 RVA: 0x0005130C File Offset: 0x0004F50C
		private void OnNavGroupChanged()
		{
			this.changeLock = true;
			if (this.navGroup.active && this.Pool.ActiveEntries.Count <= 0)
			{
				this.upNavGroup.SetAsActiveNavGroup();
			}
			this.RefreshDescription();
		}

		// Token: 0x06001605 RID: 5637 RVA: 0x00051348 File Offset: 0x0004F548
		private void OnEarlyTick(GoldMiner miner)
		{
			this.RefreshDescription();
		}

		// Token: 0x06001606 RID: 5638 RVA: 0x0005135C File Offset: 0x0004F55C
		private void SetCoord([TupleElementNames(new string[] { "x", "y" })] ValueTuple<int, int> coord)
		{
			int num = this.CoordToIndex(coord);
			this.navGroup.NavIndex = num;
			this.RefreshDescription();
		}

		// Token: 0x06001607 RID: 5639 RVA: 0x00051384 File Offset: 0x0004F584
		private void RefreshDescription()
		{
			if (!this.navGroup.active)
			{
				this.HideDescription();
				return;
			}
			if (this.Pool.ActiveEntries.Count <= 0)
			{
				this.HideDescription();
				return;
			}
			NavEntry selectedEntry = this.navGroup.GetSelectedEntry();
			if (selectedEntry == null)
			{
				this.HideDescription();
				return;
			}
			if (!selectedEntry.VCT.IsHovering)
			{
				this.HideDescription();
				return;
			}
			PassivePropDisplay component = selectedEntry.GetComponent<PassivePropDisplay>();
			if (component == null)
			{
				this.HideDescription();
				return;
			}
			this.SetupAndShowDescription(component);
		}

		// Token: 0x06001608 RID: 5640 RVA: 0x0005140D File Offset: 0x0004F60D
		private void HideDescription()
		{
			this.descriptionContainer.gameObject.SetActive(false);
		}

		// Token: 0x06001609 RID: 5641 RVA: 0x00051420 File Offset: 0x0004F620
		private void SetupAndShowDescription(PassivePropDisplay ppd)
		{
			this.descriptionContainer.gameObject.SetActive(true);
			string description = ppd.Target.Description;
			this.descriptionText.text = description;
			this.descriptionContainer.position = ppd.rectTransform.TransformPoint(ppd.rectTransform.rect.max);
		}

		// Token: 0x0600160A RID: 5642 RVA: 0x00051484 File Offset: 0x0004F684
		private int CoordToIndex([TupleElementNames(new string[] { "x", "y" })] ValueTuple<int, int> coord)
		{
			int count = this.navGroup.entries.Count;
			if (count <= 0)
			{
				return 0;
			}
			int constraintCount = this.gridLayout.constraintCount;
			int num = count / constraintCount;
			if (coord.Item2 > num)
			{
				coord.Item2 = num;
			}
			int num2 = constraintCount;
			if (coord.Item2 == num)
			{
				num2 = count % constraintCount;
			}
			if (coord.Item1 < 0)
			{
				coord.Item1 = num2 - 1;
			}
			coord.Item1 %= num2;
			if (coord.Item2 < 0)
			{
				coord.Item2 = num;
			}
			coord.Item2 %= num + 1;
			return constraintCount * coord.Item2 + coord.Item1;
		}

		// Token: 0x0600160B RID: 5643 RVA: 0x00051528 File Offset: 0x0004F728
		[return: TupleElementNames(new string[] { "x", "y" })]
		private ValueTuple<int, int> IndexToCoord(int index)
		{
			int constraintCount = this.gridLayout.constraintCount;
			int num = index / constraintCount;
			return new ValueTuple<int, int>(index % constraintCount, num);
		}

		// Token: 0x0600160C RID: 5644 RVA: 0x0005154E File Offset: 0x0004F74E
		private void OnLevelBegin(GoldMiner miner)
		{
			this.Refresh();
			this.RefreshDescription();
		}

		// Token: 0x0600160D RID: 5645 RVA: 0x0005155C File Offset: 0x0004F75C
		private void OnArtifactChanged(GoldMiner miner)
		{
			this.Refresh();
		}

		// Token: 0x0600160E RID: 5646 RVA: 0x00051564 File Offset: 0x0004F764
		private void Refresh()
		{
			this.Pool.ReleaseAll();
			if (this.master == null)
			{
				return;
			}
			GoldMinerRunData run = this.master.run;
			if (run == null)
			{
				return;
			}
			foreach (IGrouping<string, GoldMinerArtifact> grouping in from e in run.artifacts
				where e != null
				group e by e.ID)
			{
				GoldMinerArtifact goldMinerArtifact = grouping.ElementAt(0);
				this.Pool.Get(null).Setup(goldMinerArtifact, grouping.Count<GoldMinerArtifact>());
			}
		}

		// Token: 0x04001056 RID: 4182
		[SerializeField]
		private GoldMiner master;

		// Token: 0x04001057 RID: 4183
		[SerializeField]
		private RectTransform descriptionContainer;

		// Token: 0x04001058 RID: 4184
		[SerializeField]
		private TextMeshProUGUI descriptionText;

		// Token: 0x04001059 RID: 4185
		[SerializeField]
		private PassivePropDisplay entryTemplate;

		// Token: 0x0400105A RID: 4186
		[SerializeField]
		private NavGroup navGroup;

		// Token: 0x0400105B RID: 4187
		[SerializeField]
		private NavGroup upNavGroup;

		// Token: 0x0400105C RID: 4188
		[SerializeField]
		private GridLayoutGroup gridLayout;

		// Token: 0x0400105D RID: 4189
		private PrefabPool<PassivePropDisplay> _pool;

		// Token: 0x0400105E RID: 4190
		private bool changeLock;
	}
}
