using System;
using System.IO;
using Saves;

namespace Duckov
{
	// Token: 0x0200023A RID: 570
	public class CheatMode
	{
		// Token: 0x17000317 RID: 791
		// (get) Token: 0x060011A5 RID: 4517 RVA: 0x00043E90 File Offset: 0x00042090
		// (set) Token: 0x060011A6 RID: 4518 RVA: 0x00043E97 File Offset: 0x00042097
		public static bool Active
		{
			get
			{
				return CheatMode._acitive;
			}
			private set
			{
				CheatMode._acitive = value;
				Action<bool> onCheatModeStatusChanged = CheatMode.OnCheatModeStatusChanged;
				if (onCheatModeStatusChanged == null)
				{
					return;
				}
				onCheatModeStatusChanged(value);
			}
		}

		// Token: 0x14000075 RID: 117
		// (add) Token: 0x060011A7 RID: 4519 RVA: 0x00043EB0 File Offset: 0x000420B0
		// (remove) Token: 0x060011A8 RID: 4520 RVA: 0x00043EE4 File Offset: 0x000420E4
		public static event Action<bool> OnCheatModeStatusChanged;

		// Token: 0x060011A9 RID: 4521 RVA: 0x00043F17 File Offset: 0x00042117
		public static void Activate()
		{
			if (!CheatMode.CheatFileExists())
			{
				return;
			}
			CheatMode.Active = true;
			SavesSystem.Save<bool>("Cheated", true);
		}

		// Token: 0x060011AA RID: 4522 RVA: 0x00043F32 File Offset: 0x00042132
		public static void Deactivate()
		{
			CheatMode.Active = false;
		}

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x060011AB RID: 4523 RVA: 0x00043F3A File Offset: 0x0004213A
		private bool Cheated
		{
			get
			{
				return SavesSystem.Load<bool>("Cheated");
			}
		}

		// Token: 0x060011AC RID: 4524 RVA: 0x00043F46 File Offset: 0x00042146
		private static bool CheatFileExists()
		{
			return File.Exists(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WWSSADADBA"));
		}

		// Token: 0x04000DA5 RID: 3493
		private static bool _acitive;
	}
}
