using System;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using Duckov.UI.Animations;
using ItemStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001F3 RID: 499
public class SplitDialogue : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x170002A0 RID: 672
	// (get) Token: 0x06000E93 RID: 3731 RVA: 0x0003A526 File Offset: 0x00038726
	public static SplitDialogue Instance
	{
		get
		{
			if (GameplayUIManager.Instance == null)
			{
				return null;
			}
			return GameplayUIManager.Instance.SplitDialogue;
		}
	}

	// Token: 0x06000E94 RID: 3732 RVA: 0x0003A541 File Offset: 0x00038741
	private void OnEnable()
	{
		View.OnActiveViewChanged += this.OnActiveViewChanged;
	}

	// Token: 0x06000E95 RID: 3733 RVA: 0x0003A554 File Offset: 0x00038754
	private void OnDisable()
	{
		View.OnActiveViewChanged -= this.OnActiveViewChanged;
	}

	// Token: 0x06000E96 RID: 3734 RVA: 0x0003A567 File Offset: 0x00038767
	private void OnActiveViewChanged()
	{
		this.Hide();
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x0003A56F File Offset: 0x0003876F
	private void Awake()
	{
		this.confirmButton.onClick.AddListener(new UnityAction(this.OnConfirmButtonClicked));
		this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderValueChanged));
	}

	// Token: 0x06000E98 RID: 3736 RVA: 0x0003A5A9 File Offset: 0x000387A9
	private void OnSliderValueChanged(float value)
	{
		this.RefreshCountText();
	}

	// Token: 0x06000E99 RID: 3737 RVA: 0x0003A5B4 File Offset: 0x000387B4
	private void RefreshCountText()
	{
		this.countText.text = this.slider.value.ToString("0");
	}

	// Token: 0x06000E9A RID: 3738 RVA: 0x0003A5E4 File Offset: 0x000387E4
	private void OnConfirmButtonClicked()
	{
		if (this.status != SplitDialogue.Status.Normal)
		{
			return;
		}
		this.Confirm().Forget();
	}

	// Token: 0x06000E9B RID: 3739 RVA: 0x0003A5FC File Offset: 0x000387FC
	private void Setup(Item target, Inventory destination = null, int destinationIndex = -1)
	{
		this.target = target;
		this.destination = destination;
		this.destinationIndex = destinationIndex;
		this.slider.minValue = 1f;
		this.slider.maxValue = (float)target.StackCount;
		this.slider.value = (float)(target.StackCount - 1) / 2f;
		this.RefreshCountText();
		this.SwitchStatus(SplitDialogue.Status.Normal);
		this.cachedInInventory = target.InInventory;
	}

	// Token: 0x06000E9C RID: 3740 RVA: 0x0003A673 File Offset: 0x00038873
	public void Cancel()
	{
		if (this.status != SplitDialogue.Status.Normal)
		{
			return;
		}
		this.SwitchStatus(SplitDialogue.Status.Canceled);
		this.Hide();
	}

	// Token: 0x06000E9D RID: 3741 RVA: 0x0003A68C File Offset: 0x0003888C
	private async UniTask Confirm()
	{
		if (this.status == SplitDialogue.Status.Normal)
		{
			if (this.cachedInInventory == this.target.InInventory)
			{
				this.SwitchStatus(SplitDialogue.Status.Busy);
				await this.DoSplit(Mathf.RoundToInt(this.slider.value));
			}
			this.SwitchStatus(SplitDialogue.Status.Complete);
			this.Hide();
		}
	}

	// Token: 0x06000E9E RID: 3742 RVA: 0x0003A6CF File Offset: 0x000388CF
	private void Hide()
	{
		this.fadeGroup.Hide();
	}

	// Token: 0x06000E9F RID: 3743 RVA: 0x0003A6DC File Offset: 0x000388DC
	private async UniTask DoSplit(int value)
	{
		if (value != 0)
		{
			if (value == this.target.StackCount)
			{
				this.<DoSplit>g__Send|24_0(this.target);
			}
			else
			{
				Item item = await this.target.Split(value);
				this.<DoSplit>g__Send|24_0(item);
				ItemUIUtilities.Select(null);
			}
		}
	}

	// Token: 0x06000EA0 RID: 3744 RVA: 0x0003A728 File Offset: 0x00038928
	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.pointerCurrentRaycast.gameObject == base.gameObject)
		{
			this.Cancel();
		}
	}

	// Token: 0x06000EA1 RID: 3745 RVA: 0x0003A758 File Offset: 0x00038958
	private void SwitchStatus(SplitDialogue.Status status)
	{
		this.status = status;
		this.normalIndicator.SetActive(status == SplitDialogue.Status.Normal);
		this.busyIndicator.SetActive(status == SplitDialogue.Status.Busy);
		this.completeIndicator.SetActive(status == SplitDialogue.Status.Complete);
		switch (status)
		{
		default:
			return;
		}
	}

	// Token: 0x06000EA2 RID: 3746 RVA: 0x0003A7B3 File Offset: 0x000389B3
	public static void SetupAndShow(Item item)
	{
		if (SplitDialogue.Instance == null)
		{
			return;
		}
		SplitDialogue.Instance.Setup(item, null, -1);
		SplitDialogue.Instance.fadeGroup.Show();
	}

	// Token: 0x06000EA3 RID: 3747 RVA: 0x0003A7DF File Offset: 0x000389DF
	public static void SetupAndShow(Item item, Inventory destinationInventory, int destinationIndex)
	{
		if (SplitDialogue.Instance == null)
		{
			return;
		}
		SplitDialogue.Instance.Setup(item, destinationInventory, destinationIndex);
		SplitDialogue.Instance.fadeGroup.Show();
	}

	// Token: 0x06000EA5 RID: 3749 RVA: 0x0003A814 File Offset: 0x00038A14
	[CompilerGenerated]
	private void <DoSplit>g__Send|24_0(Item item)
	{
		item.Detach();
		if (this.destination != null && this.destination.Capacity > this.destinationIndex && this.destination.GetItemAt(this.destinationIndex) == null)
		{
			this.destination.AddAt(item, this.destinationIndex);
			return;
		}
		ItemUtilities.SendToPlayerCharacterInventory(item, true);
	}

	// Token: 0x04000C10 RID: 3088
	[SerializeField]
	private FadeGroup fadeGroup;

	// Token: 0x04000C11 RID: 3089
	[SerializeField]
	private Button confirmButton;

	// Token: 0x04000C12 RID: 3090
	[SerializeField]
	private TextMeshProUGUI countText;

	// Token: 0x04000C13 RID: 3091
	[SerializeField]
	private GameObject normalIndicator;

	// Token: 0x04000C14 RID: 3092
	[SerializeField]
	private GameObject busyIndicator;

	// Token: 0x04000C15 RID: 3093
	[SerializeField]
	private GameObject completeIndicator;

	// Token: 0x04000C16 RID: 3094
	[SerializeField]
	private Slider slider;

	// Token: 0x04000C17 RID: 3095
	private Item target;

	// Token: 0x04000C18 RID: 3096
	private Inventory destination;

	// Token: 0x04000C19 RID: 3097
	private int destinationIndex;

	// Token: 0x04000C1A RID: 3098
	private Inventory cachedInInventory;

	// Token: 0x04000C1B RID: 3099
	private SplitDialogue.Status status;

	// Token: 0x020004DA RID: 1242
	private enum Status
	{
		// Token: 0x04001CFE RID: 7422
		Idle,
		// Token: 0x04001CFF RID: 7423
		Normal,
		// Token: 0x04001D00 RID: 7424
		Busy,
		// Token: 0x04001D01 RID: 7425
		Complete,
		// Token: 0x04001D02 RID: 7426
		Canceled
	}
}
