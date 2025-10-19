using System;
using Cysharp.Threading.Tasks;
using Duckov.Scenes;
using UnityEngine;

// Token: 0x0200009E RID: 158
public class Egg : MonoBehaviour
{
	// Token: 0x06000548 RID: 1352 RVA: 0x00017ACA File Offset: 0x00015CCA
	private void Start()
	{
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x00017ACC File Offset: 0x00015CCC
	public void Init(Vector3 spawnPosition, Vector3 spawnVelocity, CharacterMainControl _fromCharacter, CharacterRandomPreset preset, float _life)
	{
		this.characterPreset = preset;
		base.transform.position = spawnPosition;
		if (this.rb)
		{
			this.rb.position = spawnPosition;
			this.rb.velocity = spawnVelocity;
		}
		this.fromCharacter = _fromCharacter;
		this.life = _life;
		this.inited = true;
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x00017B28 File Offset: 0x00015D28
	private async UniTaskVoid Spawn()
	{
		if (this.spawnFx)
		{
			global::UnityEngine.Object.Instantiate<GameObject>(this.spawnFx, base.transform.position, Quaternion.identity);
		}
		if (!this.fromCharacter)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			bool isMainCharacter = this.fromCharacter.IsMainCharacter;
			object obj = await this.characterPreset.CreateCharacterAsync(base.transform.position + Vector3.down * 0.25f, Vector3.forward, MultiSceneCore.MainScene.Value.buildIndex, null, false);
			AICharacterController componentInChildren = obj.GetComponentInChildren<AICharacterController>();
			obj.SetPosition(base.transform.position + Vector3.down * 0.25f);
			if (componentInChildren)
			{
				PetAI component = componentInChildren.GetComponent<PetAI>();
				if (component)
				{
					component.SetMaster(this.fromCharacter);
				}
				componentInChildren.leader = this.fromCharacter;
				if (this.fromCharacter)
				{
					componentInChildren.CharacterMainControl.SetTeam(this.fromCharacter.Team);
				}
			}
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x00017B6C File Offset: 0x00015D6C
	private void Update()
	{
		if (!this.inited)
		{
			return;
		}
		this.timer += Time.deltaTime;
		if (this.timer > this.life && !this.spawned)
		{
			this.spawned = true;
			this.Spawn().Forget();
		}
	}

	// Token: 0x040004BB RID: 1211
	public GameObject spawnFx;

	// Token: 0x040004BC RID: 1212
	public CharacterMainControl fromCharacter;

	// Token: 0x040004BD RID: 1213
	public Rigidbody rb;

	// Token: 0x040004BE RID: 1214
	private float life;

	// Token: 0x040004BF RID: 1215
	private CharacterRandomPreset characterPreset;

	// Token: 0x040004C0 RID: 1216
	private bool inited;

	// Token: 0x040004C1 RID: 1217
	private float timer;

	// Token: 0x040004C2 RID: 1218
	private bool spawned;
}
