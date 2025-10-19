using System;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.PerkTrees
{
	// Token: 0x0200024B RID: 587
	public class AddPlayerStorage : PerkBehaviour
	{
		// Token: 0x17000342 RID: 834
		// (get) Token: 0x06001253 RID: 4691 RVA: 0x00045775 File Offset: 0x00043975
		private string DescriptionFormat
		{
			get
			{
				return "PerkBehaviour_AddPlayerStorage".ToPlainText();
			}
		}

		// Token: 0x17000343 RID: 835
		// (get) Token: 0x06001254 RID: 4692 RVA: 0x00045781 File Offset: 0x00043981
		public override string Description
		{
			get
			{
				return this.DescriptionFormat.Format(new { this.addCapacity });
			}
		}

		// Token: 0x06001255 RID: 4693 RVA: 0x00045799 File Offset: 0x00043999
		protected override void OnAwake()
		{
			PlayerStorage.OnRecalculateStorageCapacity += this.OnRecalculatePlayerStorage;
		}

		// Token: 0x06001256 RID: 4694 RVA: 0x000457AC File Offset: 0x000439AC
		protected override void OnOnDestroy()
		{
			PlayerStorage.OnRecalculateStorageCapacity -= this.OnRecalculatePlayerStorage;
		}

		// Token: 0x06001257 RID: 4695 RVA: 0x000457BF File Offset: 0x000439BF
		private void OnRecalculatePlayerStorage(PlayerStorage.StorageCapacityCalculationHolder holder)
		{
			if (base.Master.Unlocked)
			{
				holder.capacity += this.addCapacity;
			}
		}

		// Token: 0x06001258 RID: 4696 RVA: 0x000457E1 File Offset: 0x000439E1
		protected override void OnUnlocked()
		{
			base.OnUnlocked();
			PlayerStorage.NotifyCapacityDirty();
		}

		// Token: 0x04000E03 RID: 3587
		[SerializeField]
		private int addCapacity;
	}
}
