using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Cinemachine.Utility;
using Duckov.Scenes;
using Duckov.Utilities;
using UI_Spline_Renderer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;

namespace Duckov.MiniMaps.UI
{
	// Token: 0x02000275 RID: 629
	public class MiniMapDisplay : MonoBehaviour, IScrollHandler, IEventSystemHandler
	{
		// Token: 0x060013D2 RID: 5074 RVA: 0x00049394 File Offset: 0x00047594
		public bool NoSignal()
		{
			foreach (MiniMapDisplayEntry miniMapDisplayEntry in this.MapEntryPool.ActiveEntries)
			{
				if (!(miniMapDisplayEntry == null) && !(miniMapDisplayEntry.SceneID != MultiSceneCore.ActiveSubSceneID) && miniMapDisplayEntry.NoSignal())
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x060013D3 RID: 5075 RVA: 0x0004940C File Offset: 0x0004760C
		private PrefabPool<MiniMapDisplayEntry> MapEntryPool
		{
			get
			{
				if (this._mapEntryPool == null)
				{
					this._mapEntryPool = new PrefabPool<MiniMapDisplayEntry>(this.mapDisplayEntryPrefab, base.transform, new Action<MiniMapDisplayEntry>(this.OnGetMapEntry), null, null, true, 10, 10000, null);
				}
				return this._mapEntryPool;
			}
		}

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x060013D4 RID: 5076 RVA: 0x00049458 File Offset: 0x00047658
		private PrefabPool<PointOfInterestEntry> PointOfInterestEntryPool
		{
			get
			{
				if (this._pointOfInterestEntryPool == null)
				{
					this._pointOfInterestEntryPool = new PrefabPool<PointOfInterestEntry>(this.pointOfInterestEntryPrefab, base.transform, new Action<PointOfInterestEntry>(this.OnGetPointOfInterestEntry), null, null, true, 10, 10000, null);
				}
				return this._pointOfInterestEntryPool;
			}
		}

		// Token: 0x060013D5 RID: 5077 RVA: 0x000494A1 File Offset: 0x000476A1
		private void OnGetPointOfInterestEntry(PointOfInterestEntry entry)
		{
			entry.gameObject.hideFlags |= HideFlags.DontSave;
		}

		// Token: 0x060013D6 RID: 5078 RVA: 0x000494B7 File Offset: 0x000476B7
		private void OnGetMapEntry(MiniMapDisplayEntry entry)
		{
			entry.gameObject.hideFlags |= HideFlags.DontSave;
		}

		// Token: 0x060013D7 RID: 5079 RVA: 0x000494CD File Offset: 0x000476CD
		private void Awake()
		{
			if (this.master == null)
			{
				this.master = base.GetComponentInParent<MiniMapView>();
			}
			this.mapDisplayEntryPrefab.gameObject.SetActive(false);
			this.pointOfInterestEntryPrefab.gameObject.SetActive(false);
		}

		// Token: 0x060013D8 RID: 5080 RVA: 0x0004950B File Offset: 0x0004770B
		private void OnEnable()
		{
			if (this.autoSetupOnEnable)
			{
				this.AutoSetup();
			}
			this.RegisterEvents();
		}

		// Token: 0x060013D9 RID: 5081 RVA: 0x00049521 File Offset: 0x00047721
		private void OnDisable()
		{
			this.UnregisterEvents();
		}

		// Token: 0x060013DA RID: 5082 RVA: 0x00049529 File Offset: 0x00047729
		private void RegisterEvents()
		{
			PointsOfInterests.OnPointRegistered += this.HandlePointOfInterest;
			PointsOfInterests.OnPointUnregistered += this.ReleasePointOfInterest;
		}

		// Token: 0x060013DB RID: 5083 RVA: 0x0004954D File Offset: 0x0004774D
		private void UnregisterEvents()
		{
			PointsOfInterests.OnPointRegistered -= this.HandlePointOfInterest;
			PointsOfInterests.OnPointUnregistered -= this.ReleasePointOfInterest;
		}

		// Token: 0x060013DC RID: 5084 RVA: 0x00049574 File Offset: 0x00047774
		internal void AutoSetup()
		{
			MiniMapSettings miniMapSettings = global::UnityEngine.Object.FindAnyObjectByType<MiniMapSettings>();
			if (miniMapSettings)
			{
				this.Setup(miniMapSettings);
			}
		}

		// Token: 0x060013DD RID: 5085 RVA: 0x00049598 File Offset: 0x00047798
		public void Setup(IMiniMapDataProvider dataProvider)
		{
			if (dataProvider == null)
			{
				return;
			}
			this.MapEntryPool.ReleaseAll();
			bool flag = dataProvider.CombinedSprite != null;
			foreach (IMiniMapEntry miniMapEntry in dataProvider.Maps)
			{
				MiniMapDisplayEntry miniMapDisplayEntry = this.MapEntryPool.Get(null);
				miniMapDisplayEntry.Setup(this, miniMapEntry, !flag);
				miniMapDisplayEntry.gameObject.SetActive(true);
			}
			if (flag)
			{
				MiniMapDisplayEntry miniMapDisplayEntry2 = this.MapEntryPool.Get(null);
				miniMapDisplayEntry2.SetupCombined(this, dataProvider);
				miniMapDisplayEntry2.gameObject.SetActive(true);
				miniMapDisplayEntry2.transform.SetAsFirstSibling();
			}
			this.SetupRotation();
			this.FitContent();
			this.HandlePointsOfInterests();
			this.HandleTeleporters();
		}

		// Token: 0x060013DE RID: 5086 RVA: 0x00049668 File Offset: 0x00047868
		private void SetupRotation()
		{
			Vector3 vector = LevelManager.Instance.GameCamera.mainVCam.transform.up.ProjectOntoPlane(Vector3.up);
			float num = Vector3.SignedAngle(Vector3.forward, vector, Vector3.up);
			base.transform.localRotation = Quaternion.Euler(0f, 0f, num);
		}

		// Token: 0x060013DF RID: 5087 RVA: 0x000496C8 File Offset: 0x000478C8
		private void HandlePointsOfInterests()
		{
			this.PointOfInterestEntryPool.ReleaseAll();
			foreach (MonoBehaviour monoBehaviour in PointsOfInterests.Points)
			{
				if (!(monoBehaviour == null))
				{
					this.HandlePointOfInterest(monoBehaviour);
				}
			}
		}

		// Token: 0x060013E0 RID: 5088 RVA: 0x00049728 File Offset: 0x00047928
		private void HandlePointOfInterest(MonoBehaviour poi)
		{
			int targetSceneIndex = poi.gameObject.scene.buildIndex;
			IPointOfInterest pointOfInterest = poi as IPointOfInterest;
			if (pointOfInterest != null && pointOfInterest.OverrideScene >= 0)
			{
				targetSceneIndex = pointOfInterest.OverrideScene;
			}
			if (MultiSceneCore.ActiveSubScene == null || targetSceneIndex != MultiSceneCore.ActiveSubScene.Value.buildIndex)
			{
				return;
			}
			MiniMapDisplayEntry miniMapDisplayEntry = this.MapEntryPool.ActiveEntries.FirstOrDefault((MiniMapDisplayEntry e) => e.SceneReference != null && e.SceneReference.BuildIndex == targetSceneIndex);
			if (miniMapDisplayEntry == null)
			{
				return;
			}
			if (miniMapDisplayEntry.Hide)
			{
				return;
			}
			this.PointOfInterestEntryPool.Get(null).Setup(this, poi, miniMapDisplayEntry);
		}

		// Token: 0x060013E1 RID: 5089 RVA: 0x000497E8 File Offset: 0x000479E8
		private void ReleasePointOfInterest(MonoBehaviour poi)
		{
			PointOfInterestEntry pointOfInterestEntry = this.PointOfInterestEntryPool.ActiveEntries.FirstOrDefault((PointOfInterestEntry e) => e != null && e.Target == poi);
			if (!pointOfInterestEntry)
			{
				return;
			}
			this.PointOfInterestEntryPool.Release(pointOfInterestEntry);
		}

		// Token: 0x060013E2 RID: 5090 RVA: 0x00049834 File Offset: 0x00047A34
		private void HandleTeleporters()
		{
			this.teleporterSplines.gameObject.SetActive(false);
		}

		// Token: 0x060013E3 RID: 5091 RVA: 0x00049854 File Offset: 0x00047A54
		private void FitContent()
		{
			ReadOnlyCollection<MiniMapDisplayEntry> activeEntries = this.MapEntryPool.ActiveEntries;
			Vector2 vector = new Vector2(float.MinValue, float.MinValue);
			Vector2 vector2 = new Vector2(float.MaxValue, float.MaxValue);
			foreach (MiniMapDisplayEntry miniMapDisplayEntry in activeEntries)
			{
				RectTransform rectTransform = miniMapDisplayEntry.transform as RectTransform;
				Vector2 vector3 = rectTransform.anchoredPosition + rectTransform.rect.min;
				Vector2 vector4 = rectTransform.anchoredPosition + rectTransform.rect.max;
				vector.x = MathF.Max(vector4.x, vector.x);
				vector.y = MathF.Max(vector4.y, vector.y);
				vector2.x = MathF.Min(vector3.x, vector2.x);
				vector2.y = MathF.Min(vector3.y, vector2.y);
			}
			Vector2 vector5 = (vector + vector2) / 2f;
			foreach (MiniMapDisplayEntry miniMapDisplayEntry2 in activeEntries)
			{
				miniMapDisplayEntry2.transform.localPosition -= vector5;
			}
			(base.transform as RectTransform).sizeDelta = new Vector2(vector.x - vector2.x + this.padding * 2f, vector.y - vector2.y + this.padding * 2f);
		}

		// Token: 0x060013E4 RID: 5092 RVA: 0x00049A24 File Offset: 0x00047C24
		public bool TryConvertWorldToMinimap(Vector3 worldPosition, string sceneID, out Vector3 result)
		{
			result = worldPosition;
			MiniMapDisplayEntry miniMapDisplayEntry = this.MapEntryPool.ActiveEntries.FirstOrDefault((MiniMapDisplayEntry e) => e != null && e.SceneID == sceneID);
			if (miniMapDisplayEntry == null)
			{
				return false;
			}
			Vector3 center = MiniMapCenter.GetCenter(sceneID);
			Vector3 vector = worldPosition - center;
			Vector3 vector2 = new Vector3(vector.x, vector.z);
			Vector3 vector3 = miniMapDisplayEntry.transform.localToWorldMatrix.MultiplyPoint(vector2);
			result = base.transform.worldToLocalMatrix.MultiplyPoint(vector3);
			return true;
		}

		// Token: 0x060013E5 RID: 5093 RVA: 0x00049ACC File Offset: 0x00047CCC
		public bool TryConvertToWorldPosition(Vector3 displayPosition, out Vector3 result)
		{
			result = default(Vector3);
			string activeSubsceneID = MultiSceneCore.ActiveSubSceneID;
			MiniMapDisplayEntry miniMapDisplayEntry = this.MapEntryPool.ActiveEntries.FirstOrDefault((MiniMapDisplayEntry e) => e != null && e.SceneID == activeSubsceneID);
			if (miniMapDisplayEntry == null)
			{
				return false;
			}
			Vector3 vector = miniMapDisplayEntry.transform.worldToLocalMatrix.MultiplyPoint(displayPosition);
			Vector3 vector2 = new Vector3(vector.x, 0f, vector.y);
			Vector3 center = MiniMapCenter.GetCenter(activeSubsceneID);
			result = center + vector2;
			return true;
		}

		// Token: 0x060013E6 RID: 5094 RVA: 0x00049B64 File Offset: 0x00047D64
		internal void Center(Vector3 minimapPos)
		{
			RectTransform rectTransform = base.transform as RectTransform;
			if (rectTransform == null)
			{
				return;
			}
			Vector3 vector = rectTransform.localToWorldMatrix.MultiplyPoint(minimapPos);
			Vector3 vector2 = (rectTransform.parent as RectTransform).position - vector;
			rectTransform.position += vector2;
		}

		// Token: 0x060013E7 RID: 5095 RVA: 0x00049BC0 File Offset: 0x00047DC0
		public void OnScroll(PointerEventData eventData)
		{
			this.master.OnScroll(eventData);
		}

		// Token: 0x060013E9 RID: 5097 RVA: 0x00049BE1 File Offset: 0x00047DE1
		[CompilerGenerated]
		internal static void <HandleTeleporters>g__ClearSplines|26_0(SplineContainer splineContainer)
		{
			while (splineContainer.Splines.Count > 0)
			{
				splineContainer.RemoveSplineAt(0);
			}
		}

		// Token: 0x04000E9A RID: 3738
		[SerializeField]
		private MiniMapView master;

		// Token: 0x04000E9B RID: 3739
		[SerializeField]
		private MiniMapDisplayEntry mapDisplayEntryPrefab;

		// Token: 0x04000E9C RID: 3740
		[SerializeField]
		private PointOfInterestEntry pointOfInterestEntryPrefab;

		// Token: 0x04000E9D RID: 3741
		[SerializeField]
		private UISplineRenderer teleporterSplines;

		// Token: 0x04000E9E RID: 3742
		[SerializeField]
		private bool autoSetupOnEnable;

		// Token: 0x04000E9F RID: 3743
		[SerializeField]
		private float padding = 25f;

		// Token: 0x04000EA0 RID: 3744
		private PrefabPool<MiniMapDisplayEntry> _mapEntryPool;

		// Token: 0x04000EA1 RID: 3745
		private PrefabPool<PointOfInterestEntry> _pointOfInterestEntryPool;
	}
}
