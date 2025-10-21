using System;

// Token: 0x020000D9 RID: 217
public class InteractableCarriable : InteractableBase
{
	// Token: 0x060006DE RID: 1758 RVA: 0x0001EF9C File Offset: 0x0001D19C
	protected override void Start()
	{
		base.Start();
		this.finishWhenTimeOut = true;
	}

	// Token: 0x060006DF RID: 1759 RVA: 0x0001EFAB File Offset: 0x0001D1AB
	protected override bool IsInteractable()
	{
		return true;
	}

	// Token: 0x060006E0 RID: 1760 RVA: 0x0001EFAE File Offset: 0x0001D1AE
	protected override void OnInteractStart(CharacterMainControl character)
	{
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x0001EFB0 File Offset: 0x0001D1B0
	protected override void OnInteractFinished()
	{
		if (!this.interactCharacter)
		{
			return;
		}
		CharacterMainControl interactCharacter = this.interactCharacter;
		base.StopInteract();
		interactCharacter.Carry(this.carryTarget);
	}

	// Token: 0x04000698 RID: 1688
	public Carriable carryTarget;
}
