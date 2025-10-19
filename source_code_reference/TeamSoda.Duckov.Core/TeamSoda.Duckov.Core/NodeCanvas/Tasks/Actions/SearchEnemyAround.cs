using System;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x02000410 RID: 1040
	public class SearchEnemyAround : ActionTask<AICharacterController>
	{
		// Token: 0x06002574 RID: 9588 RVA: 0x0008119B File Offset: 0x0007F39B
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x06002575 RID: 9589 RVA: 0x000811A0 File Offset: 0x0007F3A0
		protected override void OnExecute()
		{
			DamageInfo damageInfo = default(DamageInfo);
			this.isHurtSearch = false;
			if (base.agent.IsHurt(1.5f, 1, ref damageInfo) && damageInfo.fromCharacter && damageInfo.fromCharacter.mainDamageReceiver)
			{
				this.isHurtSearch = true;
			}
		}

		// Token: 0x06002576 RID: 9590 RVA: 0x000811F8 File Offset: 0x0007F3F8
		private void Search()
		{
			this.waitingSearchResult = true;
			float num = (this.useSight ? base.agent.sightAngle : this.searchAngle.value);
			float num2 = (this.useSight ? (base.agent.sightDistance * this.sightDistanceMultiplier.value) : this.searchDistance.value);
			if (this.isHurtSearch)
			{
				num2 *= 2f;
			}
			if (this.affactByNightVisionAbility && base.agent.CharacterMainControl)
			{
				float nightVisionAbility = base.agent.CharacterMainControl.NightVisionAbility;
				num *= Mathf.Lerp(TimeOfDayController.NightViewAngleFactor, 1f, nightVisionAbility);
			}
			bool flag = this.useSight || this.checkObsticle;
			this.searchStartTimeMarker = Time.time;
			bool thermalOn = base.agent.CharacterMainControl.ThermalOn;
			LevelManager.Instance.AIMainBrain.AddSearchTask(base.agent.transform.position + Vector3.up * 1.5f, base.agent.CharacterMainControl.CurrentAimDirection, num, num2, base.agent.CharacterMainControl.Team, flag, thermalOn, this.isHurtSearch, this.searchPickup ? base.agent.wantItem : (-1), new Action<DamageReceiver, InteractablePickup>(this.OnSearchFinished));
		}

		// Token: 0x06002577 RID: 9591 RVA: 0x00081358 File Offset: 0x0007F558
		private void OnSearchFinished(DamageReceiver dmgReceiver, InteractablePickup pickup)
		{
			if (base.agent.gameObject == null)
			{
				return;
			}
			float time = Time.time;
			float num = this.searchStartTimeMarker;
			if (dmgReceiver != null)
			{
				this.result.value = dmgReceiver;
			}
			else if (this.setNullIfNotFound)
			{
				this.result.value = null;
			}
			if (pickup != null)
			{
				this.pickupResult.value = pickup;
			}
			this.waitingSearchResult = false;
			if (base.isRunning)
			{
				base.EndAction(this.alwaysSuccess || this.result.value != null || this.pickupResult != null);
			}
		}

		// Token: 0x06002578 RID: 9592 RVA: 0x00081405 File Offset: 0x0007F605
		protected override void OnUpdate()
		{
			if (!this.waitingSearchResult)
			{
				this.Search();
			}
		}

		// Token: 0x06002579 RID: 9593 RVA: 0x00081415 File Offset: 0x0007F615
		protected override void OnStop()
		{
			this.waitingSearchResult = false;
		}

		// Token: 0x0600257A RID: 9594 RVA: 0x0008141E File Offset: 0x0007F61E
		protected override void OnPause()
		{
		}

		// Token: 0x0400197E RID: 6526
		public bool useSight;

		// Token: 0x0400197F RID: 6527
		public bool affactByNightVisionAbility;

		// Token: 0x04001980 RID: 6528
		[ShowIf("useSight", 0)]
		public BBParameter<float> searchAngle = 180f;

		// Token: 0x04001981 RID: 6529
		[ShowIf("useSight", 0)]
		public BBParameter<float> searchDistance;

		// Token: 0x04001982 RID: 6530
		[ShowIf("useSight", 1)]
		public BBParameter<float> sightDistanceMultiplier = 1f;

		// Token: 0x04001983 RID: 6531
		[ShowIf("useSight", 0)]
		public bool checkObsticle = true;

		// Token: 0x04001984 RID: 6532
		public BBParameter<DamageReceiver> result;

		// Token: 0x04001985 RID: 6533
		public BBParameter<InteractablePickup> pickupResult;

		// Token: 0x04001986 RID: 6534
		public bool searchPickup;

		// Token: 0x04001987 RID: 6535
		public bool alwaysSuccess;

		// Token: 0x04001988 RID: 6536
		public bool setNullIfNotFound;

		// Token: 0x04001989 RID: 6537
		private bool waitingSearchResult;

		// Token: 0x0400198A RID: 6538
		private float searchStartTimeMarker;

		// Token: 0x0400198B RID: 6539
		private bool isHurtSearch;
	}
}
