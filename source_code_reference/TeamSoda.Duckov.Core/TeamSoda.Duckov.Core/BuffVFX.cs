using System;
using Duckov.Buffs;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000174 RID: 372
public class BuffVFX : MonoBehaviour
{
	// Token: 0x06000B53 RID: 2899 RVA: 0x000300C3 File Offset: 0x0002E2C3
	private void Awake()
	{
		if (!this.buff)
		{
			this.buff = base.GetComponent<Buff>();
		}
		this.buff.OnSetupEvent.AddListener(new UnityAction(this.OnSetup));
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x000300FC File Offset: 0x0002E2FC
	private void OnSetup()
	{
		if (this.shockFxInstance != null)
		{
			global::UnityEngine.Object.Destroy(this.shockFxInstance);
		}
		if (!this.buff || !this.buff.Character || !this.shockFxPfb)
		{
			return;
		}
		this.shockFxInstance = global::UnityEngine.Object.Instantiate<GameObject>(this.shockFxPfb, this.buff.Character.transform);
		this.shockFxInstance.transform.localPosition = this.offsetFromCharacter;
		this.shockFxInstance.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x06000B55 RID: 2901 RVA: 0x0003019B File Offset: 0x0002E39B
	private void OnDestroy()
	{
		if (this.shockFxInstance != null)
		{
			global::UnityEngine.Object.Destroy(this.shockFxInstance);
		}
	}

	// Token: 0x06000B56 RID: 2902 RVA: 0x000301B6 File Offset: 0x0002E3B6
	public void AutoSetup()
	{
		this.buff = base.GetComponent<Buff>();
	}

	// Token: 0x040009A6 RID: 2470
	public Buff buff;

	// Token: 0x040009A7 RID: 2471
	public GameObject shockFxPfb;

	// Token: 0x040009A8 RID: 2472
	private GameObject shockFxInstance;

	// Token: 0x040009A9 RID: 2473
	public Vector3 offsetFromCharacter;
}
