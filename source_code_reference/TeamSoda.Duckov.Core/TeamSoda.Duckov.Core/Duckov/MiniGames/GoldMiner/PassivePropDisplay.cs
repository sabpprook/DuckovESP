using System;
using Duckov.MiniGames.GoldMiner.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x020002A4 RID: 676
	public class PassivePropDisplay : MonoBehaviour
	{
		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x060015F8 RID: 5624 RVA: 0x00051142 File Offset: 0x0004F342
		// (set) Token: 0x060015F9 RID: 5625 RVA: 0x0005114A File Offset: 0x0004F34A
		public RectTransform rectTransform { get; private set; }

		// Token: 0x1700040B RID: 1035
		// (get) Token: 0x060015FA RID: 5626 RVA: 0x00051153 File Offset: 0x0004F353
		public NavEntry NavEntry
		{
			get
			{
				return this.navEntry;
			}
		}

		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x060015FB RID: 5627 RVA: 0x0005115B File Offset: 0x0004F35B
		// (set) Token: 0x060015FC RID: 5628 RVA: 0x00051163 File Offset: 0x0004F363
		public GoldMinerArtifact Target { get; private set; }

		// Token: 0x060015FD RID: 5629 RVA: 0x0005116C File Offset: 0x0004F36C
		internal void Setup(GoldMinerArtifact target, int amount)
		{
			this.Target = target;
			this.icon.sprite = target.Icon;
			this.rectTransform = base.transform as RectTransform;
			this.amounText.text = ((amount > 1) ? string.Format("{0}", amount) : "");
		}

		// Token: 0x04001052 RID: 4178
		[SerializeField]
		private NavEntry navEntry;

		// Token: 0x04001053 RID: 4179
		[SerializeField]
		private Image icon;

		// Token: 0x04001054 RID: 4180
		[SerializeField]
		private TextMeshProUGUI amounText;
	}
}
