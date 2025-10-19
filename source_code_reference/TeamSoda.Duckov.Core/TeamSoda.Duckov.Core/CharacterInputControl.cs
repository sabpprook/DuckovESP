using System;
using System.Collections.Generic;
using System.Reflection;
using Dialogues;
using Duckov;
using Duckov.MiniMaps.UI;
using Duckov.Quests.UI;
using Duckov.UI;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x02000077 RID: 119
public class CharacterInputControl : MonoBehaviour
{
	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x0600045B RID: 1115 RVA: 0x00014199 File Offset: 0x00012399
	// (set) Token: 0x0600045C RID: 1116 RVA: 0x000141A0 File Offset: 0x000123A0
	public static CharacterInputControl Instance { get; private set; }

	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x0600045D RID: 1117 RVA: 0x000141A8 File Offset: 0x000123A8
	private PlayerInput PlayerInput
	{
		get
		{
			return GameManager.MainPlayerInput;
		}
	}

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x0600045E RID: 1118 RVA: 0x000141AF File Offset: 0x000123AF
	private bool usingMouseAndKeyboard
	{
		get
		{
			return InputManager.InputDevice == InputManager.InputDevices.mouseKeyboard;
		}
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x000141B9 File Offset: 0x000123B9
	private void Awake()
	{
		CharacterInputControl.Instance = this;
		this.inputActions = new CharacterInputControl.InputActionReferences(this.PlayerInput);
		this.RegisterEvents();
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x000141D8 File Offset: 0x000123D8
	private void OnDestroy()
	{
		this.UnregisterEvent();
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x000141E0 File Offset: 0x000123E0
	private void RegisterEvents()
	{
		this.Bind(this.inputActions.MoveAxis, new Action<InputAction.CallbackContext>(this.OnPlayerMoveInput));
		this.Bind(this.inputActions.Run, new Action<InputAction.CallbackContext>(this.OnPlayerRunInput));
		this.Bind(this.inputActions.MousePos, new Action<InputAction.CallbackContext>(this.OnPlayerMouseMove));
		this.Bind(this.inputActions.Skill_1_StartAim, new Action<InputAction.CallbackContext>(this.OnStartCharacterSkillAim));
		this.Bind(this.inputActions.Reload, new Action<InputAction.CallbackContext>(this.OnReloadInput));
		this.Bind(this.inputActions.Interact, new Action<InputAction.CallbackContext>(this.OnInteractInput));
		this.Bind(this.inputActions.ScrollWheel, new Action<InputAction.CallbackContext>(this.OnMouseScollerInput));
		this.Bind(this.inputActions.SwitchWeapon, new Action<InputAction.CallbackContext>(this.OnSwitchWeaponInput));
		this.Bind(this.inputActions.SwitchInteractAndBulletType, new Action<InputAction.CallbackContext>(this.OnSwitchInteractAndBulletTypeInput));
		this.Bind(this.inputActions.Trigger, new Action<InputAction.CallbackContext>(this.OnPlayerTriggerInputUsingMouseKeyboard));
		this.Bind(this.inputActions.ToggleView, new Action<InputAction.CallbackContext>(this.OnToggleViewInput));
		this.Bind(this.inputActions.ToggleNightVision, new Action<InputAction.CallbackContext>(this.OnToggleNightVisionInput));
		this.Bind(this.inputActions.CancelSkill, new Action<InputAction.CallbackContext>(this.OnCancelSkillInput));
		this.Bind(this.inputActions.Dash, new Action<InputAction.CallbackContext>(this.OnDashInput));
		this.Bind(this.inputActions.ItemShortcut1, new Action<InputAction.CallbackContext>(this.OnPlayerSwitchItemAgent1));
		this.Bind(this.inputActions.ItemShortcut2, new Action<InputAction.CallbackContext>(this.OnPlayerSwitchItemAgent2));
		this.Bind(this.inputActions.ItemShortcut3, new Action<InputAction.CallbackContext>(this.OnShortCutInput3));
		this.Bind(this.inputActions.ItemShortcut4, new Action<InputAction.CallbackContext>(this.OnShortCutInput4));
		this.Bind(this.inputActions.ItemShortcut5, new Action<InputAction.CallbackContext>(this.OnShortCutInput5));
		this.Bind(this.inputActions.ItemShortcut6, new Action<InputAction.CallbackContext>(this.OnShortCutInput6));
		this.Bind(this.inputActions.ItemShortcut7, new Action<InputAction.CallbackContext>(this.OnShortCutInput7));
		this.Bind(this.inputActions.ItemShortcut8, new Action<InputAction.CallbackContext>(this.OnShortCutInput8));
		this.Bind(this.inputActions.ADS, new Action<InputAction.CallbackContext>(this.OnPlayerAdsInput));
		this.Bind(this.inputActions.UI_Inventory, new Action<InputAction.CallbackContext>(this.OnUIInventoryInput));
		this.Bind(this.inputActions.UI_Map, new Action<InputAction.CallbackContext>(this.OnUIMapInput));
		this.Bind(this.inputActions.UI_Quest, new Action<InputAction.CallbackContext>(this.OnUIQuestViewInput));
		this.Bind(this.inputActions.StopAction, new Action<InputAction.CallbackContext>(this.OnPlayerStopAction));
		this.Bind(this.inputActions.PutAway, new Action<InputAction.CallbackContext>(this.OnPutAwayInput));
		this.Bind(this.inputActions.ItemShortcut_Melee, new Action<InputAction.CallbackContext>(this.OnPlayerSwitchItemAgentMelee));
		this.Bind(this.inputActions.MouseDelta, new Action<InputAction.CallbackContext>(this.OnPlayerMouseDelta));
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x00014553 File Offset: 0x00012753
	private void UnregisterEvent()
	{
		while (this.unbindCommands.Count > 0)
		{
			this.unbindCommands.Dequeue()();
		}
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x00014578 File Offset: 0x00012778
	private void Bind(InputAction action, Action<InputAction.CallbackContext> method)
	{
		action.performed += method;
		action.started += method;
		action.canceled += method;
		this.unbindCommands.Enqueue(delegate
		{
			this.Unbind(action, method);
		});
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x000145EA File Offset: 0x000127EA
	private void Unbind(InputAction action, Action<InputAction.CallbackContext> method)
	{
		action.performed -= method;
		action.started -= method;
		action.canceled -= method;
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x00014604 File Offset: 0x00012804
	private void Update()
	{
		if (!this.character)
		{
			this.character = CharacterMainControl.Main;
			if (!this.character)
			{
				return;
			}
		}
		if (this.usingMouseAndKeyboard)
		{
			this.inputManager.SetMousePosition(this.mousePos);
			this.inputManager.SetAimInputUsingMouse(this.mouseDelta);
			this.inputManager.SetTrigger(this.mouseKeyboardTriggerInput, this.mouseKeyboardTriggerInputThisFrame, this.mouseKeyboardTriggerReleaseThisFrame);
			if (this.character.skillAction.holdItemSkillKeeper.CheckSkillAndBinding())
			{
				this.inputManager.SetAimType(AimTypes.handheldSkill);
				if (this.mouseKeyboardTriggerInputThisFrame)
				{
					this.inputManager.StartItemSkillAim();
				}
				else if (this.mouseKeyboardTriggerReleaseThisFrame)
				{
					Debug.Log("Release");
					this.inputManager.ReleaseItemSkill();
				}
			}
			else
			{
				this.inputManager.SetAimType(AimTypes.normalAim);
			}
			this.UpdateScollerInput();
		}
		this.mouseKeyboardTriggerInputThisFrame = false;
		this.mouseKeyboardTriggerReleaseThisFrame = false;
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x000146F8 File Offset: 0x000128F8
	public void OnPlayerMoveInput(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Vector2 vector = context.ReadValue<Vector2>();
			this.inputManager.SetMoveInput(vector);
		}
		if (context.canceled)
		{
			this.inputManager.SetMoveInput(Vector2.zero);
		}
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x0001473C File Offset: 0x0001293C
	public void OnPlayerRunInput(InputAction.CallbackContext context)
	{
		this.runInput = false;
		if (context.started)
		{
			this.inputManager.SetRunInput(true);
			this.runInput = true;
		}
		if (context.canceled)
		{
			this.inputManager.SetRunInput(false);
			this.runInput = false;
		}
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x00014788 File Offset: 0x00012988
	public void OnPlayerAdsInput(InputAction.CallbackContext context)
	{
		this.adsInput = false;
		if (context.started)
		{
			this.inputManager.SetAdsInput(true);
			this.adsInput = true;
		}
		if (context.canceled)
		{
			this.inputManager.SetAdsInput(false);
			this.adsInput = false;
		}
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x000147D4 File Offset: 0x000129D4
	public void OnToggleViewInput(InputAction.CallbackContext context)
	{
		if (GameManager.Paused)
		{
			return;
		}
		if (context.started)
		{
			this.inputManager.ToggleView();
		}
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x000147F2 File Offset: 0x000129F2
	public void OnToggleNightVisionInput(InputAction.CallbackContext context)
	{
		if (GameManager.Paused)
		{
			return;
		}
		if (context.started)
		{
			this.inputManager.ToggleNightVision();
		}
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x00014810 File Offset: 0x00012A10
	public void OnPlayerTriggerInputUsingMouseKeyboard(InputAction.CallbackContext context)
	{
		if (InputManager.InputDevice != InputManager.InputDevices.mouseKeyboard)
		{
			return;
		}
		if (context.started)
		{
			this.mouseKeyboardTriggerInputThisFrame = true;
			this.mouseKeyboardTriggerInput = true;
			this.mouseKeyboardTriggerReleaseThisFrame = false;
			return;
		}
		if (context.canceled)
		{
			this.mouseKeyboardTriggerInputThisFrame = false;
			this.mouseKeyboardTriggerInput = false;
			this.mouseKeyboardTriggerReleaseThisFrame = true;
		}
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x00014862 File Offset: 0x00012A62
	public void OnPlayerMouseMove(InputAction.CallbackContext context)
	{
		this.mousePos = context.ReadValue<Vector2>();
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x00014871 File Offset: 0x00012A71
	public void OnPlayerMouseDelta(InputAction.CallbackContext context)
	{
		this.mouseDelta = context.ReadValue<Vector2>();
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x00014880 File Offset: 0x00012A80
	public void OnPlayerStopAction(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			this.inputManager.StopAction();
		}
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x00014896 File Offset: 0x00012A96
	public void OnPlayerSwitchItemAgent1(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			this.inputManager.SwitchItemAgent(1);
		}
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x000148AD File Offset: 0x00012AAD
	public void OnPlayerSwitchItemAgent2(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			this.inputManager.SwitchItemAgent(2);
		}
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x000148C4 File Offset: 0x00012AC4
	public void OnPlayerSwitchItemAgentMelee(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			this.inputManager.SwitchItemAgent(3);
		}
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x000148DB File Offset: 0x00012ADB
	public void OnStartCharacterSkillAim(InputAction.CallbackContext context)
	{
		this.inputManager.StartCharacterSkillAim();
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x000148E8 File Offset: 0x00012AE8
	public void OnCharacterSkillRelease()
	{
		this.inputManager.ReleaseCharacterSkill();
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x000148F5 File Offset: 0x00012AF5
	public void OnReloadInput(InputAction.CallbackContext context)
	{
		if (!context.performed)
		{
			return;
		}
		CharacterMainControl main = CharacterMainControl.Main;
		if (main == null)
		{
			return;
		}
		main.TryToReload(null);
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x00014914 File Offset: 0x00012B14
	public void OnUIInventoryInput(InputAction.CallbackContext context)
	{
		if (!context.performed)
		{
			return;
		}
		if (GameManager.Paused)
		{
			return;
		}
		if (DialogueUI.Active)
		{
			return;
		}
		if (SceneLoader.IsSceneLoading)
		{
			return;
		}
		if (!(View.ActiveView == null))
		{
			View.ActiveView.TryQuit();
			return;
		}
		if (LevelManager.Instance.IsBaseLevel)
		{
			PlayerStorage.Instance.InteractableLootBox.InteractWithMainCharacter();
			return;
		}
		InventoryView.Show();
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x0001497C File Offset: 0x00012B7C
	public void OnUIQuestViewInput(InputAction.CallbackContext context)
	{
		if (!context.performed)
		{
			return;
		}
		if (GameManager.Paused)
		{
			return;
		}
		if (DialogueUI.Active)
		{
			return;
		}
		if (View.ActiveView == null)
		{
			QuestView.Show();
			return;
		}
		if (View.ActiveView is QuestView)
		{
			View.ActiveView.TryQuit();
		}
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x000149CC File Offset: 0x00012BCC
	public void OnDashInput(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			this.inputManager.Dash();
		}
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x000149E4 File Offset: 0x00012BE4
	public void OnUIMapInput(InputAction.CallbackContext context)
	{
		if (!context.performed)
		{
			return;
		}
		if (GameManager.Paused)
		{
			return;
		}
		if (SceneLoader.IsSceneLoading)
		{
			return;
		}
		if (View.ActiveView == null)
		{
			MiniMapView.Show();
			return;
		}
		MiniMapView miniMapView = View.ActiveView as MiniMapView;
		if (miniMapView != null)
		{
			miniMapView.Close();
		}
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x00014A32 File Offset: 0x00012C32
	public void OnCancelSkillInput(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			this.inputManager.CancleSkill();
		}
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x00014A49 File Offset: 0x00012C49
	public void OnInteractInput(InputAction.CallbackContext context)
	{
		if (!context.performed)
		{
			return;
		}
		this.inputManager.Interact();
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x00014A60 File Offset: 0x00012C60
	public void OnPutAwayInput(InputAction.CallbackContext context)
	{
		if (!context.performed)
		{
			return;
		}
		this.inputManager.PutAway();
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x00014A77 File Offset: 0x00012C77
	public void OnMouseScollerInput(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			this.scollY = context.ReadValue<Vector2>().y;
		}
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x00014A94 File Offset: 0x00012C94
	private void UpdateScollerInput()
	{
		if (Mathf.Abs(this.scollY) > 0.5f && (float)this.scollYZeroFrames > 3f)
		{
			if (ScrollWheelBehaviour.CurrentBehaviour == ScrollWheelBehaviour.Behaviour.AmmoAndInteract)
			{
				this.inputManager.SetSwitchInteractInput((this.scollY > 0f) ? 1 : (-1));
				this.inputManager.SetSwitchBulletTypeInput((this.scollY > 0f) ? 1 : (-1));
			}
			else
			{
				this.inputManager.SetSwitchWeaponInput((this.scollY > 0f) ? 1 : (-1));
			}
		}
		if (Mathf.Abs(this.scollY) < 0.5f)
		{
			this.scollYZeroFrames++;
			return;
		}
		this.scollYZeroFrames = 0;
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x00014B48 File Offset: 0x00012D48
	public void OnSwitchWeaponInput(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			float num = context.ReadValue<float>();
			this.inputManager.SetSwitchWeaponInput((num > 0f) ? (-1) : 1);
		}
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x00014B80 File Offset: 0x00012D80
	public void OnSwitchInteractAndBulletTypeInput(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			float num = context.ReadValue<float>();
			this.inputManager.SetSwitchInteractInput((num > 0f) ? (-1) : 1);
			this.inputManager.SetSwitchBulletTypeInput((num > 0f) ? (-1) : 1);
		}
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x00014BCC File Offset: 0x00012DCC
	private void ShortCutInput(int index)
	{
		if (View.ActiveView != null)
		{
			UIInputManager.NotifyShortcutInput(index - 3);
			return;
		}
		Item item = ItemShortcut.Get(index - 3);
		if (item == null)
		{
			return;
		}
		if (!this.character)
		{
			return;
		}
		if (item && item.UsageUtilities && item.UsageUtilities.IsUsable(item, this.character))
		{
			this.character.UseItem(item);
			return;
		}
		if (item && item.GetBool("IsSkill", false))
		{
			this.character.ChangeHoldItem(item);
			return;
		}
		if (item && item.HasHandHeldAgent)
		{
			Debug.Log("has hand held");
			this.character.ChangeHoldItem(item);
		}
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x00014C91 File Offset: 0x00012E91
	public void OnShortCutInput3(InputAction.CallbackContext context)
	{
		if (!context.performed)
		{
			return;
		}
		this.ShortCutInput(3);
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x00014CA4 File Offset: 0x00012EA4
	public void OnShortCutInput4(InputAction.CallbackContext context)
	{
		if (!context.performed)
		{
			return;
		}
		this.ShortCutInput(4);
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x00014CB7 File Offset: 0x00012EB7
	public void OnShortCutInput5(InputAction.CallbackContext context)
	{
		if (!context.performed)
		{
			return;
		}
		this.ShortCutInput(5);
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x00014CCA File Offset: 0x00012ECA
	public void OnShortCutInput6(InputAction.CallbackContext context)
	{
		if (!context.performed)
		{
			return;
		}
		this.ShortCutInput(6);
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x00014CDD File Offset: 0x00012EDD
	public void OnShortCutInput7(InputAction.CallbackContext context)
	{
		if (!context.performed)
		{
			return;
		}
		this.ShortCutInput(7);
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x00014CF0 File Offset: 0x00012EF0
	public void OnShortCutInput8(InputAction.CallbackContext context)
	{
		if (!context.performed)
		{
			return;
		}
		this.ShortCutInput(8);
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x00014D04 File Offset: 0x00012F04
	internal static InputAction GetInputAction(string name)
	{
		if (CharacterInputControl.Instance == null)
		{
			return null;
		}
		InputAction inputAction;
		try
		{
			inputAction = CharacterInputControl.Instance.PlayerInput.actions[name];
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
			Debug.LogError("查找 Input Action " + name + " 时发生错误, 返回null");
			inputAction = null;
		}
		return inputAction;
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x00014D68 File Offset: 0x00012F68
	public static bool GetChangeBulletTypeWasPressed()
	{
		return CharacterInputControl.Instance.inputActions.SwitchBulletType.WasPressedThisFrame();
	}

	// Token: 0x040003C7 RID: 967
	public InputManager inputManager;

	// Token: 0x040003C8 RID: 968
	private bool runInput;

	// Token: 0x040003C9 RID: 969
	private bool adsInput;

	// Token: 0x040003CA RID: 970
	private bool aimDown;

	// Token: 0x040003CB RID: 971
	private Vector2 mousePos;

	// Token: 0x040003CC RID: 972
	private Vector2 mouseDelta;

	// Token: 0x040003CD RID: 973
	private bool mouseKeyboardTriggerInput;

	// Token: 0x040003CE RID: 974
	private bool mouseKeyboardTriggerReleaseThisFrame;

	// Token: 0x040003CF RID: 975
	private bool mouseKeyboardTriggerInputThisFrame;

	// Token: 0x040003D0 RID: 976
	private CharacterMainControl character;

	// Token: 0x040003D1 RID: 977
	private CharacterInputControl.InputActionReferences inputActions;

	// Token: 0x040003D2 RID: 978
	private Queue<Action> unbindCommands = new Queue<Action>();

	// Token: 0x040003D3 RID: 979
	private float scollY;

	// Token: 0x040003D4 RID: 980
	private int scollYZeroFrames;

	// Token: 0x02000437 RID: 1079
	private class InputActionReferences
	{
		// Token: 0x060025F7 RID: 9719 RVA: 0x000832DC File Offset: 0x000814DC
		public InputActionReferences(PlayerInput playerInput)
		{
			InputActionAsset actions = playerInput.actions;
			Type typeFromHandle = typeof(CharacterInputControl.InputActionReferences);
			Type typeFromHandle2 = typeof(InputAction);
			FieldInfo[] fields = typeFromHandle.GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.FieldType != typeFromHandle2)
				{
					Debug.LogError(fieldInfo.FieldType.Name);
				}
				else
				{
					InputAction inputAction = actions[fieldInfo.Name];
					if (inputAction == null)
					{
						Debug.LogError("找不到名为 " + fieldInfo.Name + " 的input action");
					}
					else
					{
						fieldInfo.SetValue(this, inputAction);
					}
				}
			}
			foreach (FieldInfo fieldInfo2 in fields)
			{
				if (!(fieldInfo2.FieldType != typeFromHandle2))
				{
					fieldInfo2.GetValue(this);
				}
			}
		}

		// Token: 0x04001A2F RID: 6703
		public InputAction MoveAxis;

		// Token: 0x04001A30 RID: 6704
		public InputAction Run;

		// Token: 0x04001A31 RID: 6705
		public InputAction Aim;

		// Token: 0x04001A32 RID: 6706
		public InputAction MousePos;

		// Token: 0x04001A33 RID: 6707
		public InputAction ItemShortcut1;

		// Token: 0x04001A34 RID: 6708
		public InputAction ItemShortcut2;

		// Token: 0x04001A35 RID: 6709
		public InputAction Skill_1_StartAim;

		// Token: 0x04001A36 RID: 6710
		public InputAction Reload;

		// Token: 0x04001A37 RID: 6711
		public InputAction UI_Inventory;

		// Token: 0x04001A38 RID: 6712
		public InputAction UI_Map;

		// Token: 0x04001A39 RID: 6713
		public InputAction Interact;

		// Token: 0x04001A3A RID: 6714
		public InputAction ScrollWheel;

		// Token: 0x04001A3B RID: 6715
		public InputAction SwitchWeapon;

		// Token: 0x04001A3C RID: 6716
		public InputAction SwitchInteractAndBulletType;

		// Token: 0x04001A3D RID: 6717
		public InputAction Trigger;

		// Token: 0x04001A3E RID: 6718
		public InputAction ToggleView;

		// Token: 0x04001A3F RID: 6719
		public InputAction ToggleNightVision;

		// Token: 0x04001A40 RID: 6720
		public InputAction CancelSkill;

		// Token: 0x04001A41 RID: 6721
		public InputAction Dash;

		// Token: 0x04001A42 RID: 6722
		public InputAction ItemShortcut3;

		// Token: 0x04001A43 RID: 6723
		public InputAction ItemShortcut4;

		// Token: 0x04001A44 RID: 6724
		public InputAction ItemShortcut5;

		// Token: 0x04001A45 RID: 6725
		public InputAction ItemShortcut6;

		// Token: 0x04001A46 RID: 6726
		public InputAction ItemShortcut7;

		// Token: 0x04001A47 RID: 6727
		public InputAction ItemShortcut8;

		// Token: 0x04001A48 RID: 6728
		public InputAction ADS;

		// Token: 0x04001A49 RID: 6729
		public InputAction UI_Quest;

		// Token: 0x04001A4A RID: 6730
		public InputAction StopAction;

		// Token: 0x04001A4B RID: 6731
		public InputAction PutAway;

		// Token: 0x04001A4C RID: 6732
		public InputAction ItemShortcut_Melee;

		// Token: 0x04001A4D RID: 6733
		public InputAction MouseDelta;

		// Token: 0x04001A4E RID: 6734
		public InputAction SwitchBulletType;
	}
}
