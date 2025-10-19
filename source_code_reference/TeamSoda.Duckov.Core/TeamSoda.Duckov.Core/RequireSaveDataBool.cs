using System;
using Duckov.Quests;
using Saves;
using UnityEngine;

// Token: 0x0200011D RID: 285
public class RequireSaveDataBool : Condition
{
	// Token: 0x06000978 RID: 2424 RVA: 0x00029500 File Offset: 0x00027700
	public override bool Evaluate()
	{
		bool flag = SavesSystem.Load<bool>(this.key);
		Debug.Log(string.Format("Load bool:{0}  value:{1}", this.key, flag));
		return flag == this.requireValue;
	}

	// Token: 0x0400085E RID: 2142
	[SerializeField]
	private string key;

	// Token: 0x0400085F RID: 2143
	[SerializeField]
	private bool requireValue;
}
