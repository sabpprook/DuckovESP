using System;
using UnityEngine;

// Token: 0x02000173 RID: 371
public class BowAnimation : MonoBehaviour
{
	// Token: 0x06000B4E RID: 2894 RVA: 0x0002FFAC File Offset: 0x0002E1AC
	private void Start()
	{
		if (this.gunAgent != null)
		{
			this.gunAgent.OnShootEvent += this.OnShoot;
			this.gunAgent.OnLoadedEvent += this.OnLoaded;
			if (this.gunAgent.BulletCount > 0)
			{
				this.OnLoaded();
			}
		}
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x00030009 File Offset: 0x0002E209
	private void OnDestroy()
	{
		if (this.gunAgent != null)
		{
			this.gunAgent.OnShootEvent -= this.OnShoot;
			this.gunAgent.OnLoadedEvent -= this.OnLoaded;
		}
	}

	// Token: 0x06000B50 RID: 2896 RVA: 0x00030047 File Offset: 0x0002E247
	private void OnShoot()
	{
		this.animator.SetTrigger("Shoot");
		if (this.gunAgent.BulletCount <= 0)
		{
			this.animator.SetBool("Loaded", false);
		}
	}

	// Token: 0x06000B51 RID: 2897 RVA: 0x00030078 File Offset: 0x0002E278
	private void OnLoaded()
	{
		this.animator.SetBool("Loaded", true);
	}

	// Token: 0x040009A1 RID: 2465
	public ItemAgent_Gun gunAgent;

	// Token: 0x040009A2 RID: 2466
	public Animator animator;

	// Token: 0x040009A3 RID: 2467
	private int hash_Loaded = "Loaded".GetHashCode();

	// Token: 0x040009A4 RID: 2468
	private int hash_Aiming = "Aiming".GetHashCode();

	// Token: 0x040009A5 RID: 2469
	private int hash_Shoot = "Shoot".GetHashCode();
}
