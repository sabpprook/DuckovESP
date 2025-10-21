using System;
using Duckov.Options;
using SodaCraft.Localizations;

// Token: 0x020001D3 RID: 467
public class MoveDirectionOptions : OptionsProviderBase
{
	// Token: 0x1700028A RID: 650
	// (get) Token: 0x06000DDD RID: 3549 RVA: 0x00038989 File Offset: 0x00036B89
	public override string Key
	{
		get
		{
			return "MoveDirModeSettings";
		}
	}

	// Token: 0x1700028B RID: 651
	// (get) Token: 0x06000DDE RID: 3550 RVA: 0x00038990 File Offset: 0x00036B90
	public static bool MoveViaCharacterDirection
	{
		get
		{
			return MoveDirectionOptions.moveViaCharacterDirection;
		}
	}

	// Token: 0x06000DDF RID: 3551 RVA: 0x00038997 File Offset: 0x00036B97
	public override string[] GetOptions()
	{
		return new string[]
		{
			this.cameraModeKey.ToPlainText(),
			this.aimModeKey.ToPlainText()
		};
	}

	// Token: 0x06000DE0 RID: 3552 RVA: 0x000389BC File Offset: 0x00036BBC
	public override string GetCurrentOption()
	{
		int num = OptionsManager.Load<int>(this.Key, 0);
		if (num == 0)
		{
			return this.cameraModeKey.ToPlainText();
		}
		if (num != 1)
		{
			return this.cameraModeKey.ToPlainText();
		}
		return this.aimModeKey.ToPlainText();
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x00038A02 File Offset: 0x00036C02
	public override void Set(int index)
	{
		if (index != 0)
		{
			if (index == 1)
			{
				MoveDirectionOptions.moveViaCharacterDirection = true;
			}
		}
		else
		{
			MoveDirectionOptions.moveViaCharacterDirection = false;
		}
		OptionsManager.Save<int>(this.Key, index);
	}

	// Token: 0x06000DE2 RID: 3554 RVA: 0x00038A27 File Offset: 0x00036C27
	private void Awake()
	{
		LevelManager.OnLevelInitialized += this.RefreshOnLevelInited;
	}

	// Token: 0x06000DE3 RID: 3555 RVA: 0x00038A3A File Offset: 0x00036C3A
	private void OnDestroy()
	{
		LevelManager.OnLevelInitialized -= this.RefreshOnLevelInited;
	}

	// Token: 0x06000DE4 RID: 3556 RVA: 0x00038A50 File Offset: 0x00036C50
	private void RefreshOnLevelInited()
	{
		int num = OptionsManager.Load<int>(this.Key, 0);
		this.Set(num);
	}

	// Token: 0x04000BA1 RID: 2977
	[LocalizationKey("Default")]
	public string cameraModeKey = "MoveDirectionMode_Camera";

	// Token: 0x04000BA2 RID: 2978
	[LocalizationKey("Default")]
	public string aimModeKey = "MoveDirectionMode_Aim";

	// Token: 0x04000BA3 RID: 2979
	private static bool moveViaCharacterDirection;
}
