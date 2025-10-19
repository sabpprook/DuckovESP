using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x02000009 RID: 9
	[NullableContext(1)]
	[Nullable(0)]
	public class EnemyInfo
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000020 RID: 32 RVA: 0x000030EA File Offset: 0x000012EA
		// (set) Token: 0x06000021 RID: 33 RVA: 0x000030F2 File Offset: 0x000012F2
		public string Name { get; set; } = "Unknown";

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000022 RID: 34 RVA: 0x000030FB File Offset: 0x000012FB
		// (set) Token: 0x06000023 RID: 35 RVA: 0x00003103 File Offset: 0x00001303
		public float HealthPercent { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000024 RID: 36 RVA: 0x0000310C File Offset: 0x0000130C
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00003114 File Offset: 0x00001314
		public float CurrentHealth { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000026 RID: 38 RVA: 0x0000311D File Offset: 0x0000131D
		// (set) Token: 0x06000027 RID: 39 RVA: 0x00003125 File Offset: 0x00001325
		public float MaxHealth { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000028 RID: 40 RVA: 0x0000312E File Offset: 0x0000132E
		// (set) Token: 0x06000029 RID: 41 RVA: 0x00003136 File Offset: 0x00001336
		public float Distance { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600002A RID: 42 RVA: 0x0000313F File Offset: 0x0000133F
		// (set) Token: 0x0600002B RID: 43 RVA: 0x00003147 File Offset: 0x00001347
		public string Weapon { get; set; } = "Unknown";

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600002C RID: 44 RVA: 0x00003150 File Offset: 0x00001350
		// (set) Token: 0x0600002D RID: 45 RVA: 0x00003158 File Offset: 0x00001358
		public long Value { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00003161 File Offset: 0x00001361
		// (set) Token: 0x0600002F RID: 47 RVA: 0x00003169 File Offset: 0x00001369
		public bool IsAimingAtPlayer { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00003172 File Offset: 0x00001372
		// (set) Token: 0x06000031 RID: 49 RVA: 0x0000317A File Offset: 0x0000137A
		public Color SpecialColor { get; set; } = Color.green;

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00003183 File Offset: 0x00001383
		// (set) Token: 0x06000033 RID: 51 RVA: 0x0000318B File Offset: 0x0000138B
		public Vector3 WorldPosition { get; set; }
	}
}
