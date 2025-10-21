using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using UnityEngine;

// Token: 0x020000B0 RID: 176
public class SoulCollector : MonoBehaviour
{
	// Token: 0x060005CE RID: 1486 RVA: 0x00019E66 File Offset: 0x00018066
	private void Awake()
	{
		Health.OnDead += this.OnCharacterDie;
	}

	// Token: 0x060005CF RID: 1487 RVA: 0x00019E79 File Offset: 0x00018079
	private void OnDestroy()
	{
		Health.OnDead -= this.OnCharacterDie;
	}

	// Token: 0x060005D0 RID: 1488 RVA: 0x00019E8C File Offset: 0x0001808C
	private void Update()
	{
	}

	// Token: 0x060005D1 RID: 1489 RVA: 0x00019E90 File Offset: 0x00018090
	private void OnCharacterDie(Health health, DamageInfo dmgInfo)
	{
		if (!health)
		{
			return;
		}
		if (!health.hasSoul)
		{
			return;
		}
		if (!this.selfCharacter && this.selfAgent.Item)
		{
			this.selfCharacter = this.selfAgent.Item.GetCharacterMainControl();
		}
		if (!this.selfCharacter)
		{
			return;
		}
		if (Vector3.Distance(health.transform.position, this.selfCharacter.transform.position) > 40f)
		{
			return;
		}
		int num = Mathf.RoundToInt(health.MaxHealth / 15f);
		if (num < 1)
		{
			num = 1;
		}
		if (LevelManager.Rule.AdvancedDebuffMode)
		{
			num *= 3;
		}
		this.SpawnCubes(health.transform.position + Vector3.up * 0.75f, num).Forget();
	}

	// Token: 0x060005D2 RID: 1490 RVA: 0x00019F70 File Offset: 0x00018170
	private async UniTaskVoid SpawnCubes(Vector3 startPoint, int times)
	{
		if (!(this == null))
		{
			for (int i = 0; i < times; i++)
			{
				if (this == null)
				{
					break;
				}
				global::UnityEngine.Object.Instantiate<SoulCube>(this.cubePfb, startPoint, Quaternion.identity).Init(this);
				await UniTask.WaitForSeconds(0.05f, false, PlayerLoopTiming.Update, default(CancellationToken), false);
			}
		}
	}

	// Token: 0x060005D3 RID: 1491 RVA: 0x00019FC4 File Offset: 0x000181C4
	public void AddCube()
	{
		this.AddCubeAsync().Forget();
	}

	// Token: 0x060005D4 RID: 1492 RVA: 0x00019FE0 File Offset: 0x000181E0
	private async UniTaskVoid AddCubeAsync()
	{
		if (this.cubeSlot == null)
		{
			this.cubeSlot = this.selfAgent.Item.Slots["SoulCube"];
		}
		if (this.cubeSlot != null)
		{
			if (this.cubeSlot.Content != null)
			{
				if (this.cubeSlot.Content.StackCount >= this.cubeSlot.Content.MaxStackCount)
				{
					return;
				}
				this.cubeSlot.Content.StackCount++;
			}
			else
			{
				Item item = await ItemAssetsCollection.InstantiateAsync(this.soulCubeID);
				Item item2;
				this.cubeSlot.Plug(item, out item2);
			}
			global::UnityEngine.Object.Instantiate<GameObject>(this.addFx, base.transform, false);
		}
	}

	// Token: 0x0400054E RID: 1358
	public DuckovItemAgent selfAgent;

	// Token: 0x0400054F RID: 1359
	private CharacterMainControl selfCharacter;

	// Token: 0x04000550 RID: 1360
	[ItemTypeID]
	public int soulCubeID = 1165;

	// Token: 0x04000551 RID: 1361
	private Slot cubeSlot;

	// Token: 0x04000552 RID: 1362
	public GameObject addFx;

	// Token: 0x04000553 RID: 1363
	public SoulCube cubePfb;
}
