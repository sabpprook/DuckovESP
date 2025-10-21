using System;
using System.Collections.Generic;
using System.Linq;
using Dialogues;
using Duckov.UI;
using Duckov.UI.Animations;
using UnityEngine;

// Token: 0x020001FA RID: 506
public class HUDManager : MonoBehaviour
{
	// Token: 0x14000068 RID: 104
	// (add) Token: 0x06000ECE RID: 3790 RVA: 0x0003AEF0 File Offset: 0x000390F0
	// (remove) Token: 0x06000ECF RID: 3791 RVA: 0x0003AF24 File Offset: 0x00039124
	private static event Action onHideTokensChanged;

	// Token: 0x170002A7 RID: 679
	// (get) Token: 0x06000ED0 RID: 3792 RVA: 0x0003AF58 File Offset: 0x00039158
	private bool ShouldDisplay
	{
		get
		{
			bool flag = HUDManager.hideTokens.Any((global::UnityEngine.Object e) => e != null);
			bool flag2 = View.ActiveView != null;
			bool active = DialogueUI.Active;
			bool flag3 = CustomFaceUI.ActiveView != null;
			bool active2 = CameraMode.Active;
			return !flag && !flag2 && !active && !flag3 && !active2;
		}
	}

	// Token: 0x06000ED1 RID: 3793 RVA: 0x0003AFC4 File Offset: 0x000391C4
	private void Awake()
	{
		View.OnActiveViewChanged += this.OnActiveViewChanged;
		DialogueUI.OnDialogueStatusChanged += this.OnDialogueStatusChanged;
		CustomFaceUI.OnCustomUIViewChanged += this.OnCustomFaceViewChange;
		CameraMode.OnCameraModeChanged = (Action<bool>)Delegate.Combine(CameraMode.OnCameraModeChanged, new Action<bool>(this.OnCameraModeChanged));
		HUDManager.onHideTokensChanged += this.OnHideTokensChanged;
	}

	// Token: 0x06000ED2 RID: 3794 RVA: 0x0003B038 File Offset: 0x00039238
	private void OnDestroy()
	{
		View.OnActiveViewChanged -= this.OnActiveViewChanged;
		DialogueUI.OnDialogueStatusChanged -= this.OnDialogueStatusChanged;
		CustomFaceUI.OnCustomUIViewChanged -= this.OnCustomFaceViewChange;
		CameraMode.OnCameraModeChanged = (Action<bool>)Delegate.Remove(CameraMode.OnCameraModeChanged, new Action<bool>(this.OnCameraModeChanged));
		HUDManager.onHideTokensChanged -= this.OnHideTokensChanged;
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x0003B0A9 File Offset: 0x000392A9
	private void OnHideTokensChanged()
	{
		this.Refresh();
	}

	// Token: 0x06000ED4 RID: 3796 RVA: 0x0003B0B1 File Offset: 0x000392B1
	private void OnCameraModeChanged(bool value)
	{
		this.Refresh();
	}

	// Token: 0x06000ED5 RID: 3797 RVA: 0x0003B0B9 File Offset: 0x000392B9
	private void OnDialogueStatusChanged()
	{
		this.Refresh();
	}

	// Token: 0x06000ED6 RID: 3798 RVA: 0x0003B0C1 File Offset: 0x000392C1
	private void OnActiveViewChanged()
	{
		this.Refresh();
	}

	// Token: 0x06000ED7 RID: 3799 RVA: 0x0003B0C9 File Offset: 0x000392C9
	private void OnCustomFaceViewChange()
	{
		this.Refresh();
	}

	// Token: 0x06000ED8 RID: 3800 RVA: 0x0003B0D4 File Offset: 0x000392D4
	private void Refresh()
	{
		if (this.ShouldDisplay)
		{
			this.canvasGroup.blocksRaycasts = true;
			if (this.fadeGroup.IsShown)
			{
				return;
			}
			this.fadeGroup.Show();
			return;
		}
		else
		{
			this.canvasGroup.blocksRaycasts = false;
			if (this.fadeGroup.IsHidden)
			{
				return;
			}
			this.fadeGroup.Hide();
			return;
		}
	}

	// Token: 0x06000ED9 RID: 3801 RVA: 0x0003B134 File Offset: 0x00039334
	public static void RegisterHideToken(global::UnityEngine.Object obj)
	{
		HUDManager.hideTokens.Add(obj);
		Action action = HUDManager.onHideTokensChanged;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x06000EDA RID: 3802 RVA: 0x0003B150 File Offset: 0x00039350
	public static void UnregisterHideToken(global::UnityEngine.Object obj)
	{
		HUDManager.hideTokens.Remove(obj);
		Action action = HUDManager.onHideTokensChanged;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x04000C3A RID: 3130
	[SerializeField]
	private FadeGroup fadeGroup;

	// Token: 0x04000C3B RID: 3131
	[SerializeField]
	private CanvasGroup canvasGroup;

	// Token: 0x04000C3C RID: 3132
	private static List<global::UnityEngine.Object> hideTokens = new List<global::UnityEngine.Object>();
}
