using System;
using Cysharp.Threading.Tasks;
using Saves;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x020001B9 RID: 441
public class InputRebinder : MonoBehaviour
{
	// Token: 0x06000D18 RID: 3352 RVA: 0x0003678D File Offset: 0x0003498D
	public void Rebind()
	{
		InputRebinder.RebindAsync(this.action, this.index, this.excludes, false).Forget<bool>();
	}

	// Token: 0x17000266 RID: 614
	// (get) Token: 0x06000D19 RID: 3353 RVA: 0x000367AC File Offset: 0x000349AC
	private static PlayerInput PlayerInput
	{
		get
		{
			return GameManager.MainPlayerInput;
		}
	}

	// Token: 0x17000267 RID: 615
	// (get) Token: 0x06000D1A RID: 3354 RVA: 0x000367B3 File Offset: 0x000349B3
	private static bool OperationPending
	{
		get
		{
			return InputRebinder.operation.started && !InputRebinder.operation.canceled && !InputRebinder.operation.completed;
		}
	}

	// Token: 0x06000D1B RID: 3355 RVA: 0x000367DE File Offset: 0x000349DE
	private void Awake()
	{
		InputRebinder.Load();
		UIInputManager.OnCancelEarly += this.OnUICancel;
	}

	// Token: 0x06000D1C RID: 3356 RVA: 0x000367F6 File Offset: 0x000349F6
	private void OnDestroy()
	{
		UIInputManager.OnCancelEarly -= this.OnUICancel;
	}

	// Token: 0x06000D1D RID: 3357 RVA: 0x00036809 File Offset: 0x00034A09
	private void OnUICancel(UIInputEventData data)
	{
		if (InputRebinder.OperationPending)
		{
			data.Use();
		}
	}

	// Token: 0x06000D1E RID: 3358 RVA: 0x00036818 File Offset: 0x00034A18
	public static void Load()
	{
		string text = SavesSystem.LoadGlobal<string>("InputBinding", null);
		string.IsNullOrEmpty(text);
		try
		{
			InputRebinder.PlayerInput.actions.LoadBindingOverridesFromJson(text, true);
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
			InputRebinder.PlayerInput.actions.RemoveAllBindingOverrides();
		}
	}

	// Token: 0x06000D1F RID: 3359 RVA: 0x00036874 File Offset: 0x00034A74
	public static void Save()
	{
		string text = InputRebinder.PlayerInput.actions.SaveBindingOverridesAsJson();
		SavesSystem.SaveGlobal<string>("InputBinding", text);
		Debug.Log(text);
	}

	// Token: 0x06000D20 RID: 3360 RVA: 0x000368A2 File Offset: 0x00034AA2
	public static void Clear()
	{
		InputRebinder.PlayerInput.actions.RemoveAllBindingOverrides();
		Action onBindingChanged = InputRebinder.OnBindingChanged;
		if (onBindingChanged != null)
		{
			onBindingChanged();
		}
		InputIndicator.NotifyBindingChanged();
	}

	// Token: 0x06000D21 RID: 3361 RVA: 0x000368C8 File Offset: 0x00034AC8
	private static void Rebind(string name, int index, string[] excludes = null)
	{
		if (InputRebinder.OperationPending)
		{
			return;
		}
		InputAction inputAction = InputRebinder.PlayerInput.actions[name];
		if (inputAction == null)
		{
			Debug.LogError("找不到名为 " + name + " 的 action");
			return;
		}
		Action<InputAction> onRebindBegin = InputRebinder.OnRebindBegin;
		if (onRebindBegin != null)
		{
			onRebindBegin(inputAction);
		}
		Debug.Log("Resetting");
		InputRebinder.operation.Reset();
		Debug.Log("Settingup");
		inputAction.actionMap.Disable();
		InputRebinder.operation.WithCancelingThrough("<Keyboard>/escape").WithAction(inputAction).WithTargetBinding(index)
			.OnComplete(new Action<InputActionRebindingExtensions.RebindingOperation>(InputRebinder.OnComplete))
			.OnCancel(new Action<InputActionRebindingExtensions.RebindingOperation>(InputRebinder.OnCancel));
		if (excludes != null)
		{
			foreach (string text in excludes)
			{
				InputRebinder.operation.WithControlsExcluding(text);
			}
		}
		Debug.Log("Starting");
		InputRebinder.operation.Start();
	}

	// Token: 0x06000D22 RID: 3362 RVA: 0x000369B8 File Offset: 0x00034BB8
	public static async UniTask<bool> RebindAsync(string name, int index, string[] excludes = null, bool save = false)
	{
		bool flag;
		if (InputRebinder.OperationPending)
		{
			flag = false;
		}
		else
		{
			InputRebinder.Rebind(name, index, excludes);
			while (InputRebinder.OperationPending)
			{
				await UniTask.Yield();
			}
			if (save && InputRebinder.operation.completed)
			{
				InputRebinder.Save();
			}
			flag = InputRebinder.operation.completed;
		}
		return flag;
	}

	// Token: 0x06000D23 RID: 3363 RVA: 0x00036A14 File Offset: 0x00034C14
	private static void OnCancel(InputActionRebindingExtensions.RebindingOperation operation)
	{
		Debug.Log(operation.action.name + " binding canceled");
		operation.action.actionMap.Enable();
		Action<InputAction> onRebindComplete = InputRebinder.OnRebindComplete;
		if (onRebindComplete == null)
		{
			return;
		}
		onRebindComplete(operation.action);
	}

	// Token: 0x06000D24 RID: 3364 RVA: 0x00036A60 File Offset: 0x00034C60
	private static void OnComplete(InputActionRebindingExtensions.RebindingOperation operation)
	{
		Debug.Log(operation.action.name + " bind to " + operation.selectedControl.name);
		operation.action.actionMap.Enable();
		Action<InputAction> onRebindComplete = InputRebinder.OnRebindComplete;
		if (onRebindComplete != null)
		{
			onRebindComplete(operation.action);
		}
		Action onBindingChanged = InputRebinder.OnBindingChanged;
		if (onBindingChanged != null)
		{
			onBindingChanged();
		}
		InputIndicator.NotifyRebindComplete(operation.action);
	}

	// Token: 0x04000B40 RID: 2880
	[Header("Debug")]
	[SerializeField]
	private string action = "MoveAxis";

	// Token: 0x04000B41 RID: 2881
	[SerializeField]
	private int index = 2;

	// Token: 0x04000B42 RID: 2882
	[SerializeField]
	private string[] excludes = new string[] { "<Mouse>/leftButton", "<Mouse>/rightButton", "<Pointer>/position", "<Pointer>/delta", "<Pointer>/Press" };

	// Token: 0x04000B43 RID: 2883
	public static Action<InputAction> OnRebindBegin;

	// Token: 0x04000B44 RID: 2884
	public static Action<InputAction> OnRebindComplete;

	// Token: 0x04000B45 RID: 2885
	public static Action OnBindingChanged;

	// Token: 0x04000B46 RID: 2886
	private static InputActionRebindingExtensions.RebindingOperation operation = new InputActionRebindingExtensions.RebindingOperation();

	// Token: 0x04000B47 RID: 2887
	private const string SaveKey = "InputBinding";
}
