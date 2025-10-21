using System;
using System.Collections.Generic;

namespace Duckov.Quests
{
	// Token: 0x0200032F RID: 815
	public static class ConditionExtensions
	{
		// Token: 0x06001B98 RID: 7064 RVA: 0x00064110 File Offset: 0x00062310
		public static bool Satisfied(this IEnumerable<Condition> conditions)
		{
			foreach (Condition condition in conditions)
			{
				if (!(condition == null) && !condition.Evaluate())
				{
					return false;
				}
			}
			return true;
		}
	}
}
