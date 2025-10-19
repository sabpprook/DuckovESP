using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Duckov.MiniGames.Utilities
{
	// Token: 0x02000283 RID: 643
	public class ControllerPickupAnimation : MonoBehaviour
	{
		// Token: 0x170003CA RID: 970
		// (get) Token: 0x06001495 RID: 5269 RVA: 0x0004C53C File Offset: 0x0004A73C
		private AnimationCurve pickupRotCurve
		{
			get
			{
				return this.pickupCurve;
			}
		}

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x06001496 RID: 5270 RVA: 0x0004C544 File Offset: 0x0004A744
		private AnimationCurve pickupPosCurve
		{
			get
			{
				return this.pickupCurve;
			}
		}

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x06001497 RID: 5271 RVA: 0x0004C54C File Offset: 0x0004A74C
		private AnimationCurve putDownCurve
		{
			get
			{
				return this.pickupCurve;
			}
		}

		// Token: 0x06001498 RID: 5272 RVA: 0x0004C554 File Offset: 0x0004A754
		public async UniTask PickUp(Transform endTransform)
		{
			if (!(this.controllerTransform == null))
			{
				int num = this.activeToken + 1;
				this.activeToken = num;
				int token = num;
				this.controllerTransform.DOKill(false);
				Vector3 fromPos = this.controllerTransform.position;
				Quaternion fromRot = this.controllerTransform.rotation;
				Vector3 toPos = endTransform.position;
				Quaternion toRot = endTransform.rotation;
				float time = 0f;
				while (time < this.transitionTime)
				{
					time += Time.deltaTime;
					float num2 = time / this.transitionTime;
					Vector3 vector = Vector3.LerpUnclamped(fromPos, toPos, this.pickupPosCurve.Evaluate(num2));
					Quaternion quaternion = Quaternion.SlerpUnclamped(fromRot, toRot, this.pickupRotCurve.Evaluate(num2));
					this.controllerTransform.SetPositionAndRotation(vector, quaternion);
					await UniTask.Yield();
					if (token != this.activeToken)
					{
						return;
					}
				}
				await this.controllerTransform.DOShakeRotation(0.4f, 10f, 10, 90f, true);
				this.controllerTransform.SetPositionAndRotation(toPos, toRot);
			}
		}

		// Token: 0x06001499 RID: 5273 RVA: 0x0004C5A0 File Offset: 0x0004A7A0
		public async UniTask PutDown()
		{
			if (!(this.controllerTransform == null))
			{
				int num = this.activeToken + 1;
				this.activeToken = num;
				int token = num;
				this.controllerTransform.DOKill(false);
				Vector3 fromPos = this.controllerTransform.position;
				Quaternion fromRot = this.controllerTransform.rotation;
				Vector3 toPos = this.restTransform.position;
				Quaternion toRot = this.restTransform.rotation;
				float time = 0f;
				while (time < this.transitionTime)
				{
					if (this.controllerTransform == null)
					{
						return;
					}
					time += Time.deltaTime;
					float num2 = time / this.transitionTime;
					Vector3 vector = Vector3.LerpUnclamped(fromPos, toPos, this.pickupPosCurve.Evaluate(num2));
					Quaternion quaternion = Quaternion.LerpUnclamped(fromRot, toRot, this.pickupRotCurve.Evaluate(num2));
					this.controllerTransform.SetPositionAndRotation(vector, quaternion);
					await UniTask.Yield();
					if (token != this.activeToken || this.controllerTransform == null)
					{
						return;
					}
				}
				this.controllerTransform.SetPositionAndRotation(toPos, toRot);
			}
		}

		// Token: 0x04000F1B RID: 3867
		[SerializeField]
		private Transform restTransform;

		// Token: 0x04000F1C RID: 3868
		[SerializeField]
		private Transform controllerTransform;

		// Token: 0x04000F1D RID: 3869
		[SerializeField]
		private float transitionTime = 1f;

		// Token: 0x04000F1E RID: 3870
		[SerializeField]
		private AnimationCurve pickupCurve;

		// Token: 0x04000F1F RID: 3871
		private int activeToken;
	}
}
