using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.MiniMaps
{
	// Token: 0x0200026E RID: 622
	public class MapMarkerPOI : MonoBehaviour, IPointOfInterest
	{
		// Token: 0x1700038C RID: 908
		// (get) Token: 0x0600137C RID: 4988 RVA: 0x000488B5 File Offset: 0x00046AB5
		public MapMarkerPOI.RuntimeData Data
		{
			get
			{
				return this.data;
			}
		}

		// Token: 0x1700038D RID: 909
		// (get) Token: 0x0600137D RID: 4989 RVA: 0x000488BD File Offset: 0x00046ABD
		public Sprite Icon
		{
			get
			{
				return MapMarkerManager.GetIcon(this.data.iconName);
			}
		}

		// Token: 0x1700038E RID: 910
		// (get) Token: 0x0600137E RID: 4990 RVA: 0x000488CF File Offset: 0x00046ACF
		public int OverrideScene
		{
			get
			{
				return SceneInfoCollection.GetBuildIndex(this.data.overrideSceneKey);
			}
		}

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x0600137F RID: 4991 RVA: 0x000488E1 File Offset: 0x00046AE1
		public Color Color
		{
			get
			{
				return this.data.color;
			}
		}

		// Token: 0x17000390 RID: 912
		// (get) Token: 0x06001380 RID: 4992 RVA: 0x000488EE File Offset: 0x00046AEE
		public Color ShadowColor
		{
			get
			{
				return Color.black;
			}
		}

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x06001381 RID: 4993 RVA: 0x000488F5 File Offset: 0x00046AF5
		public float ScaleFactor
		{
			get
			{
				return 0.8f;
			}
		}

		// Token: 0x06001382 RID: 4994 RVA: 0x000488FC File Offset: 0x00046AFC
		public void Setup(Vector3 worldPosition, string iconName = "", string overrideScene = "", Color? color = null)
		{
			this.data = new MapMarkerPOI.RuntimeData
			{
				worldPosition = worldPosition,
				iconName = iconName,
				overrideSceneKey = overrideScene,
				color = ((color == null) ? Color.white : color.Value)
			};
			base.transform.position = worldPosition;
			PointsOfInterests.Unregister(this);
			PointsOfInterests.Register(this);
		}

		// Token: 0x06001383 RID: 4995 RVA: 0x00048966 File Offset: 0x00046B66
		public void Setup(MapMarkerPOI.RuntimeData data)
		{
			this.data = data;
			base.transform.position = data.worldPosition;
			PointsOfInterests.Unregister(this);
			PointsOfInterests.Register(this);
		}

		// Token: 0x06001384 RID: 4996 RVA: 0x0004898C File Offset: 0x00046B8C
		public void NotifyClicked(PointerEventData eventData)
		{
			MapMarkerManager.Release(this);
		}

		// Token: 0x06001385 RID: 4997 RVA: 0x00048994 File Offset: 0x00046B94
		private void OnDestroy()
		{
			PointsOfInterests.Unregister(this);
		}

		// Token: 0x04000E81 RID: 3713
		[SerializeField]
		private MapMarkerPOI.RuntimeData data;

		// Token: 0x0200053E RID: 1342
		[Serializable]
		public struct RuntimeData
		{
			// Token: 0x04001E96 RID: 7830
			public Vector3 worldPosition;

			// Token: 0x04001E97 RID: 7831
			public string iconName;

			// Token: 0x04001E98 RID: 7832
			public string overrideSceneKey;

			// Token: 0x04001E99 RID: 7833
			public Color color;
		}
	}
}
