using System;
using Duckov.NoteIndexs;
using Duckov.UI.Animations;
using Duckov.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x02000384 RID: 900
	public class NoteIndexView : View
	{
		// Token: 0x17000604 RID: 1540
		// (get) Token: 0x06001F3D RID: 7997 RVA: 0x0006D5C0 File Offset: 0x0006B7C0
		private PrefabPool<NoteIndexView_Entry> Pool
		{
			get
			{
				if (this._pool == null)
				{
					this._pool = new PrefabPool<NoteIndexView_Entry>(this.entryTemplate, null, null, null, null, true, 10, 10000, null);
				}
				return this._pool;
			}
		}

		// Token: 0x06001F3E RID: 7998 RVA: 0x0006D5F9 File Offset: 0x0006B7F9
		private void OnEnable()
		{
			NoteIndex.onNoteStatusChanged = (Action<string>)Delegate.Combine(NoteIndex.onNoteStatusChanged, new Action<string>(this.OnNoteStatusChanged));
		}

		// Token: 0x06001F3F RID: 7999 RVA: 0x0006D61B File Offset: 0x0006B81B
		private void OnDisable()
		{
			NoteIndex.onNoteStatusChanged = (Action<string>)Delegate.Remove(NoteIndex.onNoteStatusChanged, new Action<string>(this.OnNoteStatusChanged));
		}

		// Token: 0x06001F40 RID: 8000 RVA: 0x0006D63D File Offset: 0x0006B83D
		private void Update()
		{
			if (this.needFocus)
			{
				this.needFocus = false;
				this.MoveScrollViewToActiveEntry();
			}
		}

		// Token: 0x06001F41 RID: 8001 RVA: 0x0006D654 File Offset: 0x0006B854
		private void OnNoteStatusChanged(string noteKey)
		{
			this.RefreshEntries();
		}

		// Token: 0x06001F42 RID: 8002 RVA: 0x0006D65C File Offset: 0x0006B85C
		public void DoOpen()
		{
			base.Open(null);
		}

		// Token: 0x06001F43 RID: 8003 RVA: 0x0006D665 File Offset: 0x0006B865
		protected override void OnOpen()
		{
			base.OnOpen();
			this.mainFadeGroup.Show();
			this.RefreshEntries();
			this.SetDisplayTargetNote(this.displayingNote);
		}

		// Token: 0x06001F44 RID: 8004 RVA: 0x0006D68A File Offset: 0x0006B88A
		protected override void OnClose()
		{
			base.OnClose();
			this.mainFadeGroup.Hide();
		}

		// Token: 0x06001F45 RID: 8005 RVA: 0x0006D69D File Offset: 0x0006B89D
		protected override void OnCancel()
		{
			base.Close();
		}

		// Token: 0x06001F46 RID: 8006 RVA: 0x0006D6A8 File Offset: 0x0006B8A8
		private void RefreshNoteCount()
		{
			int totalNoteCount = NoteIndex.GetTotalNoteCount();
			int unlockedNoteCount = NoteIndex.GetUnlockedNoteCount();
			this.noteCountText.text = string.Format("{0} / {1}", unlockedNoteCount, totalNoteCount);
		}

		// Token: 0x06001F47 RID: 8007 RVA: 0x0006D6E4 File Offset: 0x0006B8E4
		private void RefreshEntries()
		{
			this.RefreshNoteCount();
			this.Pool.ReleaseAll();
			if (NoteIndex.Instance == null)
			{
				return;
			}
			int num = 0;
			foreach (string text in NoteIndex.GetAllNotes(false))
			{
				Note note = NoteIndex.GetNote(text);
				if (note != null)
				{
					NoteIndexView_Entry noteIndexView_Entry = this.Pool.Get(null);
					num++;
					noteIndexView_Entry.Setup(note, new Action<NoteIndexView_Entry>(this.OnEntryClicked), new Func<string>(this.GetDisplayingNote), num);
				}
			}
			this.noEntryIndicator.SetActive(num <= 0);
		}

		// Token: 0x06001F48 RID: 8008 RVA: 0x0006D794 File Offset: 0x0006B994
		private string GetDisplayingNote()
		{
			return this.displayingNote;
		}

		// Token: 0x06001F49 RID: 8009 RVA: 0x0006D79C File Offset: 0x0006B99C
		public void SetDisplayTargetNote(string noteKey)
		{
			Note note = null;
			if (!string.IsNullOrWhiteSpace(noteKey))
			{
				note = NoteIndex.GetNote(noteKey);
			}
			if (note == null)
			{
				this.displayingNote = null;
			}
			else
			{
				this.displayingNote = note.key;
			}
			foreach (NoteIndexView_Entry noteIndexView_Entry in this.Pool.ActiveEntries)
			{
				noteIndexView_Entry.NotifySelectedDisplayingNoteChanged(this.displayingNote);
			}
			this.inspector.Setup(note);
		}

		// Token: 0x06001F4A RID: 8010 RVA: 0x0006D828 File Offset: 0x0006BA28
		private void OnEntryClicked(NoteIndexView_Entry entry)
		{
			string key = entry.key;
			if (!NoteIndex.GetNoteUnlocked(key))
			{
				this.SetDisplayTargetNote("");
				return;
			}
			this.SetDisplayTargetNote(key);
		}

		// Token: 0x06001F4B RID: 8011 RVA: 0x0006D858 File Offset: 0x0006BA58
		public static void ShowNote(string noteKey, bool unlock = true)
		{
			NoteIndexView viewInstance = View.GetViewInstance<NoteIndexView>();
			if (viewInstance == null)
			{
				return;
			}
			if (unlock)
			{
				NoteIndex.SetNoteUnlocked(noteKey);
			}
			if (!(View.ActiveView is NoteIndexView))
			{
				viewInstance.Open(null);
			}
			viewInstance.SetDisplayTargetNote(noteKey);
			viewInstance.needFocus = true;
		}

		// Token: 0x06001F4C RID: 8012 RVA: 0x0006D8A0 File Offset: 0x0006BAA0
		private void MoveScrollViewToActiveEntry()
		{
			NoteIndexView_Entry displayingEntry = this.GetDisplayingEntry();
			if (displayingEntry == null)
			{
				return;
			}
			RectTransform rectTransform = displayingEntry.transform as RectTransform;
			if (rectTransform == null)
			{
				return;
			}
			float num = -rectTransform.anchoredPosition.y;
			float height = this.indexScrollView.content.rect.height;
			float num2 = 1f - num / height;
			this.indexScrollView.verticalNormalizedPosition = num2;
		}

		// Token: 0x06001F4D RID: 8013 RVA: 0x0006D914 File Offset: 0x0006BB14
		private NoteIndexView_Entry GetDisplayingEntry()
		{
			foreach (NoteIndexView_Entry noteIndexView_Entry in this.Pool.ActiveEntries)
			{
				if (noteIndexView_Entry.key == this.displayingNote)
				{
					return noteIndexView_Entry;
				}
			}
			return null;
		}

		// Token: 0x04001558 RID: 5464
		[SerializeField]
		private FadeGroup mainFadeGroup;

		// Token: 0x04001559 RID: 5465
		[SerializeField]
		private GameObject noEntryIndicator;

		// Token: 0x0400155A RID: 5466
		[SerializeField]
		private NoteIndexView_Entry entryTemplate;

		// Token: 0x0400155B RID: 5467
		[SerializeField]
		private NoteIndexView_Inspector inspector;

		// Token: 0x0400155C RID: 5468
		[SerializeField]
		private TextMeshProUGUI noteCountText;

		// Token: 0x0400155D RID: 5469
		[SerializeField]
		private ScrollRect indexScrollView;

		// Token: 0x0400155E RID: 5470
		private PrefabPool<NoteIndexView_Entry> _pool;

		// Token: 0x0400155F RID: 5471
		private string displayingNote;

		// Token: 0x04001560 RID: 5472
		private bool needFocus;
	}
}
