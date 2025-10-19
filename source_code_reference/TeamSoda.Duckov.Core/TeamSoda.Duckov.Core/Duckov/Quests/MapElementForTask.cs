using System;
using System.Collections.Generic;
using Duckov.MiniMaps;
using Duckov.Scenes;
using UnityEngine;

namespace Duckov.Quests
{
	// Token: 0x02000339 RID: 825
	public class MapElementForTask : MonoBehaviour
	{
		// Token: 0x06001C63 RID: 7267 RVA: 0x000663D5 File Offset: 0x000645D5
		public void SetVisibility(bool _visable)
		{
			if (this.visable == _visable)
			{
				return;
			}
			this.visable = _visable;
			if (MultiSceneCore.Instance == null)
			{
				LevelManager.OnLevelInitialized += this.OnLevelInitialized;
				return;
			}
			this.SyncVisibility();
		}

		// Token: 0x06001C64 RID: 7268 RVA: 0x0006640D File Offset: 0x0006460D
		private void OnLevelInitialized()
		{
			this.SyncVisibility();
		}

		// Token: 0x06001C65 RID: 7269 RVA: 0x00066415 File Offset: 0x00064615
		private void OnDestroy()
		{
			LevelManager.OnLevelInitialized -= this.OnLevelInitialized;
		}

		// Token: 0x06001C66 RID: 7270 RVA: 0x00066428 File Offset: 0x00064628
		private void OnDisable()
		{
			LevelManager.OnLevelInitialized -= this.OnLevelInitialized;
		}

		// Token: 0x06001C67 RID: 7271 RVA: 0x0006643B File Offset: 0x0006463B
		private void SyncVisibility()
		{
			if (this.visable)
			{
				if (this.pointsInstance != null && this.pointsInstance.Count > 0)
				{
					this.DespawnAll();
				}
				this.Spawn();
				return;
			}
			this.DespawnAll();
		}

		// Token: 0x06001C68 RID: 7272 RVA: 0x00066470 File Offset: 0x00064670
		private void Spawn()
		{
			foreach (MultiSceneLocation multiSceneLocation in this.locations)
			{
				this.SpawnOnePoint(multiSceneLocation, this.name);
			}
		}

		// Token: 0x06001C69 RID: 7273 RVA: 0x000664CC File Offset: 0x000646CC
		private void SpawnOnePoint(MultiSceneLocation _location, string name)
		{
			if (this.pointsInstance == null)
			{
				this.pointsInstance = new List<SimplePointOfInterest>();
			}
			if (MultiSceneCore.Instance == null)
			{
				return;
			}
			Vector3 vector;
			if (!_location.TryGetLocationPosition(out vector))
			{
				return;
			}
			SimplePointOfInterest simplePointOfInterest = new GameObject("MapElement:" + name).AddComponent<SimplePointOfInterest>();
			Debug.Log("Spawning " + simplePointOfInterest.name + " for task", this);
			simplePointOfInterest.Color = this.iconColor;
			simplePointOfInterest.ShadowColor = this.shadowColor;
			simplePointOfInterest.ShadowDistance = this.shadowDistance;
			if (this.range > 0f)
			{
				simplePointOfInterest.IsArea = true;
				simplePointOfInterest.AreaRadius = this.range;
			}
			simplePointOfInterest.Setup(this.icon, name, false, null);
			simplePointOfInterest.SetupMultiSceneLocation(_location, true);
			this.pointsInstance.Add(simplePointOfInterest);
		}

		// Token: 0x06001C6A RID: 7274 RVA: 0x000665A0 File Offset: 0x000647A0
		public void DespawnAll()
		{
			if (this.pointsInstance == null || this.pointsInstance.Count == 0)
			{
				return;
			}
			foreach (SimplePointOfInterest simplePointOfInterest in this.pointsInstance)
			{
				global::UnityEngine.Object.Destroy(simplePointOfInterest.gameObject);
			}
			this.pointsInstance.Clear();
		}

		// Token: 0x06001C6B RID: 7275 RVA: 0x00066618 File Offset: 0x00064818
		public void DespawnPoint(SimplePointOfInterest point)
		{
			if (this.pointsInstance != null && this.pointsInstance.Contains(point))
			{
				this.pointsInstance.Remove(point);
			}
			global::UnityEngine.Object.Destroy(point.gameObject);
		}

		// Token: 0x040013C8 RID: 5064
		private bool visable;

		// Token: 0x040013C9 RID: 5065
		public new string name;

		// Token: 0x040013CA RID: 5066
		public List<MultiSceneLocation> locations;

		// Token: 0x040013CB RID: 5067
		public float range;

		// Token: 0x040013CC RID: 5068
		private List<SimplePointOfInterest> pointsInstance;

		// Token: 0x040013CD RID: 5069
		public Sprite icon;

		// Token: 0x040013CE RID: 5070
		public Color iconColor = Color.white;

		// Token: 0x040013CF RID: 5071
		public Color shadowColor = Color.white;

		// Token: 0x040013D0 RID: 5072
		public float shadowDistance;
	}
}
