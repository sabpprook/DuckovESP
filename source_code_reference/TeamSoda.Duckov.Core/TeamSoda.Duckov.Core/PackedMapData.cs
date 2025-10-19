using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020001C1 RID: 449
public class PackedMapData : ScriptableObject, IMiniMapDataProvider
{
	// Token: 0x17000278 RID: 632
	// (get) Token: 0x06000D61 RID: 3425 RVA: 0x000373F8 File Offset: 0x000355F8
	public Sprite CombinedSprite
	{
		get
		{
			return this.combinedSprite;
		}
	}

	// Token: 0x17000279 RID: 633
	// (get) Token: 0x06000D62 RID: 3426 RVA: 0x00037400 File Offset: 0x00035600
	public float PixelSize
	{
		get
		{
			return this.pixelSize;
		}
	}

	// Token: 0x1700027A RID: 634
	// (get) Token: 0x06000D63 RID: 3427 RVA: 0x00037408 File Offset: 0x00035608
	public Vector3 CombinedCenter
	{
		get
		{
			return this.combinedCenter;
		}
	}

	// Token: 0x1700027B RID: 635
	// (get) Token: 0x06000D64 RID: 3428 RVA: 0x00037410 File Offset: 0x00035610
	public List<IMiniMapEntry> Maps
	{
		get
		{
			return this.maps.ToList<IMiniMapEntry>();
		}
	}

	// Token: 0x06000D65 RID: 3429 RVA: 0x00037420 File Offset: 0x00035620
	internal void Setup(IMiniMapDataProvider origin)
	{
		this.combinedSprite = origin.CombinedSprite;
		this.pixelSize = origin.PixelSize;
		this.combinedCenter = origin.CombinedCenter;
		this.maps.Clear();
		foreach (IMiniMapEntry miniMapEntry in origin.Maps)
		{
			PackedMapData.Entry entry = new PackedMapData.Entry(miniMapEntry.Sprite, miniMapEntry.PixelSize, miniMapEntry.Offset, miniMapEntry.SceneID, miniMapEntry.Hide, miniMapEntry.NoSignal);
			this.maps.Add(entry);
		}
	}

	// Token: 0x04000B60 RID: 2912
	[SerializeField]
	private Sprite combinedSprite;

	// Token: 0x04000B61 RID: 2913
	[SerializeField]
	private float pixelSize;

	// Token: 0x04000B62 RID: 2914
	[SerializeField]
	private Vector3 combinedCenter;

	// Token: 0x04000B63 RID: 2915
	[SerializeField]
	private List<PackedMapData.Entry> maps = new List<PackedMapData.Entry>();

	// Token: 0x020004D0 RID: 1232
	[Serializable]
	public class Entry : IMiniMapEntry
	{
		// Token: 0x17000742 RID: 1858
		// (get) Token: 0x060026F5 RID: 9973 RVA: 0x0008D1B9 File Offset: 0x0008B3B9
		public Sprite Sprite
		{
			get
			{
				return this.sprite;
			}
		}

		// Token: 0x17000743 RID: 1859
		// (get) Token: 0x060026F6 RID: 9974 RVA: 0x0008D1C1 File Offset: 0x0008B3C1
		public float PixelSize
		{
			get
			{
				return this.pixelSize;
			}
		}

		// Token: 0x17000744 RID: 1860
		// (get) Token: 0x060026F7 RID: 9975 RVA: 0x0008D1C9 File Offset: 0x0008B3C9
		public Vector2 Offset
		{
			get
			{
				return this.offset;
			}
		}

		// Token: 0x17000745 RID: 1861
		// (get) Token: 0x060026F8 RID: 9976 RVA: 0x0008D1D1 File Offset: 0x0008B3D1
		public string SceneID
		{
			get
			{
				return this.sceneID;
			}
		}

		// Token: 0x17000746 RID: 1862
		// (get) Token: 0x060026F9 RID: 9977 RVA: 0x0008D1D9 File Offset: 0x0008B3D9
		public bool Hide
		{
			get
			{
				return this.hide;
			}
		}

		// Token: 0x17000747 RID: 1863
		// (get) Token: 0x060026FA RID: 9978 RVA: 0x0008D1E1 File Offset: 0x0008B3E1
		public bool NoSignal
		{
			get
			{
				return this.noSignal;
			}
		}

		// Token: 0x060026FB RID: 9979 RVA: 0x0008D1E9 File Offset: 0x0008B3E9
		public Entry()
		{
		}

		// Token: 0x060026FC RID: 9980 RVA: 0x0008D1F1 File Offset: 0x0008B3F1
		public Entry(Sprite sprite, float pixelSize, Vector2 offset, string sceneID, bool hide, bool noSignal)
		{
			this.sprite = sprite;
			this.pixelSize = pixelSize;
			this.offset = offset;
			this.sceneID = sceneID;
			this.hide = hide;
			this.noSignal = noSignal;
		}

		// Token: 0x04001CD5 RID: 7381
		[SerializeField]
		private Sprite sprite;

		// Token: 0x04001CD6 RID: 7382
		[SerializeField]
		private float pixelSize;

		// Token: 0x04001CD7 RID: 7383
		[SerializeField]
		private Vector2 offset;

		// Token: 0x04001CD8 RID: 7384
		[SerializeField]
		private string sceneID;

		// Token: 0x04001CD9 RID: 7385
		[SerializeField]
		private bool hide;

		// Token: 0x04001CDA RID: 7386
		[SerializeField]
		private bool noSignal;
	}
}
