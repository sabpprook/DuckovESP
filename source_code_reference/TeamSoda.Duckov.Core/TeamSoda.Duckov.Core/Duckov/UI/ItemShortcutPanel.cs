using System;
using ItemStatsSystem;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x020003A9 RID: 937
	public class ItemShortcutPanel : MonoBehaviour
	{
		// Token: 0x17000671 RID: 1649
		// (get) Token: 0x060021A4 RID: 8612 RVA: 0x000754D4 File Offset: 0x000736D4
		// (set) Token: 0x060021A5 RID: 8613 RVA: 0x000754DC File Offset: 0x000736DC
		public Inventory Target { get; private set; }

		// Token: 0x17000672 RID: 1650
		// (get) Token: 0x060021A6 RID: 8614 RVA: 0x000754E5 File Offset: 0x000736E5
		// (set) Token: 0x060021A7 RID: 8615 RVA: 0x000754ED File Offset: 0x000736ED
		public CharacterMainControl Character { get; internal set; }

		// Token: 0x060021A8 RID: 8616 RVA: 0x000754F6 File Offset: 0x000736F6
		private void Awake()
		{
			LevelManager.OnLevelInitialized += this.OnLevelInitialized;
			if (LevelManager.LevelInited)
			{
				this.Initialize();
			}
		}

		// Token: 0x060021A9 RID: 8617 RVA: 0x00075516 File Offset: 0x00073716
		private void OnDestroy()
		{
			LevelManager.OnLevelInitialized -= this.OnLevelInitialized;
		}

		// Token: 0x060021AA RID: 8618 RVA: 0x00075529 File Offset: 0x00073729
		private void OnLevelInitialized()
		{
			this.Initialize();
		}

		// Token: 0x060021AB RID: 8619 RVA: 0x00075534 File Offset: 0x00073734
		private void Initialize()
		{
			LevelManager instance = LevelManager.Instance;
			this.Character = ((instance != null) ? instance.MainCharacter : null);
			if (this.Character == null)
			{
				return;
			}
			LevelManager instance2 = LevelManager.Instance;
			Inventory inventory;
			if (instance2 == null)
			{
				inventory = null;
			}
			else
			{
				CharacterMainControl mainCharacter = instance2.MainCharacter;
				if (mainCharacter == null)
				{
					inventory = null;
				}
				else
				{
					Item characterItem = mainCharacter.CharacterItem;
					inventory = ((characterItem != null) ? characterItem.Inventory : null);
				}
			}
			this.Target = inventory;
			if (this.Target == null)
			{
				return;
			}
			for (int i = 0; i < this.buttons.Length; i++)
			{
				ItemShortcutButton itemShortcutButton = this.buttons[i];
				if (!(itemShortcutButton == null))
				{
					itemShortcutButton.Initialize(this, i);
				}
			}
			this.initialized = true;
		}

		// Token: 0x040016BF RID: 5823
		[SerializeField]
		private ItemShortcutButton[] buttons;

		// Token: 0x040016C2 RID: 5826
		private bool initialized;
	}
}
