using System;
using ECM2;
using UnityEngine;

namespace Duckov.MiniGames.Examples.FPS
{
	// Token: 0x020002D5 RID: 725
	public class FPSMovement : Character
	{
		// Token: 0x060016C7 RID: 5831 RVA: 0x00053544 File Offset: 0x00051744
		protected override void Awake()
		{
			base.Awake();
		}

		// Token: 0x060016C8 RID: 5832 RVA: 0x0005354C File Offset: 0x0005174C
		protected override void Start()
		{
			base.Start();
			if (this.game == null)
			{
				this.game.GetComponentInParent<MiniGame>();
			}
		}

		// Token: 0x060016C9 RID: 5833 RVA: 0x0005356E File Offset: 0x0005176E
		public void SetGame(MiniGame game)
		{
			this.game = game;
		}

		// Token: 0x060016CA RID: 5834 RVA: 0x00053577 File Offset: 0x00051777
		private void Update()
		{
			this.UpdateRotation();
			this.UpdateMovement();
			if (this.game.GetButtonDown(MiniGame.Button.B))
			{
				this.Jump();
				return;
			}
			if (this.game.GetButtonUp(MiniGame.Button.B))
			{
				this.StopJumping();
			}
		}

		// Token: 0x060016CB RID: 5835 RVA: 0x000535B0 File Offset: 0x000517B0
		private void UpdateMovement()
		{
			Vector2 axis = this.game.GetAxis(0);
			Vector3 vector = Vector3.zero;
			vector += Vector3.right * axis.x;
			vector += Vector3.forward * axis.y;
			if (base.camera)
			{
				vector = vector.relativeTo(base.cameraTransform, true);
			}
			base.SetMovementDirection(vector);
		}

		// Token: 0x060016CC RID: 5836 RVA: 0x00053620 File Offset: 0x00051820
		private void UpdateRotation()
		{
			Vector2 axis = this.game.GetAxis(1);
			this.AddYawInput(axis.x * this.lookSensitivity.x);
			if (axis.y == 0f)
			{
				return;
			}
			float num = MathLib.ClampAngle(-base.cameraTransform.localRotation.eulerAngles.x + axis.y * this.lookSensitivity.y, -80f, 80f);
			base.cameraTransform.localRotation = Quaternion.Euler(-num, 0f, 0f);
		}

		// Token: 0x060016CD RID: 5837 RVA: 0x000536B8 File Offset: 0x000518B8
		public void AddControlYawInput(float value)
		{
			this.AddYawInput(value);
		}

		// Token: 0x040010B3 RID: 4275
		[SerializeField]
		private MiniGame game;

		// Token: 0x040010B4 RID: 4276
		[SerializeField]
		private Vector2 lookSensitivity;
	}
}
