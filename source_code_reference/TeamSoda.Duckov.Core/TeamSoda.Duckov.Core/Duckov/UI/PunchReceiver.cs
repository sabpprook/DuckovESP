using System;
using DG.Tweening;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x02000388 RID: 904
	public class PunchReceiver : MonoBehaviour
	{
		// Token: 0x17000608 RID: 1544
		// (get) Token: 0x06001F6E RID: 8046 RVA: 0x0006DD7F File Offset: 0x0006BF7F
		private float PunchAnchorPositionDuration
		{
			get
			{
				return this.duration;
			}
		}

		// Token: 0x17000609 RID: 1545
		// (get) Token: 0x06001F6F RID: 8047 RVA: 0x0006DD87 File Offset: 0x0006BF87
		private float PunchScaleDuration
		{
			get
			{
				return this.duration;
			}
		}

		// Token: 0x1700060A RID: 1546
		// (get) Token: 0x06001F70 RID: 8048 RVA: 0x0006DD8F File Offset: 0x0006BF8F
		private float PunchRotationDuration
		{
			get
			{
				return this.duration;
			}
		}

		// Token: 0x1700060B RID: 1547
		// (get) Token: 0x06001F71 RID: 8049 RVA: 0x0006DD97 File Offset: 0x0006BF97
		private bool ShouldPunchPosition
		{
			get
			{
				return this.randomAnchorPosition.magnitude > 0.001f && this.punchAnchorPosition.magnitude > 0.001f;
			}
		}

		// Token: 0x06001F72 RID: 8050 RVA: 0x0006DDBF File Offset: 0x0006BFBF
		private void Awake()
		{
			if (this.rectTransform == null)
			{
				this.rectTransform = base.GetComponent<RectTransform>();
			}
			this.CachePose();
		}

		// Token: 0x06001F73 RID: 8051 RVA: 0x0006DDE1 File Offset: 0x0006BFE1
		private void Start()
		{
		}

		// Token: 0x06001F74 RID: 8052 RVA: 0x0006DDE4 File Offset: 0x0006BFE4
		[ContextMenu("Punch")]
		public void Punch()
		{
			if (!base.enabled)
			{
				return;
			}
			if (this.rectTransform == null)
			{
				return;
			}
			if (this.particle != null)
			{
				this.particle.Play();
			}
			this.rectTransform.DOKill(false);
			if (this.cacheWhenPunched)
			{
				this.CachePose();
			}
			Vector2 vector = this.punchAnchorPosition + new Vector2(global::UnityEngine.Random.Range(-this.randomAnchorPosition.x, this.randomAnchorPosition.x), global::UnityEngine.Random.Range(-this.randomAnchorPosition.y, this.randomAnchorPosition.y));
			float num = this.punchScaleUniform;
			float num2 = this.punchRotationZ + global::UnityEngine.Random.Range(-this.randomRotationZ, this.randomRotationZ);
			if (this.ShouldPunchPosition)
			{
				this.rectTransform.DOPunchAnchorPos(vector, this.PunchAnchorPositionDuration, this.vibrato, this.elasticity, false).SetEase(this.animationCurve).OnKill(new TweenCallback(this.RestorePose));
			}
			this.rectTransform.DOPunchScale(Vector3.one * num, this.PunchScaleDuration, this.vibrato, this.elasticity).SetEase(this.animationCurve).OnKill(new TweenCallback(this.RestorePose));
			this.rectTransform.DOPunchRotation(Vector3.forward * num2, this.PunchRotationDuration, this.vibrato, this.elasticity).SetEase(this.animationCurve).OnKill(new TweenCallback(this.RestorePose));
			if (!string.IsNullOrWhiteSpace(this.sfx))
			{
				AudioManager.Post(this.sfx);
			}
		}

		// Token: 0x06001F75 RID: 8053 RVA: 0x0006DF90 File Offset: 0x0006C190
		private void CachePose()
		{
			if (this.rectTransform == null)
			{
				return;
			}
			this.cachedAnchorPosition = this.rectTransform.anchoredPosition;
			this.cachedScale = this.rectTransform.localScale;
			this.cachedRotation = this.rectTransform.localRotation.eulerAngles;
		}

		// Token: 0x06001F76 RID: 8054 RVA: 0x0006DFF4 File Offset: 0x0006C1F4
		private void RestorePose()
		{
			if (this.rectTransform == null)
			{
				return;
			}
			if (this.ShouldPunchPosition)
			{
				this.rectTransform.anchoredPosition = this.cachedAnchorPosition;
			}
			this.rectTransform.localScale = this.cachedScale;
			this.rectTransform.localRotation = Quaternion.Euler(this.cachedRotation);
		}

		// Token: 0x06001F77 RID: 8055 RVA: 0x0006E05A File Offset: 0x0006C25A
		private void OnDestroy()
		{
			RectTransform rectTransform = this.rectTransform;
			if (rectTransform == null)
			{
				return;
			}
			rectTransform.DOKill(false);
		}

		// Token: 0x04001576 RID: 5494
		[SerializeField]
		private RectTransform rectTransform;

		// Token: 0x04001577 RID: 5495
		[SerializeField]
		private ParticleSystem particle;

		// Token: 0x04001578 RID: 5496
		[Min(0.0001f)]
		[SerializeField]
		private float duration = 0.01f;

		// Token: 0x04001579 RID: 5497
		public int vibrato = 10;

		// Token: 0x0400157A RID: 5498
		public float elasticity = 1f;

		// Token: 0x0400157B RID: 5499
		[SerializeField]
		private Vector2 punchAnchorPosition;

		// Token: 0x0400157C RID: 5500
		[SerializeField]
		[Range(-1f, 1f)]
		private float punchScaleUniform;

		// Token: 0x0400157D RID: 5501
		[SerializeField]
		[Range(-180f, 180f)]
		private float punchRotationZ;

		// Token: 0x0400157E RID: 5502
		[SerializeField]
		private Vector2 randomAnchorPosition;

		// Token: 0x0400157F RID: 5503
		[SerializeField]
		[Range(0f, 180f)]
		private float randomRotationZ;

		// Token: 0x04001580 RID: 5504
		[SerializeField]
		private AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04001581 RID: 5505
		[SerializeField]
		private bool cacheWhenPunched;

		// Token: 0x04001582 RID: 5506
		[SerializeField]
		private string sfx;

		// Token: 0x04001583 RID: 5507
		private Vector2 cachedAnchorPosition;

		// Token: 0x04001584 RID: 5508
		private Vector2 cachedScale;

		// Token: 0x04001585 RID: 5509
		private Vector2 cachedRotation;
	}
}
