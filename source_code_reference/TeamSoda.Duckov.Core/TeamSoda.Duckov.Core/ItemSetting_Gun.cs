using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Duckov.Buffs;
using Duckov.Utilities;
using ItemStatsSystem;
using SodaCraft.Localizations;
using UnityEngine;

// Token: 0x020000F0 RID: 240
public class ItemSetting_Gun : ItemSettingBase
{
	// Token: 0x170001A3 RID: 419
	// (get) Token: 0x060007D8 RID: 2008 RVA: 0x000230B1 File Offset: 0x000212B1
	public int TargetBulletID
	{
		get
		{
			return this.targetBulletID;
		}
	}

	// Token: 0x170001A4 RID: 420
	// (get) Token: 0x060007D9 RID: 2009 RVA: 0x000230BC File Offset: 0x000212BC
	public string CurrentBulletName
	{
		get
		{
			if (this.TargetBulletID < 0)
			{
				return "UI_Bullet_NotAssigned".ToPlainText();
			}
			return ItemAssetsCollection.GetMetaData(this.TargetBulletID).DisplayName;
		}
	}

	// Token: 0x170001A5 RID: 421
	// (get) Token: 0x060007DA RID: 2010 RVA: 0x000230F0 File Offset: 0x000212F0
	public int BulletCount
	{
		get
		{
			if (this.loadingBullets)
			{
				return -1;
			}
			if (this.bulletCount < 0)
			{
				this.bulletCount = this.GetBulletCount();
			}
			return this.bulletCount;
		}
	}

	// Token: 0x170001A6 RID: 422
	// (get) Token: 0x060007DC RID: 2012 RVA: 0x0002313C File Offset: 0x0002133C
	// (set) Token: 0x060007DB RID: 2011 RVA: 0x00023117 File Offset: 0x00021317
	private int bulletCount
	{
		get
		{
			return this._bulletCountCache;
		}
		set
		{
			this._bulletCountCache = value;
			base.Item.Variables.SetInt(this.bulletCountHash, this._bulletCountCache);
		}
	}

	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x060007DD RID: 2013 RVA: 0x00023144 File Offset: 0x00021344
	public int Capacity
	{
		get
		{
			return Mathf.RoundToInt(base.Item.GetStatValue(ItemSetting_Gun.CapacityHash));
		}
	}

	// Token: 0x170001A8 RID: 424
	// (get) Token: 0x060007DE RID: 2014 RVA: 0x0002315B File Offset: 0x0002135B
	public bool LoadingBullets
	{
		get
		{
			return this.loadingBullets;
		}
	}

	// Token: 0x170001A9 RID: 425
	// (get) Token: 0x060007DF RID: 2015 RVA: 0x00023163 File Offset: 0x00021363
	public bool LoadBulletsSuccess
	{
		get
		{
			return this.loadBulletsSuccess;
		}
	}

	// Token: 0x170001AA RID: 426
	// (get) Token: 0x060007E1 RID: 2017 RVA: 0x00023174 File Offset: 0x00021374
	// (set) Token: 0x060007E0 RID: 2016 RVA: 0x0002316B File Offset: 0x0002136B
	public Item PreferdBulletsToLoad
	{
		get
		{
			return this.preferedBulletsToLoad;
		}
		set
		{
			this.preferedBulletsToLoad = value;
		}
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x0002317C File Offset: 0x0002137C
	public void SetTargetBulletType(Item bulletItem)
	{
		if (bulletItem != null)
		{
			this.SetTargetBulletType(bulletItem.TypeID);
			return;
		}
		this.SetTargetBulletType(-1);
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x0002319C File Offset: 0x0002139C
	public void SetTargetBulletType(int typeID)
	{
		bool flag = false;
		if (this.TargetBulletID != typeID && this.TargetBulletID != -1)
		{
			flag = true;
		}
		this.targetBulletID = typeID;
		if (flag)
		{
			this.TakeOutAllBullets();
		}
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x000231CF File Offset: 0x000213CF
	public override void Start()
	{
		base.Start();
		this.AutoSetTypeInInventory(null);
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x000231E0 File Offset: 0x000213E0
	public void UseABullet()
	{
		if (LevelManager.Instance.IsBaseLevel)
		{
			return;
		}
		foreach (Item item in base.Item.Inventory)
		{
			if (!(item == null) && item.StackCount >= 1)
			{
				item.StackCount--;
				break;
			}
		}
		this.bulletCount--;
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x00023268 File Offset: 0x00021468
	public bool IsFull()
	{
		return this.bulletCount >= this.Capacity;
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x0002327C File Offset: 0x0002147C
	public bool IsValidBullet(Item newBulletItem)
	{
		if (newBulletItem == null)
		{
			return false;
		}
		if (!newBulletItem.Tags.Contains(GameplayDataSettings.Tags.Bullet))
		{
			return false;
		}
		Item currentLoadedBullet = this.GetCurrentLoadedBullet();
		if (currentLoadedBullet != null && currentLoadedBullet.TypeID == newBulletItem.TypeID && this.bulletCount >= this.Capacity)
		{
			return false;
		}
		string @string = newBulletItem.Constants.GetString(this.caliberHash, null);
		string string2 = base.Item.Constants.GetString(this.caliberHash, null);
		return !(@string != string2);
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x00023310 File Offset: 0x00021510
	public bool LoadSpecificBullet(Item newBulletItem)
	{
		Debug.Log("尝试安装指定弹药");
		if (!this.IsValidBullet(newBulletItem))
		{
			return false;
		}
		Debug.Log("指定弹药判定通过");
		ItemAgent_Gun itemAgent_Gun = base.Item.ActiveAgent as ItemAgent_Gun;
		if (!(itemAgent_Gun != null))
		{
			Inventory inventory = base.Item.InInventory;
			if (inventory != null && inventory != CharacterMainControl.Main.CharacterItem.Inventory)
			{
				inventory = null;
			}
			this.preferedBulletsToLoad = newBulletItem;
			this.LoadBulletsFromInventory(inventory).Forget();
			return true;
		}
		if (itemAgent_Gun.Holder != null)
		{
			bool flag = itemAgent_Gun.CharacterReload(newBulletItem);
			Debug.Log(string.Format("角色reload:{0}", flag));
			return true;
		}
		return false;
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x000233CC File Offset: 0x000215CC
	public async UniTaskVoid LoadBulletsFromInventory(Inventory inventory)
	{
		if (!this.loadingBullets)
		{
			this.loadingBullets = true;
			this.loadBulletsSuccess = false;
			Item item = this.preferedBulletsToLoad;
			this.preferedBulletsToLoad = null;
			if (item != null)
			{
				this.SetTargetBulletType(item);
			}
			this.bulletCount = this.GetBulletCount();
			if (this.bulletCount > 0)
			{
				Item currentLoadedBullet = this.GetCurrentLoadedBullet();
				if (currentLoadedBullet && currentLoadedBullet.TypeID != this.TargetBulletID)
				{
					this.TakeOutAllBullets();
				}
			}
			int capacity = this.Capacity;
			int needCount = capacity - this.bulletCount;
			if (needCount < 0)
			{
				this.loadBulletsSuccess = false;
				this.loadingBullets = false;
			}
			else
			{
				if (this.reloadMode == ItemSetting_Gun.ReloadModes.singleBullet)
				{
					needCount = 1;
				}
				List<Item> bullets = new List<Item>();
				if (item != null)
				{
					if (item.StackCount > needCount)
					{
						Item item2 = await item.Split(needCount);
						bullets.Add(item2);
						needCount = 0;
					}
					else
					{
						item.Detach();
						bullets.Add(item);
						needCount -= item.StackCount;
					}
				}
				if (needCount > 0 && inventory != null)
				{
					CharacterMainControl characterMainControl = base.Item.GetCharacterMainControl();
					if (characterMainControl == LevelManager.Instance.MainCharacter)
					{
						bullets.AddRange(await inventory.GetItemsOfAmount(this.targetBulletID, needCount));
					}
					else if (characterMainControl != null)
					{
						Item item3 = await ItemAssetsCollection.InstantiateAsync(this.targetBulletID);
						item3.StackCount = needCount;
						bullets.Add(item3);
					}
				}
				if (bullets.Count <= 0)
				{
					this.loadBulletsSuccess = false;
					this.loadingBullets = false;
				}
				else
				{
					foreach (Item item4 in bullets)
					{
						if (item4 == null)
						{
							this.loadBulletsSuccess = false;
						}
						else
						{
							item4.Inspected = true;
							base.Item.Inventory.AddAndMerge(item4, 0);
						}
					}
					this.bulletCount = this.GetBulletCount();
					this.loadBulletsSuccess = true;
					this.loadingBullets = false;
				}
			}
		}
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x00023418 File Offset: 0x00021618
	public bool AutoSetTypeInInventory(Inventory inventory)
	{
		string @string = base.Item.Constants.GetString(this.caliberHash, null);
		Item currentLoadedBullet = this.GetCurrentLoadedBullet();
		if (currentLoadedBullet != null)
		{
			this.SetTargetBulletType(currentLoadedBullet);
			return false;
		}
		if (inventory == null)
		{
			return false;
		}
		foreach (Item item in inventory)
		{
			if (item.GetBool("IsBullet", false) && !(item.Constants.GetString(this.caliberHash, null) != @string))
			{
				this.SetTargetBulletType(item);
				break;
			}
		}
		return this.targetBulletID != -1;
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x000234D4 File Offset: 0x000216D4
	public int GetBulletCount()
	{
		int num = 0;
		if (base.Item == null)
		{
			return 0;
		}
		foreach (Item item in base.Item.Inventory)
		{
			if (!(item == null))
			{
				num += item.StackCount;
			}
		}
		return num;
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x00023544 File Offset: 0x00021744
	public Item GetCurrentLoadedBullet()
	{
		foreach (Item item in base.Item.Inventory)
		{
			if (!(item == null))
			{
				return item;
			}
		}
		return null;
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x000235A0 File Offset: 0x000217A0
	public int GetBulletCountofTypeInInventory(int bulletItemTypeID, Inventory inventory)
	{
		if (this.targetBulletID == -1)
		{
			return 0;
		}
		int num = 0;
		foreach (Item item in inventory)
		{
			if (!(item == null) && item.TypeID == bulletItemTypeID)
			{
				num += item.StackCount;
			}
		}
		return num;
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x0002360C File Offset: 0x0002180C
	public void TakeOutAllBullets()
	{
		if (base.Item == null)
		{
			return;
		}
		List<Item> list = new List<Item>();
		foreach (Item item in base.Item.Inventory)
		{
			if (!(item == null))
			{
				list.Add(item);
			}
		}
		CharacterMainControl characterMainControl = base.Item.GetCharacterMainControl();
		if (base.Item.InInventory && LevelManager.Instance && base.Item.InInventory == LevelManager.Instance.PetProxy.Inventory)
		{
			characterMainControl = LevelManager.Instance.MainCharacter;
		}
		for (int i = 0; i < list.Count; i++)
		{
			Item item2 = list[i];
			if (!(item2 == null))
			{
				if (characterMainControl)
				{
					item2.Drop(characterMainControl, true);
					characterMainControl.PickupItem(item2);
				}
				else
				{
					bool flag = false;
					Inventory inInventory = base.Item.InInventory;
					if (inInventory)
					{
						flag = inInventory.AddAndMerge(item2, 0);
					}
					if (!flag)
					{
						item2.Detach();
						item2.DestroyTree();
					}
				}
			}
		}
		this.bulletCount = 0;
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x00023758 File Offset: 0x00021958
	public Dictionary<int, BulletTypeInfo> GetBulletTypesInInventory(Inventory inventory)
	{
		Dictionary<int, BulletTypeInfo> dictionary = new Dictionary<int, BulletTypeInfo>();
		string @string = base.Item.Constants.GetString(this.caliberHash, null);
		foreach (Item item in inventory)
		{
			if (!(item == null) && item.GetBool("IsBullet", false) && !(item.Constants.GetString(this.caliberHash, null) != @string))
			{
				if (!dictionary.ContainsKey(item.TypeID))
				{
					BulletTypeInfo bulletTypeInfo = new BulletTypeInfo();
					bulletTypeInfo.bulletTypeID = item.TypeID;
					bulletTypeInfo.count = item.StackCount;
					dictionary.Add(bulletTypeInfo.bulletTypeID, bulletTypeInfo);
				}
				else
				{
					dictionary[item.TypeID].count += item.StackCount;
				}
			}
		}
		return dictionary;
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x00023850 File Offset: 0x00021A50
	public override void SetMarkerParam(Item selfItem)
	{
		selfItem.SetBool("IsGun", true, true);
	}

	// Token: 0x04000752 RID: 1874
	private int targetBulletID = -1;

	// Token: 0x04000753 RID: 1875
	public ADSAimMarker adsAimMarker;

	// Token: 0x04000754 RID: 1876
	public GameObject muzzleFxPfb;

	// Token: 0x04000755 RID: 1877
	public Projectile bulletPfb;

	// Token: 0x04000756 RID: 1878
	public string shootKey = "Default";

	// Token: 0x04000757 RID: 1879
	public string reloadKey = "Default";

	// Token: 0x04000758 RID: 1880
	private int bulletCountHash = "BulletCount".GetHashCode();

	// Token: 0x04000759 RID: 1881
	private int _bulletCountCache = -1;

	// Token: 0x0400075A RID: 1882
	private static int CapacityHash = "Capacity".GetHashCode();

	// Token: 0x0400075B RID: 1883
	private bool loadingBullets;

	// Token: 0x0400075C RID: 1884
	private bool loadBulletsSuccess;

	// Token: 0x0400075D RID: 1885
	private int caliberHash = "Caliber".GetHashCode();

	// Token: 0x0400075E RID: 1886
	public ItemSetting_Gun.TriggerModes triggerMode;

	// Token: 0x0400075F RID: 1887
	public ItemSetting_Gun.ReloadModes reloadMode;

	// Token: 0x04000760 RID: 1888
	public bool autoReload;

	// Token: 0x04000761 RID: 1889
	public ElementTypes element;

	// Token: 0x04000762 RID: 1890
	public Buff buff;

	// Token: 0x04000763 RID: 1891
	private Item preferedBulletsToLoad;

	// Token: 0x02000468 RID: 1128
	public enum TriggerModes
	{
		// Token: 0x04001B32 RID: 6962
		auto,
		// Token: 0x04001B33 RID: 6963
		semi,
		// Token: 0x04001B34 RID: 6964
		bolt
	}

	// Token: 0x02000469 RID: 1129
	public enum ReloadModes
	{
		// Token: 0x04001B36 RID: 6966
		fullMag,
		// Token: 0x04001B37 RID: 6967
		singleBullet
	}
}
