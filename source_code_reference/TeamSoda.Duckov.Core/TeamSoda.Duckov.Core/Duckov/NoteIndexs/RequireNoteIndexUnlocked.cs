using System;
using Duckov.Quests;

namespace Duckov.NoteIndexs
{
	// Token: 0x02000266 RID: 614
	public class RequireNoteIndexUnlocked : Condition
	{
		// Token: 0x06001315 RID: 4885 RVA: 0x00047215 File Offset: 0x00045415
		public override bool Evaluate()
		{
			return NoteIndex.GetNoteUnlocked(this.key);
		}

		// Token: 0x04000E41 RID: 3649
		public string key;
	}
}
