using System;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x02000405 RID: 1029
	public class Attack : ActionTask<AICharacterController>
	{
		// Token: 0x0600252F RID: 9519 RVA: 0x00080302 File Offset: 0x0007E502
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x1700072A RID: 1834
		// (get) Token: 0x06002530 RID: 9520 RVA: 0x00080305 File Offset: 0x0007E505
		protected override string info
		{
			get
			{
				return string.Format("Attack", Array.Empty<object>());
			}
		}

		// Token: 0x06002531 RID: 9521 RVA: 0x00080316 File Offset: 0x0007E516
		protected override void OnExecute()
		{
			base.agent.CharacterMainControl.Attack();
			base.EndAction(true);
		}

		// Token: 0x06002532 RID: 9522 RVA: 0x00080330 File Offset: 0x0007E530
		protected override void OnStop()
		{
		}

		// Token: 0x06002533 RID: 9523 RVA: 0x00080332 File Offset: 0x0007E532
		protected override void OnPause()
		{
		}
	}
}
