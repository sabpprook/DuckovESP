using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000192 RID: 402
public class TimeOfDayVolumeControl : MonoBehaviour
{
	// Token: 0x1700022E RID: 558
	// (get) Token: 0x06000BD7 RID: 3031 RVA: 0x0003252B File Offset: 0x0003072B
	public VolumeProfile CurrentProfile
	{
		get
		{
			return this.currentProfile;
		}
	}

	// Token: 0x1700022F RID: 559
	// (get) Token: 0x06000BD8 RID: 3032 RVA: 0x00032533 File Offset: 0x00030733
	public VolumeProfile BufferTargetProfile
	{
		get
		{
			return this.bufferTargetProfile;
		}
	}

	// Token: 0x06000BD9 RID: 3033 RVA: 0x0003253C File Offset: 0x0003073C
	private void Update()
	{
		if (!this.blending && this.bufferTargetProfile != null)
		{
			this.StartBlendToBufferdTarget();
		}
		if (this.blending)
		{
			this.UpdateBlending(Time.deltaTime);
		}
		if (!this.blending && this.fromVolume.gameObject.activeSelf)
		{
			this.fromVolume.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000BDA RID: 3034 RVA: 0x000325A4 File Offset: 0x000307A4
	private void UpdateBlending(float deltaTime)
	{
		this.blendTimer += deltaTime;
		float num = this.blendTimer / this.blendTime;
		if (num > 1f)
		{
			num = 1f;
			this.blending = false;
		}
		this.toVolume.weight = this.blendCurve.Evaluate(num);
	}

	// Token: 0x06000BDB RID: 3035 RVA: 0x000325F9 File Offset: 0x000307F9
	public void SetTargetProfile(VolumeProfile profile)
	{
		this.bufferTargetProfile = profile;
	}

	// Token: 0x06000BDC RID: 3036 RVA: 0x00032604 File Offset: 0x00030804
	private void StartBlendToBufferdTarget()
	{
		this.blending = true;
		this.blendingTargetProfile = this.bufferTargetProfile;
		this.bufferTargetProfile = null;
		this.currentProfile = this.blendingTargetProfile;
		this.fromVolume.gameObject.SetActive(true);
		this.fromVolume.profile = this.toVolume.profile;
		this.fromVolume.weight = 1f;
		this.toVolume.profile = this.blendingTargetProfile;
		this.toVolume.weight = 0f;
		this.blendTimer = 0f;
	}

	// Token: 0x06000BDD RID: 3037 RVA: 0x0003269A File Offset: 0x0003089A
	public void ForceSetProfile(VolumeProfile profile)
	{
		this.bufferTargetProfile = profile;
		this.StartBlendToBufferdTarget();
		this.UpdateBlending(999f);
	}

	// Token: 0x04000A4A RID: 2634
	private VolumeProfile currentProfile;

	// Token: 0x04000A4B RID: 2635
	private VolumeProfile blendingTargetProfile;

	// Token: 0x04000A4C RID: 2636
	private VolumeProfile bufferTargetProfile;

	// Token: 0x04000A4D RID: 2637
	public Volume fromVolume;

	// Token: 0x04000A4E RID: 2638
	public Volume toVolume;

	// Token: 0x04000A4F RID: 2639
	private bool blending;

	// Token: 0x04000A50 RID: 2640
	private float blendTimer;

	// Token: 0x04000A51 RID: 2641
	public float blendTime = 2f;

	// Token: 0x04000A52 RID: 2642
	public AnimationCurve blendCurve;
}
