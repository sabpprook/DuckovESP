using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.PerkTrees;
using UnityEngine;

// Token: 0x020001E1 RID: 481
public class PerkTreeManager : MonoBehaviour
{
	// Token: 0x1700029A RID: 666
	// (get) Token: 0x06000E3F RID: 3647 RVA: 0x000395A6 File Offset: 0x000377A6
	public static PerkTreeManager Instance
	{
		get
		{
			return PerkTreeManager.instance;
		}
	}

	// Token: 0x06000E40 RID: 3648 RVA: 0x000395AD File Offset: 0x000377AD
	private void Awake()
	{
		if (PerkTreeManager.instance == null)
		{
			PerkTreeManager.instance = this;
			return;
		}
		Debug.LogError("检测到多个PerkTreeManager");
	}

	// Token: 0x06000E41 RID: 3649 RVA: 0x000395D0 File Offset: 0x000377D0
	public static PerkTree GetPerkTree(string id)
	{
		if (PerkTreeManager.instance == null)
		{
			return null;
		}
		PerkTree perkTree = PerkTreeManager.instance.perkTrees.FirstOrDefault((PerkTree e) => e != null && e.ID == id);
		if (perkTree == null)
		{
			Debug.LogError("未找到PerkTree id:" + id);
		}
		return perkTree;
	}

	// Token: 0x04000BC4 RID: 3012
	private static PerkTreeManager instance;

	// Token: 0x04000BC5 RID: 3013
	public List<PerkTree> perkTrees;
}
