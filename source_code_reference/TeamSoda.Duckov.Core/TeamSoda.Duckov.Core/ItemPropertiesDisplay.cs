using System;
using Duckov.UI;
using Duckov.Utilities;
using ItemStatsSystem;
using UnityEngine;

// Token: 0x02000155 RID: 341
public class ItemPropertiesDisplay : MonoBehaviour
{
	// Token: 0x17000210 RID: 528
	// (get) Token: 0x06000A7A RID: 2682 RVA: 0x0002DD10 File Offset: 0x0002BF10
	private PrefabPool<LabelAndValue> EntryPool
	{
		get
		{
			if (this._entryPool == null)
			{
				this._entryPool = new PrefabPool<LabelAndValue>(this.entryTemplate, null, null, null, null, true, 10, 10000, null);
			}
			return this._entryPool;
		}
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x0002DD49 File Offset: 0x0002BF49
	private void Awake()
	{
		this.entryTemplate.gameObject.SetActive(false);
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x0002DD5C File Offset: 0x0002BF5C
	internal void Setup(Item targetItem)
	{
		this.EntryPool.ReleaseAll();
		if (targetItem == null)
		{
			return;
		}
		foreach (ValueTuple<string, string, Polarity> valueTuple in targetItem.GetPropertyValueTextPair())
		{
			this.EntryPool.Get(null).Setup(valueTuple.Item1, valueTuple.Item2, valueTuple.Item3);
		}
	}

	// Token: 0x04000921 RID: 2337
	[SerializeField]
	private LabelAndValue entryTemplate;

	// Token: 0x04000922 RID: 2338
	private PrefabPool<LabelAndValue> _entryPool;
}
