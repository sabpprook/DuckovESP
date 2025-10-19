using System;
using Duckov.NoteIndexs;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.UI
{
	// Token: 0x02000385 RID: 901
	public class NoteIndexView_Entry : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x17000605 RID: 1541
		// (get) Token: 0x06001F4F RID: 8015 RVA: 0x0006D984 File Offset: 0x0006BB84
		public string key
		{
			get
			{
				return this.note.key;
			}
		}

		// Token: 0x06001F50 RID: 8016 RVA: 0x0006D991 File Offset: 0x0006BB91
		private void OnEnable()
		{
			NoteIndex.onNoteStatusChanged = (Action<string>)Delegate.Combine(NoteIndex.onNoteStatusChanged, new Action<string>(this.OnNoteStatusChanged));
		}

		// Token: 0x06001F51 RID: 8017 RVA: 0x0006D9B3 File Offset: 0x0006BBB3
		private void OnDisable()
		{
			NoteIndex.onNoteStatusChanged = (Action<string>)Delegate.Remove(NoteIndex.onNoteStatusChanged, new Action<string>(this.OnNoteStatusChanged));
		}

		// Token: 0x06001F52 RID: 8018 RVA: 0x0006D9D5 File Offset: 0x0006BBD5
		private void OnNoteStatusChanged(string key)
		{
			if (key != this.note.key)
			{
				return;
			}
			this.RefreshNotReadIndicator();
		}

		// Token: 0x06001F53 RID: 8019 RVA: 0x0006D9F1 File Offset: 0x0006BBF1
		private void RefreshNotReadIndicator()
		{
			this.notReadIndicator.SetActive(NoteIndex.GetNoteUnlocked(this.key) && !NoteIndex.GetNoteRead(this.key));
		}

		// Token: 0x06001F54 RID: 8020 RVA: 0x0006DA1C File Offset: 0x0006BC1C
		internal void NotifySelectedDisplayingNoteChanged(string displayingNote)
		{
			this.RefreshHighlight();
		}

		// Token: 0x06001F55 RID: 8021 RVA: 0x0006DA24 File Offset: 0x0006BC24
		private void RefreshHighlight()
		{
			bool flag = false;
			if (this.getDisplayingNote != null)
			{
				Func<string> func = this.getDisplayingNote;
				flag = ((func != null) ? func() : null) == this.key;
			}
			this.highlightIndicator.SetActive(flag);
		}

		// Token: 0x06001F56 RID: 8022 RVA: 0x0006DA68 File Offset: 0x0006BC68
		internal void Setup(Note note, Action<NoteIndexView_Entry> onClicked, Func<string> getDisplayingNote, int index)
		{
			bool noteUnlocked = NoteIndex.GetNoteUnlocked(note.key);
			this.note = note;
			this.titleText.text = (noteUnlocked ? note.Title : "???");
			this.onClicked = onClicked;
			this.getDisplayingNote = getDisplayingNote;
			if (index > 0)
			{
				this.indexText.text = index.ToString("000");
			}
			this.RefreshNotReadIndicator();
			this.RefreshHighlight();
		}

		// Token: 0x06001F57 RID: 8023 RVA: 0x0006DAD9 File Offset: 0x0006BCD9
		public void OnPointerClick(PointerEventData eventData)
		{
			Action<NoteIndexView_Entry> action = this.onClicked;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x04001561 RID: 5473
		[SerializeField]
		private GameObject highlightIndicator;

		// Token: 0x04001562 RID: 5474
		[SerializeField]
		private TextMeshProUGUI titleText;

		// Token: 0x04001563 RID: 5475
		[SerializeField]
		private TextMeshProUGUI indexText;

		// Token: 0x04001564 RID: 5476
		[SerializeField]
		private GameObject notReadIndicator;

		// Token: 0x04001565 RID: 5477
		private Note note;

		// Token: 0x04001566 RID: 5478
		private Action<NoteIndexView_Entry> onClicked;

		// Token: 0x04001567 RID: 5479
		private Func<string> getDisplayingNote;
	}
}
