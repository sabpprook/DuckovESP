using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Duckov.Scenes
{
	// Token: 0x0200032B RID: 811
	public class MultiSceneTeleporter : InteractableBase
	{
		// Token: 0x17000512 RID: 1298
		// (get) Token: 0x06001B79 RID: 7033 RVA: 0x00063CB9 File Offset: 0x00061EB9
		protected override bool ShowUnityEvents
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000513 RID: 1299
		// (get) Token: 0x06001B7A RID: 7034 RVA: 0x00063CBC File Offset: 0x00061EBC
		[SerializeField]
		public MultiSceneLocation Target
		{
			get
			{
				return this.target;
			}
		}

		// Token: 0x17000514 RID: 1300
		// (get) Token: 0x06001B7B RID: 7035 RVA: 0x00063CC4 File Offset: 0x00061EC4
		private static float TimeSinceTeleportFinished
		{
			get
			{
				return Time.time - MultiSceneTeleporter.timeWhenTeleportFinished;
			}
		}

		// Token: 0x17000515 RID: 1301
		// (get) Token: 0x06001B7C RID: 7036 RVA: 0x00063CD1 File Offset: 0x00061ED1
		private static bool Teleportable
		{
			get
			{
				return MultiSceneTeleporter.TimeSinceTeleportFinished > 1f;
			}
		}

		// Token: 0x06001B7D RID: 7037 RVA: 0x00063CDF File Offset: 0x00061EDF
		protected override void Awake()
		{
			base.Awake();
		}

		// Token: 0x06001B7E RID: 7038 RVA: 0x00063CE8 File Offset: 0x00061EE8
		private void OnDrawGizmosSelected()
		{
			Transform locationTransform = this.target.LocationTransform;
			if (locationTransform)
			{
				Gizmos.DrawLine(base.transform.position, locationTransform.position);
				Gizmos.DrawWireSphere(locationTransform.position, 0.25f);
			}
		}

		// Token: 0x06001B7F RID: 7039 RVA: 0x00063D2F File Offset: 0x00061F2F
		public void DoTeleport()
		{
			if (!MultiSceneTeleporter.Teleportable)
			{
				Debug.Log("not Teleportable");
				return;
			}
			this.TeleportTask().Forget();
		}

		// Token: 0x06001B80 RID: 7040 RVA: 0x00063D4E File Offset: 0x00061F4E
		protected override bool IsInteractable()
		{
			return MultiSceneTeleporter.Teleportable;
		}

		// Token: 0x06001B81 RID: 7041 RVA: 0x00063D58 File Offset: 0x00061F58
		private async UniTask TeleportTask()
		{
			MultiSceneTeleporter.timeWhenTeleportFinished = Time.time;
			await MultiSceneCore.Instance.LoadAndTeleport(this.target);
			MultiSceneTeleporter.timeWhenTeleportFinished = Time.time;
		}

		// Token: 0x06001B82 RID: 7042 RVA: 0x00063D9B File Offset: 0x00061F9B
		protected override void OnInteractStart(CharacterMainControl interactCharacter)
		{
			this.coolTime = 2f;
			this.finishWhenTimeOut = true;
		}

		// Token: 0x06001B83 RID: 7043 RVA: 0x00063DAF File Offset: 0x00061FAF
		protected override void OnInteractFinished()
		{
			this.DoTeleport();
			base.StopInteract();
		}

		// Token: 0x0400137E RID: 4990
		[SerializeField]
		private MultiSceneLocation target;

		// Token: 0x0400137F RID: 4991
		[SerializeField]
		private bool teleportOnTriggerEnter;

		// Token: 0x04001380 RID: 4992
		private const float CoolDown = 1f;

		// Token: 0x04001381 RID: 4993
		private static float timeWhenTeleportFinished;
	}
}
