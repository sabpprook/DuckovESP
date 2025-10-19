using System;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

// Token: 0x020000C2 RID: 194
public class InteractSelectionHUD : MonoBehaviour
{
	// Token: 0x1700012C RID: 300
	// (get) Token: 0x06000628 RID: 1576 RVA: 0x0001BB9A File Offset: 0x00019D9A
	public InteractableBase InteractTarget
	{
		get
		{
			return this.interactable;
		}
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x0001BBA2 File Offset: 0x00019DA2
	public void SetInteractable(InteractableBase _interactable, bool _hasUpDown)
	{
		this.interactable = _interactable;
		this.text.text = this.interactable.GetInteractName();
		this.UpdateRequireItem(this.interactable);
		this.selectionPoint.SetActive(_hasUpDown);
		this.hasUpDown = _hasUpDown;
	}

	// Token: 0x0600062A RID: 1578 RVA: 0x0001BBE0 File Offset: 0x00019DE0
	private void UpdateRequireItem(InteractableBase interactable)
	{
		if (!interactable || !interactable.requireItem)
		{
			this.requireCanvasGroup.alpha = 0f;
			return;
		}
		this.requireCanvasGroup.alpha = 1f;
		CharacterMainControl mainCharacter = LevelManager.Instance.MainCharacter;
		bool flag = interactable.whenToUseRequireItem > InteractableBase.WhenToUseRequireItemTypes.None;
		string text = (flag ? this.requirUseItemTextKey.ToPlainText() : this.requirItemTextKey.ToPlainText());
		this.requireText.text = text + " " + interactable.GetRequiredItemName();
		if (flag)
		{
			TextMeshProUGUI textMeshProUGUI = this.requireText;
			textMeshProUGUI.text += " x1";
		}
		this.requirementIcon.sprite = interactable.GetRequireditemIcon();
		if (interactable.TryGetRequiredItem(mainCharacter).Item1)
		{
			this.requireItemBackgroundImage.color = this.hasRequireItemColor;
			return;
		}
		this.requireItemBackgroundImage.color = this.noRequireItemColor;
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x0001BCCC File Offset: 0x00019ECC
	public void SetSelection(bool _select)
	{
		this.selecting = _select;
		this.selectIndicator.SetActive(this.selecting);
		this.upDownIndicator.SetActive(this.selecting && this.hasUpDown);
		this.selectionPoint.SetActive(!this.selecting && this.hasUpDown);
		if (_select)
		{
			UnityEvent onSelectedEvent = this.OnSelectedEvent;
			if (onSelectedEvent != null)
			{
				onSelectedEvent.Invoke();
			}
			this.background.color = this.selectedColor;
			return;
		}
		this.background.color = this.unselectedColor;
	}

	// Token: 0x040005D7 RID: 1495
	private InteractableBase interactable;

	// Token: 0x040005D8 RID: 1496
	public GameObject selectIndicator;

	// Token: 0x040005D9 RID: 1497
	public TextMeshProUGUI text;

	// Token: 0x040005DA RID: 1498
	public ProceduralImage background;

	// Token: 0x040005DB RID: 1499
	public Color selectedColor;

	// Token: 0x040005DC RID: 1500
	public Color unselectedColor;

	// Token: 0x040005DD RID: 1501
	public CanvasGroup requireCanvasGroup;

	// Token: 0x040005DE RID: 1502
	public ProceduralImage requireItemBackgroundImage;

	// Token: 0x040005DF RID: 1503
	public TextMeshProUGUI requireText;

	// Token: 0x040005E0 RID: 1504
	[LocalizationKey("UI")]
	public string requirItemTextKey = "UI_RequireItem";

	// Token: 0x040005E1 RID: 1505
	[LocalizationKey("UI")]
	public string requirUseItemTextKey = "UI_RequireUseItem";

	// Token: 0x040005E2 RID: 1506
	public Image requirementIcon;

	// Token: 0x040005E3 RID: 1507
	public Color hasRequireItemColor;

	// Token: 0x040005E4 RID: 1508
	public Color noRequireItemColor;

	// Token: 0x040005E5 RID: 1509
	private bool selecting;

	// Token: 0x040005E6 RID: 1510
	public UnityEvent OnSelectedEvent;

	// Token: 0x040005E7 RID: 1511
	public GameObject selectionPoint;

	// Token: 0x040005E8 RID: 1512
	public GameObject upDownIndicator;

	// Token: 0x040005E9 RID: 1513
	private bool hasUpDown;
}
