using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000094 RID: 148
[RequireComponent(typeof(Points))]
public class RandomCharacterSpawner : CharacterSpawnerComponentBase
{
	// Token: 0x1700010C RID: 268
	// (get) Token: 0x06000515 RID: 1301 RVA: 0x00017048 File Offset: 0x00015248
	private float minDistanceToMainCharacter
	{
		get
		{
			return this.spawnerRoot.minDistanceToPlayer;
		}
	}

	// Token: 0x1700010D RID: 269
	// (get) Token: 0x06000516 RID: 1302 RVA: 0x00017055 File Offset: 0x00015255
	private int scene
	{
		get
		{
			return this.spawnerRoot.RelatedScene;
		}
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x00017062 File Offset: 0x00015262
	private void ShowGizmo()
	{
		RandomCharacterSpawner.currentGizmosTag = this.gizmosTag;
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x0001706F File Offset: 0x0001526F
	public override void Init(CharacterSpawnerRoot root)
	{
		this.spawnerRoot = root;
		if (this.spawnPoints == null)
		{
			this.spawnPoints = base.GetComponent<Points>();
		}
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x00017092 File Offset: 0x00015292
	private void OnDestroy()
	{
		this.destroied = true;
	}

	// Token: 0x0600051A RID: 1306 RVA: 0x0001709C File Offset: 0x0001529C
	private CharacterRandomPresetInfo GetAPresetByWeight()
	{
		if (this.totalWeight < 0f)
		{
			this.totalWeight = 0f;
			for (int i = 0; i < this.randomPresetInfos.Count; i++)
			{
				if (this.randomPresetInfos[i].randomPreset == null)
				{
					this.randomPresetInfos.RemoveAt(i);
					i--;
					Debug.Log("Null preset");
				}
				else
				{
					this.totalWeight += this.randomPresetInfos[i].weight;
				}
			}
		}
		float num = global::UnityEngine.Random.Range(0f, this.totalWeight);
		float num2 = 0f;
		for (int j = 0; j < this.randomPresetInfos.Count; j++)
		{
			num2 += this.randomPresetInfos[j].weight;
			if (num < num2)
			{
				return this.randomPresetInfos[j];
			}
		}
		Debug.LogError("权重计算错误", base.gameObject);
		return this.randomPresetInfos[this.randomPresetInfos.Count - 1];
	}

	// Token: 0x0600051B RID: 1307 RVA: 0x000171A4 File Offset: 0x000153A4
	public override void StartSpawn()
	{
		this.CreateAsync().Forget();
	}

	// Token: 0x0600051C RID: 1308 RVA: 0x000171C0 File Offset: 0x000153C0
	private async UniTaskVoid CreateAsync()
	{
		if (LevelManager.Instance && LevelManager.Instance.IsBaseLevel)
		{
			this.delayTime = 0.5f;
		}
		if (!(LevelManager.Instance == null))
		{
			if (!(this.spawnPoints == null))
			{
				UnityEvent onStartCreateEvent = this.OnStartCreateEvent;
				if (onStartCreateEvent != null)
				{
					onStartCreateEvent.Invoke();
				}
				int num = global::UnityEngine.Random.Range(this.spawnCountRange.x, this.spawnCountRange.y + 1);
				this.targetSpawnCount = num;
				List<Vector3> randomPoints = this.spawnPoints.GetRandomPoints(num);
				foreach (Vector3 vector in randomPoints)
				{
					bool flag = false;
					if (!this.firstCreateStarted)
					{
						flag = true;
						this.firstCreateStarted = true;
					}
					this.CreateAt(vector, this.scene, this.masterGroup, flag && this.firstIsLeader).Forget<CharacterMainControl>();
					this.currentSpawnedCount++;
					await UniTask.WaitForSeconds(0.1f, true, PlayerLoopTiming.Update, default(CancellationToken), false);
				}
				List<Vector3>.Enumerator enumerator = default(List<Vector3>.Enumerator);
			}
		}
	}

	// Token: 0x0600051D RID: 1309 RVA: 0x00017204 File Offset: 0x00015404
	private async UniTask<CharacterMainControl> CreateAt(Vector3 point, int scene, CharacterSpawnerGroup group, bool isLeader)
	{
		CharacterMainControl characterMainControl;
		if (this.randomPresetInfos.Count <= 0)
		{
			characterMainControl = null;
		}
		else
		{
			Vector3 direction = global::UnityEngine.Random.insideUnitCircle.normalized;
			direction.z = direction.y;
			direction.y = 0f;
			while (CharacterMainControl.Main && Vector3.Distance(point, CharacterMainControl.Main.transform.position) < this.minDistanceToMainCharacter)
			{
				await UniTask.Yield();
			}
			if (this.destroied || base.gameObject == null || !LevelManager.Instance || CharacterMainControl.Main == null)
			{
				characterMainControl = null;
			}
			else
			{
				if (this.isStaticTarget)
				{
					direction = base.transform.forward;
				}
				CharacterMainControl characterMainControl2 = await this.GetAPresetByWeight().randomPreset.CreateCharacterAsync(point, direction, scene, group, isLeader);
				if (this.isStaticTarget)
				{
					Rigidbody component = characterMainControl2.GetComponent<Rigidbody>();
					component.collisionDetectionMode = CollisionDetectionMode.Discrete;
					component.isKinematic = true;
				}
				this.spawnerRoot.AddCreatedCharacter(characterMainControl2);
				characterMainControl = characterMainControl2;
			}
		}
		return characterMainControl;
	}

	// Token: 0x0600051E RID: 1310 RVA: 0x00017268 File Offset: 0x00015468
	private void OnDrawGizmos()
	{
		if (RandomCharacterSpawner.currentGizmosTag != this.gizmosTag)
		{
			return;
		}
		Gizmos.color = Color.yellow;
		if (this.spawnPoints && this.spawnPoints.points.Count > 0)
		{
			Vector3 point = this.spawnPoints.GetPoint(0);
			Vector3 vector = point + Vector3.up * 20f;
			Gizmos.DrawWireSphere(point, 10f);
			Gizmos.DrawLine(point, vector);
			Gizmos.DrawSphere(vector, 3f);
		}
	}

	// Token: 0x04000493 RID: 1171
	public Points spawnPoints;

	// Token: 0x04000494 RID: 1172
	public CharacterSpawnerRoot spawnerRoot;

	// Token: 0x04000495 RID: 1173
	public CharacterSpawnerGroup masterGroup;

	// Token: 0x04000496 RID: 1174
	public List<CharacterRandomPresetInfo> randomPresetInfos;

	// Token: 0x04000497 RID: 1175
	private float delayTime = 1f;

	// Token: 0x04000498 RID: 1176
	public Vector2Int spawnCountRange;

	// Token: 0x04000499 RID: 1177
	private float totalWeight = -1f;

	// Token: 0x0400049A RID: 1178
	public bool isStaticTarget;

	// Token: 0x0400049B RID: 1179
	public static string currentGizmosTag;

	// Token: 0x0400049C RID: 1180
	public bool firstIsLeader;

	// Token: 0x0400049D RID: 1181
	private bool firstCreateStarted;

	// Token: 0x0400049E RID: 1182
	public UnityEvent OnStartCreateEvent;

	// Token: 0x0400049F RID: 1183
	private int targetSpawnCount;

	// Token: 0x040004A0 RID: 1184
	private int currentSpawnedCount;

	// Token: 0x040004A1 RID: 1185
	private bool destroied;

	// Token: 0x040004A2 RID: 1186
	public string gizmosTag;
}
