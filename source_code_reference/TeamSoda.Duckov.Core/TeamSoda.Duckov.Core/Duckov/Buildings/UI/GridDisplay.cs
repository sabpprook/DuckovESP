using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Duckov.Buildings.UI
{
	// Token: 0x0200031B RID: 795
	public class GridDisplay : MonoBehaviour
	{
		// Token: 0x170004D8 RID: 1240
		// (get) Token: 0x06001A68 RID: 6760 RVA: 0x0005F854 File Offset: 0x0005DA54
		// (set) Token: 0x06001A69 RID: 6761 RVA: 0x0005F85B File Offset: 0x0005DA5B
		public static GridDisplay Instance { get; private set; }

		// Token: 0x06001A6A RID: 6762 RVA: 0x0005F863 File Offset: 0x0005DA63
		private void Awake()
		{
			GridDisplay.Instance = this;
			GridDisplay.Close();
		}

		// Token: 0x06001A6B RID: 6763 RVA: 0x0005F870 File Offset: 0x0005DA70
		public void Setup(BuildingArea buildingArea)
		{
			Vector2Int lowerLeftCorner = buildingArea.LowerLeftCorner;
			Vector4 vector = new Vector4((float)lowerLeftCorner.x, (float)lowerLeftCorner.y, (float)(buildingArea.Size.x * 2 - 1), (float)(buildingArea.Size.y * 2 - 1));
			Shader.SetGlobalVector("BuildingGrid_AreaPosAndSize", vector);
			GridDisplay.ShowGrid();
			GridDisplay.HidePreview();
			GridDisplay.ShowGrid();
		}

		// Token: 0x06001A6C RID: 6764 RVA: 0x0005F8DB File Offset: 0x0005DADB
		public static void Close()
		{
			GridDisplay.HidePreview();
			GridDisplay.HideGrid();
		}

		// Token: 0x06001A6D RID: 6765 RVA: 0x0005F8E8 File Offset: 0x0005DAE8
		public static async UniTask SetGridShowHide(bool show, AnimationCurve curve, float duration)
		{
			int token;
			do
			{
				token = global::UnityEngine.Random.Range(0, int.MaxValue);
			}
			while (token == GridDisplay.gridShowHideTaskToken);
			GridDisplay.gridShowHideTaskToken = token;
			float time = 0f;
			if (duration <= 0f)
			{
				Shader.SetGlobalFloat("BuildingGrid_Building", (float)(show ? 1 : 0));
			}
			else
			{
				while (time < duration)
				{
					time += Time.unscaledDeltaTime;
					float num = time / duration;
					float num2 = Mathf.Lerp((float)(show ? 0 : 1), (float)(show ? 1 : 0), curve.Evaluate(num));
					Shader.SetGlobalFloat("BuildingGrid_Building", num2);
					await UniTask.Yield();
					if (token != GridDisplay.gridShowHideTaskToken)
					{
						return;
					}
				}
				Shader.SetGlobalFloat("BuildingGrid_Building", (float)(show ? 1 : 0));
			}
		}

		// Token: 0x06001A6E RID: 6766 RVA: 0x0005F93B File Offset: 0x0005DB3B
		public static void HideGrid()
		{
			if (GridDisplay.Instance)
			{
				GridDisplay.SetGridShowHide(false, GridDisplay.Instance.hideCurve, GridDisplay.Instance.animationDuration).Forget();
				return;
			}
			Shader.SetGlobalFloat("BuildingGrid_Building", 0f);
		}

		// Token: 0x06001A6F RID: 6767 RVA: 0x0005F978 File Offset: 0x0005DB78
		public static void ShowGrid()
		{
			if (GridDisplay.Instance)
			{
				GridDisplay.SetGridShowHide(true, GridDisplay.Instance.showCurve, GridDisplay.Instance.animationDuration).Forget();
				return;
			}
			Shader.SetGlobalFloat("BuildingGrid_Building", 1f);
		}

		// Token: 0x06001A70 RID: 6768 RVA: 0x0005F9B5 File Offset: 0x0005DBB5
		public static void HidePreview()
		{
			Shader.SetGlobalVector("BuildingGrid_BuildingPosAndSize", Vector4.zero);
		}

		// Token: 0x06001A71 RID: 6769 RVA: 0x0005F9C8 File Offset: 0x0005DBC8
		internal void SetBuildingPreviewCoord(Vector2Int coord, Vector2Int dimensions, BuildingRotation rotation, bool validPlacement)
		{
			if (rotation % BuildingRotation.Half > BuildingRotation.Zero)
			{
				dimensions = new Vector2Int(dimensions.y, dimensions.x);
			}
			Vector4 vector = new Vector4((float)coord.x, (float)coord.y, (float)dimensions.x, (float)dimensions.y);
			Shader.SetGlobalVector("BuildingGrid_BuildingPosAndSize", vector);
			Shader.SetGlobalFloat("BuildingGrid_CanBuild", (float)(validPlacement ? 1 : 0));
		}

		// Token: 0x040012F4 RID: 4852
		[HideInInspector]
		[SerializeField]
		private BuildingArea targetArea;

		// Token: 0x040012F5 RID: 4853
		[SerializeField]
		private float animationDuration;

		// Token: 0x040012F6 RID: 4854
		[SerializeField]
		private AnimationCurve showCurve;

		// Token: 0x040012F7 RID: 4855
		[SerializeField]
		private AnimationCurve hideCurve;

		// Token: 0x040012F8 RID: 4856
		private static int gridShowHideTaskToken;
	}
}
