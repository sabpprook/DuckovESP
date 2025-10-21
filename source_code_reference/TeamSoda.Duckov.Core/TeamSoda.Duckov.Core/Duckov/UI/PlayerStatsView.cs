using System;
using Duckov.UI.Animations;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x020003BB RID: 955
	public class PlayerStatsView : View
	{
		// Token: 0x1700069B RID: 1691
		// (get) Token: 0x060022B3 RID: 8883 RVA: 0x00079B1F File Offset: 0x00077D1F
		public static PlayerStatsView Instance
		{
			get
			{
				return View.GetViewInstance<PlayerStatsView>();
			}
		}

		// Token: 0x060022B4 RID: 8884 RVA: 0x00079B26 File Offset: 0x00077D26
		protected override void Awake()
		{
			base.Awake();
		}

		// Token: 0x060022B5 RID: 8885 RVA: 0x00079B2E File Offset: 0x00077D2E
		protected override void OnOpen()
		{
			base.OnOpen();
			this.fadeGroup.Show();
		}

		// Token: 0x060022B6 RID: 8886 RVA: 0x00079B41 File Offset: 0x00077D41
		protected override void OnClose()
		{
			base.OnClose();
			this.fadeGroup.Hide();
		}

		// Token: 0x060022B7 RID: 8887 RVA: 0x00079B54 File Offset: 0x00077D54
		private void OnEnable()
		{
			this.RegisterEvents();
		}

		// Token: 0x060022B8 RID: 8888 RVA: 0x00079B5C File Offset: 0x00077D5C
		private void OnDisable()
		{
			this.UnregisterEvents();
		}

		// Token: 0x060022B9 RID: 8889 RVA: 0x00079B64 File Offset: 0x00077D64
		private void RegisterEvents()
		{
		}

		// Token: 0x060022BA RID: 8890 RVA: 0x00079B66 File Offset: 0x00077D66
		private void UnregisterEvents()
		{
		}

		// Token: 0x040017A5 RID: 6053
		[SerializeField]
		private FadeGroup fadeGroup;
	}
}
