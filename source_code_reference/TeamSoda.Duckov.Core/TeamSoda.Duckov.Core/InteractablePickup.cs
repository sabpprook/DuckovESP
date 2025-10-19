using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

// Token: 0x020000DB RID: 219
public class InteractablePickup : InteractableBase
{
	// Token: 0x17000148 RID: 328
	// (get) Token: 0x060006FE RID: 1790 RVA: 0x0001F941 File Offset: 0x0001DB41
	public DuckovItemAgent ItemAgent
	{
		get
		{
			return this.itemAgent;
		}
	}

	// Token: 0x060006FF RID: 1791 RVA: 0x0001F949 File Offset: 0x0001DB49
	protected override bool IsInteractable()
	{
		return true;
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x0001F94C File Offset: 0x0001DB4C
	public void OnInit()
	{
		if (this.itemAgent && this.itemAgent.Item && this.sprite)
		{
			this.sprite.sprite = this.itemAgent.Item.Icon;
		}
		this.overrideInteractName = true;
		base.InteractName = this.itemAgent.Item.DisplayNameRaw;
	}

	// Token: 0x06000701 RID: 1793 RVA: 0x0001F9BD File Offset: 0x0001DBBD
	protected override void OnInteractStart(CharacterMainControl character)
	{
		character.PickupItem(this.itemAgent.Item);
		base.StopInteract();
	}

	// Token: 0x06000702 RID: 1794 RVA: 0x0001F9D8 File Offset: 0x0001DBD8
	public void Throw(Vector3 direction, float randomAngle)
	{
		this.throwStartPoint = base.transform.position;
		if (!this.rb)
		{
			this.rb = base.gameObject.AddComponent<Rigidbody>();
		}
		this.rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
		this.rb.constraints = RigidbodyConstraints.FreezeRotation;
		if (direction.magnitude < 0.1f)
		{
			direction = Vector3.zero;
		}
		else
		{
			direction.y = 0f;
			direction.Normalize();
			direction = Quaternion.Euler(0f, global::UnityEngine.Random.Range(-randomAngle, randomAngle) * 0.5f, 0f) * direction;
			direction *= global::UnityEngine.Random.Range(0.5f, 1f) * 3f;
			direction.y = 2.5f;
		}
		this.rb.velocity = direction;
		this.DestroyRigidbody().Forget();
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x0001FABF File Offset: 0x0001DCBF
	protected override void OnDestroy()
	{
		this.destroied = true;
		base.OnDestroy();
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x0001FAD0 File Offset: 0x0001DCD0
	private async UniTaskVoid DestroyRigidbody()
	{
		await UniTask.WaitForSeconds(3, false, PlayerLoopTiming.Update, default(CancellationToken), false);
		if (!this.destroied)
		{
			if (this.rb)
			{
				if (this.rb.velocity.y < -0.2f)
				{
					this.rb.transform.position = this.throwStartPoint;
					this.rb.position = this.throwStartPoint;
					await UniTask.WaitForSeconds(3, false, PlayerLoopTiming.Update, default(CancellationToken), false);
				}
				if (!this.destroied)
				{
					if (this.rb)
					{
						if (this.rb.velocity.y < -0.2f)
						{
							this.rb.transform.position = this.throwStartPoint;
							this.rb.position = this.throwStartPoint;
						}
						if (this.rb)
						{
							global::UnityEngine.Object.Destroy(this.rb);
						}
					}
				}
			}
		}
	}

	// Token: 0x040006A7 RID: 1703
	[SerializeField]
	private DuckovItemAgent itemAgent;

	// Token: 0x040006A8 RID: 1704
	public SpriteRenderer sprite;

	// Token: 0x040006A9 RID: 1705
	private Rigidbody rb;

	// Token: 0x040006AA RID: 1706
	private Vector3 throwStartPoint;

	// Token: 0x040006AB RID: 1707
	private bool destroied;
}
