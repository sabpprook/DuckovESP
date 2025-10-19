using System;
using System.Linq;
using Duckov.Utilities;
using UnityEngine;

// Token: 0x0200009A RID: 154
public class TagUtilities
{
	// Token: 0x06000528 RID: 1320 RVA: 0x0001750C File Offset: 0x0001570C
	public static Tag TagFromString(string name)
	{
		name = name.Trim();
		Tag tag = GameplayDataSettings.Tags.AllTags.FirstOrDefault((Tag e) => e != null && e.name == name);
		if (tag == null)
		{
			Debug.LogError("未找到Tag: " + name);
		}
		return tag;
	}
}
