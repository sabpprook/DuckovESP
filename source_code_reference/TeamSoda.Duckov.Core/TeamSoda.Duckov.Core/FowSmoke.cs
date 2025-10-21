using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000A4 RID: 164
public class FowSmoke : MonoBehaviour
{
	// Token: 0x0600059D RID: 1437 RVA: 0x00019328 File Offset: 0x00017528
	private void Start()
	{
		this.UpdateSmoke().Forget();
	}

	// Token: 0x0600059E RID: 1438 RVA: 0x00019344 File Offset: 0x00017544
	private async UniTaskVoid UpdateSmoke()
	{
		if (!(this.colParent == null))
		{
			this.colParent.localScale = Vector3.one * 0.01f;
			float startTimer = 0f;
			while (startTimer < this.startTime)
			{
				await UniTask.WaitForEndOfFrame(this);
				if (this.colParent == null)
				{
					return;
				}
				startTimer += Time.deltaTime;
				this.colParent.localScale = Vector3.one * Mathf.Clamp01(startTimer / this.startTime);
			}
			await UniTask.WaitForSeconds(this.startTime, false, PlayerLoopTiming.Update, default(CancellationToken), false);
			if (this.colParent != null)
			{
				this.colParent.gameObject.SetActive(true);
			}
			await UniTask.WaitForSeconds(this.lifeTime, false, PlayerLoopTiming.Update, default(CancellationToken), false);
			UnityEvent unityEvent = this.beforeFadeOutEvent;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			for (int i = 0; i < this.particles.Length; i++)
			{
				if (!(this.particles[i] == null))
				{
					this.particles[i].emission.rateOverTime = 0f;
				}
			}
			float dieTimer = 0f;
			while (dieTimer < this.particleFadeTime)
			{
				await UniTask.WaitForEndOfFrame(this);
				dieTimer += Time.deltaTime;
				float num = Mathf.Clamp01(dieTimer / this.particleFadeTime);
				if (this.colParent == null)
				{
					return;
				}
				this.colParent.localScale = Vector3.one * (1f - num);
			}
			if (base.gameObject != null)
			{
				global::UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x0600059F RID: 1439 RVA: 0x00019387 File Offset: 0x00017587
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
	}

	// Token: 0x04000516 RID: 1302
	[SerializeField]
	private int res = 8;

	// Token: 0x04000517 RID: 1303
	[SerializeField]
	private float radius;

	// Token: 0x04000518 RID: 1304
	[SerializeField]
	private float height;

	// Token: 0x04000519 RID: 1305
	[SerializeField]
	private float thickness;

	// Token: 0x0400051A RID: 1306
	public Transform colParent;

	// Token: 0x0400051B RID: 1307
	public ParticleSystem[] particles;

	// Token: 0x0400051C RID: 1308
	public float startTime;

	// Token: 0x0400051D RID: 1309
	public float lifeTime;

	// Token: 0x0400051E RID: 1310
	public float particleFadeTime = 3f;

	// Token: 0x0400051F RID: 1311
	public UnityEvent beforeFadeOutEvent;
}
