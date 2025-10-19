using System;
using ItemStatsSystem;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x02000419 RID: 1049
	public class UseDrug : ActionTask<AICharacterController>
	{
		// Token: 0x1700073A RID: 1850
		// (get) Token: 0x060025AF RID: 9647 RVA: 0x00081C41 File Offset: 0x0007FE41
		protected override string info
		{
			get
			{
				if (!this.stopMove)
				{
					return "打药";
				}
				return "原地打药";
			}
		}

		// Token: 0x060025B0 RID: 9648 RVA: 0x00081C56 File Offset: 0x0007FE56
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x060025B1 RID: 9649 RVA: 0x00081C5C File Offset: 0x0007FE5C
		protected override void OnExecute()
		{
			Item drugItem = base.agent.GetDrugItem();
			if (drugItem == null)
			{
				base.EndAction(false);
				return;
			}
			base.agent.CharacterMainControl.UseItem(drugItem);
		}

		// Token: 0x060025B2 RID: 9650 RVA: 0x00081C98 File Offset: 0x0007FE98
		protected override void OnUpdate()
		{
			if (this.stopMove && base.agent.IsMoving())
			{
				base.agent.StopMove();
			}
			if (!base.agent || !base.agent.CharacterMainControl)
			{
				base.EndAction(false);
				return;
			}
			if (!base.agent.CharacterMainControl.useItemAction.Running)
			{
				base.EndAction(true);
			}
		}

		// Token: 0x060025B3 RID: 9651 RVA: 0x00081D0A File Offset: 0x0007FF0A
		protected override void OnStop()
		{
			base.agent.CharacterMainControl.SwitchToFirstAvailableWeapon();
		}

		// Token: 0x040019A6 RID: 6566
		public bool stopMove;
	}
}
