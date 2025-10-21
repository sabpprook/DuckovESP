using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Duckov.UI;

namespace YKF_ESP
{
	// Token: 0x02000008 RID: 8
	[NullableContext(1)]
	[Nullable(0)]
	public class CacheManager
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000013 RID: 19 RVA: 0x000027B6 File Offset: 0x000009B6
		// (set) Token: 0x06000014 RID: 20 RVA: 0x000027BE File Offset: 0x000009BE
		public InventoryDisplay[] InventoryDisplayCache { get; private set; } = new InventoryDisplay[0];

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000015 RID: 21 RVA: 0x000027C7 File Offset: 0x000009C7
		// (set) Token: 0x06000016 RID: 22 RVA: 0x000027CF File Offset: 0x000009CF
		public CharacterMainControl[] AllCharactersCache { get; private set; } = new CharacterMainControl[0];

		// Token: 0x06000017 RID: 23 RVA: 0x000027D8 File Offset: 0x000009D8
		public IEnumerator UpdateCaches()
		{
			CacheManager.<UpdateCaches>d__9 <UpdateCaches>d__ = new CacheManager.<UpdateCaches>d__9(0);
			<UpdateCaches>d__.<>4__this = this;
			return <UpdateCaches>d__;
		}

		// Token: 0x04000012 RID: 18
		private const float cacheUpdateInterval = 10f;
	}
}
