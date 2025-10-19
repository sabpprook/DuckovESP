using System;
using Duckov.Quests;
using Duckov.Scenes;

// Token: 0x02000115 RID: 277
public class Condition_HasBeenToScene : Condition
{
	// Token: 0x06000966 RID: 2406 RVA: 0x0002929D File Offset: 0x0002749D
	public override bool Evaluate()
	{
		return MultiSceneCore.GetVisited(this.sceneID);
	}

	// Token: 0x0400084F RID: 2127
	[SceneID]
	public string sceneID;
}
