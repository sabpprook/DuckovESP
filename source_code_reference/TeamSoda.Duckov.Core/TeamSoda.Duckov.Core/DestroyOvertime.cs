using System;
using UnityEngine;

// Token: 0x0200013D RID: 317
public class DestroyOvertime : MonoBehaviour
{
	// Token: 0x06000A1E RID: 2590 RVA: 0x0002B68D File Offset: 0x0002988D
	private void Awake()
	{
		if (this.life <= 0f)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000A1F RID: 2591 RVA: 0x0002B6A7 File Offset: 0x000298A7
	private void Update()
	{
		this.life -= Time.deltaTime;
		if (this.life <= 0f)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000A20 RID: 2592 RVA: 0x0002B6D3 File Offset: 0x000298D3
	private void OnValidate()
	{
		this.ProcessParticleSystem();
	}

	// Token: 0x06000A21 RID: 2593 RVA: 0x0002B6DC File Offset: 0x000298DC
	private void ProcessParticleSystem()
	{
		float num = 0f;
		ParticleSystem component = base.GetComponent<ParticleSystem>();
		if (!component)
		{
			return;
		}
		if (component != null)
		{
			ParticleSystem.MainModule main = component.main;
			main.stopAction = ParticleSystemStopAction.None;
			if (main.startLifetime.constant > num)
			{
				num = main.startLifetime.constant;
			}
		}
		ParticleSystem[] componentsInChildren = base.transform.GetComponentsInChildren<ParticleSystem>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			ParticleSystem.MainModule main2 = componentsInChildren[i].main;
			main2.stopAction = ParticleSystemStopAction.None;
			if (main2.startLifetime.constant > num)
			{
				num = main2.startLifetime.constant;
			}
		}
		this.life = num + 0.2f;
	}

	// Token: 0x040008DA RID: 2266
	public float life = 1f;
}
