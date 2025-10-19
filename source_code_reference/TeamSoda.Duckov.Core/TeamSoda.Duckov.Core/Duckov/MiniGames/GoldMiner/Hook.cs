using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x02000290 RID: 656
	public class Hook : MiniGameBehaviour
	{
		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x06001569 RID: 5481 RVA: 0x0004F4EF File Offset: 0x0004D6EF
		public Transform Axis
		{
			get
			{
				return this.hookAxis;
			}
		}

		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x0600156A RID: 5482 RVA: 0x0004F4F7 File Offset: 0x0004D6F7
		public Hook.HookStatus Status
		{
			get
			{
				return this.status;
			}
		}

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x0600156B RID: 5483 RVA: 0x0004F4FF File Offset: 0x0004D6FF
		private float RopeDistance
		{
			get
			{
				return Mathf.Lerp(this.minDist, this.maxDist, this.ropeControl);
			}
		}

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x0600156C RID: 5484 RVA: 0x0004F518 File Offset: 0x0004D718
		private float AxisAngle
		{
			get
			{
				return Mathf.Lerp(-this.maxAngle, this.maxAngle, (this.axisControl + 1f) / 2f);
			}
		}

		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x0600156D RID: 5485 RVA: 0x0004F540 File Offset: 0x0004D740
		private bool RopeOutOfBound
		{
			get
			{
				Vector3 vector = Quaternion.Euler(0f, 0f, this.AxisAngle) * Vector2.down * this.RopeDistance;
				return !this.bounds.Contains(vector);
			}
		}

		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x0600156E RID: 5486 RVA: 0x0004F58C File Offset: 0x0004D78C
		// (set) Token: 0x0600156F RID: 5487 RVA: 0x0004F594 File Offset: 0x0004D794
		public GoldMinerEntity GrabbingTarget
		{
			get
			{
				return this._grabbingTarget;
			}
			private set
			{
				this._grabbingTarget = value;
			}
		}

		// Token: 0x1400008E RID: 142
		// (add) Token: 0x06001570 RID: 5488 RVA: 0x0004F5A0 File Offset: 0x0004D7A0
		// (remove) Token: 0x06001571 RID: 5489 RVA: 0x0004F5D8 File Offset: 0x0004D7D8
		public event Action<Hook, GoldMinerEntity> OnResolveTarget;

		// Token: 0x1400008F RID: 143
		// (add) Token: 0x06001572 RID: 5490 RVA: 0x0004F610 File Offset: 0x0004D810
		// (remove) Token: 0x06001573 RID: 5491 RVA: 0x0004F648 File Offset: 0x0004D848
		public event Action<Hook> OnLaunch;

		// Token: 0x14000090 RID: 144
		// (add) Token: 0x06001574 RID: 5492 RVA: 0x0004F680 File Offset: 0x0004D880
		// (remove) Token: 0x06001575 RID: 5493 RVA: 0x0004F6B8 File Offset: 0x0004D8B8
		public event Action<Hook> OnBeginRetrieve;

		// Token: 0x14000091 RID: 145
		// (add) Token: 0x06001576 RID: 5494 RVA: 0x0004F6F0 File Offset: 0x0004D8F0
		// (remove) Token: 0x06001577 RID: 5495 RVA: 0x0004F728 File Offset: 0x0004D928
		public event Action<Hook, GoldMinerEntity> OnAttach;

		// Token: 0x14000092 RID: 146
		// (add) Token: 0x06001578 RID: 5496 RVA: 0x0004F760 File Offset: 0x0004D960
		// (remove) Token: 0x06001579 RID: 5497 RVA: 0x0004F798 File Offset: 0x0004D998
		public event Action<Hook> OnEndRetrieve;

		// Token: 0x0600157A RID: 5498 RVA: 0x0004F7CD File Offset: 0x0004D9CD
		public void SetParameters(float swingFreqFactor, float emptySpeed, float strength)
		{
			this.swingFreqFactor = swingFreqFactor;
			this.emptySpeed = emptySpeed;
			this.strength = strength;
		}

		// Token: 0x0600157B RID: 5499 RVA: 0x0004F7E4 File Offset: 0x0004D9E4
		public void Tick(float deltaTime)
		{
			this.UpdateStatus(deltaTime);
			this.UpdateHookHeadPosition();
			this.UpdateAxis();
			this.ropeLineRenderer.SetPositions(new Vector3[]
			{
				this.hookAxis.transform.position,
				this.hookHead.transform.position
			});
		}

		// Token: 0x0600157C RID: 5500 RVA: 0x0004F843 File Offset: 0x0004DA43
		private void UpdateHookHeadPosition()
		{
			this.hookHead.transform.localPosition = this.GetHookHeadPosition(this.RopeDistance);
		}

		// Token: 0x0600157D RID: 5501 RVA: 0x0004F861 File Offset: 0x0004DA61
		private Vector3 GetHookHeadPosition(float ropeDistance)
		{
			return -Vector3.up * this.RopeDistance;
		}

		// Token: 0x0600157E RID: 5502 RVA: 0x0004F878 File Offset: 0x0004DA78
		private void UpdateAxis()
		{
			this.hookAxis.transform.localRotation = Quaternion.Euler(0f, 0f, this.AxisAngle);
		}

		// Token: 0x0600157F RID: 5503 RVA: 0x0004F89F File Offset: 0x0004DA9F
		private void OnValidate()
		{
			this.UpdateHookHeadPosition();
			this.UpdateAxis();
		}

		// Token: 0x06001580 RID: 5504 RVA: 0x0004F8B0 File Offset: 0x0004DAB0
		private void UpdateStatus(float deltaTime)
		{
			switch (this.status)
			{
			case Hook.HookStatus.Idle:
				break;
			case Hook.HookStatus.Swinging:
				this.UpdateSwinging(deltaTime);
				this.UpdateClaw();
				return;
			case Hook.HookStatus.Launching:
				this.UpdateClaw();
				this.UpdateLaunching(deltaTime);
				return;
			case Hook.HookStatus.Attaching:
				this.UpdateAttaching(deltaTime);
				return;
			case Hook.HookStatus.Retrieving:
				this.UpdateRetreving(deltaTime);
				this.UpdateClaw();
				return;
			case Hook.HookStatus.Retrieved:
				this.UpdateRetrieved();
				break;
			default:
				return;
			}
		}

		// Token: 0x06001581 RID: 5505 RVA: 0x0004F91B File Offset: 0x0004DB1B
		public void Launch()
		{
			if (this.status != Hook.HookStatus.Swinging)
			{
				return;
			}
			this.EnterStatus(Hook.HookStatus.Launching);
			Action<Hook> onLaunch = this.OnLaunch;
			if (onLaunch == null)
			{
				return;
			}
			onLaunch(this);
		}

		// Token: 0x06001582 RID: 5506 RVA: 0x0004F93F File Offset: 0x0004DB3F
		public void Reset()
		{
			this.ropeControl = 0f;
		}

		// Token: 0x06001583 RID: 5507 RVA: 0x0004F94C File Offset: 0x0004DB4C
		private void UpdateClaw()
		{
			this.clawAnimator.SetBool("Grabbing", this.GrabbingTarget);
			if (!this.GrabbingTarget)
			{
				this.claw.localRotation = Quaternion.Euler(0f, 0f, -180f);
				this.claw.localPosition = Vector3.zero;
				return;
			}
			Vector2 vector = this.GrabbingTarget.transform.position - this.hookHead.transform.position;
			this.claw.rotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, vector));
			this.claw.position = this.hookHead.transform.position + vector.normalized * this.clawOffset;
		}

		// Token: 0x06001584 RID: 5508 RVA: 0x0004FA38 File Offset: 0x0004DC38
		private void UpdateSwinging(float deltaTime)
		{
			this.t += deltaTime * 90f * this.swingFreqFactor * 0.017453292f;
			this.axisControl = Mathf.Sin(this.t);
		}

		// Token: 0x06001585 RID: 5509 RVA: 0x0004FA6C File Offset: 0x0004DC6C
		private void UpdateLaunching(float deltaTime)
		{
			float num = this.emptySpeed;
			if (this.GrabbingTarget != null)
			{
				num = this.GrabbingTarget.Speed;
			}
			float num2 = (100f + this.strength) / 100f;
			num *= num2;
			float num3 = num * deltaTime / (this.maxDist - this.minDist);
			Vector3 hookHeadPosition = this.GetHookHeadPosition(this.RopeDistance);
			this.ropeControl = Mathf.MoveTowards(this.ropeControl, 1f, num3);
			this.GetHookHeadPosition(this.RopeDistance);
			Vector3 vector = this.hookAxis.localToWorldMatrix.MultiplyPoint(hookHeadPosition);
			Vector3 vector2 = this.hookAxis.localToWorldMatrix.MultiplyPoint(hookHeadPosition);
			if (this.RopeOutOfBound || this.ropeControl >= 1f)
			{
				this.EnterStatus(Hook.HookStatus.Retrieving);
			}
			this.CheckGrab(vector, vector2);
		}

		// Token: 0x06001586 RID: 5510 RVA: 0x0004FB48 File Offset: 0x0004DD48
		private void CheckGrab(Vector3 oldWorldPos, Vector3 newWorldPos)
		{
			if (this.GrabbingTarget)
			{
				return;
			}
			Vector3 vector = newWorldPos - oldWorldPos;
			foreach (RaycastHit2D raycastHit2D in Physics2D.CircleCastAll(oldWorldPos, 8f, vector.normalized, vector.magnitude))
			{
				if (!(raycastHit2D.collider == null))
				{
					GoldMinerEntity component = raycastHit2D.collider.gameObject.GetComponent<GoldMinerEntity>();
					if (!(component == null))
					{
						this.Grab(component);
						return;
					}
				}
			}
		}

		// Token: 0x06001587 RID: 5511 RVA: 0x0004FBDC File Offset: 0x0004DDDC
		private void Grab(GoldMinerEntity target)
		{
			this.GrabbingTarget = target;
			this.EnterStatus(Hook.HookStatus.Attaching);
			this.relativePos = target.transform.position - this.hookHead.transform.position;
			this.targetDist = this.relativePos.magnitude;
			this.targetRelativeRotation = Quaternion.FromToRotation(this.relativePos, this.GrabbingTarget.transform.up);
			this.retrieveETA = this.grabAnimationTime;
			Vector2 vector = this.GrabbingTarget.transform.position - this.hookHead.transform.position;
			Vector3 vector2 = new Vector3(0f, 0f, Vector2.SignedAngle(Vector2.up, vector));
			Vector3 vector3 = this.hookHead.transform.position + vector.normalized * this.clawOffset;
			this.claw.DORotate(vector2, this.retrieveETA, RotateMode.Fast).SetEase(this.grabAnimationEase);
			this.claw.DOMove(vector3, this.retrieveETA, false).SetEase(this.grabAnimationEase);
			this.clawAnimator.SetBool("Grabbing", this.GrabbingTarget);
			this.GrabbingTarget.NotifyAttached(this);
			Action<Hook, GoldMinerEntity> onAttach = this.OnAttach;
			if (onAttach == null)
			{
				return;
			}
			onAttach(this, target);
		}

		// Token: 0x06001588 RID: 5512 RVA: 0x0004FD50 File Offset: 0x0004DF50
		private void UpdateAttaching(float deltaTime)
		{
			if (this.GrabbingTarget == null)
			{
				this.EnterStatus(Hook.HookStatus.Retrieving);
				return;
			}
			this.retrieveETA -= deltaTime;
			if (this.retrieveETA <= 0f)
			{
				this.EnterStatus(Hook.HookStatus.Retrieving);
			}
		}

		// Token: 0x06001589 RID: 5513 RVA: 0x0004FD8C File Offset: 0x0004DF8C
		private void UpdateRetreving(float deltaTime)
		{
			float num = this.emptySpeed;
			if (this.GrabbingTarget != null)
			{
				num = this.GrabbingTarget.Speed;
			}
			float num2 = (100f + this.strength) / 100f;
			num *= num2;
			float num3 = num * deltaTime / (this.maxDist - this.minDist);
			this.maxDeltaWatch = num3;
			Vector3 hookHeadPosition = this.GetHookHeadPosition(this.RopeDistance);
			this.ropeControl = Mathf.MoveTowards(this.ropeControl, 0f, num3);
			this.GetHookHeadPosition(this.RopeDistance);
			Vector3 vector = this.hookAxis.localToWorldMatrix.MultiplyPoint(hookHeadPosition);
			Vector3 vector2 = this.hookAxis.localToWorldMatrix.MultiplyPoint(hookHeadPosition);
			if (this.ropeControl <= 0f)
			{
				this.ropeControl = 0f;
				this.EnterStatus(Hook.HookStatus.Retrieved);
			}
			if (this.GrabbingTarget)
			{
				Vector3 vector3 = this.GrabbingTarget.transform.position - this.hookHead.transform.position;
				if (vector3.magnitude > this.targetDist)
				{
					this.GrabbingTarget.transform.position = this.hookHead.transform.position + vector3.normalized * this.targetDist;
					Vector3 vector4 = this.targetRelativeRotation * vector3;
					this.GrabbingTarget.transform.rotation = Quaternion.FromToRotation(Vector3.up, vector4);
					return;
				}
			}
			else
			{
				this.CheckGrab(vector, vector2);
			}
		}

		// Token: 0x0600158A RID: 5514 RVA: 0x0004FF1B File Offset: 0x0004E11B
		private void UpdateRetrieved()
		{
			if (this.GrabbingTarget)
			{
				this.ResolveRetrievedObject(this.GrabbingTarget);
				this.GrabbingTarget = null;
			}
			this.EnterStatus(Hook.HookStatus.Swinging);
		}

		// Token: 0x0600158B RID: 5515 RVA: 0x0004FF44 File Offset: 0x0004E144
		private void ResolveRetrievedObject(GoldMinerEntity grabingTarget)
		{
			Action<Hook, GoldMinerEntity> onResolveTarget = this.OnResolveTarget;
			if (onResolveTarget == null)
			{
				return;
			}
			onResolveTarget(this, grabingTarget);
		}

		// Token: 0x0600158C RID: 5516 RVA: 0x0004FF58 File Offset: 0x0004E158
		private void OnExitStatus(Hook.HookStatus status)
		{
			switch (status)
			{
			case Hook.HookStatus.Idle:
			case Hook.HookStatus.Swinging:
			case Hook.HookStatus.Launching:
			case Hook.HookStatus.Attaching:
			case Hook.HookStatus.Retrieved:
				break;
			case Hook.HookStatus.Retrieving:
			{
				Action<Hook> onEndRetrieve = this.OnEndRetrieve;
				if (onEndRetrieve == null)
				{
					return;
				}
				onEndRetrieve(this);
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x0600158D RID: 5517 RVA: 0x0004FF8A File Offset: 0x0004E18A
		private void EnterStatus(Hook.HookStatus status)
		{
			this.OnExitStatus(this.status);
			this.status = status;
			this.OnEnterStatus(this.status);
		}

		// Token: 0x0600158E RID: 5518 RVA: 0x0004FFAC File Offset: 0x0004E1AC
		private void OnEnterStatus(Hook.HookStatus status)
		{
			switch (status)
			{
			case Hook.HookStatus.Idle:
			case Hook.HookStatus.Launching:
			case Hook.HookStatus.Attaching:
			case Hook.HookStatus.Retrieved:
				break;
			case Hook.HookStatus.Swinging:
				this.ropeControl = 0f;
				return;
			case Hook.HookStatus.Retrieving:
			{
				if (this.GrabbingTarget)
				{
					this.GrabbingTarget.NotifyBeginRetrieving();
				}
				Action<Hook> onBeginRetrieve = this.OnBeginRetrieve;
				if (onBeginRetrieve == null)
				{
					return;
				}
				onBeginRetrieve(this);
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x0600158F RID: 5519 RVA: 0x0005000D File Offset: 0x0004E20D
		internal Vector3 Direction
		{
			get
			{
				return -this.hookAxis.transform.up;
			}
		}

		// Token: 0x06001590 RID: 5520 RVA: 0x00050024 File Offset: 0x0004E224
		internal void ReleaseClaw()
		{
			this.GrabbingTarget = null;
		}

		// Token: 0x06001591 RID: 5521 RVA: 0x0005002D File Offset: 0x0004E22D
		internal void BeginSwing()
		{
			this.EnterStatus(Hook.HookStatus.Swinging);
		}

		// Token: 0x04000FD9 RID: 4057
		public float emptySpeed = 1000f;

		// Token: 0x04000FDA RID: 4058
		public float strength;

		// Token: 0x04000FDB RID: 4059
		public float swingFreqFactor = 1f;

		// Token: 0x04000FDC RID: 4060
		[SerializeField]
		private Transform hookAxis;

		// Token: 0x04000FDD RID: 4061
		[SerializeField]
		private HookHead hookHead;

		// Token: 0x04000FDE RID: 4062
		[SerializeField]
		private Transform claw;

		// Token: 0x04000FDF RID: 4063
		[SerializeField]
		private float clawOffset = 4f;

		// Token: 0x04000FE0 RID: 4064
		[SerializeField]
		private Animator clawAnimator;

		// Token: 0x04000FE1 RID: 4065
		[SerializeField]
		private LineRenderer ropeLineRenderer;

		// Token: 0x04000FE2 RID: 4066
		[SerializeField]
		private Bounds bounds;

		// Token: 0x04000FE3 RID: 4067
		[SerializeField]
		private float grabAnimationTime = 0.5f;

		// Token: 0x04000FE4 RID: 4068
		[SerializeField]
		private Ease grabAnimationEase = Ease.OutBounce;

		// Token: 0x04000FE5 RID: 4069
		[SerializeField]
		private float maxAngle;

		// Token: 0x04000FE6 RID: 4070
		[SerializeField]
		private float minDist;

		// Token: 0x04000FE7 RID: 4071
		[SerializeField]
		private float maxDist;

		// Token: 0x04000FE8 RID: 4072
		[Range(0f, 1f)]
		private float ropeControl;

		// Token: 0x04000FE9 RID: 4073
		[Range(-1f, 1f)]
		private float axisControl;

		// Token: 0x04000FEA RID: 4074
		private Hook.HookStatus status;

		// Token: 0x04000FEB RID: 4075
		private float t;

		// Token: 0x04000FEC RID: 4076
		private GoldMinerEntity _grabbingTarget;

		// Token: 0x04000FED RID: 4077
		private Vector2 relativePos;

		// Token: 0x04000FEE RID: 4078
		private Quaternion targetRelativeRotation;

		// Token: 0x04000FEF RID: 4079
		private float targetDist;

		// Token: 0x04000FF0 RID: 4080
		private float retrieveETA;

		// Token: 0x04000FF6 RID: 4086
		public float forceModification;

		// Token: 0x04000FF7 RID: 4087
		private float maxDeltaWatch;

		// Token: 0x02000569 RID: 1385
		public enum HookStatus
		{
			// Token: 0x04001F4E RID: 8014
			Idle,
			// Token: 0x04001F4F RID: 8015
			Swinging,
			// Token: 0x04001F50 RID: 8016
			Launching,
			// Token: 0x04001F51 RID: 8017
			Attaching,
			// Token: 0x04001F52 RID: 8018
			Retrieving,
			// Token: 0x04001F53 RID: 8019
			Retrieved
		}
	}
}
