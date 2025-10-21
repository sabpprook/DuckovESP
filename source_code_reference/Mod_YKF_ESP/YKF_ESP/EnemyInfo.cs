using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x0200000C RID: 12
	[NullableContext(1)]
	[Nullable(0)]
	public class EnemyInfo
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000038 RID: 56 RVA: 0x0000453C File Offset: 0x0000273C
		// (set) Token: 0x06000039 RID: 57 RVA: 0x00004544 File Offset: 0x00002744
		public string Name { get; set; } = "Unknown";

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600003A RID: 58 RVA: 0x0000454D File Offset: 0x0000274D
		// (set) Token: 0x0600003B RID: 59 RVA: 0x00004555 File Offset: 0x00002755
		public float HealthPercent { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600003C RID: 60 RVA: 0x0000455E File Offset: 0x0000275E
		// (set) Token: 0x0600003D RID: 61 RVA: 0x00004566 File Offset: 0x00002766
		public float CurrentHealth { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600003E RID: 62 RVA: 0x0000456F File Offset: 0x0000276F
		// (set) Token: 0x0600003F RID: 63 RVA: 0x00004577 File Offset: 0x00002777
		public float MaxHealth { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000040 RID: 64 RVA: 0x00004580 File Offset: 0x00002780
		// (set) Token: 0x06000041 RID: 65 RVA: 0x00004588 File Offset: 0x00002788
		public float Distance { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000042 RID: 66 RVA: 0x00004591 File Offset: 0x00002791
		// (set) Token: 0x06000043 RID: 67 RVA: 0x00004599 File Offset: 0x00002799
		public string Weapon { get; set; } = "Unknown";

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000044 RID: 68 RVA: 0x000045A2 File Offset: 0x000027A2
		// (set) Token: 0x06000045 RID: 69 RVA: 0x000045AA File Offset: 0x000027AA
		public long Value { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000046 RID: 70 RVA: 0x000045B3 File Offset: 0x000027B3
		// (set) Token: 0x06000047 RID: 71 RVA: 0x000045BB File Offset: 0x000027BB
		public bool IsAimingAtPlayer { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000048 RID: 72 RVA: 0x000045C4 File Offset: 0x000027C4
		// (set) Token: 0x06000049 RID: 73 RVA: 0x000045CC File Offset: 0x000027CC
		public Color SpecialColor { get; set; } = Color.green;

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600004A RID: 74 RVA: 0x000045D5 File Offset: 0x000027D5
		// (set) Token: 0x0600004B RID: 75 RVA: 0x000045DD File Offset: 0x000027DD
		public Vector3 WorldPosition { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600004C RID: 76 RVA: 0x000045E6 File Offset: 0x000027E6
		// (set) Token: 0x0600004D RID: 77 RVA: 0x000045EE File Offset: 0x000027EE
		public CharacterMainControl Character { get; set; }
	}
}
