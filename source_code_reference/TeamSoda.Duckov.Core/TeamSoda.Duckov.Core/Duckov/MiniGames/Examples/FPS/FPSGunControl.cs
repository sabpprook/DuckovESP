using System;
using UnityEngine;

namespace Duckov.MiniGames.Examples.FPS
{
	// Token: 0x020002D3 RID: 723
	public class FPSGunControl : MiniGameBehaviour
	{
		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x060016B5 RID: 5813 RVA: 0x0005327E File Offset: 0x0005147E
		public FPSGun Gun
		{
			get
			{
				return this.gun;
			}
		}

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x060016B6 RID: 5814 RVA: 0x00053286 File Offset: 0x00051486
		public float ScatterAngle
		{
			get
			{
				if (this.Gun)
				{
					return this.Gun.ScatterAngle;
				}
				return 0f;
			}
		}

		// Token: 0x060016B7 RID: 5815 RVA: 0x000532A6 File Offset: 0x000514A6
		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.gun != null)
			{
				this.SetGun(this.gun);
			}
		}

		// Token: 0x060016B8 RID: 5816 RVA: 0x000532C8 File Offset: 0x000514C8
		protected override void OnUpdate(float deltaTime)
		{
			bool buttonDown = base.Game.GetButtonDown(MiniGame.Button.A);
			bool buttonUp = base.Game.GetButtonUp(MiniGame.Button.A);
			if (buttonDown)
			{
				this.gun.SetTrigger(true);
			}
			if (buttonUp)
			{
				this.gun.SetTrigger(false);
			}
			this.UpdateGunPhysicsStatus(deltaTime);
		}

		// Token: 0x060016B9 RID: 5817 RVA: 0x00053312 File Offset: 0x00051512
		private void UpdateGunPhysicsStatus(float deltaTime)
		{
		}

		// Token: 0x060016BA RID: 5818 RVA: 0x00053314 File Offset: 0x00051514
		private void SetGun(FPSGun gunInstance)
		{
			if (gunInstance != this.gun)
			{
				global::UnityEngine.Object.Destroy(this.gun);
			}
			this.gun = gunInstance;
			this.SetupGunData();
		}

		// Token: 0x060016BB RID: 5819 RVA: 0x0005333C File Offset: 0x0005153C
		private void SetupGunData()
		{
			this.gun.Setup(this.mainCamera, this.gunParent);
		}

		// Token: 0x040010A7 RID: 4263
		[SerializeField]
		private Camera mainCamera;

		// Token: 0x040010A8 RID: 4264
		[SerializeField]
		private Transform gunParent;

		// Token: 0x040010A9 RID: 4265
		[SerializeField]
		private FPSGun gun;
	}
}
