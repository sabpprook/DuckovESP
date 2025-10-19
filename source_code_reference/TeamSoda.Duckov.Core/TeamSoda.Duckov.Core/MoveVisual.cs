using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000182 RID: 386
public class MoveVisual : MonoBehaviour
{
	// Token: 0x17000226 RID: 550
	// (get) Token: 0x06000B92 RID: 2962 RVA: 0x00030FE5 File Offset: 0x0002F1E5
	private CharacterMainControl Character
	{
		get
		{
			if (!this.characterModel)
			{
				return null;
			}
			return this.characterModel.characterMainControl;
		}
	}

	// Token: 0x06000B93 RID: 2963 RVA: 0x00031004 File Offset: 0x0002F204
	private void Awake()
	{
		foreach (ParticleSystem particleSystem in this.runParticles)
		{
			particleSystem.emission.enabled = this.running;
		}
	}

	// Token: 0x06000B94 RID: 2964 RVA: 0x00031064 File Offset: 0x0002F264
	private void Update()
	{
		if (!this.Character)
		{
			return;
		}
		if (this.Character.Running != this.running)
		{
			this.running = this.Character.Running;
			foreach (ParticleSystem particleSystem in this.runParticles)
			{
				particleSystem.emission.enabled = this.running;
			}
		}
	}

	// Token: 0x040009E1 RID: 2529
	[SerializeField]
	private CharacterModel characterModel;

	// Token: 0x040009E2 RID: 2530
	public List<ParticleSystem> runParticles;

	// Token: 0x040009E3 RID: 2531
	private bool running;
}
