using System;
using UnityEngine;

namespace Duckov.UI.Animations
{
	// Token: 0x020003DB RID: 987
	public class Revolver : MonoBehaviour
	{
		// Token: 0x060023C8 RID: 9160 RVA: 0x0007CC7C File Offset: 0x0007AE7C
		private void Update()
		{
			Quaternion quaternion = Quaternion.AngleAxis(Time.deltaTime * this.rPM / 60f * 360f, this.axis);
			Vector3 vector = base.transform.localPosition - this.pivot;
			Vector3 vector2 = quaternion * vector;
			Vector3 vector3 = this.pivot + vector2;
			base.transform.localPosition = vector3;
		}

		// Token: 0x060023C9 RID: 9161 RVA: 0x0007CCE4 File Offset: 0x0007AEE4
		private void OnDrawGizmosSelected()
		{
			if (base.transform.parent != null)
			{
				Gizmos.matrix = base.transform.parent.localToWorldMatrix;
			}
			Gizmos.DrawLine(this.pivot, base.transform.localPosition);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(this.pivot, 1f);
		}

		// Token: 0x0400184D RID: 6221
		public Vector3 pivot;

		// Token: 0x0400184E RID: 6222
		public Vector3 axis = Vector3.forward;

		// Token: 0x0400184F RID: 6223
		public float rPM;
	}
}
