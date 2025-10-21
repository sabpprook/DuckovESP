using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.UI.DialogueBubbles
{
	// Token: 0x020003E3 RID: 995
	public class DialogueBubblesManager : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x170006D4 RID: 1748
		// (get) Token: 0x060023F7 RID: 9207 RVA: 0x0007D447 File Offset: 0x0007B647
		// (set) Token: 0x060023F8 RID: 9208 RVA: 0x0007D44E File Offset: 0x0007B64E
		public static DialogueBubblesManager Instance { get; private set; }

		// Token: 0x140000F1 RID: 241
		// (add) Token: 0x060023F9 RID: 9209 RVA: 0x0007D458 File Offset: 0x0007B658
		// (remove) Token: 0x060023FA RID: 9210 RVA: 0x0007D48C File Offset: 0x0007B68C
		public static event Action<PointerEventData> onPointerClick;

		// Token: 0x060023FB RID: 9211 RVA: 0x0007D4BF File Offset: 0x0007B6BF
		private void Awake()
		{
			if (DialogueBubblesManager.Instance == null)
			{
				DialogueBubblesManager.Instance = this;
			}
			this.prefab.gameObject.SetActive(false);
			this.raycastReceiver.enabled = false;
		}

		// Token: 0x060023FC RID: 9212 RVA: 0x0007D4F4 File Offset: 0x0007B6F4
		public static async UniTask Show(string text, Transform target, float yOffset = -1f, bool needInteraction = false, bool skippable = false, float speed = -1f, float duration = 2f)
		{
			if (!(DialogueBubblesManager.Instance == null))
			{
				DialogueBubble dialogueBubble = DialogueBubblesManager.Instance.bubbles.FirstOrDefault((DialogueBubble e) => e != null && e.Target == target);
				if (dialogueBubble == null)
				{
					dialogueBubble = DialogueBubblesManager.Instance.bubbles.FirstOrDefault((DialogueBubble e) => e != null && !e.gameObject.activeSelf);
				}
				if (dialogueBubble == null)
				{
					if (DialogueBubblesManager.Instance.prefab == null)
					{
						return;
					}
					dialogueBubble = global::UnityEngine.Object.Instantiate<DialogueBubble>(DialogueBubblesManager.Instance.prefab, DialogueBubblesManager.Instance.transform);
					DialogueBubblesManager.Instance.bubbles.Add(dialogueBubble);
				}
				DialogueBubblesManager.Instance.raycastReceiver.enabled = needInteraction;
				await dialogueBubble.Show(text, target, yOffset, needInteraction, skippable, speed, duration);
				if (DialogueBubblesManager.Instance && DialogueBubblesManager.Instance.raycastReceiver)
				{
					DialogueBubblesManager.Instance.raycastReceiver.enabled = false;
				}
			}
		}

		// Token: 0x060023FD RID: 9213 RVA: 0x0007D56A File Offset: 0x0007B76A
		public void OnPointerClick(PointerEventData eventData)
		{
			Action<PointerEventData> action = DialogueBubblesManager.onPointerClick;
			if (action == null)
			{
				return;
			}
			action(eventData);
		}

		// Token: 0x04001874 RID: 6260
		[SerializeField]
		private DialogueBubble prefab;

		// Token: 0x04001875 RID: 6261
		[SerializeField]
		private Graphic raycastReceiver;

		// Token: 0x04001876 RID: 6262
		private List<DialogueBubble> bubbles = new List<DialogueBubble>();
	}
}
