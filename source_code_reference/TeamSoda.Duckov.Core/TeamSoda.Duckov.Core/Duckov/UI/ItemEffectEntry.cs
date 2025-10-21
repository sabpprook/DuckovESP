using System;
using Duckov.Utilities;
using ItemStatsSystem;
using TMPro;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x0200038F RID: 911
	public class ItemEffectEntry : MonoBehaviour, IPoolable
	{
		// Token: 0x06002009 RID: 8201 RVA: 0x000701ED File Offset: 0x0006E3ED
		public void NotifyPooled()
		{
		}

		// Token: 0x0600200A RID: 8202 RVA: 0x000701EF File Offset: 0x0006E3EF
		public void NotifyReleased()
		{
			this.UnregisterEvents();
			this.target = null;
		}

		// Token: 0x0600200B RID: 8203 RVA: 0x000701FE File Offset: 0x0006E3FE
		private void OnDisable()
		{
			this.UnregisterEvents();
		}

		// Token: 0x0600200C RID: 8204 RVA: 0x00070206 File Offset: 0x0006E406
		public void Setup(Effect target)
		{
			this.target = target;
			this.Refresh();
			this.RegisterEvents();
		}

		// Token: 0x0600200D RID: 8205 RVA: 0x0007021B File Offset: 0x0006E41B
		private void Refresh()
		{
			this.text.text = this.target.GetDisplayString();
		}

		// Token: 0x0600200E RID: 8206 RVA: 0x00070233 File Offset: 0x0006E433
		private void RegisterEvents()
		{
		}

		// Token: 0x0600200F RID: 8207 RVA: 0x00070235 File Offset: 0x0006E435
		private void UnregisterEvents()
		{
		}

		// Token: 0x040015E5 RID: 5605
		private Effect target;

		// Token: 0x040015E6 RID: 5606
		[SerializeField]
		private TextMeshProUGUI text;
	}
}
