using System;
using TMPro;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x0200029B RID: 667
	public class GoldMinerShopUI : MiniGameBehaviour
	{
		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x060015B8 RID: 5560 RVA: 0x000506CC File Offset: 0x0004E8CC
		// (set) Token: 0x060015B9 RID: 5561 RVA: 0x000506D4 File Offset: 0x0004E8D4
		public GoldMinerShop target { get; private set; }

		// Token: 0x060015BA RID: 5562 RVA: 0x000506DD File Offset: 0x0004E8DD
		private void UnregisterEvent()
		{
			if (this.target == null)
			{
				return;
			}
			GoldMinerShop target = this.target;
			target.onAfterOperation = (Action)Delegate.Remove(target.onAfterOperation, new Action(this.OnAfterOperation));
		}

		// Token: 0x060015BB RID: 5563 RVA: 0x00050715 File Offset: 0x0004E915
		private void RegisterEvent()
		{
			if (this.target == null)
			{
				return;
			}
			GoldMinerShop target = this.target;
			target.onAfterOperation = (Action)Delegate.Combine(target.onAfterOperation, new Action(this.OnAfterOperation));
		}

		// Token: 0x060015BC RID: 5564 RVA: 0x0005074D File Offset: 0x0004E94D
		private void OnAfterOperation()
		{
			this.RefreshEntries();
		}

		// Token: 0x060015BD RID: 5565 RVA: 0x00050758 File Offset: 0x0004E958
		private void RefreshEntries()
		{
			for (int i = 0; i < this.entries.Length; i++)
			{
				GoldMinerShopUIEntry goldMinerShopUIEntry = this.entries[i];
				if (i >= this.target.stock.Count)
				{
					goldMinerShopUIEntry.gameObject.SetActive(false);
				}
				else
				{
					goldMinerShopUIEntry.gameObject.SetActive(true);
					ShopEntity shopEntity = this.target.stock[i];
					goldMinerShopUIEntry.Setup(this, shopEntity);
				}
			}
		}

		// Token: 0x060015BE RID: 5566 RVA: 0x000507C8 File Offset: 0x0004E9C8
		public void Setup(GoldMinerShop shop)
		{
			this.UnregisterEvent();
			this.target = shop;
			this.RegisterEvent();
			this.RefreshEntries();
		}

		// Token: 0x060015BF RID: 5567 RVA: 0x000507E3 File Offset: 0x0004E9E3
		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			this.RefreshDescriptionText();
		}

		// Token: 0x060015C0 RID: 5568 RVA: 0x000507F4 File Offset: 0x0004E9F4
		private void RefreshDescriptionText()
		{
			string text = "";
			if (this.hoveringEntry != null && this.hoveringEntry.target != null && this.hoveringEntry.target.artifact != null)
			{
				text = this.hoveringEntry.target.artifact.Description;
			}
			this.descriptionText.text = text;
		}

		// Token: 0x04001015 RID: 4117
		[SerializeField]
		private GoldMiner master;

		// Token: 0x04001016 RID: 4118
		[SerializeField]
		private TextMeshProUGUI descriptionText;

		// Token: 0x04001017 RID: 4119
		[SerializeField]
		private GoldMinerShopUIEntry[] entries;

		// Token: 0x04001018 RID: 4120
		public int navIndex;

		// Token: 0x0400101A RID: 4122
		public bool enableInput;

		// Token: 0x0400101B RID: 4123
		public GoldMinerShopUIEntry hoveringEntry;
	}
}
