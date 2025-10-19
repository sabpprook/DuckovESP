using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000091 RID: 145
public class CharacterSpawnerGroup : CharacterSpawnerComponentBase
{
	// Token: 0x1700010A RID: 266
	// (get) Token: 0x060004FA RID: 1274 RVA: 0x00016655 File Offset: 0x00014855
	public AICharacterController LeaderAI
	{
		get
		{
			return this.leaderAI;
		}
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x0001665D File Offset: 0x0001485D
	public void Collect()
	{
		this.spawners = base.GetComponentsInChildren<RandomCharacterSpawner>().ToList<RandomCharacterSpawner>();
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x00016670 File Offset: 0x00014870
	public override void Init(CharacterSpawnerRoot root)
	{
		foreach (RandomCharacterSpawner randomCharacterSpawner in this.spawners)
		{
			if (randomCharacterSpawner == null)
			{
				Debug.LogError("生成器引用为空：" + base.gameObject.name);
			}
			else
			{
				randomCharacterSpawner.Init(root);
			}
		}
		this.spawnerRoot = root;
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x000166F0 File Offset: 0x000148F0
	public void Awake()
	{
		this.characters = new List<AICharacterController>();
		if (this.hasLeader && global::UnityEngine.Random.Range(0f, 1f) > this.hasLeaderChance)
		{
			this.hasLeader = false;
		}
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x00016724 File Offset: 0x00014924
	private void Update()
	{
		if (this.hasLeader && this.leaderAI == null && this.characters.Count > 0)
		{
			for (int i = 0; i < this.characters.Count; i++)
			{
				if (this.characters[i] == null)
				{
					this.characters.RemoveAt(i);
					i--;
				}
				else
				{
					this.leaderAI = this.characters[i];
				}
			}
		}
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x000167A3 File Offset: 0x000149A3
	public void AddCharacterSpawned(AICharacterController _character, bool isLeader)
	{
		_character.group = this;
		if (isLeader)
		{
			this.leaderAI = _character;
		}
		else if (this.hasLeader && !this.leaderAI)
		{
			this.leaderAI = _character;
		}
		this.characters.Add(_character);
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x000167E0 File Offset: 0x000149E0
	public override void StartSpawn()
	{
		bool flag = true;
		foreach (RandomCharacterSpawner randomCharacterSpawner in this.spawners)
		{
			if (!(randomCharacterSpawner == null))
			{
				randomCharacterSpawner.masterGroup = this;
				if (flag && this.hasLeader)
				{
					randomCharacterSpawner.firstIsLeader = true;
				}
				flag = false;
				randomCharacterSpawner.StartSpawn();
			}
		}
	}

	// Token: 0x0400046F RID: 1135
	public CharacterSpawnerRoot spawnerRoot;

	// Token: 0x04000470 RID: 1136
	public bool hasLeader;

	// Token: 0x04000471 RID: 1137
	[Range(0f, 1f)]
	public float hasLeaderChance = 1f;

	// Token: 0x04000472 RID: 1138
	public List<RandomCharacterSpawner> spawners;

	// Token: 0x04000473 RID: 1139
	private List<AICharacterController> characters;

	// Token: 0x04000474 RID: 1140
	private AICharacterController leaderAI;
}
