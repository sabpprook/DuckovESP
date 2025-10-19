using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200006F RID: 111
public class Grenade : MonoBehaviour
{
	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x06000428 RID: 1064 RVA: 0x0001266D File Offset: 0x0001086D
	private bool needCustomFx
	{
		get
		{
			return this.fxType == ExplosionFxTypes.custom;
		}
	}

	// Token: 0x06000429 RID: 1065 RVA: 0x00012678 File Offset: 0x00010878
	private void OnCollisionEnter(Collision collision)
	{
		if (!this.collide)
		{
			this.collide = true;
		}
		Vector3 velocity = this.rb.velocity;
		velocity.x *= 0.5f;
		velocity.z *= 0.5f;
		this.rb.velocity = velocity;
		this.rb.angularVelocity = this.rb.angularVelocity * 0.3f;
		if (this.makeSoundCount > 0 && Time.time - this.makeSoundTimeMarker > 0.3f)
		{
			this.makeSoundCount--;
			this.makeSoundTimeMarker = Time.time;
			AISound aisound = default(AISound);
			aisound.fromObject = base.gameObject;
			aisound.pos = base.transform.position;
			if (this.damageInfo.fromCharacter)
			{
				aisound.fromTeam = this.damageInfo.fromCharacter.Team;
			}
			else
			{
				aisound.fromTeam = Teams.all;
			}
			aisound.soundType = SoundTypes.unknowNoise;
			if (this.isDangerForAi)
			{
				aisound.soundType = SoundTypes.grenadeDropSound;
			}
			aisound.radius = 20f;
			AIMainBrain.MakeSound(aisound);
			if (this.hasCollideSound && this.collideSound != "")
			{
				AudioManager.Post(this.collideSound, base.gameObject);
			}
		}
	}

	// Token: 0x0600042A RID: 1066 RVA: 0x000127D8 File Offset: 0x000109D8
	public void BindAgent(ItemAgent _agent)
	{
		this.bindAgent = true;
		this.bindedAgent = _agent;
		this.bindedAgent.transform.SetParent(base.transform, false);
		this.bindedAgent.transform.localPosition = Vector3.zero;
		this.bindedAgent.gameObject.SetActive(false);
	}

	// Token: 0x0600042B RID: 1067 RVA: 0x00012830 File Offset: 0x00010A30
	private void Update()
	{
		this.lifeTimer += Time.deltaTime;
		if (!this.delayFromCollide || this.collide)
		{
			this.delayTimer += Time.deltaTime;
		}
		if (!this.bindAgent)
		{
			if (!this.exploded && this.delayTimer > this.delayTime)
			{
				this.exploded = true;
				if (!this.isLandmine)
				{
					this.Explode();
					return;
				}
				this.ActiveLandmine().Forget();
			}
			return;
		}
		if (this.bindedAgent == null)
		{
			Debug.Log("bind  null destroied");
			global::UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if (this.lifeTimer > 0.5f && !this.bindedAgent.gameObject.activeInHierarchy)
		{
			this.bindedAgent.gameObject.SetActive(true);
		}
	}

	// Token: 0x0600042C RID: 1068 RVA: 0x00012908 File Offset: 0x00010B08
	private void Explode()
	{
		if (this.createExplosion)
		{
			this.damageInfo.isExplosion = true;
			LevelManager.Instance.ExplosionManager.CreateExplosion(base.transform.position, this.damageRange, this.damageInfo, this.fxType, this.explosionShakeStrength, this.canHurtSelf);
		}
		if (this.createExplosion && this.needCustomFx && this.fx != null)
		{
			global::UnityEngine.Object.Instantiate<GameObject>(this.fx, base.transform.position, Quaternion.identity);
		}
		if (this.createOnExlode)
		{
			global::UnityEngine.Object.Instantiate<GameObject>(this.createOnExlode, base.transform.position, Quaternion.identity);
		}
		UnityEvent unityEvent = this.onExplodeEvent;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		if (this.rb != null)
		{
			this.rb.constraints = (RigidbodyConstraints)10;
		}
		if (this.destroyDelay <= 0f)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if (this.destroyDelay < 999f)
		{
			this.DestroyOverTime().Forget();
		}
	}

	// Token: 0x0600042D RID: 1069 RVA: 0x00012A24 File Offset: 0x00010C24
	private async UniTask DestroyOverTime()
	{
		await UniTask.WaitForSeconds(this.destroyDelay, false, PlayerLoopTiming.Update, default(CancellationToken), false);
		if (!(base.gameObject == null))
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x00012A68 File Offset: 0x00010C68
	private async UniTask ActiveLandmine()
	{
		if (!this.landmineActived)
		{
			this.landmineActived = true;
			if (this.animator)
			{
				this.animator.SetBool("Actived", true);
			}
			OnTriggerEnterEvent trigger = new GameObject().AddComponent<OnTriggerEnterEvent>();
			SphereCollider sphereCollider = trigger.gameObject.AddComponent<SphereCollider>();
			sphereCollider.transform.SetParent(base.transform, false);
			sphereCollider.transform.localPosition = Vector3.zero;
			sphereCollider.isTrigger = true;
			sphereCollider.radius = this.landmineTriggerRange;
			trigger.filterByTeam = true;
			trigger.selfTeam = this.selfTeam;
			trigger.Init();
			await UniTask.WaitForEndOfFrame(this);
			trigger.DoOnTriggerEnter.AddListener(new UnityAction(this.OnLinemineTriggerd));
		}
	}

	// Token: 0x0600042F RID: 1071 RVA: 0x00012AAB File Offset: 0x00010CAB
	private void OnLinemineTriggerd()
	{
		if (this.landmineTriggerd)
		{
			return;
		}
		this.landmineTriggerd = true;
		this.Explode();
	}

	// Token: 0x06000430 RID: 1072 RVA: 0x00012AC3 File Offset: 0x00010CC3
	public void SetWeaponIdInfo(int typeId)
	{
		this.damageInfo.fromWeaponItemID = typeId;
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x00012AD4 File Offset: 0x00010CD4
	public void Launch(Vector3 startPoint, Vector3 velocity, CharacterMainControl fromCharacter, bool canHurtSelf)
	{
		this.canHurtSelf = canHurtSelf;
		this.groundLayer = LayerMask.NameToLayer("Ground");
		this.rb.position = startPoint;
		base.transform.position = startPoint;
		this.rb.velocity = velocity;
		Vector3 vector = (global::UnityEngine.Random.insideUnitSphere + Vector3.one) * 7f;
		vector.y = 0f;
		this.rb.angularVelocity = vector;
		if (fromCharacter != null)
		{
			Collider component = fromCharacter.GetComponent<Collider>();
			Collider component2 = base.GetComponent<Collider>();
			this.selfTeam = fromCharacter.Team;
			this.IgnoreCollisionForSeconds(component, component2, 0.5f).Forget();
		}
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x00012B84 File Offset: 0x00010D84
	private async UniTask IgnoreCollisionForSeconds(Collider col1, Collider col2, float ignoreTime)
	{
		if (col1 != null && col2 != null)
		{
			Physics.IgnoreCollision(col1, col2, true);
		}
		await UniTask.WaitForSeconds(ignoreTime, false, PlayerLoopTiming.Update, default(CancellationToken), false);
		if (col1 != null && col2 != null)
		{
			Physics.IgnoreCollision(col1, col2, false);
		}
	}

	// Token: 0x04000337 RID: 823
	public bool hasCollideSound;

	// Token: 0x04000338 RID: 824
	public string collideSound;

	// Token: 0x04000339 RID: 825
	public int makeSoundCount = 3;

	// Token: 0x0400033A RID: 826
	private float makeSoundTimeMarker = -1f;

	// Token: 0x0400033B RID: 827
	public float damageRange;

	// Token: 0x0400033C RID: 828
	public bool isDangerForAi = true;

	// Token: 0x0400033D RID: 829
	public bool isLandmine;

	// Token: 0x0400033E RID: 830
	public float landmineTriggerRange;

	// Token: 0x0400033F RID: 831
	private bool landmineActived;

	// Token: 0x04000340 RID: 832
	private bool landmineTriggerd;

	// Token: 0x04000341 RID: 833
	public ExplosionFxTypes fxType;

	// Token: 0x04000342 RID: 834
	public GameObject fx;

	// Token: 0x04000343 RID: 835
	public Animator animator;

	// Token: 0x04000344 RID: 836
	[SerializeField]
	private Rigidbody rb;

	// Token: 0x04000345 RID: 837
	private int groundLayer;

	// Token: 0x04000346 RID: 838
	public bool delayFromCollide;

	// Token: 0x04000347 RID: 839
	public float delayTime = 1f;

	// Token: 0x04000348 RID: 840
	public bool createExplosion = true;

	// Token: 0x04000349 RID: 841
	public float explosionShakeStrength = 1f;

	// Token: 0x0400034A RID: 842
	public DamageInfo damageInfo;

	// Token: 0x0400034B RID: 843
	private bool bindAgent;

	// Token: 0x0400034C RID: 844
	private ItemAgent bindedAgent;

	// Token: 0x0400034D RID: 845
	private float lifeTimer;

	// Token: 0x0400034E RID: 846
	private float delayTimer;

	// Token: 0x0400034F RID: 847
	private Teams selfTeam;

	// Token: 0x04000350 RID: 848
	public GameObject createOnExlode;

	// Token: 0x04000351 RID: 849
	public float destroyDelay;

	// Token: 0x04000352 RID: 850
	public UnityEvent onExplodeEvent;

	// Token: 0x04000353 RID: 851
	private bool exploded;

	// Token: 0x04000354 RID: 852
	private bool canHurtSelf;

	// Token: 0x04000355 RID: 853
	private bool collide;
}
