using System;
using Duckov.Utilities;
using ItemStatsSystem;
using UnityEngine;

// Token: 0x02000130 RID: 304
public class Skill_Grenade : SkillBase
{
	// Token: 0x060009D9 RID: 2521 RVA: 0x0002A2C8 File Offset: 0x000284C8
	public override void OnRelease()
	{
		if (this.fromCharacter == null)
		{
			return;
		}
		Vector3 position = this.fromCharacter.CurrentUsingAimSocket.position;
		Vector3 releasePoint = this.skillReleaseContext.releasePoint;
		float y = releasePoint.y;
		Vector3 vector = releasePoint - this.fromCharacter.transform.position;
		vector.y = 0f;
		float num = vector.magnitude;
		if (!this.canControlCastDistance)
		{
			num = this.skillContext.castRange;
		}
		vector.Normalize();
		float num2 = 0f;
		if (this.blastCount <= 1)
		{
			this.blastCount = 1;
		}
		if (this.blastCount > 1)
		{
			if (this.blastAngle < 359f)
			{
				num2 = this.blastAngle / (float)(this.blastCount - 1);
			}
			else
			{
				num2 = this.blastAngle / (float)this.blastCount;
			}
		}
		Debug.Log(string.Format("castDistance:{0}", num));
		for (int i = 0; i < this.blastCount; i++)
		{
			Vector3 vector2 = Quaternion.Euler(0f, -this.blastAngle * 0.5f + num2 * (float)i, 0f) * vector;
			Vector3 vector3 = position + vector2 * num;
			vector3.y = y;
			Grenade grenade = global::UnityEngine.Object.Instantiate<Grenade>(this.grenadePfb, position, this.fromCharacter.CurrentUsingAimSocket.rotation);
			this.damageInfo.fromCharacter = this.fromCharacter;
			grenade.damageInfo = this.damageInfo;
			Vector3 vector4 = this.CalculateVelocity(position, vector3, this.skillContext.grenageVerticleSpeed);
			grenade.createExplosion = this.createExplosion;
			grenade.explosionShakeStrength = this.explosionShakeStrength;
			grenade.damageRange = this.skillContext.effectRange;
			grenade.delayFromCollide = this.delayFromCollide;
			grenade.delayTime = this.delay + this.blastDelayTimeSpace * (float)i;
			grenade.isLandmine = this.isLandmine;
			grenade.landmineTriggerRange = this.landmineTriggerRange;
			grenade.Launch(position, vector4, this.fromCharacter, this.canHurtSelf);
			if (this.fromItem != null)
			{
				grenade.SetWeaponIdInfo(this.fromItem.TypeID);
			}
			if (i == 0 && this.createPickup && this.fromItem != null)
			{
				Debug.Log("CreatePickup");
				this.fromItem.Detach();
				this.fromItem.AgentUtilities.ReleaseActiveAgent();
				ItemAgent itemAgent = this.fromItem.AgentUtilities.CreateAgent(GameplayDataSettings.Prefabs.PickupAgentNoRendererPrefab, ItemAgent.AgentTypes.pickUp);
				Debug.Log("newAgent Created:" + itemAgent.name);
				grenade.BindAgent(itemAgent);
			}
		}
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x0002A580 File Offset: 0x00028780
	public Vector3 CalculateVelocity(Vector3 start, Vector3 target, float verticleSpeed)
	{
		float num = Physics.gravity.magnitude;
		if (num <= 0f)
		{
			num = 1f;
		}
		float num2 = verticleSpeed / num;
		float num3 = Mathf.Sqrt(2f * Mathf.Abs(num2 * verticleSpeed * 0.5f + start.y - target.y) / num);
		float num4 = num2 + num3;
		if (num4 <= 0f)
		{
			num4 = 0.001f;
		}
		Vector3 vector = start;
		vector.y = 0f;
		Vector3 vector2 = target;
		vector2.y = 0f;
		float num5 = Vector3.Distance(vector, vector2);
		float num6 = 0f;
		Vector3 vector3 = vector2 - vector;
		if (vector3.magnitude > 0f)
		{
			vector3 = vector3.normalized;
			num6 = num5 / num4;
		}
		else
		{
			vector3 = Vector3.zero;
		}
		return vector3 * num6 + Vector3.up * verticleSpeed;
	}

	// Token: 0x040008A3 RID: 2211
	public bool canControlCastDistance = true;

	// Token: 0x040008A4 RID: 2212
	public float delay = 1f;

	// Token: 0x040008A5 RID: 2213
	public bool delayFromCollide;

	// Token: 0x040008A6 RID: 2214
	public Grenade grenadePfb;

	// Token: 0x040008A7 RID: 2215
	public bool createPickup;

	// Token: 0x040008A8 RID: 2216
	public bool isLandmine;

	// Token: 0x040008A9 RID: 2217
	public float landmineTriggerRange = 0.5f;

	// Token: 0x040008AA RID: 2218
	public bool createExplosion = true;

	// Token: 0x040008AB RID: 2219
	public bool canHurtSelf = true;

	// Token: 0x040008AC RID: 2220
	[Range(0f, 1f)]
	public float explosionShakeStrength = 1f;

	// Token: 0x040008AD RID: 2221
	public DamageInfo damageInfo;

	// Token: 0x040008AE RID: 2222
	public int blastCount = 1;

	// Token: 0x040008AF RID: 2223
	public float blastAngle;

	// Token: 0x040008B0 RID: 2224
	[Tooltip("当有多个手雷时，delay的间隔")]
	public float blastDelayTimeSpace = 0.2f;
}
