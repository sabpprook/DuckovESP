using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using Duckov.Utilities;
using ItemStatsSystem;
using UnityEngine;

// Token: 0x020001A5 RID: 421
[CreateAssetMenu]
public class DecomposeDatabase : ScriptableObject
{
	// Token: 0x17000241 RID: 577
	// (get) Token: 0x06000C75 RID: 3189 RVA: 0x00034855 File Offset: 0x00032A55
	public static DecomposeDatabase Instance
	{
		get
		{
			return GameplayDataSettings.DecomposeDatabase;
		}
	}

	// Token: 0x17000242 RID: 578
	// (get) Token: 0x06000C76 RID: 3190 RVA: 0x0003485C File Offset: 0x00032A5C
	private Dictionary<int, DecomposeFormula> Dic
	{
		get
		{
			if (this._dic == null)
			{
				this.RebuildDictionary();
			}
			return this._dic;
		}
	}

	// Token: 0x06000C77 RID: 3191 RVA: 0x00034874 File Offset: 0x00032A74
	public void RebuildDictionary()
	{
		this._dic = new Dictionary<int, DecomposeFormula>();
		foreach (DecomposeFormula decomposeFormula in this.entries)
		{
			this._dic[decomposeFormula.item] = decomposeFormula;
		}
	}

	// Token: 0x06000C78 RID: 3192 RVA: 0x000348BC File Offset: 0x00032ABC
	public DecomposeFormula GetFormula(int itemTypeID)
	{
		DecomposeFormula decomposeFormula;
		if (!this.Dic.TryGetValue(itemTypeID, out decomposeFormula))
		{
			return default(DecomposeFormula);
		}
		return decomposeFormula;
	}

	// Token: 0x06000C79 RID: 3193 RVA: 0x000348E4 File Offset: 0x00032AE4
	public static UniTask<bool> Decompose(Item item, int count)
	{
		DecomposeDatabase.<Decompose>d__8 <Decompose>d__;
		<Decompose>d__.<>t__builder = AsyncUniTaskMethodBuilder<bool>.Create();
		<Decompose>d__.item = item;
		<Decompose>d__.count = count;
		<Decompose>d__.<>1__state = -1;
		<Decompose>d__.<>t__builder.Start<DecomposeDatabase.<Decompose>d__8>(ref <Decompose>d__);
		return <Decompose>d__.<>t__builder.Task;
	}

	// Token: 0x06000C7A RID: 3194 RVA: 0x0003492F File Offset: 0x00032B2F
	public static bool CanDecompose(int itemTypeID)
	{
		return !(DecomposeDatabase.Instance == null) && DecomposeDatabase.Instance.GetFormula(itemTypeID).valid;
	}

	// Token: 0x06000C7B RID: 3195 RVA: 0x00034950 File Offset: 0x00032B50
	public static bool CanDecompose(Item item)
	{
		return !(item == null) && DecomposeDatabase.CanDecompose(item.TypeID);
	}

	// Token: 0x06000C7C RID: 3196 RVA: 0x00034968 File Offset: 0x00032B68
	public static DecomposeFormula GetDecomposeFormula(int itemTypeID)
	{
		if (DecomposeDatabase.Instance == null)
		{
			return default(DecomposeFormula);
		}
		return DecomposeDatabase.Instance.GetFormula(itemTypeID);
	}

	// Token: 0x06000C7D RID: 3197 RVA: 0x00034997 File Offset: 0x00032B97
	public void SetData(List<DecomposeFormula> formulas)
	{
		this.entries = formulas.ToArray();
	}

	// Token: 0x04000AD1 RID: 2769
	[SerializeField]
	private DecomposeFormula[] entries;

	// Token: 0x04000AD2 RID: 2770
	private Dictionary<int, DecomposeFormula> _dic;
}
