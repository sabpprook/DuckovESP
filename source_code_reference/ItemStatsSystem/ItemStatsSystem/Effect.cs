using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Duckov.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ItemStatsSystem
{
	// Token: 0x02000012 RID: 18
	public class Effect : MonoBehaviour, ISelfValidator
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000077 RID: 119 RVA: 0x000033FF File Offset: 0x000015FF
		public Item Item
		{
			get
			{
				return this.item;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000078 RID: 120 RVA: 0x00003407 File Offset: 0x00001607
		public bool Display
		{
			get
			{
				return this.display;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000079 RID: 121 RVA: 0x0000340F File Offset: 0x0000160F
		public string Description
		{
			get
			{
				return this.description;
			}
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00003417 File Offset: 0x00001617
		public string GetDisplayString()
		{
			return this.Description;
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x0600007B RID: 123 RVA: 0x00003420 File Offset: 0x00001620
		// (remove) Token: 0x0600007C RID: 124 RVA: 0x00003458 File Offset: 0x00001658
		public event Action<Effect, Item> onSetTargetItem;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x0600007D RID: 125 RVA: 0x00003490 File Offset: 0x00001690
		// (remove) Token: 0x0600007E RID: 126 RVA: 0x000034C8 File Offset: 0x000016C8
		public event Action<Effect, Item> onItemTreeChanged;

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600007F RID: 127 RVA: 0x000034FD File Offset: 0x000016FD
		public ReadOnlyCollection<EffectTrigger> Triggers
		{
			get
			{
				if (this._Triggers == null)
				{
					this._Triggers = new ReadOnlyCollection<EffectTrigger>(this.triggers);
				}
				return this._Triggers;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000080 RID: 128 RVA: 0x0000351E File Offset: 0x0000171E
		public ReadOnlyCollection<EffectFilter> Filters
		{
			get
			{
				if (this._Filters == null)
				{
					this._Filters = new ReadOnlyCollection<EffectFilter>(this.filters);
				}
				return this._Filters;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000081 RID: 129 RVA: 0x0000353F File Offset: 0x0000173F
		public ReadOnlyCollection<EffectAction> Actions
		{
			get
			{
				if (this._Actions == null)
				{
					this._Actions = new ReadOnlyCollection<EffectAction>(this.actions);
				}
				return this._Actions;
			}
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00003560 File Offset: 0x00001760
		private bool EvaluateFilters(EffectTriggerEventContext context)
		{
			foreach (EffectFilter effectFilter in this.filters)
			{
				if (effectFilter.enabled && !effectFilter.Evaluate(context))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000035C4 File Offset: 0x000017C4
		internal void Trigger(EffectTriggerEventContext context)
		{
			if (!base.enabled)
			{
				return;
			}
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (!this.EvaluateFilters(context))
			{
				return;
			}
			foreach (EffectAction effectAction in this.actions)
			{
				effectAction.NotifyTriggered(context);
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00003638 File Offset: 0x00001838
		private void OnValidate()
		{
			if (this.item == null)
			{
				Transform parent = base.transform.parent;
				this.item = ((parent != null) ? parent.GetComponent<Item>() : null);
			}
			base.transform.hideFlags = HideFlags.HideInInspector;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00003674 File Offset: 0x00001874
		public void SetItem(Item targetItem)
		{
			this.UnregisterItemEvents();
			if (targetItem == null)
			{
				this.item = null;
				base.transform.SetParent(null);
			}
			this.item = targetItem;
			foreach (EffectTrigger effectTrigger in this.triggers)
			{
				effectTrigger.NotifySetItem(this, targetItem);
			}
			Action<Effect, Item> action = this.onSetTargetItem;
			if (action != null)
			{
				action(this, targetItem);
			}
			this.RegisterItemEvents();
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00003708 File Offset: 0x00001908
		private void RegisterItemEvents()
		{
			if (this.item == null)
			{
				return;
			}
			this.item.onItemTreeChanged += this.OnItemTreeChanged;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00003730 File Offset: 0x00001930
		private void UnregisterItemEvents()
		{
			if (this.item == null)
			{
				return;
			}
			this.item.onItemTreeChanged -= this.OnItemTreeChanged;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00003758 File Offset: 0x00001958
		private void OnItemTreeChanged(Item item)
		{
			Action<Effect, Item> action = this.onItemTreeChanged;
			if (action == null)
			{
				return;
			}
			action(this, this.item);
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00003774 File Offset: 0x00001974
		public void Validate(SelfValidationResult result)
		{
			Transform parent = base.transform.parent;
			Item item = ((parent != null) ? parent.GetComponent<Item>() : null);
			if (this.item != item)
			{
				result.AddError("Item 应为直接父物体").WithFix("将 Item 设为正确的值", delegate
				{
					Transform parent2 = base.transform.parent;
					this.item = ((parent2 != null) ? parent2.GetComponent<Item>() : null);
				}, true);
			}
			else if (this.item != null && !this.item.Effects.Contains(this))
			{
				result.AddError("Item中未引用本Effect").WithFix("在Item中加入本Effect。", delegate
				{
					this.item.Effects.Add(this);
				}, true);
			}
			bool flag = false;
			foreach (EffectTrigger effectTrigger in this.triggers)
			{
				if (!this.<Validate>g__ValidateComponent|35_13(effectTrigger))
				{
					flag = true;
				}
			}
			if (flag)
			{
				result.AddError("Trigger 列表中包含来自其他 Game Object 的 Trigger。").WithFix("移除来自其他 Game Object 的 Trigger。", delegate
				{
					this.triggers.RemoveAll((EffectTrigger e) => e.gameObject != base.gameObject);
				}, true);
			}
			flag = false;
			foreach (EffectFilter effectFilter in this.filters)
			{
				if (!this.<Validate>g__ValidateComponent|35_13(effectFilter))
				{
					flag = true;
				}
			}
			if (flag)
			{
				result.AddError("Filter 列表中包含来自其他 Game Object 的 Filter。").WithFix("移除来自其他 Game Object 的 Filter。", delegate
				{
					this.filters.RemoveAll((EffectFilter e) => e.gameObject != base.gameObject);
				}, true);
			}
			flag = false;
			foreach (EffectAction effectAction in this.actions)
			{
				if (!this.<Validate>g__ValidateComponent|35_13(effectAction))
				{
					flag = true;
				}
			}
			if (flag)
			{
				result.AddError("Trigger 列表中包含来自其他 Game Object 的 Trigger。").WithFix("移除来自其他 Game Object 的 Trigger。", delegate
				{
					this.actions.RemoveAll((EffectAction e) => e.gameObject != base.gameObject);
				}, true);
			}
			if (Effect.<Validate>g__AnyDuplicate|35_11<EffectTrigger>(this.triggers))
			{
				result.AddError("Trigger 列表中有重复的元素。").WithFix("移除重复元素。", delegate
				{
					this.triggers = new List<EffectTrigger>(this.triggers.Distinct<EffectTrigger>());
				}, true);
			}
			if (Effect.<Validate>g__AnyDuplicate|35_11<EffectFilter>(this.filters))
			{
				result.AddError("Filter 列表中有重复的元素。").WithFix("移除重复元素。", delegate
				{
					this.filters = new List<EffectFilter>(this.filters.Distinct<EffectFilter>());
				}, true);
			}
			if (Effect.<Validate>g__AnyDuplicate|35_11<EffectAction>(this.actions))
			{
				result.AddError("Trigger 列表中有重复的元素。").WithFix("移除重复元素。", delegate
				{
					this.actions = new List<EffectAction>(this.actions.Distinct<EffectAction>());
				}, true);
			}
			if (Effect.<Validate>g__AnyNull|35_12<EffectTrigger>(this.triggers))
			{
				result.AddError("Trigger 列表中有空元素。").WithFix("移除空元素。", delegate
				{
					this.triggers.RemoveAll((EffectTrigger e) => e == null);
				}, true);
			}
			if (Effect.<Validate>g__AnyNull|35_12<EffectFilter>(this.filters))
			{
				result.AddError("Filter 列表中有空元素。").WithFix("移除空元素。", delegate
				{
					this.filters.RemoveAll((EffectFilter e) => e == null);
				}, true);
			}
			if (Effect.<Validate>g__AnyNull|35_12<EffectAction>(this.actions))
			{
				result.AddError("Trigger 列表中有空元素。").WithFix("移除空元素。", delegate
				{
					this.actions.RemoveAll((EffectAction e) => e == null);
				}, true);
			}
			if (this.triggers.Count < 1)
			{
				result.AddWarning("没有配置任何触发器(Trigger)，将无法触发效果。");
			}
			if (this.actions.Count < 1)
			{
				result.AddWarning("没有配置任何动作(Action),该效果没有任何实际作用。");
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00003AB8 File Offset: 0x00001CB8
		internal void AddEffectComponent(EffectComponent effectComponent)
		{
			if (effectComponent is EffectTrigger)
			{
				this.triggers.Add(effectComponent as EffectTrigger);
				effectComponent.Master = this;
				return;
			}
			if (effectComponent is EffectFilter)
			{
				this.filters.Add(effectComponent as EffectFilter);
				effectComponent.Master = this;
				return;
			}
			if (effectComponent is EffectAction)
			{
				this.actions.Add(effectComponent as EffectAction);
				effectComponent.Master = this;
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00003B27 File Offset: 0x00001D27
		private void Awake()
		{
			this.RegisterItemEvents();
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600008C RID: 140 RVA: 0x00003B2F File Offset: 0x00001D2F
		private static Color TriggerColor
		{
			get
			{
				return DuckovUtilitiesSettings.Colors.EffectTrigger;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600008D RID: 141 RVA: 0x00003B3B File Offset: 0x00001D3B
		private static Color FilterColor
		{
			get
			{
				return DuckovUtilitiesSettings.Colors.EffectFilter;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600008E RID: 142 RVA: 0x00003B47 File Offset: 0x00001D47
		private static Color ActionColor
		{
			get
			{
				return DuckovUtilitiesSettings.Colors.EffectAction;
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00003D0F File Offset: 0x00001F0F
		[CompilerGenerated]
		internal static bool <Validate>g__AnyDuplicate|35_11<T>(List<T> list)
		{
			return (from e in list
				group e by e).Any((IGrouping<T, T> g) => g.Count<T>() > 1);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00003D3C File Offset: 0x00001F3C
		[CompilerGenerated]
		internal static bool <Validate>g__AnyNull|35_12<T>(List<T> list)
		{
			return list.Any((T e) => e == null);
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00003D54 File Offset: 0x00001F54
		[CompilerGenerated]
		private bool <Validate>g__ValidateComponent|35_13(EffectComponent component)
		{
			return component == null || !(component.gameObject != base.gameObject);
		}

		// Token: 0x0400002D RID: 45
		[SerializeField]
		private Item item;

		// Token: 0x0400002E RID: 46
		[SerializeField]
		private bool display;

		// Token: 0x0400002F RID: 47
		[SerializeField]
		private string description = "未定义描述";

		// Token: 0x04000032 RID: 50
		[SerializeField]
		internal List<EffectTrigger> triggers = new List<EffectTrigger>();

		// Token: 0x04000033 RID: 51
		[SerializeField]
		internal List<EffectFilter> filters = new List<EffectFilter>();

		// Token: 0x04000034 RID: 52
		[SerializeField]
		internal List<EffectAction> actions = new List<EffectAction>();

		// Token: 0x04000035 RID: 53
		private ReadOnlyCollection<EffectTrigger> _Triggers;

		// Token: 0x04000036 RID: 54
		private ReadOnlyCollection<EffectFilter> _Filters;

		// Token: 0x04000037 RID: 55
		private ReadOnlyCollection<EffectAction> _Actions;
	}
}
