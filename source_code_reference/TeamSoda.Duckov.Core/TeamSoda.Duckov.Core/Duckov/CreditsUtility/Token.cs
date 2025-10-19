using System;

namespace Duckov.CreditsUtility
{
	// Token: 0x020002F8 RID: 760
	public struct Token
	{
		// Token: 0x060018B9 RID: 6329 RVA: 0x00059FC8 File Offset: 0x000581C8
		public Token(TokenType type, string text = null)
		{
			this.type = type;
			this.text = text;
		}

		// Token: 0x04001203 RID: 4611
		public TokenType type;

		// Token: 0x04001204 RID: 4612
		public string text;
	}
}
