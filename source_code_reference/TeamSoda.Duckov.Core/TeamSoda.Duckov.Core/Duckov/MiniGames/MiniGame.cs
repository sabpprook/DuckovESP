using System;
using System.Collections.Generic;
using UnityEngine;

namespace Duckov.MiniGames
{
	// Token: 0x0200027F RID: 639
	public class MiniGame : MonoBehaviour
	{
		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x06001454 RID: 5204 RVA: 0x0004B69D File Offset: 0x0004989D
		public string ID
		{
			get
			{
				return this.id;
			}
		}

		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x06001455 RID: 5205 RVA: 0x0004B6A5 File Offset: 0x000498A5
		public Camera Camera
		{
			get
			{
				return this.camera;
			}
		}

		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x06001456 RID: 5206 RVA: 0x0004B6AD File Offset: 0x000498AD
		public Camera UICamera
		{
			get
			{
				return this.uiCamera;
			}
		}

		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x06001457 RID: 5207 RVA: 0x0004B6B5 File Offset: 0x000498B5
		public RenderTexture RenderTexture
		{
			get
			{
				return this.renderTexture;
			}
		}

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x06001458 RID: 5208 RVA: 0x0004B6BD File Offset: 0x000498BD
		public GamingConsole Console
		{
			get
			{
				return this.console;
			}
		}

		// Token: 0x14000085 RID: 133
		// (add) Token: 0x06001459 RID: 5209 RVA: 0x0004B6C8 File Offset: 0x000498C8
		// (remove) Token: 0x0600145A RID: 5210 RVA: 0x0004B6FC File Offset: 0x000498FC
		public static event Action<MiniGame, MiniGame.MiniGameInputEventContext> OnInput;

		// Token: 0x0600145B RID: 5211 RVA: 0x0004B72F File Offset: 0x0004992F
		public void SetRenderTexture(RenderTexture texture)
		{
			this.camera.targetTexture = texture;
			if (this.uiCamera)
			{
				this.uiCamera.targetTexture = texture;
			}
		}

		// Token: 0x0600145C RID: 5212 RVA: 0x0004B758 File Offset: 0x00049958
		public RenderTexture CreateAndSetRenderTexture(int width, int height)
		{
			RenderTexture renderTexture = new RenderTexture(width, height, 32);
			this.SetRenderTexture(renderTexture);
			return renderTexture;
		}

		// Token: 0x0600145D RID: 5213 RVA: 0x0004B777 File Offset: 0x00049977
		private void Awake()
		{
			if (this.renderTexture != null)
			{
				this.SetRenderTexture(this.renderTexture);
			}
		}

		// Token: 0x0600145E RID: 5214 RVA: 0x0004B794 File Offset: 0x00049994
		public void SetInputAxis(Vector2 axis, int index = 0)
		{
			Vector2 vector = this.inputAxis_0;
			if (index == 0)
			{
				this.inputAxis_0 = axis;
			}
			if (index == 1)
			{
				this.inputAxis_1 = axis;
			}
			if (index == 0)
			{
				bool flag = axis.x < -0.1f;
				bool flag2 = axis.x > 0.1f;
				bool flag3 = axis.y > 0.1f;
				bool flag4 = axis.y < -0.1f;
				bool flag5 = vector.x < -0.1f;
				bool flag6 = vector.x > 0.1f;
				bool flag7 = vector.y > 0.1f;
				bool flag8 = vector.y < -0.1f;
				if (flag != flag5)
				{
					this.SetButton(MiniGame.Button.Left, flag);
				}
				if (flag2 != flag6)
				{
					this.SetButton(MiniGame.Button.Right, flag2);
				}
				if (flag3 != flag7)
				{
					this.SetButton(MiniGame.Button.Up, flag3);
				}
				if (flag4 != flag8)
				{
					this.SetButton(MiniGame.Button.Down, flag4);
				}
			}
			Action<MiniGame, MiniGame.MiniGameInputEventContext> onInput = MiniGame.OnInput;
			if (onInput == null)
			{
				return;
			}
			onInput(this, new MiniGame.MiniGameInputEventContext
			{
				isAxisEvent = true,
				axisIndex = index,
				axisValue = axis
			});
		}

		// Token: 0x0600145F RID: 5215 RVA: 0x0004B8A0 File Offset: 0x00049AA0
		public void SetButton(MiniGame.Button button, bool down)
		{
			MiniGame.ButtonStatus buttonStatus;
			if (!this.buttons.TryGetValue(button, out buttonStatus))
			{
				buttonStatus = new MiniGame.ButtonStatus();
				this.buttons[button] = buttonStatus;
			}
			if (down)
			{
				buttonStatus.justPressed = true;
				buttonStatus.pressed = true;
			}
			else
			{
				buttonStatus.pressed = false;
				buttonStatus.justReleased = true;
			}
			this.buttons[button] = buttonStatus;
			Action<MiniGame, MiniGame.MiniGameInputEventContext> onInput = MiniGame.OnInput;
			if (onInput == null)
			{
				return;
			}
			onInput(this, new MiniGame.MiniGameInputEventContext
			{
				isButtonEvent = true,
				button = button,
				pressing = buttonStatus.pressed,
				buttonDown = buttonStatus.justPressed,
				buttonUp = buttonStatus.justReleased
			});
		}

		// Token: 0x06001460 RID: 5216 RVA: 0x0004B950 File Offset: 0x00049B50
		public bool GetButton(MiniGame.Button button)
		{
			MiniGame.ButtonStatus buttonStatus;
			return this.buttons.TryGetValue(button, out buttonStatus) && buttonStatus.pressed;
		}

		// Token: 0x06001461 RID: 5217 RVA: 0x0004B978 File Offset: 0x00049B78
		public bool GetButtonDown(MiniGame.Button button)
		{
			MiniGame.ButtonStatus buttonStatus;
			return this.buttons.TryGetValue(button, out buttonStatus) && buttonStatus.justPressed;
		}

		// Token: 0x06001462 RID: 5218 RVA: 0x0004B9A0 File Offset: 0x00049BA0
		public bool GetButtonUp(MiniGame.Button button)
		{
			MiniGame.ButtonStatus buttonStatus;
			return this.buttons.TryGetValue(button, out buttonStatus) && buttonStatus.justReleased;
		}

		// Token: 0x06001463 RID: 5219 RVA: 0x0004B9C8 File Offset: 0x00049BC8
		public Vector2 GetAxis(int index = 0)
		{
			if (index == 0)
			{
				return this.inputAxis_0;
			}
			if (index == 1)
			{
				return this.inputAxis_1;
			}
			return default(Vector2);
		}

		// Token: 0x06001464 RID: 5220 RVA: 0x0004B9F3 File Offset: 0x00049BF3
		private void Tick(float deltaTime)
		{
			this.UpdateLogic(deltaTime);
			this.Cleanup();
		}

		// Token: 0x06001465 RID: 5221 RVA: 0x0004BA02 File Offset: 0x00049C02
		private void UpdateLogic(float deltaTime)
		{
			Action<MiniGame, float> action = MiniGame.onUpdateLogic;
			if (action == null)
			{
				return;
			}
			action(this, deltaTime);
		}

		// Token: 0x06001466 RID: 5222 RVA: 0x0004BA18 File Offset: 0x00049C18
		private void Cleanup()
		{
			foreach (MiniGame.ButtonStatus buttonStatus in this.buttons.Values)
			{
				buttonStatus.justPressed = false;
				buttonStatus.justReleased = false;
			}
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x0004BA78 File Offset: 0x00049C78
		private void Update()
		{
			if (this.tickTiming == MiniGame.TickTiming.Update)
			{
				this.Tick(Time.deltaTime);
			}
		}

		// Token: 0x06001468 RID: 5224 RVA: 0x0004BA8E File Offset: 0x00049C8E
		private void FixedUpdate()
		{
			if (this.tickTiming == MiniGame.TickTiming.FixedUpdate)
			{
				this.Tick(Time.fixedDeltaTime);
			}
		}

		// Token: 0x06001469 RID: 5225 RVA: 0x0004BAA4 File Offset: 0x00049CA4
		private void LateUpdate()
		{
			if (this.tickTiming == MiniGame.TickTiming.FixedUpdate)
			{
				this.Tick(Time.deltaTime);
			}
		}

		// Token: 0x0600146A RID: 5226 RVA: 0x0004BABC File Offset: 0x00049CBC
		public void ClearInput()
		{
			foreach (MiniGame.ButtonStatus buttonStatus in this.buttons.Values)
			{
				if (buttonStatus.pressed)
				{
					buttonStatus.justReleased = true;
				}
				buttonStatus.pressed = false;
			}
			this.SetInputAxis(default(Vector2), 0);
			this.SetInputAxis(default(Vector2), 1);
		}

		// Token: 0x0600146B RID: 5227 RVA: 0x0004BB44 File Offset: 0x00049D44
		internal void SetConsole(GamingConsole console)
		{
			this.console = console;
		}

		// Token: 0x04000EF3 RID: 3827
		[SerializeField]
		private string id;

		// Token: 0x04000EF4 RID: 3828
		public MiniGame.TickTiming tickTiming;

		// Token: 0x04000EF5 RID: 3829
		[SerializeField]
		private Camera camera;

		// Token: 0x04000EF6 RID: 3830
		[SerializeField]
		private Camera uiCamera;

		// Token: 0x04000EF7 RID: 3831
		[SerializeField]
		private RenderTexture renderTexture;

		// Token: 0x04000EF8 RID: 3832
		public static Action<MiniGame, float> onUpdateLogic;

		// Token: 0x04000EF9 RID: 3833
		private GamingConsole console;

		// Token: 0x04000EFA RID: 3834
		private Vector2 inputAxis_0;

		// Token: 0x04000EFB RID: 3835
		private Vector2 inputAxis_1;

		// Token: 0x04000EFC RID: 3836
		private Dictionary<MiniGame.Button, MiniGame.ButtonStatus> buttons = new Dictionary<MiniGame.Button, MiniGame.ButtonStatus>();

		// Token: 0x02000550 RID: 1360
		public enum TickTiming
		{
			// Token: 0x04001ECB RID: 7883
			Manual,
			// Token: 0x04001ECC RID: 7884
			Update,
			// Token: 0x04001ECD RID: 7885
			FixedUpdate,
			// Token: 0x04001ECE RID: 7886
			LateUpdate
		}

		// Token: 0x02000551 RID: 1361
		public enum Button
		{
			// Token: 0x04001ED0 RID: 7888
			None,
			// Token: 0x04001ED1 RID: 7889
			A,
			// Token: 0x04001ED2 RID: 7890
			B,
			// Token: 0x04001ED3 RID: 7891
			Start,
			// Token: 0x04001ED4 RID: 7892
			Select,
			// Token: 0x04001ED5 RID: 7893
			Left,
			// Token: 0x04001ED6 RID: 7894
			Right,
			// Token: 0x04001ED7 RID: 7895
			Up,
			// Token: 0x04001ED8 RID: 7896
			Down
		}

		// Token: 0x02000552 RID: 1362
		public class ButtonStatus
		{
			// Token: 0x04001ED9 RID: 7897
			public bool pressed;

			// Token: 0x04001EDA RID: 7898
			public bool justPressed;

			// Token: 0x04001EDB RID: 7899
			public bool justReleased;
		}

		// Token: 0x02000553 RID: 1363
		public struct MiniGameInputEventContext
		{
			// Token: 0x04001EDC RID: 7900
			public bool isButtonEvent;

			// Token: 0x04001EDD RID: 7901
			public MiniGame.Button button;

			// Token: 0x04001EDE RID: 7902
			public bool pressing;

			// Token: 0x04001EDF RID: 7903
			public bool buttonDown;

			// Token: 0x04001EE0 RID: 7904
			public bool buttonUp;

			// Token: 0x04001EE1 RID: 7905
			public bool isAxisEvent;

			// Token: 0x04001EE2 RID: 7906
			public int axisIndex;

			// Token: 0x04001EE3 RID: 7907
			public Vector2 axisValue;
		}
	}
}
