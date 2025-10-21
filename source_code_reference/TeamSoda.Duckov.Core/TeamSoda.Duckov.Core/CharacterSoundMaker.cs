using System;
using UnityEngine;

// Token: 0x0200005F RID: 95
public class CharacterSoundMaker : MonoBehaviour
{
	// Token: 0x170000CC RID: 204
	// (get) Token: 0x06000387 RID: 903 RVA: 0x0000F661 File Offset: 0x0000D861
	public float walkSoundDistance
	{
		get
		{
			if (!this.characterMainControl)
			{
				return 0f;
			}
			return this.characterMainControl.WalkSoundRange;
		}
	}

	// Token: 0x170000CD RID: 205
	// (get) Token: 0x06000388 RID: 904 RVA: 0x0000F681 File Offset: 0x0000D881
	public float runSoundDistance
	{
		get
		{
			if (!this.characterMainControl)
			{
				return 0f;
			}
			return this.characterMainControl.RunSoundRange;
		}
	}

	// Token: 0x06000389 RID: 905 RVA: 0x0000F6A4 File Offset: 0x0000D8A4
	private void Update()
	{
		if (this.characterMainControl.movementControl.Velocity.magnitude < 0.5f)
		{
			this.moveSoundTimer = 0f;
			return;
		}
		this.moveSoundTimer += Time.deltaTime;
		bool running = this.characterMainControl.Running;
		float num = 1f / (running ? this.runSoundFrequence : this.walkSoundFrequence);
		if (this.moveSoundTimer >= num)
		{
			this.moveSoundTimer = 0f;
			if (this.characterMainControl.IsInAdsInput)
			{
				return;
			}
			if (!this.characterMainControl.CharacterItem)
			{
				return;
			}
			bool flag = this.characterMainControl.CharacterItem.TotalWeight / this.characterMainControl.MaxWeight >= 0.75f;
			AISound aisound = default(AISound);
			aisound.pos = base.transform.position;
			aisound.fromTeam = this.characterMainControl.Team;
			aisound.soundType = SoundTypes.unknowNoise;
			aisound.fromObject = this.characterMainControl.gameObject;
			aisound.fromCharacter = this.characterMainControl;
			if (this.characterMainControl.Running)
			{
				if (this.runSoundDistance > 0f)
				{
					aisound.radius = this.runSoundDistance * (flag ? 1.5f : 1f);
					Action<Vector3, CharacterSoundMaker.FootStepTypes, CharacterMainControl> onFootStepSound = CharacterSoundMaker.OnFootStepSound;
					if (onFootStepSound != null)
					{
						onFootStepSound(base.transform.position, flag ? CharacterSoundMaker.FootStepTypes.runHeavy : CharacterSoundMaker.FootStepTypes.runLight, this.characterMainControl);
					}
				}
			}
			else if (this.walkSoundDistance > 0f)
			{
				aisound.radius = this.walkSoundDistance * (flag ? 1.5f : 1f);
				Action<Vector3, CharacterSoundMaker.FootStepTypes, CharacterMainControl> onFootStepSound2 = CharacterSoundMaker.OnFootStepSound;
				if (onFootStepSound2 != null)
				{
					onFootStepSound2(base.transform.position, flag ? CharacterSoundMaker.FootStepTypes.walkHeavy : CharacterSoundMaker.FootStepTypes.walkLight, this.characterMainControl);
				}
			}
			AIMainBrain.MakeSound(aisound);
		}
	}

	// Token: 0x040002A5 RID: 677
	public CharacterMainControl characterMainControl;

	// Token: 0x040002A6 RID: 678
	private float moveSoundTimer;

	// Token: 0x040002A7 RID: 679
	public float walkSoundFrequence = 4f;

	// Token: 0x040002A8 RID: 680
	public float runSoundFrequence = 7f;

	// Token: 0x040002A9 RID: 681
	public static Action<Vector3, CharacterSoundMaker.FootStepTypes, CharacterMainControl> OnFootStepSound;

	// Token: 0x0200042F RID: 1071
	public enum FootStepTypes
	{
		// Token: 0x04001A0F RID: 6671
		walkLight,
		// Token: 0x04001A10 RID: 6672
		walkHeavy,
		// Token: 0x04001A11 RID: 6673
		runLight,
		// Token: 0x04001A12 RID: 6674
		runHeavy
	}
}
