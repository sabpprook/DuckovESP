using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Utilities;
using Saves;
using UnityEngine;

namespace Duckov.Crops
{
	// Token: 0x020002E9 RID: 745
	public class Garden : MonoBehaviour
	{
		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x060017D0 RID: 6096 RVA: 0x0005720A File Offset: 0x0005540A
		public string GardenID
		{
			get
			{
				return this.gardenID;
			}
		}

		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x060017D1 RID: 6097 RVA: 0x00057212 File Offset: 0x00055412
		public string SaveKey
		{
			get
			{
				return "Garden_" + this.gardenID;
			}
		}

		// Token: 0x1400009A RID: 154
		// (add) Token: 0x060017D2 RID: 6098 RVA: 0x00057224 File Offset: 0x00055424
		// (remove) Token: 0x060017D3 RID: 6099 RVA: 0x00057258 File Offset: 0x00055458
		public static event Action OnSizeAddersChanged;

		// Token: 0x1400009B RID: 155
		// (add) Token: 0x060017D4 RID: 6100 RVA: 0x0005728C File Offset: 0x0005548C
		// (remove) Token: 0x060017D5 RID: 6101 RVA: 0x000572C0 File Offset: 0x000554C0
		public static event Action OnAutoWatersChanged;

		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x060017D6 RID: 6102 RVA: 0x000572F3 File Offset: 0x000554F3
		// (set) Token: 0x060017D7 RID: 6103 RVA: 0x000572FB File Offset: 0x000554FB
		public bool AutoWater
		{
			get
			{
				return this.autoWater;
			}
			set
			{
				this.autoWater = value;
				if (value)
				{
					this.WaterAll();
				}
			}
		}

		// Token: 0x060017D8 RID: 6104 RVA: 0x00057310 File Offset: 0x00055510
		private void WaterAll()
		{
			foreach (Crop crop in this.dictioanry.Values)
			{
				if (!(crop == null) && !crop.Watered)
				{
					crop.Water();
				}
			}
		}

		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x060017D9 RID: 6105 RVA: 0x00057378 File Offset: 0x00055578
		// (set) Token: 0x060017DA RID: 6106 RVA: 0x00057380 File Offset: 0x00055580
		public Vector2Int Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
				this.sizeDirty = true;
			}
		}

		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x060017DB RID: 6107 RVA: 0x00057390 File Offset: 0x00055590
		public PrefabPool<CellDisplay> CellPool
		{
			get
			{
				if (this._cellPool == null)
				{
					this._cellPool = new PrefabPool<CellDisplay>(this.cellDisplayTemplate, null, null, null, null, true, 10, 10000, null);
				}
				return this._cellPool;
			}
		}

		// Token: 0x17000454 RID: 1108
		public Crop this[Vector2Int coord]
		{
			get
			{
				Crop crop;
				if (this.dictioanry.TryGetValue(coord, out crop))
				{
					return crop;
				}
				return null;
			}
			private set
			{
				this.dictioanry[coord] = value;
			}
		}

		// Token: 0x060017DE RID: 6110 RVA: 0x000573FC File Offset: 0x000555FC
		private void Awake()
		{
			Garden.gardens[this.gardenID] = this;
			SavesSystem.OnCollectSaveData += this.Save;
			Garden.OnSizeAddersChanged += this.RefreshSize;
			Garden.OnAutoWatersChanged += this.RefreshAutowater;
		}

		// Token: 0x060017DF RID: 6111 RVA: 0x0005744D File Offset: 0x0005564D
		private void OnDestroy()
		{
			SavesSystem.OnCollectSaveData -= this.Save;
			Garden.OnSizeAddersChanged -= this.RefreshSize;
			Garden.OnAutoWatersChanged -= this.RefreshAutowater;
		}

		// Token: 0x060017E0 RID: 6112 RVA: 0x00057482 File Offset: 0x00055682
		private void Start()
		{
			this.RegenerateCellDisplays();
			this.Load();
			this.RefreshSize();
			this.RefreshAutowater();
		}

		// Token: 0x060017E1 RID: 6113 RVA: 0x0005749C File Offset: 0x0005569C
		private void FixedUpdate()
		{
			if (this.sizeDirty)
			{
				this.RegenerateCellDisplays();
			}
		}

		// Token: 0x060017E2 RID: 6114 RVA: 0x000574AC File Offset: 0x000556AC
		private void RefreshAutowater()
		{
			bool flag = false;
			using (List<IGardenAutoWaterProvider>.Enumerator enumerator = Garden.autoWaters.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.TakeEffect(this.gardenID))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag != this.AutoWater)
			{
				this.AutoWater = flag;
			}
		}

		// Token: 0x060017E3 RID: 6115 RVA: 0x0005751C File Offset: 0x0005571C
		private void RefreshSize()
		{
			Vector2Int vector2Int = Vector2Int.zero;
			foreach (IGardenSizeAdder gardenSizeAdder in Garden.sizeAdders)
			{
				if (gardenSizeAdder != null)
				{
					vector2Int += gardenSizeAdder.GetValue(this.gardenID);
				}
			}
			this.Size = new Vector2Int(3 + vector2Int.x, 3 + vector2Int.y);
		}

		// Token: 0x060017E4 RID: 6116 RVA: 0x000575A0 File Offset: 0x000557A0
		public void SetSize(int x, int y)
		{
			this.RegenerateCellDisplays();
		}

		// Token: 0x060017E5 RID: 6117 RVA: 0x000575A8 File Offset: 0x000557A8
		private void RegenerateCellDisplays()
		{
			this.sizeDirty = false;
			this.CellPool.ReleaseAll();
			Vector2Int vector2Int = this.Size;
			for (int i = 0; i < vector2Int.y; i++)
			{
				for (int j = 0; j < vector2Int.x; j++)
				{
					Vector3 vector = this.CoordToLocalPosition(new Vector2Int(j, i));
					CellDisplay cellDisplay = this.CellPool.Get(null);
					cellDisplay.transform.localPosition = vector;
					cellDisplay.Setup(this, j, i);
				}
			}
			Vector3 vector2 = this.CoordToLocalPosition(new Vector2Int(0, 0)) - new Vector3(this.grid.cellSize.x, 0f, this.grid.cellSize.y) / 2f;
			Vector3 vector3 = this.CoordToLocalPosition(new Vector2Int(vector2Int.x, vector2Int.y)) - new Vector3(this.grid.cellSize.x, 0f, this.grid.cellSize.y) / 2f;
			float num = vector3.x - vector2.x;
			float num2 = vector3.z - vector2.z;
			Vector3 vector4 = vector2;
			Vector3 vector5 = new Vector3(vector2.x, 0f, vector3.z);
			Vector3 vector6 = vector3;
			Vector3 vector7 = new Vector3(vector3.x, 0f, vector2.z);
			Vector3 vector8 = new Vector3(1f, 1f, num2);
			Vector3 vector9 = new Vector3(1f, 1f, num);
			Vector3 vector10 = new Vector3(1f, 1f, num2);
			Vector3 vector11 = new Vector3(1f, 1f, num);
			this.border00.localPosition = vector4;
			this.border01.localPosition = vector5;
			this.border11.localPosition = vector6;
			this.border10.localPosition = vector7;
			this.corner00.localPosition = vector4;
			this.corner01.localPosition = vector5;
			this.corner11.localPosition = vector6;
			this.corner10.localPosition = vector7;
			this.border00.localScale = vector8;
			this.border01.localScale = vector9;
			this.border11.localScale = vector10;
			this.border10.localScale = vector11;
			this.border00.localRotation = Quaternion.Euler(0f, 0f, 0f);
			this.border01.localRotation = Quaternion.Euler(0f, 90f, 0f);
			this.border11.localRotation = Quaternion.Euler(0f, 180f, 0f);
			this.border10.localRotation = Quaternion.Euler(0f, 270f, 0f);
			Vector3 vector12 = (vector2 + vector3) / 2f;
			this.interactBox.transform.localPosition = vector12;
			this.interactBox.center = Vector3.zero;
			this.interactBox.size = new Vector3(num + 0.5f, 1f, num2 + 0.5f);
		}

		// Token: 0x060017E6 RID: 6118 RVA: 0x000578D6 File Offset: 0x00055AD6
		private Crop CreateCropInstance(string id)
		{
			return global::UnityEngine.Object.Instantiate<Crop>(this.cropTemplate, base.transform);
		}

		// Token: 0x060017E7 RID: 6119 RVA: 0x000578EC File Offset: 0x00055AEC
		public void Save()
		{
			if (!LevelManager.LevelInited)
			{
				return;
			}
			Garden.SaveData saveData = new Garden.SaveData(this);
			SavesSystem.Save<Garden.SaveData>(this.SaveKey, saveData);
		}

		// Token: 0x060017E8 RID: 6120 RVA: 0x00057914 File Offset: 0x00055B14
		public void Load()
		{
			this.Clear();
			this.dictioanry.Clear();
			Garden.SaveData saveData = SavesSystem.Load<Garden.SaveData>(this.SaveKey);
			if (saveData == null)
			{
				return;
			}
			foreach (CropData cropData in saveData.crops)
			{
				Crop crop = this.CreateCropInstance(cropData.cropID);
				crop.Initialize(this, cropData);
				this[cropData.coord] = crop;
			}
		}

		// Token: 0x060017E9 RID: 6121 RVA: 0x000579A4 File Offset: 0x00055BA4
		private void Clear()
		{
			foreach (Crop crop in this.dictioanry.Values.ToList<Crop>())
			{
				if (!(crop == null))
				{
					global::UnityEngine.Object.Destroy(crop.gameObject);
				}
			}
		}

		// Token: 0x060017EA RID: 6122 RVA: 0x00057A10 File Offset: 0x00055C10
		public bool IsCoordValid(Vector2Int coord)
		{
			Vector2Int vector2Int = this.Size;
			return vector2Int.x <= 0 || vector2Int.y <= 0 || (coord.x < vector2Int.x && coord.y < vector2Int.y && coord.x >= 0 && coord.y >= 0);
		}

		// Token: 0x060017EB RID: 6123 RVA: 0x00057A73 File Offset: 0x00055C73
		public bool IsCoordOccupied(Vector2Int coord)
		{
			return this[coord] != null;
		}

		// Token: 0x060017EC RID: 6124 RVA: 0x00057A84 File Offset: 0x00055C84
		public bool Plant(Vector2Int coord, string cropID)
		{
			if (!this.IsCoordValid(coord))
			{
				return false;
			}
			if (this.IsCoordOccupied(coord))
			{
				return false;
			}
			if (!CropDatabase.IsIdValid(cropID))
			{
				Debug.Log("[Garden] Invalid crop id " + cropID, this);
				return false;
			}
			Crop crop = this.CreateCropInstance(cropID);
			crop.InitializeNew(this, cropID, coord);
			this[coord] = crop;
			if (this.autoWater)
			{
				crop.Water();
			}
			return true;
		}

		// Token: 0x060017ED RID: 6125 RVA: 0x00057AEC File Offset: 0x00055CEC
		public void Water(Vector2Int coord)
		{
			Crop crop = this[coord];
			if (crop == null)
			{
				return;
			}
			crop.Water();
		}

		// Token: 0x060017EE RID: 6126 RVA: 0x00057B14 File Offset: 0x00055D14
		public Vector3 CoordToWorldPosition(Vector2Int coord)
		{
			Vector3 vector = this.CoordToLocalPosition(coord);
			return base.transform.TransformPoint(vector);
		}

		// Token: 0x060017EF RID: 6127 RVA: 0x00057B38 File Offset: 0x00055D38
		public Vector3 CoordToLocalPosition(Vector2Int coord)
		{
			Vector3 cellCenterLocal = this.grid.GetCellCenterLocal((Vector3Int)coord);
			float z = this.grid.cellSize.z;
			float num = cellCenterLocal.y - z / 2f;
			Vector3 vector = cellCenterLocal;
			vector.y = num;
			return vector;
		}

		// Token: 0x060017F0 RID: 6128 RVA: 0x00057B80 File Offset: 0x00055D80
		public Vector2Int WorldPositionToCoord(Vector3 wPos)
		{
			Vector3 vector = wPos + Vector3.up * 0.1f * this.grid.cellSize.z;
			return (Vector2Int)this.grid.WorldToCell(vector);
		}

		// Token: 0x060017F1 RID: 6129 RVA: 0x00057BC9 File Offset: 0x00055DC9
		internal void Release(Crop crop)
		{
			global::UnityEngine.Object.Destroy(crop.gameObject);
		}

		// Token: 0x060017F2 RID: 6130 RVA: 0x00057BD8 File Offset: 0x00055DD8
		private void OnDrawGizmosSelected()
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			float x = this.grid.cellSize.x;
			float y = this.grid.cellSize.y;
			Vector2Int vector2Int = this.Size;
			for (int i = 0; i <= vector2Int.x; i++)
			{
				Vector3 vector = Vector3.right * (float)i * x;
				Vector3 vector2 = vector + Vector3.forward * (float)vector2Int.y * y;
				Gizmos.DrawLine(vector, vector2);
			}
			for (int j = 0; j <= vector2Int.y; j++)
			{
				Vector3 vector3 = Vector3.forward * (float)j * y;
				Vector3 vector4 = vector3 + Vector3.right * (float)vector2Int.x * x;
				Gizmos.DrawLine(vector3, vector4);
			}
		}

		// Token: 0x060017F3 RID: 6131 RVA: 0x00057CB9 File Offset: 0x00055EB9
		internal static void Register(IGardenSizeAdder obj)
		{
			Garden.sizeAdders.Add(obj);
			Action onSizeAddersChanged = Garden.OnSizeAddersChanged;
			if (onSizeAddersChanged == null)
			{
				return;
			}
			onSizeAddersChanged();
		}

		// Token: 0x060017F4 RID: 6132 RVA: 0x00057CD5 File Offset: 0x00055ED5
		internal static void Register(IGardenAutoWaterProvider obj)
		{
			Garden.autoWaters.Add(obj);
			Action onAutoWatersChanged = Garden.OnAutoWatersChanged;
			if (onAutoWatersChanged == null)
			{
				return;
			}
			onAutoWatersChanged();
		}

		// Token: 0x060017F5 RID: 6133 RVA: 0x00057CF1 File Offset: 0x00055EF1
		internal static void Unregister(IGardenSizeAdder obj)
		{
			Garden.sizeAdders.Remove(obj);
			Action onSizeAddersChanged = Garden.OnSizeAddersChanged;
			if (onSizeAddersChanged == null)
			{
				return;
			}
			onSizeAddersChanged();
		}

		// Token: 0x060017F6 RID: 6134 RVA: 0x00057D0E File Offset: 0x00055F0E
		internal static void Unregister(IGardenAutoWaterProvider obj)
		{
			Garden.autoWaters.Remove(obj);
			Action onAutoWatersChanged = Garden.OnAutoWatersChanged;
			if (onAutoWatersChanged == null)
			{
				return;
			}
			onAutoWatersChanged();
		}

		// Token: 0x0400116C RID: 4460
		[SerializeField]
		private string gardenID = "Default";

		// Token: 0x0400116D RID: 4461
		public static List<IGardenSizeAdder> sizeAdders = new List<IGardenSizeAdder>();

		// Token: 0x0400116E RID: 4462
		public static List<IGardenAutoWaterProvider> autoWaters = new List<IGardenAutoWaterProvider>();

		// Token: 0x04001171 RID: 4465
		public static Dictionary<string, Garden> gardens = new Dictionary<string, Garden>();

		// Token: 0x04001172 RID: 4466
		[SerializeField]
		private Grid grid;

		// Token: 0x04001173 RID: 4467
		[SerializeField]
		private Crop cropTemplate;

		// Token: 0x04001174 RID: 4468
		[SerializeField]
		private Transform border00;

		// Token: 0x04001175 RID: 4469
		[SerializeField]
		private Transform border01;

		// Token: 0x04001176 RID: 4470
		[SerializeField]
		private Transform border11;

		// Token: 0x04001177 RID: 4471
		[SerializeField]
		private Transform border10;

		// Token: 0x04001178 RID: 4472
		[SerializeField]
		private Transform corner00;

		// Token: 0x04001179 RID: 4473
		[SerializeField]
		private Transform corner01;

		// Token: 0x0400117A RID: 4474
		[SerializeField]
		private Transform corner11;

		// Token: 0x0400117B RID: 4475
		[SerializeField]
		private Transform corner10;

		// Token: 0x0400117C RID: 4476
		[SerializeField]
		private BoxCollider interactBox;

		// Token: 0x0400117D RID: 4477
		[SerializeField]
		private Vector2Int size;

		// Token: 0x0400117E RID: 4478
		[SerializeField]
		private bool autoWater;

		// Token: 0x0400117F RID: 4479
		public Vector3 cameraRigCenter = new Vector3(3f, 0f, 3f);

		// Token: 0x04001180 RID: 4480
		private bool sizeDirty;

		// Token: 0x04001181 RID: 4481
		[SerializeField]
		private CellDisplay cellDisplayTemplate;

		// Token: 0x04001182 RID: 4482
		private PrefabPool<CellDisplay> _cellPool;

		// Token: 0x04001183 RID: 4483
		private Dictionary<Vector2Int, Crop> dictioanry = new Dictionary<Vector2Int, Crop>();

		// Token: 0x0200057F RID: 1407
		[Serializable]
		private class SaveData
		{
			// Token: 0x0600283D RID: 10301 RVA: 0x00094714 File Offset: 0x00092914
			public SaveData(Garden garden)
			{
				this.crops = new List<CropData>();
				foreach (Crop crop in garden.dictioanry.Values)
				{
					if (!(crop == null))
					{
						this.crops.Add(crop.Data);
					}
				}
			}

			// Token: 0x04001F9E RID: 8094
			[SerializeField]
			public List<CropData> crops;
		}
	}
}
