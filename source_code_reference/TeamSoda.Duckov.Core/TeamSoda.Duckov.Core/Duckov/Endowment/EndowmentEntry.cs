using System;
using System.Text;
using ItemStatsSystem;
using ItemStatsSystem.Stats;
using SodaCraft.Localizations;
using UnityEngine;
using UnityEngine.Events;

namespace Duckov.Endowment
{
	// Token: 0x020002F0 RID: 752
	public class EndowmentEntry : MonoBehaviour
	{
		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x0600185C RID: 6236 RVA: 0x00058FCF File Offset: 0x000571CF
		public EndowmentIndex Index
		{
			get
			{
				return this.index;
			}
		}

		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x0600185D RID: 6237 RVA: 0x00058FD7 File Offset: 0x000571D7
		// (set) Token: 0x0600185E RID: 6238 RVA: 0x00058FEE File Offset: 0x000571EE
		[LocalizationKey("Default")]
		private string displayNameKey
		{
			get
			{
				return string.Format("Endowmment_{0}", this.index);
			}
			set
			{
			}
		}

		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x0600185F RID: 6239 RVA: 0x00058FF0 File Offset: 0x000571F0
		// (set) Token: 0x06001860 RID: 6240 RVA: 0x00059007 File Offset: 0x00057207
		[LocalizationKey("Default")]
		private string descriptionKey
		{
			get
			{
				return string.Format("Endowmment_{0}_Desc", this.index);
			}
			set
			{
			}
		}

		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x06001861 RID: 6241 RVA: 0x00059009 File Offset: 0x00057209
		public string RequirementText
		{
			get
			{
				return this.requirementTextKey.ToPlainText();
			}
		}

		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x06001862 RID: 6242 RVA: 0x00059016 File Offset: 0x00057216
		public Sprite Icon
		{
			get
			{
				return this.icon;
			}
		}

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x06001863 RID: 6243 RVA: 0x0005901E File Offset: 0x0005721E
		public string DisplayName
		{
			get
			{
				return this.displayNameKey.ToPlainText();
			}
		}

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x06001864 RID: 6244 RVA: 0x0005902B File Offset: 0x0005722B
		public string Description
		{
			get
			{
				return this.descriptionKey.ToPlainText();
			}
		}

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x06001865 RID: 6245 RVA: 0x00059038 File Offset: 0x00057238
		public string DescriptionAndEffects
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				string description = this.Description;
				stringBuilder.AppendLine(description);
				foreach (EndowmentEntry.ModifierDescription modifierDescription in this.Modifiers)
				{
					stringBuilder.AppendLine("- " + modifierDescription.DescriptionText);
				}
				return stringBuilder.ToString();
			}
		}

		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x06001866 RID: 6246 RVA: 0x00059096 File Offset: 0x00057296
		public EndowmentEntry.ModifierDescription[] Modifiers
		{
			get
			{
				return this.modifiers;
			}
		}

		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x06001867 RID: 6247 RVA: 0x0005909E File Offset: 0x0005729E
		private Item CharacterItem
		{
			get
			{
				if (CharacterMainControl.Main == null)
				{
					return null;
				}
				return CharacterMainControl.Main.CharacterItem;
			}
		}

		// Token: 0x1700046D RID: 1133
		// (get) Token: 0x06001868 RID: 6248 RVA: 0x000590B9 File Offset: 0x000572B9
		public bool UnlockedByDefault
		{
			get
			{
				return this.unlockedByDefault;
			}
		}

		// Token: 0x06001869 RID: 6249 RVA: 0x000590C1 File Offset: 0x000572C1
		public void Activate()
		{
			this.ApplyModifiers();
			UnityEvent<EndowmentEntry> unityEvent = this.onActivate;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}

		// Token: 0x0600186A RID: 6250 RVA: 0x000590DA File Offset: 0x000572DA
		public void Deactivate()
		{
			this.DeleteModifiers();
			UnityEvent<EndowmentEntry> unityEvent = this.onDeactivate;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}

		// Token: 0x0600186B RID: 6251 RVA: 0x000590F4 File Offset: 0x000572F4
		private void ApplyModifiers()
		{
			if (this.CharacterItem == null)
			{
				return;
			}
			this.DeleteModifiers();
			foreach (EndowmentEntry.ModifierDescription modifierDescription in this.modifiers)
			{
				this.CharacterItem.AddModifier(modifierDescription.statKey, new Modifier(modifierDescription.type, modifierDescription.value, this));
			}
		}

		// Token: 0x0600186C RID: 6252 RVA: 0x00059157 File Offset: 0x00057357
		private void DeleteModifiers()
		{
			if (this.CharacterItem == null)
			{
				return;
			}
			this.CharacterItem.RemoveAllModifiersFrom(this);
		}

		// Token: 0x040011C9 RID: 4553
		[SerializeField]
		private EndowmentIndex index;

		// Token: 0x040011CA RID: 4554
		[SerializeField]
		private Sprite icon;

		// Token: 0x040011CB RID: 4555
		[SerializeField]
		[LocalizationKey("Default")]
		private string requirementTextKey;

		// Token: 0x040011CC RID: 4556
		[SerializeField]
		private bool unlockedByDefault;

		// Token: 0x040011CD RID: 4557
		[SerializeField]
		private EndowmentEntry.ModifierDescription[] modifiers;

		// Token: 0x040011CE RID: 4558
		public UnityEvent<EndowmentEntry> onActivate;

		// Token: 0x040011CF RID: 4559
		public UnityEvent<EndowmentEntry> onDeactivate;

		// Token: 0x02000582 RID: 1410
		[Serializable]
		public struct ModifierDescription
		{
			// Token: 0x17000764 RID: 1892
			// (get) Token: 0x06002842 RID: 10306 RVA: 0x000947D4 File Offset: 0x000929D4
			// (set) Token: 0x06002843 RID: 10307 RVA: 0x000947E6 File Offset: 0x000929E6
			[LocalizationKey("Default")]
			private string DisplayNameKey
			{
				get
				{
					return "Stat_" + this.statKey;
				}
				set
				{
				}
			}

			// Token: 0x17000765 RID: 1893
			// (get) Token: 0x06002844 RID: 10308 RVA: 0x000947E8 File Offset: 0x000929E8
			public string DescriptionText
			{
				get
				{
					string text = this.DisplayNameKey.ToPlainText();
					string text2 = "";
					ModifierType modifierType = this.type;
					if (modifierType != ModifierType.Add)
					{
						if (modifierType != ModifierType.PercentageAdd)
						{
							if (modifierType == ModifierType.PercentageMultiply)
							{
								text2 = string.Format("x{0:00.#}%", (1f + this.value) * 100f);
							}
						}
						else if (this.value >= 0f)
						{
							text2 = string.Format("+{0:00.#}%", this.value * 100f);
						}
						else
						{
							text2 = string.Format("-{0:00.#}%", -this.value * 100f);
						}
					}
					else if (this.value >= 0f)
					{
						text2 = string.Format("+{0}", this.value);
					}
					else
					{
						text2 = string.Format("{0}", this.value);
					}
					return text + " " + text2;
				}
			}

			// Token: 0x04001FA8 RID: 8104
			public string statKey;

			// Token: 0x04001FA9 RID: 8105
			public ModifierType type;

			// Token: 0x04001FAA RID: 8106
			public float value;
		}
	}
}
