using System;
using UnityEngine;

// Token: 0x02000095 RID: 149
[Serializable]
public struct CharacterRandomPresetInfo
{
	// Token: 0x040004A3 RID: 1187
	public CharacterRandomPreset randomPreset;

	// Token: 0x040004A4 RID: 1188
	[Range(0f, 1f)]
	public float weight;
}
