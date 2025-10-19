using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Duckov;
using Duckov.UI.Animations;
using Duckov.Utilities;
using NodeCanvas.DialogueTrees;
using SodaCraft.Localizations;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Dialogues
{
	// Token: 0x02000218 RID: 536
	public class DialogueUI : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x170002DF RID: 735
		// (get) Token: 0x06001000 RID: 4096 RVA: 0x0003E744 File Offset: 0x0003C944
		private PrefabPool<DialogueUIChoice> ChoicePool
		{
			get
			{
				if (this._choicePool == null)
				{
					this._choicePool = new PrefabPool<DialogueUIChoice>(this.choiceTemplate, null, null, null, null, true, 10, 10000, null);
				}
				return this._choicePool;
			}
		}

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x06001001 RID: 4097 RVA: 0x0003E77D File Offset: 0x0003C97D
		public static bool Active
		{
			get
			{
				return !(DialogueUI.instance == null) && DialogueUI.instance.mainFadeGroup.IsShown;
			}
		}

		// Token: 0x1400006E RID: 110
		// (add) Token: 0x06001002 RID: 4098 RVA: 0x0003E7A0 File Offset: 0x0003C9A0
		// (remove) Token: 0x06001003 RID: 4099 RVA: 0x0003E7D4 File Offset: 0x0003C9D4
		public static event Action OnDialogueStatusChanged;

		// Token: 0x06001004 RID: 4100 RVA: 0x0003E807 File Offset: 0x0003CA07
		private void Awake()
		{
			DialogueUI.instance = this;
			this.choiceTemplate.gameObject.SetActive(false);
			this.RegisterEvents();
		}

		// Token: 0x06001005 RID: 4101 RVA: 0x0003E826 File Offset: 0x0003CA26
		private void OnDestroy()
		{
			this.UnregisterEvents();
		}

		// Token: 0x06001006 RID: 4102 RVA: 0x0003E82E File Offset: 0x0003CA2E
		private void Update()
		{
			this.RefreshActorPositionIndicator();
			if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
			{
				this.Confirm();
			}
		}

		// Token: 0x06001007 RID: 4103 RVA: 0x0003E854 File Offset: 0x0003CA54
		private void OnEnable()
		{
		}

		// Token: 0x06001008 RID: 4104 RVA: 0x0003E856 File Offset: 0x0003CA56
		private void OnDisable()
		{
		}

		// Token: 0x06001009 RID: 4105 RVA: 0x0003E858 File Offset: 0x0003CA58
		private void RegisterEvents()
		{
			DialogueTree.OnDialogueStarted += this.OnDialogueStarted;
			DialogueTree.OnDialoguePaused += this.OnDialoguePaused;
			DialogueTree.OnDialogueFinished += this.OnDialogueFinished;
			DialogueTree.OnSubtitlesRequest += this.OnSubtitlesRequest;
			DialogueTree.OnMultipleChoiceRequest += this.OnMultipleChoiceRequest;
		}

		// Token: 0x0600100A RID: 4106 RVA: 0x0003E8BC File Offset: 0x0003CABC
		private void UnregisterEvents()
		{
			DialogueTree.OnDialogueStarted -= this.OnDialogueStarted;
			DialogueTree.OnDialoguePaused -= this.OnDialoguePaused;
			DialogueTree.OnDialogueFinished -= this.OnDialogueFinished;
			DialogueTree.OnSubtitlesRequest -= this.OnSubtitlesRequest;
			DialogueTree.OnMultipleChoiceRequest -= this.OnMultipleChoiceRequest;
		}

		// Token: 0x0600100B RID: 4107 RVA: 0x0003E91E File Offset: 0x0003CB1E
		private void OnMultipleChoiceRequest(MultipleChoiceRequestInfo info)
		{
			this.DoMultipleChoice(info).Forget();
		}

		// Token: 0x0600100C RID: 4108 RVA: 0x0003E92C File Offset: 0x0003CB2C
		private void OnSubtitlesRequest(SubtitlesRequestInfo info)
		{
			this.DoSubtitle(info).Forget();
		}

		// Token: 0x0600100D RID: 4109 RVA: 0x0003E93A File Offset: 0x0003CB3A
		public static void HideTextFadeGroup()
		{
			DialogueUI.instance.MHideTextFadeGroup();
		}

		// Token: 0x0600100E RID: 4110 RVA: 0x0003E946 File Offset: 0x0003CB46
		private void MHideTextFadeGroup()
		{
			this.textAreaFadeGroup.Hide();
		}

		// Token: 0x0600100F RID: 4111 RVA: 0x0003E953 File Offset: 0x0003CB53
		private void OnDialogueFinished(DialogueTree tree)
		{
			this.textAreaFadeGroup.Hide();
			InputManager.ActiveInput(base.gameObject);
			this.mainFadeGroup.Hide();
			Action onDialogueStatusChanged = DialogueUI.OnDialogueStatusChanged;
			if (onDialogueStatusChanged == null)
			{
				return;
			}
			onDialogueStatusChanged();
		}

		// Token: 0x06001010 RID: 4112 RVA: 0x0003E985 File Offset: 0x0003CB85
		private void OnDialoguePaused(DialogueTree tree)
		{
			Action onDialogueStatusChanged = DialogueUI.OnDialogueStatusChanged;
			if (onDialogueStatusChanged == null)
			{
				return;
			}
			onDialogueStatusChanged();
		}

		// Token: 0x06001011 RID: 4113 RVA: 0x0003E996 File Offset: 0x0003CB96
		private void OnDialogueStarted(DialogueTree tree)
		{
			InputManager.DisableInput(base.gameObject);
			this.mainFadeGroup.Show();
			Action onDialogueStatusChanged = DialogueUI.OnDialogueStatusChanged;
			if (onDialogueStatusChanged != null)
			{
				onDialogueStatusChanged();
			}
			this.actorNameFadeGroup.SkipHide();
		}

		// Token: 0x06001012 RID: 4114 RVA: 0x0003E9CC File Offset: 0x0003CBCC
		private async UniTask DoSubtitle(SubtitlesRequestInfo info)
		{
			this.SetupActorInfo(info.actor);
			this.continueIndicator.SetActive(false);
			string text = info.statement.text;
			TMP_TextInfo textInfo = this.text.GetTextInfo(text);
			this.text.text = text;
			this.text.maxVisibleCharacters = 0;
			await this.textAreaFadeGroup.ShowAndReturnTask();
			int totalCharacterCount = textInfo.characterCount;
			float buffer = 0f;
			this.confirmed = false;
			for (int c = 1; c <= totalCharacterCount; c++)
			{
				while (buffer < 1f && !this.confirmed)
				{
					buffer += Time.unscaledDeltaTime * this.speed;
					await UniTask.NextFrame();
				}
				buffer -= 1f;
				if (c == 1)
				{
					AudioManager.Post("UI/dialogue_start");
				}
				else
				{
					AudioManager.Post("UI/dialogue_bump");
				}
				this.text.maxVisibleCharacters = c;
			}
			this.text.maxVisibleCharacters = totalCharacterCount;
			await this.WaitForConfirm();
			await this.textAreaFadeGroup.HideAndReturnTask();
			this.text.text = string.Empty;
			this.talkingActor = null;
			info.Continue();
		}

		// Token: 0x06001013 RID: 4115 RVA: 0x0003EA18 File Offset: 0x0003CC18
		private void SetupActorInfo(IDialogueActor actor)
		{
			DuckovDialogueActor duckovDialogueActor = actor as DuckovDialogueActor;
			if (duckovDialogueActor == null)
			{
				this.actorNameFadeGroup.Hide();
				this.actorPortraitContainer.gameObject.SetActive(false);
				this.actorPositionIndicator.gameObject.SetActive(false);
				this.talkingActor = null;
				return;
			}
			this.talkingActor = duckovDialogueActor;
			Sprite portraitSprite = duckovDialogueActor.portraitSprite;
			string nameKey = duckovDialogueActor.NameKey;
			Transform transform = duckovDialogueActor.transform;
			this.actorNameText.text = nameKey.ToPlainText();
			this.actorNameFadeGroup.Show();
			this.actorPortraitContainer.SetActive(portraitSprite);
			this.actorPortraitDisplay.sprite = portraitSprite;
			if (this.talkingActor.transform != null)
			{
				this.actorPositionIndicator.gameObject.SetActive(true);
			}
			this.RefreshActorPositionIndicator();
		}

		// Token: 0x06001014 RID: 4116 RVA: 0x0003EAE4 File Offset: 0x0003CCE4
		private void RefreshActorPositionIndicator()
		{
			if (this.talkingActor == null)
			{
				this.actorPositionIndicator.gameObject.SetActive(false);
				return;
			}
			this.actorPositionIndicator.MatchWorldPosition(this.talkingActor.transform.position + this.talkingActor.Offset, default(Vector3));
		}

		// Token: 0x06001015 RID: 4117 RVA: 0x0003EB48 File Offset: 0x0003CD48
		private async UniTask DoMultipleChoice(MultipleChoiceRequestInfo info)
		{
			await this.DisplayOptions(info.options);
			int choice = await this.WaitForChoice();
			await this.choiceListFadeGroup.HideAndReturnTask();
			info.SelectOption(choice);
		}

		// Token: 0x06001016 RID: 4118 RVA: 0x0003EB94 File Offset: 0x0003CD94
		private async UniTask DisplayOptions(Dictionary<IStatement, int> options)
		{
			this.ChoicePool.ReleaseAll();
			foreach (KeyValuePair<IStatement, int> keyValuePair in options)
			{
				DialogueUIChoice dialogueUIChoice = this.ChoicePool.Get(null);
				dialogueUIChoice.Setup(this, keyValuePair);
				dialogueUIChoice.transform.SetAsLastSibling();
			}
			this.choiceMenu.SelectDefault();
			await this.choiceListFadeGroup.ShowAndReturnTask();
			this.choiceMenu.Focused = true;
		}

		// Token: 0x06001017 RID: 4119 RVA: 0x0003EBDF File Offset: 0x0003CDDF
		internal void NotifyChoiceConfirmed(DialogueUIChoice choice)
		{
			this.confirmedChoice = choice.Index;
		}

		// Token: 0x06001018 RID: 4120 RVA: 0x0003EBF0 File Offset: 0x0003CDF0
		private async UniTask<int> WaitForChoice()
		{
			this.confirmedChoice = -1;
			this.waitingForChoice = true;
			while (this.confirmedChoice < 0)
			{
				await UniTask.NextFrame();
			}
			this.waitingForChoice = false;
			return this.confirmedChoice;
		}

		// Token: 0x06001019 RID: 4121 RVA: 0x0003EC33 File Offset: 0x0003CE33
		public void Confirm()
		{
			this.confirmed = true;
		}

		// Token: 0x0600101A RID: 4122 RVA: 0x0003EC3C File Offset: 0x0003CE3C
		private async UniTask WaitForConfirm()
		{
			this.continueIndicator.SetActive(true);
			this.confirmed = false;
			while (!this.confirmed)
			{
				await UniTask.NextFrame();
			}
			this.continueIndicator.SetActive(false);
		}

		// Token: 0x0600101B RID: 4123 RVA: 0x0003EC7F File Offset: 0x0003CE7F
		public void OnPointerClick(PointerEventData eventData)
		{
			this.Confirm();
		}

		// Token: 0x04000CD2 RID: 3282
		private static DialogueUI instance;

		// Token: 0x04000CD3 RID: 3283
		[SerializeField]
		private FadeGroup mainFadeGroup;

		// Token: 0x04000CD4 RID: 3284
		[SerializeField]
		private FadeGroup textAreaFadeGroup;

		// Token: 0x04000CD5 RID: 3285
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x04000CD6 RID: 3286
		[SerializeField]
		private GameObject continueIndicator;

		// Token: 0x04000CD7 RID: 3287
		[SerializeField]
		private float speed = 10f;

		// Token: 0x04000CD8 RID: 3288
		[SerializeField]
		private RectTransform actorPositionIndicator;

		// Token: 0x04000CD9 RID: 3289
		[SerializeField]
		private FadeGroup actorNameFadeGroup;

		// Token: 0x04000CDA RID: 3290
		[SerializeField]
		private TextMeshProUGUI actorNameText;

		// Token: 0x04000CDB RID: 3291
		[SerializeField]
		private GameObject actorPortraitContainer;

		// Token: 0x04000CDC RID: 3292
		[SerializeField]
		private Image actorPortraitDisplay;

		// Token: 0x04000CDD RID: 3293
		[SerializeField]
		private FadeGroup choiceListFadeGroup;

		// Token: 0x04000CDE RID: 3294
		[SerializeField]
		private Menu choiceMenu;

		// Token: 0x04000CDF RID: 3295
		[SerializeField]
		private DialogueUIChoice choiceTemplate;

		// Token: 0x04000CE0 RID: 3296
		private PrefabPool<DialogueUIChoice> _choicePool;

		// Token: 0x04000CE1 RID: 3297
		private DuckovDialogueActor talkingActor;

		// Token: 0x04000CE3 RID: 3299
		private int confirmedChoice;

		// Token: 0x04000CE4 RID: 3300
		private bool waitingForChoice;

		// Token: 0x04000CE5 RID: 3301
		private bool confirmed;
	}
}
