using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x0200000F RID: 15
	[NullableContext(1)]
	[Nullable(0)]
	public class ESPRenderer
	{
		// Token: 0x06000061 RID: 97 RVA: 0x00004CD9 File Offset: 0x00002ED9
		public ESPRenderer(ESPSettings settings, LogManager logManager)
		{
			this.settings = settings;
			this.logManager = logManager;
			this.CreateLineMaterial();
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00004CF5 File Offset: 0x00002EF5
		public void UpdateSettings(ESPSettings newSettings)
		{
			this.settings = newSettings;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00004D00 File Offset: 0x00002F00
		public void DrawLines(List<EnemyInfo> enemyInfoList, CharacterMainControl player, Camera mainCamera)
		{
			if (!this.settings.ShowLines || ((player != null) ? player.transform : null) == null || this.lineMaterial == null)
			{
				return;
			}
			Vector3 playerPosition = EnemyInfoHelper.GetPlayerPosition(player);
			GL.PushMatrix();
			this.lineMaterial.SetPass(0);
			GL.LoadOrtho();
			GL.Begin(1);
			foreach (EnemyInfo enemyInfo in enemyInfoList)
			{
				if (enemyInfo.Distance <= this.settings.MaxLineDistance)
				{
					Vector3 vector = mainCamera.WorldToScreenPoint(playerPosition);
					Vector3 vector2 = mainCamera.WorldToScreenPoint(enemyInfo.WorldPosition);
					if (vector.z > 0f && vector2.z > 0f)
					{
						Vector2 vector3 = new Vector2(vector.x / (float)Screen.width, vector.y / (float)Screen.height);
						Vector2 vector4 = new Vector2(vector2.x / (float)Screen.width, vector2.y / (float)Screen.height);
						GL.Color(enemyInfo.IsAimingAtPlayer ? Color.red : enemyInfo.SpecialColor);
						this.DrawThickLine(vector3, vector4);
					}
				}
			}
			GL.End();
			GL.PopMatrix();
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00004E5C File Offset: 0x0000305C
		private void DrawThickLine(Vector2 p1, Vector2 p2)
		{
			float num = this.settings.LineWidth / (float)Screen.width;
			GL.Vertex3(p1.x, p1.y, 0f);
			GL.Vertex3(p2.x, p2.y, 0f);
			GL.Vertex3(p1.x + num, p1.y, 0f);
			GL.Vertex3(p2.x + num, p2.y, 0f);
			GL.Vertex3(p1.x - num, p1.y, 0f);
			GL.Vertex3(p2.x - num, p2.y, 0f);
			GL.Vertex3(p1.x, p1.y + num, 0f);
			GL.Vertex3(p2.x, p2.y + num, 0f);
			GL.Vertex3(p1.x, p1.y - num, 0f);
			GL.Vertex3(p2.x, p2.y - num, 0f);
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00004F68 File Offset: 0x00003168
		private void CreateLineMaterial()
		{
			if (this.lineMaterial == null)
			{
				Shader shader = Shader.Find("Hidden/Internal-Colored") ?? Shader.Find("Sprites/Default");
				if (shader != null)
				{
					this.lineMaterial = new Material(shader);
					this.lineMaterial.hideFlags = HideFlags.HideAndDontSave;
					this.lineMaterial.SetInt("_SrcBlend", 5);
					this.lineMaterial.SetInt("_DstBlend", 10);
					this.lineMaterial.SetInt("_Cull", 0);
					this.lineMaterial.SetInt("_ZWrite", 0);
				}
			}
			if (this.lineTexture == null)
			{
				this.lineTexture = new Texture2D(1, 1);
				this.lineTexture.SetPixel(0, 0, Color.white);
				this.lineTexture.Apply();
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x0000503E File Offset: 0x0000323E
		public void Dispose()
		{
			if (this.lineMaterial != null)
			{
				Object.DestroyImmediate(this.lineMaterial);
			}
			if (this.lineTexture != null)
			{
				Object.DestroyImmediate(this.lineTexture);
			}
		}

		// Token: 0x04000037 RID: 55
		private ESPSettings settings;

		// Token: 0x04000038 RID: 56
		private LogManager logManager;

		// Token: 0x04000039 RID: 57
		private Material lineMaterial;

		// Token: 0x0400003A RID: 58
		private Texture2D lineTexture;
	}
}
