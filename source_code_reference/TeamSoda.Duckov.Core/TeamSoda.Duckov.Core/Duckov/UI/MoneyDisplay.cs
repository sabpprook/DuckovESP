using System;
using Duckov.Economy;
using Saves;
using TMPro;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x0200037B RID: 891
	public class MoneyDisplay : MonoBehaviour
	{
		// Token: 0x06001EE3 RID: 7907 RVA: 0x0006C68A File Offset: 0x0006A88A
		private void Awake()
		{
			EconomyManager.OnMoneyChanged += this.OnMoneyChanged;
			SavesSystem.OnSetFile += this.OnSaveFileChanged;
			this.Refresh();
		}

		// Token: 0x06001EE4 RID: 7908 RVA: 0x0006C6B4 File Offset: 0x0006A8B4
		private void OnDestroy()
		{
			EconomyManager.OnMoneyChanged -= this.OnMoneyChanged;
			SavesSystem.OnSetFile -= this.OnSaveFileChanged;
		}

		// Token: 0x06001EE5 RID: 7909 RVA: 0x0006C6D8 File Offset: 0x0006A8D8
		private void OnEnable()
		{
			this.Refresh();
		}

		// Token: 0x06001EE6 RID: 7910 RVA: 0x0006C6E0 File Offset: 0x0006A8E0
		private void Refresh()
		{
			this.text.text = EconomyManager.Money.ToString(this.format);
		}

		// Token: 0x06001EE7 RID: 7911 RVA: 0x0006C70B File Offset: 0x0006A90B
		private void OnMoneyChanged(long arg1, long arg2)
		{
			this.Refresh();
		}

		// Token: 0x06001EE8 RID: 7912 RVA: 0x0006C713 File Offset: 0x0006A913
		private void OnSaveFileChanged()
		{
			this.Refresh();
		}

		// Token: 0x0400152A RID: 5418
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x0400152B RID: 5419
		[SerializeField]
		private string format = "n0";
	}
}
