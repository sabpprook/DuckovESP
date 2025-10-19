using System;
using Duckov.UI.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020001F9 RID: 505
public class MultiInteractionMenuButton : MonoBehaviour
{
	// Token: 0x06000EC8 RID: 3784 RVA: 0x0003AE56 File Offset: 0x00039056
	private void Awake()
	{
		this.button.onClick.AddListener(new UnityAction(this.OnButtonClicked));
	}

	// Token: 0x06000EC9 RID: 3785 RVA: 0x0003AE74 File Offset: 0x00039074
	private void OnButtonClicked()
	{
		if (this.target == null)
		{
			return;
		}
		CharacterMainControl main = CharacterMainControl.Main;
		if (main == null)
		{
			return;
		}
		main.Interact(this.target);
	}

	// Token: 0x06000ECA RID: 3786 RVA: 0x0003AE9A File Offset: 0x0003909A
	internal void Setup(InteractableBase target)
	{
		base.gameObject.SetActive(true);
		this.target = target;
		this.text.text = target.InteractName;
		this.fadeGroup.SkipHide();
	}

	// Token: 0x06000ECB RID: 3787 RVA: 0x0003AECB File Offset: 0x000390CB
	internal void Show()
	{
		this.fadeGroup.Show();
	}

	// Token: 0x06000ECC RID: 3788 RVA: 0x0003AED8 File Offset: 0x000390D8
	internal void Hide()
	{
		this.fadeGroup.Hide();
	}

	// Token: 0x04000C36 RID: 3126
	[SerializeField]
	private Button button;

	// Token: 0x04000C37 RID: 3127
	[SerializeField]
	private TextMeshProUGUI text;

	// Token: 0x04000C38 RID: 3128
	[SerializeField]
	private FadeGroup fadeGroup;

	// Token: 0x04000C39 RID: 3129
	private InteractableBase target;
}
