using System;
using System.Collections.Generic;
using Duckov.Utilities;
using UnityEngine;

// Token: 0x02000102 RID: 258
public class ExplosionManager : MonoBehaviour
{
	// Token: 0x0600087A RID: 2170 RVA: 0x00025C1D File Offset: 0x00023E1D
	private void Awake()
	{
		this.ObsHits = new RaycastHit[3];
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x00025C2C File Offset: 0x00023E2C
	public void CreateExplosion(Vector3 center, float radius, DamageInfo dmgInfo, ExplosionFxTypes fxType = ExplosionFxTypes.normal, float shakeStrength = 1f, bool canHurtSelf = true)
	{
		Vector3.Distance(center, CharacterMainControl.Main.transform.position);
		if (Vector3.Distance(center, CharacterMainControl.Main.transform.position) < 30f)
		{
			CameraShaker.Shake((center - LevelManager.Instance.MainCharacter.transform.position).normalized * 0.4f * shakeStrength, CameraShaker.CameraShakeTypes.explosion);
		}
		dmgInfo.isExplosion = true;
		if (this.damagedHealth == null)
		{
			this.damagedHealth = new List<Health>();
			this.colliders = new Collider[8];
			this.damageReceiverLayers = GameplayDataSettings.Layers.damageReceiverLayerMask;
		}
		this.damagedHealth.Clear();
		Teams teams = Teams.all;
		if (dmgInfo.fromCharacter && !canHurtSelf)
		{
			teams = dmgInfo.fromCharacter.Team;
		}
		int num = Physics.OverlapSphereNonAlloc(center, radius, this.colliders, this.damageReceiverLayers);
		for (int i = 0; i < num; i++)
		{
			DamageReceiver component = this.colliders[i].gameObject.GetComponent<DamageReceiver>();
			if (component != null && Team.IsEnemy(teams, component.Team) && (!(component.health != null) || !this.CheckObsticle(center + Vector3.up * 0.2f, this.colliders[i].gameObject.transform.position + Vector3.up * 0.6f)))
			{
				bool flag = false;
				bool flag2 = false;
				if (component.health != null)
				{
					if (this.damagedHealth.Contains(component.health))
					{
						flag = true;
					}
					else
					{
						this.damagedHealth.Add(component.health);
					}
					CharacterMainControl characterMainControl = component.health.TryGetCharacter();
					if (characterMainControl && characterMainControl.Dashing)
					{
						flag2 = true;
					}
				}
				if (!flag && !flag2)
				{
					dmgInfo.toDamageReceiver = component;
					dmgInfo.damagePoint = component.transform.position + Vector3.up * 0.6f;
					dmgInfo.damageNormal = (dmgInfo.damagePoint - center).normalized;
					component.Hurt(dmgInfo);
				}
			}
		}
		switch (fxType)
		{
		case ExplosionFxTypes.normal:
			global::UnityEngine.Object.Instantiate<GameObject>(this.normalFxPfb, center, Quaternion.identity);
			return;
		case ExplosionFxTypes.flash:
			global::UnityEngine.Object.Instantiate<GameObject>(this.flashFxPfb, center, Quaternion.identity);
			break;
		case ExplosionFxTypes.fire:
		case ExplosionFxTypes.ice:
			break;
		default:
			return;
		}
	}

	// Token: 0x0600087C RID: 2172 RVA: 0x00025EBC File Offset: 0x000240BC
	private bool CheckObsticle(Vector3 startPoint, Vector3 endPoint)
	{
		this.obsticleLayers = GameplayDataSettings.Layers.wallLayerMask | GameplayDataSettings.Layers.groundLayerMask;
		startPoint.y = 0.5f;
		endPoint.y = 0.5f;
		return Physics.RaycastNonAlloc(new Ray(startPoint, (endPoint - startPoint).normalized), this.ObsHits, (endPoint - startPoint).magnitude, this.obsticleLayers) > 0;
	}

	// Token: 0x040007B5 RID: 1973
	private LayerMask damageReceiverLayers;

	// Token: 0x040007B6 RID: 1974
	private LayerMask obsticleLayers;

	// Token: 0x040007B7 RID: 1975
	private List<Health> damagedHealth;

	// Token: 0x040007B8 RID: 1976
	private Collider[] colliders;

	// Token: 0x040007B9 RID: 1977
	public GameObject normalFxPfb;

	// Token: 0x040007BA RID: 1978
	public GameObject flashFxPfb;

	// Token: 0x040007BB RID: 1979
	private RaycastHit[] ObsHits;
}
