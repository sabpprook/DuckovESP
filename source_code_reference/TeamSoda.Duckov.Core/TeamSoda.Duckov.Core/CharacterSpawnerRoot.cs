using System;
using System.Collections.Generic;
using Duckov.Scenes;
using Duckov.Weathers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

// Token: 0x02000093 RID: 147
public class CharacterSpawnerRoot : MonoBehaviour
{
	// Token: 0x1700010B RID: 267
	// (get) Token: 0x06000508 RID: 1288 RVA: 0x00016AA1 File Offset: 0x00014CA1
	public int RelatedScene
	{
		get
		{
			return this.relatedScene;
		}
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x00016AAC File Offset: 0x00014CAC
	private void Awake()
	{
		if (this.createdCharacters == null)
		{
			this.createdCharacters = new List<CharacterMainControl>();
		}
		if (this.despawningCharacters == null)
		{
			this.despawningCharacters = new List<CharacterMainControl>();
		}
		if (!this.useTimeOfDay && !this.checkWeather)
		{
			this.despawnIfTimingWrong = false;
		}
		if (this.needTrigger && this.trigger)
		{
			this.trigger.triggerOnce = false;
			this.trigger.onlyMainCharacter = true;
			this.trigger.DoOnTriggerEnter.AddListener(new UnityAction(this.DoOnTriggerEnter));
			this.trigger.DoOnTriggerExit.AddListener(new UnityAction(this.DoOnTriggerLeave));
		}
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x00016B5C File Offset: 0x00014D5C
	private void OnDestroy()
	{
		if (this.needTrigger && this.trigger)
		{
			this.trigger.DoOnTriggerEnter.RemoveListener(new UnityAction(this.DoOnTriggerEnter));
			this.trigger.DoOnTriggerExit.RemoveListener(new UnityAction(this.DoOnTriggerLeave));
		}
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x00016BB6 File Offset: 0x00014DB6
	private void Start()
	{
		if (LevelManager.Instance && LevelManager.Instance.IsBaseLevel)
		{
			this.minDistanceToPlayer = 0f;
		}
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x00016BDC File Offset: 0x00014DDC
	private void Update()
	{
		if (!this.inited && LevelManager.LevelInited)
		{
			this.Init();
		}
		bool flag = this.CheckTiming();
		if (this.inited && !this.created && flag)
		{
			this.StartSpawn();
		}
		if (this.created && !flag && this.despawnIfTimingWrong)
		{
			this.despawningCharacters.AddRange(this.createdCharacters);
			this.createdCharacters.Clear();
			this.created = false;
		}
		this.despawnTickTimer -= Time.deltaTime;
		if (this.despawnTickTimer < 0f && this.despawnIfTimingWrong && this.despawningCharacters.Count > 0)
		{
			this.CheckDespawn();
		}
		if (this.despawnTickTimer < 0f && !this.allDead && this.stillhasAliveCharacters && !this.allDeadEventInvoked)
		{
			if (this.createdCharacters.Count <= 0)
			{
				this.allDead = true;
			}
			else
			{
				this.allDead = true;
				foreach (CharacterMainControl characterMainControl in this.createdCharacters)
				{
					if (characterMainControl != null && characterMainControl.Health && !characterMainControl.Health.IsDead)
					{
						this.allDead = false;
						break;
					}
				}
			}
			if (this.allDead)
			{
				this.stillhasAliveCharacters = false;
				UnityEvent onAllDeadEvent = this.OnAllDeadEvent;
				if (onAllDeadEvent != null)
				{
					onAllDeadEvent.Invoke();
				}
				this.allDeadEventInvoked = true;
			}
		}
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x00016D78 File Offset: 0x00014F78
	private void CheckDespawn()
	{
		for (int i = 0; i < this.despawningCharacters.Count; i++)
		{
			CharacterMainControl characterMainControl = this.despawningCharacters[i];
			if (!characterMainControl)
			{
				this.despawningCharacters.RemoveAt(i);
				i--;
			}
			else if (!characterMainControl.gameObject.activeInHierarchy)
			{
				global::UnityEngine.Object.Destroy(characterMainControl.gameObject);
				this.despawningCharacters.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x00016DEC File Offset: 0x00014FEC
	private bool CheckTiming()
	{
		if (LevelManager.Instance == null)
		{
			return false;
		}
		if (this.needTrigger && !this.playerInTrigger)
		{
			return false;
		}
		bool flag;
		if (this.useTimeOfDay)
		{
			float num = (float)GameClock.TimeOfDay.TotalHours % 24f;
			flag = (num >= this.spawnTimeRangeFrom && num <= this.spawnTimeRangeTo) || (this.spawnTimeRangeTo < this.spawnTimeRangeFrom && (num >= this.spawnTimeRangeFrom || num <= this.spawnTimeRangeTo));
		}
		else
		{
			flag = LevelManager.Instance.LevelTime >= this.whenToSpawn;
		}
		bool flag2 = true;
		if (this.checkWeather && !this.targetWeathers.Contains(TimeOfDayController.Instance.CurrentWeather))
		{
			flag2 = false;
		}
		return flag && flag2;
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x00016EB8 File Offset: 0x000150B8
	private void Init()
	{
		this.inited = true;
		this.spawnerComponent.Init(this);
		int buildIndex = SceneManager.GetActiveScene().buildIndex;
		bool flag = true;
		if (MultiSceneCore.Instance != null)
		{
			flag = MultiSceneCore.Instance.usedCreatorIds.Contains(this.SpawnerGuid);
		}
		if (flag)
		{
			Debug.Log("Contain this spawner");
			global::UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		this.relatedScene = SceneManager.GetActiveScene().buildIndex;
		base.transform.SetParent(null);
		MultiSceneCore.MoveToMainScene(base.gameObject);
		MultiSceneCore.Instance.usedCreatorIds.Add(this.SpawnerGuid);
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x00016F68 File Offset: 0x00015168
	private void StartSpawn()
	{
		if (this.created)
		{
			return;
		}
		this.created = true;
		if (global::UnityEngine.Random.Range(0f, 1f) > this.spawnChance)
		{
			return;
		}
		UnityEvent onStartEvent = this.OnStartEvent;
		if (onStartEvent != null)
		{
			onStartEvent.Invoke();
		}
		if (this.spawnerComponent)
		{
			this.spawnerComponent.StartSpawn();
		}
	}

	// Token: 0x06000511 RID: 1297 RVA: 0x00016FC6 File Offset: 0x000151C6
	private void DoOnTriggerEnter()
	{
		this.playerInTrigger = true;
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x00016FCF File Offset: 0x000151CF
	private void DoOnTriggerLeave()
	{
		this.playerInTrigger = false;
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x00016FD8 File Offset: 0x000151D8
	public void AddCreatedCharacter(CharacterMainControl c)
	{
		this.createdCharacters.Add(c);
		this.stillhasAliveCharacters = true;
	}

	// Token: 0x04000479 RID: 1145
	public bool needTrigger;

	// Token: 0x0400047A RID: 1146
	public OnTriggerEnterEvent trigger;

	// Token: 0x0400047B RID: 1147
	private bool playerInTrigger;

	// Token: 0x0400047C RID: 1148
	private bool created;

	// Token: 0x0400047D RID: 1149
	private bool inited;

	// Token: 0x0400047E RID: 1150
	[Range(0f, 1f)]
	public float spawnChance = 1f;

	// Token: 0x0400047F RID: 1151
	public float minDistanceToPlayer = 25f;

	// Token: 0x04000480 RID: 1152
	public bool useTimeOfDay;

	// Token: 0x04000481 RID: 1153
	public float whenToSpawn;

	// Token: 0x04000482 RID: 1154
	[Range(0f, 24f)]
	public float spawnTimeRangeFrom;

	// Token: 0x04000483 RID: 1155
	[Range(0f, 24f)]
	public float spawnTimeRangeTo;

	// Token: 0x04000484 RID: 1156
	[FormerlySerializedAs("despawnIfOutOfTime")]
	public bool despawnIfTimingWrong;

	// Token: 0x04000485 RID: 1157
	public bool checkWeather;

	// Token: 0x04000486 RID: 1158
	public List<Weather> targetWeathers;

	// Token: 0x04000487 RID: 1159
	private int relatedScene = -1;

	// Token: 0x04000488 RID: 1160
	[SerializeField]
	private CharacterSpawnerComponentBase spawnerComponent;

	// Token: 0x04000489 RID: 1161
	public bool autoRefreshGuid = true;

	// Token: 0x0400048A RID: 1162
	public int SpawnerGuid;

	// Token: 0x0400048B RID: 1163
	private List<CharacterMainControl> createdCharacters = new List<CharacterMainControl>();

	// Token: 0x0400048C RID: 1164
	private List<CharacterMainControl> despawningCharacters = new List<CharacterMainControl>();

	// Token: 0x0400048D RID: 1165
	private float despawnTickTimer = 1f;

	// Token: 0x0400048E RID: 1166
	public UnityEvent OnStartEvent;

	// Token: 0x0400048F RID: 1167
	public UnityEvent OnAllDeadEvent;

	// Token: 0x04000490 RID: 1168
	private bool allDeadEventInvoked;

	// Token: 0x04000491 RID: 1169
	private bool stillhasAliveCharacters;

	// Token: 0x04000492 RID: 1170
	private bool allDead;
}
