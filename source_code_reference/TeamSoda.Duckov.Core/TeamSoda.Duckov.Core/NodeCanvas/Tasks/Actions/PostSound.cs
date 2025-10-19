using System;
using Duckov;
using NodeCanvas.Framework;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x0200040C RID: 1036
	public class PostSound : ActionTask<AICharacterController>
	{
		// Token: 0x0600255C RID: 9564 RVA: 0x00080BBA File Offset: 0x0007EDBA
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x1700072F RID: 1839
		// (get) Token: 0x0600255D RID: 9565 RVA: 0x00080BBD File Offset: 0x0007EDBD
		protected override string info
		{
			get
			{
				return string.Format("Post Sound: {0} ", this.voiceSound.ToString());
			}
		}

		// Token: 0x0600255E RID: 9566 RVA: 0x00080BDC File Offset: 0x0007EDDC
		protected override void OnExecute()
		{
			if (base.agent && base.agent.CharacterMainControl)
			{
				if (!base.agent.canTalk)
				{
					base.EndAction(true);
					return;
				}
				GameObject gameObject = base.agent.CharacterMainControl.gameObject;
				switch (this.voiceSound)
				{
				case PostSound.VoiceSounds.normal:
					AudioManager.PostQuak("normal", base.agent.CharacterMainControl.AudioVoiceType, gameObject);
					break;
				case PostSound.VoiceSounds.surprise:
					AudioManager.PostQuak("surprise", base.agent.CharacterMainControl.AudioVoiceType, gameObject);
					break;
				case PostSound.VoiceSounds.death:
					AudioManager.PostQuak("death", base.agent.CharacterMainControl.AudioVoiceType, gameObject);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			base.EndAction(true);
		}

		// Token: 0x0600255F RID: 9567 RVA: 0x00080CB6 File Offset: 0x0007EEB6
		protected override void OnStop()
		{
		}

		// Token: 0x06002560 RID: 9568 RVA: 0x00080CB8 File Offset: 0x0007EEB8
		protected override void OnPause()
		{
		}

		// Token: 0x04001971 RID: 6513
		public PostSound.VoiceSounds voiceSound;

		// Token: 0x0200066C RID: 1644
		public enum VoiceSounds
		{
			// Token: 0x04002318 RID: 8984
			normal,
			// Token: 0x04002319 RID: 8985
			surprise,
			// Token: 0x0400231A RID: 8986
			death
		}
	}
}
