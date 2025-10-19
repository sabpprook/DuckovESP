using System;
using Duckov.Economy;
using Duckov.Scenes;
using Saves;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000D2 RID: 210
public class ConstructionSite : MonoBehaviour
{
	// Token: 0x17000130 RID: 304
	// (get) Token: 0x06000671 RID: 1649 RVA: 0x0001D357 File Offset: 0x0001B557
	private Color KeyFieldColor
	{
		get
		{
			if (string.IsNullOrWhiteSpace(this._key))
			{
				return Color.red;
			}
			return Color.white;
		}
	}

	// Token: 0x17000131 RID: 305
	// (get) Token: 0x06000672 RID: 1650 RVA: 0x0001D374 File Offset: 0x0001B574
	private string SaveKey
	{
		get
		{
			return "ConstructionSite_" + this._key;
		}
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x0001D388 File Offset: 0x0001B588
	private void Awake()
	{
		this.costTaker.onPayed += this.OnBuilt;
		this.Load();
		SavesSystem.OnCollectSaveData += this.Save;
		this.costTaker.SetCost(this.cost);
		this.RefreshGameObjects();
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x0001D3DA File Offset: 0x0001B5DA
	private void OnDestroy()
	{
		SavesSystem.OnCollectSaveData -= this.Save;
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x0001D3F0 File Offset: 0x0001B5F0
	private void Save()
	{
		if (this.dontSave)
		{
			int inLevelDataKey = this.GetInLevelDataKey();
			if (MultiSceneCore.Instance.inLevelData.ContainsKey(inLevelDataKey))
			{
				MultiSceneCore.Instance.inLevelData[inLevelDataKey] = this.wasBuilt;
				return;
			}
			MultiSceneCore.Instance.inLevelData.Add(inLevelDataKey, this.wasBuilt);
			return;
		}
		else
		{
			if (string.IsNullOrWhiteSpace(this._key))
			{
				Debug.LogError(string.Format("Construction Site {0} 没有配置保存用的key", base.gameObject));
				return;
			}
			SavesSystem.Save<bool>(this.SaveKey, this.wasBuilt);
			return;
		}
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x0001D48C File Offset: 0x0001B68C
	private int GetInLevelDataKey()
	{
		Vector3 vector = base.transform.position * 10f;
		int num = Mathf.RoundToInt(vector.x);
		int num2 = Mathf.RoundToInt(vector.y);
		int num3 = Mathf.RoundToInt(vector.z);
		Vector3Int vector3Int = new Vector3Int(num, num2, num3);
		return ("ConstSite" + vector3Int.ToString()).GetHashCode();
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x0001D4F8 File Offset: 0x0001B6F8
	private void Load()
	{
		if (!this.dontSave)
		{
			if (string.IsNullOrWhiteSpace(this._key))
			{
				Debug.LogError(string.Format("Construction Site {0} 没有配置保存用的key", base.gameObject));
			}
			this.wasBuilt = SavesSystem.Load<bool>(this.SaveKey);
		}
		else
		{
			int inLevelDataKey = this.GetInLevelDataKey();
			object obj;
			MultiSceneCore.Instance.inLevelData.TryGetValue(inLevelDataKey, out obj);
			if (obj != null)
			{
				this.wasBuilt = (bool)obj;
			}
		}
		if (this.wasBuilt)
		{
			this.OnActivate();
			return;
		}
		this.OnDeactivate();
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x0001D580 File Offset: 0x0001B780
	private void Start()
	{
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x0001D584 File Offset: 0x0001B784
	private void OnBuilt(CostTaker taker)
	{
		this.wasBuilt = true;
		UnityEvent<ConstructionSite> unityEvent = this.onBuilt;
		if (unityEvent != null)
		{
			unityEvent.Invoke(this);
		}
		this.RefreshGameObjects();
		foreach (GameObject gameObject in this.setActiveOnBuilt)
		{
			if (gameObject)
			{
				gameObject.SetActive(true);
			}
		}
		this.Save();
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x0001D5DE File Offset: 0x0001B7DE
	private void OnActivate()
	{
		UnityEvent<ConstructionSite> unityEvent = this.onActivate;
		if (unityEvent != null)
		{
			unityEvent.Invoke(this);
		}
		this.RefreshGameObjects();
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x0001D5F8 File Offset: 0x0001B7F8
	private void OnDeactivate()
	{
		UnityEvent<ConstructionSite> unityEvent = this.onDeactivate;
		if (unityEvent != null)
		{
			unityEvent.Invoke(this);
		}
		this.RefreshGameObjects();
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x0001D614 File Offset: 0x0001B814
	public void RefreshGameObjects()
	{
		this.costTaker.gameObject.SetActive(!this.wasBuilt);
		foreach (GameObject gameObject in this.notBuiltGameObjects)
		{
			if (gameObject)
			{
				gameObject.SetActive(!this.wasBuilt);
			}
		}
		foreach (GameObject gameObject2 in this.builtGameObjects)
		{
			if (gameObject2)
			{
				gameObject2.SetActive(this.wasBuilt);
			}
		}
	}

	// Token: 0x04000646 RID: 1606
	[SerializeField]
	private string _key;

	// Token: 0x04000647 RID: 1607
	[SerializeField]
	private bool dontSave;

	// Token: 0x04000648 RID: 1608
	private bool saveInMultiSceneCore;

	// Token: 0x04000649 RID: 1609
	[SerializeField]
	private Cost cost;

	// Token: 0x0400064A RID: 1610
	[SerializeField]
	private CostTaker costTaker;

	// Token: 0x0400064B RID: 1611
	[SerializeField]
	private GameObject[] notBuiltGameObjects;

	// Token: 0x0400064C RID: 1612
	[SerializeField]
	private GameObject[] builtGameObjects;

	// Token: 0x0400064D RID: 1613
	[SerializeField]
	private GameObject[] setActiveOnBuilt;

	// Token: 0x0400064E RID: 1614
	[SerializeField]
	private UnityEvent<ConstructionSite> onBuilt;

	// Token: 0x0400064F RID: 1615
	[SerializeField]
	private UnityEvent<ConstructionSite> onActivate;

	// Token: 0x04000650 RID: 1616
	[SerializeField]
	private UnityEvent<ConstructionSite> onDeactivate;

	// Token: 0x04000651 RID: 1617
	private bool wasBuilt;
}
