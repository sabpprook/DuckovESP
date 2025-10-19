using System;
using Eflatun.SceneReference;
using SodaCraft.Localizations;
using UnityEngine;

namespace Duckov.Scenes
{
	// Token: 0x0200032A RID: 810
	[Serializable]
	public struct MultiSceneLocation
	{
		// Token: 0x1700050D RID: 1293
		// (get) Token: 0x06001B6E RID: 7022 RVA: 0x00063BB6 File Offset: 0x00061DB6
		// (set) Token: 0x06001B6F RID: 7023 RVA: 0x00063BBE File Offset: 0x00061DBE
		public Transform LocationTransform
		{
			get
			{
				return this.GetLocationTransform();
			}
			private set
			{
			}
		}

		// Token: 0x1700050E RID: 1294
		// (get) Token: 0x06001B70 RID: 7024 RVA: 0x00063BC0 File Offset: 0x00061DC0
		// (set) Token: 0x06001B71 RID: 7025 RVA: 0x00063BC8 File Offset: 0x00061DC8
		public string SceneID
		{
			get
			{
				return this.sceneID;
			}
			set
			{
				this.sceneID = value;
			}
		}

		// Token: 0x1700050F RID: 1295
		// (get) Token: 0x06001B72 RID: 7026 RVA: 0x00063BD4 File Offset: 0x00061DD4
		public SceneReference Scene
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

		// Token: 0x17000510 RID: 1296
		// (get) Token: 0x06001B73 RID: 7027 RVA: 0x00063BF8 File Offset: 0x00061DF8
		// (set) Token: 0x06001B74 RID: 7028 RVA: 0x00063C00 File Offset: 0x00061E00
		public string LocationName
		{
			get
			{
				return this.locationName;
			}
			set
			{
				this.locationName = value;
			}
		}

		// Token: 0x17000511 RID: 1297
		// (get) Token: 0x06001B75 RID: 7029 RVA: 0x00063C09 File Offset: 0x00061E09
		public string DisplayName
		{
			get
			{
				return this.displayName.ToPlainText();
			}
		}

		// Token: 0x06001B76 RID: 7030 RVA: 0x00063C16 File Offset: 0x00061E16
		public Transform GetLocationTransform()
		{
			if (this.Scene == null)
			{
				return null;
			}
			if (this.Scene.UnsafeReason != SceneReferenceUnsafeReason.None)
			{
				return null;
			}
			return SceneLocationsProvider.GetLocation(this.Scene, this.locationName);
		}

		// Token: 0x06001B77 RID: 7031 RVA: 0x00063C44 File Offset: 0x00061E44
		public bool TryGetLocationPosition(out Vector3 result)
		{
			result = default(Vector3);
			if (MultiSceneCore.Instance == null)
			{
				return false;
			}
			if (MultiSceneCore.Instance.TryGetCachedPosition(this.sceneID, this.locationName, out result))
			{
				return true;
			}
			Transform location = SceneLocationsProvider.GetLocation(this.sceneID, this.locationName);
			if (location != null)
			{
				result = location.transform.position;
				return true;
			}
			return false;
		}

		// Token: 0x06001B78 RID: 7032 RVA: 0x00063CB1 File Offset: 0x00061EB1
		internal string GetDisplayName()
		{
			return this.DisplayName;
		}

		// Token: 0x0400137B RID: 4987
		[SerializeField]
		private string sceneID;

		// Token: 0x0400137C RID: 4988
		[SerializeField]
		private string locationName;

		// Token: 0x0400137D RID: 4989
		[SerializeField]
		private string displayName;
	}
}
