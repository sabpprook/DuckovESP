using System;
using System.Collections.Generic;
using System.Linq;
using Saves;
using Sirenix.Utilities;
using UnityEngine;

namespace Duckov.NoteIndexs
{
	// Token: 0x02000261 RID: 609
	public class NoteIndex : MonoBehaviour
	{
		// Token: 0x1700036E RID: 878
		// (get) Token: 0x060012EF RID: 4847 RVA: 0x00046E04 File Offset: 0x00045004
		public static NoteIndex Instance
		{
			get
			{
				return GameManager.NoteIndex;
			}
		}

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x060012F0 RID: 4848 RVA: 0x00046E0B File Offset: 0x0004500B
		public List<Note> Notes
		{
			get
			{
				return this.notes;
			}
		}

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x060012F1 RID: 4849 RVA: 0x00046E13 File Offset: 0x00045013
		private Dictionary<string, Note> MDic
		{
			get
			{
				if (this._dic == null)
				{
					this.RebuildDic();
				}
				return this._dic;
			}
		}

		// Token: 0x060012F2 RID: 4850 RVA: 0x00046E2C File Offset: 0x0004502C
		private void RebuildDic()
		{
			if (this._dic == null)
			{
				this._dic = new Dictionary<string, Note>();
			}
			this._dic.Clear();
			foreach (Note note in this.notes)
			{
				this._dic[note.key] = note;
			}
		}

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x060012F3 RID: 4851 RVA: 0x00046EA8 File Offset: 0x000450A8
		public HashSet<string> UnlockedNotes
		{
			get
			{
				return this.unlockedNotes;
			}
		}

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x060012F4 RID: 4852 RVA: 0x00046EB0 File Offset: 0x000450B0
		public HashSet<string> ReadNotes
		{
			get
			{
				return this.unlockedNotes;
			}
		}

		// Token: 0x060012F5 RID: 4853 RVA: 0x00046EB8 File Offset: 0x000450B8
		public static IEnumerable<string> GetAllNotes(bool unlockedOnly = true)
		{
			if (NoteIndex.Instance == null)
			{
				yield break;
			}
			foreach (Note note in NoteIndex.Instance.notes)
			{
				string key = note.key;
				if (!unlockedOnly || NoteIndex.GetNoteUnlocked(key))
				{
					yield return note.key;
				}
			}
			List<Note>.Enumerator enumerator = default(List<Note>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060012F6 RID: 4854 RVA: 0x00046EC8 File Offset: 0x000450C8
		private void Awake()
		{
			SavesSystem.OnCollectSaveData += this.Save;
			SavesSystem.OnSetFile += this.Load;
			this.Load();
		}

		// Token: 0x060012F7 RID: 4855 RVA: 0x00046EF2 File Offset: 0x000450F2
		private void OnDestroy()
		{
			SavesSystem.OnCollectSaveData -= this.Save;
			SavesSystem.OnSetFile -= this.Load;
		}

		// Token: 0x060012F8 RID: 4856 RVA: 0x00046F18 File Offset: 0x00045118
		private void Save()
		{
			NoteIndex.SaveData saveData = new NoteIndex.SaveData(this);
			SavesSystem.Save<NoteIndex.SaveData>("NoteIndexData", saveData);
		}

		// Token: 0x060012F9 RID: 4857 RVA: 0x00046F38 File Offset: 0x00045138
		private void Load()
		{
			SavesSystem.Load<NoteIndex.SaveData>("NoteIndexData").Setup(this);
		}

		// Token: 0x060012FA RID: 4858 RVA: 0x00046F58 File Offset: 0x00045158
		public void MSetEntryDynamic(Note note)
		{
			this.MDic[note.key] = note;
		}

		// Token: 0x060012FB RID: 4859 RVA: 0x00046F6C File Offset: 0x0004516C
		public Note MGetNote(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				Debug.LogError("Trying to get note with an empty key.");
				return null;
			}
			Note note;
			if (!this.MDic.TryGetValue(key, out note))
			{
				Debug.LogError("Cannot find note: " + key);
				return null;
			}
			return note;
		}

		// Token: 0x060012FC RID: 4860 RVA: 0x00046FB0 File Offset: 0x000451B0
		public static Note GetNote(string key)
		{
			if (NoteIndex.Instance == null)
			{
				return null;
			}
			return NoteIndex.Instance.MGetNote(key);
		}

		// Token: 0x060012FD RID: 4861 RVA: 0x00046FCC File Offset: 0x000451CC
		public static bool SetNoteDynamic(Note note)
		{
			if (NoteIndex.Instance == null)
			{
				return false;
			}
			NoteIndex.Instance.MSetEntryDynamic(note);
			return true;
		}

		// Token: 0x060012FE RID: 4862 RVA: 0x00046FE9 File Offset: 0x000451E9
		public static bool GetNoteUnlocked(string noteKey)
		{
			return !(NoteIndex.Instance == null) && NoteIndex.Instance.unlockedNotes.Contains(noteKey);
		}

		// Token: 0x060012FF RID: 4863 RVA: 0x0004700A File Offset: 0x0004520A
		public static bool GetNoteRead(string noteKey)
		{
			return !(NoteIndex.Instance == null) && NoteIndex.Instance.readNotes.Contains(noteKey);
		}

		// Token: 0x06001300 RID: 4864 RVA: 0x0004702B File Offset: 0x0004522B
		public static void SetNoteUnlocked(string noteKey)
		{
			if (NoteIndex.Instance == null)
			{
				return;
			}
			NoteIndex.Instance.unlockedNotes.Add(noteKey);
			Action<string> action = NoteIndex.onNoteStatusChanged;
			if (action == null)
			{
				return;
			}
			action(noteKey);
		}

		// Token: 0x06001301 RID: 4865 RVA: 0x0004705C File Offset: 0x0004525C
		public static void SetNoteRead(string noteKey)
		{
			if (NoteIndex.Instance == null)
			{
				return;
			}
			NoteIndex.Instance.readNotes.Add(noteKey);
			Action<string> action = NoteIndex.onNoteStatusChanged;
			if (action == null)
			{
				return;
			}
			action(noteKey);
		}

		// Token: 0x06001302 RID: 4866 RVA: 0x0004708D File Offset: 0x0004528D
		internal static int GetTotalNoteCount()
		{
			if (NoteIndex.Instance == null)
			{
				return 0;
			}
			return NoteIndex.Instance.Notes.Count<Note>();
		}

		// Token: 0x06001303 RID: 4867 RVA: 0x000470AD File Offset: 0x000452AD
		internal static int GetUnlockedNoteCount()
		{
			if (NoteIndex.Instance == null)
			{
				return 0;
			}
			return NoteIndex.Instance.UnlockedNotes.Count;
		}

		// Token: 0x04000E33 RID: 3635
		[SerializeField]
		private List<Note> notes = new List<Note>();

		// Token: 0x04000E34 RID: 3636
		private Dictionary<string, Note> _dic;

		// Token: 0x04000E35 RID: 3637
		private HashSet<string> unlockedNotes = new HashSet<string>();

		// Token: 0x04000E36 RID: 3638
		private HashSet<string> readNotes = new HashSet<string>();

		// Token: 0x04000E37 RID: 3639
		public static Action<string> onNoteStatusChanged;

		// Token: 0x04000E38 RID: 3640
		private const string SaveKey = "NoteIndexData";

		// Token: 0x02000536 RID: 1334
		[Serializable]
		private struct SaveData
		{
			// Token: 0x060027A8 RID: 10152 RVA: 0x0009116B File Offset: 0x0008F36B
			public SaveData(NoteIndex from)
			{
				this.unlockedNotes = from.unlockedNotes.ToList<string>();
				this.readNotes = from.unlockedNotes.ToList<string>();
			}

			// Token: 0x060027A9 RID: 10153 RVA: 0x00091190 File Offset: 0x0008F390
			public void Setup(NoteIndex to)
			{
				to.unlockedNotes.Clear();
				if (this.unlockedNotes != null)
				{
					to.unlockedNotes.AddRange(this.unlockedNotes);
				}
				to.readNotes.Clear();
				if (this.readNotes != null)
				{
					to.readNotes.AddRange(this.readNotes);
				}
			}

			// Token: 0x04001E75 RID: 7797
			public List<string> unlockedNotes;

			// Token: 0x04001E76 RID: 7798
			public List<string> readNotes;
		}
	}
}
