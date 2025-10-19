using System;

namespace Duckov.Tasks
{
	// Token: 0x0200036B RID: 875
	public interface ITaskBehaviour
	{
		// Token: 0x06001E4B RID: 7755
		void Begin();

		// Token: 0x06001E4C RID: 7756
		bool IsPending();

		// Token: 0x06001E4D RID: 7757
		bool IsComplete();

		// Token: 0x06001E4E RID: 7758 RVA: 0x0006AC18 File Offset: 0x00068E18
		void Skip()
		{
		}
	}
}
