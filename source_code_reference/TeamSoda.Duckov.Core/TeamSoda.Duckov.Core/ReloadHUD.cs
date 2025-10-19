using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020000C8 RID: 200
public class ReloadHUD : MonoBehaviour
{
	// Token: 0x06000645 RID: 1605 RVA: 0x0001C428 File Offset: 0x0001A628
	private void Update()
	{
		if (this.characterMainControl == null)
		{
			this.characterMainControl = LevelManager.Instance.MainCharacter;
			if (this.characterMainControl == null)
			{
				return;
			}
			this.button.onClick.AddListener(new UnityAction(this.Reload));
		}
		this.reloadable = this.characterMainControl.GetGunReloadable();
		if (this.reloadable != this.button.interactable)
		{
			this.button.interactable = this.reloadable;
			if (this.reloadable)
			{
				UnityEvent onShowEvent = this.OnShowEvent;
				if (onShowEvent != null)
				{
					onShowEvent.Invoke();
				}
			}
			else
			{
				UnityEvent onHideEvent = this.OnHideEvent;
				if (onHideEvent != null)
				{
					onHideEvent.Invoke();
				}
			}
		}
		this.frame++;
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x0001C4ED File Offset: 0x0001A6ED
	private void OnDestroy()
	{
		this.button.onClick.RemoveAllListeners();
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x0001C4FF File Offset: 0x0001A6FF
	private void Reload()
	{
		if (this.characterMainControl)
		{
			this.characterMainControl.TryToReload(null);
		}
	}

	// Token: 0x04000606 RID: 1542
	private CharacterMainControl characterMainControl;

	// Token: 0x04000607 RID: 1543
	public Button button;

	// Token: 0x04000608 RID: 1544
	private bool reloadable;

	// Token: 0x04000609 RID: 1545
	public UnityEvent OnShowEvent;

	// Token: 0x0400060A RID: 1546
	public UnityEvent OnHideEvent;

	// Token: 0x0400060B RID: 1547
	private int frame;
}
