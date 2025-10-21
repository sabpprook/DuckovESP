using System;
using UnityEngine;

namespace Duckov.Crops
{
	// Token: 0x020002E1 RID: 737
	public class CellDisplay : MonoBehaviour
	{
		// Token: 0x06001798 RID: 6040 RVA: 0x00056684 File Offset: 0x00054884
		internal void Setup(Garden garden, int coordx, int coordy)
		{
			this.garden = garden;
			this.coord = new Vector2Int(coordx, coordy);
			bool flag = false;
			Crop crop = garden[this.coord];
			if (crop != null)
			{
				flag = crop.Watered;
			}
			this.RefreshGraphics(flag);
		}

		// Token: 0x06001799 RID: 6041 RVA: 0x000566CB File Offset: 0x000548CB
		private void OnEnable()
		{
			Crop.onCropStatusChange += this.HandleCropEvent;
		}

		// Token: 0x0600179A RID: 6042 RVA: 0x000566DE File Offset: 0x000548DE
		private void OnDisable()
		{
			Crop.onCropStatusChange -= this.HandleCropEvent;
		}

		// Token: 0x0600179B RID: 6043 RVA: 0x000566F4 File Offset: 0x000548F4
		private void HandleCropEvent(Crop crop, Crop.CropEvent e)
		{
			if (crop == null)
			{
				return;
			}
			if (this.garden == null)
			{
				return;
			}
			CropData data = crop.Data;
			if (data.gardenID != this.garden.GardenID || data.coord != this.coord)
			{
				return;
			}
			this.RefreshGraphics(crop.Watered && e != Crop.CropEvent.BeforeDestroy && e != Crop.CropEvent.Harvest);
		}

		// Token: 0x0600179C RID: 6044 RVA: 0x00056769 File Offset: 0x00054969
		private void RefreshGraphics(bool watered)
		{
			if (watered)
			{
				this.ApplyGraphicsStype(this.styleWatered);
				return;
			}
			this.ApplyGraphicsStype(this.styleDry);
		}

		// Token: 0x0600179D RID: 6045 RVA: 0x00056788 File Offset: 0x00054988
		private void ApplyGraphicsStype(CellDisplay.GraphicsStyle style)
		{
			if (this.propertyBlock == null)
			{
				this.propertyBlock = new MaterialPropertyBlock();
			}
			this.propertyBlock.Clear();
			string text = "_TintColor";
			string text2 = "_Smoothness";
			this.propertyBlock.SetColor(text, style.color);
			this.propertyBlock.SetFloat(text2, style.smoothness);
			this.renderer.SetPropertyBlock(this.propertyBlock);
		}

		// Token: 0x04001139 RID: 4409
		[SerializeField]
		private Renderer renderer;

		// Token: 0x0400113A RID: 4410
		[SerializeField]
		private CellDisplay.GraphicsStyle styleDry;

		// Token: 0x0400113B RID: 4411
		[SerializeField]
		private CellDisplay.GraphicsStyle styleWatered;

		// Token: 0x0400113C RID: 4412
		private Garden garden;

		// Token: 0x0400113D RID: 4413
		private Vector2Int coord;

		// Token: 0x0400113E RID: 4414
		private MaterialPropertyBlock propertyBlock;

		// Token: 0x02000579 RID: 1401
		[Serializable]
		private struct GraphicsStyle
		{
			// Token: 0x04001F91 RID: 8081
			public Color color;

			// Token: 0x04001F92 RID: 8082
			public float smoothness;
		}
	}
}
