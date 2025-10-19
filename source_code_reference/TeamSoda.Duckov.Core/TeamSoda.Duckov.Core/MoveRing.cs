using System;
using UnityEngine;

// Token: 0x02000181 RID: 385
public class MoveRing : MonoBehaviour
{
	// Token: 0x17000225 RID: 549
	// (get) Token: 0x06000B8C RID: 2956 RVA: 0x00030E27 File Offset: 0x0002F027
	private CharacterMainControl character
	{
		get
		{
			return this.inputManager.characterMainControl;
		}
	}

	// Token: 0x06000B8D RID: 2957 RVA: 0x00030E34 File Offset: 0x0002F034
	public void SetThreshold(float threshold)
	{
		this.runThreshold = threshold;
	}

	// Token: 0x06000B8E RID: 2958 RVA: 0x00030E40 File Offset: 0x0002F040
	public void LateUpdate()
	{
		if (!this.inputManager)
		{
			if (LevelManager.Instance == null)
			{
				return;
			}
			this.inputManager = LevelManager.Instance.InputManager;
			return;
		}
		else
		{
			if (!this.character)
			{
				this.SetMove(Vector3.zero, 0f);
				return;
			}
			base.transform.position = this.character.transform.position + Vector3.up * 0.02f;
			this.SetThreshold(this.inputManager.runThreshold);
			this.SetMove(this.inputManager.WorldMoveInput.normalized, this.inputManager.WorldMoveInput.magnitude);
			this.SetRunning(this.character.Running);
			if (this.ring.enabled != this.character.gameObject.activeInHierarchy)
			{
				this.ring.enabled = this.character.gameObject.activeInHierarchy;
			}
			return;
		}
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x00030F4C File Offset: 0x0002F14C
	public void SetMove(Vector3 direction, float value)
	{
		if (this.ringMat)
		{
			this.ringMat.SetVector("_Direction", direction);
			this.ringMat.SetFloat("_Distance", value);
			this.ringMat.SetFloat("_Threshold", this.runThreshold);
			return;
		}
		if (!this.ring)
		{
			return;
		}
		this.ringMat = this.ring.material;
	}

	// Token: 0x06000B90 RID: 2960 RVA: 0x00030FC3 File Offset: 0x0002F1C3
	public void SetRunning(bool running)
	{
		this.ringMat.SetFloat("_Running", (float)(running ? 1 : 0));
	}

	// Token: 0x040009DD RID: 2525
	public Renderer ring;

	// Token: 0x040009DE RID: 2526
	public float runThreshold;

	// Token: 0x040009DF RID: 2527
	private Material ringMat;

	// Token: 0x040009E0 RID: 2528
	private InputManager inputManager;
}
