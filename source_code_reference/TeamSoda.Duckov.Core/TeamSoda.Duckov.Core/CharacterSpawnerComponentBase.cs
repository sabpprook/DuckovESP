using System;
using UnityEngine;

// Token: 0x02000090 RID: 144
public abstract class CharacterSpawnerComponentBase : MonoBehaviour
{
	// Token: 0x060004F7 RID: 1271
	public abstract void Init(CharacterSpawnerRoot root);

	// Token: 0x060004F8 RID: 1272
	public abstract void StartSpawn();
}
