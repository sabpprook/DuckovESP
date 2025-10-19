using System;
using Cinemachine;
using Cinemachine.Utility;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using Duckov.UI.Animations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Duckov.Buildings.UI
{
	// Token: 0x02000316 RID: 790
	public class BuilderView : View, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x170004D2 RID: 1234
		// (get) Token: 0x06001A1F RID: 6687 RVA: 0x0005E5A9 File Offset: 0x0005C7A9
		public static BuilderView Instance
		{
			get
			{
				return View.GetViewInstance<BuilderView>();
			}
		}

		// Token: 0x06001A20 RID: 6688 RVA: 0x0005E5B0 File Offset: 0x0005C7B0
		public void SetupAndShow(BuildingArea targetArea)
		{
			this.targetArea = targetArea;
			base.Open(null);
		}

		// Token: 0x06001A21 RID: 6689 RVA: 0x0005E5C0 File Offset: 0x0005C7C0
		protected override void Awake()
		{
			base.Awake();
			this.input_Rotate.action.actionMap.Enable();
			this.input_MoveCamera.action.actionMap.Enable();
			this.selectionPanel.onButtonSelected += this.OnButtonSelected;
			this.selectionPanel.onRecycleRequested += this.OnRecycleRequested;
			BuildingManager.OnBuildingListChanged += this.OnBuildingListChanged;
		}

		// Token: 0x06001A22 RID: 6690 RVA: 0x0005E63C File Offset: 0x0005C83C
		private void OnRecycleRequested(BuildingBtnEntry entry)
		{
			BuildingManager.ReturnBuildingsOfType(entry.Info.id, null).Forget<int>();
		}

		// Token: 0x06001A23 RID: 6691 RVA: 0x0005E654 File Offset: 0x0005C854
		protected override void OnDestroy()
		{
			base.OnDestroy();
			BuildingManager.OnBuildingListChanged -= this.OnBuildingListChanged;
		}

		// Token: 0x06001A24 RID: 6692 RVA: 0x0005E66D File Offset: 0x0005C86D
		private void OnBuildingListChanged()
		{
			this.selectionPanel.Refresh();
		}

		// Token: 0x06001A25 RID: 6693 RVA: 0x0005E67C File Offset: 0x0005C87C
		private void OnButtonSelected(BuildingBtnEntry entry)
		{
			if (!entry.CostEnough)
			{
				this.NotifyCostNotEnough(entry);
				return;
			}
			if (entry.Info.ReachedAmountLimit)
			{
				return;
			}
			this.BeginPlacing(entry.Info);
		}

		// Token: 0x06001A26 RID: 6694 RVA: 0x0005E6B8 File Offset: 0x0005C8B8
		private void NotifyCostNotEnough(BuildingBtnEntry entry)
		{
			Debug.Log("Resource not enough " + entry.Info.DisplayName);
		}

		// Token: 0x06001A27 RID: 6695 RVA: 0x0005E6E2 File Offset: 0x0005C8E2
		private void SetMode(BuilderView.Mode mode)
		{
			this.placingModeInputIndicator.SetActive(false);
			this.OnExitMode(this.mode);
			this.mode = mode;
			switch (mode)
			{
			case BuilderView.Mode.None:
			case BuilderView.Mode.Destroying:
				break;
			case BuilderView.Mode.Placing:
				this.placingModeInputIndicator.SetActive(true);
				break;
			default:
				return;
			}
		}

		// Token: 0x06001A28 RID: 6696 RVA: 0x0005E722 File Offset: 0x0005C922
		private void OnExitMode(BuilderView.Mode mode)
		{
			this.contextMenu.Hide();
			switch (mode)
			{
			case BuilderView.Mode.None:
			case BuilderView.Mode.Destroying:
				break;
			case BuilderView.Mode.Placing:
				this.OnExitPlacing();
				break;
			default:
				return;
			}
		}

		// Token: 0x06001A29 RID: 6697 RVA: 0x0005E748 File Offset: 0x0005C948
		public void BeginPlacing(BuildingInfo info)
		{
			if (this.previewBuilding != null)
			{
				global::UnityEngine.Object.Destroy(this.previewBuilding.gameObject);
			}
			this.placingBuildingInfo = info;
			this.SetMode(BuilderView.Mode.Placing);
			if (info.Prefab == null)
			{
				Debug.LogError("建筑 " + info.DisplayName + " 没有prefab");
			}
			this.previewBuilding = global::UnityEngine.Object.Instantiate<Building>(info.Prefab);
			if (this.previewBuilding.ID != info.id)
			{
				Debug.LogError("建筑 " + info.DisplayName + " 的 prefab 上的 ID 设置错误");
			}
			this.SetupPreview(this.previewBuilding);
			this.UpdatePlacing();
		}

		// Token: 0x06001A2A RID: 6698 RVA: 0x0005E802 File Offset: 0x0005CA02
		public void BeginDestroying()
		{
			this.SetMode(BuilderView.Mode.Destroying);
		}

		// Token: 0x06001A2B RID: 6699 RVA: 0x0005E80B File Offset: 0x0005CA0B
		private void SetupPreview(Building previewBuilding)
		{
			if (previewBuilding == null)
			{
				return;
			}
			previewBuilding.SetupPreview();
		}

		// Token: 0x06001A2C RID: 6700 RVA: 0x0005E81D File Offset: 0x0005CA1D
		private void OnExitPlacing()
		{
			if (this.previewBuilding != null)
			{
				global::UnityEngine.Object.Destroy(this.previewBuilding.gameObject);
			}
			GridDisplay.HidePreview();
		}

		// Token: 0x06001A2D RID: 6701 RVA: 0x0005E844 File Offset: 0x0005CA44
		private void Update()
		{
			switch (this.mode)
			{
			case BuilderView.Mode.None:
				this.UpdateNone();
				break;
			case BuilderView.Mode.Placing:
				this.UpdatePlacing();
				break;
			case BuilderView.Mode.Destroying:
				this.UpdateDestroying();
				break;
			}
			this.UpdateCamera();
			this.UpdateContextMenuIndicator();
		}

		// Token: 0x06001A2E RID: 6702 RVA: 0x0005E890 File Offset: 0x0005CA90
		private unsafe void UpdateContextMenuIndicator()
		{
			Vector2Int vector2Int;
			this.TryGetPointingCoord(out vector2Int, null);
			bool flag = this.targetArea.GetBuildingInstanceAt(vector2Int);
			bool isActiveAndEnabled = this.contextMenu.isActiveAndEnabled;
			Vector2 vector;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.followCursorUI.parent as RectTransform, *Mouse.current.position.value, null, out vector);
			this.followCursorUI.localPosition = vector;
			bool flag2 = flag && !isActiveAndEnabled;
			if (flag2 && !this.hoveringBuildingFadeGroup.IsShown)
			{
				this.hoveringBuildingFadeGroup.Show();
			}
			if (!flag2 && this.hoveringBuildingFadeGroup.IsShown)
			{
				this.hoveringBuildingFadeGroup.Hide();
			}
		}

		// Token: 0x06001A2F RID: 6703 RVA: 0x0005E940 File Offset: 0x0005CB40
		private void UpdateNone()
		{
			if (this.input_RequestContextMenu.action.WasPressedThisFrame())
			{
				Vector2Int vector2Int;
				if (!this.TryGetPointingCoord(out vector2Int, null))
				{
					return;
				}
				Building buildingInstanceAt = this.targetArea.GetBuildingInstanceAt(vector2Int);
				if (buildingInstanceAt == null)
				{
					this.contextMenu.Hide();
					return;
				}
				this.contextMenu.Setup(buildingInstanceAt);
			}
		}

		// Token: 0x06001A30 RID: 6704 RVA: 0x0005E99C File Offset: 0x0005CB9C
		private void UpdateDestroying()
		{
			Vector2Int vector2Int;
			if (!this.TryGetPointingCoord(out vector2Int, null))
			{
				GridDisplay.HidePreview();
				return;
			}
			BuildingManager.BuildingData buildingAt = this.targetArea.AreaData.GetBuildingAt(vector2Int);
			if (buildingAt == null)
			{
				GridDisplay.HidePreview();
				return;
			}
			this.gridDisplay.SetBuildingPreviewCoord(buildingAt.Coord, buildingAt.Dimensions, buildingAt.Rotation, false);
		}

		// Token: 0x06001A31 RID: 6705 RVA: 0x0005E9F4 File Offset: 0x0005CBF4
		private void ConfirmDestroy()
		{
			Vector2Int vector2Int;
			if (!this.TryGetPointingCoord(out vector2Int, null))
			{
				return;
			}
			BuildingManager.BuildingData buildingAt = this.targetArea.AreaData.GetBuildingAt(vector2Int);
			if (buildingAt == null)
			{
				return;
			}
			BuildingManager.ReturnBuilding(buildingAt.GUID, null).Forget<bool>();
			this.SetMode(BuilderView.Mode.None);
		}

		// Token: 0x06001A32 RID: 6706 RVA: 0x0005EA3C File Offset: 0x0005CC3C
		private void ConfirmPlacement()
		{
			if (this.previewBuilding == null)
			{
				Debug.Log("No Previewing Building");
				return;
			}
			Vector2Int vector2Int;
			if (!this.TryGetPointingCoord(out vector2Int, this.previewBuilding))
			{
				this.previewBuilding.gameObject.SetActive(false);
				Debug.Log("Mouse Not in Plane!");
				return;
			}
			if (!this.IsValidPlacement(this.previewBuilding.Dimensions, this.previewRotation, vector2Int))
			{
				Debug.Log("Invalid Placement!");
				return;
			}
			BuildingManager.BuyAndPlace(this.targetArea.AreaID, this.previewBuilding.ID, vector2Int, this.previewRotation);
			this.SetMode(BuilderView.Mode.None);
		}

		// Token: 0x06001A33 RID: 6707 RVA: 0x0005EAE0 File Offset: 0x0005CCE0
		private void UpdatePlacing()
		{
			if (this.previewBuilding)
			{
				Vector2Int vector2Int;
				if (!this.TryGetPointingCoord(out vector2Int, this.previewBuilding))
				{
					this.previewBuilding.gameObject.SetActive(false);
					return;
				}
				bool flag = this.IsValidPlacement(this.previewBuilding.Dimensions, this.previewRotation, vector2Int);
				this.gridDisplay.SetBuildingPreviewCoord(vector2Int, this.previewBuilding.Dimensions, this.previewRotation, flag);
				this.ShowPreview(vector2Int);
				if (this.input_Rotate.action.WasPressedThisFrame())
				{
					float num = this.input_Rotate.action.ReadValue<float>();
					this.previewRotation = (BuildingRotation)(((float)this.previewRotation + num + 4f) % 4f);
				}
				if (this.input_RequestContextMenu.action.WasPressedThisFrame())
				{
					this.SetMode(BuilderView.Mode.None);
					return;
				}
			}
			else
			{
				this.SetMode(BuilderView.Mode.None);
			}
		}

		// Token: 0x06001A34 RID: 6708 RVA: 0x0005EBC0 File Offset: 0x0005CDC0
		private void ShowPreview(Vector2Int coord)
		{
			Vector3 vector = this.targetArea.CoordToWorldPosition(coord, this.previewBuilding.Dimensions, this.previewRotation);
			this.previewBuilding.transform.position = vector;
			this.previewBuilding.gameObject.SetActive(true);
			Quaternion quaternion = Quaternion.Euler(new Vector3(0f, (float)((BuildingRotation)90 * this.previewRotation), 0f));
			this.previewBuilding.transform.rotation = this.targetArea.transform.rotation * quaternion;
		}

		// Token: 0x06001A35 RID: 6709 RVA: 0x0005EC54 File Offset: 0x0005CE54
		public bool TryGetPointingCoord(out Vector2Int coord, Building previewBuilding = null)
		{
			coord = default(Vector2Int);
			Ray pointRay = UIInputManager.GetPointRay();
			float num;
			if (!this.targetArea.Plane.Raycast(pointRay, out num))
			{
				return false;
			}
			Vector3 point = pointRay.GetPoint(num);
			if (previewBuilding != null)
			{
				coord = this.targetArea.CursorToCoord(point, previewBuilding.Dimensions, this.previewRotation);
				return true;
			}
			coord = this.targetArea.CursorToCoord(point, Vector2Int.one, BuildingRotation.Zero);
			return true;
		}

		// Token: 0x06001A36 RID: 6710 RVA: 0x0005ECD8 File Offset: 0x0005CED8
		private bool IsValidPlacement(Vector2Int dimensions, BuildingRotation rotation, Vector2Int coord)
		{
			return this.targetArea.IsPlacementWithinRange(dimensions, rotation, coord) && !this.targetArea.AreaData.Collide(dimensions, rotation, coord) && !this.targetArea.PhysicsCollide(dimensions, rotation, coord, 0f, 2f);
		}

		// Token: 0x06001A37 RID: 6711 RVA: 0x0005ED2C File Offset: 0x0005CF2C
		protected override void OnOpen()
		{
			base.OnOpen();
			this.SetMode(BuilderView.Mode.None);
			this.fadeGroup.Show();
			this.selectionPanel.Setup(this.targetArea);
			this.gridDisplay.Setup(this.targetArea);
			this.cameraCursor = this.targetArea.transform.position;
			this.UpdateCamera();
		}

		// Token: 0x06001A38 RID: 6712 RVA: 0x0005ED8F File Offset: 0x0005CF8F
		protected override void OnClose()
		{
			base.OnClose();
			this.fadeGroup.Hide();
			GridDisplay.Close();
			if (this.previewBuilding != null)
			{
				global::UnityEngine.Object.Destroy(this.previewBuilding.gameObject);
			}
		}

		// Token: 0x06001A39 RID: 6713 RVA: 0x0005EDC8 File Offset: 0x0005CFC8
		private void UpdateCamera()
		{
			if (this.input_MoveCamera.action.IsPressed())
			{
				Vector2 vector = this.input_MoveCamera.action.ReadValue<Vector2>();
				Transform transform = this.vcam.transform;
				float num = Mathf.Abs(Vector3.Dot(transform.forward, Vector3.up));
				float num2 = Mathf.Abs(Vector3.Dot(transform.up, Vector3.up));
				Vector3 vector2 = ((num > num2) ? transform.up : transform.forward).ProjectOntoPlane(Vector3.up);
				Vector3 vector3 = transform.right.ProjectOntoPlane(Vector3.up);
				this.cameraCursor += (vector3 * vector.x + vector2 * vector.y) * this.cameraSpeed * Time.unscaledDeltaTime;
				this.cameraCursor.x = Mathf.Clamp(this.cameraCursor.x, this.targetArea.transform.position.x - (float)this.targetArea.Size.x, this.targetArea.transform.position.x + (float)this.targetArea.Size.x);
				this.cameraCursor.z = Mathf.Clamp(this.cameraCursor.z, this.targetArea.transform.position.z - (float)this.targetArea.Size.y, this.targetArea.transform.position.z + (float)this.targetArea.Size.y);
			}
			this.vcam.transform.position = this.cameraCursor + Quaternion.Euler(0f, this.yaw, 0f) * Quaternion.Euler(this.pitch, 0f, 0f) * Vector3.forward * this.cameraDistance;
			this.vcam.transform.LookAt(this.cameraCursor, Vector3.up);
		}

		// Token: 0x06001A3A RID: 6714 RVA: 0x0005F000 File Offset: 0x0005D200
		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				this.contextMenu.Hide();
				BuilderView.Mode mode = this.mode;
				if (mode == BuilderView.Mode.Placing)
				{
					this.ConfirmPlacement();
					return;
				}
				if (mode != BuilderView.Mode.Destroying)
				{
					return;
				}
				this.ConfirmDestroy();
			}
		}

		// Token: 0x06001A3B RID: 6715 RVA: 0x0005F03D File Offset: 0x0005D23D
		public static void Show(BuildingArea target)
		{
			BuilderView.Instance.SetupAndShow(target);
		}

		// Token: 0x040012C2 RID: 4802
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x040012C3 RID: 4803
		[SerializeField]
		private BuildingSelectionPanel selectionPanel;

		// Token: 0x040012C4 RID: 4804
		[SerializeField]
		private BuildingContextMenu contextMenu;

		// Token: 0x040012C5 RID: 4805
		[SerializeField]
		private GameObject placingModeInputIndicator;

		// Token: 0x040012C6 RID: 4806
		[SerializeField]
		private RectTransform followCursorUI;

		// Token: 0x040012C7 RID: 4807
		[SerializeField]
		private FadeGroup hoveringBuildingFadeGroup;

		// Token: 0x040012C8 RID: 4808
		[SerializeField]
		private CinemachineVirtualCamera vcam;

		// Token: 0x040012C9 RID: 4809
		[SerializeField]
		private float cameraSpeed = 10f;

		// Token: 0x040012CA RID: 4810
		[SerializeField]
		private float pitch = 45f;

		// Token: 0x040012CB RID: 4811
		[SerializeField]
		private float cameraDistance = 10f;

		// Token: 0x040012CC RID: 4812
		[SerializeField]
		private float yaw = -45f;

		// Token: 0x040012CD RID: 4813
		[SerializeField]
		private Vector3 cameraCursor;

		// Token: 0x040012CE RID: 4814
		[SerializeField]
		private BuildingInfo placingBuildingInfo;

		// Token: 0x040012CF RID: 4815
		[SerializeField]
		private InputActionReference input_Rotate;

		// Token: 0x040012D0 RID: 4816
		[SerializeField]
		private InputActionReference input_RequestContextMenu;

		// Token: 0x040012D1 RID: 4817
		[SerializeField]
		private InputActionReference input_MoveCamera;

		// Token: 0x040012D2 RID: 4818
		[SerializeField]
		private GridDisplay gridDisplay;

		// Token: 0x040012D3 RID: 4819
		[SerializeField]
		private BuildingArea targetArea;

		// Token: 0x040012D4 RID: 4820
		[SerializeField]
		private BuilderView.Mode mode;

		// Token: 0x040012D5 RID: 4821
		private Building previewBuilding;

		// Token: 0x040012D6 RID: 4822
		[SerializeField]
		private BuildingRotation previewRotation;

		// Token: 0x020005B5 RID: 1461
		private enum Mode
		{
			// Token: 0x0400204C RID: 8268
			None,
			// Token: 0x0400204D RID: 8269
			Placing,
			// Token: 0x0400204E RID: 8270
			Destroying
		}
	}
}
