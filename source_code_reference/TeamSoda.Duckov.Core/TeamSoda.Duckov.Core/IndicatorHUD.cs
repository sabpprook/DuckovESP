using System;
using UnityEngine;

// Token: 0x020000BC RID: 188
public class IndicatorHUD : MonoBehaviour
{
	// Token: 0x06000612 RID: 1554 RVA: 0x0001B478 File Offset: 0x00019678
	private void Start()
	{
		if ((LevelManager.Instance == null || LevelManager.Instance.IsBaseLevel) && this.mapIndicator)
		{
			this.mapIndicator.SetActive(false);
		}
		this.toggleParent.SetActive(this.startActive);
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x0001B4C8 File Offset: 0x000196C8
	private void Awake()
	{
		UIInputManager.OnToggleIndicatorHUD += this.Toggle;
	}

	// Token: 0x06000614 RID: 1556 RVA: 0x0001B4DB File Offset: 0x000196DB
	private void OnDestroy()
	{
		UIInputManager.OnToggleIndicatorHUD -= this.Toggle;
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x0001B4EE File Offset: 0x000196EE
	private void Toggle(UIInputEventData data)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.toggleParent.SetActive(!this.toggleParent.activeInHierarchy);
	}

	// Token: 0x040005B1 RID: 1457
	public GameObject mapIndicator;

	// Token: 0x040005B2 RID: 1458
	public GameObject toggleParent;

	// Token: 0x040005B3 RID: 1459
	public bool startActive;
}
