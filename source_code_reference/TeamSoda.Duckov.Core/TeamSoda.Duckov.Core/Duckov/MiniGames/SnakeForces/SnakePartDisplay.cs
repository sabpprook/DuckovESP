using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.MiniGames.SnakeForces
{
	// Token: 0x02000287 RID: 647
	public class SnakePartDisplay : MiniGameBehaviour
	{
		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x060014DE RID: 5342 RVA: 0x0004D81D File Offset: 0x0004BA1D
		// (set) Token: 0x060014DF RID: 5343 RVA: 0x0004D825 File Offset: 0x0004BA25
		public SnakeDisplay Master { get; private set; }

		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x060014E0 RID: 5344 RVA: 0x0004D82E File Offset: 0x0004BA2E
		// (set) Token: 0x060014E1 RID: 5345 RVA: 0x0004D836 File Offset: 0x0004BA36
		public SnakeForce.Part Target { get; private set; }

		// Token: 0x060014E2 RID: 5346 RVA: 0x0004D840 File Offset: 0x0004BA40
		internal void Setup(SnakeDisplay master, SnakeForce.Part part)
		{
			if (this.Target != null)
			{
				this.Target.OnMove -= this.OnTargetMove;
			}
			this.Master = master;
			this.Target = part;
			this.cachedCoord = this.Target.coord;
			base.transform.localPosition = this.Master.GetPosition(this.cachedCoord);
			this.Target.OnMove += this.OnTargetMove;
		}

		// Token: 0x060014E3 RID: 5347 RVA: 0x0004D8C0 File Offset: 0x0004BAC0
		private void OnTargetMove(SnakeForce.Part part)
		{
			if (!base.enabled)
			{
				return;
			}
			int sqrMagnitude = (this.Target.coord - this.cachedCoord).sqrMagnitude;
			this.cachedCoord = this.Target.coord;
			Vector3 position = this.Master.GetPosition(this.cachedCoord);
			this.DoMove(position);
		}

		// Token: 0x060014E4 RID: 5348 RVA: 0x0004D91F File Offset: 0x0004BB1F
		private void DoMove(Vector3 vector3)
		{
			base.transform.localPosition = vector3;
		}

		// Token: 0x060014E5 RID: 5349 RVA: 0x0004D930 File Offset: 0x0004BB30
		internal void Punch()
		{
			base.transform.DOKill(true);
			base.transform.localScale = Vector3.one;
			base.transform.DOPunchScale(Vector3.one * 1.1f, 0.2f, 4, 1f);
		}

		// Token: 0x060014E6 RID: 5350 RVA: 0x0004D980 File Offset: 0x0004BB80
		internal void PunchColor(Color color)
		{
			this.image.DOKill(false);
			this.image.color = color;
			this.image.DOColor(Color.white, 0.4f);
		}

		// Token: 0x04000F5E RID: 3934
		[SerializeField]
		private Image image;

		// Token: 0x04000F5F RID: 3935
		private Vector2Int cachedCoord;
	}
}
