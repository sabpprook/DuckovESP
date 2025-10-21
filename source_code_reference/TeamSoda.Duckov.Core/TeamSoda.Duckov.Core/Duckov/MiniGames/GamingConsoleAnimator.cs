using System;
using UnityEngine;

namespace Duckov.MiniGames
{
	// Token: 0x0200027D RID: 637
	public class GamingConsoleAnimator : MonoBehaviour
	{
		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x06001449 RID: 5193 RVA: 0x0004B4DC File Offset: 0x000496DC
		[SerializeField]
		private MiniGame Game
		{
			get
			{
				if (this.console == null)
				{
					return null;
				}
				return this.console.Game;
			}
		}

		// Token: 0x0600144A RID: 5194 RVA: 0x0004B4F9 File Offset: 0x000496F9
		private void Update()
		{
			this.Tick();
		}

		// Token: 0x0600144B RID: 5195 RVA: 0x0004B504 File Offset: 0x00049704
		private void Tick()
		{
			if (this.Game == null)
			{
				this.Clear();
				return;
			}
			if (CameraMode.Active)
			{
				return;
			}
			this.joyStick_Target = this.Game.GetAxis(0);
			this.joyStick_Current = Vector2.Lerp(this.joyStick_Current, this.joyStick_Target, 0.25f);
			Vector2 vector = this.joyStick_Current;
			this.animator.SetFloat("AxisX", vector.x);
			this.animator.SetFloat("AxisY", vector.y);
			this.animator.SetBool("ButtonA", this.Game.GetButton(MiniGame.Button.A));
			this.animator.SetBool("ButtonB", this.Game.GetButton(MiniGame.Button.B));
		}

		// Token: 0x0600144C RID: 5196 RVA: 0x0004B5C8 File Offset: 0x000497C8
		private void Clear()
		{
			this.animator.SetBool("ButtonA", false);
			this.animator.SetBool("ButtonB", false);
			this.animator.SetFloat("AxisX", 0f);
			this.animator.SetFloat("AxisY", 0f);
		}

		// Token: 0x04000EED RID: 3821
		[SerializeField]
		private Animator animator;

		// Token: 0x04000EEE RID: 3822
		[SerializeField]
		private GamingConsole console;

		// Token: 0x04000EEF RID: 3823
		private Vector2 joyStick_Current;

		// Token: 0x04000EF0 RID: 3824
		private Vector2 joyStick_Target;
	}
}
