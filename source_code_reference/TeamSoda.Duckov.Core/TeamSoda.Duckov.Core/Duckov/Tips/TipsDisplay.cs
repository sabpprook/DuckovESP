using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Duckov.Tips
{
	// Token: 0x02000243 RID: 579
	public class TipsDisplay : MonoBehaviour
	{
		// Token: 0x06001205 RID: 4613 RVA: 0x00044B5C File Offset: 0x00042D5C
		public void DisplayRandom()
		{
			if (this.entries.Length == 0)
			{
				return;
			}
			TipEntry tipEntry = this.entries[global::UnityEngine.Random.Range(0, this.entries.Length)];
			this.text.text = tipEntry.Description;
		}

		// Token: 0x06001206 RID: 4614 RVA: 0x00044BA0 File Offset: 0x00042DA0
		public void Display(string tipID)
		{
			TipEntry tipEntry = this.entries.FirstOrDefault((TipEntry e) => e.TipID == tipID);
			if (tipEntry.TipID != tipID)
			{
				return;
			}
			this.text.text = tipEntry.Description;
		}

		// Token: 0x06001207 RID: 4615 RVA: 0x00044BF9 File Offset: 0x00042DF9
		private void OnEnable()
		{
			this.canvasGroup.alpha = (SceneLoader.HideTips ? 0f : 1f);
			this.DisplayRandom();
		}

		// Token: 0x04000DDD RID: 3549
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x04000DDE RID: 3550
		[SerializeField]
		private TipEntry[] entries;

		// Token: 0x04000DDF RID: 3551
		[SerializeField]
		private CanvasGroup canvasGroup;
	}
}
