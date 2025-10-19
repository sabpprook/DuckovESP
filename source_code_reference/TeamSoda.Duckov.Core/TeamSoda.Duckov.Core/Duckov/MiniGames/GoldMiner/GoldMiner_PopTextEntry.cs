using System;
using TMPro;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x020002A0 RID: 672
	public class GoldMiner_PopTextEntry : MonoBehaviour
	{
		// Token: 0x060015D6 RID: 5590 RVA: 0x00050BFC File Offset: 0x0004EDFC
		public void Setup(Vector3 pos, string text, Action<GoldMiner_PopTextEntry> releaseAction)
		{
			this.initialized = true;
			this.tmp.text = text;
			this.life = 0f;
			base.transform.position = pos;
			this.releaseAction = releaseAction;
		}

		// Token: 0x060015D7 RID: 5591 RVA: 0x00050C30 File Offset: 0x0004EE30
		private void Update()
		{
			if (!this.initialized)
			{
				return;
			}
			this.life += Time.deltaTime;
			base.transform.position += Vector3.up * this.moveSpeed * Time.deltaTime;
			if (this.life >= this.lifeTime)
			{
				this.Release();
			}
		}

		// Token: 0x060015D8 RID: 5592 RVA: 0x00050C9C File Offset: 0x0004EE9C
		private void Release()
		{
			if (this.releaseAction != null)
			{
				this.releaseAction(this);
				return;
			}
			global::UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x04001031 RID: 4145
		public TextMeshProUGUI tmp;

		// Token: 0x04001032 RID: 4146
		public float lifeTime;

		// Token: 0x04001033 RID: 4147
		public float moveSpeed = 1f;

		// Token: 0x04001034 RID: 4148
		private bool initialized;

		// Token: 0x04001035 RID: 4149
		private float life;

		// Token: 0x04001036 RID: 4150
		private Action<GoldMiner_PopTextEntry> releaseAction;
	}
}
