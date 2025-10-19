using System;
using Duckov.UI;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x02000170 RID: 368
public class UIInputManager : MonoBehaviour
{
	// Token: 0x1700021C RID: 540
	// (get) Token: 0x06000B0D RID: 2829 RVA: 0x0002F019 File Offset: 0x0002D219
	public static UIInputManager Instance
	{
		get
		{
			return GameManager.UiInputManager;
		}
	}

	// Token: 0x14000050 RID: 80
	// (add) Token: 0x06000B0E RID: 2830 RVA: 0x0002F020 File Offset: 0x0002D220
	// (remove) Token: 0x06000B0F RID: 2831 RVA: 0x0002F054 File Offset: 0x0002D254
	public static event Action<UIInputEventData> OnNavigate;

	// Token: 0x14000051 RID: 81
	// (add) Token: 0x06000B10 RID: 2832 RVA: 0x0002F088 File Offset: 0x0002D288
	// (remove) Token: 0x06000B11 RID: 2833 RVA: 0x0002F0BC File Offset: 0x0002D2BC
	public static event Action<UIInputEventData> OnConfirm;

	// Token: 0x14000052 RID: 82
	// (add) Token: 0x06000B12 RID: 2834 RVA: 0x0002F0F0 File Offset: 0x0002D2F0
	// (remove) Token: 0x06000B13 RID: 2835 RVA: 0x0002F124 File Offset: 0x0002D324
	public static event Action<UIInputEventData> OnToggleIndicatorHUD;

	// Token: 0x14000053 RID: 83
	// (add) Token: 0x06000B14 RID: 2836 RVA: 0x0002F158 File Offset: 0x0002D358
	// (remove) Token: 0x06000B15 RID: 2837 RVA: 0x0002F18C File Offset: 0x0002D38C
	public static event Action<UIInputEventData> OnCancelEarly;

	// Token: 0x14000054 RID: 84
	// (add) Token: 0x06000B16 RID: 2838 RVA: 0x0002F1C0 File Offset: 0x0002D3C0
	// (remove) Token: 0x06000B17 RID: 2839 RVA: 0x0002F1F4 File Offset: 0x0002D3F4
	public static event Action<UIInputEventData> OnCancel;

	// Token: 0x14000055 RID: 85
	// (add) Token: 0x06000B18 RID: 2840 RVA: 0x0002F228 File Offset: 0x0002D428
	// (remove) Token: 0x06000B19 RID: 2841 RVA: 0x0002F25C File Offset: 0x0002D45C
	public static event Action<UIInputEventData> OnFastPick;

	// Token: 0x14000056 RID: 86
	// (add) Token: 0x06000B1A RID: 2842 RVA: 0x0002F290 File Offset: 0x0002D490
	// (remove) Token: 0x06000B1B RID: 2843 RVA: 0x0002F2C4 File Offset: 0x0002D4C4
	public static event Action<UIInputEventData> OnDropItem;

	// Token: 0x14000057 RID: 87
	// (add) Token: 0x06000B1C RID: 2844 RVA: 0x0002F2F8 File Offset: 0x0002D4F8
	// (remove) Token: 0x06000B1D RID: 2845 RVA: 0x0002F32C File Offset: 0x0002D52C
	public static event Action<UIInputEventData> OnUseItem;

	// Token: 0x14000058 RID: 88
	// (add) Token: 0x06000B1E RID: 2846 RVA: 0x0002F360 File Offset: 0x0002D560
	// (remove) Token: 0x06000B1F RID: 2847 RVA: 0x0002F394 File Offset: 0x0002D594
	public static event Action<UIInputEventData> OnToggleCameraMode;

	// Token: 0x14000059 RID: 89
	// (add) Token: 0x06000B20 RID: 2848 RVA: 0x0002F3C8 File Offset: 0x0002D5C8
	// (remove) Token: 0x06000B21 RID: 2849 RVA: 0x0002F3FC File Offset: 0x0002D5FC
	public static event Action<UIInputEventData> OnWishlistHoveringItem;

	// Token: 0x1400005A RID: 90
	// (add) Token: 0x06000B22 RID: 2850 RVA: 0x0002F430 File Offset: 0x0002D630
	// (remove) Token: 0x06000B23 RID: 2851 RVA: 0x0002F464 File Offset: 0x0002D664
	public static event Action<UIInputEventData> OnNextPage;

	// Token: 0x1400005B RID: 91
	// (add) Token: 0x06000B24 RID: 2852 RVA: 0x0002F498 File Offset: 0x0002D698
	// (remove) Token: 0x06000B25 RID: 2853 RVA: 0x0002F4CC File Offset: 0x0002D6CC
	public static event Action<UIInputEventData> OnPreviousPage;

	// Token: 0x1400005C RID: 92
	// (add) Token: 0x06000B26 RID: 2854 RVA: 0x0002F500 File Offset: 0x0002D700
	// (remove) Token: 0x06000B27 RID: 2855 RVA: 0x0002F534 File Offset: 0x0002D734
	public static event Action<UIInputEventData> OnLockInventoryIndex;

	// Token: 0x1400005D RID: 93
	// (add) Token: 0x06000B28 RID: 2856 RVA: 0x0002F568 File Offset: 0x0002D768
	// (remove) Token: 0x06000B29 RID: 2857 RVA: 0x0002F59C File Offset: 0x0002D79C
	public static event Action<UIInputEventData, int> OnShortcutInput;

	// Token: 0x1400005E RID: 94
	// (add) Token: 0x06000B2A RID: 2858 RVA: 0x0002F5D0 File Offset: 0x0002D7D0
	// (remove) Token: 0x06000B2B RID: 2859 RVA: 0x0002F604 File Offset: 0x0002D804
	public static event Action<InputAction.CallbackContext> OnInteractInputContext;

	// Token: 0x1700021D RID: 541
	// (get) Token: 0x06000B2C RID: 2860 RVA: 0x0002F637 File Offset: 0x0002D837
	public static bool Ctrl
	{
		get
		{
			return Keyboard.current != null && Keyboard.current.ctrlKey.isPressed;
		}
	}

	// Token: 0x1700021E RID: 542
	// (get) Token: 0x06000B2D RID: 2861 RVA: 0x0002F651 File Offset: 0x0002D851
	public static bool Alt
	{
		get
		{
			return Keyboard.current != null && Keyboard.current.altKey.isPressed;
		}
	}

	// Token: 0x1700021F RID: 543
	// (get) Token: 0x06000B2E RID: 2862 RVA: 0x0002F66B File Offset: 0x0002D86B
	public static bool Shift
	{
		get
		{
			return Keyboard.current != null && Keyboard.current.shiftKey.isPressed;
		}
	}

	// Token: 0x17000220 RID: 544
	// (get) Token: 0x06000B2F RID: 2863 RVA: 0x0002F688 File Offset: 0x0002D888
	public static Vector2 Point
	{
		get
		{
			if (!Application.isPlaying)
			{
				return default(Vector2);
			}
			if (UIInputManager.Instance == null)
			{
				return default(Vector2);
			}
			if (UIInputManager.Instance.inputActionPoint == null)
			{
				return default(Vector2);
			}
			return UIInputManager.Instance.inputActionPoint.ReadValue<Vector2>();
		}
	}

	// Token: 0x17000221 RID: 545
	// (get) Token: 0x06000B30 RID: 2864 RVA: 0x0002F6E4 File Offset: 0x0002D8E4
	public static Vector2 MouseDelta
	{
		get
		{
			if (!Application.isPlaying)
			{
				return default(Vector2);
			}
			if (UIInputManager.Instance == null)
			{
				return default(Vector2);
			}
			if (UIInputManager.Instance.inputActionMouseDelta == null)
			{
				return default(Vector2);
			}
			return UIInputManager.Instance.inputActionMouseDelta.ReadValue<Vector2>();
		}
	}

	// Token: 0x17000222 RID: 546
	// (get) Token: 0x06000B31 RID: 2865 RVA: 0x0002F73E File Offset: 0x0002D93E
	public static bool WasClickedThisFrame
	{
		get
		{
			return Application.isPlaying && !(UIInputManager.Instance == null) && UIInputManager.Instance.inputActionMouseClick != null && UIInputManager.Instance.inputActionMouseClick.WasPressedThisFrame();
		}
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x0002F778 File Offset: 0x0002D978
	public static Ray GetPointRay()
	{
		if (UIInputManager.Instance == null)
		{
			return default(Ray);
		}
		GameCamera instance = GameCamera.Instance;
		if (instance == null)
		{
			return default(Ray);
		}
		return instance.renderCamera.ScreenPointToRay(UIInputManager.Point);
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x0002F7CC File Offset: 0x0002D9CC
	private void Awake()
	{
		if (UIInputManager.Instance != this)
		{
			return;
		}
		InputActionAsset actions = GameManager.MainPlayerInput.actions;
		this.inputActionNavigate = actions["UI_Navigate"];
		this.inputActionConfirm = actions["UI_Confirm"];
		this.inputActionCancel = actions["UI_Cancel"];
		this.inputActionPoint = actions["Point"];
		this.inputActionFastPick = actions["Interact"];
		this.inputActionDropItem = actions["UI_Item_Drop"];
		this.inputActionUseItem = actions["UI_Item_use"];
		this.inputActionToggleIndicatorHUD = actions["UI_ToggleIndicatorHUD"];
		this.inputActionToggleCameraMode = actions["UI_ToggleCameraMode"];
		this.inputActionWishlistHoveringItem = actions["UI_WishlistHoveringItem"];
		this.inputActionNextPage = actions["UI_NextPage"];
		this.inputActionPreviousPage = actions["UI_PreviousPage"];
		this.inputActionLockInventoryIndex = actions["UI_LockInventoryIndex"];
		this.inputActionMouseDelta = actions["MouseDelta"];
		this.inputActionMouseClick = actions["Click"];
		this.inputActionInteract = actions["Interact"];
		this.Bind(this.inputActionNavigate, new Action<InputAction.CallbackContext>(this.OnInputActionNavigate));
		this.Bind(this.inputActionConfirm, new Action<InputAction.CallbackContext>(this.OnInputActionConfirm));
		this.Bind(this.inputActionCancel, new Action<InputAction.CallbackContext>(this.OnInputActionCancel));
		this.Bind(this.inputActionFastPick, new Action<InputAction.CallbackContext>(this.OnInputActionFastPick));
		this.Bind(this.inputActionDropItem, new Action<InputAction.CallbackContext>(this.OnInputActionDropItem));
		this.Bind(this.inputActionUseItem, new Action<InputAction.CallbackContext>(this.OnInputActionUseItem));
		this.Bind(this.inputActionToggleIndicatorHUD, new Action<InputAction.CallbackContext>(this.OnInputActionToggleIndicatorHUD));
		this.Bind(this.inputActionToggleCameraMode, new Action<InputAction.CallbackContext>(this.OnInputActionToggleCameraMode));
		this.Bind(this.inputActionWishlistHoveringItem, new Action<InputAction.CallbackContext>(this.OnInputWishlistHoveringItem));
		this.Bind(this.inputActionNextPage, new Action<InputAction.CallbackContext>(this.OnInputActionNextPage));
		this.Bind(this.inputActionPreviousPage, new Action<InputAction.CallbackContext>(this.OnInputActionPrevioursPage));
		this.Bind(this.inputActionLockInventoryIndex, new Action<InputAction.CallbackContext>(this.OnInputActionLockInventoryIndex));
		this.Bind(this.inputActionInteract, new Action<InputAction.CallbackContext>(this.OnInputActionInteract));
	}

	// Token: 0x06000B34 RID: 2868 RVA: 0x0002FA3C File Offset: 0x0002DC3C
	private void OnDestroy()
	{
		this.UnBind(this.inputActionNavigate, new Action<InputAction.CallbackContext>(this.OnInputActionNavigate));
		this.UnBind(this.inputActionConfirm, new Action<InputAction.CallbackContext>(this.OnInputActionConfirm));
		this.UnBind(this.inputActionCancel, new Action<InputAction.CallbackContext>(this.OnInputActionCancel));
		this.UnBind(this.inputActionFastPick, new Action<InputAction.CallbackContext>(this.OnInputActionFastPick));
		this.UnBind(this.inputActionUseItem, new Action<InputAction.CallbackContext>(this.OnInputActionUseItem));
		this.UnBind(this.inputActionToggleIndicatorHUD, new Action<InputAction.CallbackContext>(this.OnInputActionToggleIndicatorHUD));
		this.UnBind(this.inputActionToggleCameraMode, new Action<InputAction.CallbackContext>(this.OnInputActionToggleCameraMode));
		this.UnBind(this.inputActionWishlistHoveringItem, new Action<InputAction.CallbackContext>(this.OnInputWishlistHoveringItem));
		this.UnBind(this.inputActionNextPage, new Action<InputAction.CallbackContext>(this.OnInputActionNextPage));
		this.UnBind(this.inputActionPreviousPage, new Action<InputAction.CallbackContext>(this.OnInputActionPrevioursPage));
		this.UnBind(this.inputActionLockInventoryIndex, new Action<InputAction.CallbackContext>(this.OnInputActionLockInventoryIndex));
		this.UnBind(this.inputActionInteract, new Action<InputAction.CallbackContext>(this.OnInputActionInteract));
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x0002FB69 File Offset: 0x0002DD69
	private void OnInputActionInteract(InputAction.CallbackContext context)
	{
		Action<InputAction.CallbackContext> onInteractInputContext = UIInputManager.OnInteractInputContext;
		if (onInteractInputContext == null)
		{
			return;
		}
		onInteractInputContext(context);
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x0002FB7B File Offset: 0x0002DD7B
	private void OnInputActionLockInventoryIndex(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			Action<UIInputEventData> onLockInventoryIndex = UIInputManager.OnLockInventoryIndex;
			if (onLockInventoryIndex == null)
			{
				return;
			}
			onLockInventoryIndex(new UIInputEventData());
		}
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x0002FB9A File Offset: 0x0002DD9A
	private void OnInputActionNextPage(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			Action<UIInputEventData> onNextPage = UIInputManager.OnNextPage;
			if (onNextPage == null)
			{
				return;
			}
			onNextPage(new UIInputEventData());
		}
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x0002FBB9 File Offset: 0x0002DDB9
	private void OnInputActionPrevioursPage(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			Action<UIInputEventData> onPreviousPage = UIInputManager.OnPreviousPage;
			if (onPreviousPage == null)
			{
				return;
			}
			onPreviousPage(new UIInputEventData());
		}
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x0002FBD8 File Offset: 0x0002DDD8
	private void OnInputWishlistHoveringItem(InputAction.CallbackContext context)
	{
		if (!context.started)
		{
			return;
		}
		Action<UIInputEventData> onWishlistHoveringItem = UIInputManager.OnWishlistHoveringItem;
		if (onWishlistHoveringItem == null)
		{
			return;
		}
		onWishlistHoveringItem(new UIInputEventData());
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x0002FBF8 File Offset: 0x0002DDF8
	private void OnInputActionToggleCameraMode(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			Action<UIInputEventData> onToggleCameraMode = UIInputManager.OnToggleCameraMode;
			if (onToggleCameraMode == null)
			{
				return;
			}
			onToggleCameraMode(new UIInputEventData());
		}
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x0002FC17 File Offset: 0x0002DE17
	private void OnInputActionDropItem(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			Action<UIInputEventData> onDropItem = UIInputManager.OnDropItem;
			if (onDropItem == null)
			{
				return;
			}
			onDropItem(new UIInputEventData());
		}
	}

	// Token: 0x06000B3C RID: 2876 RVA: 0x0002FC36 File Offset: 0x0002DE36
	private void OnInputActionUseItem(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			Action<UIInputEventData> onUseItem = UIInputManager.OnUseItem;
			if (onUseItem == null)
			{
				return;
			}
			onUseItem(new UIInputEventData());
		}
	}

	// Token: 0x06000B3D RID: 2877 RVA: 0x0002FC55 File Offset: 0x0002DE55
	private void OnInputActionFastPick(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			Action<UIInputEventData> onFastPick = UIInputManager.OnFastPick;
			if (onFastPick == null)
			{
				return;
			}
			onFastPick(new UIInputEventData());
		}
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x0002FC74 File Offset: 0x0002DE74
	private void OnInputActionCancel(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			UIInputEventData uiinputEventData = new UIInputEventData
			{
				cancel = true
			};
			Action<UIInputEventData> onCancelEarly = UIInputManager.OnCancelEarly;
			if (onCancelEarly != null)
			{
				onCancelEarly(uiinputEventData);
			}
			if (uiinputEventData.Used)
			{
				return;
			}
			Action<UIInputEventData> onCancel = UIInputManager.OnCancel;
			if (onCancel != null)
			{
				onCancel(uiinputEventData);
			}
			if (uiinputEventData.Used)
			{
				return;
			}
			if (LevelManager.Instance != null && View.ActiveView == null)
			{
				PauseMenu.Toggle();
			}
		}
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x0002FCEA File Offset: 0x0002DEEA
	private void OnInputActionConfirm(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			Action<UIInputEventData> onConfirm = UIInputManager.OnConfirm;
			if (onConfirm == null)
			{
				return;
			}
			onConfirm(new UIInputEventData
			{
				confirm = true
			});
		}
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x0002FD10 File Offset: 0x0002DF10
	private void OnInputActionNavigate(InputAction.CallbackContext context)
	{
		Vector2 vector = context.ReadValue<Vector2>();
		Action<UIInputEventData> onNavigate = UIInputManager.OnNavigate;
		if (onNavigate == null)
		{
			return;
		}
		onNavigate(new UIInputEventData
		{
			vector = vector
		});
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x0002FD40 File Offset: 0x0002DF40
	private void OnInputActionToggleIndicatorHUD(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			Action<UIInputEventData> onToggleIndicatorHUD = UIInputManager.OnToggleIndicatorHUD;
			if (onToggleIndicatorHUD == null)
			{
				return;
			}
			onToggleIndicatorHUD(new UIInputEventData());
		}
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x0002FD5F File Offset: 0x0002DF5F
	private void Bind(InputAction inputAction, Action<InputAction.CallbackContext> action)
	{
		inputAction.Enable();
		inputAction.started += action;
		inputAction.performed += action;
		inputAction.canceled += action;
	}

	// Token: 0x06000B43 RID: 2883 RVA: 0x0002FD7C File Offset: 0x0002DF7C
	private void UnBind(InputAction inputAction, Action<InputAction.CallbackContext> action)
	{
		if (inputAction != null)
		{
			inputAction.started -= action;
			inputAction.performed -= action;
			inputAction.canceled -= action;
		}
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x0002FD96 File Offset: 0x0002DF96
	internal static void NotifyShortcutInput(int index)
	{
		UIInputManager.OnShortcutInput(new UIInputEventData
		{
			confirm = true
		}, index);
	}

	// Token: 0x04000979 RID: 2425
	private static bool instantiated;

	// Token: 0x0400097A RID: 2426
	private InputAction inputActionNavigate;

	// Token: 0x0400097B RID: 2427
	private InputAction inputActionConfirm;

	// Token: 0x0400097C RID: 2428
	private InputAction inputActionCancel;

	// Token: 0x0400097D RID: 2429
	private InputAction inputActionPoint;

	// Token: 0x0400097E RID: 2430
	private InputAction inputActionMouseDelta;

	// Token: 0x0400097F RID: 2431
	private InputAction inputActionMouseClick;

	// Token: 0x04000980 RID: 2432
	private InputAction inputActionFastPick;

	// Token: 0x04000981 RID: 2433
	private InputAction inputActionDropItem;

	// Token: 0x04000982 RID: 2434
	private InputAction inputActionUseItem;

	// Token: 0x04000983 RID: 2435
	private InputAction inputActionToggleIndicatorHUD;

	// Token: 0x04000984 RID: 2436
	private InputAction inputActionToggleCameraMode;

	// Token: 0x04000985 RID: 2437
	private InputAction inputActionWishlistHoveringItem;

	// Token: 0x04000986 RID: 2438
	private InputAction inputActionNextPage;

	// Token: 0x04000987 RID: 2439
	private InputAction inputActionPreviousPage;

	// Token: 0x04000988 RID: 2440
	private InputAction inputActionLockInventoryIndex;

	// Token: 0x04000989 RID: 2441
	private InputAction inputActionInteract;
}
