using System;
using UnityEngine;
using UnityEngine.Events;

namespace Duckov.MiniGames.GoldMiner.UI
{
	// Token: 0x020002A9 RID: 681
	public class NavEntry : MonoBehaviour
	{
		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x0600161C RID: 5660 RVA: 0x0005199E File Offset: 0x0004FB9E
		// (set) Token: 0x0600161D RID: 5661 RVA: 0x000519A6 File Offset: 0x0004FBA6
		public bool selectionState { get; private set; }

		// Token: 0x0600161E RID: 5662 RVA: 0x000519B0 File Offset: 0x0004FBB0
		private void Awake()
		{
			if (this.masterGroup == null)
			{
				this.masterGroup = base.GetComponentInParent<NavGroup>();
			}
			this.VCT = base.GetComponent<VirtualCursorTarget>();
			if (this.VCT)
			{
				this.VCT.onEnter.AddListener(new UnityAction(this.TrySelectThis));
				this.VCT.onClick.AddListener(new UnityAction(this.Interact));
			}
		}

		// Token: 0x0600161F RID: 5663 RVA: 0x00051A28 File Offset: 0x0004FC28
		private void Interact()
		{
			this.NotifyInteract();
		}

		// Token: 0x06001620 RID: 5664 RVA: 0x00051A30 File Offset: 0x0004FC30
		public void NotifySelectionState(bool value)
		{
			this.selectionState = value;
			this.selectedIndicator.SetActive(this.selectionState);
		}

		// Token: 0x06001621 RID: 5665 RVA: 0x00051A4A File Offset: 0x0004FC4A
		internal void NotifyInteract()
		{
			Action<NavEntry> action = this.onInteract;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06001622 RID: 5666 RVA: 0x00051A5D File Offset: 0x0004FC5D
		public void TrySelectThis()
		{
			if (this.masterGroup == null)
			{
				return;
			}
			this.masterGroup.TrySelect(this);
		}

		// Token: 0x0400106C RID: 4204
		public GameObject selectedIndicator;

		// Token: 0x0400106D RID: 4205
		public Action<NavEntry> onInteract;

		// Token: 0x0400106E RID: 4206
		public Action<NavEntry> onTrySelectThis;

		// Token: 0x0400106F RID: 4207
		public NavGroup masterGroup;

		// Token: 0x04001070 RID: 4208
		public VirtualCursorTarget VCT;
	}
}
