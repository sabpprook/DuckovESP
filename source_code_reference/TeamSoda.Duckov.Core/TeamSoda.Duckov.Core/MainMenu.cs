using System;
using UnityEngine;

// Token: 0x02000166 RID: 358
public class MainMenu : MonoBehaviour
{
	// Token: 0x06000AE0 RID: 2784 RVA: 0x0002EB0C File Offset: 0x0002CD0C
	private void Awake()
	{
		Action onMainMenuAwake = MainMenu.OnMainMenuAwake;
		if (onMainMenuAwake == null)
		{
			return;
		}
		onMainMenuAwake();
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x0002EB1D File Offset: 0x0002CD1D
	private void OnDestroy()
	{
		Action onMainMenuDestroy = MainMenu.OnMainMenuDestroy;
		if (onMainMenuDestroy == null)
		{
			return;
		}
		onMainMenuDestroy();
	}

	// Token: 0x0400095F RID: 2399
	public static Action OnMainMenuAwake;

	// Token: 0x04000960 RID: 2400
	public static Action OnMainMenuDestroy;
}
