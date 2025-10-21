using System;
using UnityEngine;

// Token: 0x0200018B RID: 395
public class ShootRangeRing : MonoBehaviour
{
	// Token: 0x06000BBD RID: 3005 RVA: 0x00031C98 File Offset: 0x0002FE98
	private void Awake()
	{
	}

	// Token: 0x06000BBE RID: 3006 RVA: 0x00031C9C File Offset: 0x0002FE9C
	private void Update()
	{
		if (!this.character)
		{
			this.character = LevelManager.Instance.MainCharacter;
			this.character.OnHoldAgentChanged += this.OnAgentChanged;
			this.OnAgentChanged(this.character.CurrentHoldItemAgent);
			return;
		}
		if (this.ringRenderer.gameObject.activeInHierarchy && !this.gunAgent)
		{
			this.ringRenderer.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000BBF RID: 3007 RVA: 0x00031D20 File Offset: 0x0002FF20
	private void LateUpdate()
	{
		if (!this.character)
		{
			return;
		}
		base.transform.rotation = Quaternion.LookRotation(this.character.CurrentAimDirection, Vector3.up);
		base.transform.position = this.character.transform.position;
	}

	// Token: 0x06000BC0 RID: 3008 RVA: 0x00031D76 File Offset: 0x0002FF76
	private void OnDestroy()
	{
		if (this.character)
		{
			this.character.OnHoldAgentChanged -= this.OnAgentChanged;
		}
	}

	// Token: 0x06000BC1 RID: 3009 RVA: 0x00031D9C File Offset: 0x0002FF9C
	private void OnAgentChanged(DuckovItemAgent agent)
	{
		if (agent == null)
		{
			return;
		}
		this.gunAgent = this.character.GetGun();
		if (this.gunAgent)
		{
			this.ringRenderer.gameObject.SetActive(true);
			this.ringRenderer.transform.localScale = Vector3.one * this.character.GetAimRange() * 0.5f;
			return;
		}
		this.ringRenderer.gameObject.SetActive(false);
	}

	// Token: 0x04000A16 RID: 2582
	private CharacterMainControl character;

	// Token: 0x04000A17 RID: 2583
	public MeshRenderer ringRenderer;

	// Token: 0x04000A18 RID: 2584
	private ItemAgent_Gun gunAgent;
}
