using System;
using Cysharp.Threading.Tasks;
using Duckov.Economy;
using UnityEngine;

namespace Duckov.Crops
{
	// Token: 0x020002E2 RID: 738
	public class Crop : MonoBehaviour
	{
		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x0600179F RID: 6047 RVA: 0x000567FC File Offset: 0x000549FC
		public CropData Data
		{
			get
			{
				return this.data;
			}
		}

		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x060017A0 RID: 6048 RVA: 0x00056804 File Offset: 0x00054A04
		public CropInfo Info
		{
			get
			{
				return this.info;
			}
		}

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x060017A1 RID: 6049 RVA: 0x0005680C File Offset: 0x00054A0C
		public float Progress
		{
			get
			{
				return (float)this.data.growTicks / (float)this.info.totalGrowTicks;
			}
		}

		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x060017A2 RID: 6050 RVA: 0x00056827 File Offset: 0x00054A27
		public bool Ripen
		{
			get
			{
				return this.initialized && this.data.growTicks >= this.info.totalGrowTicks;
			}
		}

		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x060017A3 RID: 6051 RVA: 0x0005684E File Offset: 0x00054A4E
		public bool Watered
		{
			get
			{
				return this.data.watered;
			}
		}

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x060017A4 RID: 6052 RVA: 0x0005685C File Offset: 0x00054A5C
		public string DisplayName
		{
			get
			{
				return this.Info.DisplayName;
			}
		}

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x060017A5 RID: 6053 RVA: 0x00056878 File Offset: 0x00054A78
		public TimeSpan RemainingTime
		{
			get
			{
				if (!this.initialized)
				{
					return TimeSpan.Zero;
				}
				long num = this.info.totalGrowTicks - this.data.growTicks;
				if (num < 0L)
				{
					return TimeSpan.Zero;
				}
				return TimeSpan.FromTicks(num);
			}
		}

		// Token: 0x14000099 RID: 153
		// (add) Token: 0x060017A6 RID: 6054 RVA: 0x000568BC File Offset: 0x00054ABC
		// (remove) Token: 0x060017A7 RID: 6055 RVA: 0x000568F0 File Offset: 0x00054AF0
		public static event Action<Crop, Crop.CropEvent> onCropStatusChange;

		// Token: 0x060017A8 RID: 6056 RVA: 0x00056924 File Offset: 0x00054B24
		public bool Harvest()
		{
			if (!this.Ripen)
			{
				return false;
			}
			if (this.Watered)
			{
				this.data.score = this.data.score + 50;
			}
			int product = this.info.GetProduct(this.data.Ranking);
			if (product <= 0)
			{
				Debug.LogError("Crop product is invalid:\ncrop:" + this.info.id);
				return false;
			}
			Cost cost = new Cost(new ValueTuple<int, long>[]
			{
				new ValueTuple<int, long>(product, (long)this.info.resultAmount)
			});
			cost.Return(false, false, 1, null).Forget();
			this.DestroyCrop();
			Action<Crop> action = this.onHarvest;
			if (action != null)
			{
				action(this);
			}
			Action<Crop, Crop.CropEvent> action2 = Crop.onCropStatusChange;
			if (action2 != null)
			{
				action2(this, Crop.CropEvent.Harvest);
			}
			return true;
		}

		// Token: 0x060017A9 RID: 6057 RVA: 0x000569EC File Offset: 0x00054BEC
		public void DestroyCrop()
		{
			Action<Crop> action = this.onBeforeDestroy;
			if (action != null)
			{
				action(this);
			}
			Action<Crop, Crop.CropEvent> action2 = Crop.onCropStatusChange;
			if (action2 != null)
			{
				action2(this, Crop.CropEvent.BeforeDestroy);
			}
			this.garden.Release(this);
		}

		// Token: 0x060017AA RID: 6058 RVA: 0x00056A20 File Offset: 0x00054C20
		public void InitializeNew(Garden garden, string id, Vector2Int coord)
		{
			CropData cropData = new CropData
			{
				gardenID = garden.GardenID,
				cropID = id,
				coord = coord,
				LastUpdateDateTime = DateTime.Now
			};
			this.Initialize(garden, cropData);
			Action<Crop> action = this.onPlant;
			if (action != null)
			{
				action(this);
			}
			Action<Crop, Crop.CropEvent> action2 = Crop.onCropStatusChange;
			if (action2 == null)
			{
				return;
			}
			action2(this, Crop.CropEvent.Plant);
		}

		// Token: 0x060017AB RID: 6059 RVA: 0x00056A8C File Offset: 0x00054C8C
		public void Initialize(Garden garden, CropData data)
		{
			this.garden = garden;
			string cropID = data.cropID;
			CropInfo? cropInfo = CropDatabase.GetCropInfo(cropID);
			if (cropInfo == null)
			{
				Debug.LogError("找不到 corpInfo id: " + cropID);
				return;
			}
			this.info = cropInfo.Value;
			this.data = data;
			this.RefreshDisplayInstance();
			this.initialized = true;
			Vector3 vector = garden.CoordToLocalPosition(data.coord);
			base.transform.localPosition = vector;
		}

		// Token: 0x060017AC RID: 6060 RVA: 0x00056B04 File Offset: 0x00054D04
		private void RefreshDisplayInstance()
		{
			if (this.displayInstance != null)
			{
				if (Application.isPlaying)
				{
					global::UnityEngine.Object.Destroy(this.displayInstance.gameObject);
				}
				else
				{
					global::UnityEngine.Object.DestroyImmediate(this.displayInstance.gameObject);
				}
			}
			if (this.info.displayPrefab == null)
			{
				Debug.LogError("找不到Display Prefab: " + this.info.DisplayName);
				return;
			}
			this.displayInstance = global::UnityEngine.Object.Instantiate<GameObject>(this.info.displayPrefab, this.displayParent);
			this.displayInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		}

		// Token: 0x060017AD RID: 6061 RVA: 0x00056BAC File Offset: 0x00054DAC
		public void Water()
		{
			if (this.data.watered)
			{
				return;
			}
			this.data.watered = true;
			Action<Crop> action = this.onWater;
			if (action != null)
			{
				action(this);
			}
			Action<Crop, Crop.CropEvent> action2 = Crop.onCropStatusChange;
			if (action2 == null)
			{
				return;
			}
			action2(this, Crop.CropEvent.Water);
		}

		// Token: 0x060017AE RID: 6062 RVA: 0x00056BEB File Offset: 0x00054DEB
		private void FixedUpdate()
		{
			this.Tick();
		}

		// Token: 0x060017AF RID: 6063 RVA: 0x00056BF4 File Offset: 0x00054DF4
		private void Tick()
		{
			if (!this.initialized)
			{
				return;
			}
			TimeSpan timeSpan = DateTime.Now - this.data.LastUpdateDateTime;
			this.data.LastUpdateDateTime = DateTime.Now;
			if (!this.data.watered)
			{
				return;
			}
			if (this.Ripen)
			{
				return;
			}
			long ticks = timeSpan.Ticks;
			this.data.growTicks = this.data.growTicks + ticks;
			if (this.Ripen)
			{
				this.OnRipen();
			}
		}

		// Token: 0x060017B0 RID: 6064 RVA: 0x00056C6D File Offset: 0x00054E6D
		private void OnRipen()
		{
			Action<Crop> action = this.onRipen;
			if (action != null)
			{
				action(this);
			}
			Action<Crop, Crop.CropEvent> action2 = Crop.onCropStatusChange;
			if (action2 == null)
			{
				return;
			}
			action2(this, Crop.CropEvent.Ripen);
		}

		// Token: 0x0400113F RID: 4415
		[SerializeField]
		private Transform displayParent;

		// Token: 0x04001140 RID: 4416
		private Garden garden;

		// Token: 0x04001141 RID: 4417
		private bool initialized;

		// Token: 0x04001142 RID: 4418
		private CropData data;

		// Token: 0x04001143 RID: 4419
		private CropInfo info;

		// Token: 0x04001144 RID: 4420
		private GameObject displayInstance;

		// Token: 0x04001145 RID: 4421
		public Action<Crop> onPlant;

		// Token: 0x04001146 RID: 4422
		public Action<Crop> onWater;

		// Token: 0x04001147 RID: 4423
		public Action<Crop> onRipen;

		// Token: 0x04001148 RID: 4424
		public Action<Crop> onHarvest;

		// Token: 0x04001149 RID: 4425
		public Action<Crop> onBeforeDestroy;

		// Token: 0x0200057A RID: 1402
		public enum CropEvent
		{
			// Token: 0x04001F94 RID: 8084
			Plant,
			// Token: 0x04001F95 RID: 8085
			Water,
			// Token: 0x04001F96 RID: 8086
			Ripen,
			// Token: 0x04001F97 RID: 8087
			Harvest,
			// Token: 0x04001F98 RID: 8088
			BeforeDestroy
		}
	}
}
