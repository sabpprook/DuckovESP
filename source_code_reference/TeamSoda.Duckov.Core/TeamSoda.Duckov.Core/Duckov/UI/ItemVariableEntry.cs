using System;
using Duckov.Utilities;
using TMPro;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x02000392 RID: 914
	public class ItemVariableEntry : MonoBehaviour, IPoolable
	{
		// Token: 0x06002021 RID: 8225 RVA: 0x0007044D File Offset: 0x0006E64D
		public void NotifyPooled()
		{
		}

		// Token: 0x06002022 RID: 8226 RVA: 0x0007044F File Offset: 0x0006E64F
		public void NotifyReleased()
		{
			this.UnregisterEvents();
			this.target = null;
		}

		// Token: 0x06002023 RID: 8227 RVA: 0x0007045E File Offset: 0x0006E65E
		private void OnDisable()
		{
			this.UnregisterEvents();
		}

		// Token: 0x06002024 RID: 8228 RVA: 0x00070466 File Offset: 0x0006E666
		internal void Setup(CustomData target)
		{
			this.UnregisterEvents();
			this.target = target;
			this.Refresh();
			this.RegisterEvents();
		}

		// Token: 0x06002025 RID: 8229 RVA: 0x00070481 File Offset: 0x0006E681
		private void Refresh()
		{
			this.displayName.text = this.target.DisplayName;
			this.value.text = this.target.GetValueDisplayString("");
		}

		// Token: 0x06002026 RID: 8230 RVA: 0x000704B4 File Offset: 0x0006E6B4
		private void RegisterEvents()
		{
			if (this.target == null)
			{
				return;
			}
			this.target.OnSetData += this.OnTargetSetData;
		}

		// Token: 0x06002027 RID: 8231 RVA: 0x000704D6 File Offset: 0x0006E6D6
		private void UnregisterEvents()
		{
			if (this.target == null)
			{
				return;
			}
			this.target.OnSetData -= this.OnTargetSetData;
		}

		// Token: 0x06002028 RID: 8232 RVA: 0x000704F8 File Offset: 0x0006E6F8
		private void OnTargetSetData(CustomData data)
		{
			this.Refresh();
		}

		// Token: 0x040015F0 RID: 5616
		private CustomData target;

		// Token: 0x040015F1 RID: 5617
		[SerializeField]
		private TextMeshProUGUI displayName;

		// Token: 0x040015F2 RID: 5618
		[SerializeField]
		private TextMeshProUGUI value;
	}
}
