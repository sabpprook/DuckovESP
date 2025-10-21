using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020001DB RID: 475
public class UIButtonRevertBinding : MonoBehaviour
{
	// Token: 0x06000E22 RID: 3618 RVA: 0x00039257 File Offset: 0x00037457
	private void Awake()
	{
		if (this.button == null)
		{
			this.button = base.GetComponent<Button>();
		}
		this.button.onClick.AddListener(new UnityAction(this.OnBtnClick));
	}

	// Token: 0x06000E23 RID: 3619 RVA: 0x0003928F File Offset: 0x0003748F
	public void OnBtnClick()
	{
		InputRebinder.Clear();
		InputRebinder.Save();
	}

	// Token: 0x04000BB6 RID: 2998
	[SerializeField]
	private Button button;
}
