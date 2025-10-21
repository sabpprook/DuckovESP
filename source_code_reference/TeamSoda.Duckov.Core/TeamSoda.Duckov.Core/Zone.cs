using System;
using System.Collections.Generic;
using Duckov.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020000B5 RID: 181
[RequireComponent(typeof(Rigidbody))]
public class Zone : MonoBehaviour
{
	// Token: 0x17000123 RID: 291
	// (get) Token: 0x060005E7 RID: 1511 RVA: 0x0001A4A4 File Offset: 0x000186A4
	public HashSet<Health> Healths
	{
		get
		{
			return this.healths;
		}
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x0001A4AC File Offset: 0x000186AC
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.healths = new HashSet<Health>();
		this.rb.isKinematic = true;
		this.rb.useGravity = false;
		this.sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
		if (this.setActiveByDistance)
		{
			SetActiveByPlayerDistance.Register(base.gameObject, this.sceneBuildIndex);
		}
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x0001A514 File Offset: 0x00018714
	private void OnDestroy()
	{
		if (this.setActiveByDistance)
		{
			SetActiveByPlayerDistance.Unregister(base.gameObject, this.sceneBuildIndex);
		}
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x0001A530 File Offset: 0x00018730
	private void OnTriggerEnter(Collider other)
	{
		if (!LevelManager.LevelInited)
		{
			return;
		}
		if (other.gameObject.layer != LayerMask.NameToLayer("Character"))
		{
			return;
		}
		Health component = other.GetComponent<Health>();
		if (component == null)
		{
			return;
		}
		if (this.onlyPlayerTeam && component.team != Teams.player)
		{
			return;
		}
		if (!this.healths.Contains(component))
		{
			this.healths.Add(component);
		}
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x0001A59C File Offset: 0x0001879C
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer != LayerMask.NameToLayer("Character"))
		{
			return;
		}
		Health component = other.GetComponent<Health>();
		if (component == null)
		{
			return;
		}
		if (this.onlyPlayerTeam && component.team != Teams.player)
		{
			return;
		}
		if (this.healths.Contains(component))
		{
			this.healths.Remove(component);
		}
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x0001A5FE File Offset: 0x000187FE
	private void OnDisable()
	{
		this.healths.Clear();
	}

	// Token: 0x0400056F RID: 1391
	public bool onlyPlayerTeam;

	// Token: 0x04000570 RID: 1392
	private HashSet<Health> healths;

	// Token: 0x04000571 RID: 1393
	public bool setActiveByDistance = true;

	// Token: 0x04000572 RID: 1394
	private Rigidbody rb;

	// Token: 0x04000573 RID: 1395
	private int sceneBuildIndex = -1;
}
