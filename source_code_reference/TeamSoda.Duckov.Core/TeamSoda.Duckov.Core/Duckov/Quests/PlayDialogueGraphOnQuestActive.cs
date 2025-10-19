using System;
using Duckov.UI;
using NodeCanvas.DialogueTrees;
using UnityEngine;

namespace Duckov.Quests
{
	// Token: 0x0200033A RID: 826
	public class PlayDialogueGraphOnQuestActive : MonoBehaviour
	{
		// Token: 0x06001C6D RID: 7277 RVA: 0x00066666 File Offset: 0x00064866
		private void Awake()
		{
			if (this.quest == null)
			{
				this.quest = base.GetComponent<Quest>();
			}
			this.quest.onActivated += this.OnQuestActivated;
		}

		// Token: 0x06001C6E RID: 7278 RVA: 0x00066699 File Offset: 0x00064899
		private void OnQuestActivated(Quest quest)
		{
			if (View.ActiveView != null)
			{
				View.ActiveView.Close();
			}
			this.SetupActors();
			this.PlayDialogue();
		}

		// Token: 0x06001C6F RID: 7279 RVA: 0x000666BE File Offset: 0x000648BE
		private void PlayDialogue()
		{
			this.dialogueTreeController.StartDialogue();
		}

		// Token: 0x06001C70 RID: 7280 RVA: 0x000666CC File Offset: 0x000648CC
		private void SetupActors()
		{
			if (this.dialogueTreeController.behaviour == null)
			{
				Debug.LogError("Dialoguetree没有配置", this.dialogueTreeController);
				return;
			}
			foreach (DialogueTree.ActorParameter actorParameter in this.dialogueTreeController.behaviour.actorParameters)
			{
				string name = actorParameter.name;
				if (!string.IsNullOrEmpty(name))
				{
					DuckovDialogueActor duckovDialogueActor = DuckovDialogueActor.Get(name);
					if (duckovDialogueActor == null)
					{
						Debug.LogError("未找到actor ID:" + name);
					}
					else
					{
						this.dialogueTreeController.SetActorReference(name, duckovDialogueActor);
					}
				}
			}
		}

		// Token: 0x040013D1 RID: 5073
		[SerializeField]
		private Quest quest;

		// Token: 0x040013D2 RID: 5074
		[SerializeField]
		private DialogueTreeController dialogueTreeController;
	}
}
