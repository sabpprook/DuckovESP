using System;
using UnityEngine;

namespace Duckov
{
	// Token: 0x0200022F RID: 559
	[Serializable]
	public class CursorData
	{
		// Token: 0x17000306 RID: 774
		// (get) Token: 0x06001168 RID: 4456 RVA: 0x00043738 File Offset: 0x00041938
		public Texture2D texture
		{
			get
			{
				if (this.textures.Length == 0)
				{
					return null;
				}
				return this.textures[0];
			}
		}

		// Token: 0x06001169 RID: 4457 RVA: 0x00043750 File Offset: 0x00041950
		internal void Apply(int frame)
		{
			if (this.textures == null || this.textures.Length < 1)
			{
				Cursor.SetCursor(null, default(Vector2), CursorMode.Auto);
				return;
			}
			if (frame < 0)
			{
				int num = this.textures.Length;
				frame = (-frame / this.textures.Length + 1) * num + frame;
			}
			frame %= this.textures.Length;
			Cursor.SetCursor(this.textures[frame], this.hotspot, CursorMode.Auto);
		}

		// Token: 0x04000D7D RID: 3453
		public Texture2D[] textures;

		// Token: 0x04000D7E RID: 3454
		public Vector2 hotspot;

		// Token: 0x04000D7F RID: 3455
		public float fps;
	}
}
