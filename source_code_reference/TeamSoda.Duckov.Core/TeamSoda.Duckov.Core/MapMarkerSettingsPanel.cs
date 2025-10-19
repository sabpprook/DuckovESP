using System;
using System.Collections.Generic;
using Duckov.MiniMaps;
using Duckov.Utilities;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001C0 RID: 448
public class MapMarkerSettingsPanel : MonoBehaviour
{
	// Token: 0x17000275 RID: 629
	// (get) Token: 0x06000D58 RID: 3416 RVA: 0x000371A3 File Offset: 0x000353A3
	private List<Sprite> Icons
	{
		get
		{
			return MapMarkerManager.Icons;
		}
	}

	// Token: 0x17000276 RID: 630
	// (get) Token: 0x06000D59 RID: 3417 RVA: 0x000371AC File Offset: 0x000353AC
	private PrefabPool<MapMarkerPanelButton> IconBtnPool
	{
		get
		{
			if (this._iconBtnPool == null)
			{
				this._iconBtnPool = new PrefabPool<MapMarkerPanelButton>(this.iconBtnTemplate, null, null, null, null, true, 10, 10000, null);
			}
			return this._iconBtnPool;
		}
	}

	// Token: 0x17000277 RID: 631
	// (get) Token: 0x06000D5A RID: 3418 RVA: 0x000371E8 File Offset: 0x000353E8
	private PrefabPool<MapMarkerPanelButton> ColorBtnPool
	{
		get
		{
			if (this._colorBtnPool == null)
			{
				this._colorBtnPool = new PrefabPool<MapMarkerPanelButton>(this.colorBtnTemplate, null, null, null, null, true, 10, 10000, null);
			}
			return this._colorBtnPool;
		}
	}

	// Token: 0x06000D5B RID: 3419 RVA: 0x00037224 File Offset: 0x00035424
	private void OnEnable()
	{
		this.Setup();
		MapMarkerManager.OnColorChanged = (Action<Color>)Delegate.Combine(MapMarkerManager.OnColorChanged, new Action<Color>(this.OnColorChanged));
		MapMarkerManager.OnIconChanged = (Action<int>)Delegate.Combine(MapMarkerManager.OnIconChanged, new Action<int>(this.OnIconChanged));
	}

	// Token: 0x06000D5C RID: 3420 RVA: 0x00037278 File Offset: 0x00035478
	private void OnDisable()
	{
		MapMarkerManager.OnColorChanged = (Action<Color>)Delegate.Remove(MapMarkerManager.OnColorChanged, new Action<Color>(this.OnColorChanged));
		MapMarkerManager.OnIconChanged = (Action<int>)Delegate.Remove(MapMarkerManager.OnIconChanged, new Action<int>(this.OnIconChanged));
	}

	// Token: 0x06000D5D RID: 3421 RVA: 0x000372C5 File Offset: 0x000354C5
	private void OnIconChanged(int obj)
	{
		this.Setup();
	}

	// Token: 0x06000D5E RID: 3422 RVA: 0x000372CD File Offset: 0x000354CD
	private void OnColorChanged(Color color)
	{
		this.Setup();
	}

	// Token: 0x06000D5F RID: 3423 RVA: 0x000372D8 File Offset: 0x000354D8
	private void Setup()
	{
		if (MapMarkerManager.Instance == null)
		{
			return;
		}
		this.IconBtnPool.ReleaseAll();
		this.ColorBtnPool.ReleaseAll();
		Color[] array = this.colors;
		for (int i = 0; i < array.Length; i++)
		{
			Color cur = array[i];
			MapMarkerPanelButton mapMarkerPanelButton = this.ColorBtnPool.Get(null);
			mapMarkerPanelButton.Image.color = cur;
			mapMarkerPanelButton.Setup(delegate
			{
				MapMarkerManager.SelectColor(cur);
			}, cur == MapMarkerManager.SelectedColor);
		}
		for (int j = 0; j < this.Icons.Count; j++)
		{
			Sprite sprite = this.Icons[j];
			if (!(sprite == null))
			{
				MapMarkerPanelButton mapMarkerPanelButton2 = this.IconBtnPool.Get(null);
				Image image = mapMarkerPanelButton2.Image;
				image.sprite = sprite;
				image.color = MapMarkerManager.SelectedColor;
				int index = j;
				mapMarkerPanelButton2.Setup(delegate
				{
					MapMarkerManager.SelectIcon(index);
				}, index == MapMarkerManager.SelectedIconIndex);
			}
		}
	}

	// Token: 0x04000B5B RID: 2907
	[SerializeField]
	private Color[] colors;

	// Token: 0x04000B5C RID: 2908
	[SerializeField]
	private MapMarkerPanelButton iconBtnTemplate;

	// Token: 0x04000B5D RID: 2909
	[SerializeField]
	private MapMarkerPanelButton colorBtnTemplate;

	// Token: 0x04000B5E RID: 2910
	private PrefabPool<MapMarkerPanelButton> _iconBtnPool;

	// Token: 0x04000B5F RID: 2911
	private PrefabPool<MapMarkerPanelButton> _colorBtnPool;
}
