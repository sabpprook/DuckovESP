using System;
using Duckov.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.Quests.UI
{
	// Token: 0x0200034B RID: 843
	public class TaskEntry : MonoBehaviour, IPoolable, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x17000572 RID: 1394
		// (get) Token: 0x06001D2D RID: 7469 RVA: 0x000687C6 File Offset: 0x000669C6
		// (set) Token: 0x06001D2E RID: 7470 RVA: 0x000687CE File Offset: 0x000669CE
		public bool Interactable
		{
			get
			{
				return this.interactable;
			}
			internal set
			{
				this.interactable = value;
			}
		}

		// Token: 0x06001D2F RID: 7471 RVA: 0x000687D7 File Offset: 0x000669D7
		private void Awake()
		{
			this.interactionButton.onClick.AddListener(new UnityAction(this.OnInteractionButtonClicked));
		}

		// Token: 0x06001D30 RID: 7472 RVA: 0x000687F5 File Offset: 0x000669F5
		private void OnInteractionButtonClicked()
		{
			if (this.target == null)
			{
				return;
			}
			this.target.Interact();
		}

		// Token: 0x06001D31 RID: 7473 RVA: 0x00068811 File Offset: 0x00066A11
		public void NotifyPooled()
		{
		}

		// Token: 0x06001D32 RID: 7474 RVA: 0x00068813 File Offset: 0x00066A13
		public void NotifyReleased()
		{
			this.UnregisterEvents();
			this.target = null;
		}

		// Token: 0x06001D33 RID: 7475 RVA: 0x00068822 File Offset: 0x00066A22
		internal void Setup(Task target)
		{
			this.UnregisterEvents();
			this.target = target;
			this.RegisterEvents();
			this.Refresh();
		}

		// Token: 0x06001D34 RID: 7476 RVA: 0x00068840 File Offset: 0x00066A40
		private void Refresh()
		{
			if (this.target == null)
			{
				return;
			}
			this.description.text = this.target.Description;
			foreach (string text in this.target.ExtraDescriptsions)
			{
				TextMeshProUGUI textMeshProUGUI = this.description;
				textMeshProUGUI.text = textMeshProUGUI.text + "  \n- " + text;
			}
			Sprite icon = this.target.Icon;
			if (icon)
			{
				this.taskIcon.sprite = icon;
				this.taskIcon.gameObject.SetActive(true);
			}
			else
			{
				this.taskIcon.gameObject.SetActive(false);
			}
			bool flag = this.target.IsFinished();
			this.statusIcon.sprite = (flag ? this.satisfiedIcon : this.unsatisfiedIcon);
			if (this.Interactable && !flag && this.target.Interactable)
			{
				bool possibleValidInteraction = this.target.PossibleValidInteraction;
				this.interactionText.text = this.target.InteractText;
				this.interactionPlaceHolderText.text = this.target.InteractText;
				this.interactionButton.gameObject.SetActive(possibleValidInteraction);
				this.targetNotInteractablePlaceHolder.gameObject.SetActive(!possibleValidInteraction);
				return;
			}
			this.interactionButton.gameObject.SetActive(false);
			this.targetNotInteractablePlaceHolder.gameObject.SetActive(false);
		}

		// Token: 0x06001D35 RID: 7477 RVA: 0x000689B8 File Offset: 0x00066BB8
		private void RegisterEvents()
		{
			if (this.target == null)
			{
				return;
			}
			this.target.onStatusChanged += this.OnTargetStatusChanged;
		}

		// Token: 0x06001D36 RID: 7478 RVA: 0x000689E0 File Offset: 0x00066BE0
		private void UnregisterEvents()
		{
			if (this.target == null)
			{
				return;
			}
			this.target.onStatusChanged -= this.OnTargetStatusChanged;
		}

		// Token: 0x06001D37 RID: 7479 RVA: 0x00068A08 File Offset: 0x00066C08
		private void OnTargetStatusChanged(Task task)
		{
			if (task != this.target)
			{
				Debug.LogError("目标不匹配。");
				return;
			}
			this.Refresh();
		}

		// Token: 0x06001D38 RID: 7480 RVA: 0x00068A29 File Offset: 0x00066C29
		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.used)
			{
				return;
			}
			if (CheatMode.Active && UIInputManager.Ctrl && UIInputManager.Alt && UIInputManager.Shift)
			{
				this.target.ForceFinish();
				eventData.Use();
			}
		}

		// Token: 0x0400144C RID: 5196
		[SerializeField]
		private Image statusIcon;

		// Token: 0x0400144D RID: 5197
		[SerializeField]
		private Image taskIcon;

		// Token: 0x0400144E RID: 5198
		[SerializeField]
		private TextMeshProUGUI description;

		// Token: 0x0400144F RID: 5199
		[SerializeField]
		private Button interactionButton;

		// Token: 0x04001450 RID: 5200
		[SerializeField]
		private GameObject targetNotInteractablePlaceHolder;

		// Token: 0x04001451 RID: 5201
		[SerializeField]
		private TextMeshProUGUI interactionText;

		// Token: 0x04001452 RID: 5202
		[SerializeField]
		private TextMeshProUGUI interactionPlaceHolderText;

		// Token: 0x04001453 RID: 5203
		[SerializeField]
		private Sprite unsatisfiedIcon;

		// Token: 0x04001454 RID: 5204
		[SerializeField]
		private Sprite satisfiedIcon;

		// Token: 0x04001455 RID: 5205
		[SerializeField]
		private bool interactable;

		// Token: 0x04001456 RID: 5206
		private Task target;
	}
}
