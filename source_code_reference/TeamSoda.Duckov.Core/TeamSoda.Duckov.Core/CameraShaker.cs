using System;
using Cinemachine;
using UnityEngine;

// Token: 0x02000176 RID: 374
public class CameraShaker : MonoBehaviour
{
	// Token: 0x06000B5D RID: 2909 RVA: 0x000302F6 File Offset: 0x0002E4F6
	private void Awake()
	{
		CameraShaker._instance = this;
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x00030300 File Offset: 0x0002E500
	public static void Shake(Vector3 velocity, CameraShaker.CameraShakeTypes shakeType)
	{
		if (CameraShaker._instance == null)
		{
			return;
		}
		switch (shakeType)
		{
		case CameraShaker.CameraShakeTypes.recoil:
			CameraShaker._instance.recoilSource.GenerateImpulseWithVelocity(velocity);
			return;
		case CameraShaker.CameraShakeTypes.explosion:
			CameraShaker._instance.explosionSource.GenerateImpulseWithVelocity(velocity);
			return;
		case CameraShaker.CameraShakeTypes.meleeAttackHit:
			CameraShaker._instance.meleeAttackSource.GenerateImpulseWithVelocity(velocity);
			return;
		default:
			return;
		}
	}

	// Token: 0x040009AF RID: 2479
	private static CameraShaker _instance;

	// Token: 0x040009B0 RID: 2480
	public CinemachineImpulseSource recoilSource;

	// Token: 0x040009B1 RID: 2481
	public CinemachineImpulseSource meleeAttackSource;

	// Token: 0x040009B2 RID: 2482
	public CinemachineImpulseSource explosionSource;

	// Token: 0x020004B3 RID: 1203
	public enum CameraShakeTypes
	{
		// Token: 0x04001C68 RID: 7272
		recoil,
		// Token: 0x04001C69 RID: 7273
		explosion,
		// Token: 0x04001C6A RID: 7274
		meleeAttackHit
	}
}
