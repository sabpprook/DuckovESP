using System;
using Duckov.Utilities;
using ItemStatsSystem;
using TMPro;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x02000390 RID: 912
	public class ItemModifierEntry : MonoBehaviour, IPoolable
	{
		// Token: 0x06002011 RID: 8209 RVA: 0x0007023F File Offset: 0x0006E43F
		public void NotifyPooled()
		{
		}

		// Token: 0x06002012 RID: 8210 RVA: 0x00070241 File Offset: 0x0006E441
		public void NotifyReleased()
		{
			this.UnregisterEvents();
		}

		// Token: 0x06002013 RID: 8211 RVA: 0x00070249 File Offset: 0x0006E449
		internal void Setup(ModifierDescription target)
		{
			this.UnregisterEvents();
			this.target = target;
			this.Refresh();
			this.RegisterEvents();
		}

		// Token: 0x06002014 RID: 8212 RVA: 0x00070264 File Offset: 0x0006E464
		private void Refresh()
		{
			this.displayName.text = this.target.DisplayName;
			StatInfoDatabase.Entry entry = StatInfoDatabase.Get(this.target.Key);
			this.value.text = this.target.GetDisplayValueString(entry.DisplayFormat);
			Color color = this.color_Neutral;
			Polarity polarity = entry.polarity;
			if (this.target.Value != 0f)
			{
				switch (polarity)
				{
				case Polarity.Negative:
					color = ((this.target.Value < 0f) ? this.color_Positive : this.color_Negative);
					break;
				case Polarity.Positive:
					color = ((this.target.Value > 0f) ? this.color_Positive : this.color_Negative);
					break;
				}
			}
			this.value.color = color;
		}

		// Token: 0x06002015 RID: 8213 RVA: 0x0007033B File Offset: 0x0006E53B
		private void RegisterEvents()
		{
			ModifierDescription modifierDescription = this.target;
		}

		// Token: 0x06002016 RID: 8214 RVA: 0x00070344 File Offset: 0x0006E544
		private void UnregisterEvents()
		{
			ModifierDescription modifierDescription = this.target;
		}

		// Token: 0x040015E7 RID: 5607
		private ModifierDescription target;

		// Token: 0x040015E8 RID: 5608
		[SerializeField]
		private TextMeshProUGUI displayName;

		// Token: 0x040015E9 RID: 5609
		[SerializeField]
		private TextMeshProUGUI value;

		// Token: 0x040015EA RID: 5610
		[SerializeField]
		private Color color_Neutral;

		// Token: 0x040015EB RID: 5611
		[SerializeField]
		private Color color_Positive;

		// Token: 0x040015EC RID: 5612
		[SerializeField]
		private Color color_Negative;
	}
}
