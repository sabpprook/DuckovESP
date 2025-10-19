using System;
using Duckov.UI.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Duckov.UI
{
	// Token: 0x0200037D RID: 893
	public class Tooltips : MonoBehaviour
	{
		// Token: 0x170005F4 RID: 1524
		// (get) Token: 0x06001EEB RID: 7915 RVA: 0x0006C72E File Offset: 0x0006A92E
		// (set) Token: 0x06001EEC RID: 7916 RVA: 0x0006C735 File Offset: 0x0006A935
		public static ITooltipsProvider CurrentProvider { get; private set; }

		// Token: 0x06001EED RID: 7917 RVA: 0x0006C73D File Offset: 0x0006A93D
		public static void NotifyEnterTooltipsProvider(ITooltipsProvider provider)
		{
			Tooltips.CurrentProvider = provider;
			Action<ITooltipsProvider> onEnterProvider = Tooltips.OnEnterProvider;
			if (onEnterProvider == null)
			{
				return;
			}
			onEnterProvider(provider);
		}

		// Token: 0x06001EEE RID: 7918 RVA: 0x0006C755 File Offset: 0x0006A955
		public static void NotifyExitTooltipsProvider(ITooltipsProvider provider)
		{
			if (Tooltips.CurrentProvider != provider)
			{
				return;
			}
			Tooltips.CurrentProvider = null;
			Action<ITooltipsProvider> onExitProvider = Tooltips.OnExitProvider;
			if (onExitProvider == null)
			{
				return;
			}
			onExitProvider(provider);
		}

		// Token: 0x06001EEF RID: 7919 RVA: 0x0006C778 File Offset: 0x0006A978
		private void Awake()
		{
			if (this.rectTransform == null)
			{
				this.rectTransform = base.GetComponent<RectTransform>();
			}
			Tooltips.OnEnterProvider = (Action<ITooltipsProvider>)Delegate.Combine(Tooltips.OnEnterProvider, new Action<ITooltipsProvider>(this.DoOnEnterProvider));
			Tooltips.OnExitProvider = (Action<ITooltipsProvider>)Delegate.Combine(Tooltips.OnExitProvider, new Action<ITooltipsProvider>(this.DoOnExitProvider));
		}

		// Token: 0x06001EF0 RID: 7920 RVA: 0x0006C7E0 File Offset: 0x0006A9E0
		private void OnDestroy()
		{
			Tooltips.OnEnterProvider = (Action<ITooltipsProvider>)Delegate.Remove(Tooltips.OnEnterProvider, new Action<ITooltipsProvider>(this.DoOnEnterProvider));
			Tooltips.OnExitProvider = (Action<ITooltipsProvider>)Delegate.Remove(Tooltips.OnExitProvider, new Action<ITooltipsProvider>(this.DoOnExitProvider));
		}

		// Token: 0x06001EF1 RID: 7921 RVA: 0x0006C82D File Offset: 0x0006AA2D
		private void Update()
		{
			if (this.contents.gameObject.activeSelf)
			{
				this.RefreshPosition();
			}
		}

		// Token: 0x06001EF2 RID: 7922 RVA: 0x0006C847 File Offset: 0x0006AA47
		private void DoOnExitProvider(ITooltipsProvider provider)
		{
			this.fadeGroup.Hide();
		}

		// Token: 0x06001EF3 RID: 7923 RVA: 0x0006C854 File Offset: 0x0006AA54
		private void DoOnEnterProvider(ITooltipsProvider provider)
		{
			this.text.text = provider.GetTooltipsText();
			this.fadeGroup.Show();
		}

		// Token: 0x06001EF4 RID: 7924 RVA: 0x0006C874 File Offset: 0x0006AA74
		private unsafe void RefreshPosition()
		{
			Vector2 vector = *Mouse.current.position.value;
			Vector2 vector2;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, vector, null, out vector2);
			this.contents.localPosition = vector2;
		}

		// Token: 0x0400152C RID: 5420
		[SerializeField]
		private RectTransform rectTransform;

		// Token: 0x0400152D RID: 5421
		[SerializeField]
		private RectTransform contents;

		// Token: 0x0400152E RID: 5422
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x0400152F RID: 5423
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x04001531 RID: 5425
		private static Action<ITooltipsProvider> OnEnterProvider;

		// Token: 0x04001532 RID: 5426
		private static Action<ITooltipsProvider> OnExitProvider;
	}
}
