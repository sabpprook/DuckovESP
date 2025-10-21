using System;
using Duckov.UI;

namespace Duckov.NoteIndexs
{
	// Token: 0x02000265 RID: 613
	public class NoteInteract : InteractableBase
	{
		// Token: 0x06001310 RID: 4880 RVA: 0x0004716B File Offset: 0x0004536B
		protected override void Start()
		{
			base.Start();
			if (NoteIndex.GetNoteUnlocked(this.noteKey))
			{
				base.gameObject.SetActive(false);
			}
			this.finishWhenTimeOut = true;
		}

		// Token: 0x06001311 RID: 4881 RVA: 0x00047193 File Offset: 0x00045393
		protected override void OnInteractFinished()
		{
			NoteIndex.SetNoteUnlocked(this.noteKey);
			NoteIndexView.ShowNote(this.noteKey, true);
			base.gameObject.SetActive(false);
		}

		// Token: 0x06001312 RID: 4882 RVA: 0x000471B8 File Offset: 0x000453B8
		private void OnValidate()
		{
			this.noteTitle = "Note_" + this.noteKey + "_Title";
			this.noteContent = "Note_" + this.noteKey + "_Content";
		}

		// Token: 0x06001313 RID: 4883 RVA: 0x000471F0 File Offset: 0x000453F0
		public void ReName()
		{
			base.gameObject.name = "Note_" + this.noteKey;
		}

		// Token: 0x04000E3E RID: 3646
		public string noteKey;

		// Token: 0x04000E3F RID: 3647
		[LocalizationKey("Default")]
		public string noteTitle;

		// Token: 0x04000E40 RID: 3648
		[LocalizationKey("Default")]
		public string noteContent;
	}
}
