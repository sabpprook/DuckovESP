using System;
using Duckov.UI.Animations;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x0200038E RID: 910
	public class ItemDetailsPanel : ManagedUIElement
	{
		// Token: 0x06001FFF RID: 8191 RVA: 0x00070102 File Offset: 0x0006E302
		protected override void Awake()
		{
			base.Awake();
			if (ItemDetailsPanel.instance == null)
			{
				ItemDetailsPanel.instance = this;
			}
			this.closeButton.onClick.AddListener(new UnityAction(this.OnCloseButtonClicked));
		}

		// Token: 0x06002000 RID: 8192 RVA: 0x00070139 File Offset: 0x0006E339
		private void OnCloseButtonClicked()
		{
			base.Close();
		}

		// Token: 0x06002001 RID: 8193 RVA: 0x00070141 File Offset: 0x0006E341
		public static void Show(Item target, ManagedUIElement source = null)
		{
			if (ItemDetailsPanel.instance == null)
			{
				return;
			}
			ItemDetailsPanel.instance.Open(target, source);
		}

		// Token: 0x06002002 RID: 8194 RVA: 0x0007015D File Offset: 0x0006E35D
		public void Open(Item target, ManagedUIElement source)
		{
			this.target = target;
			this.source = source;
			base.Open(source);
		}

		// Token: 0x06002003 RID: 8195 RVA: 0x00070174 File Offset: 0x0006E374
		protected override void OnOpen()
		{
			if (this.target == null)
			{
				return;
			}
			base.gameObject.SetActive(true);
			this.Setup(this.target);
			this.fadeGroup.Show();
		}

		// Token: 0x06002004 RID: 8196 RVA: 0x000701A8 File Offset: 0x0006E3A8
		protected override void OnClose()
		{
			this.UnregisterEvents();
			this.target = null;
			this.fadeGroup.Hide();
		}

		// Token: 0x06002005 RID: 8197 RVA: 0x000701C2 File Offset: 0x0006E3C2
		private void OnDisable()
		{
			this.UnregisterEvents();
		}

		// Token: 0x06002006 RID: 8198 RVA: 0x000701CA File Offset: 0x0006E3CA
		internal void Setup(Item target)
		{
			this.display.Setup(target);
		}

		// Token: 0x06002007 RID: 8199 RVA: 0x000701D8 File Offset: 0x0006E3D8
		private void UnregisterEvents()
		{
			this.display.UnregisterEvents();
		}

		// Token: 0x040015DF RID: 5599
		private static ItemDetailsPanel instance;

		// Token: 0x040015E0 RID: 5600
		private Item target;

		// Token: 0x040015E1 RID: 5601
		[SerializeField]
		private ItemDetailsDisplay display;

		// Token: 0x040015E2 RID: 5602
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x040015E3 RID: 5603
		[SerializeField]
		private Button closeButton;

		// Token: 0x040015E4 RID: 5604
		private ManagedUIElement source;
	}
}
