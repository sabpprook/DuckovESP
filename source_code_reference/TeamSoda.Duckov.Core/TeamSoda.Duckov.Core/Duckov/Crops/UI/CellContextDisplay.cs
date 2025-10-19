using System;
using Duckov.Economy;
using TMPro;
using UnityEngine;

namespace Duckov.Crops.UI
{
	// Token: 0x020002EC RID: 748
	public class CellContextDisplay : MonoBehaviour
	{
		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x060017FB RID: 6139 RVA: 0x00057D83 File Offset: 0x00055F83
		private Garden Garden
		{
			get
			{
				if (this.master == null)
				{
					return null;
				}
				return this.master.Target;
			}
		}

		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x060017FC RID: 6140 RVA: 0x00057DA0 File Offset: 0x00055FA0
		private Vector2Int HoveringCoord
		{
			get
			{
				if (this.master == null)
				{
					return default(Vector2Int);
				}
				return this.master.HoveringCoord;
			}
		}

		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x060017FD RID: 6141 RVA: 0x00057DD0 File Offset: 0x00055FD0
		private Crop HoveringCrop
		{
			get
			{
				if (this.master == null)
				{
					return null;
				}
				return this.master.HoveringCrop;
			}
		}

		// Token: 0x060017FE RID: 6142 RVA: 0x00057DED File Offset: 0x00055FED
		private void Show()
		{
			this.canvasGroup.alpha = 1f;
		}

		// Token: 0x060017FF RID: 6143 RVA: 0x00057DFF File Offset: 0x00055FFF
		private void Hide()
		{
			this.canvasGroup.alpha = 0f;
		}

		// Token: 0x06001800 RID: 6144 RVA: 0x00057E11 File Offset: 0x00056011
		private void Awake()
		{
			this.master.onContextChanged += this.OnContextChanged;
		}

		// Token: 0x06001801 RID: 6145 RVA: 0x00057E2A File Offset: 0x0005602A
		private void Start()
		{
			this.Refresh();
		}

		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x06001802 RID: 6146 RVA: 0x00057E32 File Offset: 0x00056032
		private bool AnyContent
		{
			get
			{
				return this.plantInfo.activeSelf || this.currentCropInfo.activeSelf || this.operationInfo.activeSelf;
			}
		}

		// Token: 0x06001803 RID: 6147 RVA: 0x00057E5B File Offset: 0x0005605B
		private void Update()
		{
			if (this.master.Hovering && this.AnyContent)
			{
				this.Show();
			}
			else
			{
				this.Hide();
			}
			if (this.HoveringCrop)
			{
				this.UpdateCurrentCropInfo();
			}
		}

		// Token: 0x06001804 RID: 6148 RVA: 0x00057E94 File Offset: 0x00056094
		private void LateUpdate()
		{
			Vector3 vector = this.Garden.CoordToWorldPosition(this.HoveringCoord) + Vector3.up * 2f;
			Vector2 vector2 = RectTransformUtility.WorldToScreenPoint(Camera.main, vector);
			base.transform.position = vector2;
		}

		// Token: 0x06001805 RID: 6149 RVA: 0x00057EE4 File Offset: 0x000560E4
		private void OnContextChanged()
		{
			this.Refresh();
		}

		// Token: 0x06001806 RID: 6150 RVA: 0x00057EEC File Offset: 0x000560EC
		private void Refresh()
		{
			this.HideAll();
			switch (this.master.Tool)
			{
			case GardenView.ToolType.None:
				break;
			case GardenView.ToolType.Plant:
				if (this.HoveringCrop)
				{
					this.SetupCurrentCropInfo();
					return;
				}
				this.SetupPlantInfo();
				if (this.master.PlantingSeedTypeID > 0)
				{
					this.SetupOperationInfo();
					return;
				}
				break;
			case GardenView.ToolType.Harvest:
				if (this.HoveringCrop == null)
				{
					return;
				}
				this.SetupCurrentCropInfo();
				if (this.HoveringCrop.Ripen)
				{
					this.SetupOperationInfo();
					return;
				}
				break;
			case GardenView.ToolType.Water:
				if (this.HoveringCrop == null)
				{
					return;
				}
				this.SetupCurrentCropInfo();
				this.SetupOperationInfo();
				return;
			case GardenView.ToolType.Destroy:
				if (this.HoveringCrop == null)
				{
					return;
				}
				this.SetupCurrentCropInfo();
				this.SetupOperationInfo();
				break;
			default:
				return;
			}
		}

		// Token: 0x06001807 RID: 6151 RVA: 0x00057FB5 File Offset: 0x000561B5
		private void SetupCurrentCropInfo()
		{
			this.currentCropInfo.SetActive(true);
			this.cropNameText.text = this.HoveringCrop.DisplayName;
			this.UpdateCurrentCropInfo();
		}

		// Token: 0x06001808 RID: 6152 RVA: 0x00057FE0 File Offset: 0x000561E0
		private void UpdateCurrentCropInfo()
		{
			if (this.HoveringCrop == null)
			{
				return;
			}
			this.cropCountdownText.text = this.HoveringCrop.RemainingTime.ToString("hh\\:mm\\:ss");
			this.cropCountdownText.gameObject.SetActive(!this.HoveringCrop.Ripen && this.HoveringCrop.Data.watered);
			this.noWaterIndicator.SetActive(!this.HoveringCrop.Data.watered);
			this.ripenIndicator.SetActive(this.HoveringCrop.Ripen);
		}

		// Token: 0x06001809 RID: 6153 RVA: 0x00058083 File Offset: 0x00056283
		private void SetupOperationInfo()
		{
			this.operationInfo.SetActive(true);
			this.operationNameText.text = this.master.ToolDisplayName;
		}

		// Token: 0x0600180A RID: 6154 RVA: 0x000580A8 File Offset: 0x000562A8
		private void SetupPlantInfo()
		{
			if (!this.master.SeedSelected)
			{
				return;
			}
			this.plantInfo.SetActive(true);
			this.plantingCropNameText.text = this.master.SeedMeta.DisplayName;
			this.plantCostDisplay.Setup(new Cost(new ValueTuple<int, long>[]
			{
				new ValueTuple<int, long>(this.master.PlantingSeedTypeID, 1L)
			}), 1);
		}

		// Token: 0x0600180B RID: 6155 RVA: 0x0005811D File Offset: 0x0005631D
		private void HideAll()
		{
			this.plantInfo.SetActive(false);
			this.currentCropInfo.SetActive(false);
			this.operationInfo.SetActive(false);
			this.Hide();
		}

		// Token: 0x04001184 RID: 4484
		[SerializeField]
		private GardenView master;

		// Token: 0x04001185 RID: 4485
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x04001186 RID: 4486
		[SerializeField]
		private GameObject plantInfo;

		// Token: 0x04001187 RID: 4487
		[SerializeField]
		private TextMeshProUGUI plantingCropNameText;

		// Token: 0x04001188 RID: 4488
		[SerializeField]
		private CostDisplay plantCostDisplay;

		// Token: 0x04001189 RID: 4489
		[SerializeField]
		private GameObject currentCropInfo;

		// Token: 0x0400118A RID: 4490
		[SerializeField]
		private TextMeshProUGUI cropNameText;

		// Token: 0x0400118B RID: 4491
		[SerializeField]
		private TextMeshProUGUI cropCountdownText;

		// Token: 0x0400118C RID: 4492
		[SerializeField]
		private GameObject noWaterIndicator;

		// Token: 0x0400118D RID: 4493
		[SerializeField]
		private GameObject ripenIndicator;

		// Token: 0x0400118E RID: 4494
		[SerializeField]
		private GameObject operationInfo;

		// Token: 0x0400118F RID: 4495
		[SerializeField]
		private TextMeshProUGUI operationNameText;
	}
}
