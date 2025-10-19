using System;
using Duckov.UI;
using Duckov.UI.Animations;
using UnityEngine;

namespace Duckov.MiniGames
{
	// Token: 0x0200027E RID: 638
	public class GamingConsoleHUD : View
	{
		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x0600144E RID: 5198 RVA: 0x0004B629 File Offset: 0x00049829
		private static GamingConsoleHUD Instance
		{
			get
			{
				if (GamingConsoleHUD._instance_cache == null)
				{
					GamingConsoleHUD._instance_cache = View.GetViewInstance<GamingConsoleHUD>();
				}
				return GamingConsoleHUD._instance_cache;
			}
		}

		// Token: 0x0600144F RID: 5199 RVA: 0x0004B647 File Offset: 0x00049847
		public static void Show()
		{
			if (GamingConsoleHUD.Instance == null)
			{
				return;
			}
			GamingConsoleHUD.Instance.LocalShow();
		}

		// Token: 0x06001450 RID: 5200 RVA: 0x0004B661 File Offset: 0x00049861
		public static void Hide()
		{
			if (GamingConsoleHUD.Instance == null)
			{
				return;
			}
			GamingConsoleHUD.Instance.LocalHide();
		}

		// Token: 0x06001451 RID: 5201 RVA: 0x0004B67B File Offset: 0x0004987B
		private void LocalShow()
		{
			this.contentFadeGroup.Show();
		}

		// Token: 0x06001452 RID: 5202 RVA: 0x0004B688 File Offset: 0x00049888
		private void LocalHide()
		{
			this.contentFadeGroup.Hide();
		}

		// Token: 0x04000EF1 RID: 3825
		[SerializeField]
		private FadeGroup contentFadeGroup;

		// Token: 0x04000EF2 RID: 3826
		private static GamingConsoleHUD _instance_cache;
	}
}
