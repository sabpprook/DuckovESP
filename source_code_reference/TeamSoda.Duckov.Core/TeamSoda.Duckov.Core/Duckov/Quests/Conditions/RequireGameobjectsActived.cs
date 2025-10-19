using System;
using UnityEngine;

namespace Duckov.Quests.Conditions
{
	// Token: 0x02000360 RID: 864
	public class RequireGameobjectsActived : Condition
	{
		// Token: 0x06001E21 RID: 7713 RVA: 0x0006A2A4 File Offset: 0x000684A4
		public override bool Evaluate()
		{
			foreach (GameObject gameObject in this.targets)
			{
				if (gameObject == null || !gameObject.activeInHierarchy)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x04001494 RID: 5268
		[SerializeField]
		private GameObject[] targets;
	}
}
