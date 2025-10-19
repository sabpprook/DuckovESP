using System;
using System.Collections.Generic;
using Duckov.Quests;
using NodeCanvas.DialogueTrees;
using NodeCanvas.StateMachines;
using Saves;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001AE RID: 430
public class CutScene : MonoBehaviour
{
	// Token: 0x1700024A RID: 586
	// (get) Token: 0x06000CB4 RID: 3252 RVA: 0x00035357 File Offset: 0x00033557
	private string SaveKey
	{
		get
		{
			return "CutScene_" + this.id;
		}
	}

	// Token: 0x1700024B RID: 587
	// (get) Token: 0x06000CB5 RID: 3253 RVA: 0x00035369 File Offset: 0x00033569
	private bool UseTrigger
	{
		get
		{
			return this.playTiming == CutScene.PlayTiming.OnTriggerEnter;
		}
	}

	// Token: 0x1700024C RID: 588
	// (get) Token: 0x06000CB6 RID: 3254 RVA: 0x00035374 File Offset: 0x00033574
	private bool HideFSMOwnerField
	{
		get
		{
			return !this.fsmOwner && this.dialogueTreeOwner;
		}
	}

	// Token: 0x1700024D RID: 589
	// (get) Token: 0x06000CB7 RID: 3255 RVA: 0x00035390 File Offset: 0x00033590
	private bool HideDialogueTreeOwnerField
	{
		get
		{
			return this.fsmOwner && !this.dialogueTreeOwner;
		}
	}

	// Token: 0x1700024E RID: 590
	// (get) Token: 0x06000CB8 RID: 3256 RVA: 0x000353AF File Offset: 0x000335AF
	private bool Played
	{
		get
		{
			return SavesSystem.Load<bool>(this.SaveKey);
		}
	}

	// Token: 0x06000CB9 RID: 3257 RVA: 0x000353BC File Offset: 0x000335BC
	public void MarkPlayed()
	{
		if (string.IsNullOrWhiteSpace(this.id))
		{
			return;
		}
		SavesSystem.Save<bool>(this.SaveKey, true);
	}

	// Token: 0x06000CBA RID: 3258 RVA: 0x000353D8 File Offset: 0x000335D8
	private void OnEnable()
	{
	}

	// Token: 0x06000CBB RID: 3259 RVA: 0x000353DA File Offset: 0x000335DA
	private void Awake()
	{
		if (this.UseTrigger)
		{
			this.InitializeTrigger();
		}
	}

	// Token: 0x06000CBC RID: 3260 RVA: 0x000353EC File Offset: 0x000335EC
	private void InitializeTrigger()
	{
		if (this.trigger == null)
		{
			Debug.LogError("CutScene想要使用Trigger触发，但没有配置Trigger引用。", this);
		}
		OnTriggerEnterEvent onTriggerEnterEvent = this.trigger.AddComponent<OnTriggerEnterEvent>();
		onTriggerEnterEvent.onlyMainCharacter = true;
		onTriggerEnterEvent.triggerOnce = true;
		onTriggerEnterEvent.DoOnTriggerEnter.AddListener(new UnityAction(this.PlayIfNessisary));
	}

	// Token: 0x06000CBD RID: 3261 RVA: 0x00035441 File Offset: 0x00033641
	private void Start()
	{
		if (this.playTiming == CutScene.PlayTiming.Start)
		{
			this.PlayIfNessisary();
		}
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x00035454 File Offset: 0x00033654
	private void Update()
	{
		if (this.playing)
		{
			if (this.fsmOwner)
			{
				if (!this.fsmOwner.isRunning)
				{
					this.playing = false;
					this.OnPlayFinished();
					return;
				}
			}
			else if (this.dialogueTreeOwner && !this.dialogueTreeOwner.isRunning)
			{
				this.playing = false;
				this.OnPlayFinished();
			}
		}
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x000354B8 File Offset: 0x000336B8
	private void OnPlayFinished()
	{
		this.MarkPlayed();
		if (this.setActiveFalseWhenFinished)
		{
			base.gameObject.SetActive(false);
		}
		if (this.playOnce && string.IsNullOrWhiteSpace(this.id))
		{
			Debug.LogError("CutScene没有填写ID，无法记录", base.gameObject);
		}
	}

	// Token: 0x06000CC0 RID: 3264 RVA: 0x00035504 File Offset: 0x00033704
	public void PlayIfNessisary()
	{
		if (this.playOnce && this.Played)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (!this.prerequisites.Satisfied())
		{
			return;
		}
		this.Play();
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x00035538 File Offset: 0x00033738
	public void Play()
	{
		if (this.fsmOwner)
		{
			this.fsmOwner.StartBehaviour();
			this.playing = true;
			return;
		}
		if (this.dialogueTreeOwner)
		{
			if (this.setupActorReferencesUsingIDs)
			{
				this.SetupActors();
			}
			this.dialogueTreeOwner.StartBehaviour();
			this.playing = true;
		}
	}

	// Token: 0x06000CC2 RID: 3266 RVA: 0x00035594 File Offset: 0x00033794
	private void SetupActors()
	{
		if (this.dialogueTreeOwner == null)
		{
			return;
		}
		if (this.dialogueTreeOwner.behaviour == null)
		{
			Debug.LogError("Dialoguetree没有配置", this.dialogueTreeOwner);
			return;
		}
		foreach (DialogueTree.ActorParameter actorParameter in this.dialogueTreeOwner.behaviour.actorParameters)
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
					this.dialogueTreeOwner.SetActorReference(name, duckovDialogueActor);
				}
			}
		}
	}

	// Token: 0x04000B01 RID: 2817
	[SerializeField]
	private string id;

	// Token: 0x04000B02 RID: 2818
	[SerializeField]
	private bool playOnce = true;

	// Token: 0x04000B03 RID: 2819
	[SerializeField]
	private bool setActiveFalseWhenFinished = true;

	// Token: 0x04000B04 RID: 2820
	[SerializeField]
	private bool setupActorReferencesUsingIDs;

	// Token: 0x04000B05 RID: 2821
	[SerializeField]
	private Collider trigger;

	// Token: 0x04000B06 RID: 2822
	[SerializeField]
	private List<Condition> prerequisites = new List<Condition>();

	// Token: 0x04000B07 RID: 2823
	[SerializeField]
	private FSMOwner fsmOwner;

	// Token: 0x04000B08 RID: 2824
	[SerializeField]
	private DialogueTreeController dialogueTreeOwner;

	// Token: 0x04000B09 RID: 2825
	[SerializeField]
	private CutScene.PlayTiming playTiming;

	// Token: 0x04000B0A RID: 2826
	private bool playing;

	// Token: 0x020004C7 RID: 1223
	public enum PlayTiming
	{
		// Token: 0x04001CB3 RID: 7347
		Start,
		// Token: 0x04001CB4 RID: 7348
		OnTriggerEnter = 2,
		// Token: 0x04001CB5 RID: 7349
		Manual
	}
}
