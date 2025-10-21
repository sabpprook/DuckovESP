using System;
using UnityEngine;

// Token: 0x0200010E RID: 270
public class PetHouse : MonoBehaviour
{
	// Token: 0x170001F0 RID: 496
	// (get) Token: 0x0600093D RID: 2365 RVA: 0x00028E0C File Offset: 0x0002700C
	public static PetHouse Instance
	{
		get
		{
			return PetHouse.instance;
		}
	}

	// Token: 0x0600093E RID: 2366 RVA: 0x00028E13 File Offset: 0x00027013
	private void Awake()
	{
		PetHouse.instance = this;
		if (LevelManager.LevelInited)
		{
			this.OnLevelInited();
			return;
		}
		LevelManager.OnLevelInitialized += this.OnLevelInited;
	}

	// Token: 0x0600093F RID: 2367 RVA: 0x00028E3A File Offset: 0x0002703A
	private void OnDestroy()
	{
		LevelManager.OnLevelInitialized -= this.OnLevelInited;
		if (this.petTarget)
		{
			this.petTarget.SetStandBy(false, this.petTarget.transform.position);
		}
	}

	// Token: 0x06000940 RID: 2368 RVA: 0x00028E78 File Offset: 0x00027078
	private void OnLevelInited()
	{
		CharacterMainControl petCharacter = LevelManager.Instance.PetCharacter;
		petCharacter.SetPosition(this.petMarker.position);
		this.petTarget = petCharacter.GetComponentInChildren<PetAI>();
		if (this.petTarget != null)
		{
			this.petTarget.SetStandBy(true, this.petMarker.position);
		}
	}

	// Token: 0x0400083D RID: 2109
	private static PetHouse instance;

	// Token: 0x0400083E RID: 2110
	public Transform petMarker;

	// Token: 0x0400083F RID: 2111
	private PetAI petTarget;
}
