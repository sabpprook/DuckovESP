using System;
using UnityEngine;

// Token: 0x0200008A RID: 138
public class AISpecialAttachmentBase : MonoBehaviour
{
	// Token: 0x060004E0 RID: 1248 RVA: 0x000160A2 File Offset: 0x000142A2
	public void Init(AICharacterController _ai, CharacterMainControl _character)
	{
		this.aiCharacterController = _ai;
		this.character = _character;
		this.OnInited();
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x000160B8 File Offset: 0x000142B8
	protected virtual void OnInited()
	{
	}

	// Token: 0x04000418 RID: 1048
	public AICharacterController aiCharacterController;

	// Token: 0x04000419 RID: 1049
	public CharacterMainControl character;
}
