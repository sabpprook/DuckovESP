using System;
using UnityEngine;

// Token: 0x02000177 RID: 375
public class DarkRoomFade : MonoBehaviour
{
	// Token: 0x06000B60 RID: 2912 RVA: 0x00030368 File Offset: 0x0002E568
	public void StartFade()
	{
		this.started = true;
		base.enabled = true;
		this.startPos = CharacterMainControl.Main.transform.position;
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x0003038D File Offset: 0x0002E58D
	private void Awake()
	{
		this.range = 0f;
		this.UpdateMaterial();
		if (!this.started)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x000303B0 File Offset: 0x0002E5B0
	private void Update()
	{
		if (!this.started)
		{
			base.enabled = false;
		}
		this.range += this.speed * Time.deltaTime;
		this.UpdateMaterial();
		if (this.range > this.maxRange)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06000B63 RID: 2915 RVA: 0x00030400 File Offset: 0x0002E600
	private void UpdateMaterial()
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetFloat("_Range", this.range);
		materialPropertyBlock.SetVector("_CenterPos", this.startPos);
		Renderer[] array = this.renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetPropertyBlock(materialPropertyBlock);
		}
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x00030458 File Offset: 0x0002E658
	private void Collect()
	{
		this.renderers = base.GetComponentsInChildren<Renderer>();
		Renderer[] array = this.renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].sharedMaterial.SetFloat("_Range", 0f);
		}
	}

	// Token: 0x06000B65 RID: 2917 RVA: 0x000304A0 File Offset: 0x0002E6A0
	public void SetRenderers(bool enable)
	{
		Renderer[] array = this.renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = enable;
		}
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x000304CC File Offset: 0x0002E6CC
	public static void SetRenderersEnable(bool enable)
	{
		DarkRoomFade[] array = global::UnityEngine.Object.FindObjectsByType<DarkRoomFade>(FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetRenderers(enable);
		}
	}

	// Token: 0x040009B3 RID: 2483
	public float maxRange = 100f;

	// Token: 0x040009B4 RID: 2484
	public float speed = 20f;

	// Token: 0x040009B5 RID: 2485
	public Renderer[] renderers;

	// Token: 0x040009B6 RID: 2486
	private Vector3 startPos;

	// Token: 0x040009B7 RID: 2487
	private float range;

	// Token: 0x040009B8 RID: 2488
	private bool started;
}
