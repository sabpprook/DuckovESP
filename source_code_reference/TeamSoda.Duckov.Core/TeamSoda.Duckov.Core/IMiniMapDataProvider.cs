using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001BD RID: 445
public interface IMiniMapDataProvider
{
	// Token: 0x1700026A RID: 618
	// (get) Token: 0x06000D4B RID: 3403
	Sprite CombinedSprite { get; }

	// Token: 0x1700026B RID: 619
	// (get) Token: 0x06000D4C RID: 3404
	List<IMiniMapEntry> Maps { get; }

	// Token: 0x1700026C RID: 620
	// (get) Token: 0x06000D4D RID: 3405
	float PixelSize { get; }

	// Token: 0x1700026D RID: 621
	// (get) Token: 0x06000D4E RID: 3406
	Vector3 CombinedCenter { get; }
}
