using System;
using Duckov;
using UnityEngine;

// Token: 0x0200019D RID: 413
public class PostAudioEventOnEnter : StateMachineBehaviour
{
	// Token: 0x06000C33 RID: 3123 RVA: 0x00033793 File Offset: 0x00031993
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		AudioManager.Post(this.eventName, animator.gameObject);
	}

	// Token: 0x04000A93 RID: 2707
	[SerializeField]
	private string eventName;
}
