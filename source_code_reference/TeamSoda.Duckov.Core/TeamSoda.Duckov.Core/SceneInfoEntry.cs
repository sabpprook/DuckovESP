using System;
using Eflatun.SceneReference;
using SodaCraft.Localizations;
using UnityEngine;

// Token: 0x02000127 RID: 295
[Serializable]
public class SceneInfoEntry
{
	// Token: 0x060009A2 RID: 2466 RVA: 0x00029A5C File Offset: 0x00027C5C
	public SceneInfoEntry()
	{
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x00029A64 File Offset: 0x00027C64
	public SceneInfoEntry(string id, SceneReference sceneReference)
	{
		this.id = id;
		this.sceneReference = sceneReference;
	}

	// Token: 0x170001F7 RID: 503
	// (get) Token: 0x060009A4 RID: 2468 RVA: 0x00029A7A File Offset: 0x00027C7A
	public int BuildIndex
	{
		get
		{
			if (this.sceneReference.UnsafeReason != SceneReferenceUnsafeReason.None)
			{
				return -1;
			}
			return this.sceneReference.BuildIndex;
		}
	}

	// Token: 0x170001F8 RID: 504
	// (get) Token: 0x060009A5 RID: 2469 RVA: 0x00029A96 File Offset: 0x00027C96
	public string ID
	{
		get
		{
			return this.id;
		}
	}

	// Token: 0x170001F9 RID: 505
	// (get) Token: 0x060009A6 RID: 2470 RVA: 0x00029A9E File Offset: 0x00027C9E
	public SceneReference SceneReference
	{
		get
		{
			return this.sceneReference;
		}
	}

	// Token: 0x170001FA RID: 506
	// (get) Token: 0x060009A7 RID: 2471 RVA: 0x00029AA6 File Offset: 0x00027CA6
	public string Description
	{
		get
		{
			return this.description.ToPlainText();
		}
	}

	// Token: 0x170001FB RID: 507
	// (get) Token: 0x060009A8 RID: 2472 RVA: 0x00029AB3 File Offset: 0x00027CB3
	public string DisplayName
	{
		get
		{
			if (string.IsNullOrEmpty(this.displayName))
			{
				return this.id;
			}
			return this.displayName.ToPlainText();
		}
	}

	// Token: 0x170001FC RID: 508
	// (get) Token: 0x060009A9 RID: 2473 RVA: 0x00029AD4 File Offset: 0x00027CD4
	public string DisplayNameRaw
	{
		get
		{
			if (string.IsNullOrEmpty(this.displayName))
			{
				return this.id;
			}
			return this.displayName;
		}
	}

	// Token: 0x170001FD RID: 509
	// (get) Token: 0x060009AA RID: 2474 RVA: 0x00029AF0 File Offset: 0x00027CF0
	public bool IsLoaded
	{
		get
		{
			return this.sceneReference != null && this.sceneReference.UnsafeReason == SceneReferenceUnsafeReason.None && this.sceneReference.LoadedScene.isLoaded;
		}
	}

	// Token: 0x0400086D RID: 2157
	[SerializeField]
	private string id;

	// Token: 0x0400086E RID: 2158
	[SerializeField]
	private SceneReference sceneReference;

	// Token: 0x0400086F RID: 2159
	[LocalizationKey("Default")]
	[SerializeField]
	private string displayName;

	// Token: 0x04000870 RID: 2160
	[LocalizationKey("Default")]
	[SerializeField]
	private string description;
}
