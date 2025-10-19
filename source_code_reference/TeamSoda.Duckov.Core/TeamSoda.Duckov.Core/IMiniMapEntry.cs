using System;
using UnityEngine;

// Token: 0x020001BE RID: 446
public interface IMiniMapEntry
{
	// Token: 0x1700026E RID: 622
	// (get) Token: 0x06000D4F RID: 3407
	Sprite Sprite { get; }

	// Token: 0x1700026F RID: 623
	// (get) Token: 0x06000D50 RID: 3408
	float PixelSize { get; }

	// Token: 0x17000270 RID: 624
	// (get) Token: 0x06000D51 RID: 3409
	Vector2 Offset { get; }

	// Token: 0x17000271 RID: 625
	// (get) Token: 0x06000D52 RID: 3410
	string SceneID { get; }

	// Token: 0x17000272 RID: 626
	// (get) Token: 0x06000D53 RID: 3411
	bool Hide { get; }

	// Token: 0x17000273 RID: 627
	// (get) Token: 0x06000D54 RID: 3412
	bool NoSignal { get; }
}
