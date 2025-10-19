using System;
using System.Runtime.CompilerServices;

namespace YKF_ESP
{
	// Token: 0x02000006 RID: 6
	[NullableContext(1)]
	[Nullable(0)]
	public static class CharacterExtensions
	{
		// Token: 0x0600000B RID: 11 RVA: 0x000020E0 File Offset: 0x000002E0
		public static bool IsDead(this CharacterMainControl character)
		{
			bool flag;
			try
			{
				flag = character.Health == null || character.Health.IsDead;
			}
			catch
			{
				flag = true;
			}
			return flag;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002124 File Offset: 0x00000324
		public static bool IsValidEnemy(this CharacterMainControl character, CharacterMainControl player)
		{
			bool flag;
			try
			{
				flag = Team.IsEnemy(player.Team, character.Team);
			}
			catch
			{
				flag = true;
			}
			return flag;
		}
	}
}
