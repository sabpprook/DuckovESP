using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000092 RID: 146
public class CharacterSpawnerGroupSelector : CharacterSpawnerComponentBase
{
	// Token: 0x06000502 RID: 1282 RVA: 0x0001686C File Offset: 0x00014A6C
	public void Collect()
	{
		this.groups = base.GetComponentsInChildren<CharacterSpawnerGroup>().ToList<CharacterSpawnerGroup>();
		foreach (CharacterSpawnerGroup characterSpawnerGroup in this.groups)
		{
			characterSpawnerGroup.Collect();
		}
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x000168D0 File Offset: 0x00014AD0
	public override void Init(CharacterSpawnerRoot root)
	{
		foreach (CharacterSpawnerGroup characterSpawnerGroup in this.groups)
		{
			if (characterSpawnerGroup == null)
			{
				Debug.LogError("生成器引用为空");
			}
			else
			{
				characterSpawnerGroup.Init(root);
			}
		}
		this.spawnerRoot = root;
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x00016940 File Offset: 0x00014B40
	public override void StartSpawn()
	{
		if (this.spawnGroupCountRange.y > this.groups.Count)
		{
			this.spawnGroupCountRange.y = this.groups.Count;
		}
		if (this.spawnGroupCountRange.x > this.groups.Count)
		{
			this.spawnGroupCountRange.x = this.groups.Count;
		}
		int num = global::UnityEngine.Random.Range(this.spawnGroupCountRange.x, this.spawnGroupCountRange.y);
		this.finalCount = num;
		this.RandomSpawn(num);
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x000169D3 File Offset: 0x00014BD3
	private void OnValidate()
	{
		if (this.groups.Count < 0)
		{
			return;
		}
		if (this.spawnGroupCountRange.x > this.spawnGroupCountRange.y)
		{
			this.spawnGroupCountRange.y = this.spawnGroupCountRange.x;
		}
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x00016A14 File Offset: 0x00014C14
	public void RandomSpawn(int count)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.groups.Count; i++)
		{
			list.Add(i);
		}
		for (int j = 0; j < count; j++)
		{
			int num = global::UnityEngine.Random.Range(0, list.Count);
			int num2 = list[num];
			list.RemoveAt(num);
			CharacterSpawnerGroup characterSpawnerGroup = this.groups[num2];
			if (characterSpawnerGroup)
			{
				characterSpawnerGroup.StartSpawn();
			}
		}
	}

	// Token: 0x04000475 RID: 1141
	public CharacterSpawnerRoot spawnerRoot;

	// Token: 0x04000476 RID: 1142
	public List<CharacterSpawnerGroup> groups;

	// Token: 0x04000477 RID: 1143
	public Vector2Int spawnGroupCountRange = new Vector2Int(1, 1);

	// Token: 0x04000478 RID: 1144
	private int finalCount;
}
