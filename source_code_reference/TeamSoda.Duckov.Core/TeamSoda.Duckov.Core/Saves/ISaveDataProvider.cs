using System;

namespace Saves
{
	// Token: 0x02000221 RID: 545
	public interface ISaveDataProvider
	{
		// Token: 0x0600105F RID: 4191
		object GenerateSaveData();

		// Token: 0x06001060 RID: 4192
		void SetupSaveData(object data);
	}
}
