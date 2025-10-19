using System;
using System.Collections.Generic;
using ItemStatsSystem;
using ItemStatsSystem.Stats;
using UnityEngine;

namespace Duckov.Buildings
{
	// Token: 0x02000312 RID: 786
	public class BuildingEffect : MonoBehaviour
	{
		// Token: 0x060019EB RID: 6635 RVA: 0x0005D85E File Offset: 0x0005BA5E
		private void Awake()
		{
			BuildingManager.OnBuildingListChanged += this.OnBuildingStatusChanged;
			LevelManager.OnLevelInitialized += this.OnLevelInitialized;
		}

		// Token: 0x060019EC RID: 6636 RVA: 0x0005D882 File Offset: 0x0005BA82
		private void OnDestroy()
		{
			this.DisableEffects();
			BuildingManager.OnBuildingListChanged -= this.OnBuildingStatusChanged;
			LevelManager.OnLevelInitialized -= this.OnLevelInitialized;
		}

		// Token: 0x060019ED RID: 6637 RVA: 0x0005D8AC File Offset: 0x0005BAAC
		private void OnLevelInitialized()
		{
			this.Refresh();
		}

		// Token: 0x060019EE RID: 6638 RVA: 0x0005D8B4 File Offset: 0x0005BAB4
		private void Start()
		{
			this.Refresh();
		}

		// Token: 0x060019EF RID: 6639 RVA: 0x0005D8BC File Offset: 0x0005BABC
		private void OnBuildingStatusChanged()
		{
			this.Refresh();
		}

		// Token: 0x060019F0 RID: 6640 RVA: 0x0005D8C4 File Offset: 0x0005BAC4
		private void Refresh()
		{
			this.DisableEffects();
			if (this.IsBuildingConstructed())
			{
				this.EnableEffects();
			}
		}

		// Token: 0x060019F1 RID: 6641 RVA: 0x0005D8DA File Offset: 0x0005BADA
		private bool IsBuildingConstructed()
		{
			return BuildingManager.Any(this.buildingID, false);
		}

		// Token: 0x060019F2 RID: 6642 RVA: 0x0005D8E8 File Offset: 0x0005BAE8
		private void DisableEffects()
		{
			foreach (Modifier modifier in this.modifiers)
			{
				if (modifier != null)
				{
					modifier.RemoveFromTarget();
				}
			}
			this.modifiers.Clear();
		}

		// Token: 0x060019F3 RID: 6643 RVA: 0x0005D948 File Offset: 0x0005BB48
		private void EnableEffects()
		{
			this.DisableEffects();
			if (CharacterMainControl.Main == null)
			{
				return;
			}
			foreach (BuildingEffect.ModifierDescription modifierDescription in this.modifierDescriptions)
			{
				this.Apply(modifierDescription);
			}
		}

		// Token: 0x060019F4 RID: 6644 RVA: 0x0005D9B0 File Offset: 0x0005BBB0
		private void Apply(BuildingEffect.ModifierDescription description)
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
				stat = ((characterItem != null) ? characterItem.GetStat(description.stat) : null);
			}
			Stat stat2 = stat;
			if (stat2 == null)
			{
				return;
			}
			Modifier modifier = new Modifier(description.type, description.value, this);
			stat2.AddModifier(modifier);
			this.modifiers.Add(modifier);
		}

		// Token: 0x040012AA RID: 4778
		[SerializeField]
		private string buildingID;

		// Token: 0x040012AB RID: 4779
		[SerializeField]
		private List<BuildingEffect.ModifierDescription> modifierDescriptions = new List<BuildingEffect.ModifierDescription>();

		// Token: 0x040012AC RID: 4780
		private List<Modifier> modifiers = new List<Modifier>();

		// Token: 0x020005A2 RID: 1442
		[Serializable]
		public struct ModifierDescription
		{
			// Token: 0x04002020 RID: 8224
			public string stat;

			// Token: 0x04002021 RID: 8225
			public ModifierType type;

			// Token: 0x04002022 RID: 8226
			public float value;
		}
	}
}
