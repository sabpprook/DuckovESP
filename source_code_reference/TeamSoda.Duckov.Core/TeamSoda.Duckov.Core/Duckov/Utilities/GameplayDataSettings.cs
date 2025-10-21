using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Duckov.Achievements;
using Duckov.Buffs;
using Duckov.Buildings;
using Duckov.Crops;
using Duckov.Quests;
using Duckov.Quests.Relations;
using Duckov.UI;
using Eflatun.SceneReference;
using ItemStatsSystem;
using LeTai.TrueShadow;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Duckov.Utilities
{
	// Token: 0x020003F4 RID: 1012
	[CreateAssetMenu(menuName = "Settings/Gameplay Data Settings")]
	public class GameplayDataSettings : ScriptableObject
	{
		// Token: 0x170006F5 RID: 1781
		// (get) Token: 0x06002490 RID: 9360 RVA: 0x0007EB1F File Offset: 0x0007CD1F
		private static GameplayDataSettings Default
		{
			get
			{
				if (GameplayDataSettings.cachedDefault == null)
				{
					GameplayDataSettings.cachedDefault = Resources.Load<GameplayDataSettings>("GameplayDataSettings");
				}
				return GameplayDataSettings.cachedDefault;
			}
		}

		// Token: 0x170006F6 RID: 1782
		// (get) Token: 0x06002491 RID: 9361 RVA: 0x0007EB42 File Offset: 0x0007CD42
		public static InputActionAsset InputActions
		{
			get
			{
				return GameplayDataSettings.Default.inputActions;
			}
		}

		// Token: 0x170006F7 RID: 1783
		// (get) Token: 0x06002492 RID: 9362 RVA: 0x0007EB4E File Offset: 0x0007CD4E
		public static CustomFaceData CustomFaceData
		{
			get
			{
				return GameplayDataSettings.Default.customFaceData;
			}
		}

		// Token: 0x170006F8 RID: 1784
		// (get) Token: 0x06002493 RID: 9363 RVA: 0x0007EB5A File Offset: 0x0007CD5A
		public static GameplayDataSettings.TagsData Tags
		{
			get
			{
				return GameplayDataSettings.Default.tags;
			}
		}

		// Token: 0x170006F9 RID: 1785
		// (get) Token: 0x06002494 RID: 9364 RVA: 0x0007EB66 File Offset: 0x0007CD66
		public static GameplayDataSettings.PrefabsData Prefabs
		{
			get
			{
				return GameplayDataSettings.Default.prefabs;
			}
		}

		// Token: 0x170006FA RID: 1786
		// (get) Token: 0x06002495 RID: 9365 RVA: 0x0007EB72 File Offset: 0x0007CD72
		public static UIPrefabsReference UIPrefabs
		{
			get
			{
				return GameplayDataSettings.Default.uiPrefabs;
			}
		}

		// Token: 0x170006FB RID: 1787
		// (get) Token: 0x06002496 RID: 9366 RVA: 0x0007EB7E File Offset: 0x0007CD7E
		public static GameplayDataSettings.ItemAssetsData ItemAssets
		{
			get
			{
				return GameplayDataSettings.Default.itemAssets;
			}
		}

		// Token: 0x170006FC RID: 1788
		// (get) Token: 0x06002497 RID: 9367 RVA: 0x0007EB8A File Offset: 0x0007CD8A
		public static GameplayDataSettings.StringListsData StringLists
		{
			get
			{
				return GameplayDataSettings.Default.stringLists;
			}
		}

		// Token: 0x170006FD RID: 1789
		// (get) Token: 0x06002498 RID: 9368 RVA: 0x0007EB96 File Offset: 0x0007CD96
		public static GameplayDataSettings.LayersData Layers
		{
			get
			{
				return GameplayDataSettings.Default.layers;
			}
		}

		// Token: 0x170006FE RID: 1790
		// (get) Token: 0x06002499 RID: 9369 RVA: 0x0007EBA2 File Offset: 0x0007CDA2
		public static GameplayDataSettings.SceneManagementData SceneManagement
		{
			get
			{
				return GameplayDataSettings.Default.sceneManagement;
			}
		}

		// Token: 0x170006FF RID: 1791
		// (get) Token: 0x0600249A RID: 9370 RVA: 0x0007EBAE File Offset: 0x0007CDAE
		public static GameplayDataSettings.BuffsData Buffs
		{
			get
			{
				return GameplayDataSettings.Default.buffs;
			}
		}

		// Token: 0x17000700 RID: 1792
		// (get) Token: 0x0600249B RID: 9371 RVA: 0x0007EBBA File Offset: 0x0007CDBA
		public static GameplayDataSettings.QuestsData Quests
		{
			get
			{
				return GameplayDataSettings.Default.quests;
			}
		}

		// Token: 0x17000701 RID: 1793
		// (get) Token: 0x0600249C RID: 9372 RVA: 0x0007EBC6 File Offset: 0x0007CDC6
		public static QuestCollection QuestCollection
		{
			get
			{
				return GameplayDataSettings.Default.quests.QuestCollection;
			}
		}

		// Token: 0x17000702 RID: 1794
		// (get) Token: 0x0600249D RID: 9373 RVA: 0x0007EBD7 File Offset: 0x0007CDD7
		public static QuestRelationGraph QuestRelation
		{
			get
			{
				return GameplayDataSettings.Default.quests.QuestRelation;
			}
		}

		// Token: 0x17000703 RID: 1795
		// (get) Token: 0x0600249E RID: 9374 RVA: 0x0007EBE8 File Offset: 0x0007CDE8
		public static GameplayDataSettings.EconomyData Economy
		{
			get
			{
				return GameplayDataSettings.Default.economyData;
			}
		}

		// Token: 0x17000704 RID: 1796
		// (get) Token: 0x0600249F RID: 9375 RVA: 0x0007EBF4 File Offset: 0x0007CDF4
		public static GameplayDataSettings.UIStyleData UIStyle
		{
			get
			{
				return GameplayDataSettings.Default.uiStyleData;
			}
		}

		// Token: 0x17000705 RID: 1797
		// (get) Token: 0x060024A0 RID: 9376 RVA: 0x0007EC00 File Offset: 0x0007CE00
		public static BuildingDataCollection BuildingDataCollection
		{
			get
			{
				return GameplayDataSettings.Default.buildingDataCollection;
			}
		}

		// Token: 0x17000706 RID: 1798
		// (get) Token: 0x060024A1 RID: 9377 RVA: 0x0007EC0C File Offset: 0x0007CE0C
		public static CraftingFormulaCollection CraftingFormulas
		{
			get
			{
				return GameplayDataSettings.Default.craftingFormulas;
			}
		}

		// Token: 0x17000707 RID: 1799
		// (get) Token: 0x060024A2 RID: 9378 RVA: 0x0007EC18 File Offset: 0x0007CE18
		public static DecomposeDatabase DecomposeDatabase
		{
			get
			{
				return GameplayDataSettings.Default.decomposeDatabase;
			}
		}

		// Token: 0x17000708 RID: 1800
		// (get) Token: 0x060024A3 RID: 9379 RVA: 0x0007EC24 File Offset: 0x0007CE24
		public static StatInfoDatabase StatInfo
		{
			get
			{
				return GameplayDataSettings.Default.statInfo;
			}
		}

		// Token: 0x17000709 RID: 1801
		// (get) Token: 0x060024A4 RID: 9380 RVA: 0x0007EC30 File Offset: 0x0007CE30
		public static StockShopDatabase StockshopDatabase
		{
			get
			{
				return GameplayDataSettings.Default.stockShopDatabase;
			}
		}

		// Token: 0x1700070A RID: 1802
		// (get) Token: 0x060024A5 RID: 9381 RVA: 0x0007EC3C File Offset: 0x0007CE3C
		public static GameplayDataSettings.LootingData Looting
		{
			get
			{
				return GameplayDataSettings.Default.looting;
			}
		}

		// Token: 0x1700070B RID: 1803
		// (get) Token: 0x060024A6 RID: 9382 RVA: 0x0007EC48 File Offset: 0x0007CE48
		public static AchievementDatabase AchievementDatabase
		{
			get
			{
				return GameplayDataSettings.Default.achivementDatabase;
			}
		}

		// Token: 0x1700070C RID: 1804
		// (get) Token: 0x060024A7 RID: 9383 RVA: 0x0007EC54 File Offset: 0x0007CE54
		public static CropDatabase CropDatabase
		{
			get
			{
				return GameplayDataSettings.Default.cropDatabase;
			}
		}

		// Token: 0x060024A8 RID: 9384 RVA: 0x0007EC60 File Offset: 0x0007CE60
		internal static Sprite GetSprite(string key)
		{
			return GameplayDataSettings.Default.spriteData.GetSprite(key);
		}

		// Token: 0x040018E3 RID: 6371
		private static GameplayDataSettings cachedDefault;

		// Token: 0x040018E4 RID: 6372
		[SerializeField]
		private GameplayDataSettings.TagsData tags;

		// Token: 0x040018E5 RID: 6373
		[SerializeField]
		private GameplayDataSettings.PrefabsData prefabs;

		// Token: 0x040018E6 RID: 6374
		[SerializeField]
		private UIPrefabsReference uiPrefabs;

		// Token: 0x040018E7 RID: 6375
		[SerializeField]
		private GameplayDataSettings.ItemAssetsData itemAssets;

		// Token: 0x040018E8 RID: 6376
		[SerializeField]
		private GameplayDataSettings.StringListsData stringLists;

		// Token: 0x040018E9 RID: 6377
		[SerializeField]
		private GameplayDataSettings.LayersData layers;

		// Token: 0x040018EA RID: 6378
		[SerializeField]
		private GameplayDataSettings.SceneManagementData sceneManagement;

		// Token: 0x040018EB RID: 6379
		[SerializeField]
		private GameplayDataSettings.BuffsData buffs;

		// Token: 0x040018EC RID: 6380
		[SerializeField]
		private GameplayDataSettings.QuestsData quests;

		// Token: 0x040018ED RID: 6381
		[SerializeField]
		private GameplayDataSettings.EconomyData economyData;

		// Token: 0x040018EE RID: 6382
		[SerializeField]
		private GameplayDataSettings.UIStyleData uiStyleData;

		// Token: 0x040018EF RID: 6383
		[SerializeField]
		private InputActionAsset inputActions;

		// Token: 0x040018F0 RID: 6384
		[SerializeField]
		private BuildingDataCollection buildingDataCollection;

		// Token: 0x040018F1 RID: 6385
		[SerializeField]
		private CustomFaceData customFaceData;

		// Token: 0x040018F2 RID: 6386
		[SerializeField]
		private CraftingFormulaCollection craftingFormulas;

		// Token: 0x040018F3 RID: 6387
		[SerializeField]
		private DecomposeDatabase decomposeDatabase;

		// Token: 0x040018F4 RID: 6388
		[SerializeField]
		private StatInfoDatabase statInfo;

		// Token: 0x040018F5 RID: 6389
		[SerializeField]
		private StockShopDatabase stockShopDatabase;

		// Token: 0x040018F6 RID: 6390
		[SerializeField]
		private GameplayDataSettings.LootingData looting;

		// Token: 0x040018F7 RID: 6391
		[SerializeField]
		private AchievementDatabase achivementDatabase;

		// Token: 0x040018F8 RID: 6392
		[SerializeField]
		private CropDatabase cropDatabase;

		// Token: 0x040018F9 RID: 6393
		[SerializeField]
		private GameplayDataSettings.SpritesData spriteData;

		// Token: 0x0200064E RID: 1614
		[Serializable]
		public class LootingData
		{
			// Token: 0x06002A0B RID: 10763 RVA: 0x0009F884 File Offset: 0x0009DA84
			public float MGetInspectingTime(Item item)
			{
				int num = item.Quality;
				if (num < 0)
				{
					num = 0;
				}
				if (num >= this.inspectingTimes.Length)
				{
					num = this.inspectingTimes.Length - 1;
				}
				return this.inspectingTimes[num];
			}

			// Token: 0x06002A0C RID: 10764 RVA: 0x0009F8BC File Offset: 0x0009DABC
			public static float GetInspectingTime(Item item)
			{
				GameplayDataSettings.LootingData looting = GameplayDataSettings.Looting;
				if (looting == null)
				{
					return 1f;
				}
				return looting.MGetInspectingTime(item);
			}

			// Token: 0x04002285 RID: 8837
			public float[] inspectingTimes;
		}

		// Token: 0x0200064F RID: 1615
		[Serializable]
		public class TagsData
		{
			// Token: 0x17000791 RID: 1937
			// (get) Token: 0x06002A0E RID: 10766 RVA: 0x0009F8E7 File Offset: 0x0009DAE7
			public Tag Character
			{
				get
				{
					return this.character;
				}
			}

			// Token: 0x17000792 RID: 1938
			// (get) Token: 0x06002A0F RID: 10767 RVA: 0x0009F8EF File Offset: 0x0009DAEF
			public Tag LockInDemoTag
			{
				get
				{
					return this.lockInDemoTag;
				}
			}

			// Token: 0x17000793 RID: 1939
			// (get) Token: 0x06002A10 RID: 10768 RVA: 0x0009F8F7 File Offset: 0x0009DAF7
			public Tag Helmat
			{
				get
				{
					return this.helmat;
				}
			}

			// Token: 0x17000794 RID: 1940
			// (get) Token: 0x06002A11 RID: 10769 RVA: 0x0009F8FF File Offset: 0x0009DAFF
			public Tag Armor
			{
				get
				{
					return this.armor;
				}
			}

			// Token: 0x17000795 RID: 1941
			// (get) Token: 0x06002A12 RID: 10770 RVA: 0x0009F907 File Offset: 0x0009DB07
			public Tag Backpack
			{
				get
				{
					return this.backpack;
				}
			}

			// Token: 0x17000796 RID: 1942
			// (get) Token: 0x06002A13 RID: 10771 RVA: 0x0009F90F File Offset: 0x0009DB0F
			public Tag Bullet
			{
				get
				{
					return this.bullet;
				}
			}

			// Token: 0x17000797 RID: 1943
			// (get) Token: 0x06002A14 RID: 10772 RVA: 0x0009F917 File Offset: 0x0009DB17
			public Tag Bait
			{
				get
				{
					return this.bait;
				}
			}

			// Token: 0x17000798 RID: 1944
			// (get) Token: 0x06002A15 RID: 10773 RVA: 0x0009F91F File Offset: 0x0009DB1F
			public Tag AdvancedDebuffMode
			{
				get
				{
					return this.advancedDebuffMode;
				}
			}

			// Token: 0x17000799 RID: 1945
			// (get) Token: 0x06002A16 RID: 10774 RVA: 0x0009F927 File Offset: 0x0009DB27
			public Tag Special
			{
				get
				{
					return this.special;
				}
			}

			// Token: 0x1700079A RID: 1946
			// (get) Token: 0x06002A17 RID: 10775 RVA: 0x0009F92F File Offset: 0x0009DB2F
			public Tag DestroyOnLootBox
			{
				get
				{
					return this.destroyOnLootBox;
				}
			}

			// Token: 0x1700079B RID: 1947
			// (get) Token: 0x06002A18 RID: 10776 RVA: 0x0009F937 File Offset: 0x0009DB37
			public Tag DontDropOnDeadInSlot
			{
				get
				{
					return this.dontDropOnDeadInSlot;
				}
			}

			// Token: 0x1700079C RID: 1948
			// (get) Token: 0x06002A19 RID: 10777 RVA: 0x0009F93F File Offset: 0x0009DB3F
			public ReadOnlyCollection<Tag> AllTags
			{
				get
				{
					if (this.tagsReadOnly == null)
					{
						this.tagsReadOnly = this.allTags.AsReadOnly();
					}
					return this.tagsReadOnly;
				}
			}

			// Token: 0x1700079D RID: 1949
			// (get) Token: 0x06002A1A RID: 10778 RVA: 0x0009F960 File Offset: 0x0009DB60
			public Tag Gun
			{
				get
				{
					if (this.gun == null)
					{
						this.gun = this.Get("Gun");
					}
					return this.gun;
				}
			}

			// Token: 0x06002A1B RID: 10779 RVA: 0x0009F988 File Offset: 0x0009DB88
			internal Tag Get(string name)
			{
				foreach (Tag tag in this.AllTags)
				{
					if (tag.name == name)
					{
						return tag;
					}
				}
				return null;
			}

			// Token: 0x04002286 RID: 8838
			[SerializeField]
			private Tag character;

			// Token: 0x04002287 RID: 8839
			[SerializeField]
			private Tag lockInDemoTag;

			// Token: 0x04002288 RID: 8840
			[SerializeField]
			private Tag helmat;

			// Token: 0x04002289 RID: 8841
			[SerializeField]
			private Tag armor;

			// Token: 0x0400228A RID: 8842
			[SerializeField]
			private Tag backpack;

			// Token: 0x0400228B RID: 8843
			[SerializeField]
			private Tag bullet;

			// Token: 0x0400228C RID: 8844
			[SerializeField]
			private Tag bait;

			// Token: 0x0400228D RID: 8845
			[SerializeField]
			private Tag advancedDebuffMode;

			// Token: 0x0400228E RID: 8846
			[SerializeField]
			private Tag special;

			// Token: 0x0400228F RID: 8847
			[SerializeField]
			private Tag destroyOnLootBox;

			// Token: 0x04002290 RID: 8848
			[FormerlySerializedAs("dontDropOnDead")]
			[SerializeField]
			private Tag dontDropOnDeadInSlot;

			// Token: 0x04002291 RID: 8849
			[SerializeField]
			private List<Tag> allTags = new List<Tag>();

			// Token: 0x04002292 RID: 8850
			private ReadOnlyCollection<Tag> tagsReadOnly;

			// Token: 0x04002293 RID: 8851
			private Tag gun;
		}

		// Token: 0x02000650 RID: 1616
		[Serializable]
		public class PrefabsData
		{
			// Token: 0x1700079E RID: 1950
			// (get) Token: 0x06002A1D RID: 10781 RVA: 0x0009F9F7 File Offset: 0x0009DBF7
			public LevelManager LevelManagerPrefab
			{
				get
				{
					return this.levelManagerPrefab;
				}
			}

			// Token: 0x1700079F RID: 1951
			// (get) Token: 0x06002A1E RID: 10782 RVA: 0x0009F9FF File Offset: 0x0009DBFF
			public CharacterMainControl CharacterPrefab
			{
				get
				{
					return this.characterPrefab;
				}
			}

			// Token: 0x170007A0 RID: 1952
			// (get) Token: 0x06002A1F RID: 10783 RVA: 0x0009FA07 File Offset: 0x0009DC07
			public GameObject BulletHitObsticleFx
			{
				get
				{
					return this.bulletHitObsticleFx;
				}
			}

			// Token: 0x170007A1 RID: 1953
			// (get) Token: 0x06002A20 RID: 10784 RVA: 0x0009FA0F File Offset: 0x0009DC0F
			public GameObject QuestMarker
			{
				get
				{
					return this.questMarker;
				}
			}

			// Token: 0x170007A2 RID: 1954
			// (get) Token: 0x06002A21 RID: 10785 RVA: 0x0009FA17 File Offset: 0x0009DC17
			public DuckovItemAgent PickupAgentPrefab
			{
				get
				{
					return this.pickupAgentPrefab;
				}
			}

			// Token: 0x170007A3 RID: 1955
			// (get) Token: 0x06002A22 RID: 10786 RVA: 0x0009FA1F File Offset: 0x0009DC1F
			public DuckovItemAgent PickupAgentNoRendererPrefab
			{
				get
				{
					return this.pickupAgentNoRendererPrefab;
				}
			}

			// Token: 0x170007A4 RID: 1956
			// (get) Token: 0x06002A23 RID: 10787 RVA: 0x0009FA27 File Offset: 0x0009DC27
			public DuckovItemAgent HandheldAgentPrefab
			{
				get
				{
					return this.handheldAgentPrefab;
				}
			}

			// Token: 0x170007A5 RID: 1957
			// (get) Token: 0x06002A24 RID: 10788 RVA: 0x0009FA2F File Offset: 0x0009DC2F
			public InteractableLootbox LootBoxPrefab
			{
				get
				{
					return this.lootBoxPrefab;
				}
			}

			// Token: 0x170007A6 RID: 1958
			// (get) Token: 0x06002A25 RID: 10789 RVA: 0x0009FA37 File Offset: 0x0009DC37
			public InteractableLootbox LootBoxPrefab_Tomb
			{
				get
				{
					return this.lootBoxPrefab_Tomb;
				}
			}

			// Token: 0x170007A7 RID: 1959
			// (get) Token: 0x06002A26 RID: 10790 RVA: 0x0009FA3F File Offset: 0x0009DC3F
			public InteractMarker InteractMarker
			{
				get
				{
					return this.interactMarker;
				}
			}

			// Token: 0x170007A8 RID: 1960
			// (get) Token: 0x06002A27 RID: 10791 RVA: 0x0009FA47 File Offset: 0x0009DC47
			public HeadCollider HeadCollider
			{
				get
				{
					return this.headCollider;
				}
			}

			// Token: 0x170007A9 RID: 1961
			// (get) Token: 0x06002A28 RID: 10792 RVA: 0x0009FA4F File Offset: 0x0009DC4F
			public Projectile DefaultBullet
			{
				get
				{
					return this.defaultBullet;
				}
			}

			// Token: 0x170007AA RID: 1962
			// (get) Token: 0x06002A29 RID: 10793 RVA: 0x0009FA57 File Offset: 0x0009DC57
			public GameObject BuildingBlockAreaMesh
			{
				get
				{
					return this.buildingBlockAreaMesh;
				}
			}

			// Token: 0x170007AB RID: 1963
			// (get) Token: 0x06002A2A RID: 10794 RVA: 0x0009FA5F File Offset: 0x0009DC5F
			public GameObject AlertFxPrefab
			{
				get
				{
					return this.alertFxPrefab;
				}
			}

			// Token: 0x170007AC RID: 1964
			// (get) Token: 0x06002A2B RID: 10795 RVA: 0x0009FA67 File Offset: 0x0009DC67
			public UIInputManager UIInputManagerPrefab
			{
				get
				{
					return this.uiInputManagerPrefab;
				}
			}

			// Token: 0x04002294 RID: 8852
			[SerializeField]
			private LevelManager levelManagerPrefab;

			// Token: 0x04002295 RID: 8853
			[SerializeField]
			private CharacterMainControl characterPrefab;

			// Token: 0x04002296 RID: 8854
			[SerializeField]
			private GameObject bulletHitObsticleFx;

			// Token: 0x04002297 RID: 8855
			[SerializeField]
			private GameObject questMarker;

			// Token: 0x04002298 RID: 8856
			[SerializeField]
			private DuckovItemAgent pickupAgentPrefab;

			// Token: 0x04002299 RID: 8857
			[SerializeField]
			private DuckovItemAgent pickupAgentNoRendererPrefab;

			// Token: 0x0400229A RID: 8858
			[SerializeField]
			private DuckovItemAgent handheldAgentPrefab;

			// Token: 0x0400229B RID: 8859
			[SerializeField]
			private InteractableLootbox lootBoxPrefab;

			// Token: 0x0400229C RID: 8860
			[SerializeField]
			private InteractableLootbox lootBoxPrefab_Tomb;

			// Token: 0x0400229D RID: 8861
			[SerializeField]
			private InteractMarker interactMarker;

			// Token: 0x0400229E RID: 8862
			[SerializeField]
			private HeadCollider headCollider;

			// Token: 0x0400229F RID: 8863
			[SerializeField]
			private Projectile defaultBullet;

			// Token: 0x040022A0 RID: 8864
			[SerializeField]
			private UIInputManager uiInputManagerPrefab;

			// Token: 0x040022A1 RID: 8865
			[SerializeField]
			private GameObject buildingBlockAreaMesh;

			// Token: 0x040022A2 RID: 8866
			[SerializeField]
			private GameObject alertFxPrefab;
		}

		// Token: 0x02000651 RID: 1617
		[Serializable]
		public class BuffsData
		{
			// Token: 0x170007AD RID: 1965
			// (get) Token: 0x06002A2D RID: 10797 RVA: 0x0009FA77 File Offset: 0x0009DC77
			public Buff BleedSBuff
			{
				get
				{
					return this.bleedSBuff;
				}
			}

			// Token: 0x170007AE RID: 1966
			// (get) Token: 0x06002A2E RID: 10798 RVA: 0x0009FA7F File Offset: 0x0009DC7F
			public Buff UnlimitBleedBuff
			{
				get
				{
					return this.unlimitBleedBuff;
				}
			}

			// Token: 0x170007AF RID: 1967
			// (get) Token: 0x06002A2F RID: 10799 RVA: 0x0009FA87 File Offset: 0x0009DC87
			public Buff BoneCrackBuff
			{
				get
				{
					return this.boneCrackBuff;
				}
			}

			// Token: 0x170007B0 RID: 1968
			// (get) Token: 0x06002A30 RID: 10800 RVA: 0x0009FA8F File Offset: 0x0009DC8F
			public Buff WoundBuff
			{
				get
				{
					return this.woundBuff;
				}
			}

			// Token: 0x170007B1 RID: 1969
			// (get) Token: 0x06002A31 RID: 10801 RVA: 0x0009FA97 File Offset: 0x0009DC97
			public Buff Weight_Light
			{
				get
				{
					return this.weight_Light;
				}
			}

			// Token: 0x170007B2 RID: 1970
			// (get) Token: 0x06002A32 RID: 10802 RVA: 0x0009FA9F File Offset: 0x0009DC9F
			public Buff Weight_Heavy
			{
				get
				{
					return this.weight_Heavy;
				}
			}

			// Token: 0x170007B3 RID: 1971
			// (get) Token: 0x06002A33 RID: 10803 RVA: 0x0009FAA7 File Offset: 0x0009DCA7
			public Buff Weight_SuperHeavy
			{
				get
				{
					return this.weight_SuperHeavy;
				}
			}

			// Token: 0x170007B4 RID: 1972
			// (get) Token: 0x06002A34 RID: 10804 RVA: 0x0009FAAF File Offset: 0x0009DCAF
			public Buff Weight_Overweight
			{
				get
				{
					return this.weight_Overweight;
				}
			}

			// Token: 0x170007B5 RID: 1973
			// (get) Token: 0x06002A35 RID: 10805 RVA: 0x0009FAB7 File Offset: 0x0009DCB7
			public Buff Pain
			{
				get
				{
					return this.pain;
				}
			}

			// Token: 0x170007B6 RID: 1974
			// (get) Token: 0x06002A36 RID: 10806 RVA: 0x0009FABF File Offset: 0x0009DCBF
			public Buff BaseBuff
			{
				get
				{
					return this.baseBuff;
				}
			}

			// Token: 0x170007B7 RID: 1975
			// (get) Token: 0x06002A37 RID: 10807 RVA: 0x0009FAC7 File Offset: 0x0009DCC7
			public Buff Starve
			{
				get
				{
					return this.starve;
				}
			}

			// Token: 0x170007B8 RID: 1976
			// (get) Token: 0x06002A38 RID: 10808 RVA: 0x0009FACF File Offset: 0x0009DCCF
			public Buff Thirsty
			{
				get
				{
					return this.thirsty;
				}
			}

			// Token: 0x170007B9 RID: 1977
			// (get) Token: 0x06002A39 RID: 10809 RVA: 0x0009FAD7 File Offset: 0x0009DCD7
			public Buff Burn
			{
				get
				{
					return this.burn;
				}
			}

			// Token: 0x170007BA RID: 1978
			// (get) Token: 0x06002A3A RID: 10810 RVA: 0x0009FADF File Offset: 0x0009DCDF
			public Buff Poison
			{
				get
				{
					return this.poison;
				}
			}

			// Token: 0x170007BB RID: 1979
			// (get) Token: 0x06002A3B RID: 10811 RVA: 0x0009FAE7 File Offset: 0x0009DCE7
			public Buff Electric
			{
				get
				{
					return this.electric;
				}
			}

			// Token: 0x170007BC RID: 1980
			// (get) Token: 0x06002A3C RID: 10812 RVA: 0x0009FAEF File Offset: 0x0009DCEF
			public Buff Space
			{
				get
				{
					return this.space;
				}
			}

			// Token: 0x06002A3D RID: 10813 RVA: 0x0009FAF8 File Offset: 0x0009DCF8
			public string GetBuffDisplayName(int id)
			{
				Buff buff = this.allBuffs.Find((Buff e) => e != null && e.ID == id);
				if (buff == null)
				{
					return "?";
				}
				return buff.DisplayName;
			}

			// Token: 0x040022A3 RID: 8867
			[SerializeField]
			private Buff bleedSBuff;

			// Token: 0x040022A4 RID: 8868
			[SerializeField]
			private Buff unlimitBleedBuff;

			// Token: 0x040022A5 RID: 8869
			[SerializeField]
			private Buff boneCrackBuff;

			// Token: 0x040022A6 RID: 8870
			[SerializeField]
			private Buff woundBuff;

			// Token: 0x040022A7 RID: 8871
			[SerializeField]
			private Buff weight_Light;

			// Token: 0x040022A8 RID: 8872
			[SerializeField]
			private Buff weight_Heavy;

			// Token: 0x040022A9 RID: 8873
			[SerializeField]
			private Buff weight_SuperHeavy;

			// Token: 0x040022AA RID: 8874
			[SerializeField]
			private Buff weight_Overweight;

			// Token: 0x040022AB RID: 8875
			[SerializeField]
			private Buff pain;

			// Token: 0x040022AC RID: 8876
			[SerializeField]
			private Buff baseBuff;

			// Token: 0x040022AD RID: 8877
			[SerializeField]
			private Buff starve;

			// Token: 0x040022AE RID: 8878
			[SerializeField]
			private Buff thirsty;

			// Token: 0x040022AF RID: 8879
			[SerializeField]
			private Buff burn;

			// Token: 0x040022B0 RID: 8880
			[SerializeField]
			private Buff poison;

			// Token: 0x040022B1 RID: 8881
			[SerializeField]
			private Buff electric;

			// Token: 0x040022B2 RID: 8882
			[SerializeField]
			private Buff space;

			// Token: 0x040022B3 RID: 8883
			[SerializeField]
			private List<Buff> allBuffs;
		}

		// Token: 0x02000652 RID: 1618
		[Serializable]
		public class ItemAssetsData
		{
			// Token: 0x170007BD RID: 1981
			// (get) Token: 0x06002A3F RID: 10815 RVA: 0x0009FB47 File Offset: 0x0009DD47
			public int DefaultCharacterItemTypeID
			{
				get
				{
					return this.defaultCharacterItemTypeID;
				}
			}

			// Token: 0x170007BE RID: 1982
			// (get) Token: 0x06002A40 RID: 10816 RVA: 0x0009FB4F File Offset: 0x0009DD4F
			public int CashItemTypeID
			{
				get
				{
					return this.cashItemTypeID;
				}
			}

			// Token: 0x040022B4 RID: 8884
			[SerializeField]
			[ItemTypeID]
			private int defaultCharacterItemTypeID;

			// Token: 0x040022B5 RID: 8885
			[SerializeField]
			[ItemTypeID]
			private int cashItemTypeID;
		}

		// Token: 0x02000653 RID: 1619
		public class StringListsData
		{
			// Token: 0x040022B6 RID: 8886
			public static StringList StatKeys;

			// Token: 0x040022B7 RID: 8887
			public static StringList SlotTypes;

			// Token: 0x040022B8 RID: 8888
			public static StringList ItemAgentKeys;
		}

		// Token: 0x02000654 RID: 1620
		[Serializable]
		public class LayersData
		{
			// Token: 0x06002A43 RID: 10819 RVA: 0x0009FB67 File Offset: 0x0009DD67
			public static bool IsLayerInLayerMask(int layer, LayerMask layerMask)
			{
				return ((1 << layer) & layerMask) != 0;
			}

			// Token: 0x040022B9 RID: 8889
			public LayerMask damageReceiverLayerMask;

			// Token: 0x040022BA RID: 8890
			public LayerMask wallLayerMask;

			// Token: 0x040022BB RID: 8891
			public LayerMask groundLayerMask;

			// Token: 0x040022BC RID: 8892
			public LayerMask halfObsticleLayer;

			// Token: 0x040022BD RID: 8893
			public LayerMask fowBlockLayers;

			// Token: 0x040022BE RID: 8894
			public LayerMask fowBlockLayersWithThermal;
		}

		// Token: 0x02000655 RID: 1621
		[Serializable]
		public class SceneManagementData
		{
			// Token: 0x170007BF RID: 1983
			// (get) Token: 0x06002A45 RID: 10821 RVA: 0x0009FB83 File Offset: 0x0009DD83
			public SceneInfoCollection SceneInfoCollection
			{
				get
				{
					return this.sceneInfoCollection;
				}
			}

			// Token: 0x170007C0 RID: 1984
			// (get) Token: 0x06002A46 RID: 10822 RVA: 0x0009FB8B File Offset: 0x0009DD8B
			public SceneReference PrologueScene
			{
				get
				{
					return this.prologueScene;
				}
			}

			// Token: 0x170007C1 RID: 1985
			// (get) Token: 0x06002A47 RID: 10823 RVA: 0x0009FB93 File Offset: 0x0009DD93
			public SceneReference MainMenuScene
			{
				get
				{
					return this.mainMenuScene;
				}
			}

			// Token: 0x170007C2 RID: 1986
			// (get) Token: 0x06002A48 RID: 10824 RVA: 0x0009FB9B File Offset: 0x0009DD9B
			public SceneReference BaseScene
			{
				get
				{
					return this.baseScene;
				}
			}

			// Token: 0x170007C3 RID: 1987
			// (get) Token: 0x06002A49 RID: 10825 RVA: 0x0009FBA3 File Offset: 0x0009DDA3
			public SceneReference FailLoadingScreenScene
			{
				get
				{
					return this.failLoadingScreenScene;
				}
			}

			// Token: 0x170007C4 RID: 1988
			// (get) Token: 0x06002A4A RID: 10826 RVA: 0x0009FBAB File Offset: 0x0009DDAB
			public SceneReference EvacuateScreenScene
			{
				get
				{
					return this.evacuateScreenScene;
				}
			}

			// Token: 0x040022BF RID: 8895
			[SerializeField]
			private SceneInfoCollection sceneInfoCollection;

			// Token: 0x040022C0 RID: 8896
			[SerializeField]
			private SceneReference prologueScene;

			// Token: 0x040022C1 RID: 8897
			[SerializeField]
			private SceneReference mainMenuScene;

			// Token: 0x040022C2 RID: 8898
			[SerializeField]
			private SceneReference baseScene;

			// Token: 0x040022C3 RID: 8899
			[SerializeField]
			private SceneReference failLoadingScreenScene;

			// Token: 0x040022C4 RID: 8900
			[SerializeField]
			private SceneReference evacuateScreenScene;
		}

		// Token: 0x02000656 RID: 1622
		[Serializable]
		public class QuestsData
		{
			// Token: 0x170007C5 RID: 1989
			// (get) Token: 0x06002A4C RID: 10828 RVA: 0x0009FBBB File Offset: 0x0009DDBB
			private string DefaultQuestGiverDisplayName
			{
				get
				{
					return this.defaultQuestGiverDisplayName;
				}
			}

			// Token: 0x170007C6 RID: 1990
			// (get) Token: 0x06002A4D RID: 10829 RVA: 0x0009FBC3 File Offset: 0x0009DDC3
			public QuestCollection QuestCollection
			{
				get
				{
					return this.questCollection;
				}
			}

			// Token: 0x170007C7 RID: 1991
			// (get) Token: 0x06002A4E RID: 10830 RVA: 0x0009FBCB File Offset: 0x0009DDCB
			public QuestRelationGraph QuestRelation
			{
				get
				{
					return this.questRelation;
				}
			}

			// Token: 0x06002A4F RID: 10831 RVA: 0x0009FBD4 File Offset: 0x0009DDD4
			public GameplayDataSettings.QuestsData.QuestGiverInfo GetInfo(QuestGiverID id)
			{
				return this.questGiverInfos.Find((GameplayDataSettings.QuestsData.QuestGiverInfo e) => e != null && e.id == id);
			}

			// Token: 0x06002A50 RID: 10832 RVA: 0x0009FC08 File Offset: 0x0009DE08
			public string GetDisplayName(QuestGiverID id)
			{
				return string.Format("Character_{0}", id).ToPlainText();
			}

			// Token: 0x040022C5 RID: 8901
			[SerializeField]
			private QuestCollection questCollection;

			// Token: 0x040022C6 RID: 8902
			[SerializeField]
			private QuestRelationGraph questRelation;

			// Token: 0x040022C7 RID: 8903
			[SerializeField]
			private List<GameplayDataSettings.QuestsData.QuestGiverInfo> questGiverInfos;

			// Token: 0x040022C8 RID: 8904
			[SerializeField]
			private string defaultQuestGiverDisplayName = "佚名";

			// Token: 0x02000673 RID: 1651
			[Serializable]
			public class QuestGiverInfo
			{
				// Token: 0x170007D4 RID: 2004
				// (get) Token: 0x06002A88 RID: 10888 RVA: 0x000A0BF9 File Offset: 0x0009EDF9
				public string DisplayName
				{
					get
					{
						return this.displayName;
					}
				}

				// Token: 0x04002323 RID: 8995
				public QuestGiverID id;

				// Token: 0x04002324 RID: 8996
				[SerializeField]
				private string displayName;
			}
		}

		// Token: 0x02000657 RID: 1623
		[Serializable]
		public class EconomyData
		{
			// Token: 0x170007C8 RID: 1992
			// (get) Token: 0x06002A52 RID: 10834 RVA: 0x0009FC3D File Offset: 0x0009DE3D
			public ReadOnlyCollection<int> UnlockedItemByDefault
			{
				get
				{
					return this.unlockItemByDefault.AsReadOnly();
				}
			}

			// Token: 0x040022C9 RID: 8905
			[SerializeField]
			[ItemTypeID]
			private List<int> unlockItemByDefault = new List<int>();
		}

		// Token: 0x02000658 RID: 1624
		[Serializable]
		public class UIStyleData
		{
			// Token: 0x170007C9 RID: 1993
			// (get) Token: 0x06002A54 RID: 10836 RVA: 0x0009FC5D File Offset: 0x0009DE5D
			public Sprite CritPopSprite
			{
				get
				{
					return this.critPopSprite;
				}
			}

			// Token: 0x170007CA RID: 1994
			// (get) Token: 0x06002A55 RID: 10837 RVA: 0x0009FC65 File Offset: 0x0009DE65
			public Sprite DefaultTeleporterIcon
			{
				get
				{
					return this.defaultTeleporterIcon;
				}
			}

			// Token: 0x170007CB RID: 1995
			// (get) Token: 0x06002A56 RID: 10838 RVA: 0x0009FC6D File Offset: 0x0009DE6D
			public Sprite EleteCharacterIcon
			{
				get
				{
					return this.eleteCharacterIcon;
				}
			}

			// Token: 0x170007CC RID: 1996
			// (get) Token: 0x06002A57 RID: 10839 RVA: 0x0009FC75 File Offset: 0x0009DE75
			public Sprite BossCharacterIcon
			{
				get
				{
					return this.bossCharacterIcon;
				}
			}

			// Token: 0x170007CD RID: 1997
			// (get) Token: 0x06002A58 RID: 10840 RVA: 0x0009FC7D File Offset: 0x0009DE7D
			public Sprite PmcCharacterIcon
			{
				get
				{
					return this.pmcCharacterIcon;
				}
			}

			// Token: 0x170007CE RID: 1998
			// (get) Token: 0x06002A59 RID: 10841 RVA: 0x0009FC85 File Offset: 0x0009DE85
			public Sprite MerchantCharacterIcon
			{
				get
				{
					return this.merchantCharacterIcon;
				}
			}

			// Token: 0x170007CF RID: 1999
			// (get) Token: 0x06002A5A RID: 10842 RVA: 0x0009FC8D File Offset: 0x0009DE8D
			public Sprite PetCharacterIcon
			{
				get
				{
					return this.petCharacterIcon;
				}
			}

			// Token: 0x170007D0 RID: 2000
			// (get) Token: 0x06002A5B RID: 10843 RVA: 0x0009FC95 File Offset: 0x0009DE95
			public float TeleporterIconScale
			{
				get
				{
					return this.teleporterIconScale;
				}
			}

			// Token: 0x170007D1 RID: 2001
			// (get) Token: 0x06002A5C RID: 10844 RVA: 0x0009FC9D File Offset: 0x0009DE9D
			public Sprite FallbackItemIcon
			{
				get
				{
					return this.fallbackItemIcon;
				}
			}

			// Token: 0x170007D2 RID: 2002
			// (get) Token: 0x06002A5D RID: 10845 RVA: 0x0009FCA5 File Offset: 0x0009DEA5
			public TextMeshProUGUI TemplateTextUGUI
			{
				get
				{
					return this.templateTextUGUI;
				}
			}

			// Token: 0x170007D3 RID: 2003
			// (get) Token: 0x06002A5E RID: 10846 RVA: 0x0009FCAD File Offset: 0x0009DEAD
			[SerializeField]
			private TMP_Asset DefaultFont
			{
				get
				{
					return this.defaultFont;
				}
			}

			// Token: 0x06002A5F RID: 10847 RVA: 0x0009FCB8 File Offset: 0x0009DEB8
			[return: TupleElementNames(new string[] { "shadowOffset", "color", "innerGlow" })]
			public ValueTuple<float, Color, bool> GetShadowOffsetAndColorOfQuality(DisplayQuality displayQuality)
			{
				GameplayDataSettings.UIStyleData.DisplayQualityLook displayQualityLook = this.displayQualityLooks.Find((GameplayDataSettings.UIStyleData.DisplayQualityLook e) => e != null && e.quality == displayQuality);
				if (displayQualityLook == null)
				{
					return new ValueTuple<float, Color, bool>(this.defaultDisplayQualityShadowOffset, this.defaultDisplayQualityShadowColor, this.defaultDIsplayQualityShadowInnerGlow);
				}
				return new ValueTuple<float, Color, bool>(displayQualityLook.shadowOffset, displayQualityLook.shadowColor, displayQualityLook.innerGlow);
			}

			// Token: 0x06002A60 RID: 10848 RVA: 0x0009FD1C File Offset: 0x0009DF1C
			public void ApplyDisplayQualityShadow(DisplayQuality displayQuality, TrueShadow target)
			{
				ValueTuple<float, Color, bool> shadowOffsetAndColorOfQuality = this.GetShadowOffsetAndColorOfQuality(displayQuality);
				target.OffsetDistance = shadowOffsetAndColorOfQuality.Item1;
				target.Color = shadowOffsetAndColorOfQuality.Item2;
				target.Inset = shadowOffsetAndColorOfQuality.Item3;
			}

			// Token: 0x06002A61 RID: 10849 RVA: 0x0009FD58 File Offset: 0x0009DF58
			public GameplayDataSettings.UIStyleData.DisplayQualityLook GetDisplayQualityLook(DisplayQuality q)
			{
				GameplayDataSettings.UIStyleData.DisplayQualityLook displayQualityLook = this.displayQualityLooks.Find((GameplayDataSettings.UIStyleData.DisplayQualityLook e) => e != null && e.quality == q);
				if (displayQualityLook == null)
				{
					return new GameplayDataSettings.UIStyleData.DisplayQualityLook
					{
						quality = q,
						shadowOffset = this.defaultDisplayQualityShadowOffset,
						shadowColor = this.defaultDisplayQualityShadowColor,
						innerGlow = this.defaultDIsplayQualityShadowInnerGlow
					};
				}
				return displayQualityLook;
			}

			// Token: 0x06002A62 RID: 10850 RVA: 0x0009FDC4 File Offset: 0x0009DFC4
			public GameplayDataSettings.UIStyleData.DisplayElementDamagePopTextLook GetElementDamagePopTextLook(ElementTypes elementType)
			{
				GameplayDataSettings.UIStyleData.DisplayElementDamagePopTextLook displayElementDamagePopTextLook = this.elementDamagePopTextLook.Find((GameplayDataSettings.UIStyleData.DisplayElementDamagePopTextLook e) => e != null && e.elementType == elementType);
				if (displayElementDamagePopTextLook == null)
				{
					return new GameplayDataSettings.UIStyleData.DisplayElementDamagePopTextLook
					{
						elementType = ElementTypes.physics,
						normalSize = 1f,
						critSize = 1.6f,
						color = Color.white
					};
				}
				return displayElementDamagePopTextLook;
			}

			// Token: 0x040022CA RID: 8906
			[SerializeField]
			private List<GameplayDataSettings.UIStyleData.DisplayQualityLook> displayQualityLooks = new List<GameplayDataSettings.UIStyleData.DisplayQualityLook>();

			// Token: 0x040022CB RID: 8907
			[SerializeField]
			private List<GameplayDataSettings.UIStyleData.DisplayElementDamagePopTextLook> elementDamagePopTextLook = new List<GameplayDataSettings.UIStyleData.DisplayElementDamagePopTextLook>();

			// Token: 0x040022CC RID: 8908
			[SerializeField]
			private float defaultDisplayQualityShadowOffset = 8f;

			// Token: 0x040022CD RID: 8909
			[SerializeField]
			private Color defaultDisplayQualityShadowColor = Color.black;

			// Token: 0x040022CE RID: 8910
			[SerializeField]
			private bool defaultDIsplayQualityShadowInnerGlow;

			// Token: 0x040022CF RID: 8911
			[SerializeField]
			private Sprite defaultTeleporterIcon;

			// Token: 0x040022D0 RID: 8912
			[SerializeField]
			private float teleporterIconScale = 0.5f;

			// Token: 0x040022D1 RID: 8913
			[SerializeField]
			private Sprite critPopSprite;

			// Token: 0x040022D2 RID: 8914
			[SerializeField]
			private Sprite fallbackItemIcon;

			// Token: 0x040022D3 RID: 8915
			[SerializeField]
			private Sprite eleteCharacterIcon;

			// Token: 0x040022D4 RID: 8916
			[SerializeField]
			private Sprite bossCharacterIcon;

			// Token: 0x040022D5 RID: 8917
			[SerializeField]
			private Sprite pmcCharacterIcon;

			// Token: 0x040022D6 RID: 8918
			[SerializeField]
			private Sprite merchantCharacterIcon;

			// Token: 0x040022D7 RID: 8919
			[SerializeField]
			private Sprite petCharacterIcon;

			// Token: 0x040022D8 RID: 8920
			[SerializeField]
			private TMP_Asset defaultFont;

			// Token: 0x040022D9 RID: 8921
			[SerializeField]
			private TextMeshProUGUI templateTextUGUI;

			// Token: 0x02000675 RID: 1653
			[Serializable]
			public class DisplayQualityLook
			{
				// Token: 0x06002A8C RID: 10892 RVA: 0x000A0C26 File Offset: 0x0009EE26
				public void Apply(TrueShadow trueShadow)
				{
					trueShadow.OffsetDistance = this.shadowOffset;
					trueShadow.Color = this.shadowColor;
					trueShadow.Inset = this.innerGlow;
				}

				// Token: 0x04002326 RID: 8998
				public DisplayQuality quality;

				// Token: 0x04002327 RID: 8999
				public float shadowOffset;

				// Token: 0x04002328 RID: 9000
				public Color shadowColor;

				// Token: 0x04002329 RID: 9001
				public bool innerGlow;
			}

			// Token: 0x02000676 RID: 1654
			[Serializable]
			public class DisplayElementDamagePopTextLook
			{
				// Token: 0x0400232A RID: 9002
				public ElementTypes elementType;

				// Token: 0x0400232B RID: 9003
				public float normalSize;

				// Token: 0x0400232C RID: 9004
				public float critSize;

				// Token: 0x0400232D RID: 9005
				public Color color;
			}
		}

		// Token: 0x02000659 RID: 1625
		[Serializable]
		public class SpritesData
		{
			// Token: 0x06002A64 RID: 10852 RVA: 0x0009FE68 File Offset: 0x0009E068
			public Sprite GetSprite(string key)
			{
				foreach (GameplayDataSettings.SpritesData.Entry entry in this.entries)
				{
					if (entry.key == key)
					{
						return entry.sprite;
					}
				}
				return null;
			}

			// Token: 0x040022DA RID: 8922
			public List<GameplayDataSettings.SpritesData.Entry> entries;

			// Token: 0x0200067A RID: 1658
			[Serializable]
			public struct Entry
			{
				// Token: 0x04002331 RID: 9009
				public string key;

				// Token: 0x04002332 RID: 9010
				public Sprite sprite;
			}
		}
	}
}
