using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Duckov.Economy;
using ItemStatsSystem;
using Saves;
using UnityEngine;

// Token: 0x020001A4 RID: 420
public class CraftingManager : MonoBehaviour
{
	// Token: 0x1700023E RID: 574
	// (get) Token: 0x06000C67 RID: 3175 RVA: 0x0003457D File Offset: 0x0003277D
	private static CraftingFormulaCollection FormulaCollection
	{
		get
		{
			return CraftingFormulaCollection.Instance;
		}
	}

	// Token: 0x1700023F RID: 575
	// (get) Token: 0x06000C68 RID: 3176 RVA: 0x00034584 File Offset: 0x00032784
	// (set) Token: 0x06000C69 RID: 3177 RVA: 0x0003458B File Offset: 0x0003278B
	public static CraftingManager Instance { get; private set; }

	// Token: 0x06000C6A RID: 3178 RVA: 0x00034593 File Offset: 0x00032793
	private void Awake()
	{
		CraftingManager.Instance = this;
		this.Load();
		SavesSystem.OnCollectSaveData += this.Save;
	}

	// Token: 0x06000C6B RID: 3179 RVA: 0x000345B2 File Offset: 0x000327B2
	private void OnDestroy()
	{
		SavesSystem.OnCollectSaveData -= this.Save;
	}

	// Token: 0x06000C6C RID: 3180 RVA: 0x000345C5 File Offset: 0x000327C5
	private void Save()
	{
		SavesSystem.Save<List<string>>("Crafting/UnlockedFormulaIDs", this.unlockedFormulaIDs);
	}

	// Token: 0x06000C6D RID: 3181 RVA: 0x000345D8 File Offset: 0x000327D8
	private void Load()
	{
		this.unlockedFormulaIDs = SavesSystem.Load<List<string>>("Crafting/UnlockedFormulaIDs");
		if (this.unlockedFormulaIDs == null)
		{
			this.unlockedFormulaIDs = new List<string>();
		}
		foreach (CraftingFormula craftingFormula in CraftingManager.FormulaCollection.Entries)
		{
			if (craftingFormula.unlockByDefault && !this.unlockedFormulaIDs.Contains(craftingFormula.id))
			{
				this.unlockedFormulaIDs.Add(craftingFormula.id);
			}
		}
		this.unlockedFormulaIDs.Sort();
	}

	// Token: 0x17000240 RID: 576
	// (get) Token: 0x06000C6E RID: 3182 RVA: 0x0003467C File Offset: 0x0003287C
	public static IEnumerable<string> UnlockedFormulaIDs
	{
		get
		{
			if (!(CraftingManager.Instance == null))
			{
				foreach (CraftingFormula craftingFormula in CraftingFormulaCollection.Instance.Entries)
				{
					if (CraftingManager.IsFormulaUnlocked(craftingFormula.id))
					{
						yield return craftingFormula.id;
					}
				}
				IEnumerator<CraftingFormula> enumerator = null;
			}
			yield break;
			yield break;
		}
	}

	// Token: 0x06000C6F RID: 3183 RVA: 0x00034688 File Offset: 0x00032888
	public static void UnlockFormula(string formulaID)
	{
		if (CraftingManager.Instance == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(formulaID))
		{
			Debug.LogError("Invalid formula ID");
			return;
		}
		CraftingFormula craftingFormula = CraftingManager.FormulaCollection.Entries.FirstOrDefault((CraftingFormula e) => e.id == formulaID);
		if (!craftingFormula.IDValid)
		{
			Debug.LogError("Invalid formula ID: " + formulaID);
			return;
		}
		if (craftingFormula.unlockByDefault)
		{
			Debug.LogError("Formula is unlocked by default: " + formulaID);
			return;
		}
		if (CraftingManager.Instance.unlockedFormulaIDs.Contains(formulaID))
		{
			return;
		}
		CraftingManager.Instance.unlockedFormulaIDs.Add(formulaID);
		Action<string> onFormulaUnlocked = CraftingManager.OnFormulaUnlocked;
		if (onFormulaUnlocked == null)
		{
			return;
		}
		onFormulaUnlocked(formulaID);
	}

	// Token: 0x06000C70 RID: 3184 RVA: 0x00034764 File Offset: 0x00032964
	private async UniTask<List<Item>> Craft(CraftingFormula formula)
	{
		List<Item> list;
		if (!formula.cost.Enough)
		{
			list = null;
		}
		else
		{
			Cost cost = new Cost(new ValueTuple<int, long>[]
			{
				new ValueTuple<int, long>(formula.result.id, (long)formula.result.amount)
			});
			if (!formula.cost.Pay(true, true))
			{
				list = null;
			}
			else
			{
				List<Item> generatedBuffer = new List<Item>();
				await cost.Return(false, true, 1, generatedBuffer);
				foreach (Item item in generatedBuffer)
				{
					if (!(item == null))
					{
						Action<CraftingFormula, Item> onItemCrafted = CraftingManager.OnItemCrafted;
						if (onItemCrafted != null)
						{
							onItemCrafted(formula, item);
						}
					}
				}
				list = generatedBuffer;
			}
		}
		return list;
	}

	// Token: 0x06000C71 RID: 3185 RVA: 0x000347A8 File Offset: 0x000329A8
	public async UniTask<List<Item>> Craft(string id)
	{
		CraftingFormula craftingFormula;
		List<Item> list;
		if (!CraftingFormulaCollection.TryGetFormula(id, out craftingFormula))
		{
			list = null;
		}
		else
		{
			list = await this.Craft(craftingFormula);
		}
		return list;
	}

	// Token: 0x06000C72 RID: 3186 RVA: 0x000347F3 File Offset: 0x000329F3
	internal static bool IsFormulaUnlocked(string value)
	{
		return !(CraftingManager.Instance == null) && !string.IsNullOrEmpty(value) && CraftingManager.Instance.unlockedFormulaIDs.Contains(value);
	}

	// Token: 0x06000C73 RID: 3187 RVA: 0x00034820 File Offset: 0x00032A20
	internal static CraftingFormula GetFormula(string id)
	{
		CraftingFormula craftingFormula;
		if (CraftingFormulaCollection.TryGetFormula(id, out craftingFormula))
		{
			return craftingFormula;
		}
		return default(CraftingFormula);
	}

	// Token: 0x04000ACC RID: 2764
	public static Action<CraftingFormula, Item> OnItemCrafted;

	// Token: 0x04000ACD RID: 2765
	public static Action<string> OnFormulaUnlocked;

	// Token: 0x04000ACF RID: 2767
	private const string SaveKey = "Crafting/UnlockedFormulaIDs";

	// Token: 0x04000AD0 RID: 2768
	private List<string> unlockedFormulaIDs = new List<string>();
}
