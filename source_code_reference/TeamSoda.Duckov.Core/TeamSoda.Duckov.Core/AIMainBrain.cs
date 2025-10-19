using System;
using System.Collections.Generic;
using Duckov.Utilities;
using ParadoxNotion;
using UnityEngine;

// Token: 0x020000FA RID: 250
public class AIMainBrain : MonoBehaviour
{
	// Token: 0x170001B8 RID: 440
	// (get) Token: 0x06000857 RID: 2135 RVA: 0x000250D4 File Offset: 0x000232D4
	private static CharacterMainControl mainCharacter
	{
		get
		{
			if (AIMainBrain._mc == null)
			{
				AIMainBrain._mc = CharacterMainControl.Main;
			}
			return AIMainBrain._mc;
		}
	}

	// Token: 0x14000039 RID: 57
	// (add) Token: 0x06000858 RID: 2136 RVA: 0x000250F4 File Offset: 0x000232F4
	// (remove) Token: 0x06000859 RID: 2137 RVA: 0x00025128 File Offset: 0x00023328
	public static event Action<AISound> OnSoundSpawned;

	// Token: 0x1400003A RID: 58
	// (add) Token: 0x0600085A RID: 2138 RVA: 0x0002515C File Offset: 0x0002335C
	// (remove) Token: 0x0600085B RID: 2139 RVA: 0x00025190 File Offset: 0x00023390
	public static event Action<AISound> OnPlayerHearSound;

	// Token: 0x0600085C RID: 2140 RVA: 0x000251C3 File Offset: 0x000233C3
	public static void MakeSound(AISound sound)
	{
		Action<AISound> onSoundSpawned = AIMainBrain.OnSoundSpawned;
		if (onSoundSpawned != null)
		{
			onSoundSpawned(sound);
		}
		AIMainBrain.FilterPlayerHearSound(sound);
	}

	// Token: 0x0600085D RID: 2141 RVA: 0x000251DC File Offset: 0x000233DC
	private static void FilterPlayerHearSound(AISound sound)
	{
		if (!AIMainBrain.mainCharacter)
		{
			return;
		}
		if (!Team.IsEnemy(Teams.player, sound.fromTeam))
		{
			return;
		}
		if (sound.fromCharacter && sound.fromCharacter.characterModel && !sound.fromCharacter.characterModel.Hidden && !GameCamera.Instance.IsOffScreen(sound.pos))
		{
			return;
		}
		float num = Vector3.Distance(sound.pos, AIMainBrain.mainCharacter.transform.position);
		if (AIMainBrain.mainCharacter.SoundVisable < 0.2f)
		{
			return;
		}
		float hearingAbility = AIMainBrain.mainCharacter.HearingAbility;
		if (num > sound.radius * hearingAbility)
		{
			return;
		}
		Action<AISound> onPlayerHearSound = AIMainBrain.OnPlayerHearSound;
		if (onPlayerHearSound == null)
		{
			return;
		}
		onPlayerHearSound(sound);
	}

	// Token: 0x0600085E RID: 2142 RVA: 0x0002529D File Offset: 0x0002349D
	public void Awake()
	{
		this.searchTasks = new Queue<AIMainBrain.SearchTaskContext>();
		this.checkObsticleTasks = new Queue<AIMainBrain.CheckObsticleTaskContext>();
		this.fowBlockLayer = LayerMask.NameToLayer("FowBlock");
	}

	// Token: 0x0600085F RID: 2143 RVA: 0x000252C8 File Offset: 0x000234C8
	private void Start()
	{
		this.dmgReceiverLayers = GameplayDataSettings.Layers.damageReceiverLayerMask;
		this.interactLayers = 1 << LayerMask.NameToLayer("Interactable");
		this.obsticleLayers = GameplayDataSettings.Layers.fowBlockLayers;
		this.obsticleLayersWithThermal = GameplayDataSettings.Layers.fowBlockLayersWithThermal;
		this.cols = new Collider[15];
		this.ObsHits = new RaycastHit[15];
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x0002533C File Offset: 0x0002353C
	private void Update()
	{
		int num = 0;
		while (num < this.maxSeachCount && this.searchTasks.Count > 0)
		{
			this.DoSearch(this.searchTasks.Dequeue());
			num++;
		}
		int num2 = 0;
		while (num2 < this.maxCheckObsticleCount && this.checkObsticleTasks.Count > 0)
		{
			this.DoCheckObsticle(this.checkObsticleTasks.Dequeue());
			num2++;
		}
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x000253AC File Offset: 0x000235AC
	private void DoSearch(AIMainBrain.SearchTaskContext context)
	{
		int num = Physics.OverlapSphereNonAlloc(context.searchCenter, context.searchDistance, this.cols, (context.searchPickupID > 0) ? (this.dmgReceiverLayers | this.interactLayers) : this.dmgReceiverLayers, QueryTriggerInteraction.Collide);
		if (num <= 0)
		{
			context.onSearchFinishedCallback(null, null);
			return;
		}
		float num2 = 9999f;
		DamageReceiver damageReceiver = null;
		float num3 = 9999f;
		InteractablePickup interactablePickup = null;
		float num4 = 1.5f;
		for (int i = 0; i < num; i++)
		{
			Collider collider = this.cols[i];
			if (Mathf.Abs(context.searchCenter.y - collider.transform.position.y) <= 4f)
			{
				float num5 = Vector3.Distance(context.searchCenter, collider.transform.position);
				if (Vector3.Angle(context.searchDirection.normalized, (collider.transform.position - context.searchCenter).normalized) <= context.searchAngle * 0.5f || num5 <= num4)
				{
					this.dmgReceiverTemp = null;
					float num6 = 1f;
					if (collider.gameObject.IsInLayerMask(this.dmgReceiverLayers))
					{
						this.dmgReceiverTemp = collider.GetComponent<DamageReceiver>();
						if (this.dmgReceiverTemp != null && this.dmgReceiverTemp.health)
						{
							CharacterMainControl characterMainControl = this.dmgReceiverTemp.health.TryGetCharacter();
							if (characterMainControl)
							{
								num6 = characterMainControl.VisableDistanceFactor;
							}
						}
					}
					if (num5 <= context.searchDistance * num6 && (num5 < num2 || num5 < num3) && (!context.checkObsticle || num5 <= num4 || !this.CheckObsticle(context.searchCenter, collider.transform.position + Vector3.up * 1.5f, context.thermalOn, context.ignoreFowBlockLayer)))
					{
						if (this.dmgReceiverTemp)
						{
							if (!(this.dmgReceiverTemp.health == null) && Team.IsEnemy(context.selfTeam, this.dmgReceiverTemp.Team))
							{
								num2 = num5;
								damageReceiver = this.dmgReceiverTemp;
							}
						}
						else if (context.searchPickupID > 0)
						{
							InteractablePickup component = collider.GetComponent<InteractablePickup>();
							if (component && component.ItemAgent && component.ItemAgent.Item && component.ItemAgent.Item.TypeID == context.searchPickupID)
							{
								num3 = num5;
								interactablePickup = component;
							}
						}
					}
				}
			}
		}
		context.onSearchFinishedCallback(damageReceiver, interactablePickup);
	}

	// Token: 0x06000862 RID: 2146 RVA: 0x00025664 File Offset: 0x00023864
	public void AddSearchTask(Vector3 center, Vector3 dir, float searchAngle, float searchDistance, Teams selfTeam, bool checkObsticle, bool thermalOn, bool ignoreFowBlockLayer, int searchPickupID, Action<DamageReceiver, InteractablePickup> callback)
	{
		AIMainBrain.SearchTaskContext searchTaskContext = new AIMainBrain.SearchTaskContext(center, dir, searchAngle, searchDistance, selfTeam, checkObsticle, thermalOn, ignoreFowBlockLayer, searchPickupID, callback);
		this.searchTasks.Enqueue(searchTaskContext);
	}

	// Token: 0x06000863 RID: 2147 RVA: 0x00025698 File Offset: 0x00023898
	private void DoCheckObsticle(AIMainBrain.CheckObsticleTaskContext context)
	{
		bool flag = this.CheckObsticle(context.start, context.end, context.thermalOn, context.ignoreFowBlockLayer);
		context.onCheckFinishCallback(flag);
	}

	// Token: 0x06000864 RID: 2148 RVA: 0x000256D0 File Offset: 0x000238D0
	public void AddCheckObsticleTask(Vector3 start, Vector3 end, bool thermalOn, bool ignoreFowBlockLayer, Action<bool> callback)
	{
		AIMainBrain.CheckObsticleTaskContext checkObsticleTaskContext = new AIMainBrain.CheckObsticleTaskContext(start, end, thermalOn, ignoreFowBlockLayer, callback);
		this.checkObsticleTasks.Enqueue(checkObsticleTaskContext);
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x000256F8 File Offset: 0x000238F8
	private bool CheckObsticle(Vector3 startPoint, Vector3 endPoint, bool thermalOn, bool ignoreFowBlockLayer)
	{
		Ray ray = new Ray(startPoint, (endPoint - startPoint).normalized);
		LayerMask layerMask = (thermalOn ? this.obsticleLayersWithThermal : this.obsticleLayers);
		if (ignoreFowBlockLayer)
		{
			layerMask &= ~(1 << this.fowBlockLayer);
		}
		return Physics.RaycastNonAlloc(ray, this.ObsHits, (endPoint - startPoint).magnitude, layerMask) > 0;
	}

	// Token: 0x0400078E RID: 1934
	private Queue<AIMainBrain.SearchTaskContext> searchTasks;

	// Token: 0x0400078F RID: 1935
	private Queue<AIMainBrain.CheckObsticleTaskContext> checkObsticleTasks;

	// Token: 0x04000790 RID: 1936
	private LayerMask dmgReceiverLayers;

	// Token: 0x04000791 RID: 1937
	private LayerMask interactLayers;

	// Token: 0x04000792 RID: 1938
	private LayerMask obsticleLayers;

	// Token: 0x04000793 RID: 1939
	private LayerMask obsticleLayersWithThermal;

	// Token: 0x04000794 RID: 1940
	private Collider[] cols;

	// Token: 0x04000795 RID: 1941
	private RaycastHit[] ObsHits;

	// Token: 0x04000796 RID: 1942
	public int maxSeachCount;

	// Token: 0x04000797 RID: 1943
	public int maxCheckObsticleCount;

	// Token: 0x04000798 RID: 1944
	private static CharacterMainControl _mc;

	// Token: 0x0400079B RID: 1947
	private int fowBlockLayer;

	// Token: 0x0400079C RID: 1948
	private DamageReceiver dmgReceiverTemp;

	// Token: 0x02000480 RID: 1152
	public struct SearchTaskContext
	{
		// Token: 0x06002664 RID: 9828 RVA: 0x00087FC8 File Offset: 0x000861C8
		public SearchTaskContext(Vector3 center, Vector3 dir, float searchAngle, float searchDistance, Teams selfTeam, bool checkObsticle, bool thermalOn, bool ignoreFowBlockLayer, int searchPickupID, Action<DamageReceiver, InteractablePickup> callback)
		{
			this.searchCenter = center;
			this.searchDirection = dir;
			this.searchAngle = searchAngle;
			this.searchDistance = searchDistance;
			this.selfTeam = selfTeam;
			this.thermalOn = thermalOn;
			this.checkObsticle = checkObsticle;
			this.searchPickupID = searchPickupID;
			this.onSearchFinishedCallback = callback;
			this.ignoreFowBlockLayer = ignoreFowBlockLayer;
		}

		// Token: 0x04001B76 RID: 7030
		public Vector3 searchCenter;

		// Token: 0x04001B77 RID: 7031
		public Vector3 searchDirection;

		// Token: 0x04001B78 RID: 7032
		public float searchAngle;

		// Token: 0x04001B79 RID: 7033
		public float searchDistance;

		// Token: 0x04001B7A RID: 7034
		public Teams selfTeam;

		// Token: 0x04001B7B RID: 7035
		public bool checkObsticle;

		// Token: 0x04001B7C RID: 7036
		public bool thermalOn;

		// Token: 0x04001B7D RID: 7037
		public bool ignoreFowBlockLayer;

		// Token: 0x04001B7E RID: 7038
		public int searchPickupID;

		// Token: 0x04001B7F RID: 7039
		public Action<DamageReceiver, InteractablePickup> onSearchFinishedCallback;
	}

	// Token: 0x02000481 RID: 1153
	public struct CheckObsticleTaskContext
	{
		// Token: 0x06002665 RID: 9829 RVA: 0x00088022 File Offset: 0x00086222
		public CheckObsticleTaskContext(Vector3 start, Vector3 end, bool thermalOn, bool ignoreFowBlockLayer, Action<bool> onCheckFinishCallback)
		{
			this.start = start;
			this.end = end;
			this.thermalOn = thermalOn;
			this.onCheckFinishCallback = onCheckFinishCallback;
			this.ignoreFowBlockLayer = ignoreFowBlockLayer;
		}

		// Token: 0x04001B80 RID: 7040
		public Vector3 start;

		// Token: 0x04001B81 RID: 7041
		public Vector3 end;

		// Token: 0x04001B82 RID: 7042
		public bool thermalOn;

		// Token: 0x04001B83 RID: 7043
		public bool ignoreFowBlockLayer;

		// Token: 0x04001B84 RID: 7044
		public Action<bool> onCheckFinishCallback;
	}
}
