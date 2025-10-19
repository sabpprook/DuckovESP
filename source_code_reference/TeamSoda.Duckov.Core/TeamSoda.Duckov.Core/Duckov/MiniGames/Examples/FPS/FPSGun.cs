using System;
using DG.Tweening;
using UnityEngine;

namespace Duckov.MiniGames.Examples.FPS
{
	// Token: 0x020002D2 RID: 722
	public class FPSGun : MiniGameBehaviour
	{
		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x060016AA RID: 5802 RVA: 0x00052E14 File Offset: 0x00051014
		public float ScatterAngle
		{
			get
			{
				return Mathf.Lerp(this.minScatterAngle, this.maxScatterAngle, this.scatterStatus);
			}
		}

		// Token: 0x060016AB RID: 5803 RVA: 0x00052E30 File Offset: 0x00051030
		private void Fire()
		{
			this.coolDown = 1f / this.fireRate;
			this.DoCast();
			this.muzzleFlash.Play();
			this.DoFireAnimation();
			this.scatterStatus = Mathf.MoveTowards(this.scatterStatus, 1f, this.scatterIncrementPerShot);
		}

		// Token: 0x060016AC RID: 5804 RVA: 0x00052E84 File Offset: 0x00051084
		private void DoFireAnimation()
		{
			this.graphicsTransform.DOKill(true);
			this.graphicsTransform.localPosition = Vector3.zero;
			this.graphicsTransform.localRotation = Quaternion.identity;
			this.graphicsTransform.DOPunchPosition(Vector3.back * 0.2f, 0.2f, 10, 1f, false);
			this.graphicsTransform.DOShakeRotation(0.5f, -Vector3.right * 10f, 10, 90f, true);
		}

		// Token: 0x060016AD RID: 5805 RVA: 0x00052F14 File Offset: 0x00051114
		private void DoCast()
		{
			Ray ray = this.mainCamera.ViewportPointToRay(Vector3.one * 0.5f);
			Vector2 vector = global::UnityEngine.Random.insideUnitCircle * this.ScatterAngle / 2f;
			Vector3 vector2 = Quaternion.Euler(vector.y, vector.x, 0f) * Vector3.forward;
			Vector3 vector3 = this.mainCamera.transform.localToWorldMatrix.MultiplyVector(vector2);
			ray.direction = vector3;
			RaycastHit raycastHit;
			Physics.Raycast(ray, out raycastHit, 100f, this.castLayers);
			this.HandleBulletTracer(raycastHit);
			if (raycastHit.collider == null)
			{
				return;
			}
			FPSDamageInfo fpsdamageInfo = new FPSDamageInfo
			{
				source = this,
				amount = 1f,
				point = raycastHit.point,
				normal = raycastHit.normal
			};
			FPSDamageReceiver component = raycastHit.collider.GetComponent<FPSDamageReceiver>();
			if (component)
			{
				component.CastDamage(fpsdamageInfo);
				return;
			}
			this.HandleNormalHit(fpsdamageInfo);
		}

		// Token: 0x060016AE RID: 5806 RVA: 0x00053034 File Offset: 0x00051234
		private void HandleBulletTracer(RaycastHit castInfo)
		{
			if (this.bulletTracer == null)
			{
				return;
			}
			if (!true)
			{
				return;
			}
			Vector3 position = this.muzzle.transform.position;
			Vector3 vector = this.muzzle.transform.forward;
			if (castInfo.collider != null)
			{
				vector = castInfo.point - position;
				if ((castInfo.point - position).magnitude < 5f)
				{
					this.bulletTracer.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -vector);
					this.bulletTracer.transform.position = castInfo.point;
				}
				else
				{
					this.bulletTracer.transform.rotation = Quaternion.FromToRotation(Vector3.forward, vector);
					this.bulletTracer.transform.position = this.muzzle.position;
				}
			}
			else
			{
				this.bulletTracer.transform.rotation = Quaternion.FromToRotation(Vector3.forward, vector);
				this.bulletTracer.transform.position = this.muzzle.position;
			}
			this.bulletTracer.Emit(1);
		}

		// Token: 0x060016AF RID: 5807 RVA: 0x00053165 File Offset: 0x00051365
		private void HandleNormalHit(FPSDamageInfo info)
		{
			FXPool.Play(this.normalHitFXPrefab, info.point, Quaternion.FromToRotation(Vector3.forward, info.normal));
		}

		// Token: 0x060016B0 RID: 5808 RVA: 0x00053189 File Offset: 0x00051389
		internal void SetTrigger(bool value)
		{
			this.trigger = value;
			if (value)
			{
				this.justPressedTrigger = true;
			}
		}

		// Token: 0x060016B1 RID: 5809 RVA: 0x0005319C File Offset: 0x0005139C
		internal void Setup(Camera mainCamera, Transform gunParent)
		{
			base.transform.SetParent(gunParent, false);
			base.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			this.mainCamera = mainCamera;
		}

		// Token: 0x060016B2 RID: 5810 RVA: 0x000531C8 File Offset: 0x000513C8
		protected override void OnUpdate(float deltaTime)
		{
			if (this.coolDown > 0f)
			{
				this.coolDown -= deltaTime;
				this.coolDown = Mathf.Max(0f, this.coolDown);
			}
			if (this.coolDown <= 0f && this.trigger && (this.auto || this.justPressedTrigger))
			{
				this.Fire();
			}
			this.justPressedTrigger = false;
			this.scatterStatus = Mathf.MoveTowards(this.scatterStatus, 0f, this.scatterDecayRate * deltaTime);
			this.UpdateGunPhysicsStatus(deltaTime);
		}

		// Token: 0x060016B3 RID: 5811 RVA: 0x0005325D File Offset: 0x0005145D
		private void UpdateGunPhysicsStatus(float deltaTime)
		{
		}

		// Token: 0x04001094 RID: 4244
		[SerializeField]
		private float fireRate = 1f;

		// Token: 0x04001095 RID: 4245
		[SerializeField]
		private bool auto;

		// Token: 0x04001096 RID: 4246
		[SerializeField]
		private Transform muzzle;

		// Token: 0x04001097 RID: 4247
		[SerializeField]
		private ParticleSystem muzzleFlash;

		// Token: 0x04001098 RID: 4248
		[SerializeField]
		private ParticleSystem bulletTracer;

		// Token: 0x04001099 RID: 4249
		[SerializeField]
		private LayerMask castLayers = -1;

		// Token: 0x0400109A RID: 4250
		[SerializeField]
		private ParticleSystem normalHitFXPrefab;

		// Token: 0x0400109B RID: 4251
		[SerializeField]
		private float minScatterAngle;

		// Token: 0x0400109C RID: 4252
		[SerializeField]
		private float maxScatterAngle;

		// Token: 0x0400109D RID: 4253
		[SerializeField]
		private float scatterIncrementPerShot;

		// Token: 0x0400109E RID: 4254
		[SerializeField]
		private float scatterDecayRate;

		// Token: 0x0400109F RID: 4255
		[SerializeField]
		private Transform graphicsTransform;

		// Token: 0x040010A0 RID: 4256
		[SerializeField]
		private FPSGun.Pose idlePose;

		// Token: 0x040010A1 RID: 4257
		[SerializeField]
		private FPSGun.Pose recoilPose;

		// Token: 0x040010A2 RID: 4258
		private float scatterStatus;

		// Token: 0x040010A3 RID: 4259
		private float coolDown;

		// Token: 0x040010A4 RID: 4260
		private Camera mainCamera;

		// Token: 0x040010A5 RID: 4261
		private bool trigger;

		// Token: 0x040010A6 RID: 4262
		private bool justPressedTrigger;

		// Token: 0x0200056C RID: 1388
		[Serializable]
		public struct Pose
		{
			// Token: 0x06002822 RID: 10274 RVA: 0x00094184 File Offset: 0x00092384
			public static FPSGun.Pose Extraterpolate(FPSGun.Pose poseA, FPSGun.Pose poseB, float t)
			{
				return new FPSGun.Pose
				{
					localPosition = Vector3.LerpUnclamped(poseA.localPosition, poseB.localPosition, t),
					localRotation = Quaternion.LerpUnclamped(poseA.localRotation, poseB.localRotation, t)
				};
			}

			// Token: 0x06002823 RID: 10275 RVA: 0x000941CC File Offset: 0x000923CC
			public Pose(Transform fromTransform)
			{
				this.localPosition = fromTransform.localPosition;
				this.localRotation = fromTransform.localRotation;
			}

			// Token: 0x04001F5A RID: 8026
			[SerializeField]
			private Vector3 localPosition;

			// Token: 0x04001F5B RID: 8027
			[SerializeField]
			private Quaternion localRotation;
		}
	}
}
