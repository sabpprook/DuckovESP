using System;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;

// Token: 0x02000061 RID: 97
public class HalfObsticle : MonoBehaviour
{
	// Token: 0x06000393 RID: 915 RVA: 0x0000FC7C File Offset: 0x0000DE7C
	private void Awake()
	{
		this.outline.enabled = false;
		this.defaultVisuals.SetActive(true);
		this.deadVisuals.SetActive(false);
		this.health.OnDeadEvent += this.Dead;
		if (this.airWallCollider)
		{
			this.airWallCollider.gameObject.SetActive(true);
		}
	}

	// Token: 0x06000394 RID: 916 RVA: 0x0000FCE2 File Offset: 0x0000DEE2
	private void OnValidate()
	{
	}

	// Token: 0x06000395 RID: 917 RVA: 0x0000FCE4 File Offset: 0x0000DEE4
	public void Dead(DamageInfo dmgInfo)
	{
		if (this.dead)
		{
			return;
		}
		this.dead = true;
		this.defaultVisuals.SetActive(false);
		this.deadVisuals.SetActive(true);
	}

	// Token: 0x06000396 RID: 918 RVA: 0x0000FD10 File Offset: 0x0000DF10
	public void OnTriggerEnter(Collider other)
	{
		CharacterMainControl component = other.GetComponent<CharacterMainControl>();
		if (!component)
		{
			return;
		}
		component.AddnearByHalfObsticles(this.parts);
		if (component.IsMainCharacter)
		{
			this.outline.enabled = true;
		}
	}

	// Token: 0x06000397 RID: 919 RVA: 0x0000FD50 File Offset: 0x0000DF50
	public void OnTriggerExit(Collider other)
	{
		CharacterMainControl component = other.GetComponent<CharacterMainControl>();
		if (!component)
		{
			return;
		}
		component.RemoveNearByHalfObsticles(this.parts);
		if (component.IsMainCharacter)
		{
			this.outline.enabled = false;
		}
	}

	// Token: 0x040002B7 RID: 695
	public Outlinable outline;

	// Token: 0x040002B8 RID: 696
	public HealthSimpleBase health;

	// Token: 0x040002B9 RID: 697
	public List<GameObject> parts;

	// Token: 0x040002BA RID: 698
	public GameObject defaultVisuals;

	// Token: 0x040002BB RID: 699
	public GameObject deadVisuals;

	// Token: 0x040002BC RID: 700
	public Collider airWallCollider;

	// Token: 0x040002BD RID: 701
	private bool dead;
}
