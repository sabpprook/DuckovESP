using System;
using UnityEngine;

// Token: 0x020000D7 RID: 215
public class DoorTrigger : MonoBehaviour
{
	// Token: 0x060006B3 RID: 1715 RVA: 0x0001E1C4 File Offset: 0x0001C3C4
	private void OnTriggerEnter(Collider collision)
	{
		if (this.parent.IsOpen)
		{
			return;
		}
		if (!this.parent.NoRequireItem)
		{
			return;
		}
		if (this.parent.Interact && !this.parent.Interact.gameObject.activeInHierarchy)
		{
			return;
		}
		if (collision.gameObject.layer != LayerMask.NameToLayer("Character"))
		{
			return;
		}
		CharacterMainControl component = collision.gameObject.GetComponent<CharacterMainControl>();
		if (!component || component.Team == Teams.player)
		{
			return;
		}
		this.parent.Open();
	}

	// Token: 0x04000675 RID: 1653
	public Door parent;
}
