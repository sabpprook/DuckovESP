using System;
using Duckov.UI;
using Duckov.UI.Animations;
using UnityEngine;

namespace Duckov.MasterKeys.UI
{
	// Token: 0x020002E0 RID: 736
	public class MasterKeysView : View, ISingleSelectionMenu<MasterKeysIndexEntry>
	{
		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x0600178D RID: 6029 RVA: 0x000565A1 File Offset: 0x000547A1
		public static MasterKeysView Instance
		{
			get
			{
				return View.GetViewInstance<MasterKeysView>();
			}
		}

		// Token: 0x0600178E RID: 6030 RVA: 0x000565A8 File Offset: 0x000547A8
		protected override void Awake()
		{
			base.Awake();
			this.listDisplay.onEntryPointerClicked += this.OnEntryClicked;
		}

		// Token: 0x0600178F RID: 6031 RVA: 0x000565C7 File Offset: 0x000547C7
		private void OnEntryClicked(MasterKeysIndexEntry entry)
		{
			this.RefreshInspectorDisplay();
		}

		// Token: 0x06001790 RID: 6032 RVA: 0x000565CF File Offset: 0x000547CF
		public MasterKeysIndexEntry GetSelection()
		{
			return this.listDisplay.GetSelection();
		}

		// Token: 0x06001791 RID: 6033 RVA: 0x000565DC File Offset: 0x000547DC
		public bool SetSelection(MasterKeysIndexEntry selection)
		{
			this.listDisplay.GetSelection();
			return true;
		}

		// Token: 0x06001792 RID: 6034 RVA: 0x000565EB File Offset: 0x000547EB
		protected override void OnOpen()
		{
			base.OnOpen();
			this.fadeGroup.Show();
			this.SetSelection(null);
			this.RefreshListDisplay();
			this.RefreshInspectorDisplay();
		}

		// Token: 0x06001793 RID: 6035 RVA: 0x00056612 File Offset: 0x00054812
		protected override void OnClose()
		{
			base.OnClose();
			this.fadeGroup.Hide();
		}

		// Token: 0x06001794 RID: 6036 RVA: 0x00056625 File Offset: 0x00054825
		private void RefreshListDisplay()
		{
			this.listDisplay.Refresh();
		}

		// Token: 0x06001795 RID: 6037 RVA: 0x00056634 File Offset: 0x00054834
		private void RefreshInspectorDisplay()
		{
			MasterKeysIndexEntry selection = this.GetSelection();
			this.inspector.Setup(selection);
		}

		// Token: 0x06001796 RID: 6038 RVA: 0x00056654 File Offset: 0x00054854
		internal static void Show()
		{
			if (MasterKeysView.Instance == null)
			{
				Debug.Log(" Master keys view Instance is null");
				return;
			}
			MasterKeysView.Instance.Open(null);
		}

		// Token: 0x04001136 RID: 4406
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x04001137 RID: 4407
		[SerializeField]
		private MasterKeysIndexList listDisplay;

		// Token: 0x04001138 RID: 4408
		[SerializeField]
		private MasterKeysIndexInspector inspector;
	}
}
