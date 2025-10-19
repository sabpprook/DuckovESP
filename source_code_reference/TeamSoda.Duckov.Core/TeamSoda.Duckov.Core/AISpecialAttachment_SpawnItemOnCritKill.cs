using System;
using Cysharp.Threading.Tasks;
using ItemStatsSystem;
using UnityEngine;

// Token: 0x0200008C RID: 140
public class AISpecialAttachment_SpawnItemOnCritKill : AISpecialAttachmentBase
{
	// Token: 0x060004E5 RID: 1253 RVA: 0x000160E4 File Offset: 0x000142E4
	protected override void OnInited()
	{
		this.character.BeforeCharacterSpawnLootOnDead += this.BeforeCharacterSpawnLootOnDead;
		this.SpawnItem().Forget();
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x00016118 File Offset: 0x00014318
	private async UniTaskVoid SpawnItem()
	{
		Item item = await ItemAssetsCollection.InstantiateAsync(this.itemToSpawn);
		this.itemInstance = item;
		this.itemInstance.transform.SetParent(base.transform, false);
		if (this.hasDead)
		{
			global::UnityEngine.Object.Destroy(this.itemInstance.gameObject);
		}
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x0001615B File Offset: 0x0001435B
	private void OnDestroy()
	{
		if (this.character)
		{
			this.character.BeforeCharacterSpawnLootOnDead -= this.BeforeCharacterSpawnLootOnDead;
		}
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x00016184 File Offset: 0x00014384
	private void BeforeCharacterSpawnLootOnDead(DamageInfo dmgInfo)
	{
		this.hasDead = true;
		Debug.Log(string.Format("Die crit:{0}", dmgInfo.crit));
		bool flag = dmgInfo.crit > 0;
		if (this.inverse == flag || this.character == null)
		{
			if (this.itemInstance != null)
			{
				global::UnityEngine.Object.Destroy(this.itemInstance.gameObject);
			}
			return;
		}
		Debug.Log("pick up on crit");
		if (this.itemInstance != null)
		{
			this.character.CharacterItem.Inventory.AddAndMerge(this.itemInstance, 0);
		}
	}

	// Token: 0x0400041B RID: 1051
	[ItemTypeID]
	public int itemToSpawn;

	// Token: 0x0400041C RID: 1052
	private Item itemInstance;

	// Token: 0x0400041D RID: 1053
	private bool hasDead;

	// Token: 0x0400041E RID: 1054
	public bool inverse;
}
