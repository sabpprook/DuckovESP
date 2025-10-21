using System;
using UnityEngine;
using UnityEngine.Events;

namespace Duckov.BasketBalls
{
	// Token: 0x0200030D RID: 781
	public class BasketTrigger : MonoBehaviour
	{
		// Token: 0x060019AB RID: 6571 RVA: 0x0005CB34 File Offset: 0x0005AD34
		private void OnTriggerEnter(Collider other)
		{
			Debug.Log("ONTRIGGERENTER:" + other.name);
			BasketBall component = other.GetComponent<BasketBall>();
			if (component == null)
			{
				return;
			}
			this.onGoal.Invoke(component);
		}

		// Token: 0x04001291 RID: 4753
		public UnityEvent<BasketBall> onGoal;
	}
}
