using System;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020000A7 RID: 167
public class KunEvents : MonoBehaviour
{
	// Token: 0x060005AB RID: 1451 RVA: 0x000195B6 File Offset: 0x000177B6
	private void Awake()
	{
		this.setActiveObject.SetActive(false);
		if (!this.dialogueBubbleProxy)
		{
			this.dialogueBubbleProxy.GetComponent<DialogueBubbleProxy>();
		}
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x000195E0 File Offset: 0x000177E0
	public void Check()
	{
		bool flag = false;
		if (CharacterMainControl.Main == null)
		{
			return;
		}
		CharacterMainControl main = CharacterMainControl.Main;
		CharacterModel characterModel = main.characterModel;
		if (!characterModel)
		{
			return;
		}
		CustomFaceInstance customFace = characterModel.CustomFace;
		if (!customFace)
		{
			return;
		}
		bool flag2 = customFace.ConvertToSaveData().hairID == this.hairID;
		Item armorItem = main.GetArmorItem();
		if (armorItem != null && armorItem.TypeID == this.armorID)
		{
			flag = true;
		}
		if (!flag2 && !flag)
		{
			this.dialogueBubbleProxy.textKey = this.notRight;
		}
		else if (flag2 && !flag)
		{
			this.dialogueBubbleProxy.textKey = this.onlyRightFace;
		}
		else if (!flag2 && flag)
		{
			this.dialogueBubbleProxy.textKey = this.onlyRightCloth;
		}
		else
		{
			this.dialogueBubbleProxy.textKey = this.allRight;
			this.setActiveObject.SetActive(true);
		}
		this.dialogueBubbleProxy.Pop();
	}

	// Token: 0x04000527 RID: 1319
	[SerializeField]
	private int hairID = 6;

	// Token: 0x04000528 RID: 1320
	[ItemTypeID]
	[SerializeField]
	private int armorID;

	// Token: 0x04000529 RID: 1321
	public DialogueBubbleProxy dialogueBubbleProxy;

	// Token: 0x0400052A RID: 1322
	[LocalizationKey("Dialogues")]
	public string notRight;

	// Token: 0x0400052B RID: 1323
	[LocalizationKey("Dialogues")]
	public string onlyRightFace;

	// Token: 0x0400052C RID: 1324
	[LocalizationKey("Dialogues")]
	public string onlyRightCloth;

	// Token: 0x0400052D RID: 1325
	[LocalizationKey("Dialogues")]
	public string allRight;

	// Token: 0x0400052E RID: 1326
	[FormerlySerializedAs("SetActiveObject")]
	public GameObject setActiveObject;
}
