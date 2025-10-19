using System;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x02000408 RID: 1032
	public class FindMainCharacter : ActionTask<AICharacterController>
	{
		// Token: 0x06002543 RID: 9539 RVA: 0x000806B0 File Offset: 0x0007E8B0
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x06002544 RID: 9540 RVA: 0x000806B3 File Offset: 0x0007E8B3
		protected override void OnExecute()
		{
			if (LevelManager.Instance == null)
			{
				return;
			}
			this.mainCharacter.value = LevelManager.Instance.MainCharacter;
			if (this.mainCharacter.value != null)
			{
				base.EndAction(true);
			}
		}

		// Token: 0x06002545 RID: 9541 RVA: 0x000806F2 File Offset: 0x0007E8F2
		protected override void OnUpdate()
		{
			if (LevelManager.Instance == null)
			{
				return;
			}
			this.mainCharacter.value = LevelManager.Instance.MainCharacter;
			if (this.mainCharacter.value != null)
			{
				base.EndAction(true);
			}
		}

		// Token: 0x06002546 RID: 9542 RVA: 0x00080731 File Offset: 0x0007E931
		protected override void OnStop()
		{
		}

		// Token: 0x06002547 RID: 9543 RVA: 0x00080733 File Offset: 0x0007E933
		protected override void OnPause()
		{
		}

		// Token: 0x04001960 RID: 6496
		public BBParameter<CharacterMainControl> mainCharacter;
	}
}
