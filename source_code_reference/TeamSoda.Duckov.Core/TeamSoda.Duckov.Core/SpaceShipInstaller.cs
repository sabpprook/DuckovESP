using System;
using Duckov;
using Duckov.Quests;
using Duckov.UI;
using Saves;
using SodaCraft.Localizations;
using UnityEngine;

// Token: 0x020000B2 RID: 178
public class SpaceShipInstaller : MonoBehaviour
{
	// Token: 0x17000122 RID: 290
	// (get) Token: 0x060005DA RID: 1498 RVA: 0x0001A269 File Offset: 0x00018469
	// (set) Token: 0x060005DB RID: 1499 RVA: 0x0001A276 File Offset: 0x00018476
	private bool Installed
	{
		get
		{
			return SavesSystem.Load<bool>(this.saveDataKey);
		}
		set
		{
			SavesSystem.Save<bool>(this.saveDataKey, value);
		}
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x0001A284 File Offset: 0x00018484
	private void Awake()
	{
		if (this.buildFx)
		{
			this.buildFx.SetActive(false);
		}
		this.interactable.overrideInteractName = true;
		this.interactable._overrideInteractNameKey = this.interactKey;
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x0001A2BC File Offset: 0x000184BC
	public void Install()
	{
		if (this.buildFx)
		{
			this.buildFx.SetActive(true);
		}
		AudioManager.Post("Archived/Building/Default/Constructed", base.gameObject);
		this.Installed = true;
		this.SyncGraphic(true);
		this.interactable.gameObject.SetActive(false);
		NotificationText.Push(this.notificationKey.ToPlainText());
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x0001A322 File Offset: 0x00018522
	private void SyncGraphic(bool _installed)
	{
		if (this.builtGraphic)
		{
			this.builtGraphic.SetActive(_installed);
		}
		if (this.unbuiltGraphic)
		{
			this.unbuiltGraphic.SetActive(!_installed);
		}
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x0001A35C File Offset: 0x0001855C
	private void Update()
	{
		if (!LevelManager.LevelInited)
		{
			return;
		}
		if (!this.inited)
		{
			bool flag = this.Installed;
			if (flag)
			{
				TaskEvent.EmitTaskEvent(this.saveDataKey);
			}
			else if (QuestManager.IsQuestFinished(this.questID))
			{
				flag = true;
				this.Installed = true;
			}
			this.interactable.gameObject.SetActive(!flag && QuestManager.IsQuestActive(this.questID));
			this.SyncGraphic(flag);
			this.inited = true;
		}
		if (!this.Installed && !this.interactable.gameObject.activeSelf && QuestManager.IsQuestActive(this.questID))
		{
			this.interactable.gameObject.SetActive(true);
		}
	}

	// Token: 0x04000562 RID: 1378
	[SerializeField]
	private string saveDataKey;

	// Token: 0x04000563 RID: 1379
	[SerializeField]
	private int questID;

	// Token: 0x04000564 RID: 1380
	[SerializeField]
	private InteractableBase interactable;

	// Token: 0x04000565 RID: 1381
	[SerializeField]
	[LocalizationKey("Default")]
	private string notificationKey;

	// Token: 0x04000566 RID: 1382
	[SerializeField]
	[LocalizationKey("Default")]
	private string interactKey;

	// Token: 0x04000567 RID: 1383
	private bool inited;

	// Token: 0x04000568 RID: 1384
	public GameObject builtGraphic;

	// Token: 0x04000569 RID: 1385
	public GameObject unbuiltGraphic;

	// Token: 0x0400056A RID: 1386
	public GameObject buildFx;
}
