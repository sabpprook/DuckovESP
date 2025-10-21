using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x02000010 RID: 16
	[NullableContext(1)]
	[Nullable(0)]
	public class ESPSettings
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000067 RID: 103 RVA: 0x00005072 File Offset: 0x00003272
		// (set) Token: 0x06000068 RID: 104 RVA: 0x0000507A File Offset: 0x0000327A
		public string Version { get; set; } = "2.0";

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000069 RID: 105 RVA: 0x00005083 File Offset: 0x00003283
		// (set) Token: 0x0600006A RID: 106 RVA: 0x0000508B File Offset: 0x0000328B
		public bool ShowEnemyList { get; set; } = true;

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600006B RID: 107 RVA: 0x00005094 File Offset: 0x00003294
		// (set) Token: 0x0600006C RID: 108 RVA: 0x0000509C File Offset: 0x0000329C
		public bool ShowLines { get; set; } = true;

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600006D RID: 109 RVA: 0x000050A5 File Offset: 0x000032A5
		// (set) Token: 0x0600006E RID: 110 RVA: 0x000050AD File Offset: 0x000032AD
		public Color LineColor { get; set; } = Color.green;

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600006F RID: 111 RVA: 0x000050B6 File Offset: 0x000032B6
		// (set) Token: 0x06000070 RID: 112 RVA: 0x000050BE File Offset: 0x000032BE
		public bool EnablePMCAlert { get; set; } = true;

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000071 RID: 113 RVA: 0x000050C7 File Offset: 0x000032C7
		// (set) Token: 0x06000072 RID: 114 RVA: 0x000050CF File Offset: 0x000032CF
		public bool EnableTraderAlert { get; set; } = true;

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000073 RID: 115 RVA: 0x000050D8 File Offset: 0x000032D8
		// (set) Token: 0x06000074 RID: 116 RVA: 0x000050E0 File Offset: 0x000032E0
		public float MaxDistance { get; set; } = 200f;

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000075 RID: 117 RVA: 0x000050E9 File Offset: 0x000032E9
		// (set) Token: 0x06000076 RID: 118 RVA: 0x000050F1 File Offset: 0x000032F1
		public float MaxLineDistance { get; set; } = 50f;

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000077 RID: 119 RVA: 0x000050FA File Offset: 0x000032FA
		// (set) Token: 0x06000078 RID: 120 RVA: 0x00005102 File Offset: 0x00003302
		public float MaxUIDistance { get; set; } = 60f;

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000079 RID: 121 RVA: 0x0000510B File Offset: 0x0000330B
		// (set) Token: 0x0600007A RID: 122 RVA: 0x00005113 File Offset: 0x00003313
		public bool LogToFile { get; set; } = true;

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600007B RID: 123 RVA: 0x0000511C File Offset: 0x0000331C
		// (set) Token: 0x0600007C RID: 124 RVA: 0x00005124 File Offset: 0x00003324
		public float LineWidth { get; set; } = 2f;

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600007D RID: 125 RVA: 0x0000512D File Offset: 0x0000332D
		// (set) Token: 0x0600007E RID: 126 RVA: 0x00005135 File Offset: 0x00003335
		public int EnemyListDistanceFontSize { get; set; } = 14;

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600007F RID: 127 RVA: 0x0000513E File Offset: 0x0000333E
		// (set) Token: 0x06000080 RID: 128 RVA: 0x00005146 File Offset: 0x00003346
		public float WindowOpacity { get; set; } = 0.85f;

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000081 RID: 129 RVA: 0x0000514F File Offset: 0x0000334F
		// (set) Token: 0x06000082 RID: 130 RVA: 0x00005157 File Offset: 0x00003357
		public float WindowWidth { get; set; } = 310f;

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000083 RID: 131 RVA: 0x00005160 File Offset: 0x00003360
		// (set) Token: 0x06000084 RID: 132 RVA: 0x00005168 File Offset: 0x00003368
		public float WindowHeight { get; set; } = 240f;

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000085 RID: 133 RVA: 0x00005171 File Offset: 0x00003371
		// (set) Token: 0x06000086 RID: 134 RVA: 0x00005179 File Offset: 0x00003379
		public long HighValueThreshold { get; set; } = 5000L;

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000087 RID: 135 RVA: 0x00005182 File Offset: 0x00003382
		// (set) Token: 0x06000088 RID: 136 RVA: 0x0000518A File Offset: 0x0000338A
		public string ToggleKey { get; set; } = "F1";

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000089 RID: 137 RVA: 0x00005193 File Offset: 0x00003393
		// (set) Token: 0x0600008A RID: 138 RVA: 0x0000519B File Offset: 0x0000339B
		public float WindowPosX { get; set; } = 20f;

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600008B RID: 139 RVA: 0x000051A4 File Offset: 0x000033A4
		// (set) Token: 0x0600008C RID: 140 RVA: 0x000051AC File Offset: 0x000033AC
		public float WindowPosY { get; set; } = 20f;

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600008D RID: 141 RVA: 0x000051B5 File Offset: 0x000033B5
		// (set) Token: 0x0600008E RID: 142 RVA: 0x000051BD File Offset: 0x000033BD
		public bool PlayBGM { get; set; } = true;

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600008F RID: 143 RVA: 0x000051C6 File Offset: 0x000033C6
		// (set) Token: 0x06000090 RID: 144 RVA: 0x000051CE File Offset: 0x000033CE
		public float MagicBulletRange { get; set; } = 30f;

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000091 RID: 145 RVA: 0x000051D7 File Offset: 0x000033D7
		// (set) Token: 0x06000092 RID: 146 RVA: 0x000051DF File Offset: 0x000033DF
		public string MagicBulletToggleKey { get; set; } = "F2";

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000093 RID: 147 RVA: 0x000051E8 File Offset: 0x000033E8
		// (set) Token: 0x06000094 RID: 148 RVA: 0x000051F0 File Offset: 0x000033F0
		public bool MagicBulletActive { get; set; }
	}
}
