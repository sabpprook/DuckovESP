using System;
using Duckov.Utilities;
using ItemStatsSystem;
using LeTai.TrueShadow;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x0200038D RID: 909
	public class ItemDetailsDisplay : MonoBehaviour
	{
		// Token: 0x17000622 RID: 1570
		// (get) Token: 0x06001FE4 RID: 8164 RVA: 0x0006F893 File Offset: 0x0006DA93
		private string DurabilityToolTipsFormat
		{
			get
			{
				return this.durabilityToolTipsFormatKey.ToPlainText();
			}
		}

		// Token: 0x17000623 RID: 1571
		// (get) Token: 0x06001FE5 RID: 8165 RVA: 0x0006F8A0 File Offset: 0x0006DAA0
		public ItemSlotCollectionDisplay SlotCollectionDisplay
		{
			get
			{
				return this.slotCollectionDisplay;
			}
		}

		// Token: 0x17000624 RID: 1572
		// (get) Token: 0x06001FE6 RID: 8166 RVA: 0x0006F8A8 File Offset: 0x0006DAA8
		private PrefabPool<ItemVariableEntry> VariablePool
		{
			get
			{
				if (this._variablePool == null)
				{
					this._variablePool = new PrefabPool<ItemVariableEntry>(this.variableEntryPrefab, this.propertiesParent, null, null, null, true, 10, 10000, null);
				}
				return this._variablePool;
			}
		}

		// Token: 0x17000625 RID: 1573
		// (get) Token: 0x06001FE7 RID: 8167 RVA: 0x0006F8E8 File Offset: 0x0006DAE8
		private PrefabPool<ItemStatEntry> StatPool
		{
			get
			{
				if (this._statPool == null)
				{
					this._statPool = new PrefabPool<ItemStatEntry>(this.statEntryPrefab, this.propertiesParent, null, null, null, true, 10, 10000, null);
				}
				return this._statPool;
			}
		}

		// Token: 0x17000626 RID: 1574
		// (get) Token: 0x06001FE8 RID: 8168 RVA: 0x0006F928 File Offset: 0x0006DB28
		private PrefabPool<ItemModifierEntry> ModifierPool
		{
			get
			{
				if (this._modifierPool == null)
				{
					this._modifierPool = new PrefabPool<ItemModifierEntry>(this.modifierEntryPrefab, this.propertiesParent, null, null, null, true, 10, 10000, null);
				}
				return this._modifierPool;
			}
		}

		// Token: 0x17000627 RID: 1575
		// (get) Token: 0x06001FE9 RID: 8169 RVA: 0x0006F968 File Offset: 0x0006DB68
		private PrefabPool<ItemEffectEntry> EffectPool
		{
			get
			{
				if (this._effectPool == null)
				{
					this._effectPool = new PrefabPool<ItemEffectEntry>(this.effectEntryPrefab, this.propertiesParent, null, null, null, true, 10, 10000, null);
				}
				return this._effectPool;
			}
		}

		// Token: 0x17000628 RID: 1576
		// (get) Token: 0x06001FEA RID: 8170 RVA: 0x0006F9A6 File Offset: 0x0006DBA6
		public Item Target
		{
			get
			{
				return this.target;
			}
		}

		// Token: 0x06001FEB RID: 8171 RVA: 0x0006F9B0 File Offset: 0x0006DBB0
		internal void Setup(Item target)
		{
			this.UnregisterEvents();
			this.Clear();
			if (target == null)
			{
				return;
			}
			this.target = target;
			this.icon.sprite = target.Icon;
			ValueTuple<float, Color, bool> shadowOffsetAndColorOfQuality = GameplayDataSettings.UIStyle.GetShadowOffsetAndColorOfQuality(target.DisplayQuality);
			this.iconShadow.IgnoreCasterColor = true;
			this.iconShadow.OffsetDistance = shadowOffsetAndColorOfQuality.Item1;
			this.iconShadow.Color = shadowOffsetAndColorOfQuality.Item2;
			this.iconShadow.Inset = shadowOffsetAndColorOfQuality.Item3;
			this.displayName.text = target.DisplayName;
			this.itemID.text = string.Format("#{0}", target.TypeID);
			this.description.text = target.Description;
			this.countContainer.SetActive(target.Stackable);
			this.count.text = target.StackCount.ToString();
			this.tagsDisplay.Setup(target);
			this.usageUtilitiesDisplay.Setup(target);
			this.usableIndicator.gameObject.SetActive(target.UsageUtilities != null);
			this.RefreshDurability();
			this.slotCollectionDisplay.Setup(target, false);
			this.registeredIndicator.SetActive(target.IsRegistered());
			this.RefreshWeightText();
			this.SetupGunDisplays();
			this.SetupVariables();
			this.SetupConstants();
			this.SetupStats();
			this.SetupModifiers();
			this.SetupEffects();
			this.RegisterEvents();
		}

		// Token: 0x06001FEC RID: 8172 RVA: 0x0006FB2F File Offset: 0x0006DD2F
		private void Awake()
		{
			this.SlotCollectionDisplay.onElementDoubleClicked += this.OnElementDoubleClicked;
		}

		// Token: 0x06001FED RID: 8173 RVA: 0x0006FB48 File Offset: 0x0006DD48
		private void OnElementDoubleClicked(ItemSlotCollectionDisplay collectionDisplay, SlotDisplay slotDisplay)
		{
			if (!collectionDisplay.Editable)
			{
				return;
			}
			Item item = slotDisplay.GetItem();
			if (item == null)
			{
				return;
			}
			ItemUtilities.SendToPlayer(item, false, PlayerStorage.Instance != null);
		}

		// Token: 0x06001FEE RID: 8174 RVA: 0x0006FB81 File Offset: 0x0006DD81
		private void OnDestroy()
		{
			this.UnregisterEvents();
		}

		// Token: 0x06001FEF RID: 8175 RVA: 0x0006FB89 File Offset: 0x0006DD89
		private void Clear()
		{
			this.tagsDisplay.Clear();
			this.VariablePool.ReleaseAll();
			this.StatPool.ReleaseAll();
			this.ModifierPool.ReleaseAll();
			this.EffectPool.ReleaseAll();
		}

		// Token: 0x06001FF0 RID: 8176 RVA: 0x0006FBC4 File Offset: 0x0006DDC4
		private void SetupGunDisplays()
		{
			Item item = this.Target;
			ItemSetting_Gun itemSetting_Gun = ((item != null) ? item.GetComponent<ItemSetting_Gun>() : null);
			if (itemSetting_Gun == null)
			{
				this.bulletTypeDisplay.gameObject.SetActive(false);
				return;
			}
			this.bulletTypeDisplay.gameObject.SetActive(true);
			this.bulletTypeDisplay.Setup(itemSetting_Gun.TargetBulletID);
		}

		// Token: 0x06001FF1 RID: 8177 RVA: 0x0006FC24 File Offset: 0x0006DE24
		private void SetupVariables()
		{
			if (this.target.Variables == null)
			{
				return;
			}
			foreach (CustomData customData in this.target.Variables)
			{
				if (customData.Display)
				{
					ItemVariableEntry itemVariableEntry = this.VariablePool.Get(this.propertiesParent);
					itemVariableEntry.Setup(customData);
					itemVariableEntry.transform.SetAsLastSibling();
				}
			}
		}

		// Token: 0x06001FF2 RID: 8178 RVA: 0x0006FCA8 File Offset: 0x0006DEA8
		private void SetupConstants()
		{
			if (this.target.Constants == null)
			{
				return;
			}
			foreach (CustomData customData in this.target.Constants)
			{
				if (customData.Display)
				{
					ItemVariableEntry itemVariableEntry = this.VariablePool.Get(this.propertiesParent);
					itemVariableEntry.Setup(customData);
					itemVariableEntry.transform.SetAsLastSibling();
				}
			}
		}

		// Token: 0x06001FF3 RID: 8179 RVA: 0x0006FD2C File Offset: 0x0006DF2C
		private void SetupStats()
		{
			if (this.target.Stats == null)
			{
				return;
			}
			foreach (Stat stat in this.target.Stats)
			{
				if (stat.Display)
				{
					ItemStatEntry itemStatEntry = this.StatPool.Get(this.propertiesParent);
					itemStatEntry.Setup(stat);
					itemStatEntry.transform.SetAsLastSibling();
				}
			}
		}

		// Token: 0x06001FF4 RID: 8180 RVA: 0x0006FDB8 File Offset: 0x0006DFB8
		private void SetupModifiers()
		{
			if (this.target.Modifiers == null)
			{
				return;
			}
			foreach (ModifierDescription modifierDescription in this.target.Modifiers)
			{
				if (modifierDescription.Display)
				{
					ItemModifierEntry itemModifierEntry = this.ModifierPool.Get(this.propertiesParent);
					itemModifierEntry.Setup(modifierDescription);
					itemModifierEntry.transform.SetAsLastSibling();
				}
			}
		}

		// Token: 0x06001FF5 RID: 8181 RVA: 0x0006FE44 File Offset: 0x0006E044
		private void SetupEffects()
		{
			foreach (Effect effect in this.target.Effects)
			{
				if (effect.Display)
				{
					ItemEffectEntry itemEffectEntry = this.EffectPool.Get(this.propertiesParent);
					itemEffectEntry.Setup(effect);
					itemEffectEntry.transform.SetAsLastSibling();
				}
			}
		}

		// Token: 0x06001FF6 RID: 8182 RVA: 0x0006FEC0 File Offset: 0x0006E0C0
		private void RegisterEvents()
		{
			if (this.target == null)
			{
				return;
			}
			this.target.onDestroy += this.OnTargetDestroy;
			this.target.onChildChanged += this.OnTargetChildChanged;
			this.target.onSetStackCount += this.OnTargetSetStackCount;
			this.target.onDurabilityChanged += this.OnTargetDurabilityChanged;
		}

		// Token: 0x06001FF7 RID: 8183 RVA: 0x0006FF38 File Offset: 0x0006E138
		private void RefreshWeightText()
		{
			this.weightText.text = string.Format(this.weightFormat, this.target.TotalWeight);
		}

		// Token: 0x06001FF8 RID: 8184 RVA: 0x0006FF60 File Offset: 0x0006E160
		private void OnTargetSetStackCount(Item item)
		{
			this.RefreshWeightText();
		}

		// Token: 0x06001FF9 RID: 8185 RVA: 0x0006FF68 File Offset: 0x0006E168
		private void OnTargetChildChanged(Item obj)
		{
			this.RefreshWeightText();
		}

		// Token: 0x06001FFA RID: 8186 RVA: 0x0006FF70 File Offset: 0x0006E170
		internal void UnregisterEvents()
		{
			if (this.target == null)
			{
				return;
			}
			this.target.onDestroy -= this.OnTargetDestroy;
			this.target.onChildChanged -= this.OnTargetChildChanged;
			this.target.onSetStackCount -= this.OnTargetSetStackCount;
			this.target.onDurabilityChanged -= this.OnTargetDurabilityChanged;
		}

		// Token: 0x06001FFB RID: 8187 RVA: 0x0006FFE8 File Offset: 0x0006E1E8
		private void OnTargetDurabilityChanged(Item item)
		{
			this.RefreshDurability();
		}

		// Token: 0x06001FFC RID: 8188 RVA: 0x0006FFF0 File Offset: 0x0006E1F0
		private void RefreshDurability()
		{
			bool useDurability = this.target.UseDurability;
			this.durabilityContainer.SetActive(useDurability);
			if (useDurability)
			{
				float durability = this.target.Durability;
				float maxDurability = this.target.MaxDurability;
				float maxDurabilityWithLoss = this.target.MaxDurabilityWithLoss;
				string text = string.Format("{0:0}%", this.target.DurabilityLoss * 100f);
				float num = durability / maxDurability;
				this.durabilityText.text = string.Format("{0:0} / {1:0}", durability, maxDurabilityWithLoss);
				this.durabilityToolTips.text = this.DurabilityToolTipsFormat.Format(new
				{
					curDurability = durability,
					maxDurability = maxDurability,
					maxDurabilityWithLoss = maxDurabilityWithLoss,
					lossPercentage = text
				});
				this.durabilityFill.fillAmount = num;
				this.durabilityFill.color = this.durabilityColorOverT.Evaluate(num);
				this.durabilityLoss.fillAmount = this.target.DurabilityLoss;
			}
		}

		// Token: 0x06001FFD RID: 8189 RVA: 0x000700E2 File Offset: 0x0006E2E2
		private void OnTargetDestroy(Item item)
		{
		}

		// Token: 0x040015BF RID: 5567
		[SerializeField]
		private Image icon;

		// Token: 0x040015C0 RID: 5568
		[SerializeField]
		private TrueShadow iconShadow;

		// Token: 0x040015C1 RID: 5569
		[SerializeField]
		private TextMeshProUGUI displayName;

		// Token: 0x040015C2 RID: 5570
		[SerializeField]
		private TextMeshProUGUI itemID;

		// Token: 0x040015C3 RID: 5571
		[SerializeField]
		private TextMeshProUGUI description;

		// Token: 0x040015C4 RID: 5572
		[SerializeField]
		private GameObject countContainer;

		// Token: 0x040015C5 RID: 5573
		[SerializeField]
		private TextMeshProUGUI count;

		// Token: 0x040015C6 RID: 5574
		[SerializeField]
		private GameObject durabilityContainer;

		// Token: 0x040015C7 RID: 5575
		[SerializeField]
		private TextMeshProUGUI durabilityText;

		// Token: 0x040015C8 RID: 5576
		[SerializeField]
		private TooltipsProvider durabilityToolTips;

		// Token: 0x040015C9 RID: 5577
		[SerializeField]
		[LocalizationKey("Default")]
		private string durabilityToolTipsFormatKey = "UI_DurabilityToolTips";

		// Token: 0x040015CA RID: 5578
		[SerializeField]
		private Image durabilityFill;

		// Token: 0x040015CB RID: 5579
		[SerializeField]
		private Image durabilityLoss;

		// Token: 0x040015CC RID: 5580
		[SerializeField]
		private Gradient durabilityColorOverT;

		// Token: 0x040015CD RID: 5581
		[SerializeField]
		private TextMeshProUGUI weightText;

		// Token: 0x040015CE RID: 5582
		[SerializeField]
		private ItemSlotCollectionDisplay slotCollectionDisplay;

		// Token: 0x040015CF RID: 5583
		[SerializeField]
		private RectTransform propertiesParent;

		// Token: 0x040015D0 RID: 5584
		[SerializeField]
		private BulletTypeDisplay bulletTypeDisplay;

		// Token: 0x040015D1 RID: 5585
		[SerializeField]
		private TagsDisplay tagsDisplay;

		// Token: 0x040015D2 RID: 5586
		[SerializeField]
		private GameObject usableIndicator;

		// Token: 0x040015D3 RID: 5587
		[SerializeField]
		private UsageUtilitiesDisplay usageUtilitiesDisplay;

		// Token: 0x040015D4 RID: 5588
		[SerializeField]
		private GameObject registeredIndicator;

		// Token: 0x040015D5 RID: 5589
		[SerializeField]
		private ItemVariableEntry variableEntryPrefab;

		// Token: 0x040015D6 RID: 5590
		[SerializeField]
		private ItemStatEntry statEntryPrefab;

		// Token: 0x040015D7 RID: 5591
		[SerializeField]
		private ItemModifierEntry modifierEntryPrefab;

		// Token: 0x040015D8 RID: 5592
		[SerializeField]
		private ItemEffectEntry effectEntryPrefab;

		// Token: 0x040015D9 RID: 5593
		[SerializeField]
		private string weightFormat = "{0:0.#} kg";

		// Token: 0x040015DA RID: 5594
		private Item target;

		// Token: 0x040015DB RID: 5595
		private PrefabPool<ItemVariableEntry> _variablePool;

		// Token: 0x040015DC RID: 5596
		private PrefabPool<ItemStatEntry> _statPool;

		// Token: 0x040015DD RID: 5597
		private PrefabPool<ItemModifierEntry> _modifierPool;

		// Token: 0x040015DE RID: 5598
		private PrefabPool<ItemEffectEntry> _effectPool;
	}
}
