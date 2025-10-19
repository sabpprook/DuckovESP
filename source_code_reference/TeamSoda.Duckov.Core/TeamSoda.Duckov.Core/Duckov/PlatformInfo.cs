using System;
using UnityEngine;

namespace Duckov
{
	// Token: 0x02000236 RID: 566
	public static class PlatformInfo
	{
		// Token: 0x17000314 RID: 788
		// (get) Token: 0x06001194 RID: 4500 RVA: 0x00043D4C File Offset: 0x00041F4C
		// (set) Token: 0x06001195 RID: 4501 RVA: 0x00043D61 File Offset: 0x00041F61
		public static Platform Platform
		{
			get
			{
				if (Application.isEditor)
				{
					return Platform.UnityEditor;
				}
				return GameMetaData.Instance.Platform;
			}
			set
			{
				GameMetaData.Instance.Platform = value;
			}
		}

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x06001196 RID: 4502 RVA: 0x00043D6E File Offset: 0x00041F6E
		// (set) Token: 0x06001197 RID: 4503 RVA: 0x00043D75 File Offset: 0x00041F75
		public static Func<string> GetIDFunc
		{
			get
			{
				return PlatformInfo._getIDFunc;
			}
			set
			{
				PlatformInfo._getIDFunc = value;
			}
		}

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x06001198 RID: 4504 RVA: 0x00043D7D File Offset: 0x00041F7D
		// (set) Token: 0x06001199 RID: 4505 RVA: 0x00043D84 File Offset: 0x00041F84
		public static Func<string> GetDisplayNameFunc
		{
			get
			{
				return PlatformInfo._getDisplayNameFunc;
			}
			set
			{
				PlatformInfo._getDisplayNameFunc = value;
			}
		}

		// Token: 0x0600119A RID: 4506 RVA: 0x00043D8C File Offset: 0x00041F8C
		public static string GetID()
		{
			string text = null;
			if (PlatformInfo.GetIDFunc != null)
			{
				text = PlatformInfo.GetIDFunc();
			}
			if (text == null)
			{
				text = Environment.MachineName;
			}
			return text;
		}

		// Token: 0x0600119B RID: 4507 RVA: 0x00043DB7 File Offset: 0x00041FB7
		public static string GetDisplayName()
		{
			if (PlatformInfo.GetDisplayNameFunc != null)
			{
				return PlatformInfo.GetDisplayNameFunc();
			}
			return "UNKOWN";
		}

		// Token: 0x04000D98 RID: 3480
		private static Func<string> _getIDFunc;

		// Token: 0x04000D99 RID: 3481
		private static Func<string> _getDisplayNameFunc;
	}
}
