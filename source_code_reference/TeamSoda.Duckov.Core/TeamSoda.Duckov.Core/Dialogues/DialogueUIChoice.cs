using System;
using System.Collections.Generic;
using DG.Tweening;
using NodeCanvas.DialogueTrees;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dialogues
{
	// Token: 0x02000219 RID: 537
	public class DialogueUIChoice : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler
	{
		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x0600101D RID: 4125 RVA: 0x0003EC9A File Offset: 0x0003CE9A
		public int Index
		{
			get
			{
				return this.index;
			}
		}

		// Token: 0x0600101E RID: 4126 RVA: 0x0003ECA4 File Offset: 0x0003CEA4
		private void Awake()
		{
			MenuItem menuItem = this.menuItem;
			menuItem.onSelected = (Action<MenuItem>)Delegate.Combine(menuItem.onSelected, new Action<MenuItem>(this.Refresh));
			MenuItem menuItem2 = this.menuItem;
			menuItem2.onDeselected = (Action<MenuItem>)Delegate.Combine(menuItem2.onDeselected, new Action<MenuItem>(this.Refresh));
			MenuItem menuItem3 = this.menuItem;
			menuItem3.onFocusStatusChanged = (Action<MenuItem, bool>)Delegate.Combine(menuItem3.onFocusStatusChanged, new Action<MenuItem, bool>(this.Refresh));
			MenuItem menuItem4 = this.menuItem;
			menuItem4.onConfirmed = (Action<MenuItem>)Delegate.Combine(menuItem4.onConfirmed, new Action<MenuItem>(this.OnConfirm));
		}

		// Token: 0x0600101F RID: 4127 RVA: 0x0003ED4D File Offset: 0x0003CF4D
		private void OnConfirm(MenuItem item)
		{
			this.Confirm();
		}

		// Token: 0x06001020 RID: 4128 RVA: 0x0003ED58 File Offset: 0x0003CF58
		private void AnimateConfirm()
		{
			this.confirmIndicator.DOKill(false);
			this.confirmIndicator.DOGradientColor(this.confirmAnimationColor, this.confirmAnimationDuration).OnComplete(delegate
			{
				this.confirmIndicator.color = Color.clear;
			}).OnKill(delegate
			{
				this.confirmIndicator.color = Color.clear;
			});
		}

		// Token: 0x06001021 RID: 4129 RVA: 0x0003EDAC File Offset: 0x0003CFAC
		private void Refresh(MenuItem item, bool focus)
		{
			this.selectionIndicator.SetActive(this.menuItem.IsSelected);
		}

		// Token: 0x06001022 RID: 4130 RVA: 0x0003EDC4 File Offset: 0x0003CFC4
		private void Refresh(MenuItem item)
		{
			this.selectionIndicator.SetActive(this.menuItem.IsSelected);
		}

		// Token: 0x06001023 RID: 4131 RVA: 0x0003EDDC File Offset: 0x0003CFDC
		private void Confirm()
		{
			this.master.NotifyChoiceConfirmed(this);
			this.AnimateConfirm();
		}

		// Token: 0x06001024 RID: 4132 RVA: 0x0003EDF0 File Offset: 0x0003CFF0
		public void OnPointerClick(PointerEventData eventData)
		{
			this.Confirm();
		}

		// Token: 0x06001025 RID: 4133 RVA: 0x0003EDF8 File Offset: 0x0003CFF8
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.menuItem.Select();
		}

		// Token: 0x06001026 RID: 4134 RVA: 0x0003EE08 File Offset: 0x0003D008
		internal void Setup(DialogueUI master, KeyValuePair<IStatement, int> cur)
		{
			this.master = master;
			this.index = cur.Value;
			this.text.text = cur.Key.text;
			this.confirmIndicator.color = Color.clear;
			this.Refresh(this.menuItem);
		}

		// Token: 0x04000CE6 RID: 3302
		[SerializeField]
		private MenuItem menuItem;

		// Token: 0x04000CE7 RID: 3303
		[SerializeField]
		private GameObject selectionIndicator;

		// Token: 0x04000CE8 RID: 3304
		[SerializeField]
		private Image confirmIndicator;

		// Token: 0x04000CE9 RID: 3305
		[SerializeField]
		private Gradient confirmAnimationColor;

		// Token: 0x04000CEA RID: 3306
		[SerializeField]
		private float confirmAnimationDuration = 0.2f;

		// Token: 0x04000CEB RID: 3307
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x04000CEC RID: 3308
		private DialogueUI master;

		// Token: 0x04000CED RID: 3309
		private int index;
	}
}
