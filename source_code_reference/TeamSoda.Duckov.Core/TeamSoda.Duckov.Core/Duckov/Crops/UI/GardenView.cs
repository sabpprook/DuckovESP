using System;
using System.Runtime.CompilerServices;
using Duckov.Economy;
using Duckov.UI;
using Duckov.UI.Animations;
using ItemStatsSystem;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.Crops.UI
{
	// Token: 0x020002ED RID: 749
	public class GardenView : View, IPointerClickHandler, IEventSystemHandler, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, ICursorDataProvider
	{
		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x0600180D RID: 6157 RVA: 0x00058151 File Offset: 0x00056351
		// (set) Token: 0x0600180E RID: 6158 RVA: 0x00058158 File Offset: 0x00056358
		public static GardenView Instance { get; private set; }

		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x0600180F RID: 6159 RVA: 0x00058160 File Offset: 0x00056360
		// (set) Token: 0x06001810 RID: 6160 RVA: 0x00058168 File Offset: 0x00056368
		public Garden Target { get; private set; }

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x06001811 RID: 6161 RVA: 0x00058171 File Offset: 0x00056371
		// (set) Token: 0x06001812 RID: 6162 RVA: 0x00058179 File Offset: 0x00056379
		public bool SeedSelected { get; private set; }

		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06001813 RID: 6163 RVA: 0x00058182 File Offset: 0x00056382
		// (set) Token: 0x06001814 RID: 6164 RVA: 0x0005818A File Offset: 0x0005638A
		public int PlantingSeedTypeID
		{
			get
			{
				return this._plantingSeedTypeID;
			}
			private set
			{
				this._plantingSeedTypeID = value;
				this.SeedMeta = ItemAssetsCollection.GetMetaData(value);
			}
		}

		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x06001815 RID: 6165 RVA: 0x0005819F File Offset: 0x0005639F
		// (set) Token: 0x06001816 RID: 6166 RVA: 0x000581A7 File Offset: 0x000563A7
		public ItemMetaData SeedMeta { get; private set; }

		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x06001817 RID: 6167 RVA: 0x000581B0 File Offset: 0x000563B0
		// (set) Token: 0x06001818 RID: 6168 RVA: 0x000581B8 File Offset: 0x000563B8
		public GardenView.ToolType Tool { get; private set; }

		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x06001819 RID: 6169 RVA: 0x000581C1 File Offset: 0x000563C1
		// (set) Token: 0x0600181A RID: 6170 RVA: 0x000581C9 File Offset: 0x000563C9
		public bool Hovering { get; private set; }

		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x0600181B RID: 6171 RVA: 0x000581D2 File Offset: 0x000563D2
		// (set) Token: 0x0600181C RID: 6172 RVA: 0x000581DA File Offset: 0x000563DA
		public Vector2Int HoveringCoord { get; private set; }

		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x0600181D RID: 6173 RVA: 0x000581E3 File Offset: 0x000563E3
		// (set) Token: 0x0600181E RID: 6174 RVA: 0x000581EB File Offset: 0x000563EB
		public Crop HoveringCrop { get; private set; }

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x0600181F RID: 6175 RVA: 0x000581F4 File Offset: 0x000563F4
		public string ToolDisplayName
		{
			get
			{
				switch (this.Tool)
				{
				case GardenView.ToolType.None:
					return "...";
				case GardenView.ToolType.Plant:
					return this.textKey_Plant.ToPlainText();
				case GardenView.ToolType.Harvest:
					return this.textKey_Harvest.ToPlainText();
				case GardenView.ToolType.Water:
					return this.textKey_Water.ToPlainText();
				case GardenView.ToolType.Destroy:
					return this.textKey_Destroy.ToPlainText();
				default:
					return "?";
				}
			}
		}

		// Token: 0x1400009C RID: 156
		// (add) Token: 0x06001820 RID: 6176 RVA: 0x00058260 File Offset: 0x00056460
		// (remove) Token: 0x06001821 RID: 6177 RVA: 0x00058298 File Offset: 0x00056498
		public event Action onContextChanged;

		// Token: 0x1400009D RID: 157
		// (add) Token: 0x06001822 RID: 6178 RVA: 0x000582D0 File Offset: 0x000564D0
		// (remove) Token: 0x06001823 RID: 6179 RVA: 0x00058308 File Offset: 0x00056508
		public event Action onToolChanged;

		// Token: 0x06001824 RID: 6180 RVA: 0x0005833D File Offset: 0x0005653D
		protected override void Awake()
		{
			base.Awake();
			this.btn_ChangePlant.onClick.AddListener(new UnityAction(this.OnBtnChangePlantClicked));
			ItemUtilities.OnPlayerItemOperation += this.OnPlayerItemOperation;
			GardenView.Instance = this;
		}

		// Token: 0x06001825 RID: 6181 RVA: 0x00058378 File Offset: 0x00056578
		protected override void OnDestroy()
		{
			base.OnDestroy();
			ItemUtilities.OnPlayerItemOperation -= this.OnPlayerItemOperation;
		}

		// Token: 0x06001826 RID: 6182 RVA: 0x00058391 File Offset: 0x00056591
		private void OnDisable()
		{
			if (this.cellHoveringGizmos)
			{
				this.cellHoveringGizmos.gameObject.SetActive(false);
			}
		}

		// Token: 0x06001827 RID: 6183 RVA: 0x000583B1 File Offset: 0x000565B1
		private void OnPlayerItemOperation()
		{
			if (base.gameObject.activeSelf && this.SeedSelected)
			{
				this.RefreshSeedAmount();
			}
		}

		// Token: 0x06001828 RID: 6184 RVA: 0x000583CE File Offset: 0x000565CE
		public static void Show(Garden target)
		{
			GardenView.Instance.Target = target;
			GardenView.Instance.Open(null);
		}

		// Token: 0x06001829 RID: 6185 RVA: 0x000583E8 File Offset: 0x000565E8
		protected override void OnOpen()
		{
			base.OnOpen();
			if (this.Target == null)
			{
				this.Target = global::UnityEngine.Object.FindObjectOfType<Garden>();
			}
			if (this.Target == null)
			{
				Debug.Log("No Garden instance found. Aborting..");
				base.Close();
			}
			this.fadeGroup.Show();
			this.RefreshSeedInfoDisplay();
			this.EnableCursor();
			this.SetTool(this.Tool);
			this.CenterCamera();
		}

		// Token: 0x0600182A RID: 6186 RVA: 0x0005845B File Offset: 0x0005665B
		protected override void OnClose()
		{
			base.OnClose();
			this.cropSelector.Hide();
			this.fadeGroup.Hide();
			this.ReleaseCursor();
		}

		// Token: 0x0600182B RID: 6187 RVA: 0x0005847F File Offset: 0x0005667F
		private void EnableCursor()
		{
			CursorManager.Register(this);
		}

		// Token: 0x0600182C RID: 6188 RVA: 0x00058487 File Offset: 0x00056687
		private void ReleaseCursor()
		{
			CursorManager.Unregister(this);
		}

		// Token: 0x0600182D RID: 6189 RVA: 0x00058490 File Offset: 0x00056690
		private void ChangeCursor()
		{
			CursorManager.NotifyRefresh();
		}

		// Token: 0x0600182E RID: 6190 RVA: 0x00058497 File Offset: 0x00056697
		private void Update()
		{
			this.UpdateContext();
			this.UpdateCursor3D();
		}

		// Token: 0x0600182F RID: 6191 RVA: 0x000584A5 File Offset: 0x000566A5
		private void OnBtnChangePlantClicked()
		{
			this.cropSelector.Show();
		}

		// Token: 0x06001830 RID: 6192 RVA: 0x000584B4 File Offset: 0x000566B4
		private void OnContextChanged()
		{
			Action action = this.onContextChanged;
			if (action != null)
			{
				action();
			}
			this.RefreshHoveringGizmos();
			this.RefreshCursor();
			if (this.dragging && this.Hovering)
			{
				this.ExecuteTool(this.HoveringCoord);
			}
			this.ChangeCursor();
			this.RefreshCursor3DActive();
		}

		// Token: 0x06001831 RID: 6193 RVA: 0x00058508 File Offset: 0x00056708
		private void RefreshCursor()
		{
			this.cursorIcon.gameObject.SetActive(false);
			this.cursorAmountDisplay.gameObject.SetActive(false);
			this.cursorItemDisplay.gameObject.SetActive(false);
			switch (this.Tool)
			{
			case GardenView.ToolType.None:
				break;
			case GardenView.ToolType.Plant:
				this.cursorAmountDisplay.gameObject.SetActive(this.SeedSelected);
				this.cursorItemDisplay.gameObject.SetActive(this.SeedSelected);
				this.cursorIcon.sprite = this.iconPlant;
				return;
			case GardenView.ToolType.Harvest:
				this.cursorIcon.gameObject.SetActive(true);
				this.cursorIcon.sprite = this.iconHarvest;
				return;
			case GardenView.ToolType.Water:
				this.cursorIcon.gameObject.SetActive(true);
				this.cursorIcon.sprite = this.iconWater;
				return;
			case GardenView.ToolType.Destroy:
				this.cursorIcon.gameObject.SetActive(true);
				this.cursorIcon.sprite = this.iconDestroy;
				break;
			default:
				return;
			}
		}

		// Token: 0x06001832 RID: 6194 RVA: 0x00058610 File Offset: 0x00056810
		private void RefreshHoveringGizmos()
		{
			if (!this.cellHoveringGizmos)
			{
				return;
			}
			if (!this.Hovering || !base.enabled)
			{
				this.cellHoveringGizmos.gameObject.SetActive(false);
				return;
			}
			this.cellHoveringGizmos.gameObject.SetActive(true);
			this.cellHoveringGizmos.SetParent(null);
			this.cellHoveringGizmos.localScale = Vector3.one;
			this.cellHoveringGizmos.position = this.Target.CoordToWorldPosition(this.HoveringCoord);
			this.cellHoveringGizmos.rotation = Quaternion.LookRotation(-Vector3.up);
		}

		// Token: 0x06001833 RID: 6195 RVA: 0x000586B0 File Offset: 0x000568B0
		public void SetTool(GardenView.ToolType action)
		{
			this.Tool = action;
			this.OnContextChanged();
			this.plantModePanel.SetActive(action == GardenView.ToolType.Plant);
			Action action2 = this.onToolChanged;
			if (action2 != null)
			{
				action2();
			}
			this.RefreshSeedAmount();
		}

		// Token: 0x06001834 RID: 6196 RVA: 0x000586E5 File Offset: 0x000568E5
		private CursorData GetCursorByTool(GardenView.ToolType action)
		{
			return null;
		}

		// Token: 0x06001835 RID: 6197 RVA: 0x000586E8 File Offset: 0x000568E8
		private void UpdateContext()
		{
			bool hovering = this.Hovering;
			Crop hoveringCrop = this.HoveringCrop;
			Vector2Int hoveringCoord = this.HoveringCoord;
			Vector2Int? pointingCoord = this.GetPointingCoord();
			if (pointingCoord == null)
			{
				this.HoveringCrop = null;
				return;
			}
			this.HoveringCoord = pointingCoord.Value;
			this.HoveringCrop = this.Target[this.HoveringCoord];
			this.Hovering = this.hoveringBG;
			if (!this.HoveringCrop)
			{
				this.Hovering &= this.Target.IsCoordValid(this.HoveringCoord);
			}
			if (hovering != this.HoveringCrop || hoveringCrop != this.HoveringCrop || hoveringCoord != this.HoveringCoord)
			{
				this.OnContextChanged();
			}
		}

		// Token: 0x06001836 RID: 6198 RVA: 0x000587B0 File Offset: 0x000569B0
		private void UpdateCursor3D()
		{
			Vector3 vector;
			bool flag = this.TryPointerOnPlanePoint(UIInputManager.Point, out vector);
			this.show3DCursor = flag && this.Hovering;
			this.cursor3DTransform.gameObject.SetActive(this.show3DCursor);
			if (!flag)
			{
				return;
			}
			Vector3 position = this.cursor3DTransform.position;
			Vector3 vector2 = vector + this.cursor3DOffset;
			Vector3 vector3;
			if (this.show3DCursor)
			{
				vector3 = Vector3.Lerp(position, vector2, 0.25f);
			}
			else
			{
				vector3 = vector2;
			}
			this.cursor3DTransform.position = vector3;
		}

		// Token: 0x06001837 RID: 6199 RVA: 0x00058838 File Offset: 0x00056A38
		private void RefreshCursor3DActive()
		{
			this.cursor3D_Plant.SetActive(this.<RefreshCursor3DActive>g__ShouldShowCursor|99_0(GardenView.ToolType.Plant));
			this.cursor3D_Water.SetActive(this.<RefreshCursor3DActive>g__ShouldShowCursor|99_0(GardenView.ToolType.Water));
			this.cursor3D_Harvest.SetActive(this.<RefreshCursor3DActive>g__ShouldShowCursor|99_0(GardenView.ToolType.Harvest));
			this.cursor3D_Destory.SetActive(this.<RefreshCursor3DActive>g__ShouldShowCursor|99_0(GardenView.ToolType.Destroy));
		}

		// Token: 0x06001838 RID: 6200 RVA: 0x0005888D File Offset: 0x00056A8D
		public void SelectSeed(int seedTypeID)
		{
			this.PlantingSeedTypeID = seedTypeID;
			if (seedTypeID > 0)
			{
				this.SeedSelected = true;
			}
			this.RefreshSeedInfoDisplay();
			this.OnContextChanged();
		}

		// Token: 0x06001839 RID: 6201 RVA: 0x000588B0 File Offset: 0x00056AB0
		private void RefreshSeedInfoDisplay()
		{
			if (this.SeedSelected)
			{
				this.seedItemDisplay.Setup(this.PlantingSeedTypeID);
				this.cursorItemDisplay.Setup(this.PlantingSeedTypeID);
			}
			this.seedItemDisplay.gameObject.SetActive(this.SeedSelected);
			this.seedItemPlaceHolder.gameObject.SetActive(!this.SeedSelected);
			this.RefreshSeedAmount();
		}

		// Token: 0x0600183A RID: 6202 RVA: 0x0005891C File Offset: 0x00056B1C
		private bool TryPointerOnPlanePoint(Vector2 pointerPos, out Vector3 planePoint)
		{
			planePoint = default(Vector3);
			if (this.Target == null)
			{
				return false;
			}
			Ray ray = RectTransformUtility.ScreenPointToRay(Camera.main, pointerPos);
			Plane plane = new Plane(this.Target.transform.up, this.Target.transform.position);
			float num;
			if (!plane.Raycast(ray, out num))
			{
				return false;
			}
			planePoint = ray.GetPoint(num);
			return true;
		}

		// Token: 0x0600183B RID: 6203 RVA: 0x00058990 File Offset: 0x00056B90
		private bool TryPointerPosToCoord(Vector2 pointerPos, out Vector2Int result)
		{
			result = default(Vector2Int);
			if (this.Target == null)
			{
				return false;
			}
			Ray ray = RectTransformUtility.ScreenPointToRay(Camera.main, pointerPos);
			Plane plane = new Plane(this.Target.transform.up, this.Target.transform.position);
			float num;
			if (!plane.Raycast(ray, out num))
			{
				return false;
			}
			Vector3 point = ray.GetPoint(num);
			result = this.Target.WorldPositionToCoord(point);
			return true;
		}

		// Token: 0x0600183C RID: 6204 RVA: 0x00058A14 File Offset: 0x00056C14
		private Vector2Int? GetPointingCoord()
		{
			Vector2Int vector2Int;
			if (!this.TryPointerPosToCoord(UIInputManager.Point, out vector2Int))
			{
				return null;
			}
			return new Vector2Int?(vector2Int);
		}

		// Token: 0x0600183D RID: 6205 RVA: 0x00058A40 File Offset: 0x00056C40
		public void OnPointerClick(PointerEventData eventData)
		{
			Vector2Int vector2Int;
			if (!this.TryPointerPosToCoord(eventData.position, out vector2Int))
			{
				return;
			}
			this.ExecuteTool(vector2Int);
		}

		// Token: 0x0600183E RID: 6206 RVA: 0x00058A68 File Offset: 0x00056C68
		private void ExecuteTool(Vector2Int coord)
		{
			switch (this.Tool)
			{
			case GardenView.ToolType.None:
				break;
			case GardenView.ToolType.Plant:
				this.CropActionPlant(coord);
				return;
			case GardenView.ToolType.Harvest:
				this.CropActionHarvest(coord);
				return;
			case GardenView.ToolType.Water:
				this.CropActionWater(coord);
				return;
			case GardenView.ToolType.Destroy:
				this.CropActionDestroy(coord);
				break;
			default:
				return;
			}
		}

		// Token: 0x0600183F RID: 6207 RVA: 0x00058AB8 File Offset: 0x00056CB8
		private void CropActionDestroy(Vector2Int coord)
		{
			Crop crop = this.Target[coord];
			if (crop == null)
			{
				return;
			}
			if (crop.Ripen)
			{
				crop.Harvest();
				return;
			}
			crop.DestroyCrop();
		}

		// Token: 0x06001840 RID: 6208 RVA: 0x00058AF4 File Offset: 0x00056CF4
		private void CropActionWater(Vector2Int coord)
		{
			Crop crop = this.Target[coord];
			if (crop == null)
			{
				return;
			}
			crop.Water();
		}

		// Token: 0x06001841 RID: 6209 RVA: 0x00058B20 File Offset: 0x00056D20
		private void CropActionHarvest(Vector2Int coord)
		{
			Crop crop = this.Target[coord];
			if (crop == null)
			{
				return;
			}
			crop.Harvest();
		}

		// Token: 0x06001842 RID: 6210 RVA: 0x00058B4C File Offset: 0x00056D4C
		private void CropActionPlant(Vector2Int coord)
		{
			if (!this.Target.IsCoordValid(coord))
			{
				return;
			}
			if (this.Target[coord] != null)
			{
				return;
			}
			CropInfo? cropInfoFromSeedType = this.GetCropInfoFromSeedType(this.PlantingSeedTypeID);
			if (cropInfoFromSeedType == null)
			{
				return;
			}
			Cost cost = new Cost(new ValueTuple<int, long>[]
			{
				new ValueTuple<int, long>(this.PlantingSeedTypeID, 1L)
			});
			if (!cost.Pay(true, true))
			{
				return;
			}
			this.Target.Plant(coord, cropInfoFromSeedType.Value.id);
		}

		// Token: 0x06001843 RID: 6211 RVA: 0x00058BDC File Offset: 0x00056DDC
		private CropInfo? GetCropInfoFromSeedType(int plantingSeedTypeID)
		{
			SeedInfo seedInfo = CropDatabase.GetSeedInfo(plantingSeedTypeID);
			if (seedInfo.cropIDs == null)
			{
				return null;
			}
			if (seedInfo.cropIDs.Count <= 0)
			{
				return null;
			}
			return CropDatabase.GetCropInfo(seedInfo.GetRandomCropID());
		}

		// Token: 0x06001844 RID: 6212 RVA: 0x00058C28 File Offset: 0x00056E28
		public void OnPointerMove(PointerEventData eventData)
		{
			if (eventData.pointerCurrentRaycast.gameObject == this.mainEventReceiver)
			{
				this.hoveringBG = true;
				return;
			}
			this.hoveringBG = false;
		}

		// Token: 0x06001845 RID: 6213 RVA: 0x00058C60 File Offset: 0x00056E60
		private void RefreshSeedAmount()
		{
			if (this.SeedSelected)
			{
				int itemCount = ItemUtilities.GetItemCount(this.PlantingSeedTypeID);
				this.seedAmount = itemCount;
				string text = string.Format("x{0}", itemCount);
				this.seedAmountText.text = text;
				this.cursorAmountDisplay.text = text;
				return;
			}
			this.seedAmountText.text = "";
			this.cursorAmountDisplay.text = "";
			this.seedAmount = 0;
		}

		// Token: 0x06001846 RID: 6214 RVA: 0x00058CD9 File Offset: 0x00056ED9
		public void OnPointerDown(PointerEventData eventData)
		{
			this.dragging = true;
		}

		// Token: 0x06001847 RID: 6215 RVA: 0x00058CE2 File Offset: 0x00056EE2
		public void OnPointerUp(PointerEventData eventData)
		{
			this.dragging = false;
		}

		// Token: 0x06001848 RID: 6216 RVA: 0x00058CEB File Offset: 0x00056EEB
		public void OnPointerExit(PointerEventData eventData)
		{
			this.dragging = false;
		}

		// Token: 0x06001849 RID: 6217 RVA: 0x00058CF4 File Offset: 0x00056EF4
		private void UpdateCamera()
		{
			this.cameraRig.transform.position = this.camFocusPos;
		}

		// Token: 0x0600184A RID: 6218 RVA: 0x00058D0C File Offset: 0x00056F0C
		private void CenterCamera()
		{
			if (this.Target == null)
			{
				return;
			}
			this.camFocusPos = this.Target.transform.TransformPoint(this.Target.cameraRigCenter);
			this.UpdateCamera();
		}

		// Token: 0x0600184B RID: 6219 RVA: 0x00058D44 File Offset: 0x00056F44
		public CursorData GetCursorData()
		{
			return this.GetCursorByTool(this.Tool);
		}

		// Token: 0x0600184D RID: 6221 RVA: 0x00058DAC File Offset: 0x00056FAC
		[CompilerGenerated]
		private bool <RefreshCursor3DActive>g__ShouldShowCursor|99_0(GardenView.ToolType toolType)
		{
			if (this.Tool != toolType)
			{
				return false;
			}
			if (!this.Hovering)
			{
				return false;
			}
			switch (toolType)
			{
			case GardenView.ToolType.None:
				return false;
			case GardenView.ToolType.Plant:
				return this.SeedSelected && this.seedAmount > 0 && !this.HoveringCrop;
			case GardenView.ToolType.Harvest:
				return this.HoveringCrop && this.HoveringCrop.Ripen;
			case GardenView.ToolType.Water:
				return this.HoveringCrop;
			case GardenView.ToolType.Destroy:
				return this.HoveringCrop;
			default:
				return false;
			}
		}

		// Token: 0x04001191 RID: 4497
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x04001192 RID: 4498
		[SerializeField]
		private GameObject mainEventReceiver;

		// Token: 0x04001193 RID: 4499
		[SerializeField]
		private Button btn_ChangePlant;

		// Token: 0x04001194 RID: 4500
		[SerializeField]
		private GameObject plantModePanel;

		// Token: 0x04001195 RID: 4501
		[SerializeField]
		private ItemMetaDisplay seedItemDisplay;

		// Token: 0x04001196 RID: 4502
		[SerializeField]
		private GameObject seedItemPlaceHolder;

		// Token: 0x04001197 RID: 4503
		[SerializeField]
		private TextMeshProUGUI seedAmountText;

		// Token: 0x04001198 RID: 4504
		[SerializeField]
		private GardenViewCropSelector cropSelector;

		// Token: 0x04001199 RID: 4505
		[SerializeField]
		private Transform cellHoveringGizmos;

		// Token: 0x0400119A RID: 4506
		[SerializeField]
		[LocalizationKey("Default")]
		private string textKey_Plant = "Garden_Plant";

		// Token: 0x0400119B RID: 4507
		[SerializeField]
		[LocalizationKey("Default")]
		private string textKey_Harvest = "Garden_Harvest";

		// Token: 0x0400119C RID: 4508
		[SerializeField]
		[LocalizationKey("Default")]
		private string textKey_Destroy = "Garden_Destroy";

		// Token: 0x0400119D RID: 4509
		[SerializeField]
		[LocalizationKey("Default")]
		private string textKey_Water = "Garden_Water";

		// Token: 0x0400119E RID: 4510
		[SerializeField]
		[LocalizationKey("Default")]
		private string textKey_TargetOccupied = "Garden_TargetOccupied";

		// Token: 0x0400119F RID: 4511
		[SerializeField]
		private Transform cameraRig;

		// Token: 0x040011A0 RID: 4512
		[SerializeField]
		private Image cursorIcon;

		// Token: 0x040011A1 RID: 4513
		[SerializeField]
		private TextMeshProUGUI cursorAmountDisplay;

		// Token: 0x040011A2 RID: 4514
		[SerializeField]
		private ItemMetaDisplay cursorItemDisplay;

		// Token: 0x040011A3 RID: 4515
		[SerializeField]
		private Sprite iconPlant;

		// Token: 0x040011A4 RID: 4516
		[SerializeField]
		private Sprite iconHarvest;

		// Token: 0x040011A5 RID: 4517
		[SerializeField]
		private Sprite iconWater;

		// Token: 0x040011A6 RID: 4518
		[SerializeField]
		private Sprite iconDestroy;

		// Token: 0x040011A7 RID: 4519
		[SerializeField]
		private CursorData cursorPlant;

		// Token: 0x040011A8 RID: 4520
		[SerializeField]
		private CursorData cursorHarvest;

		// Token: 0x040011A9 RID: 4521
		[SerializeField]
		private CursorData cursorWater;

		// Token: 0x040011AA RID: 4522
		[SerializeField]
		private CursorData cursorDestroy;

		// Token: 0x040011AB RID: 4523
		[SerializeField]
		private Transform cursor3DTransform;

		// Token: 0x040011AC RID: 4524
		[SerializeField]
		private Vector3 cursor3DOffset = Vector3.up;

		// Token: 0x040011AD RID: 4525
		[SerializeField]
		private GameObject cursor3D_Plant;

		// Token: 0x040011AE RID: 4526
		[SerializeField]
		private GameObject cursor3D_Harvest;

		// Token: 0x040011AF RID: 4527
		[SerializeField]
		private GameObject cursor3D_Water;

		// Token: 0x040011B0 RID: 4528
		[SerializeField]
		private GameObject cursor3D_Destory;

		// Token: 0x040011B1 RID: 4529
		private Vector3 camFocusPos;

		// Token: 0x040011B4 RID: 4532
		private int _plantingSeedTypeID;

		// Token: 0x040011BC RID: 4540
		private bool enabledCursor;

		// Token: 0x040011BD RID: 4541
		private bool show3DCursor;

		// Token: 0x040011BE RID: 4542
		private bool hoveringBG;

		// Token: 0x040011BF RID: 4543
		private int seedAmount;

		// Token: 0x040011C0 RID: 4544
		private bool dragging;

		// Token: 0x02000580 RID: 1408
		public enum ToolType
		{
			// Token: 0x04001FA0 RID: 8096
			None,
			// Token: 0x04001FA1 RID: 8097
			Plant,
			// Token: 0x04001FA2 RID: 8098
			Harvest,
			// Token: 0x04001FA3 RID: 8099
			Water,
			// Token: 0x04001FA4 RID: 8100
			Destroy
		}
	}
}
