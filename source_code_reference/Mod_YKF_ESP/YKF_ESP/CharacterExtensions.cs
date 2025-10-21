using System;
using System.Runtime.CompilerServices;

namespace YKF_ESP
{
	// Token: 0x02000009 RID: 9
	[NullableContext(1)]
	[Nullable(0)]
	public static class CharacterExtensions
	{
		// Token: 0x06000019 RID: 25 RVA: 0x00002808 File Offset: 0x00000A08
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

		// Token: 0x0600001A RID: 26 RVA: 0x0000284C File Offset: 0x00000A4C
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
