using System;
using System.Collections.Generic;
using System.Linq;
using Eflatun.SceneReference;
using UnityEngine;

namespace Duckov.MiniMaps
{
	// Token: 0x0200026F RID: 623
	public class MiniMapCenter : MonoBehaviour
	{
		// Token: 0x17000392 RID: 914
		// (get) Token: 0x06001387 RID: 4999 RVA: 0x000489A4 File Offset: 0x00046BA4
		public float WorldSize
		{
			get
			{
				return this.worldSize;
			}
		}

		// Token: 0x06001388 RID: 5000 RVA: 0x000489AC File Offset: 0x00046BAC
		private void OnEnable()
		{
			MiniMapCenter.activeMiniMapCenters.Add(this);
			if (MiniMapCenter.activeMiniMapCenters.Count > 1)
			{
				if (MiniMapCenter.activeMiniMapCenters.Find((MiniMapCenter e) => e != null && e != this && e.gameObject.scene.buildIndex == base.gameObject.scene.buildIndex))
				{
					Debug.LogError("场景 " + base.gameObject.scene.name + " 似乎存在两个MiniMapCenter！");
				}
				return;
			}
			this.CacheThisCenter();
		}

		// Token: 0x06001389 RID: 5001 RVA: 0x00048A1C File Offset: 0x00046C1C
		private void CacheThisCenter()
		{
			MiniMapSettings instance = MiniMapSettings.Instance;
			if (instance == null)
			{
				return;
			}
			Vector3 position = base.transform.position;
			instance.Cache(this);
		}

		// Token: 0x0600138A RID: 5002 RVA: 0x00048A4C File Offset: 0x00046C4C
		private void OnDisable()
		{
			MiniMapCenter.activeMiniMapCenters.Remove(this);
		}

		// Token: 0x0600138B RID: 5003 RVA: 0x00048A5C File Offset: 0x00046C5C
		internal static Vector3 GetCenterOfObjectScene(MonoBehaviour target)
		{
			int num = target.gameObject.scene.buildIndex;
			IPointOfInterest pointOfInterest = target as IPointOfInterest;
			if (pointOfInterest != null && pointOfInterest.OverrideScene >= 0)
			{
				num = pointOfInterest.OverrideScene;
			}
			return MiniMapCenter.GetCenter(num);
		}

		// Token: 0x0600138C RID: 5004 RVA: 0x00048AA0 File Offset: 0x00046CA0
		internal static string GetSceneID(MonoBehaviour target)
		{
			int sceneBuildIndex = target.gameObject.scene.buildIndex;
			IPointOfInterest pointOfInterest = target as IPointOfInterest;
			if (pointOfInterest != null && pointOfInterest.OverrideScene >= 0)
			{
				sceneBuildIndex = pointOfInterest.OverrideScene;
			}
			MiniMapSettings instance = MiniMapSettings.Instance;
			if (instance == null)
			{
				return null;
			}
			MiniMapSettings.MapEntry mapEntry = instance.maps.Find((MiniMapSettings.MapEntry e) => e.SceneReference.UnsafeReason == SceneReferenceUnsafeReason.None && e.SceneReference.BuildIndex == sceneBuildIndex);
			if (mapEntry == null)
			{
				return null;
			}
			return mapEntry.sceneID;
		}

		// Token: 0x0600138D RID: 5005 RVA: 0x00048B20 File Offset: 0x00046D20
		internal static Vector3 GetCenter(int sceneBuildIndex)
		{
			MiniMapSettings instance = MiniMapSettings.Instance;
			if (instance == null)
			{
				return Vector3.zero;
			}
			MiniMapSettings.MapEntry mapEntry = instance.maps.FirstOrDefault((MiniMapSettings.MapEntry e) => e.SceneReference.UnsafeReason == SceneReferenceUnsafeReason.None && e.SceneReference.BuildIndex == sceneBuildIndex);
			if (mapEntry != null)
			{
				return mapEntry.mapWorldCenter;
			}
			return instance.combinedCenter;
		}

		// Token: 0x0600138E RID: 5006 RVA: 0x00048B77 File Offset: 0x00046D77
		internal static Vector3 GetCenter(string sceneID)
		{
			return MiniMapCenter.GetCenter(SceneInfoCollection.GetBuildIndex(sceneID));
		}

		// Token: 0x0600138F RID: 5007 RVA: 0x00048B84 File Offset: 0x00046D84
		internal static Vector3 GetCombinedCenter()
		{
			MiniMapSettings instance = MiniMapSettings.Instance;
			if (instance == null)
			{
				return Vector3.zero;
			}
			return instance.combinedCenter;
		}

		// Token: 0x06001390 RID: 5008 RVA: 0x00048BAC File Offset: 0x00046DAC
		private void OnDrawGizmosSelected()
		{
			if (this.WorldSize < 0f)
			{
				return;
			}
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(this.WorldSize, 1f, this.WorldSize));
		}

		// Token: 0x04000E82 RID: 3714
		private static List<MiniMapCenter> activeMiniMapCenters = new List<MiniMapCenter>();

		// Token: 0x04000E83 RID: 3715
		[SerializeField]
		private float worldSize = -1f;
	}
}
