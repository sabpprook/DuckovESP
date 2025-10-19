using System;
using Duckov.UI;
using UnityEngine;

namespace Duckov.NoteIndexs
{
	// Token: 0x02000264 RID: 612
	public class NoteIndexProxy : MonoBehaviour
	{
		// Token: 0x0600130D RID: 4877 RVA: 0x00047152 File Offset: 0x00045352
		public void UnlockNote(string key)
		{
			NoteIndex.SetNoteUnlocked(key);
		}

		// Token: 0x0600130E RID: 4878 RVA: 0x0004715A File Offset: 0x0004535A
		public void UnlockAndShowNote(string key)
		{
			NoteIndexView.ShowNote(key, true);
		}
	}
}
