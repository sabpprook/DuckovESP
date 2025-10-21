using System;

// Token: 0x02000097 RID: 151
public static class CharacterMainControlExtensions
{
	// Token: 0x06000524 RID: 1316 RVA: 0x00017414 File Offset: 0x00015614
	public static bool IsMainCharacter(this CharacterMainControl character)
	{
		if (character == null)
		{
			return false;
		}
		LevelManager instance = LevelManager.Instance;
		return ((instance != null) ? instance.MainCharacter : null) == character;
	}
}
