using System;
using System.Collections;
using System.Collections.Generic;

namespace Duckov.CreditsUtility
{
	// Token: 0x020002F9 RID: 761
	public class CreditsLexer : IEnumerable<Token>, IEnumerable
	{
		// Token: 0x060018BA RID: 6330 RVA: 0x00059FD8 File Offset: 0x000581D8
		public CreditsLexer(string content)
		{
			this.content = content;
			this.cursor = 0;
			this.lineBegin = 0;
		}

		// Token: 0x060018BB RID: 6331 RVA: 0x00059FF5 File Offset: 0x000581F5
		public void Reset()
		{
			this.cursor = 0;
			this.lineBegin = 0;
		}

		// Token: 0x060018BC RID: 6332 RVA: 0x0005A008 File Offset: 0x00058208
		private void TrimLeft()
		{
			while ((int)this.cursor < this.content.Length)
			{
				char c = this.content[(int)this.cursor];
				if (!char.IsWhiteSpace(c))
				{
					return;
				}
				if (c == '\n')
				{
					return;
				}
				this.cursor += 1;
			}
		}

		// Token: 0x060018BD RID: 6333 RVA: 0x0005A05C File Offset: 0x0005825C
		public Token Next()
		{
			this.TrimLeft();
			if ((int)this.cursor >= this.content.Length)
			{
				this.cursor += 1;
				return new Token(TokenType.End, null);
			}
			char c = this.content[(int)this.cursor];
			if (c == '\n')
			{
				this.cursor += 1;
				return new Token(TokenType.EmptyLine, null);
			}
			if (c == '#')
			{
				this.cursor += 1;
				int num = (int)this.cursor;
				while ((int)this.cursor < this.content.Length && this.content[(int)this.cursor] != '\n')
				{
					this.cursor += 1;
				}
				this.cursor += 1;
				return new Token(TokenType.Comment, this.content.Substring(num, (int)this.cursor));
			}
			if (c == '[')
			{
				this.cursor += 1;
				int num2 = (int)this.cursor;
				while ((int)this.cursor < this.content.Length)
				{
					if (this.content[(int)this.cursor] == ']')
					{
						string text = this.content.Substring(num2, (int)this.cursor - num2);
						while ((int)this.cursor < this.content.Length)
						{
							this.cursor += 1;
							if ((int)this.cursor >= this.content.Length)
							{
								break;
							}
							c = this.content[(int)this.cursor];
							if (c == '\n')
							{
								this.cursor += 1;
								break;
							}
							if (!char.IsWhiteSpace(c))
							{
								break;
							}
						}
						return new Token(TokenType.Instructor, text);
					}
					if (this.content[(int)this.cursor] == '\n')
					{
						this.cursor += 1;
						return new Token(TokenType.Invalid, this.content.Substring(num2, (int)this.cursor - num2));
					}
					this.cursor += 1;
				}
				return new Token(TokenType.Invalid, this.content.Substring(num2 - 1));
			}
			int num3 = (int)this.cursor;
			string text2;
			while ((int)this.cursor < this.content.Length)
			{
				c = this.content[(int)this.cursor];
				if (c == '\n')
				{
					text2 = this.content.Substring(num3, (int)this.cursor - num3);
					this.cursor += 1;
					return new Token(TokenType.String, this.ConvertEscapes(text2));
				}
				if (c == '#')
				{
					text2 = this.content.Substring(num3, (int)this.cursor - num3);
					return new Token(TokenType.String, this.ConvertEscapes(text2));
				}
				this.cursor += 1;
			}
			text2 = this.content.Substring(num3, (int)this.cursor - num3);
			return new Token(TokenType.String, this.ConvertEscapes(text2));
		}

		// Token: 0x060018BE RID: 6334 RVA: 0x0005A347 File Offset: 0x00058547
		private string ConvertEscapes(string raw)
		{
			return raw.Replace("\\n", "\n");
		}

		// Token: 0x060018BF RID: 6335 RVA: 0x0005A359 File Offset: 0x00058559
		public IEnumerator<Token> GetEnumerator()
		{
			while ((int)this.cursor < this.content.Length)
			{
				Token token = this.Next();
				yield return token;
			}
			yield break;
		}

		// Token: 0x060018C0 RID: 6336 RVA: 0x0005A368 File Offset: 0x00058568
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x04001205 RID: 4613
		private readonly string content;

		// Token: 0x04001206 RID: 4614
		private ushort cursor;

		// Token: 0x04001207 RID: 4615
		private ushort lineBegin;
	}
}
