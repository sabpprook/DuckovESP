using System;
using UnityEngine;

// Token: 0x020000A8 RID: 168
public class PaperBox : MonoBehaviour
{
	// Token: 0x060005AE RID: 1454 RVA: 0x000196E4 File Offset: 0x000178E4
	private void Update()
	{
		if (!this.character)
		{
			return;
		}
		if (!this.setActiveWhileStandStill)
		{
			return;
		}
		bool flag = this.character.Velocity.magnitude < 0.2f;
		if (this.setActiveWhileStandStill.gameObject.activeSelf != flag)
		{
			this.setActiveWhileStandStill.gameObject.SetActive(flag);
		}
	}

	// Token: 0x0400052F RID: 1327
	[HideInInspector]
	public CharacterMainControl character;

	// Token: 0x04000530 RID: 1328
	public Transform setActiveWhileStandStill;
}
