using System;
using System.Collections.Generic;
using Duckov.Scenes;
using Saves;
using UnityEngine;

namespace Duckov.MiniMaps
{
	// Token: 0x0200026D RID: 621
	public class MapMarkerManager : MonoBehaviour
	{
		// Token: 0x17000385 RID: 901
		// (get) Token: 0x06001368 RID: 4968 RVA: 0x000484D7 File Offset: 0x000466D7
		// (set) Token: 0x06001369 RID: 4969 RVA: 0x000484DE File Offset: 0x000466DE
		public static MapMarkerManager Instance { get; private set; }

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x0600136A RID: 4970 RVA: 0x000484E6 File Offset: 0x000466E6
		public static int SelectedIconIndex
		{
			get
			{
				if (MapMarkerManager.Instance == null)
				{
					return 0;
				}
				return MapMarkerManager.Instance.selectedIconIndex;
			}
		}

		// Token: 0x17000387 RID: 903
		// (get) Token: 0x0600136B RID: 4971 RVA: 0x00048501 File Offset: 0x00046701
		public static Color SelectedColor
		{
			get
			{
				if (MapMarkerManager.Instance == null)
				{
					return Color.white;
				}
				return MapMarkerManager.Instance.selectedColor;
			}
		}

		// Token: 0x17000388 RID: 904
		// (get) Token: 0x0600136C RID: 4972 RVA: 0x00048520 File Offset: 0x00046720
		public static Sprite SelectedIcon
		{
			get
			{
				if (MapMarkerManager.Instance == null)
				{
					return null;
				}
				if (MapMarkerManager.Instance.icons.Count <= MapMarkerManager.SelectedIconIndex)
				{
					return null;
				}
				return MapMarkerManager.Instance.icons[MapMarkerManager.SelectedIconIndex];
			}
		}

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x0600136D RID: 4973 RVA: 0x00048560 File Offset: 0x00046760
		public static string SelectedIconName
		{
			get
			{
				if (MapMarkerManager.Instance == null)
				{
					return null;
				}
				Sprite selectedIcon = MapMarkerManager.SelectedIcon;
				if (selectedIcon == null)
				{
					return null;
				}
				return selectedIcon.name;
			}
		}

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x0600136E RID: 4974 RVA: 0x00048593 File Offset: 0x00046793
		public static List<Sprite> Icons
		{
			get
			{
				if (MapMarkerManager.Instance == null)
				{
					return null;
				}
				return MapMarkerManager.Instance.icons;
			}
		}

		// Token: 0x0600136F RID: 4975 RVA: 0x000485AE File Offset: 0x000467AE
		private void Awake()
		{
			MapMarkerManager.Instance = this;
			SavesSystem.OnCollectSaveData += this.OnCollectSaveData;
		}

		// Token: 0x06001370 RID: 4976 RVA: 0x000485C7 File Offset: 0x000467C7
		private void Start()
		{
			this.Load();
		}

		// Token: 0x06001371 RID: 4977 RVA: 0x000485CF File Offset: 0x000467CF
		private void OnDestroy()
		{
			SavesSystem.OnCollectSaveData -= this.OnCollectSaveData;
		}

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x06001372 RID: 4978 RVA: 0x000485E2 File Offset: 0x000467E2
		private string SaveKey
		{
			get
			{
				return "MapMarkerManager_" + MultiSceneCore.MainSceneID;
			}
		}

		// Token: 0x06001373 RID: 4979 RVA: 0x000485F4 File Offset: 0x000467F4
		private void Load()
		{
			this.loaded = true;
			MapMarkerManager.SaveData saveData = SavesSystem.Load<MapMarkerManager.SaveData>(this.SaveKey);
			if (saveData.pois != null)
			{
				foreach (MapMarkerPOI.RuntimeData runtimeData in saveData.pois)
				{
					MapMarkerManager.Request(runtimeData);
				}
			}
		}

		// Token: 0x06001374 RID: 4980 RVA: 0x00048660 File Offset: 0x00046860
		private void OnCollectSaveData()
		{
			if (!this.loaded)
			{
				return;
			}
			MapMarkerManager.SaveData saveData = new MapMarkerManager.SaveData
			{
				pois = new List<MapMarkerPOI.RuntimeData>()
			};
			foreach (MapMarkerPOI mapMarkerPOI in this.pois)
			{
				if (!(mapMarkerPOI == null))
				{
					saveData.pois.Add(mapMarkerPOI.Data);
				}
			}
			SavesSystem.Save<MapMarkerManager.SaveData>(this.SaveKey, saveData);
		}

		// Token: 0x06001375 RID: 4981 RVA: 0x000486F4 File Offset: 0x000468F4
		public static void Request(MapMarkerPOI.RuntimeData data)
		{
			if (MapMarkerManager.Instance == null)
			{
				return;
			}
			MapMarkerPOI mapMarkerPOI = global::UnityEngine.Object.Instantiate<MapMarkerPOI>(MapMarkerManager.Instance.markerPrefab);
			mapMarkerPOI.Setup(data);
			MapMarkerManager.Instance.pois.Add(mapMarkerPOI);
			MultiSceneCore.MoveToMainScene(mapMarkerPOI.gameObject);
		}

		// Token: 0x06001376 RID: 4982 RVA: 0x00048744 File Offset: 0x00046944
		public static void Request(Vector3 worldPos)
		{
			if (MapMarkerManager.Instance == null)
			{
				return;
			}
			MapMarkerPOI mapMarkerPOI = global::UnityEngine.Object.Instantiate<MapMarkerPOI>(MapMarkerManager.Instance.markerPrefab);
			mapMarkerPOI.Setup(worldPos, MapMarkerManager.SelectedIconName, MultiSceneCore.ActiveSubSceneID, new Color?(MapMarkerManager.SelectedColor));
			MapMarkerManager.Instance.pois.Add(mapMarkerPOI);
			MultiSceneCore.MoveToMainScene(mapMarkerPOI.gameObject);
		}

		// Token: 0x06001377 RID: 4983 RVA: 0x000487A5 File Offset: 0x000469A5
		public static void Release(MapMarkerPOI entry)
		{
			if (entry == null)
			{
				return;
			}
			if (MapMarkerManager.Instance != null)
			{
				MapMarkerManager.Instance.pois.Remove(entry);
			}
			if (entry != null)
			{
				global::UnityEngine.Object.Destroy(entry.gameObject);
			}
		}

		// Token: 0x06001378 RID: 4984 RVA: 0x000487E4 File Offset: 0x000469E4
		internal static Sprite GetIcon(string iconName)
		{
			if (MapMarkerManager.Instance == null)
			{
				return null;
			}
			if (MapMarkerManager.Instance.icons == null)
			{
				return null;
			}
			return MapMarkerManager.Instance.icons.Find((Sprite e) => e != null && e.name == iconName);
		}

		// Token: 0x06001379 RID: 4985 RVA: 0x00048836 File Offset: 0x00046A36
		internal static void SelectColor(Color color)
		{
			if (MapMarkerManager.Instance == null)
			{
				return;
			}
			MapMarkerManager.Instance.selectedColor = color;
			Action<Color> onColorChanged = MapMarkerManager.OnColorChanged;
			if (onColorChanged == null)
			{
				return;
			}
			onColorChanged(color);
		}

		// Token: 0x0600137A RID: 4986 RVA: 0x00048861 File Offset: 0x00046A61
		internal static void SelectIcon(int index)
		{
			if (MapMarkerManager.Instance == null)
			{
				return;
			}
			MapMarkerManager.Instance.selectedIconIndex = index;
			Action<int> onIconChanged = MapMarkerManager.OnIconChanged;
			if (onIconChanged == null)
			{
				return;
			}
			onIconChanged(index);
		}

		// Token: 0x04000E79 RID: 3705
		[SerializeField]
		private List<Sprite> icons = new List<Sprite>();

		// Token: 0x04000E7A RID: 3706
		[SerializeField]
		private MapMarkerPOI markerPrefab;

		// Token: 0x04000E7B RID: 3707
		[SerializeField]
		private int selectedIconIndex;

		// Token: 0x04000E7C RID: 3708
		[SerializeField]
		private Color selectedColor = Color.white;

		// Token: 0x04000E7D RID: 3709
		public static Action<int> OnIconChanged;

		// Token: 0x04000E7E RID: 3710
		public static Action<Color> OnColorChanged;

		// Token: 0x04000E7F RID: 3711
		private bool loaded;

		// Token: 0x04000E80 RID: 3712
		private List<MapMarkerPOI> pois = new List<MapMarkerPOI>();

		// Token: 0x0200053C RID: 1340
		[Serializable]
		private struct SaveData
		{
			// Token: 0x04001E93 RID: 7827
			public string mainSceneName;

			// Token: 0x04001E94 RID: 7828
			public List<MapMarkerPOI.RuntimeData> pois;
		}
	}
}
