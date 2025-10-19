using System;
using System.Collections.Generic;
using Drawing;
using Duckov.Achievements;
using UnityEngine;

namespace Duckov.Buildings
{
	// Token: 0x0200030E RID: 782
	public class BuildingArea : MonoBehaviour, IDrawGizmos
	{
		// Token: 0x170004B4 RID: 1204
		// (get) Token: 0x060019AE RID: 6574 RVA: 0x0005CBA6 File Offset: 0x0005ADA6
		public string AreaID
		{
			get
			{
				return this.areaID;
			}
		}

		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x060019AF RID: 6575 RVA: 0x0005CBAE File Offset: 0x0005ADAE
		public Vector2Int Size
		{
			get
			{
				return this.size;
			}
		}

		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x060019B0 RID: 6576 RVA: 0x0005CBB6 File Offset: 0x0005ADB6
		public Vector2Int LowerLeftCorner
		{
			get
			{
				return this.CenterCoord - (this.size - Vector2Int.one);
			}
		}

		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x060019B1 RID: 6577 RVA: 0x0005CBD3 File Offset: 0x0005ADD3
		private Vector2Int CenterCoord
		{
			get
			{
				return new Vector2Int(Mathf.RoundToInt(base.transform.position.x), Mathf.RoundToInt(base.transform.position.z));
			}
		}

		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x060019B2 RID: 6578 RVA: 0x0005CC04 File Offset: 0x0005AE04
		private int Width
		{
			get
			{
				return this.size.x;
			}
		}

		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x060019B3 RID: 6579 RVA: 0x0005CC11 File Offset: 0x0005AE11
		private int Height
		{
			get
			{
				return this.size.y;
			}
		}

		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x060019B4 RID: 6580 RVA: 0x0005CC1E File Offset: 0x0005AE1E
		public BuildingManager.BuildingAreaData AreaData
		{
			get
			{
				return BuildingManager.GetOrCreateAreaData(this.AreaID);
			}
		}

		// Token: 0x170004BB RID: 1211
		// (get) Token: 0x060019B5 RID: 6581 RVA: 0x0005CC2B File Offset: 0x0005AE2B
		public Plane Plane
		{
			get
			{
				return new Plane(base.transform.up, base.transform.position);
			}
		}

		// Token: 0x060019B6 RID: 6582 RVA: 0x0005CC48 File Offset: 0x0005AE48
		private void Awake()
		{
			BuildingManager.OnBuildingBuilt += this.OnBuildingBuilt;
		}

		// Token: 0x060019B7 RID: 6583 RVA: 0x0005CC5B File Offset: 0x0005AE5B
		private void OnDestroy()
		{
			BuildingManager.OnBuildingBuilt -= this.OnBuildingBuilt;
		}

		// Token: 0x060019B8 RID: 6584 RVA: 0x0005CC70 File Offset: 0x0005AE70
		private void OnBuildingBuilt(int guid)
		{
			BuildingManager.BuildingData buildingData = BuildingManager.GetBuildingData(guid, null);
			if (buildingData == null)
			{
				return;
			}
			this.Display(buildingData);
		}

		// Token: 0x060019B9 RID: 6585 RVA: 0x0005CC90 File Offset: 0x0005AE90
		private void Start()
		{
			this.RepaintAll();
		}

		// Token: 0x060019BA RID: 6586 RVA: 0x0005CC98 File Offset: 0x0005AE98
		public void DrawGizmos()
		{
			if (!GizmoContext.InSelection(this))
			{
				return;
			}
			int num = this.CenterCoord.x - (this.size.x - 1);
			int num2 = this.CenterCoord.x + (this.size.x - 1) + 1;
			int num3 = this.CenterCoord.y - (this.size.y - 1);
			int num4 = this.CenterCoord.y + (this.size.y - 1) + 1;
			Vector3 vector = new Vector3(-0.5f, 0f, -0.5f);
			for (int i = num; i <= num2; i++)
			{
				Draw.Line(new Vector3((float)i, 0f, (float)num3) + vector, new Vector3((float)i, 0f, (float)num4) + vector);
			}
			for (int j = num3; j <= num4; j++)
			{
				Draw.Line(new Vector3((float)num, 0f, (float)j) + vector, new Vector3((float)num2, 0f, (float)j) + vector);
			}
		}

		// Token: 0x060019BB RID: 6587 RVA: 0x0005CDC4 File Offset: 0x0005AFC4
		public bool IsPlacementWithinRange(Vector2Int dimensions, BuildingRotation rotation, Vector2Int coord)
		{
			if (rotation % BuildingRotation.Half > BuildingRotation.Zero)
			{
				dimensions = new Vector2Int(dimensions.y, dimensions.x);
			}
			coord -= this.CenterCoord;
			return coord.x > -this.size.x && coord.y > -this.size.y && coord.x + dimensions.x <= this.size.x && coord.y + dimensions.y <= this.size.y;
		}

		// Token: 0x060019BC RID: 6588 RVA: 0x0005CE64 File Offset: 0x0005B064
		public Vector2Int CursorToCoord(Vector3 point, Vector2Int dimensions, BuildingRotation rotation)
		{
			if (rotation % BuildingRotation.Half > BuildingRotation.Zero)
			{
				dimensions = new Vector2Int(dimensions.y, dimensions.x);
			}
			int num = Mathf.RoundToInt(point.x) - dimensions.x / 2;
			int num2 = Mathf.RoundToInt(point.z) - dimensions.y / 2;
			return new Vector2Int(num, num2);
		}

		// Token: 0x060019BD RID: 6589 RVA: 0x0005CEC0 File Offset: 0x0005B0C0
		private void ReleaseAllBuildings()
		{
			for (int i = this.activeBuildings.Count - 1; i >= 0; i--)
			{
				Building building = this.activeBuildings[i];
				if (!(building == null))
				{
					global::UnityEngine.Object.Destroy(building.gameObject);
				}
			}
			this.activeBuildings.Clear();
		}

		// Token: 0x060019BE RID: 6590 RVA: 0x0005CF14 File Offset: 0x0005B114
		public void RepaintAll()
		{
			this.ReleaseAllBuildings();
			BuildingManager.BuildingAreaData areaData = this.AreaData;
			if (areaData == null)
			{
				return;
			}
			foreach (BuildingManager.BuildingData buildingData in areaData.Buildings)
			{
				this.Display(buildingData);
			}
		}

		// Token: 0x060019BF RID: 6591 RVA: 0x0005CF78 File Offset: 0x0005B178
		private void Display(BuildingManager.BuildingData building)
		{
			if (building == null)
			{
				return;
			}
			Building prefab = building.Info.Prefab;
			if (prefab == null)
			{
				Debug.LogError("No prefab for building " + building.ID);
				return;
			}
			for (int i = this.activeBuildings.Count - 1; i >= 0; i--)
			{
				Building building2 = this.activeBuildings[i];
				if (building2 == null)
				{
					this.activeBuildings.RemoveAt(i);
				}
				else if (building2.GUID == building.GUID)
				{
					Debug.LogError(string.Format("重复显示建筑{0}({1})", building.Info.DisplayName, building.GUID));
					return;
				}
			}
			Building building3 = global::UnityEngine.Object.Instantiate<Building>(prefab, base.transform);
			building3.Setup(building);
			building3.transform.position = building.GetTransformPosition();
			this.activeBuildings.Add(building3);
			if (building3.unlockAchievement && AchievementManager.Instance)
			{
				AchievementManager.Instance.Unlock("Building_" + building3.ID.Trim());
			}
		}

		// Token: 0x060019C0 RID: 6592 RVA: 0x0005D094 File Offset: 0x0005B294
		internal Vector3 CoordToWorldPosition(Vector2Int coord, Vector2Int dimensions, BuildingRotation rotation)
		{
			if (rotation % BuildingRotation.Half > BuildingRotation.Zero)
			{
				dimensions = new Vector2Int(dimensions.y, dimensions.x);
			}
			return new Vector3((float)coord.x - 0.5f + (float)dimensions.x / 2f, 0f, (float)coord.y - 0.5f + (float)dimensions.y / 2f);
		}

		// Token: 0x060019C1 RID: 6593 RVA: 0x0005D100 File Offset: 0x0005B300
		internal bool PhysicsCollide(Vector2Int dimensions, BuildingRotation rotation, Vector2Int coord, float castBeginHeight = 0f, float castHeight = 2f)
		{
			if (rotation % BuildingRotation.Half != BuildingRotation.Zero)
			{
				dimensions = new Vector2Int(dimensions.y, dimensions.x);
			}
			this.raycastHitCount = 0;
			for (int i = coord.y; i < coord.y + dimensions.y; i++)
			{
				for (int j = coord.x; j < coord.x + dimensions.x; j++)
				{
					Vector3 vector = new Vector3((float)j, castBeginHeight, (float)i);
					this.raycastHitCount += Physics.RaycastNonAlloc(vector, Vector3.up, this.raycastHitBuffer, castHeight, this.physicsCollisionLayers);
					this.raycastHitCount += Physics.RaycastNonAlloc(vector + Vector3.up * castHeight, Vector3.down, this.raycastHitBuffer, castHeight, this.physicsCollisionLayers);
					if (this.raycastHitCount > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060019C2 RID: 6594 RVA: 0x0005D1FC File Offset: 0x0005B3FC
		internal Building GetBuildingInstanceAt(Vector2Int coord)
		{
			BuildingManager.BuildingData buildingData = this.AreaData.GetBuildingAt(coord);
			if (buildingData == null)
			{
				return null;
			}
			return this.activeBuildings.Find((Building e) => e != null && e.GUID == buildingData.GUID);
		}

		// Token: 0x04001292 RID: 4754
		[SerializeField]
		private string areaID;

		// Token: 0x04001293 RID: 4755
		[SerializeField]
		private Vector2Int size;

		// Token: 0x04001294 RID: 4756
		[SerializeField]
		private LayerMask physicsCollisionLayers = -1;

		// Token: 0x04001295 RID: 4757
		private List<Building> activeBuildings = new List<Building>();

		// Token: 0x04001296 RID: 4758
		private int raycastHitCount;

		// Token: 0x04001297 RID: 4759
		private RaycastHit[] raycastHitBuffer = new RaycastHit[5];
	}
}
