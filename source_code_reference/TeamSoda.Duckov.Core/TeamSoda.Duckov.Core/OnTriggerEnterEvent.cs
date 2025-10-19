using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000DE RID: 222
public class OnTriggerEnterEvent : MonoBehaviour
{
	// Token: 0x17000149 RID: 329
	// (get) Token: 0x06000713 RID: 1811 RVA: 0x0001FDD3 File Offset: 0x0001DFD3
	private bool hideLayerMask
	{
		get
		{
			return this.onlyMainCharacter || this.filterByTeam;
		}
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x0001FDE5 File Offset: 0x0001DFE5
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x0001FDF0 File Offset: 0x0001DFF0
	public void Init()
	{
		Collider component = base.GetComponent<Collider>();
		if (component)
		{
			component.isTrigger = true;
		}
		if (this.filterByTeam)
		{
			this.layerMask = 1 << LayerMask.NameToLayer("Character");
		}
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x0001FE35 File Offset: 0x0001E035
	private void OnCollisionEnter(Collision collision)
	{
		this.OnEvent(collision.gameObject, true);
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x0001FE44 File Offset: 0x0001E044
	private void OnCollisionExit(Collision collision)
	{
		this.OnEvent(collision.gameObject, false);
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x0001FE53 File Offset: 0x0001E053
	private void OnTriggerEnter(Collider other)
	{
		this.OnEvent(other.gameObject, true);
	}

	// Token: 0x06000719 RID: 1817 RVA: 0x0001FE62 File Offset: 0x0001E062
	private void OnTriggerExit(Collider other)
	{
		this.OnEvent(other.gameObject, false);
	}

	// Token: 0x0600071A RID: 1818 RVA: 0x0001FE74 File Offset: 0x0001E074
	private void OnEvent(GameObject other, bool enter)
	{
		if (this.triggerOnce && this.triggered)
		{
			return;
		}
		if (this.onlyMainCharacter)
		{
			if (CharacterMainControl.Main == null || other != CharacterMainControl.Main.gameObject)
			{
				return;
			}
		}
		else
		{
			if (((1 << other.layer) | this.layerMask) != this.layerMask)
			{
				return;
			}
			if (this.filterByTeam)
			{
				CharacterMainControl component = other.GetComponent<CharacterMainControl>();
				if (!component)
				{
					return;
				}
				Teams team = component.Team;
				if (!Team.IsEnemy(this.selfTeam, team))
				{
					return;
				}
			}
		}
		this.triggered = true;
		if (enter)
		{
			UnityEvent doOnTriggerEnter = this.DoOnTriggerEnter;
			if (doOnTriggerEnter == null)
			{
				return;
			}
			doOnTriggerEnter.Invoke();
			return;
		}
		else
		{
			UnityEvent doOnTriggerExit = this.DoOnTriggerExit;
			if (doOnTriggerExit == null)
			{
				return;
			}
			doOnTriggerExit.Invoke();
			return;
		}
	}

	// Token: 0x040006B9 RID: 1721
	public bool onlyMainCharacter;

	// Token: 0x040006BA RID: 1722
	public bool filterByTeam;

	// Token: 0x040006BB RID: 1723
	public Teams selfTeam;

	// Token: 0x040006BC RID: 1724
	public LayerMask layerMask;

	// Token: 0x040006BD RID: 1725
	public bool triggerOnce;

	// Token: 0x040006BE RID: 1726
	public UnityEvent DoOnTriggerEnter = new UnityEvent();

	// Token: 0x040006BF RID: 1727
	public UnityEvent DoOnTriggerExit = new UnityEvent();

	// Token: 0x040006C0 RID: 1728
	private bool triggered;

	// Token: 0x040006C1 RID: 1729
	private bool mainCharacterIn;
}
