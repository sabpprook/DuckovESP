using System;
using System.Collections.Generic;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000E2 RID: 226
public class DuckovItemAgent : ItemAgent
{
	// Token: 0x1700014A RID: 330
	// (get) Token: 0x06000720 RID: 1824 RVA: 0x0001FFE7 File Offset: 0x0001E1E7
	public CharacterMainControl Holder
	{
		get
		{
			return this.holder;
		}
	}

	// Token: 0x1700014B RID: 331
	// (get) Token: 0x06000721 RID: 1825 RVA: 0x0001FFF0 File Offset: 0x0001E1F0
	private Dictionary<string, Transform> SocketsDic
	{
		get
		{
			if (this._socketsDic == null)
			{
				this._socketsDic = new Dictionary<string, Transform>();
				foreach (Transform transform in this.socketsList)
				{
					this._socketsDic.Add(transform.name, transform);
				}
			}
			return this._socketsDic;
		}
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x00020068 File Offset: 0x0001E268
	public Transform GetSocket(string socketName, bool createNew)
	{
		Transform transform;
		bool flag = this.SocketsDic.TryGetValue(socketName, out transform);
		if (flag && transform == null)
		{
			this.SocketsDic.Remove(socketName);
		}
		if (!flag && createNew)
		{
			transform = new GameObject(socketName).transform;
			transform.SetParent(base.transform);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			this.SocketsDic.Add(socketName, transform);
		}
		return transform;
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x000200DF File Offset: 0x0001E2DF
	public void SetHolder(CharacterMainControl _holder)
	{
		this.holder = _holder;
		if (this.setActiveIfMainCharacter)
		{
			this.setActiveIfMainCharacter.SetActive(_holder.IsMainCharacter);
		}
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x00020106 File Offset: 0x0001E306
	public CharacterMainControl GetHolder()
	{
		return this.holder;
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x0002010E File Offset: 0x0001E30E
	protected override void OnInitialize()
	{
		base.OnInitialize();
		this.InitInterfaces();
		UnityEvent onInitializdEvent = this.OnInitializdEvent;
		if (onInitializdEvent == null)
		{
			return;
		}
		onInitializdEvent.Invoke();
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x0002012C File Offset: 0x0001E32C
	private void InitInterfaces()
	{
		this.usableInterface = this as IAgentUsable;
	}

	// Token: 0x1700014C RID: 332
	// (get) Token: 0x06000727 RID: 1831 RVA: 0x0002013A File Offset: 0x0001E33A
	public IAgentUsable UsableInterface
	{
		get
		{
			return this.usableInterface;
		}
	}

	// Token: 0x040006CE RID: 1742
	public HandheldSocketTypes handheldSocket = HandheldSocketTypes.normalHandheld;

	// Token: 0x040006CF RID: 1743
	public HandheldAnimationType handAnimationType = HandheldAnimationType.normal;

	// Token: 0x040006D0 RID: 1744
	private CharacterMainControl holder;

	// Token: 0x040006D1 RID: 1745
	public UnityEvent OnInitializdEvent;

	// Token: 0x040006D2 RID: 1746
	[SerializeField]
	private List<Transform> socketsList = new List<Transform>();

	// Token: 0x040006D3 RID: 1747
	public GameObject setActiveIfMainCharacter;

	// Token: 0x040006D4 RID: 1748
	private Dictionary<string, Transform> _socketsDic;

	// Token: 0x040006D5 RID: 1749
	private IAgentUsable usableInterface;
}
