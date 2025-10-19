using System;
using UnityEngine;

namespace Duckov.MiniGames.Examples.FPS
{
	// Token: 0x020002CE RID: 718
	public class FPS_Enemy_HomingFly : MiniGameBehaviour
	{
		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x0600169B RID: 5787 RVA: 0x00052CA6 File Offset: 0x00050EA6
		private bool CanSeeTarget
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x0600169C RID: 5788 RVA: 0x00052CA9 File Offset: 0x00050EA9
		private bool Dead
		{
			get
			{
				return this.health.Dead;
			}
		}

		// Token: 0x0600169D RID: 5789 RVA: 0x00052CB6 File Offset: 0x00050EB6
		private void Awake()
		{
			if (this.rigidbody == null)
			{
				this.rigidbody = base.GetComponent<Rigidbody>();
			}
			this.health.onDead += this.OnDead;
		}

		// Token: 0x0600169E RID: 5790 RVA: 0x00052CE9 File Offset: 0x00050EE9
		private void OnDead(FPSHealth health)
		{
			this.rigidbody.useGravity = true;
		}

		// Token: 0x0600169F RID: 5791 RVA: 0x00052CF7 File Offset: 0x00050EF7
		protected override void OnUpdate(float deltaTime)
		{
			if (this.Dead)
			{
				this.UpdateDead(deltaTime);
				return;
			}
			if (this.CanSeeTarget)
			{
				this.UpdateHoming(deltaTime);
				return;
			}
			this.UpdateIdle(deltaTime);
		}

		// Token: 0x060016A0 RID: 5792 RVA: 0x00052D20 File Offset: 0x00050F20
		private void UpdateIdle(float deltaTime)
		{
		}

		// Token: 0x060016A1 RID: 5793 RVA: 0x00052D22 File Offset: 0x00050F22
		private void UpdateDead(float deltaTime)
		{
		}

		// Token: 0x060016A2 RID: 5794 RVA: 0x00052D24 File Offset: 0x00050F24
		private void UpdateHoming(float deltaTime)
		{
		}

		// Token: 0x04001089 RID: 4233
		[SerializeField]
		private Rigidbody rigidbody;

		// Token: 0x0400108A RID: 4234
		[SerializeField]
		private FPSHealth health;
	}
}
