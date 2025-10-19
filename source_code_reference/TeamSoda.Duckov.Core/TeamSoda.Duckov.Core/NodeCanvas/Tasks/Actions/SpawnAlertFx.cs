using System;
using Duckov.Utilities;
using NodeCanvas.Framework;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x02000415 RID: 1045
	public class SpawnAlertFx : ActionTask<AICharacterController>
	{
		// Token: 0x06002596 RID: 9622 RVA: 0x00081759 File Offset: 0x0007F959
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x17000736 RID: 1846
		// (get) Token: 0x06002597 RID: 9623 RVA: 0x0008175C File Offset: 0x0007F95C
		protected override string info
		{
			get
			{
				return string.Format("AlertFx", Array.Empty<object>());
			}
		}

		// Token: 0x06002598 RID: 9624 RVA: 0x00081770 File Offset: 0x0007F970
		protected override void OnExecute()
		{
			if (!base.agent || !base.agent.CharacterMainControl)
			{
				base.EndAction(true);
			}
			Transform rightHandSocket = base.agent.CharacterMainControl.RightHandSocket;
			if (!rightHandSocket)
			{
				base.EndAction(true);
			}
			global::UnityEngine.Object.Instantiate<GameObject>(GameplayDataSettings.Prefabs.AlertFxPrefab, rightHandSocket).transform.localPosition = Vector3.zero;
			if (this.alertTime.value <= 0f)
			{
				base.EndAction(true);
			}
		}

		// Token: 0x06002599 RID: 9625 RVA: 0x000817FB File Offset: 0x0007F9FB
		protected override void OnUpdate()
		{
			if (base.elapsedTime >= this.alertTime.value)
			{
				base.EndAction(true);
			}
		}

		// Token: 0x0600259A RID: 9626 RVA: 0x00081817 File Offset: 0x0007FA17
		protected override void OnStop()
		{
		}

		// Token: 0x0600259B RID: 9627 RVA: 0x00081819 File Offset: 0x0007FA19
		protected override void OnPause()
		{
		}

		// Token: 0x04001996 RID: 6550
		public BBParameter<float> alertTime = 0.2f;
	}
}
