using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Duckov.MiniGames
{
	// Token: 0x02000281 RID: 641
	public class MiniGameInputHandler : MonoBehaviour
	{
		// Token: 0x06001476 RID: 5238 RVA: 0x0004BC44 File Offset: 0x00049E44
		private void Awake()
		{
			InputActionAsset actions = GameManager.MainPlayerInput.actions;
			this.inputActionMove = actions["MoveAxis"];
			this.inputActionButtonA = actions["MiniGameA"];
			this.inputActionButtonB = actions["MiniGameB"];
			this.inputActionSelect = actions["MiniGameSelect"];
			this.inputActionStart = actions["MiniGameStart"];
			this.inputActionMouseDelta = actions["MouseDelta"];
			this.inputActionButtonA.actionMap.Enable();
			this.Bind(this.inputActionMove, new Action<InputAction.CallbackContext>(this.OnMove));
			this.Bind(this.inputActionButtonA, new Action<InputAction.CallbackContext>(this.OnButtonA));
			this.Bind(this.inputActionButtonB, new Action<InputAction.CallbackContext>(this.OnButtonB));
			this.Bind(this.inputActionSelect, new Action<InputAction.CallbackContext>(this.OnSelect));
			this.Bind(this.inputActionStart, new Action<InputAction.CallbackContext>(this.OnStart));
			this.Bind(this.inputActionMouseDelta, new Action<InputAction.CallbackContext>(this.OnMouseDelta));
		}

		// Token: 0x06001477 RID: 5239 RVA: 0x0004BD62 File Offset: 0x00049F62
		private void OnMouseDelta(InputAction.CallbackContext context)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (this.game == null)
			{
				return;
			}
			this.game.SetInputAxis(context.ReadValue<Vector2>(), 1);
		}

		// Token: 0x06001478 RID: 5240 RVA: 0x0004BD8F File Offset: 0x00049F8F
		public void ClearInput()
		{
			MiniGame miniGame = this.game;
			if (miniGame == null)
			{
				return;
			}
			miniGame.ClearInput();
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x0004BDA1 File Offset: 0x00049FA1
		private void OnDisable()
		{
			this.ClearInput();
		}

		// Token: 0x0600147A RID: 5242 RVA: 0x0004BDA9 File Offset: 0x00049FA9
		private void SetGameButtonByContext(MiniGame.Button button, InputAction.CallbackContext context)
		{
			if (context.started)
			{
				this.game.SetButton(button, true);
				return;
			}
			if (context.canceled)
			{
				this.game.SetButton(button, false);
			}
		}

		// Token: 0x0600147B RID: 5243 RVA: 0x0004BDD8 File Offset: 0x00049FD8
		private void OnStart(InputAction.CallbackContext context)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (this.game == null)
			{
				return;
			}
			this.SetGameButtonByContext(MiniGame.Button.Start, context);
		}

		// Token: 0x0600147C RID: 5244 RVA: 0x0004BDFA File Offset: 0x00049FFA
		private void OnSelect(InputAction.CallbackContext context)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (this.game == null)
			{
				return;
			}
			this.SetGameButtonByContext(MiniGame.Button.Select, context);
		}

		// Token: 0x0600147D RID: 5245 RVA: 0x0004BE1C File Offset: 0x0004A01C
		private void OnButtonB(InputAction.CallbackContext context)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (this.game == null)
			{
				return;
			}
			this.SetGameButtonByContext(MiniGame.Button.B, context);
		}

		// Token: 0x0600147E RID: 5246 RVA: 0x0004BE3E File Offset: 0x0004A03E
		private void OnButtonA(InputAction.CallbackContext context)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (this.game == null)
			{
				return;
			}
			this.SetGameButtonByContext(MiniGame.Button.A, context);
		}

		// Token: 0x0600147F RID: 5247 RVA: 0x0004BE60 File Offset: 0x0004A060
		private void OnMove(InputAction.CallbackContext context)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (this.game == null)
			{
				return;
			}
			this.game.SetInputAxis(context.ReadValue<Vector2>(), 0);
		}

		// Token: 0x06001480 RID: 5248 RVA: 0x0004BE90 File Offset: 0x0004A090
		private void OnDestroy()
		{
			foreach (Action action in this.unbindCommands)
			{
				if (action != null)
				{
					action();
				}
			}
		}

		// Token: 0x06001481 RID: 5249 RVA: 0x0004BEE8 File Offset: 0x0004A0E8
		private void Bind(InputAction inputAction, Action<InputAction.CallbackContext> action)
		{
			inputAction.Enable();
			inputAction.started += action;
			inputAction.performed += action;
			inputAction.canceled += action;
			this.unbindCommands.Add(delegate
			{
				inputAction.started -= action;
				inputAction.performed -= action;
				inputAction.canceled -= action;
			});
		}

		// Token: 0x06001482 RID: 5250 RVA: 0x0004BF5E File Offset: 0x0004A15E
		internal void SetGame(MiniGame game)
		{
			this.game = game;
		}

		// Token: 0x04000EFF RID: 3839
		[SerializeField]
		private MiniGame game;

		// Token: 0x04000F00 RID: 3840
		private InputAction inputActionMove;

		// Token: 0x04000F01 RID: 3841
		private InputAction inputActionButtonA;

		// Token: 0x04000F02 RID: 3842
		private InputAction inputActionButtonB;

		// Token: 0x04000F03 RID: 3843
		private InputAction inputActionSelect;

		// Token: 0x04000F04 RID: 3844
		private InputAction inputActionStart;

		// Token: 0x04000F05 RID: 3845
		private InputAction inputActionMouseDelta;

		// Token: 0x04000F06 RID: 3846
		private List<Action> unbindCommands = new List<Action>();
	}
}
