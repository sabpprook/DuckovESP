using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x0200000D RID: 13
	[NullableContext(1)]
	[Nullable(0)]
	public class ESPSettings
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000042 RID: 66 RVA: 0x00003872 File Offset: 0x00001A72
		// (set) Token: 0x06000043 RID: 67 RVA: 0x0000387A File Offset: 0x00001A7A
		public bool ShowEnemyList { get; set; } = true;

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00003883 File Offset: 0x00001A83
		// (set) Token: 0x06000045 RID: 69 RVA: 0x0000388B File Offset: 0x00001A8B
		public bool ShowLines { get; set; } = true;

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000046 RID: 70 RVA: 0x00003894 File Offset: 0x00001A94
		// (set) Token: 0x06000047 RID: 71 RVA: 0x0000389C File Offset: 0x00001A9C
		public Color LineColor { get; set; } = Color.green;

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000048 RID: 72 RVA: 0x000038A5 File Offset: 0x00001AA5
		// (set) Token: 0x06000049 RID: 73 RVA: 0x000038AD File Offset: 0x00001AAD
		public bool EnablePMCAlert { get; set; } = true;

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600004A RID: 74 RVA: 0x000038B6 File Offset: 0x00001AB6
		// (set) Token: 0x0600004B RID: 75 RVA: 0x000038BE File Offset: 0x00001ABE
		public bool EnableTraderAlert { get; set; } = true;

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600004C RID: 76 RVA: 0x000038C7 File Offset: 0x00001AC7
		// (set) Token: 0x0600004D RID: 77 RVA: 0x000038CF File Offset: 0x00001ACF
		public float MaxDistance { get; set; } = 200f;

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600004E RID: 78 RVA: 0x000038D8 File Offset: 0x00001AD8
		// (set) Token: 0x0600004F RID: 79 RVA: 0x000038E0 File Offset: 0x00001AE0
		public float MaxLineDistance { get; set; } = 50f;

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000050 RID: 80 RVA: 0x000038E9 File Offset: 0x00001AE9
		// (set) Token: 0x06000051 RID: 81 RVA: 0x000038F1 File Offset: 0x00001AF1
		public float MaxUIDistance { get; set; } = 60f;

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000052 RID: 82 RVA: 0x000038FA File Offset: 0x00001AFA
		// (set) Token: 0x06000053 RID: 83 RVA: 0x00003902 File Offset: 0x00001B02
		public bool LogToFile { get; set; } = true;

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000054 RID: 84 RVA: 0x0000390B File Offset: 0x00001B0B
		// (set) Token: 0x06000055 RID: 85 RVA: 0x00003913 File Offset: 0x00001B13
		public float LineWidth { get; set; } = 2f;

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000056 RID: 86 RVA: 0x0000391C File Offset: 0x00001B1C
		// (set) Token: 0x06000057 RID: 87 RVA: 0x00003924 File Offset: 0x00001B24
		public int EnemyListDistanceFontSize { get; set; } = 18;

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000058 RID: 88 RVA: 0x0000392D File Offset: 0x00001B2D
		// (set) Token: 0x06000059 RID: 89 RVA: 0x00003935 File Offset: 0x00001B35
		public float WindowOpacity { get; set; } = 0.85f;

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600005A RID: 90 RVA: 0x0000393E File Offset: 0x00001B3E
		// (set) Token: 0x0600005B RID: 91 RVA: 0x00003946 File Offset: 0x00001B46
		public float WindowWidth { get; set; } = 350f;

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600005C RID: 92 RVA: 0x0000394F File Offset: 0x00001B4F
		// (set) Token: 0x0600005D RID: 93 RVA: 0x00003957 File Offset: 0x00001B57
		public float WindowHeight { get; set; } = 280f;

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00003960 File Offset: 0x00001B60
		// (set) Token: 0x0600005F RID: 95 RVA: 0x00003968 File Offset: 0x00001B68
		public long HighValueThreshold { get; set; } = 5000L;

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00003971 File Offset: 0x00001B71
		// (set) Token: 0x06000061 RID: 97 RVA: 0x00003979 File Offset: 0x00001B79
		public string ToggleKey { get; set; } = "F1";

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00003982 File Offset: 0x00001B82
		// (set) Token: 0x06000063 RID: 99 RVA: 0x0000398A File Offset: 0x00001B8A
		public float WindowPosX { get; set; } = 20f;

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00003993 File Offset: 0x00001B93
		// (set) Token: 0x06000065 RID: 101 RVA: 0x0000399B File Offset: 0x00001B9B
		public float WindowPosY { get; set; } = 20f;
	}
}
