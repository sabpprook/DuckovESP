using System;
using TMPro;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x0200029A RID: 666
	public class GoldMinerMoneyDisplay : MonoBehaviour
	{
		// Token: 0x060015B6 RID: 5558 RVA: 0x00050688 File Offset: 0x0004E888
		private void Update()
		{
			this.text.text = this.master.Money.ToString(this.format);
		}

		// Token: 0x04001012 RID: 4114
		[SerializeField]
		private GoldMiner master;

		// Token: 0x04001013 RID: 4115
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x04001014 RID: 4116
		[SerializeField]
		private string format = "$0";
	}
}
