using System;
using Duckov.Quests;
using Duckov.Scenes;

// Token: 0x0200011C RID: 284
public class RequireInLevelDataBool : Condition
{
	// Token: 0x06000975 RID: 2421 RVA: 0x00029454 File Offset: 0x00027654
	public override bool Evaluate()
	{
		if (!MultiSceneCore.Instance)
		{
			return false;
		}
		if (!this.keyHashInited)
		{
			this.InitKeyHash();
		}
		object obj;
		return !this.isEmptyString && (MultiSceneCore.Instance.inLevelData.TryGetValue(this.keyHash, out obj) && obj is bool) && (bool)obj;
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x000294B0 File Offset: 0x000276B0
	private void InitKeyHash()
	{
		if (this.keyString == "")
		{
			this.isEmptyString = true;
		}
		this.keyHash = this.keyString.GetHashCode();
		this.keyHashInited = true;
	}

	// Token: 0x0400085A RID: 2138
	public string keyString = "";

	// Token: 0x0400085B RID: 2139
	private int keyHash = -1;

	// Token: 0x0400085C RID: 2140
	private bool keyHashInited;

	// Token: 0x0400085D RID: 2141
	private bool isEmptyString;
}
