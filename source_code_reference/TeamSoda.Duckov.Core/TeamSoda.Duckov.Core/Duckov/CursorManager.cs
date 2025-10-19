using System;
using System.Collections.Generic;
using UnityEngine;

namespace Duckov
{
	// Token: 0x0200022E RID: 558
	public class CursorManager : MonoBehaviour
	{
		// Token: 0x17000305 RID: 773
		// (get) Token: 0x0600115A RID: 4442 RVA: 0x0004357C File Offset: 0x0004177C
		// (set) Token: 0x0600115B RID: 4443 RVA: 0x00043583 File Offset: 0x00041783
		public static CursorManager Instance { get; private set; }

		// Token: 0x0600115C RID: 4444 RVA: 0x0004358B File Offset: 0x0004178B
		public static void Register(ICursorDataProvider dataProvider)
		{
			CursorManager.cursorDataStack.Add(dataProvider);
			CursorManager.ApplyStackData();
		}

		// Token: 0x0600115D RID: 4445 RVA: 0x0004359D File Offset: 0x0004179D
		public static bool Unregister(ICursorDataProvider dataProvider)
		{
			if (CursorManager.cursorDataStack.Count < 1)
			{
				return false;
			}
			if (!CursorManager.cursorDataStack.Contains(dataProvider))
			{
				return false;
			}
			bool flag = CursorManager.cursorDataStack.Remove(dataProvider);
			CursorManager.ApplyStackData();
			return flag;
		}

		// Token: 0x0600115E RID: 4446 RVA: 0x000435D0 File Offset: 0x000417D0
		private static void ApplyStackData()
		{
			if (CursorManager.Instance == null)
			{
				return;
			}
			if (CursorManager.cursorDataStack.Count <= 0)
			{
				CursorManager.Instance.MSetDefaultCursor();
				return;
			}
			ICursorDataProvider cursorDataProvider = CursorManager.cursorDataStack[CursorManager.cursorDataStack.Count - 1];
			if (cursorDataProvider == null)
			{
				CursorManager.Instance.MSetDefaultCursor();
			}
			CursorManager.Instance.MSetCursor(cursorDataProvider.GetCursorData());
		}

		// Token: 0x0600115F RID: 4447 RVA: 0x00043637 File Offset: 0x00041837
		private void Awake()
		{
			CursorManager.Instance = this;
			this.MSetCursor(this.defaultCursor);
		}

		// Token: 0x06001160 RID: 4448 RVA: 0x0004364C File Offset: 0x0004184C
		private void Update()
		{
			if (this.currentCursor == null)
			{
				return;
			}
			if (this.currentCursor.textures.Length < 2)
			{
				return;
			}
			this.fpsBuffer += Time.unscaledDeltaTime * this.currentCursor.fps;
			if (this.fpsBuffer > 1f)
			{
				this.fpsBuffer = 0f;
				this.frame++;
				this.RefreshCursor();
			}
		}

		// Token: 0x06001161 RID: 4449 RVA: 0x000436BD File Offset: 0x000418BD
		private void RefreshCursor()
		{
			if (this.currentCursor == null)
			{
				return;
			}
			this.currentCursor.Apply(this.frame);
		}

		// Token: 0x06001162 RID: 4450 RVA: 0x000436D9 File Offset: 0x000418D9
		public void MSetDefaultCursor()
		{
			this.MSetCursor(this.defaultCursor);
		}

		// Token: 0x06001163 RID: 4451 RVA: 0x000436E7 File Offset: 0x000418E7
		public void MSetCursor(CursorData data)
		{
			this.currentCursor = data;
			this.frame = 12;
			this.RefreshCursor();
		}

		// Token: 0x06001164 RID: 4452 RVA: 0x00043700 File Offset: 0x00041900
		private void OnDestroy()
		{
			Cursor.SetCursor(null, default(Vector2), CursorMode.Auto);
		}

		// Token: 0x06001165 RID: 4453 RVA: 0x0004371D File Offset: 0x0004191D
		internal static void NotifyRefresh()
		{
			CursorManager.ApplyStackData();
		}

		// Token: 0x04000D78 RID: 3448
		[SerializeField]
		private CursorData defaultCursor;

		// Token: 0x04000D79 RID: 3449
		public CursorData currentCursor;

		// Token: 0x04000D7A RID: 3450
		private static List<ICursorDataProvider> cursorDataStack = new List<ICursorDataProvider>();

		// Token: 0x04000D7B RID: 3451
		private int frame;

		// Token: 0x04000D7C RID: 3452
		private float fpsBuffer;
	}
}
