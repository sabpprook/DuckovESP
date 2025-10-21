using System;
using System.Collections.Generic;
using UnityEngine;

namespace Duckov.MiniGames.Examples.HelloWorld
{
	// Token: 0x020002CC RID: 716
	public class Move : MiniGameBehaviour
	{
		// Token: 0x06001691 RID: 5777 RVA: 0x00052B0F File Offset: 0x00050D0F
		private void Awake()
		{
			if (this.rigidbody == null)
			{
				this.rigidbody = base.GetComponent<Rigidbody>();
			}
		}

		// Token: 0x06001692 RID: 5778 RVA: 0x00052B2C File Offset: 0x00050D2C
		protected override void OnUpdate(float deltaTime)
		{
			bool flag = this.CanJump();
			Vector2 vector = base.Game.GetAxis(0) * this.speed;
			float y = this.rigidbody.velocity.y;
			if (base.Game.GetButtonDown(MiniGame.Button.A) && flag)
			{
				y = this.jumpSpeed;
			}
			this.rigidbody.velocity = new Vector3(vector.x, y, vector.y);
		}

		// Token: 0x06001693 RID: 5779 RVA: 0x00052B9D File Offset: 0x00050D9D
		private bool CanJump()
		{
			return this.touchingColliders.Count > 0;
		}

		// Token: 0x06001694 RID: 5780 RVA: 0x00052BB0 File Offset: 0x00050DB0
		private void OnCollisionEnter(Collision collision)
		{
			this.touchingColliders.Add(collision.collider);
		}

		// Token: 0x06001695 RID: 5781 RVA: 0x00052BC3 File Offset: 0x00050DC3
		private void OnCollisionExit(Collision collision)
		{
			this.touchingColliders.Remove(collision.collider);
		}

		// Token: 0x04001082 RID: 4226
		[SerializeField]
		private Rigidbody rigidbody;

		// Token: 0x04001083 RID: 4227
		[SerializeField]
		private float speed = 10f;

		// Token: 0x04001084 RID: 4228
		[SerializeField]
		private float jumpSpeed = 5f;

		// Token: 0x04001085 RID: 4229
		private List<Collider> touchingColliders = new List<Collider>();
	}
}
