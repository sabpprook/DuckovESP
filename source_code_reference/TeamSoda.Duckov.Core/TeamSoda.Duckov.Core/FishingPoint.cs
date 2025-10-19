using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000A3 RID: 163
public class FishingPoint : MonoBehaviour
{
	// Token: 0x06000598 RID: 1432 RVA: 0x00019253 File Offset: 0x00017453
	private void Awake()
	{
		this.OnPlayerTakeFishingRod(null);
		this.Interactable.OnInteractFinishedEvent.AddListener(new UnityAction<CharacterMainControl, InteractableBase>(this.OnInteractFinished));
	}

	// Token: 0x06000599 RID: 1433 RVA: 0x00019278 File Offset: 0x00017478
	private void OnDestroy()
	{
		if (this.Interactable)
		{
			this.Interactable.OnInteractFinishedEvent.RemoveListener(new UnityAction<CharacterMainControl, InteractableBase>(this.OnInteractFinished));
		}
	}

	// Token: 0x0600059A RID: 1434 RVA: 0x000192A3 File Offset: 0x000174A3
	private void OnPlayerTakeFishingRod(FishingRod rod)
	{
	}

	// Token: 0x0600059B RID: 1435 RVA: 0x000192A8 File Offset: 0x000174A8
	private void OnInteractFinished(CharacterMainControl character, InteractableBase interact)
	{
		if (!character)
		{
			return;
		}
		character.SetPosition(this.playerPoint.position);
		character.SetAimPoint(this.playerPoint.position + this.playerPoint.forward * 10f);
		character.movementControl.SetAimDirection(this.playerPoint.forward);
		character.StartAction(this.action);
	}

	// Token: 0x04000513 RID: 1299
	public InteractableBase Interactable;

	// Token: 0x04000514 RID: 1300
	public Action_Fishing action;

	// Token: 0x04000515 RID: 1301
	public Transform playerPoint;
}
