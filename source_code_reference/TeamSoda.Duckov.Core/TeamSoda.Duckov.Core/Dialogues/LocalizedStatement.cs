using System;
using NodeCanvas.DialogueTrees;
using SodaCraft.Localizations;
using UnityEngine;

namespace Dialogues
{
	// Token: 0x0200021A RID: 538
	[Serializable]
	public class LocalizedStatement : IStatement
	{
		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x0600102A RID: 4138 RVA: 0x0003EE93 File Offset: 0x0003D093
		public string text
		{
			get
			{
				return this.textKey.ToPlainText();
			}
		}

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x0600102B RID: 4139 RVA: 0x0003EEA0 File Offset: 0x0003D0A0
		// (set) Token: 0x0600102C RID: 4140 RVA: 0x0003EEA8 File Offset: 0x0003D0A8
		public string textKey
		{
			get
			{
				return this._textKey;
			}
			set
			{
				this._textKey = value;
			}
		}

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x0600102D RID: 4141 RVA: 0x0003EEB1 File Offset: 0x0003D0B1
		// (set) Token: 0x0600102E RID: 4142 RVA: 0x0003EEB9 File Offset: 0x0003D0B9
		public AudioClip audio
		{
			get
			{
				return this._audio;
			}
			set
			{
				this._audio = value;
			}
		}

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x0600102F RID: 4143 RVA: 0x0003EEC2 File Offset: 0x0003D0C2
		// (set) Token: 0x06001030 RID: 4144 RVA: 0x0003EECA File Offset: 0x0003D0CA
		public string meta
		{
			get
			{
				return this._meta;
			}
			set
			{
				this._meta = value;
			}
		}

		// Token: 0x06001031 RID: 4145 RVA: 0x0003EED3 File Offset: 0x0003D0D3
		public LocalizedStatement()
		{
		}

		// Token: 0x06001032 RID: 4146 RVA: 0x0003EEF1 File Offset: 0x0003D0F1
		public LocalizedStatement(string textKey)
		{
			this._textKey = textKey;
		}

		// Token: 0x06001033 RID: 4147 RVA: 0x0003EF16 File Offset: 0x0003D116
		public LocalizedStatement(string textKey, AudioClip audio)
		{
			this._textKey = textKey;
			this.audio = audio;
		}

		// Token: 0x06001034 RID: 4148 RVA: 0x0003EF42 File Offset: 0x0003D142
		public LocalizedStatement(string textKey, AudioClip audio, string meta)
		{
			this._textKey = textKey;
			this.audio = audio;
			this.meta = meta;
		}

		// Token: 0x04000CEE RID: 3310
		[SerializeField]
		private string _textKey = string.Empty;

		// Token: 0x04000CEF RID: 3311
		[SerializeField]
		private AudioClip _audio;

		// Token: 0x04000CF0 RID: 3312
		[SerializeField]
		private string _meta = string.Empty;
	}
}
