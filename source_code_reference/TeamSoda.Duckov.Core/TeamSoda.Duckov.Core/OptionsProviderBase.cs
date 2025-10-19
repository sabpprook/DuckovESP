using System;
using UnityEngine;

// Token: 0x020001C7 RID: 455
public abstract class OptionsProviderBase : MonoBehaviour
{
	// Token: 0x17000280 RID: 640
	// (get) Token: 0x06000D80 RID: 3456
	public abstract string Key { get; }

	// Token: 0x06000D81 RID: 3457
	public abstract string[] GetOptions();

	// Token: 0x06000D82 RID: 3458
	public abstract string GetCurrentOption();

	// Token: 0x06000D83 RID: 3459
	public abstract void Set(int index);
}
