using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using Duckov.UI.Animations;
using ItemStatsSystem;
using UnityEngine;

namespace Fishing.UI
{
	// Token: 0x02000217 RID: 535
	public class FishingUI : View
	{
		// Token: 0x06000FF5 RID: 4085 RVA: 0x0003E5AC File Offset: 0x0003C7AC
		protected override void Awake()
		{
			base.Awake();
			Action_Fishing.OnPlayerStartSelectBait += this.OnStartSelectBait;
			Action_Fishing.OnPlayerStopCatching += this.OnStopCatching;
			Action_Fishing.OnPlayerStopFishing += this.OnStopFishing;
		}

		// Token: 0x06000FF6 RID: 4086 RVA: 0x0003E5E7 File Offset: 0x0003C7E7
		protected override void OnDestroy()
		{
			Action_Fishing.OnPlayerStopFishing -= this.OnStopFishing;
			Action_Fishing.OnPlayerStartSelectBait -= this.OnStartSelectBait;
			Action_Fishing.OnPlayerStopCatching -= this.OnStopCatching;
			base.OnDestroy();
		}

		// Token: 0x06000FF7 RID: 4087 RVA: 0x0003E622 File Offset: 0x0003C822
		internal override void TryQuit()
		{
		}

		// Token: 0x06000FF8 RID: 4088 RVA: 0x0003E624 File Offset: 0x0003C824
		protected override void OnOpen()
		{
			base.OnOpen();
			this.fadeGroup.Show();
			Debug.Log("Open Fishing Panel");
		}

		// Token: 0x06000FF9 RID: 4089 RVA: 0x0003E641 File Offset: 0x0003C841
		protected override void OnClose()
		{
			base.OnClose();
			this.fadeGroup.Hide();
		}

		// Token: 0x06000FFA RID: 4090 RVA: 0x0003E654 File Offset: 0x0003C854
		private void OnStopFishing(Action_Fishing fishing)
		{
			this.baitSelectPanel.NotifyStop();
			this.confirmPanel.NotifyStop();
			base.Close();
		}

		// Token: 0x06000FFB RID: 4091 RVA: 0x0003E672 File Offset: 0x0003C872
		private void OnStartSelectBait(Action_Fishing fishing, ICollection<Item> availableBaits, Func<Item, bool> baitSelectionResultCallback)
		{
			this.SelectBaitTask(availableBaits, baitSelectionResultCallback).Forget();
		}

		// Token: 0x06000FFC RID: 4092 RVA: 0x0003E684 File Offset: 0x0003C884
		private async UniTask SelectBaitTask(ICollection<Item> availableBaits, Func<Item, bool> baitSelectionResultCallback)
		{
			base.Open(null);
			await this.baitSelectPanel.DoBaitSelection(availableBaits, baitSelectionResultCallback);
			base.Close();
		}

		// Token: 0x06000FFD RID: 4093 RVA: 0x0003E6D7 File Offset: 0x0003C8D7
		private void OnStopCatching(Action_Fishing fishing, Item catchedItem, Action<bool> confirmCallback)
		{
			this.ConfirmTask(catchedItem, confirmCallback).Forget();
		}

		// Token: 0x06000FFE RID: 4094 RVA: 0x0003E6E8 File Offset: 0x0003C8E8
		private async UniTask ConfirmTask(Item catchedItem, Action<bool> confirmCallback)
		{
			base.Open(null);
			await this.confirmPanel.DoConfirmDialogue(catchedItem, confirmCallback);
			base.Close();
		}

		// Token: 0x04000CCF RID: 3279
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x04000CD0 RID: 3280
		[SerializeField]
		private BaitSelectPanel baitSelectPanel;

		// Token: 0x04000CD1 RID: 3281
		[SerializeField]
		private ConfirmPanel confirmPanel;
	}
}
