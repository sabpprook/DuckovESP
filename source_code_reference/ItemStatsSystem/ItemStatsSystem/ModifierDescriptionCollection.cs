using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemStatsSystem
{
	// Token: 0x02000020 RID: 32
	public class ModifierDescriptionCollection : ItemComponent, ICollection<ModifierDescription>, IEnumerable<ModifierDescription>, IEnumerable
	{
		// Token: 0x17000077 RID: 119
		// (get) Token: 0x060001B9 RID: 441 RVA: 0x00007524 File Offset: 0x00005724
		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x060001BB RID: 443 RVA: 0x00007540 File Offset: 0x00005740
		// (set) Token: 0x060001BA RID: 442 RVA: 0x00007531 File Offset: 0x00005731
		public bool ModifierEnable
		{
			get
			{
				return this._modifierEnableCache;
			}
			set
			{
				this._modifierEnableCache = value;
				this.ReapplyModifiers();
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x060001BC RID: 444 RVA: 0x00007548 File Offset: 0x00005748
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000754B File Offset: 0x0000574B
		internal override void OnInitialize()
		{
			base.Master.onItemTreeChanged += this.OnItemTreeChange;
			base.Master.onDurabilityChanged += this.OnDurabilityChange;
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000757B File Offset: 0x0000577B
		private void OnDurabilityChange(Item item)
		{
			this.ReapplyModifiers();
		}

		// Token: 0x060001BF RID: 447 RVA: 0x00007583 File Offset: 0x00005783
		private void OnDestroy()
		{
			if (base.Master)
			{
				base.Master.onItemTreeChanged -= this.OnItemTreeChange;
				base.Master.onDurabilityChanged -= this.OnDurabilityChange;
			}
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x000075C0 File Offset: 0x000057C0
		private void OnItemTreeChange(Item item)
		{
			this.ReapplyModifiers();
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x000075C8 File Offset: 0x000057C8
		public void ReapplyModifiers()
		{
			if (base.Master == null)
			{
				return;
			}
			bool flag = this.ModifierEnable;
			if (base.Master.UseDurability && base.Master.Durability <= 0f)
			{
				flag = false;
			}
			if (!flag)
			{
				foreach (ModifierDescription modifierDescription in this.list)
				{
					modifierDescription.Release();
				}
				return;
			}
			foreach (ModifierDescription modifierDescription2 in this.list)
			{
				modifierDescription2.ReapplyModifier(this);
			}
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x00007694 File Offset: 0x00005894
		public void Add(ModifierDescription item)
		{
			this.list.Add(item);
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x000076A4 File Offset: 0x000058A4
		public void Clear()
		{
			if (this.list == null)
			{
				this.list = new List<ModifierDescription>();
			}
			foreach (ModifierDescription modifierDescription in this.list)
			{
				modifierDescription.Release();
			}
			this.list.Clear();
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00007714 File Offset: 0x00005914
		public bool Contains(ModifierDescription item)
		{
			return this.list.Contains(item);
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x00007722 File Offset: 0x00005922
		public void CopyTo(ModifierDescription[] array, int arrayIndex)
		{
			this.list.CopyTo(array, arrayIndex);
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x00007731 File Offset: 0x00005931
		public bool Remove(ModifierDescription item)
		{
			if (item != null && this.list.Contains(item))
			{
				item.Release();
			}
			return this.list.Remove(item);
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x00007756 File Offset: 0x00005956
		public IEnumerator<ModifierDescription> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x00007768 File Offset: 0x00005968
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000777A File Offset: 0x0000597A
		public ModifierDescription Find(Predicate<ModifierDescription> predicate)
		{
			return this.list.Find(predicate);
		}

		// Token: 0x0400009B RID: 155
		private bool _modifierEnableCache = true;

		// Token: 0x0400009C RID: 156
		[SerializeField]
		private List<ModifierDescription> list;
	}
}
