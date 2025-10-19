using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using Duckov.UI.DialogueBubbles;
using UnityEngine;

namespace Duckov.Quests
{
	// Token: 0x0200033B RID: 827
	public class ShowDialogueOnQuestActivate : MonoBehaviour
	{
		// Token: 0x06001C72 RID: 7282 RVA: 0x0006678C File Offset: 0x0006498C
		private void Awake()
		{
			if (this.quest == null)
			{
				this.quest = base.GetComponent<Quest>();
			}
			this.quest.onActivated += this.OnQuestActivated;
		}

		// Token: 0x06001C73 RID: 7283 RVA: 0x000667BF File Offset: 0x000649BF
		private void OnQuestActivated(Quest quest)
		{
			this.ShowDIalogue().Forget();
		}

		// Token: 0x06001C74 RID: 7284 RVA: 0x000667CC File Offset: 0x000649CC
		private async UniTask ShowDIalogue()
		{
			this.cachedQuestGiverTransform = null;
			await GameplayUIManager.TemporaryHide();
			this.cachedQuestGiverTransform = this.GetQuestGiverTransform(this.quest);
			if (this.cachedQuestGiverTransform == null)
			{
				Debug.LogError("没找到QuestGiver " + this.quest.QuestGiverID.ToString() + " 的transform");
			}
			else
			{
				foreach (ShowDialogueOnQuestActivate.DialogueEntry dialogueEntry in this.dialogueEntries)
				{
					await this.ShowDialogueEntry(dialogueEntry);
				}
				List<ShowDialogueOnQuestActivate.DialogueEntry>.Enumerator enumerator = default(List<ShowDialogueOnQuestActivate.DialogueEntry>.Enumerator);
			}
			await GameplayUIManager.ReverseTemporaryHide();
		}

		// Token: 0x06001C75 RID: 7285 RVA: 0x00066810 File Offset: 0x00064A10
		private async UniTask ShowDialogueEntry(ShowDialogueOnQuestActivate.DialogueEntry cur)
		{
			await DialogueBubblesManager.Show(cur.content, this.cachedQuestGiverTransform, -1f, true, true, -1f, 2f);
		}

		// Token: 0x06001C76 RID: 7286 RVA: 0x0006685C File Offset: 0x00064A5C
		private Transform GetQuestGiverTransform(Quest quest)
		{
			QuestGiverID id = quest.QuestGiverID;
			QuestGiver questGiver = global::UnityEngine.Object.FindObjectsByType<QuestGiver>(FindObjectsSortMode.None).FirstOrDefault((QuestGiver e) => e != null && e.ID == id);
			if (questGiver == null)
			{
				return null;
			}
			return questGiver.transform;
		}

		// Token: 0x040013D3 RID: 5075
		[SerializeField]
		private Quest quest;

		// Token: 0x040013D4 RID: 5076
		[SerializeField]
		private List<ShowDialogueOnQuestActivate.DialogueEntry> dialogueEntries;

		// Token: 0x040013D5 RID: 5077
		private Transform cachedQuestGiverTransform;

		// Token: 0x020005F4 RID: 1524
		[Serializable]
		public class DialogueEntry
		{
			// Token: 0x04002109 RID: 8457
			[TextArea]
			public string content;
		}
	}
}
