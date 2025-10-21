using System;
using Cysharp.Threading.Tasks;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Token: 0x020001DC RID: 476
public class UIKeybindingEntry : MonoBehaviour
{
	// Token: 0x17000295 RID: 661
	// (get) Token: 0x06000E25 RID: 3621 RVA: 0x000392A4 File Offset: 0x000374A4
	// (set) Token: 0x06000E26 RID: 3622 RVA: 0x000392F3 File Offset: 0x000374F3
	[LocalizationKey("UIText")]
	private string displayNameKey
	{
		get
		{
			if (!string.IsNullOrEmpty(this.overrideDisplayNameKey))
			{
				return this.overrideDisplayNameKey;
			}
			if (this.actionRef == null)
			{
				return "?";
			}
			return "Input_" + this.actionRef.action.name;
		}
		set
		{
		}
	}

	// Token: 0x06000E27 RID: 3623 RVA: 0x000392F5 File Offset: 0x000374F5
	private void Awake()
	{
		this.rebindButton.onClick.AddListener(new UnityAction(this.OnButtonClick));
		this.Setup();
		LocalizationManager.OnSetLanguage += this.OnLanguageChanged;
	}

	// Token: 0x06000E28 RID: 3624 RVA: 0x0003932A File Offset: 0x0003752A
	private void OnDestroy()
	{
		LocalizationManager.OnSetLanguage -= this.OnLanguageChanged;
	}

	// Token: 0x06000E29 RID: 3625 RVA: 0x0003933D File Offset: 0x0003753D
	private void OnLanguageChanged(SystemLanguage language)
	{
		this.label.text = this.displayNameKey.ToPlainText();
	}

	// Token: 0x06000E2A RID: 3626 RVA: 0x00039355 File Offset: 0x00037555
	private void OnButtonClick()
	{
		InputRebinder.RebindAsync(this.actionRef.action.name, this.index, this.excludes, true).Forget<bool>();
	}

	// Token: 0x06000E2B RID: 3627 RVA: 0x0003937E File Offset: 0x0003757E
	private void OnValidate()
	{
		this.Setup();
	}

	// Token: 0x06000E2C RID: 3628 RVA: 0x00039386 File Offset: 0x00037586
	private void Setup()
	{
		this.indicator.Setup(this.actionRef, this.index);
		this.label.text = this.displayNameKey.ToPlainText();
	}

	// Token: 0x04000BB7 RID: 2999
	[SerializeField]
	private InputActionReference actionRef;

	// Token: 0x04000BB8 RID: 3000
	[SerializeField]
	private int index;

	// Token: 0x04000BB9 RID: 3001
	[SerializeField]
	private string overrideDisplayNameKey;

	// Token: 0x04000BBA RID: 3002
	private string[] excludes = new string[] { "<Mouse>/leftButton", "<Mouse>/rightButton", "<Pointer>/position", "<Pointer>/delta", "<Pointer>/press", "<Mouse>/scroll" };

	// Token: 0x04000BBB RID: 3003
	[SerializeField]
	private TextMeshProUGUI label;

	// Token: 0x04000BBC RID: 3004
	[SerializeField]
	private Button rebindButton;

	// Token: 0x04000BBD RID: 3005
	[SerializeField]
	private InputIndicator indicator;
}
