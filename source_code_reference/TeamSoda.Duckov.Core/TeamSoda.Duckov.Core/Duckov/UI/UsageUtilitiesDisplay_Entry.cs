using System;
using ItemStatsSystem;
using TMPro;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x02000395 RID: 917
	public class UsageUtilitiesDisplay_Entry : MonoBehaviour
	{
		// Token: 0x1700062D RID: 1581
		// (get) Token: 0x06002036 RID: 8246 RVA: 0x00070770 File Offset: 0x0006E970
		// (set) Token: 0x06002037 RID: 8247 RVA: 0x00070778 File Offset: 0x0006E978
		public UsageBehavior Target { get; private set; }

		// Token: 0x06002038 RID: 8248 RVA: 0x00070784 File Offset: 0x0006E984
		internal void Setup(UsageBehavior cur)
		{
			this.text.text = cur.DisplaySettings.Description;
		}

		// Token: 0x040015F9 RID: 5625
		[SerializeField]
		private TextMeshProUGUI text;
	}
}
