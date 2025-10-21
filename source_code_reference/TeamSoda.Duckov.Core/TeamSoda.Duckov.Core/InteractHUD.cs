using System;
using System.Collections.Generic;
using Duckov.Utilities;
using UnityEngine;

// Token: 0x020000C1 RID: 193
public class InteractHUD : MonoBehaviour
{
	// Token: 0x1700012B RID: 299
	// (get) Token: 0x0600061F RID: 1567 RVA: 0x0001B730 File Offset: 0x00019930
	private PrefabPool<InteractSelectionHUD> Selections
	{
		get
		{
			if (this._selectionsCache == null)
			{
				this._selectionsCache = new PrefabPool<InteractSelectionHUD>(this.selectionPrefab, null, null, null, null, true, 10, 10000, null);
			}
			return this._selectionsCache;
		}
	}

	// Token: 0x06000620 RID: 1568 RVA: 0x0001B769 File Offset: 0x00019969
	private void Awake()
	{
		this.interactableGroup = new List<InteractableBase>();
		this.selectionsHUD = new List<InteractSelectionHUD>();
		this.selectionPrefab.gameObject.SetActive(false);
		this.master.gameObject.SetActive(false);
	}

	// Token: 0x06000621 RID: 1569 RVA: 0x0001B7A4 File Offset: 0x000199A4
	private void Update()
	{
		if (this.characterMainControl == null)
		{
			this.characterMainControl = LevelManager.Instance.MainCharacter;
			if (this.characterMainControl == null)
			{
				return;
			}
		}
		if (this.camera == null)
		{
			this.camera = Camera.main;
			if (this.camera == null)
			{
				return;
			}
		}
		bool flag = false;
		bool flag2 = false;
		this.interactableMaster = this.characterMainControl.interactAction.MasterInteractableAround;
		bool flag3 = InputManager.InputActived && (!this.characterMainControl.CurrentAction || !this.characterMainControl.CurrentAction.Running);
		Shader.SetGlobalFloat(this.interactableHash, flag3 ? 1f : 0f);
		this.interactable = this.interactableMaster != null && flag3;
		if (this.interactable)
		{
			if (this.interactableMaster != this.interactableMasterTemp)
			{
				this.interactableMasterTemp = this.interactableMaster;
				flag = true;
				flag2 = true;
			}
			if (this.interactableIndexTemp != this.characterMainControl.interactAction.InteractIndexInGroup)
			{
				this.interactableIndexTemp = this.characterMainControl.interactAction.InteractIndexInGroup;
				flag2 = true;
			}
		}
		else
		{
			this.interactableMasterTemp = null;
		}
		if (this.interactable != this.master.gameObject.activeInHierarchy)
		{
			this.master.gameObject.SetActive(this.interactable);
		}
		if (flag)
		{
			this.RefreshContent();
			this.SyncPos();
		}
		if (flag2)
		{
			this.RefreshSelection();
		}
	}

	// Token: 0x06000622 RID: 1570 RVA: 0x0001B92B File Offset: 0x00019B2B
	private void LateUpdate()
	{
		if (this.characterMainControl == null)
		{
			return;
		}
		if (this.camera == null)
		{
			return;
		}
		this.SyncPos();
		this.UpdateInteractLine();
	}

	// Token: 0x06000623 RID: 1571 RVA: 0x0001B958 File Offset: 0x00019B58
	private void SyncPos()
	{
		if (!this.syncPosToTarget)
		{
			return;
		}
		if (!this.interactableMaster)
		{
			return;
		}
		Vector3 vector = this.interactableMaster.transform.TransformPoint(this.interactableMaster.interactMarkerOffset);
		Vector3 vector2 = LevelManager.Instance.GameCamera.renderCamera.WorldToScreenPoint(vector);
		Vector2 vector3;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(base.transform.parent as RectTransform, vector2, null, out vector3);
		base.transform.localPosition = vector3;
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x0001B9E0 File Offset: 0x00019BE0
	private void RefreshContent()
	{
		if (this.interactableMaster == null)
		{
			return;
		}
		this.selectionsHUD.Clear();
		this.interactableGroup.Clear();
		foreach (InteractableBase interactableBase in this.interactableMaster.GetInteractableList())
		{
			if (interactableBase != null)
			{
				this.interactableGroup.Add(interactableBase);
			}
		}
		this.Selections.ReleaseAll();
		foreach (InteractableBase interactableBase2 in this.interactableGroup)
		{
			InteractSelectionHUD interactSelectionHUD = this.Selections.Get(null);
			interactSelectionHUD.transform.SetAsLastSibling();
			interactSelectionHUD.SetInteractable(interactableBase2, this.interactableGroup.Count > 1);
			this.selectionsHUD.Add(interactSelectionHUD);
		}
		this.master.ForceUpdateRectTransforms();
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x0001BAF8 File Offset: 0x00019CF8
	private void RefreshSelection()
	{
		InteractableBase interactTarget = this.characterMainControl.interactAction.InteractTarget;
		foreach (InteractSelectionHUD interactSelectionHUD in this.selectionsHUD)
		{
			if (interactSelectionHUD.InteractTarget == interactTarget)
			{
				interactSelectionHUD.SetSelection(true);
			}
			else
			{
				interactSelectionHUD.SetSelection(false);
			}
		}
		this.master.ForceUpdateRectTransforms();
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x0001BB80 File Offset: 0x00019D80
	private void UpdateInteractLine()
	{
	}

	// Token: 0x040005CA RID: 1482
	private CharacterMainControl characterMainControl;

	// Token: 0x040005CB RID: 1483
	public RectTransform master;

	// Token: 0x040005CC RID: 1484
	private InteractableBase interactableMaster;

	// Token: 0x040005CD RID: 1485
	private InteractableBase interactableMasterTemp;

	// Token: 0x040005CE RID: 1486
	private List<InteractableBase> interactableGroup;

	// Token: 0x040005CF RID: 1487
	private List<InteractSelectionHUD> selectionsHUD;

	// Token: 0x040005D0 RID: 1488
	private int interactableIndexTemp;

	// Token: 0x040005D1 RID: 1489
	private bool interactable;

	// Token: 0x040005D2 RID: 1490
	private Camera camera;

	// Token: 0x040005D3 RID: 1491
	public bool syncPosToTarget;

	// Token: 0x040005D4 RID: 1492
	public InteractSelectionHUD selectionPrefab;

	// Token: 0x040005D5 RID: 1493
	private int interactableHash = Shader.PropertyToID("Interactable");

	// Token: 0x040005D6 RID: 1494
	private PrefabPool<InteractSelectionHUD> _selectionsCache;
}
