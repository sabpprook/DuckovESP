using System;
using NodeCanvas.Framework;
using SodaCraft.Localizations;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x0200040B RID: 1035
	public class PopText : ActionTask<AICharacterController>
	{
		// Token: 0x1700072C RID: 1836
		// (get) Token: 0x06002554 RID: 9556 RVA: 0x00080B16 File Offset: 0x0007ED16
		private string Key
		{
			get
			{
				return this.content.value;
			}
		}

		// Token: 0x1700072D RID: 1837
		// (get) Token: 0x06002555 RID: 9557 RVA: 0x00080B23 File Offset: 0x0007ED23
		private string DisplayText
		{
			get
			{
				return this.Key.ToPlainText();
			}
		}

		// Token: 0x06002556 RID: 9558 RVA: 0x00080B30 File Offset: 0x0007ED30
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x1700072E RID: 1838
		// (get) Token: 0x06002557 RID: 9559 RVA: 0x00080B33 File Offset: 0x0007ED33
		protected override string info
		{
			get
			{
				return string.Format("Pop:'{0}'", this.DisplayText);
			}
		}

		// Token: 0x06002558 RID: 9560 RVA: 0x00080B48 File Offset: 0x0007ED48
		protected override void OnExecute()
		{
			if (this.checkHide && base.agent.CharacterMainControl.Hidden)
			{
				base.EndAction(true);
				return;
			}
			if (!base.agent.canTalk)
			{
				base.EndAction(true);
				return;
			}
			base.agent.CharacterMainControl.PopText(this.DisplayText, -1f);
			base.EndAction(true);
		}

		// Token: 0x06002559 RID: 9561 RVA: 0x00080BAE File Offset: 0x0007EDAE
		protected override void OnStop()
		{
		}

		// Token: 0x0600255A RID: 9562 RVA: 0x00080BB0 File Offset: 0x0007EDB0
		protected override void OnPause()
		{
		}

		// Token: 0x0400196F RID: 6511
		public BBParameter<string> content;

		// Token: 0x04001970 RID: 6512
		public bool checkHide;
	}
}
