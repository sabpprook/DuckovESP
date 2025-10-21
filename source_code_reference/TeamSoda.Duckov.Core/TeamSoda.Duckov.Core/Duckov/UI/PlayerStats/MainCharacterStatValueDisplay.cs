using System;
using ItemStatsSystem;
using TMPro;
using UnityEngine;

namespace Duckov.UI.PlayerStats
{
	// Token: 0x020003C8 RID: 968
	public class MainCharacterStatValueDisplay : MonoBehaviour
	{
		// Token: 0x0600232D RID: 9005 RVA: 0x0007B328 File Offset: 0x00079528
		private void OnEnable()
		{
			if (this.target == null)
			{
				CharacterMainControl main = CharacterMainControl.Main;
				Stat stat;
				if (main == null)
				{
					stat = null;
				}
				else
				{
					Item characterItem = main.CharacterItem;
					stat = ((characterItem != null) ? characterItem.GetStat(this.statKey.GetHashCode()) : null);
				}
				this.target = stat;
			}
			this.Refresh();
			this.RegisterEvents();
		}

		// Token: 0x0600232E RID: 9006 RVA: 0x0007B377 File Offset: 0x00079577
		private void OnDisable()
		{
			this.UnregisterEvents();
		}

		// Token: 0x0600232F RID: 9007 RVA: 0x0007B37F File Offset: 0x0007957F
		private void AutoRename()
		{
			base.gameObject.name = "StatDisplay_" + this.statKey;
		}

		// Token: 0x06002330 RID: 9008 RVA: 0x0007B39C File Offset: 0x0007959C
		private void RegisterEvents()
		{
			if (this.target == null)
			{
				return;
			}
			this.target.OnSetDirty += this.OnTargetDirty;
		}

		// Token: 0x06002331 RID: 9009 RVA: 0x0007B3BE File Offset: 0x000795BE
		private void UnregisterEvents()
		{
			if (this.target == null)
			{
				return;
			}
			this.target.OnSetDirty -= this.OnTargetDirty;
		}

		// Token: 0x06002332 RID: 9010 RVA: 0x0007B3E0 File Offset: 0x000795E0
		private void OnTargetDirty(Stat stat)
		{
			this.Refresh();
		}

		// Token: 0x06002333 RID: 9011 RVA: 0x0007B3E8 File Offset: 0x000795E8
		private void Refresh()
		{
			if (this.target == null)
			{
				return;
			}
			this.displayNameText.text = this.target.DisplayName;
			float value = this.target.Value;
			this.valueText.text = string.Format(this.format, value);
		}

		// Token: 0x040017F3 RID: 6131
		[SerializeField]
		private string statKey;

		// Token: 0x040017F4 RID: 6132
		[SerializeField]
		private TextMeshProUGUI displayNameText;

		// Token: 0x040017F5 RID: 6133
		[SerializeField]
		private TextMeshProUGUI valueText;

		// Token: 0x040017F6 RID: 6134
		[SerializeField]
		private string format = "{0:0.0}";

		// Token: 0x040017F7 RID: 6135
		private Stat target;
	}
}
