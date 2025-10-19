using System;
using System.Collections.Generic;
using Eflatun.SceneReference;
using UnityEngine;

namespace Duckov.Scenes
{
	// Token: 0x02000329 RID: 809
	[Serializable]
	public class SubSceneEntry
	{
		// Token: 0x17000508 RID: 1288
		// (get) Token: 0x06001B67 RID: 7015 RVA: 0x00063AB8 File Offset: 0x00061CB8
		public string AmbientSound
		{
			get
			{
				return this.overrideAmbientSound;
			}
		}

		// Token: 0x17000509 RID: 1289
		// (get) Token: 0x06001B68 RID: 7016 RVA: 0x00063AC0 File Offset: 0x00061CC0
		public bool IsInDoor
		{
			get
			{
				return this.isInDoor;
			}
		}

		// Token: 0x1700050A RID: 1290
		// (get) Token: 0x06001B69 RID: 7017 RVA: 0x00063AC8 File Offset: 0x00061CC8
		public SceneInfoEntry Info
		{
			get
			{
				return SceneInfoCollection.GetSceneInfo(this.sceneID);
			}
		}

		// Token: 0x1700050B RID: 1291
		// (get) Token: 0x06001B6A RID: 7018 RVA: 0x00063AD8 File Offset: 0x00061CD8
		public SceneReference SceneReference
		{
			get
			{
				SceneInfoEntry info = this.Info;
				if (info == null)
				{
					Debug.LogWarning("未找到场景" + this.sceneID + "的相关信息，获取SceneReference失败。");
					return null;
				}
				return info.SceneReference;
			}
		}

		// Token: 0x1700050C RID: 1292
		// (get) Token: 0x06001B6B RID: 7019 RVA: 0x00063B14 File Offset: 0x00061D14
		public string DisplayName
		{
			get
			{
				SceneInfoEntry info = this.Info;
				if (info == null)
				{
					return this.sceneID;
				}
				return info.DisplayName;
			}
		}

		// Token: 0x06001B6C RID: 7020 RVA: 0x00063B38 File Offset: 0x00061D38
		internal bool TryGetCachedPosition(string locationPath, out Vector3 result)
		{
			result = default(Vector3);
			if (this.cachedLocations == null)
			{
				return false;
			}
			SubSceneEntry.Location location = this.cachedLocations.Find((SubSceneEntry.Location e) => e.path == locationPath);
			if (location == null)
			{
				return false;
			}
			result = location.position;
			return true;
		}

		// Token: 0x04001376 RID: 4982
		[SceneID]
		public string sceneID;

		// Token: 0x04001377 RID: 4983
		[SerializeField]
		private string overrideAmbientSound = "Default";

		// Token: 0x04001378 RID: 4984
		[SerializeField]
		private bool isInDoor;

		// Token: 0x04001379 RID: 4985
		public List<SubSceneEntry.Location> cachedLocations = new List<SubSceneEntry.Location>();

		// Token: 0x0400137A RID: 4986
		public List<SubSceneEntry.TeleporterInfo> cachedTeleporters = new List<SubSceneEntry.TeleporterInfo>();

		// Token: 0x020005D8 RID: 1496
		[Serializable]
		public class Location
		{
			// Token: 0x17000788 RID: 1928
			// (get) Token: 0x06002914 RID: 10516 RVA: 0x0009866A File Offset: 0x0009686A
			public string DisplayName
			{
				get
				{
					return this.displayName;
				}
			}

			// Token: 0x17000789 RID: 1929
			// (get) Token: 0x06002915 RID: 10517 RVA: 0x00098672 File Offset: 0x00096872
			// (set) Token: 0x06002916 RID: 10518 RVA: 0x0009867A File Offset: 0x0009687A
			public string DisplayNameRaw
			{
				get
				{
					return this.displayName;
				}
				set
				{
					this.displayName = value;
				}
			}

			// Token: 0x040020C9 RID: 8393
			public string path;

			// Token: 0x040020CA RID: 8394
			public Vector3 position;

			// Token: 0x040020CB RID: 8395
			public bool showInMap;

			// Token: 0x040020CC RID: 8396
			[SerializeField]
			private string displayName;
		}

		// Token: 0x020005D9 RID: 1497
		[Serializable]
		public class TeleporterInfo
		{
			// Token: 0x040020CD RID: 8397
			public Vector3 position;

			// Token: 0x040020CE RID: 8398
			public MultiSceneLocation target;

			// Token: 0x040020CF RID: 8399
			public Vector3 nearestTeleporterPositionToTarget;
		}
	}
}
