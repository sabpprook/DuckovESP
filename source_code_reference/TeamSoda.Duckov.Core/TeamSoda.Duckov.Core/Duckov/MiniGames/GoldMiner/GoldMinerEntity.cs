using System;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x0200028D RID: 653
	public class GoldMinerEntity : MiniGameBehaviour
	{
		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x0600154B RID: 5451 RVA: 0x0004EEDB File Offset: 0x0004D0DB
		// (set) Token: 0x0600154C RID: 5452 RVA: 0x0004EEE3 File Offset: 0x0004D0E3
		public GoldMiner master { get; private set; }

		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x0600154D RID: 5453 RVA: 0x0004EEEC File Offset: 0x0004D0EC
		public string TypeID
		{
			get
			{
				return this.typeID;
			}
		}

		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x0600154E RID: 5454 RVA: 0x0004EEF4 File Offset: 0x0004D0F4
		public float Speed
		{
			get
			{
				return this.speed;
			}
		}

		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x0600154F RID: 5455 RVA: 0x0004EEFC File Offset: 0x0004D0FC
		// (set) Token: 0x06001550 RID: 5456 RVA: 0x0004EF04 File Offset: 0x0004D104
		public int Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		// Token: 0x06001551 RID: 5457 RVA: 0x0004EF0D File Offset: 0x0004D10D
		public void SetMaster(GoldMiner master)
		{
			this.master = master;
		}

		// Token: 0x06001552 RID: 5458 RVA: 0x0004EF16 File Offset: 0x0004D116
		public void NotifyAttached(Hook hook)
		{
			Action<GoldMinerEntity, Hook> onAttached = this.OnAttached;
			if (onAttached != null)
			{
				onAttached(this, hook);
			}
			FXPool.Play(this.contactFX, base.transform.position, base.transform.rotation);
		}

		// Token: 0x06001553 RID: 5459 RVA: 0x0004EF4D File Offset: 0x0004D14D
		public void NotifyBeginRetrieving()
		{
			FXPool.Play(this.beginMoveFX, base.transform.position, base.transform.rotation);
		}

		// Token: 0x06001554 RID: 5460 RVA: 0x0004EF71 File Offset: 0x0004D171
		internal void Explode(Vector3 origin)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
			FXPool.Play(this.explodeFX, base.transform.position, base.transform.rotation);
		}

		// Token: 0x06001555 RID: 5461 RVA: 0x0004EFA0 File Offset: 0x0004D1A0
		internal void NotifyResolved(GoldMiner game)
		{
			Action<GoldMinerEntity, GoldMiner> onResolved = this.OnResolved;
			if (onResolved != null)
			{
				onResolved(this, game);
			}
			FXPool.Play(this.resolveFX, base.transform.position, base.transform.rotation);
		}

		// Token: 0x04000FC0 RID: 4032
		[SerializeField]
		private string typeID;

		// Token: 0x04000FC1 RID: 4033
		[SerializeField]
		public GoldMinerEntity.Size size;

		// Token: 0x04000FC2 RID: 4034
		[SerializeField]
		public GoldMinerEntity.Tag[] tags;

		// Token: 0x04000FC3 RID: 4035
		[SerializeField]
		private int value;

		// Token: 0x04000FC4 RID: 4036
		[SerializeField]
		private float speed = 1f;

		// Token: 0x04000FC5 RID: 4037
		[SerializeField]
		private ParticleSystem contactFX;

		// Token: 0x04000FC6 RID: 4038
		[SerializeField]
		private ParticleSystem beginMoveFX;

		// Token: 0x04000FC7 RID: 4039
		[SerializeField]
		private ParticleSystem resolveFX;

		// Token: 0x04000FC8 RID: 4040
		[SerializeField]
		private ParticleSystem explodeFX;

		// Token: 0x04000FC9 RID: 4041
		public Action<GoldMinerEntity, Hook> OnAttached;

		// Token: 0x04000FCA RID: 4042
		public Action<GoldMinerEntity, GoldMiner> OnResolved;

		// Token: 0x02000564 RID: 1380
		public enum Size
		{
			// Token: 0x04001F32 RID: 7986
			XS = -2,
			// Token: 0x04001F33 RID: 7987
			S,
			// Token: 0x04001F34 RID: 7988
			M,
			// Token: 0x04001F35 RID: 7989
			L,
			// Token: 0x04001F36 RID: 7990
			XL
		}

		// Token: 0x02000565 RID: 1381
		public enum Tag
		{
			// Token: 0x04001F38 RID: 7992
			None,
			// Token: 0x04001F39 RID: 7993
			Rock,
			// Token: 0x04001F3A RID: 7994
			Gold,
			// Token: 0x04001F3B RID: 7995
			Diamond,
			// Token: 0x04001F3C RID: 7996
			Mine,
			// Token: 0x04001F3D RID: 7997
			Chest,
			// Token: 0x04001F3E RID: 7998
			Pig,
			// Token: 0x04001F3F RID: 7999
			Cable
		}
	}
}
