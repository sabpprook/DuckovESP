using System;
using Duckov.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

// Token: 0x020001E8 RID: 488
public class TaskSkipperUI : MonoBehaviour
{
	// Token: 0x06000E61 RID: 3681 RVA: 0x00039E14 File Offset: 0x00038014
	private void Awake()
	{
		UIInputManager.OnInteractInputContext += this.OnInteractInputContext;
		this.anyButtonListener = InputSystem.onAnyButtonPress.Call(new Action<InputControl>(this.OnAnyButton));
		this.skipped = false;
		this.alpha = 0f;
	}

	// Token: 0x06000E62 RID: 3682 RVA: 0x00039E60 File Offset: 0x00038060
	private void OnAnyButton(InputControl control)
	{
		this.Show();
	}

	// Token: 0x06000E63 RID: 3683 RVA: 0x00039E68 File Offset: 0x00038068
	private void OnDestroy()
	{
		UIInputManager.OnInteractInputContext -= this.OnInteractInputContext;
		this.anyButtonListener.Dispose();
	}

	// Token: 0x06000E64 RID: 3684 RVA: 0x00039E86 File Offset: 0x00038086
	private void OnInteractInputContext(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			this.pressing = true;
		}
		if (context.canceled)
		{
			this.pressing = false;
		}
	}

	// Token: 0x06000E65 RID: 3685 RVA: 0x00039EA8 File Offset: 0x000380A8
	private void Update()
	{
		this.UpdatePressing();
		this.UpdateFill();
		this.UpdateCanvasGroup();
	}

	// Token: 0x06000E66 RID: 3686 RVA: 0x00039EBC File Offset: 0x000380BC
	private void Show()
	{
		this.show = true;
		this.hideTimer = this.hideAfterSeconds;
	}

	// Token: 0x06000E67 RID: 3687 RVA: 0x00039ED4 File Offset: 0x000380D4
	private void UpdatePressing()
	{
		if (UIInputManager.Instance == null)
		{
			this.pressing = Keyboard.current.fKey.isPressed;
		}
		if (this.pressing && !this.skipped)
		{
			this.pressTime += Time.deltaTime;
			if (this.pressTime >= this.totalTime)
			{
				this.skipped = true;
				this.target.Skip();
			}
			this.Show();
			return;
		}
		if (!this.skipped)
		{
			this.pressTime = Mathf.MoveTowards(this.pressTime, 0f, Time.deltaTime);
		}
	}

	// Token: 0x06000E68 RID: 3688 RVA: 0x00039F70 File Offset: 0x00038170
	private void UpdateFill()
	{
		float num = this.pressTime / this.totalTime;
		this.fill.fillAmount = num;
	}

	// Token: 0x06000E69 RID: 3689 RVA: 0x00039F98 File Offset: 0x00038198
	private void UpdateCanvasGroup()
	{
		if (this.show)
		{
			this.alpha = Mathf.MoveTowards(this.alpha, 1f, 10f * Time.deltaTime);
			this.hideTimer = Mathf.MoveTowards(this.hideTimer, 0f, Time.deltaTime);
			if (this.hideTimer < 0.01f)
			{
				this.show = false;
			}
		}
		else
		{
			this.alpha = Mathf.MoveTowards(this.alpha, 0f, 10f * Time.deltaTime);
		}
		this.canvasGroup.alpha = this.alpha;
	}

	// Token: 0x04000BEC RID: 3052
	[SerializeField]
	private TaskList target;

	// Token: 0x04000BED RID: 3053
	[SerializeField]
	private CanvasGroup canvasGroup;

	// Token: 0x04000BEE RID: 3054
	[SerializeField]
	private Image fill;

	// Token: 0x04000BEF RID: 3055
	[SerializeField]
	private float totalTime = 2f;

	// Token: 0x04000BF0 RID: 3056
	[SerializeField]
	private float hideAfterSeconds = 2f;

	// Token: 0x04000BF1 RID: 3057
	private float pressTime;

	// Token: 0x04000BF2 RID: 3058
	private float alpha;

	// Token: 0x04000BF3 RID: 3059
	private float hideTimer;

	// Token: 0x04000BF4 RID: 3060
	private bool show;

	// Token: 0x04000BF5 RID: 3061
	private IDisposable anyButtonListener;

	// Token: 0x04000BF6 RID: 3062
	private bool pressing;

	// Token: 0x04000BF7 RID: 3063
	private bool skipped;
}
