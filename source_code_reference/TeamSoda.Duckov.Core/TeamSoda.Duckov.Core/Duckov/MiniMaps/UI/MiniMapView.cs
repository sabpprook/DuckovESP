using System;
using Duckov.Scenes;
using Duckov.UI;
using Duckov.UI.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Duckov.MiniMaps.UI
{
	// Token: 0x02000277 RID: 631
	public class MiniMapView : View
	{
		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x060013FA RID: 5114 RVA: 0x00049F21 File Offset: 0x00048121
		public static MiniMapView Instance
		{
			get
			{
				return View.GetViewInstance<MiniMapView>();
			}
		}

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x060013FC RID: 5116 RVA: 0x00049F40 File Offset: 0x00048140
		// (set) Token: 0x060013FB RID: 5115 RVA: 0x00049F28 File Offset: 0x00048128
		private float Zoom
		{
			get
			{
				return this._zoom;
			}
			set
			{
				value = Mathf.Clamp01(value);
				this._zoom = value;
				this.OnSetZoom(value);
			}
		}

		// Token: 0x060013FD RID: 5117 RVA: 0x00049F48 File Offset: 0x00048148
		private void OnSetZoom(float scale)
		{
			this.RefreshZoom();
		}

		// Token: 0x060013FE RID: 5118 RVA: 0x00049F50 File Offset: 0x00048150
		private void RefreshZoom()
		{
			if (this.display == null)
			{
				return;
			}
			RectTransform rectTransform = base.transform as RectTransform;
			Transform transform = this.display.transform;
			Vector3 vector = rectTransform.localToWorldMatrix.MultiplyPoint(rectTransform.rect.center);
			Vector3 vector2 = transform.worldToLocalMatrix.MultiplyPoint(vector);
			this.display.transform.localScale = Vector3.one * Mathf.Lerp(this.zoomMin, this.zoomMax, this.zoomCurve.Evaluate(this.Zoom));
			Vector3 vector3 = transform.localToWorldMatrix.MultiplyPoint(vector2) - vector;
			this.display.transform.position -= vector3;
			this.zoomSlider.SetValueWithoutNotify(this.Zoom);
		}

		// Token: 0x060013FF RID: 5119 RVA: 0x0004A038 File Offset: 0x00048238
		protected override void OnOpen()
		{
			base.OnOpen();
			this.fadeGroup.Show();
			this.display.AutoSetup();
			MultiSceneCore instance = MultiSceneCore.Instance;
			SceneInfoEntry sceneInfoEntry = ((instance != null) ? instance.SceneInfo : null);
			if (sceneInfoEntry != null)
			{
				this.mapNameText.text = sceneInfoEntry.DisplayName;
				this.mapInfoText.text = sceneInfoEntry.Description;
			}
			else
			{
				this.mapNameText.text = "";
				this.mapInfoText.text = "";
			}
			this.zoomSlider.SetValueWithoutNotify(this.Zoom);
			this.RefreshZoom();
			this.CeneterPlayer();
		}

		// Token: 0x06001400 RID: 5120 RVA: 0x0004A0D7 File Offset: 0x000482D7
		protected override void OnClose()
		{
			base.OnClose();
			this.fadeGroup.Hide();
		}

		// Token: 0x06001401 RID: 5121 RVA: 0x0004A0EA File Offset: 0x000482EA
		protected override void Awake()
		{
			base.Awake();
			this.zoomSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnZoomSliderValueChanged));
		}

		// Token: 0x06001402 RID: 5122 RVA: 0x0004A10E File Offset: 0x0004830E
		private void FixedUpdate()
		{
			this.RefreshNoSignalIndicator();
		}

		// Token: 0x06001403 RID: 5123 RVA: 0x0004A116 File Offset: 0x00048316
		private void RefreshNoSignalIndicator()
		{
			this.noSignalIndicator.SetActive(this.display.NoSignal());
		}

		// Token: 0x06001404 RID: 5124 RVA: 0x0004A12E File Offset: 0x0004832E
		private void OnZoomSliderValueChanged(float value)
		{
			this.Zoom = value;
		}

		// Token: 0x06001405 RID: 5125 RVA: 0x0004A137 File Offset: 0x00048337
		public static void Show()
		{
			if (MiniMapView.Instance == null)
			{
				return;
			}
			if (MiniMapSettings.Instance == null)
			{
				return;
			}
			MiniMapView.Instance.Open(null);
		}

		// Token: 0x06001406 RID: 5126 RVA: 0x0004A160 File Offset: 0x00048360
		public void CeneterPlayer()
		{
			CharacterMainControl main = CharacterMainControl.Main;
			if (main == null)
			{
				return;
			}
			Vector3 vector;
			if (!this.display.TryConvertWorldToMinimap(main.transform.position, SceneInfoCollection.GetSceneID(SceneManager.GetActiveScene().buildIndex), out vector))
			{
				return;
			}
			this.display.Center(vector);
		}

		// Token: 0x06001407 RID: 5127 RVA: 0x0004A1B6 File Offset: 0x000483B6
		public static bool TryConvertWorldToMinimapPosition(Vector3 worldPosition, string sceneID, out Vector3 result)
		{
			result = default(Vector3);
			return !(MiniMapView.Instance == null) && MiniMapView.Instance.display.TryConvertWorldToMinimap(worldPosition, sceneID, out result);
		}

		// Token: 0x06001408 RID: 5128 RVA: 0x0004A1E0 File Offset: 0x000483E0
		public static bool TryConvertWorldToMinimapPosition(Vector3 worldPosition, out Vector3 result)
		{
			result = default(Vector3);
			if (MiniMapView.Instance == null)
			{
				return false;
			}
			string sceneID = SceneInfoCollection.GetSceneID(SceneManager.GetActiveScene().buildIndex);
			return MiniMapView.TryConvertWorldToMinimapPosition(worldPosition, sceneID, out result);
		}

		// Token: 0x06001409 RID: 5129 RVA: 0x0004A21E File Offset: 0x0004841E
		internal void OnScroll(PointerEventData eventData)
		{
			this.Zoom += eventData.scrollDelta.y * this.scrollSensitivity;
			eventData.Use();
		}

		// Token: 0x0600140A RID: 5130 RVA: 0x0004A245 File Offset: 0x00048445
		internal static void RequestMarkPOI(Vector3 worldPos)
		{
			MapMarkerManager.Request(worldPos);
		}

		// Token: 0x0600140B RID: 5131 RVA: 0x0004A24D File Offset: 0x0004844D
		public void LoadData(PackedMapData mapData)
		{
			if (mapData == null)
			{
				return;
			}
			this.display.Setup(mapData);
		}

		// Token: 0x0600140C RID: 5132 RVA: 0x0004A265 File Offset: 0x00048465
		public void LoadCurrent()
		{
			this.display.AutoSetup();
		}

		// Token: 0x04000EA9 RID: 3753
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x04000EAA RID: 3754
		[SerializeField]
		private MiniMapDisplay display;

		// Token: 0x04000EAB RID: 3755
		[SerializeField]
		private TextMeshProUGUI mapNameText;

		// Token: 0x04000EAC RID: 3756
		[SerializeField]
		private TextMeshProUGUI mapInfoText;

		// Token: 0x04000EAD RID: 3757
		[SerializeField]
		private Slider zoomSlider;

		// Token: 0x04000EAE RID: 3758
		[SerializeField]
		private float zoomMin = 5f;

		// Token: 0x04000EAF RID: 3759
		[SerializeField]
		private float zoomMax = 20f;

		// Token: 0x04000EB0 RID: 3760
		[SerializeField]
		[HideInInspector]
		private float _zoom = 5f;

		// Token: 0x04000EB1 RID: 3761
		[SerializeField]
		[Range(0f, 0.01f)]
		private float scrollSensitivity = 0.01f;

		// Token: 0x04000EB2 RID: 3762
		[SerializeField]
		private SimplePointOfInterest markPoiTemplate;

		// Token: 0x04000EB3 RID: 3763
		[SerializeField]
		private AnimationCurve zoomCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04000EB4 RID: 3764
		[SerializeField]
		private GameObject noSignalIndicator;
	}
}
