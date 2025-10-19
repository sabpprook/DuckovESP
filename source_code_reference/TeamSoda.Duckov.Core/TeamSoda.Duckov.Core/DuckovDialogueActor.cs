using System;
using System.Collections.Generic;
using NodeCanvas.DialogueTrees;
using UnityEngine;

// Token: 0x020001AF RID: 431
public class DuckovDialogueActor : MonoBehaviour, IDialogueActor
{
	// Token: 0x1700024F RID: 591
	// (get) Token: 0x06000CC4 RID: 3268 RVA: 0x0003567D File Offset: 0x0003387D
	private static List<DuckovDialogueActor> ActiveActors
	{
		get
		{
			if (DuckovDialogueActor._activeActors == null)
			{
				DuckovDialogueActor._activeActors = new List<DuckovDialogueActor>();
			}
			return DuckovDialogueActor._activeActors;
		}
	}

	// Token: 0x06000CC5 RID: 3269 RVA: 0x00035695 File Offset: 0x00033895
	public static void Register(DuckovDialogueActor actor)
	{
		if (DuckovDialogueActor.ActiveActors.Contains(actor))
		{
			Debug.Log("Actor " + actor.nameKey + " 在重复注册", actor);
			return;
		}
		DuckovDialogueActor.ActiveActors.Add(actor);
	}

	// Token: 0x06000CC6 RID: 3270 RVA: 0x000356CB File Offset: 0x000338CB
	public static void Unregister(DuckovDialogueActor actor)
	{
		DuckovDialogueActor.ActiveActors.Remove(actor);
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x000356DC File Offset: 0x000338DC
	public static DuckovDialogueActor Get(string id)
	{
		return DuckovDialogueActor.ActiveActors.Find((DuckovDialogueActor e) => e.ID == id);
	}

	// Token: 0x17000250 RID: 592
	// (get) Token: 0x06000CC8 RID: 3272 RVA: 0x0003570C File Offset: 0x0003390C
	public string ID
	{
		get
		{
			return this.id;
		}
	}

	// Token: 0x17000251 RID: 593
	// (get) Token: 0x06000CC9 RID: 3273 RVA: 0x00035714 File Offset: 0x00033914
	public Vector3 Offset
	{
		get
		{
			return this.offset;
		}
	}

	// Token: 0x17000252 RID: 594
	// (get) Token: 0x06000CCA RID: 3274 RVA: 0x0003571C File Offset: 0x0003391C
	public string NameKey
	{
		get
		{
			return this.nameKey;
		}
	}

	// Token: 0x17000253 RID: 595
	// (get) Token: 0x06000CCB RID: 3275 RVA: 0x00035724 File Offset: 0x00033924
	public Texture2D portrait
	{
		get
		{
			return null;
		}
	}

	// Token: 0x17000254 RID: 596
	// (get) Token: 0x06000CCC RID: 3276 RVA: 0x00035727 File Offset: 0x00033927
	public Sprite portraitSprite
	{
		get
		{
			return this._portraitSprite;
		}
	}

	// Token: 0x17000255 RID: 597
	// (get) Token: 0x06000CCD RID: 3277 RVA: 0x00035730 File Offset: 0x00033930
	public Color dialogueColor
	{
		get
		{
			return default(Color);
		}
	}

	// Token: 0x17000256 RID: 598
	// (get) Token: 0x06000CCE RID: 3278 RVA: 0x00035748 File Offset: 0x00033948
	public Vector3 dialoguePosition
	{
		get
		{
			return default(Vector3);
		}
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x0003575E File Offset: 0x0003395E
	private void OnEnable()
	{
		DuckovDialogueActor.Register(this);
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x00035766 File Offset: 0x00033966
	private void OnDisable()
	{
		DuckovDialogueActor.Unregister(this);
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x00035776 File Offset: 0x00033976
	string IDialogueActor.get_name()
	{
		return base.name;
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x0003577E File Offset: 0x0003397E
	Transform IDialogueActor.get_transform()
	{
		return base.transform;
	}

	// Token: 0x04000B0B RID: 2827
	private static List<DuckovDialogueActor> _activeActors;

	// Token: 0x04000B0C RID: 2828
	[SerializeField]
	private string id;

	// Token: 0x04000B0D RID: 2829
	[SerializeField]
	private Sprite _portraitSprite;

	// Token: 0x04000B0E RID: 2830
	[SerializeField]
	[LocalizationKey("Default")]
	private string nameKey;

	// Token: 0x04000B0F RID: 2831
	[SerializeField]
	private Vector3 offset;
}
