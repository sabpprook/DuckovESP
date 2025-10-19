using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Scenes;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Duckov.MiniMaps
{
	// Token: 0x02000270 RID: 624
	public class MiniMapSettings : MonoBehaviour, IMiniMapDataProvider
	{
		// Token: 0x17000393 RID: 915
		// (get) Token: 0x06001394 RID: 5012 RVA: 0x00048C55 File Offset: 0x00046E55
		public Sprite CombinedSprite
		{
			get
			{
				return this.combinedSprite;
			}
		}

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x06001395 RID: 5013 RVA: 0x00048C5D File Offset: 0x00046E5D
		public Vector3 CombinedCenter
		{
			get
			{
				return this.combinedCenter;
			}
		}

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x06001396 RID: 5014 RVA: 0x00048C65 File Offset: 0x00046E65
		public List<IMiniMapEntry> Maps
		{
			get
			{
				return this.maps.ToList<IMiniMapEntry>();
			}
		}

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x06001397 RID: 5015 RVA: 0x00048C72 File Offset: 0x00046E72
		// (set) Token: 0x06001398 RID: 5016 RVA: 0x00048C79 File Offset: 0x00046E79
		public static MiniMapSettings Instance { get; private set; }

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x06001399 RID: 5017 RVA: 0x00048C84 File Offset: 0x00046E84
		public float PixelSize
		{
			get
			{
				int width = this.combinedSprite.texture.width;
				if (width > 0 && this.combinedSize > 0f)
				{
					return this.combinedSize / (float)width;
				}
				return -1f;
			}
		}

		// Token: 0x0600139A RID: 5018 RVA: 0x00048CC4 File Offset: 0x00046EC4
		private void Awake()
		{
			foreach (MiniMapSettings.MapEntry mapEntry in this.maps)
			{
				SpriteRenderer offsetReference = mapEntry.offsetReference;
				if (offsetReference != null)
				{
					offsetReference.gameObject.SetActive(false);
				}
			}
			if (MiniMapSettings.Instance == null)
			{
				MiniMapSettings.Instance = this;
			}
		}

		// Token: 0x0600139B RID: 5019 RVA: 0x00048D40 File Offset: 0x00046F40
		public static bool TryGetMinimapPosition(Vector3 worldPosition, string sceneID, out Vector3 result)
		{
			result = worldPosition;
			if (MiniMapSettings.Instance == null)
			{
				return false;
			}
			if (string.IsNullOrEmpty(sceneID))
			{
				return false;
			}
			MiniMapSettings.MapEntry mapEntry = MiniMapSettings.Instance.maps.FirstOrDefault((MiniMapSettings.MapEntry e) => e != null && e.sceneID == sceneID);
			if (mapEntry == null)
			{
				return false;
			}
			Vector3 vector = worldPosition - mapEntry.mapWorldCenter;
			Vector3 vector2 = mapEntry.mapWorldCenter - MiniMapSettings.Instance.combinedCenter;
			vector + vector2;
			return true;
		}

		// Token: 0x0600139C RID: 5020 RVA: 0x00048DCC File Offset: 0x00046FCC
		public static bool TryGetWorldPosition(Vector3 minimapPosition, string sceneID, out Vector3 result)
		{
			result = minimapPosition;
			if (MiniMapSettings.Instance == null)
			{
				return false;
			}
			if (string.IsNullOrEmpty(sceneID))
			{
				return false;
			}
			MiniMapSettings.MapEntry mapEntry = MiniMapSettings.Instance.maps.FirstOrDefault((MiniMapSettings.MapEntry e) => e != null && e.sceneID == sceneID);
			if (mapEntry == null)
			{
				return false;
			}
			result = mapEntry.mapWorldCenter + minimapPosition;
			return true;
		}

		// Token: 0x0600139D RID: 5021 RVA: 0x00048E40 File Offset: 0x00047040
		public static bool TryGetMinimapPosition(Vector3 worldPosition, out Vector3 result)
		{
			result = worldPosition;
			Scene activeScene = SceneManager.GetActiveScene();
			if (!activeScene.isLoaded)
			{
				return false;
			}
			string sceneID = SceneInfoCollection.GetSceneID(activeScene.buildIndex);
			return MiniMapSettings.TryGetMinimapPosition(worldPosition, sceneID, out result);
		}

		// Token: 0x0600139E RID: 5022 RVA: 0x00048E7C File Offset: 0x0004707C
		internal void Cache(MiniMapCenter miniMapCenter)
		{
			int scene = miniMapCenter.gameObject.scene.buildIndex;
			MiniMapSettings.MapEntry mapEntry = this.maps.FirstOrDefault((MiniMapSettings.MapEntry e) => e.SceneReference != null && e.SceneReference.UnsafeReason == SceneReferenceUnsafeReason.None && e.SceneReference.BuildIndex == scene);
			if (mapEntry == null)
			{
				return;
			}
			mapEntry.mapWorldCenter = miniMapCenter.transform.position;
		}

		// Token: 0x04000E84 RID: 3716
		public List<MiniMapSettings.MapEntry> maps;

		// Token: 0x04000E85 RID: 3717
		public Vector3 combinedCenter;

		// Token: 0x04000E86 RID: 3718
		public float combinedSize;

		// Token: 0x04000E87 RID: 3719
		public Sprite combinedSprite;

		// Token: 0x02000541 RID: 1345
		[Serializable]
		public class MapEntry : IMiniMapEntry
		{
			// Token: 0x17000758 RID: 1880
			// (get) Token: 0x060027C1 RID: 10177 RVA: 0x00091C30 File Offset: 0x0008FE30
			public SceneReference SceneReference
			{
				get
				{
					SceneInfoEntry sceneInfo = SceneInfoCollection.GetSceneInfo(this.sceneID);
					if (sceneInfo == null)
					{
						return null;
					}
					return sceneInfo.SceneReference;
				}
			}

			// Token: 0x17000759 RID: 1881
			// (get) Token: 0x060027C2 RID: 10178 RVA: 0x00091C54 File Offset: 0x0008FE54
			public string SceneID
			{
				get
				{
					return this.sceneID;
				}
			}

			// Token: 0x1700075A RID: 1882
			// (get) Token: 0x060027C3 RID: 10179 RVA: 0x00091C5C File Offset: 0x0008FE5C
			public Sprite Sprite
			{
				get
				{
					return this.sprite;
				}
			}

			// Token: 0x1700075B RID: 1883
			// (get) Token: 0x060027C4 RID: 10180 RVA: 0x00091C64 File Offset: 0x0008FE64
			public bool Hide
			{
				get
				{
					return this.hide;
				}
			}

			// Token: 0x1700075C RID: 1884
			// (get) Token: 0x060027C5 RID: 10181 RVA: 0x00091C6C File Offset: 0x0008FE6C
			public bool NoSignal
			{
				get
				{
					return this.noSignal;
				}
			}

			// Token: 0x1700075D RID: 1885
			// (get) Token: 0x060027C6 RID: 10182 RVA: 0x00091C74 File Offset: 0x0008FE74
			public float PixelSize
			{
				get
				{
					int width = this.sprite.texture.width;
					if (width > 0 && this.imageWorldSize > 0f)
					{
						return this.imageWorldSize / (float)width;
					}
					return -1f;
				}
			}

			// Token: 0x1700075E RID: 1886
			// (get) Token: 0x060027C7 RID: 10183 RVA: 0x00091CB2 File Offset: 0x0008FEB2
			public Vector2 Offset
			{
				get
				{
					if (this.offsetReference == null)
					{
						return Vector2.zero;
					}
					return this.offsetReference.transform.localPosition;
				}
			}

			// Token: 0x060027C8 RID: 10184 RVA: 0x00091CDD File Offset: 0x0008FEDD
			public MapEntry()
			{
			}

			// Token: 0x060027C9 RID: 10185 RVA: 0x00091CE5 File Offset: 0x0008FEE5
			public MapEntry(MiniMapSettings.MapEntry copyFrom)
			{
				this.imageWorldSize = copyFrom.imageWorldSize;
				this.sceneID = copyFrom.sceneID;
				this.sprite = copyFrom.sprite;
			}

			// Token: 0x04001E9C RID: 7836
			public float imageWorldSize;

			// Token: 0x04001E9D RID: 7837
			[SceneID]
			public string sceneID;

			// Token: 0x04001E9E RID: 7838
			public Sprite sprite;

			// Token: 0x04001E9F RID: 7839
			public SpriteRenderer offsetReference;

			// Token: 0x04001EA0 RID: 7840
			public Vector3 mapWorldCenter;

			// Token: 0x04001EA1 RID: 7841
			public bool hide;

			// Token: 0x04001EA2 RID: 7842
			public bool noSignal;
		}

		// Token: 0x02000542 RID: 1346
		public struct Data
		{
		}
	}
}
