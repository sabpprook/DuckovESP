using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Saves;
using SodaCraft.Localizations;
using UnityEngine;

namespace Duckov.PerkTrees
{
	// Token: 0x0200024D RID: 589
	public class PerkTree : MonoBehaviour, ISaveDataProvider
	{
		// Token: 0x17000345 RID: 837
		// (get) Token: 0x06001260 RID: 4704 RVA: 0x00045841 File Offset: 0x00043A41
		// (set) Token: 0x0600125F RID: 4703 RVA: 0x0004583F File Offset: 0x00043A3F
		[LocalizationKey("Perks")]
		private string perkTreeName
		{
			get
			{
				return this.displayNameKey;
			}
			set
			{
			}
		}

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x06001261 RID: 4705 RVA: 0x00045849 File Offset: 0x00043A49
		public string ID
		{
			get
			{
				return this.perkTreeID;
			}
		}

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x06001262 RID: 4706 RVA: 0x00045851 File Offset: 0x00043A51
		private string displayNameKey
		{
			get
			{
				return "PerkTree_" + this.ID;
			}
		}

		// Token: 0x17000348 RID: 840
		// (get) Token: 0x06001263 RID: 4707 RVA: 0x00045863 File Offset: 0x00043A63
		public string DisplayName
		{
			get
			{
				return this.displayNameKey.ToPlainText();
			}
		}

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x06001264 RID: 4708 RVA: 0x00045870 File Offset: 0x00043A70
		public bool Horizontal
		{
			get
			{
				return this.horizontal;
			}
		}

		// Token: 0x14000078 RID: 120
		// (add) Token: 0x06001265 RID: 4709 RVA: 0x00045878 File Offset: 0x00043A78
		// (remove) Token: 0x06001266 RID: 4710 RVA: 0x000458B0 File Offset: 0x00043AB0
		public event Action<PerkTree> onPerkTreeStatusChanged;

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x06001267 RID: 4711 RVA: 0x000458E5 File Offset: 0x00043AE5
		public ReadOnlyCollection<Perk> Perks
		{
			get
			{
				if (this.perks_ReadOnly == null)
				{
					this.perks_ReadOnly = this.perks.AsReadOnly();
				}
				return this.perks_ReadOnly;
			}
		}

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x06001268 RID: 4712 RVA: 0x00045906 File Offset: 0x00043B06
		public PerkTreeRelationGraphOwner RelationGraphOwner
		{
			get
			{
				return this.relationGraphOwner;
			}
		}

		// Token: 0x06001269 RID: 4713 RVA: 0x0004590E File Offset: 0x00043B0E
		private void Awake()
		{
			this.Load();
			SavesSystem.OnCollectSaveData += this.Save;
			SavesSystem.OnSetFile += this.Load;
		}

		// Token: 0x0600126A RID: 4714 RVA: 0x00045938 File Offset: 0x00043B38
		private void Start()
		{
			foreach (Perk perk in this.perks)
			{
				if (!(perk == null) && perk.DefaultUnlocked)
				{
					perk.ForceUnlock();
				}
			}
		}

		// Token: 0x0600126B RID: 4715 RVA: 0x0004599C File Offset: 0x00043B9C
		private void OnDestroy()
		{
			SavesSystem.OnCollectSaveData -= this.Save;
			SavesSystem.OnSetFile -= this.Load;
		}

		// Token: 0x0600126C RID: 4716 RVA: 0x000459C0 File Offset: 0x00043BC0
		public object GenerateSaveData()
		{
			return new PerkTree.SaveData(this);
		}

		// Token: 0x0600126D RID: 4717 RVA: 0x000459C8 File Offset: 0x00043BC8
		public void SetupSaveData(object data)
		{
			foreach (Perk perk in this.perks)
			{
				perk.Unlocked = false;
			}
			PerkTree.SaveData saveData = data as PerkTree.SaveData;
			if (saveData == null)
			{
				return;
			}
			using (List<Perk>.Enumerator enumerator = this.perks.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Perk cur = enumerator.Current;
					if (!(cur == null))
					{
						PerkTree.SaveData.Entry entry = saveData.entries.Find((PerkTree.SaveData.Entry e) => e != null && e.perkName == cur.name);
						if (entry != null)
						{
							cur.Unlocked = entry.unlocked;
							cur.unlocking = entry.unlocking;
							cur.unlockingBeginTimeRaw = entry.unlockingBeginTime;
						}
					}
				}
			}
		}

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x0600126E RID: 4718 RVA: 0x00045AC8 File Offset: 0x00043CC8
		private string SaveKey
		{
			get
			{
				return "PerkTree_" + this.perkTreeID;
			}
		}

		// Token: 0x0600126F RID: 4719 RVA: 0x00045ADA File Offset: 0x00043CDA
		public void Save()
		{
			SavesSystem.Save<PerkTree.SaveData>(this.SaveKey, this.GenerateSaveData() as PerkTree.SaveData);
		}

		// Token: 0x06001270 RID: 4720 RVA: 0x00045AF4 File Offset: 0x00043CF4
		public void Load()
		{
			if (!SavesSystem.KeyExisits(this.SaveKey))
			{
				return;
			}
			PerkTree.SaveData saveData = SavesSystem.Load<PerkTree.SaveData>(this.SaveKey);
			this.SetupSaveData(saveData);
			this.loaded = true;
		}

		// Token: 0x06001271 RID: 4721 RVA: 0x00045B2C File Offset: 0x00043D2C
		public void ReapplyPerks()
		{
			foreach (Perk perk in this.perks)
			{
				perk.Unlocked = false;
			}
			foreach (Perk perk2 in this.perks)
			{
				perk2.Unlocked = perk2.Unlocked;
			}
		}

		// Token: 0x06001272 RID: 4722 RVA: 0x00045BC4 File Offset: 0x00043DC4
		internal bool AreAllParentsUnlocked(Perk perk)
		{
			PerkRelationNode relatedNode = this.RelationGraphOwner.GetRelatedNode(perk);
			if (relatedNode == null)
			{
				return false;
			}
			foreach (PerkRelationNode perkRelationNode in this.relationGraphOwner.RelationGraph.GetIncomingNodes(relatedNode))
			{
				Perk relatedNode2 = perkRelationNode.relatedNode;
				if (!(relatedNode2 == null) && !relatedNode2.Unlocked)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06001273 RID: 4723 RVA: 0x00045C4C File Offset: 0x00043E4C
		internal void NotifyChildStateChanged(Perk perk)
		{
			PerkRelationNode relatedNode = this.RelationGraphOwner.GetRelatedNode(perk);
			if (relatedNode == null)
			{
				return;
			}
			foreach (PerkRelationNode perkRelationNode in this.relationGraphOwner.RelationGraph.GetOutgoingNodes(relatedNode))
			{
				perkRelationNode.NotifyIncomingStateChanged();
			}
			Action<PerkTree> action = this.onPerkTreeStatusChanged;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06001274 RID: 4724 RVA: 0x00045CCC File Offset: 0x00043ECC
		private void Collect()
		{
			this.perks.Clear();
			Perk[] componentsInChildren = base.transform.GetComponentsInChildren<Perk>();
			Perk[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Master = this;
			}
			this.perks.AddRange(componentsInChildren);
		}

		// Token: 0x04000E06 RID: 3590
		[SerializeField]
		private string perkTreeID = "DefaultPerkTree";

		// Token: 0x04000E07 RID: 3591
		[SerializeField]
		private bool horizontal;

		// Token: 0x04000E08 RID: 3592
		[SerializeField]
		private PerkTreeRelationGraphOwner relationGraphOwner;

		// Token: 0x04000E09 RID: 3593
		[SerializeField]
		internal List<Perk> perks = new List<Perk>();

		// Token: 0x04000E0B RID: 3595
		private ReadOnlyCollection<Perk> perks_ReadOnly;

		// Token: 0x04000E0C RID: 3596
		private bool loaded;

		// Token: 0x02000530 RID: 1328
		[Serializable]
		private class SaveData
		{
			// Token: 0x06002798 RID: 10136 RVA: 0x00090ED0 File Offset: 0x0008F0D0
			public SaveData(PerkTree perkTree)
			{
				this.entries = new List<PerkTree.SaveData.Entry>();
				for (int i = 0; i < perkTree.perks.Count; i++)
				{
					Perk perk = perkTree.perks[i];
					if (!(perk == null))
					{
						this.entries.Add(new PerkTree.SaveData.Entry(perk));
					}
				}
			}

			// Token: 0x04001E67 RID: 7783
			public List<PerkTree.SaveData.Entry> entries;

			// Token: 0x0200066F RID: 1647
			[Serializable]
			public class Entry
			{
				// Token: 0x06002A82 RID: 10882 RVA: 0x000A0B79 File Offset: 0x0009ED79
				public Entry(Perk perk)
				{
					this.perkName = perk.name;
					this.unlocked = perk.Unlocked;
					this.unlocking = perk.Unlocking;
					this.unlockingBeginTime = perk.unlockingBeginTimeRaw;
				}

				// Token: 0x0400231B RID: 8987
				public string perkName;

				// Token: 0x0400231C RID: 8988
				public bool unlocking;

				// Token: 0x0400231D RID: 8989
				public long unlockingBeginTime;

				// Token: 0x0400231E RID: 8990
				public bool unlocked;
			}
		}
	}
}
