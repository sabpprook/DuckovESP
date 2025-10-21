using System;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.Sounds
{
	// Token: 0x02000245 RID: 581
	public class SoundDisplay : MonoBehaviour
	{
		// Token: 0x1700032C RID: 812
		// (get) Token: 0x0600120D RID: 4621 RVA: 0x00044C50 File Offset: 0x00042E50
		public float Value
		{
			get
			{
				return this.value;
			}
		}

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x0600120E RID: 4622 RVA: 0x00044C58 File Offset: 0x00042E58
		public AISound CurrentSount
		{
			get
			{
				return this.sound;
			}
		}

		// Token: 0x0600120F RID: 4623 RVA: 0x00044C60 File Offset: 0x00042E60
		internal void Trigger(AISound sound)
		{
			this.sound = sound;
			base.gameObject.SetActive(true);
			this.velocity = this.triggerVelocity;
			this.value += this.velocity * Time.deltaTime;
		}

		// Token: 0x06001210 RID: 4624 RVA: 0x00044C9C File Offset: 0x00042E9C
		private void Update()
		{
			this.velocity -= this.gravity * Time.deltaTime;
			this.value += this.velocity * Time.deltaTime;
			if (this.value > 1f || this.value < 0f)
			{
				this.velocity = 0f;
			}
			this.value = Mathf.Clamp01(this.value);
			this.image.color = new Color(1f, 1f, 1f, this.value);
		}

		// Token: 0x04000DE1 RID: 3553
		[SerializeField]
		private Image image;

		// Token: 0x04000DE2 RID: 3554
		[SerializeField]
		private float removeRecordAfterTime = 1f;

		// Token: 0x04000DE3 RID: 3555
		[SerializeField]
		private float triggerVelocity = 10f;

		// Token: 0x04000DE4 RID: 3556
		[SerializeField]
		private float gravity = 1f;

		// Token: 0x04000DE5 RID: 3557
		[SerializeField]
		private float untriggerVelocity = 100f;

		// Token: 0x04000DE6 RID: 3558
		private float value;

		// Token: 0x04000DE7 RID: 3559
		private float velocity;

		// Token: 0x04000DE8 RID: 3560
		private AISound sound;
	}
}
