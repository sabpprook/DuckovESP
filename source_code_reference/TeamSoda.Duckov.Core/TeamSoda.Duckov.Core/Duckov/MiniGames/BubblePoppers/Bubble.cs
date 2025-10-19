using System;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.MiniGames.BubblePoppers
{
	// Token: 0x020002D6 RID: 726
	public class Bubble : MiniGameBehaviour
	{
		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x060016CF RID: 5839 RVA: 0x000536C9 File Offset: 0x000518C9
		// (set) Token: 0x060016D0 RID: 5840 RVA: 0x000536D1 File Offset: 0x000518D1
		public BubblePopper Master { get; private set; }

		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x060016D1 RID: 5841 RVA: 0x000536DA File Offset: 0x000518DA
		public float Radius
		{
			get
			{
				return this.radius;
			}
		}

		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x060016D2 RID: 5842 RVA: 0x000536E2 File Offset: 0x000518E2
		public int ColorIndex
		{
			get
			{
				return this.colorIndex;
			}
		}

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x060016D3 RID: 5843 RVA: 0x000536EA File Offset: 0x000518EA
		public Color DisplayColor
		{
			get
			{
				if (this.Master == null)
				{
					return Color.white;
				}
				return this.Master.GetDisplayColor(this.ColorIndex);
			}
		}

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x060016D4 RID: 5844 RVA: 0x00053711 File Offset: 0x00051911
		// (set) Token: 0x060016D5 RID: 5845 RVA: 0x00053719 File Offset: 0x00051919
		public Vector2Int Coord { get; internal set; }

		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x060016D6 RID: 5846 RVA: 0x00053722 File Offset: 0x00051922
		// (set) Token: 0x060016D7 RID: 5847 RVA: 0x0005372A File Offset: 0x0005192A
		public Vector2 MoveDirection { get; internal set; }

		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x060016D8 RID: 5848 RVA: 0x00053733 File Offset: 0x00051933
		// (set) Token: 0x060016D9 RID: 5849 RVA: 0x0005373B File Offset: 0x0005193B
		public Vector2 Velocity { get; internal set; }

		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x060016DA RID: 5850 RVA: 0x00053744 File Offset: 0x00051944
		// (set) Token: 0x060016DB RID: 5851 RVA: 0x0005374C File Offset: 0x0005194C
		public Bubble.Status status { get; private set; }

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x060016DC RID: 5852 RVA: 0x00053755 File Offset: 0x00051955
		// (set) Token: 0x060016DD RID: 5853 RVA: 0x00053767 File Offset: 0x00051967
		private Vector2 gPos
		{
			get
			{
				return this.graphicsRoot.localPosition;
			}
			set
			{
				this.graphicsRoot.localPosition = value;
			}
		}

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x060016DE RID: 5854 RVA: 0x0005377C File Offset: 0x0005197C
		private Vector2 gForce
		{
			get
			{
				return (new Vector2(Mathf.PerlinNoise(7.3f, Time.time * this.vibrationFrequency) * 2f - 1f, Mathf.PerlinNoise(0.3f, Time.time * this.vibrationFrequency) * 2f - 1f) * this.vibrationAmp - this.gPos) * this.gSpring;
			}
		}

		// Token: 0x060016DF RID: 5855 RVA: 0x000537F3 File Offset: 0x000519F3
		internal void Setup(BubblePopper master, int colorIndex)
		{
			this.Master = master;
			this.colorIndex = colorIndex;
			this.image.color = this.DisplayColor;
		}

		// Token: 0x060016E0 RID: 5856 RVA: 0x00053814 File Offset: 0x00051A14
		internal void Launch(Vector2 direction)
		{
			this.MoveDirection = direction;
			this.status = Bubble.Status.Moving;
		}

		// Token: 0x060016E1 RID: 5857 RVA: 0x00053824 File Offset: 0x00051A24
		internal void NotifyExplode(Vector2Int origin)
		{
			this.status = Bubble.Status.Explode;
			Vector2Int vector2Int = this.Coord - origin;
			float magnitude = vector2Int.magnitude;
			this.explodeETA = magnitude * 0.025f;
			this.Impact(vector2Int.normalized * 5f);
		}

		// Token: 0x060016E2 RID: 5858 RVA: 0x00053878 File Offset: 0x00051A78
		internal void NotifyAttached(Vector2Int coord)
		{
			Vector2 vector = this.Master.Layout.CoordToLocalPosition(coord);
			base.transform.position = this.Master.Layout.transform.localToWorldMatrix.MultiplyPoint(vector);
			this.status = Bubble.Status.Attached;
			this.Coord = coord;
		}

		// Token: 0x060016E3 RID: 5859 RVA: 0x000538D3 File Offset: 0x00051AD3
		public void NotifyDetached()
		{
			this.status = Bubble.Status.Detached;
			this.Velocity = Vector2.zero;
			this.explodeCountDown = this.explodeAfterDetachedFor;
		}

		// Token: 0x060016E4 RID: 5860 RVA: 0x000538F3 File Offset: 0x00051AF3
		protected override void OnUpdate(float deltaTime)
		{
			this.UpdateLogic(deltaTime);
			this.UpdateGraphics(deltaTime);
		}

		// Token: 0x060016E5 RID: 5861 RVA: 0x00053903 File Offset: 0x00051B03
		private void UpdateLogic(float deltaTime)
		{
			if (this.Master == null)
			{
				return;
			}
			if (this.Master.Busy)
			{
				return;
			}
			if (this.status == Bubble.Status.Moving)
			{
				this.Master.MoveBubble(this, deltaTime);
			}
		}

		// Token: 0x060016E6 RID: 5862 RVA: 0x00053938 File Offset: 0x00051B38
		private void UpdateGraphics(float deltaTime)
		{
			if (this.status == Bubble.Status.Explode)
			{
				this.explodeETA -= deltaTime;
				if (this.explodeETA <= 0f)
				{
					FXPool.Play(this.explodeFXrefab, base.transform.position, base.transform.rotation, this.DisplayColor);
					this.Master.Release(this);
				}
			}
			if (this.status == Bubble.Status.Detached)
			{
				base.transform.localPosition += this.Velocity * deltaTime;
				this.Velocity += -Vector2.up * this.gravity;
				this.explodeCountDown -= deltaTime;
				if (this.explodeCountDown <= 0f)
				{
					this.NotifyExplode(this.Coord);
				}
			}
			this.UpdateElasticMovement(deltaTime);
		}

		// Token: 0x060016E7 RID: 5863 RVA: 0x00053A24 File Offset: 0x00051C24
		private void UpdateElasticMovement(float deltaTime)
		{
			float num = ((Vector2.Dot(this.gVelocity, this.gForce.normalized) < 0f) ? this.gDamping : 1f);
			this.gVelocity += this.gForce * deltaTime;
			this.gVelocity = Vector2.MoveTowards(this.gVelocity, Vector2.zero, num * this.gVelocity.magnitude * deltaTime);
			this.gPos += this.gVelocity;
		}

		// Token: 0x060016E8 RID: 5864 RVA: 0x00053AB8 File Offset: 0x00051CB8
		public void Impact(Vector2 velocity)
		{
			this.gVelocity = velocity;
		}

		// Token: 0x060016E9 RID: 5865 RVA: 0x00053AC1 File Offset: 0x00051CC1
		internal void Rest()
		{
			this.gPos = Vector2.zero;
			this.gVelocity = Vector2.zero;
		}

		// Token: 0x040010B6 RID: 4278
		[SerializeField]
		private float radius;

		// Token: 0x040010B7 RID: 4279
		[SerializeField]
		private int colorIndex;

		// Token: 0x040010B8 RID: 4280
		[SerializeField]
		private float gravity;

		// Token: 0x040010B9 RID: 4281
		[SerializeField]
		private float explodeAfterDetachedFor = 1f;

		// Token: 0x040010BA RID: 4282
		[SerializeField]
		private ParticleSystem explodeFXrefab;

		// Token: 0x040010BB RID: 4283
		[SerializeField]
		private Image image;

		// Token: 0x040010BC RID: 4284
		[SerializeField]
		private RectTransform graphicsRoot;

		// Token: 0x040010BD RID: 4285
		[SerializeField]
		private float gSpring = 1f;

		// Token: 0x040010BE RID: 4286
		[SerializeField]
		private float gDamping = 10f;

		// Token: 0x040010BF RID: 4287
		[SerializeField]
		private float vibrationFrequency = 10f;

		// Token: 0x040010C0 RID: 4288
		[SerializeField]
		private float vibrationAmp = 4f;

		// Token: 0x040010C5 RID: 4293
		private float explodeETA;

		// Token: 0x040010C6 RID: 4294
		private float explodeCountDown;

		// Token: 0x040010C7 RID: 4295
		private Vector2 gVelocity;

		// Token: 0x0200056D RID: 1389
		public enum Status
		{
			// Token: 0x04001F5D RID: 8029
			Idle,
			// Token: 0x04001F5E RID: 8030
			Moving,
			// Token: 0x04001F5F RID: 8031
			Attached,
			// Token: 0x04001F60 RID: 8032
			Detached,
			// Token: 0x04001F61 RID: 8033
			Explode
		}
	}
}
