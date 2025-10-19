using System;
using UnityEngine;

namespace Duckov.Quests
{
	// Token: 0x0200032E RID: 814
	public class Condition : MonoBehaviour
	{
		// Token: 0x06001B95 RID: 7061 RVA: 0x000640FC File Offset: 0x000622FC
		public virtual bool Evaluate()
		{
			return false;
		}

		// Token: 0x17000517 RID: 1303
		// (get) Token: 0x06001B96 RID: 7062 RVA: 0x000640FF File Offset: 0x000622FF
		public virtual string DisplayText
		{
			get
			{
				return "";
			}
		}
	}
}
