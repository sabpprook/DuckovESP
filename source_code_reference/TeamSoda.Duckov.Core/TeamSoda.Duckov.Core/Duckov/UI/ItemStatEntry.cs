using System;
using Duckov.Utilities;
using ItemStatsSystem;
using TMPro;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x02000391 RID: 913
	public class ItemStatEntry : MonoBehaviour, IPoolable
	{
		// Token: 0x06002018 RID: 8216 RVA: 0x00070355 File Offset: 0x0006E555
		public void NotifyPooled()
		{
		}

		// Token: 0x06002019 RID: 8217 RVA: 0x00070357 File Offset: 0x0006E557
		public void NotifyReleased()
		{
			this.UnregisterEvents();
			this.target = null;
		}

		// Token: 0x0600201A RID: 8218 RVA: 0x00070366 File Offset: 0x0006E566
		private void OnDisable()
		{
			this.UnregisterEvents();
		}

		// Token: 0x0600201B RID: 8219 RVA: 0x0007036E File Offset: 0x0006E56E
		internal void Setup(Stat target)
		{
			this.UnregisterEvents();
			this.target = target;
			this.RegisterEvents();
			this.Refresh();
		}

		// Token: 0x0600201C RID: 8220 RVA: 0x0007038C File Offset: 0x0006E58C
		private void Refresh()
		{
			StatInfoDatabase.Entry entry = StatInfoDatabase.Get(this.target.Key);
			this.displayName.text = this.target.DisplayName;
			this.value.text = this.target.Value.ToString(entry.DisplayFormat);
		}

		// Token: 0x0600201D RID: 8221 RVA: 0x000703E5 File Offset: 0x0006E5E5
		private void RegisterEvents()
		{
			if (this.target == null)
			{
				return;
			}
			this.target.OnSetDirty += this.OnTargetSetDirty;
		}

		// Token: 0x0600201E RID: 8222 RVA: 0x00070407 File Offset: 0x0006E607
		private void UnregisterEvents()
		{
			if (this.target == null)
			{
				return;
			}
			this.target.OnSetDirty -= this.OnTargetSetDirty;
		}

		// Token: 0x0600201F RID: 8223 RVA: 0x00070429 File Offset: 0x0006E629
		private void OnTargetSetDirty(Stat stat)
		{
			if (stat != this.target)
			{
				Debug.LogError("ItemStatEntry.target与事件触发者不匹配。");
				return;
			}
			this.Refresh();
		}

		// Token: 0x040015ED RID: 5613
		private Stat target;

		// Token: 0x040015EE RID: 5614
		[SerializeField]
		private TextMeshProUGUI displayName;

		// Token: 0x040015EF RID: 5615
		[SerializeField]
		private TextMeshProUGUI value;
	}
}
