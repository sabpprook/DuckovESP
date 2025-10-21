using System;
using UnityEngine;

// Token: 0x0200016F RID: 367
public class SetActiveByInputDevice : MonoBehaviour
{
	// Token: 0x06000B09 RID: 2825 RVA: 0x0002EFBD File Offset: 0x0002D1BD
	private void Awake()
	{
		this.OnInputDeviceChanged();
		InputManager.OnInputDeviceChanged += this.OnInputDeviceChanged;
	}

	// Token: 0x06000B0A RID: 2826 RVA: 0x0002EFD6 File Offset: 0x0002D1D6
	private void OnDestroy()
	{
		InputManager.OnInputDeviceChanged -= this.OnInputDeviceChanged;
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x0002EFE9 File Offset: 0x0002D1E9
	private void OnInputDeviceChanged()
	{
		if (InputManager.InputDevice == this.activeIfDeviceIs)
		{
			base.gameObject.SetActive(true);
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000978 RID: 2424
	public InputManager.InputDevices activeIfDeviceIs;
}
