using System;
using UnityEngine;

namespace Duckov.Modding
{
	// Token: 0x02000267 RID: 615
	public abstract class ModBehaviour : MonoBehaviour
	{
		// Token: 0x17000378 RID: 888
		// (get) Token: 0x06001317 RID: 4887 RVA: 0x0004722A File Offset: 0x0004542A
		// (set) Token: 0x06001318 RID: 4888 RVA: 0x00047232 File Offset: 0x00045432
		public ModManager master { get; private set; }

		// Token: 0x17000379 RID: 889
		// (get) Token: 0x06001319 RID: 4889 RVA: 0x0004723B File Offset: 0x0004543B
		// (set) Token: 0x0600131A RID: 4890 RVA: 0x00047243 File Offset: 0x00045443
		public ModInfo info { get; private set; }

		// Token: 0x0600131B RID: 4891 RVA: 0x0004724C File Offset: 0x0004544C
		public void Setup(ModManager master, ModInfo info)
		{
			this.master = master;
			this.info = info;
			this.OnAfterSetup();
		}

		// Token: 0x0600131C RID: 4892 RVA: 0x00047262 File Offset: 0x00045462
		public void NotifyBeforeDeactivate()
		{
			this.OnBeforeDeactivate();
		}

		// Token: 0x0600131D RID: 4893 RVA: 0x0004726A File Offset: 0x0004546A
		protected virtual void OnAfterSetup()
		{
		}

		// Token: 0x0600131E RID: 4894 RVA: 0x0004726C File Offset: 0x0004546C
		protected virtual void OnBeforeDeactivate()
		{
		}
	}
}
