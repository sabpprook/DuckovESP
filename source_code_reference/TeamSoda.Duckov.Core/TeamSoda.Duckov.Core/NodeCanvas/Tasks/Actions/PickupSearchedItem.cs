using System;
using NodeCanvas.Framework;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x0200040A RID: 1034
	public class PickupSearchedItem : ActionTask<AICharacterController>
	{
		// Token: 0x06002550 RID: 9552 RVA: 0x00080A46 File Offset: 0x0007EC46
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x06002551 RID: 9553 RVA: 0x00080A4C File Offset: 0x0007EC4C
		protected override void OnExecute()
		{
			if (base.agent == null || base.agent.CharacterMainControl == null || base.agent.searchedPickup == null)
			{
				base.EndAction(false);
				return;
			}
			if (Vector3.Distance(base.agent.transform.position, base.agent.searchedPickup.transform.position) > 1.5f)
			{
				base.EndAction(false);
				return;
			}
			if (base.agent.searchedPickup.ItemAgent != null)
			{
				base.agent.CharacterMainControl.PickupItem(base.agent.searchedPickup.ItemAgent.Item);
			}
		}

		// Token: 0x06002552 RID: 9554 RVA: 0x00080B0C File Offset: 0x0007ED0C
		protected override void OnUpdate()
		{
		}
	}
}
