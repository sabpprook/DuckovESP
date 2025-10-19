using System;
using Duckov;
using NodeCanvas.Framework;
using SodaCraft.Localizations;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x02000418 RID: 1048
	public class TryToReloadIfEmpty : ActionTask<AICharacterController>
	{
		// Token: 0x17000737 RID: 1847
		// (get) Token: 0x060025A6 RID: 9638 RVA: 0x00081ADE File Offset: 0x0007FCDE
		public string SoundKey
		{
			get
			{
				return "normal";
			}
		}

		// Token: 0x17000738 RID: 1848
		// (get) Token: 0x060025A7 RID: 9639 RVA: 0x00081AE5 File Offset: 0x0007FCE5
		private string Key
		{
			get
			{
				return this.poptextWhileReloading;
			}
		}

		// Token: 0x17000739 RID: 1849
		// (get) Token: 0x060025A8 RID: 9640 RVA: 0x00081AED File Offset: 0x0007FCED
		private string DisplayText
		{
			get
			{
				return this.poptextWhileReloading.ToPlainText();
			}
		}

		// Token: 0x060025A9 RID: 9641 RVA: 0x00081AFA File Offset: 0x0007FCFA
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x060025AA RID: 9642 RVA: 0x00081B00 File Offset: 0x0007FD00
		protected override void OnExecute()
		{
			ItemAgent_Gun gun = base.agent.CharacterMainControl.GetGun();
			if (gun == null)
			{
				base.EndAction(true);
				return;
			}
			if (gun.BulletCount <= 0)
			{
				base.agent.CharacterMainControl.TryToReload(null);
				if (!this.isFirstTime)
				{
					if (!base.agent.CharacterMainControl.Health.Hidden && this.poptextWhileReloading != string.Empty && base.agent.canTalk)
					{
						base.agent.CharacterMainControl.PopText(this.poptextWhileReloading.ToPlainText(), -1f);
					}
					if (this.postSound && this.SoundKey != string.Empty && base.agent && base.agent.CharacterMainControl)
					{
						AudioManager.PostQuak(this.SoundKey, base.agent.CharacterMainControl.AudioVoiceType, base.agent.CharacterMainControl.gameObject);
					}
				}
			}
			this.isFirstTime = false;
			base.EndAction(true);
		}

		// Token: 0x060025AB RID: 9643 RVA: 0x00081C21 File Offset: 0x0007FE21
		protected override void OnUpdate()
		{
		}

		// Token: 0x060025AC RID: 9644 RVA: 0x00081C23 File Offset: 0x0007FE23
		protected override void OnStop()
		{
		}

		// Token: 0x060025AD RID: 9645 RVA: 0x00081C25 File Offset: 0x0007FE25
		protected override void OnPause()
		{
		}

		// Token: 0x040019A3 RID: 6563
		public string poptextWhileReloading = "PopText_Reloading";

		// Token: 0x040019A4 RID: 6564
		public bool postSound;

		// Token: 0x040019A5 RID: 6565
		private bool isFirstTime = true;
	}
}
