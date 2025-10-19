using System;
using Duckov.UI.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x020001BA RID: 442
public class InputRebinderIndicator : MonoBehaviour
{
	// Token: 0x06000D27 RID: 3367 RVA: 0x00036B3C File Offset: 0x00034D3C
	private void Awake()
	{
		InputRebinder.OnRebindBegin = (Action<InputAction>)Delegate.Combine(InputRebinder.OnRebindBegin, new Action<InputAction>(this.OnRebindBegin));
		InputRebinder.OnRebindComplete = (Action<InputAction>)Delegate.Combine(InputRebinder.OnRebindComplete, new Action<InputAction>(this.OnRebindComplete));
		this.fadeGroup.SkipHide();
	}

	// Token: 0x06000D28 RID: 3368 RVA: 0x00036B94 File Offset: 0x00034D94
	private void OnRebindComplete(InputAction action)
	{
		this.fadeGroup.Hide();
	}

	// Token: 0x06000D29 RID: 3369 RVA: 0x00036BA1 File Offset: 0x00034DA1
	private void OnRebindBegin(InputAction action)
	{
		this.fadeGroup.Show();
	}

	// Token: 0x04000B48 RID: 2888
	[SerializeField]
	private FadeGroup fadeGroup;
}
