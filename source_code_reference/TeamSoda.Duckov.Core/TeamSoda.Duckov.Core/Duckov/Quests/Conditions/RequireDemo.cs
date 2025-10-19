using System;
using UnityEngine;

namespace Duckov.Quests.Conditions
{
	// Token: 0x0200035E RID: 862
	public class RequireDemo : Condition
	{
		// Token: 0x06001E1D RID: 7709 RVA: 0x0006A263 File Offset: 0x00068463
		public override bool Evaluate()
		{
			if (this.inverse)
			{
				return !GameMetaData.Instance.IsDemo;
			}
			return GameMetaData.Instance.IsDemo;
		}

		// Token: 0x04001490 RID: 5264
		[SerializeField]
		private bool inverse;
	}
}
