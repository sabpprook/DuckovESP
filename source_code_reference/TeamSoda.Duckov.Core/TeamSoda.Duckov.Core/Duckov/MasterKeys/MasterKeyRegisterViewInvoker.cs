using System;
using Duckov.MasterKeys.UI;

namespace Duckov.MasterKeys
{
	// Token: 0x020002DA RID: 730
	public class MasterKeyRegisterViewInvoker : InteractableBase
	{
		// Token: 0x06001743 RID: 5955 RVA: 0x00055836 File Offset: 0x00053A36
		protected override void Awake()
		{
			base.Awake();
			this.finishWhenTimeOut = true;
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x00055845 File Offset: 0x00053A45
		protected override void OnInteractFinished()
		{
			MasterKeysRegisterView.Show();
		}
	}
}
