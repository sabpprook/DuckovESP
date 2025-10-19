using System;
using System.Collections.Generic;
using Duckov;
using Duckov.Scenes;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000D6 RID: 214
public class Door : MonoBehaviour
{
	// Token: 0x17000136 RID: 310
	// (get) Token: 0x060006A1 RID: 1697 RVA: 0x0001DB8E File Offset: 0x0001BD8E
	public bool IsOpen
	{
		get
		{
			return !this.closed;
		}
	}

	// Token: 0x17000137 RID: 311
	// (get) Token: 0x060006A2 RID: 1698 RVA: 0x0001DB99 File Offset: 0x0001BD99
	public bool NoRequireItem
	{
		get
		{
			return !this.interact || !this.interact.requireItem;
		}
	}

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x060006A3 RID: 1699 RVA: 0x0001DBB8 File Offset: 0x0001BDB8
	public InteractableBase Interact
	{
		get
		{
			return this.interact;
		}
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x0001DBC0 File Offset: 0x0001BDC0
	private void Start()
	{
		if (this._doorClosedDataKeyCached == -1)
		{
			this._doorClosedDataKeyCached = this.GetKey();
		}
		object obj;
		if (!this.ignoreInLevelData && MultiSceneCore.Instance && MultiSceneCore.Instance.inLevelData.TryGetValue(this._doorClosedDataKeyCached, out obj) && obj is bool)
		{
			bool flag = (bool)obj;
			Debug.Log(string.Format("存在门存档信息：{0}", flag));
			this.closed = flag;
		}
		this.targetLerpValue = (this.closedLerpValue = (this.closed ? 1f : 0f));
		this.SyncNavmeshCut();
		this.SetPartsByLerpValue(true);
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x0001DC6A File Offset: 0x0001BE6A
	private void OnEnable()
	{
		if (this.doorCollider)
		{
			this.doorCollider.isTrigger = true;
		}
	}

	// Token: 0x060006A6 RID: 1702 RVA: 0x0001DC85 File Offset: 0x0001BE85
	private void OnDisable()
	{
		if (this.doorCollider)
		{
			this.doorCollider.isTrigger = false;
		}
	}

	// Token: 0x060006A7 RID: 1703 RVA: 0x0001DCA0 File Offset: 0x0001BEA0
	private void SyncNavmeshCut()
	{
		if (!this.closed)
		{
			bool flag = this.activeNavmeshCutWhenDoorIsOpen;
			foreach (NavmeshCut navmeshCut in this.navmeshCuts)
			{
				if (navmeshCut)
				{
					navmeshCut.enabled = flag;
				}
			}
			return;
		}
		if (this.NoRequireItem)
		{
			return;
		}
	}

	// Token: 0x060006A8 RID: 1704 RVA: 0x0001DD1C File Offset: 0x0001BF1C
	private void Update()
	{
		this.targetLerpValue = (this.closed ? 1f : 0f);
		if (this.targetLerpValue == this.closedLerpValue)
		{
			base.enabled = false;
		}
		this.closedLerpValue = Mathf.MoveTowards(this.closedLerpValue, this.targetLerpValue, Time.deltaTime / this.lerpTime);
		this.SetPartsByLerpValue(this.targetLerpValue == this.closedLerpValue);
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x0001DD8F File Offset: 0x0001BF8F
	public void Switch()
	{
		this.SetClosed(!this.closed, true);
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x0001DDA1 File Offset: 0x0001BFA1
	public void Open()
	{
		this.SetClosed(false, true);
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x0001DDAB File Offset: 0x0001BFAB
	public void Close()
	{
		this.SetClosed(true, true);
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x0001DDB5 File Offset: 0x0001BFB5
	public void ForceSetClosed(bool _closed, bool triggerEvent)
	{
		this.SetClosed(_closed, triggerEvent);
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x0001DDC0 File Offset: 0x0001BFC0
	private void SetClosed(bool _closed, bool triggerEvent = true)
	{
		if (!LevelManager.LevelInited)
		{
			Debug.LogError("在关卡没有初始化时，不能对门进行设置");
			return;
		}
		if (triggerEvent)
		{
			if (_closed)
			{
				UnityEvent onCloseEvent = this.OnCloseEvent;
				if (onCloseEvent != null)
				{
					onCloseEvent.Invoke();
				}
			}
			else
			{
				UnityEvent onOpenEvent = this.OnOpenEvent;
				if (onOpenEvent != null)
				{
					onOpenEvent.Invoke();
				}
			}
		}
		Debug.Log(string.Format("Set Door Closed:{0}", _closed));
		if (this._doorClosedDataKeyCached == -1)
		{
			this._doorClosedDataKeyCached = this.GetKey();
		}
		this.closed = _closed;
		this.targetLerpValue = (this.closed ? 1f : 0f);
		if (this.closedLerpValue != this.targetLerpValue)
		{
			base.enabled = true;
		}
		if (this.hasSound)
		{
			AudioManager.Post(_closed ? this.closeSound : this.openSound, base.gameObject);
		}
		if (MultiSceneCore.Instance)
		{
			MultiSceneCore.Instance.inLevelData[this._doorClosedDataKeyCached] = this.closed;
		}
		else
		{
			Debug.Log("没有MultiScene Core，无法存储data");
		}
		this.SyncNavmeshCut();
	}

	// Token: 0x060006AE RID: 1710 RVA: 0x0001DECC File Offset: 0x0001C0CC
	private List<Door.DoorTransformInfo> GetCurrentTransformInfos()
	{
		List<Door.DoorTransformInfo> list = new List<Door.DoorTransformInfo>();
		foreach (Transform transform in this.doorParts)
		{
			Door.DoorTransformInfo doorTransformInfo = default(Door.DoorTransformInfo);
			if (transform != null)
			{
				doorTransformInfo.target = transform;
				doorTransformInfo.localPosition = transform.localPosition;
				doorTransformInfo.localRotation = transform.localRotation;
				doorTransformInfo.activation = transform.gameObject.activeSelf;
			}
			list.Add(doorTransformInfo);
		}
		return list;
	}

	// Token: 0x060006AF RID: 1711 RVA: 0x0001DF70 File Offset: 0x0001C170
	public void SetParts(List<Door.DoorTransformInfo> transforms)
	{
		for (int i = 0; i < transforms.Count; i++)
		{
			Door.DoorTransformInfo doorTransformInfo = transforms[i];
			if (!(doorTransformInfo.target == null))
			{
				doorTransformInfo.target.localPosition = doorTransformInfo.localPosition;
				doorTransformInfo.target.localRotation = doorTransformInfo.localRotation;
				doorTransformInfo.target.gameObject.SetActive(doorTransformInfo.activation);
			}
		}
	}

	// Token: 0x060006B0 RID: 1712 RVA: 0x0001DFE4 File Offset: 0x0001C1E4
	private void SetPartsByLerpValue(bool setActivation)
	{
		if (this.doorParts.Count != this.closeTransforms.Count || this.doorParts.Count != this.openTransforms.Count)
		{
			return;
		}
		for (int i = 0; i < this.openTransforms.Count; i++)
		{
			Door.DoorTransformInfo doorTransformInfo = this.openTransforms[i];
			Door.DoorTransformInfo doorTransformInfo2 = this.closeTransforms[i];
			if (!(doorTransformInfo.target == null) && !(doorTransformInfo.target != doorTransformInfo2.target))
			{
				doorTransformInfo.target.localPosition = Vector3.Lerp(doorTransformInfo.localPosition, doorTransformInfo2.localPosition, this.closedLerpValue);
				doorTransformInfo.target.localRotation = Quaternion.Lerp(doorTransformInfo.localRotation, doorTransformInfo2.localRotation, this.closedLerpValue);
				if (setActivation)
				{
					if (this.closedLerpValue >= 1f)
					{
						doorTransformInfo.target.gameObject.SetActive(doorTransformInfo2.activation);
					}
					else
					{
						doorTransformInfo.target.gameObject.SetActive(doorTransformInfo.activation);
					}
				}
			}
		}
	}

	// Token: 0x060006B1 RID: 1713 RVA: 0x0001E10C File Offset: 0x0001C30C
	private int GetKey()
	{
		Vector3 vector = base.transform.position * 10f;
		int num = Mathf.RoundToInt(vector.x);
		int num2 = Mathf.RoundToInt(vector.y);
		int num3 = Mathf.RoundToInt(vector.z);
		Vector3Int vector3Int = new Vector3Int(num, num2, num3);
		return string.Format("Door_{0}", vector3Int).GetHashCode();
	}

	// Token: 0x04000662 RID: 1634
	private bool closed = true;

	// Token: 0x04000663 RID: 1635
	private float closedLerpValue;

	// Token: 0x04000664 RID: 1636
	private float targetLerpValue;

	// Token: 0x04000665 RID: 1637
	[SerializeField]
	private float lerpTime = 0.5f;

	// Token: 0x04000666 RID: 1638
	[SerializeField]
	private List<Transform> doorParts;

	// Token: 0x04000667 RID: 1639
	[SerializeField]
	private List<Door.DoorTransformInfo> closeTransforms;

	// Token: 0x04000668 RID: 1640
	[SerializeField]
	private List<Door.DoorTransformInfo> openTransforms;

	// Token: 0x04000669 RID: 1641
	[SerializeField]
	private DoorTrigger doorTrigger;

	// Token: 0x0400066A RID: 1642
	[SerializeField]
	private Collider doorCollider;

	// Token: 0x0400066B RID: 1643
	[SerializeField]
	private List<NavmeshCut> navmeshCuts = new List<NavmeshCut>();

	// Token: 0x0400066C RID: 1644
	[SerializeField]
	private bool activeNavmeshCutWhenDoorIsOpen = true;

	// Token: 0x0400066D RID: 1645
	[SerializeField]
	private bool ignoreInLevelData;

	// Token: 0x0400066E RID: 1646
	private int _doorClosedDataKeyCached = -1;

	// Token: 0x0400066F RID: 1647
	[SerializeField]
	private InteractableBase interact;

	// Token: 0x04000670 RID: 1648
	public bool hasSound;

	// Token: 0x04000671 RID: 1649
	public string openSound = "SFX/Actions/door_normal_open";

	// Token: 0x04000672 RID: 1650
	public string closeSound = "SFX/Actions/door_normal_close";

	// Token: 0x04000673 RID: 1651
	public UnityEvent OnOpenEvent;

	// Token: 0x04000674 RID: 1652
	public UnityEvent OnCloseEvent;

	// Token: 0x0200045E RID: 1118
	[Serializable]
	public struct DoorTransformInfo
	{
		// Token: 0x04001B05 RID: 6917
		public Transform target;

		// Token: 0x04001B06 RID: 6918
		public Vector3 localPosition;

		// Token: 0x04001B07 RID: 6919
		public quaternion localRotation;

		// Token: 0x04001B08 RID: 6920
		public bool activation;
	}
}
