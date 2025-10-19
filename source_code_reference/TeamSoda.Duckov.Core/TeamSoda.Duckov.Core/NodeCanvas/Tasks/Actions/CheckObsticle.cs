using System;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x02000406 RID: 1030
	public class CheckObsticle : ActionTask<AICharacterController>
	{
		// Token: 0x06002535 RID: 9525 RVA: 0x0008033C File Offset: 0x0007E53C
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x06002536 RID: 9526 RVA: 0x00080340 File Offset: 0x0007E540
		protected override void OnExecute()
		{
			this.isHurtSearch = false;
			DamageInfo damageInfo = default(DamageInfo);
			if (base.agent.IsHurt(1.5f, 1, ref damageInfo) && damageInfo.fromCharacter && damageInfo.fromCharacter.mainDamageReceiver)
			{
				this.isHurtSearch = true;
			}
		}

		// Token: 0x06002537 RID: 9527 RVA: 0x00080398 File Offset: 0x0007E598
		private void Check()
		{
			this.waitingResult = true;
			Vector3 vector = (this.useTransform ? this.targetTransform.value.position : this.targetPoint.value);
			vector += Vector3.up * 0.4f;
			Vector3 vector2 = base.agent.transform.position + Vector3.up * 0.4f;
			ItemAgent_Gun gun = base.agent.CharacterMainControl.GetGun();
			if (gun && gun.muzzle)
			{
				vector2 = gun.muzzle.position - gun.muzzle.forward * 0.1f;
			}
			LevelManager.Instance.AIMainBrain.AddCheckObsticleTask(vector2, vector, base.agent.CharacterMainControl.ThermalOn, this.isHurtSearch, new Action<bool>(this.OnCheckFinished));
		}

		// Token: 0x06002538 RID: 9528 RVA: 0x0008048C File Offset: 0x0007E68C
		private void OnCheckFinished(bool result)
		{
			if (base.agent.gameObject == null)
			{
				return;
			}
			base.agent.hasObsticleToTarget = result;
			this.waitingResult = false;
			if (base.isRunning)
			{
				base.EndAction(this.alwaysSuccess || result);
			}
		}

		// Token: 0x06002539 RID: 9529 RVA: 0x000804DA File Offset: 0x0007E6DA
		protected override void OnUpdate()
		{
			if (!this.waitingResult)
			{
				this.Check();
			}
		}

		// Token: 0x0600253A RID: 9530 RVA: 0x000804EA File Offset: 0x0007E6EA
		protected override void OnStop()
		{
			this.waitingResult = false;
		}

		// Token: 0x0600253B RID: 9531 RVA: 0x000804F3 File Offset: 0x0007E6F3
		protected override void OnPause()
		{
		}

		// Token: 0x04001954 RID: 6484
		public bool useTransform;

		// Token: 0x04001955 RID: 6485
		[ShowIf("useTransform", 1)]
		public BBParameter<Transform> targetTransform;

		// Token: 0x04001956 RID: 6486
		[ShowIf("useTransform", 0)]
		public BBParameter<Vector3> targetPoint;

		// Token: 0x04001957 RID: 6487
		public bool alwaysSuccess;

		// Token: 0x04001958 RID: 6488
		private bool waitingResult;

		// Token: 0x04001959 RID: 6489
		private bool isHurtSearch;
	}
}
