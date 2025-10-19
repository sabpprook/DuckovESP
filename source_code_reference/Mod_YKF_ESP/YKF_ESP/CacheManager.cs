using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Duckov.UI;

namespace YKF_ESP
{
	// Token: 0x02000005 RID: 5
	[NullableContext(1)]
	[Nullable(0)]
	public class CacheManager
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000005 RID: 5 RVA: 0x0000208E File Offset: 0x0000028E
		// (set) Token: 0x06000006 RID: 6 RVA: 0x00002096 File Offset: 0x00000296
		public InventoryDisplay[] InventoryDisplayCache { get; private set; } = new InventoryDisplay[0];

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000007 RID: 7 RVA: 0x0000209F File Offset: 0x0000029F
		// (set) Token: 0x06000008 RID: 8 RVA: 0x000020A7 File Offset: 0x000002A7
		public CharacterMainControl[] AllCharactersCache { get; private set; } = new CharacterMainControl[0];

		// Token: 0x06000009 RID: 9 RVA: 0x000020B0 File Offset: 0x000002B0
		public IEnumerator UpdateCaches()
		{
			CacheManager.<UpdateCaches>d__9 <UpdateCaches>d__ = new CacheManager.<UpdateCaches>d__9(0);
			<UpdateCaches>d__.<>4__this = this;
			return <UpdateCaches>d__;
		}

		// Token: 0x04000005 RID: 5
		private const float cacheUpdateInterval = 3f;
	}
}
