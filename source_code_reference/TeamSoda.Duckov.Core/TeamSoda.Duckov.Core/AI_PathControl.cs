using System;
using Pathfinding;
using UnityEngine;

// Token: 0x0200004C RID: 76
public class AI_PathControl : MonoBehaviour
{
	// Token: 0x17000064 RID: 100
	// (get) Token: 0x060001DE RID: 478 RVA: 0x00009395 File Offset: 0x00007595
	public bool ReachedEndOfPath
	{
		get
		{
			return this.reachedEndOfPath;
		}
	}

	// Token: 0x17000065 RID: 101
	// (get) Token: 0x060001DF RID: 479 RVA: 0x0000939D File Offset: 0x0000759D
	public bool Moving
	{
		get
		{
			return this.moving;
		}
	}

	// Token: 0x17000066 RID: 102
	// (get) Token: 0x060001E0 RID: 480 RVA: 0x000093A5 File Offset: 0x000075A5
	public bool WaitingForPathResult
	{
		get
		{
			return this.waitingForPathResult;
		}
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x000093AD File Offset: 0x000075AD
	public void Start()
	{
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x000093AF File Offset: 0x000075AF
	public void MoveToPos(Vector3 pos)
	{
		this.reachedEndOfPath = false;
		this.path = null;
		this.seeker.StartPath(base.transform.position, pos, new OnPathDelegate(this.OnPathComplete));
		this.waitingForPathResult = true;
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x000093EA File Offset: 0x000075EA
	public void OnPathComplete(Path p)
	{
		if (!p.error)
		{
			this.path = p;
			this.currentWaypoint = 0;
			this.moving = true;
		}
		this.waitingForPathResult = false;
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x00009410 File Offset: 0x00007610
	public void Update()
	{
		this.moving = this.path != null;
		if (this.path == null)
		{
			return;
		}
		this.reachedEndOfPath = false;
		float num;
		for (;;)
		{
			num = Vector3.Distance(base.transform.position, this.path.vectorPath[this.currentWaypoint]);
			if (num >= this.nextWaypointDistance)
			{
				goto IL_0080;
			}
			if (this.currentWaypoint + 1 >= this.path.vectorPath.Count)
			{
				break;
			}
			this.currentWaypoint++;
		}
		this.reachedEndOfPath = true;
		IL_0080:
		Vector3 normalized = (this.path.vectorPath[this.currentWaypoint] - base.transform.position).normalized;
		if (this.reachedEndOfPath)
		{
			float num2 = Mathf.Sqrt(num / this.nextWaypointDistance);
			this.controller.SetMoveInput(normalized * num2);
			if (num < this.stopDistance)
			{
				this.path = null;
				this.controller.SetMoveInput(Vector2.zero);
				return;
			}
		}
		else
		{
			this.controller.SetMoveInput(normalized);
		}
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x00009526 File Offset: 0x00007726
	public void StopMove()
	{
		this.path = null;
		this.controller.SetMoveInput(Vector3.zero);
		this.waitingForPathResult = false;
	}

	// Token: 0x0400019B RID: 411
	public Seeker seeker;

	// Token: 0x0400019C RID: 412
	public CharacterMainControl controller;

	// Token: 0x0400019D RID: 413
	public Path path;

	// Token: 0x0400019E RID: 414
	public float nextWaypointDistance = 3f;

	// Token: 0x0400019F RID: 415
	private int currentWaypoint;

	// Token: 0x040001A0 RID: 416
	private bool reachedEndOfPath;

	// Token: 0x040001A1 RID: 417
	public float stopDistance = 0.2f;

	// Token: 0x040001A2 RID: 418
	private bool moving;

	// Token: 0x040001A3 RID: 419
	private bool waitingForPathResult;
}
