using System;
using Drawing;
using Duckov.Utilities;
using SodaCraft.Localizations;
using Unity.Mathematics;
using UnityEngine;

namespace Duckov.Buildings
{
	// Token: 0x0200030F RID: 783
	public class Building : MonoBehaviour, IDrawGizmos
	{
		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x060019C4 RID: 6596 RVA: 0x0005D24A File Offset: 0x0005B44A
		private int guid
		{
			get
			{
				return this.data.GUID;
			}
		}

		// Token: 0x170004BD RID: 1213
		// (get) Token: 0x060019C5 RID: 6597 RVA: 0x0005D257 File Offset: 0x0005B457
		public int GUID
		{
			get
			{
				return this.guid;
			}
		}

		// Token: 0x170004BE RID: 1214
		// (get) Token: 0x060019C6 RID: 6598 RVA: 0x0005D25F File Offset: 0x0005B45F
		public string ID
		{
			get
			{
				return this.id;
			}
		}

		// Token: 0x170004BF RID: 1215
		// (get) Token: 0x060019C7 RID: 6599 RVA: 0x0005D267 File Offset: 0x0005B467
		public Vector2Int Dimensions
		{
			get
			{
				return this.dimensions;
			}
		}

		// Token: 0x060019C8 RID: 6600 RVA: 0x0005D270 File Offset: 0x0005B470
		public Vector3 GetOffset(BuildingRotation rotation = BuildingRotation.Zero)
		{
			bool flag = rotation % BuildingRotation.Half > BuildingRotation.Zero;
			float num = (float)((flag ? this.dimensions.y : this.dimensions.x) - 1);
			float num2 = (float)((flag ? this.dimensions.x : this.dimensions.y) - 1);
			return new Vector3(num / 2f, 0f, num2 / 2f);
		}

		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x060019CA RID: 6602 RVA: 0x0005D2DA File Offset: 0x0005B4DA
		// (set) Token: 0x060019C9 RID: 6601 RVA: 0x0005D2D8 File Offset: 0x0005B4D8
		[LocalizationKey("Default")]
		public string DisplayNameKey
		{
			get
			{
				return "Building_" + this.ID;
			}
			set
			{
			}
		}

		// Token: 0x170004C1 RID: 1217
		// (get) Token: 0x060019CB RID: 6603 RVA: 0x0005D2EC File Offset: 0x0005B4EC
		public string DisplayName
		{
			get
			{
				return this.DisplayNameKey.ToPlainText();
			}
		}

		// Token: 0x060019CC RID: 6604 RVA: 0x0005D2F9 File Offset: 0x0005B4F9
		public static string GetDisplayName(string id)
		{
			return ("Building_" + id).ToPlainText();
		}

		// Token: 0x170004C2 RID: 1218
		// (get) Token: 0x060019CE RID: 6606 RVA: 0x0005D30D File Offset: 0x0005B50D
		// (set) Token: 0x060019CD RID: 6605 RVA: 0x0005D30B File Offset: 0x0005B50B
		[LocalizationKey("Default")]
		public string DescriptionKey
		{
			get
			{
				return "Building_" + this.ID + "_Desc";
			}
			set
			{
			}
		}

		// Token: 0x170004C3 RID: 1219
		// (get) Token: 0x060019CF RID: 6607 RVA: 0x0005D324 File Offset: 0x0005B524
		public string Description
		{
			get
			{
				return this.DescriptionKey.ToPlainText();
			}
		}

		// Token: 0x060019D0 RID: 6608 RVA: 0x0005D334 File Offset: 0x0005B534
		private void Awake()
		{
			if (this.graphicsContainer == null)
			{
				Debug.LogError("建筑" + this.DisplayName + "未配置 Graphics Container");
				Transform transform = base.transform.Find("Graphics");
				this.graphicsContainer = ((transform != null) ? transform.gameObject : null);
			}
			if (this.functionContainer == null)
			{
				Debug.LogError("建筑" + this.DisplayName + "未配置 Function Container");
				Transform transform2 = base.transform.Find("Function");
				this.functionContainer = ((transform2 != null) ? transform2.gameObject : null);
			}
			this.CreateAreaMesh();
		}

		// Token: 0x060019D1 RID: 6609 RVA: 0x0005D3DC File Offset: 0x0005B5DC
		private void CreateAreaMesh()
		{
			if (this.areaMesh == null)
			{
				this.areaMesh = global::UnityEngine.Object.Instantiate<GameObject>(GameplayDataSettings.Prefabs.BuildingBlockAreaMesh, base.transform);
				this.areaMesh.transform.localPosition = Vector3.zero;
				this.areaMesh.transform.localRotation = quaternion.identity;
				this.areaMesh.transform.localScale = new Vector3((float)this.dimensions.x - 0.02f, 1f, (float)this.dimensions.y - 0.02f);
				this.areaMesh.transform.SetParent(this.functionContainer.transform, true);
			}
		}

		// Token: 0x060019D2 RID: 6610 RVA: 0x0005D49E File Offset: 0x0005B69E
		private void RegisterEvents()
		{
			BuildingManager.OnBuildingDestroyed += this.OnBuildingDestroyed;
		}

		// Token: 0x060019D3 RID: 6611 RVA: 0x0005D4B1 File Offset: 0x0005B6B1
		private void OnBuildingDestroyed(int guid)
		{
			if (guid == this.GUID)
			{
				this.Release();
			}
		}

		// Token: 0x060019D4 RID: 6612 RVA: 0x0005D4C2 File Offset: 0x0005B6C2
		private void Release()
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x060019D5 RID: 6613 RVA: 0x0005D4CF File Offset: 0x0005B6CF
		private void UnregisterEvents()
		{
			BuildingManager.OnBuildingDestroyed -= this.OnBuildingDestroyed;
		}

		// Token: 0x060019D6 RID: 6614 RVA: 0x0005D4E4 File Offset: 0x0005B6E4
		public void DrawGizmos()
		{
			if (!GizmoContext.InSelection(this))
			{
				return;
			}
			using (Draw.WithColor(new Color(1f, 1f, 1f, 0.5f)))
			{
				using (Draw.InLocalSpace(base.transform))
				{
					float3 @float = this.GetOffset(BuildingRotation.Zero);
					float2 float2 = new float2(0.9f, 0.9f);
					for (int i = 0; i < this.Dimensions.y; i++)
					{
						for (int j = 0; j < this.Dimensions.x; j++)
						{
							Draw.SolidPlane(new float3((float)j, 0f, (float)i) - @float, Vector3.up, float2);
						}
					}
				}
			}
		}

		// Token: 0x060019D7 RID: 6615 RVA: 0x0005D5E0 File Offset: 0x0005B7E0
		internal void Setup(BuildingManager.BuildingData data)
		{
			this.data = data;
			base.transform.localRotation = Quaternion.Euler(0f, (float)(data.Rotation * (BuildingRotation)90), 0f);
			this.RegisterEvents();
		}

		// Token: 0x060019D8 RID: 6616 RVA: 0x0005D613 File Offset: 0x0005B813
		private void OnDestroy()
		{
			this.UnregisterEvents();
		}

		// Token: 0x060019D9 RID: 6617 RVA: 0x0005D61C File Offset: 0x0005B81C
		internal void SetupPreview()
		{
			this.functionContainer.SetActive(false);
			Collider[] componentsInChildren = this.graphicsContainer.GetComponentsInChildren<Collider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}

		// Token: 0x04001298 RID: 4760
		[SerializeField]
		private string id;

		// Token: 0x04001299 RID: 4761
		[SerializeField]
		private Vector2Int dimensions;

		// Token: 0x0400129A RID: 4762
		[SerializeField]
		private GameObject graphicsContainer;

		// Token: 0x0400129B RID: 4763
		[SerializeField]
		private GameObject functionContainer;

		// Token: 0x0400129C RID: 4764
		private BuildingManager.BuildingData data;

		// Token: 0x0400129D RID: 4765
		public bool unlockAchievement;

		// Token: 0x0400129E RID: 4766
		private GameObject areaMesh;
	}
}
