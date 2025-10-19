using System;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using Duckov.Utilities;
using ItemStatsSystem;
using SodaCraft.Localizations;
using UnityEngine;

// Token: 0x020000F9 RID: 249
public class UseToCreateItem : UsageBehavior
{
	// Token: 0x170001B7 RID: 439
	// (get) Token: 0x06000851 RID: 2129 RVA: 0x00024FD4 File Offset: 0x000231D4
	public override UsageBehavior.DisplaySettingsData DisplaySettings
	{
		get
		{
			return new UsageBehavior.DisplaySettingsData
			{
				display = true,
				description = this.descKey.ToPlainText()
			};
		}
	}

	// Token: 0x06000852 RID: 2130 RVA: 0x00025004 File Offset: 0x00023204
	public override bool CanBeUsed(Item item, object user)
	{
		return user as CharacterMainControl;
	}

	// Token: 0x06000853 RID: 2131 RVA: 0x00025018 File Offset: 0x00023218
	protected override void OnUse(Item item, object user)
	{
		CharacterMainControl characterMainControl = user as CharacterMainControl;
		if (!characterMainControl)
		{
			return;
		}
		if (this.entries.entries.Count == 0)
		{
			return;
		}
		UseToCreateItem.Entry random = this.entries.GetRandom(0f);
		this.Generate(random.itemTypeID, characterMainControl).Forget();
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x0002506C File Offset: 0x0002326C
	private async UniTask Generate(int typeID, CharacterMainControl character)
	{
		if (!this.running)
		{
			this.running = true;
			Item item = await ItemAssetsCollection.InstantiateAsync(typeID);
			string displayName = item.DisplayName;
			bool flag = character.PickupItem(item);
			NotificationText.Push(this.notificationKey.ToPlainText() + " " + displayName);
			if (!flag && item != null)
			{
				if (item.ActiveAgent != null)
				{
					item.AgentUtilities.ReleaseActiveAgent();
				}
				PlayerStorage.Push(item, false);
			}
			this.running = false;
		}
	}

	// Token: 0x06000855 RID: 2133 RVA: 0x000250BF File Offset: 0x000232BF
	private void OnValidate()
	{
		this.entries.RefreshPercent();
	}

	// Token: 0x0400078A RID: 1930
	[SerializeField]
	private RandomContainer<UseToCreateItem.Entry> entries;

	// Token: 0x0400078B RID: 1931
	[LocalizationKey("Items")]
	public string descKey;

	// Token: 0x0400078C RID: 1932
	[LocalizationKey("Default")]
	public string notificationKey;

	// Token: 0x0400078D RID: 1933
	private bool running;

	// Token: 0x0200047E RID: 1150
	[Serializable]
	private struct Entry
	{
		// Token: 0x04001B6F RID: 7023
		[ItemTypeID]
		[SerializeField]
		public int itemTypeID;
	}
}
