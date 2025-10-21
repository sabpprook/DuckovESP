using System;
using Duckov;
using ItemStatsSystem;
using UnityEngine;

// Token: 0x0200012F RID: 303
public abstract class SkillBase : MonoBehaviour
{
	// Token: 0x17000205 RID: 517
	// (get) Token: 0x060009D4 RID: 2516 RVA: 0x0002A205 File Offset: 0x00028405
	public float LastReleaseTime
	{
		get
		{
			return this.lastReleaseTime;
		}
	}

	// Token: 0x17000206 RID: 518
	// (get) Token: 0x060009D5 RID: 2517 RVA: 0x0002A20D File Offset: 0x0002840D
	public SkillContext SkillContext
	{
		get
		{
			return this.skillContext;
		}
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x0002A218 File Offset: 0x00028418
	public void ReleaseSkill(SkillReleaseContext releaseContext, CharacterMainControl from)
	{
		this.lastReleaseTime = Time.time;
		this.skillReleaseContext = releaseContext;
		this.fromCharacter = from;
		this.fromCharacter.UseStamina(this.staminaCost);
		if (this.hasReleaseSound && this.fromCharacter != null && this.onReleaseSound != "")
		{
			AudioManager.Post(this.onReleaseSound, from.gameObject);
		}
		this.OnRelease();
		Action onSkillReleasedEvent = this.OnSkillReleasedEvent;
		if (onSkillReleasedEvent == null)
		{
			return;
		}
		onSkillReleasedEvent();
	}

	// Token: 0x060009D7 RID: 2519
	public abstract void OnRelease();

	// Token: 0x04000898 RID: 2200
	public bool hasReleaseSound;

	// Token: 0x04000899 RID: 2201
	public string onReleaseSound;

	// Token: 0x0400089A RID: 2202
	public Sprite icon;

	// Token: 0x0400089B RID: 2203
	public float staminaCost = 10f;

	// Token: 0x0400089C RID: 2204
	public float coolDownTime = 1f;

	// Token: 0x0400089D RID: 2205
	private float lastReleaseTime = -999f;

	// Token: 0x0400089E RID: 2206
	[SerializeField]
	protected SkillContext skillContext;

	// Token: 0x0400089F RID: 2207
	protected SkillReleaseContext skillReleaseContext;

	// Token: 0x040008A0 RID: 2208
	protected CharacterMainControl fromCharacter;

	// Token: 0x040008A1 RID: 2209
	[HideInInspector]
	public Item fromItem;

	// Token: 0x040008A2 RID: 2210
	public Action OnSkillReleasedEvent;
}
