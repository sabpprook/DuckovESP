using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Duckov.MiniGames.Utilities
{
	// Token: 0x02000282 RID: 642
	public class ControllerAnimator : MonoBehaviour
	{
		// Token: 0x06001484 RID: 5252 RVA: 0x0004BF7A File Offset: 0x0004A17A
		private void OnEnable()
		{
			MiniGame.OnInput += this.OnMiniGameInput;
		}

		// Token: 0x06001485 RID: 5253 RVA: 0x0004BF8D File Offset: 0x0004A18D
		private void OnDisable()
		{
			MiniGame.OnInput -= this.OnMiniGameInput;
		}

		// Token: 0x06001486 RID: 5254 RVA: 0x0004BFA0 File Offset: 0x0004A1A0
		private void OnMiniGameInput(MiniGame game, MiniGame.MiniGameInputEventContext context)
		{
			if (this.master == null)
			{
				return;
			}
			if (this.master.Game != game)
			{
				return;
			}
			this.HandleInput(context);
		}

		// Token: 0x06001487 RID: 5255 RVA: 0x0004BFCC File Offset: 0x0004A1CC
		private void HandleInput(MiniGame.MiniGameInputEventContext context)
		{
			if (context.isButtonEvent)
			{
				this.HandleButtonEvent(context);
				return;
			}
			if (context.isAxisEvent)
			{
				this.HandleAxisEvent(context);
			}
		}

		// Token: 0x06001488 RID: 5256 RVA: 0x0004BFED File Offset: 0x0004A1ED
		private void HandleAxisEvent(MiniGame.MiniGameInputEventContext context)
		{
			if (context.axisIndex != 0)
			{
				return;
			}
			this.SetAxis(context.axisValue);
		}

		// Token: 0x06001489 RID: 5257 RVA: 0x0004C004 File Offset: 0x0004A204
		private void HandleButtonEvent(MiniGame.MiniGameInputEventContext context)
		{
			switch (context.button)
			{
			case MiniGame.Button.A:
				this.HandleBtnPushRest(this.btn_A, context.pressing);
				break;
			case MiniGame.Button.B:
				this.HandleBtnPushRest(this.btn_B, context.pressing);
				break;
			case MiniGame.Button.Start:
				this.HandleBtnPushRest(this.btn_Start, context.pressing);
				break;
			case MiniGame.Button.Select:
				this.HandleBtnPushRest(this.btn_Select, context.pressing);
				break;
			case MiniGame.Button.Left:
			case MiniGame.Button.Right:
			case MiniGame.Button.Up:
			case MiniGame.Button.Down:
				this.PlayAxisPressReleaseFX(context.button, context.pressing);
				break;
			}
			if (context.pressing)
			{
				switch (context.button)
				{
				case MiniGame.Button.None:
					break;
				case MiniGame.Button.A:
					this.ApplyTorque(1f, -0.5f);
					return;
				case MiniGame.Button.B:
					this.ApplyTorque(1f, -0f);
					return;
				case MiniGame.Button.Start:
					this.ApplyTorque(0.5f, -0.5f);
					return;
				case MiniGame.Button.Select:
					this.ApplyTorque(-0.5f, -0.5f);
					return;
				case MiniGame.Button.Left:
					this.ApplyTorque(-1f, 0f);
					return;
				case MiniGame.Button.Right:
					this.ApplyTorque(-0.5f, 0f);
					return;
				case MiniGame.Button.Up:
					this.ApplyTorque(-1f, 0.5f);
					return;
				case MiniGame.Button.Down:
					this.ApplyTorque(-1f, -0.5f);
					return;
				default:
					return;
				}
			}
			else
			{
				this.ApplyTorque(global::UnityEngine.Random.insideUnitCircle * 0.25f);
			}
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x0004C180 File Offset: 0x0004A380
		private void PlayAxisPressReleaseFX(MiniGame.Button button, bool pressing)
		{
			Transform transform = null;
			switch (button)
			{
			case MiniGame.Button.Left:
				transform = this.fxPos_Left;
				break;
			case MiniGame.Button.Right:
				transform = this.fxPos_Right;
				break;
			case MiniGame.Button.Up:
				transform = this.fxPos_Up;
				break;
			case MiniGame.Button.Down:
				transform = this.fxPos_Down;
				break;
			}
			if (transform == null)
			{
				return;
			}
			if (pressing)
			{
				FXPool.Play(this.buttonPressFX, transform.position, transform.rotation);
				return;
			}
			FXPool.Play(this.buttonRestFX, transform.position, transform.rotation);
		}

		// Token: 0x0600148B RID: 5259 RVA: 0x0004C20C File Offset: 0x0004A40C
		private void ApplyTorque(float x, float y)
		{
			if (this.mainTransform == null)
			{
				return;
			}
			this.mainTransform.DOKill(false);
			Vector3 vector = new Vector3(-y, -x, 0f) * this.torqueStrength;
			this.mainTransform.localRotation = Quaternion.identity;
			this.mainTransform.DOPunchRotation(vector, this.torqueDuration, this.torqueVibrato, this.torqueElasticity);
		}

		// Token: 0x0600148C RID: 5260 RVA: 0x0004C27E File Offset: 0x0004A47E
		private void ApplyTorque(Vector2 torque)
		{
			this.ApplyTorque(torque.x, torque.y);
		}

		// Token: 0x0600148D RID: 5261 RVA: 0x0004C292 File Offset: 0x0004A492
		private void HandleBtnPushRest(Transform btnTrans, bool pressed)
		{
			if (pressed)
			{
				this.Push(btnTrans);
				return;
			}
			this.Rest(btnTrans);
		}

		// Token: 0x0600148E RID: 5262 RVA: 0x0004C2A6 File Offset: 0x0004A4A6
		internal void SetConsole(GamingConsole master)
		{
			this.master = master;
			this.RefreshAll();
		}

		// Token: 0x0600148F RID: 5263 RVA: 0x0004C2B8 File Offset: 0x0004A4B8
		private void RefreshAll()
		{
			this.RestAll();
			if (this.master == null)
			{
				return;
			}
			MiniGame game = this.master.Game;
			if (game == null)
			{
				return;
			}
			if (game.GetButton(MiniGame.Button.A))
			{
				this.Push(this.btn_A);
			}
			if (game.GetButton(MiniGame.Button.B))
			{
				this.Push(this.btn_B);
			}
			if (game.GetButton(MiniGame.Button.Select))
			{
				this.Push(this.btn_Select);
			}
			if (game.GetButton(MiniGame.Button.Start))
			{
				this.Push(this.btn_Start);
			}
			this.SetAxis(game.GetAxis(0));
		}

		// Token: 0x06001490 RID: 5264 RVA: 0x0004C354 File Offset: 0x0004A554
		private void RestAll()
		{
			this.Rest(this.btn_A);
			this.Rest(this.btn_B);
			this.Rest(this.btn_Start);
			this.Rest(this.btn_Select);
			this.Rest(this.btn_Axis);
			this.SetAxis(Vector2.zero);
		}

		// Token: 0x06001491 RID: 5265 RVA: 0x0004C3A8 File Offset: 0x0004A5A8
		private void SetAxis(Vector2 axis)
		{
			if (this.btn_Axis == null)
			{
				return;
			}
			axis = axis.normalized;
			Vector3 vector = new Vector3(0f, -axis.x * this.axisAmp, axis.y * this.axisAmp);
			Quaternion localRotation = this.btn_Axis.localRotation;
			Quaternion quaternion = Quaternion.Euler(vector);
			quaternion * Quaternion.Inverse(localRotation);
			this.btn_Axis.localRotation = quaternion;
		}

		// Token: 0x06001492 RID: 5266 RVA: 0x0004C420 File Offset: 0x0004A620
		private void Push(Transform btnTransform)
		{
			if (btnTransform == null)
			{
				return;
			}
			btnTransform.DOKill(false);
			btnTransform.DOLocalMoveX(-this.btnDepth, this.transitionDuration, false).SetEase(Ease.OutElastic);
			if (this.buttonPressFX)
			{
				FXPool.Play(this.buttonPressFX, btnTransform.position, btnTransform.rotation);
			}
		}

		// Token: 0x06001493 RID: 5267 RVA: 0x0004C480 File Offset: 0x0004A680
		private void Rest(Transform btnTransform)
		{
			if (btnTransform == null)
			{
				return;
			}
			btnTransform.DOKill(false);
			btnTransform.DOLocalMoveX(0f, this.transitionDuration, false).SetEase(Ease.OutElastic);
			if (this.buttonRestFX)
			{
				FXPool.Play(this.buttonRestFX, btnTransform.position, btnTransform.rotation);
			}
		}

		// Token: 0x04000F07 RID: 3847
		private GamingConsole master;

		// Token: 0x04000F08 RID: 3848
		public Transform mainTransform;

		// Token: 0x04000F09 RID: 3849
		public Transform btn_A;

		// Token: 0x04000F0A RID: 3850
		public Transform btn_B;

		// Token: 0x04000F0B RID: 3851
		public Transform btn_Start;

		// Token: 0x04000F0C RID: 3852
		public Transform btn_Select;

		// Token: 0x04000F0D RID: 3853
		public Transform btn_Axis;

		// Token: 0x04000F0E RID: 3854
		public Transform fxPos_Up;

		// Token: 0x04000F0F RID: 3855
		public Transform fxPos_Right;

		// Token: 0x04000F10 RID: 3856
		public Transform fxPos_Down;

		// Token: 0x04000F11 RID: 3857
		public Transform fxPos_Left;

		// Token: 0x04000F12 RID: 3858
		[SerializeField]
		private float transitionDuration = 0.2f;

		// Token: 0x04000F13 RID: 3859
		[SerializeField]
		private float axisAmp = 10f;

		// Token: 0x04000F14 RID: 3860
		[SerializeField]
		private float btnDepth = 0.003f;

		// Token: 0x04000F15 RID: 3861
		[SerializeField]
		private float torqueStrength = 5f;

		// Token: 0x04000F16 RID: 3862
		[SerializeField]
		private float torqueDuration = 0.5f;

		// Token: 0x04000F17 RID: 3863
		[SerializeField]
		private int torqueVibrato = 1;

		// Token: 0x04000F18 RID: 3864
		[SerializeField]
		private float torqueElasticity = 1f;

		// Token: 0x04000F19 RID: 3865
		[SerializeField]
		private ParticleSystem buttonPressFX;

		// Token: 0x04000F1A RID: 3866
		[SerializeField]
		private ParticleSystem buttonRestFX;
	}
}
