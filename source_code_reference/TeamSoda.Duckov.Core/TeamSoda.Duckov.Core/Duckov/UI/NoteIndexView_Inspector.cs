using System;
using Duckov.NoteIndexs;
using Duckov.UI.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x02000386 RID: 902
	public class NoteIndexView_Inspector : MonoBehaviour
	{
		// Token: 0x06001F59 RID: 8025 RVA: 0x0006DAF4 File Offset: 0x0006BCF4
		private void Awake()
		{
			this.placeHolder.Show();
			this.content.SkipHide();
		}

		// Token: 0x06001F5A RID: 8026 RVA: 0x0006DB0C File Offset: 0x0006BD0C
		internal void Setup(Note value)
		{
			if (value == null)
			{
				this.placeHolder.Show();
				this.content.Hide();
				return;
			}
			this.note = value;
			this.SetupContent(this.note);
			this.placeHolder.Hide();
			this.content.Show();
			NoteIndex.SetNoteRead(value.key);
		}

		// Token: 0x06001F5B RID: 8027 RVA: 0x0006DB68 File Offset: 0x0006BD68
		private void SetupContent(Note value)
		{
			this.textTitle.text = value.Title;
			this.textContent.text = value.Content;
			this.image.sprite = value.image;
			this.image.gameObject.SetActive(value.image == null);
		}

		// Token: 0x04001568 RID: 5480
		[SerializeField]
		private FadeGroup placeHolder;

		// Token: 0x04001569 RID: 5481
		[SerializeField]
		private FadeGroup content;

		// Token: 0x0400156A RID: 5482
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x0400156B RID: 5483
		[SerializeField]
		private TextMeshProUGUI textContent;

		// Token: 0x0400156C RID: 5484
		[SerializeField]
		private Image image;

		// Token: 0x0400156D RID: 5485
		private Note note;
	}
}
