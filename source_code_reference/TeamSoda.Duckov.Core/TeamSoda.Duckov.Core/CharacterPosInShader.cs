using System;
using UnityEngine;

// Token: 0x020000FE RID: 254
public class CharacterPosInShader : MonoBehaviour
{
	// Token: 0x0600086B RID: 2155 RVA: 0x0002582F File Offset: 0x00023A2F
	private void Update()
	{
		if (!CharacterMainControl.Main)
		{
			return;
		}
		Shader.SetGlobalVector(this.characterPosHash, CharacterMainControl.Main.transform.position);
	}

	// Token: 0x040007A8 RID: 1960
	private int characterPosHash = Shader.PropertyToID("CharacterPos");
}
