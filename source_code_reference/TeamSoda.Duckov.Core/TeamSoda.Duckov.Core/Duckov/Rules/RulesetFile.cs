using System;
using UnityEngine;

namespace Duckov.Rules
{
	// Token: 0x020003EF RID: 1007
	[CreateAssetMenu(menuName = "Duckov/Ruleset")]
	public class RulesetFile : ScriptableObject
	{
		// Token: 0x170006EC RID: 1772
		// (get) Token: 0x06002455 RID: 9301 RVA: 0x0007E2D2 File Offset: 0x0007C4D2
		public Ruleset Data
		{
			get
			{
				return this.data;
			}
		}

		// Token: 0x040018C3 RID: 6339
		[SerializeField]
		private Ruleset data;
	}
}
