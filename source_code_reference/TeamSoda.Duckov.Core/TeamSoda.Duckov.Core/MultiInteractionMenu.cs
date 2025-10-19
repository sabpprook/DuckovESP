using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.Utilities;
using UnityEngine;

// Token: 0x020001F8 RID: 504
public class MultiInteractionMenu : MonoBehaviour
{
	// Token: 0x170002A4 RID: 676
	// (get) Token: 0x06000EBD RID: 3773 RVA: 0x0003AC62 File Offset: 0x00038E62
	// (set) Token: 0x06000EBE RID: 3774 RVA: 0x0003AC69 File Offset: 0x00038E69
	public static MultiInteractionMenu Instance { get; private set; }

	// Token: 0x170002A5 RID: 677
	// (get) Token: 0x06000EBF RID: 3775 RVA: 0x0003AC74 File Offset: 0x00038E74
	private PrefabPool<MultiInteractionMenuButton> ButtonPool
	{
		get
		{
			if (this._buttonPool == null)
			{
				this._buttonPool = new PrefabPool<MultiInteractionMenuButton>(this.buttonTemplate, this.buttonTemplate.transform.parent, null, null, null, true, 10, 10000, null);
				this.buttonTemplate.gameObject.SetActive(false);
			}
			return this._buttonPool;
		}
	}

	// Token: 0x170002A6 RID: 678
	// (get) Token: 0x06000EC0 RID: 3776 RVA: 0x0003ACCD File Offset: 0x00038ECD
	public MultiInteraction Target
	{
		get
		{
			return this.target;
		}
	}

	// Token: 0x06000EC1 RID: 3777 RVA: 0x0003ACD5 File Offset: 0x00038ED5
	private void Awake()
	{
		if (MultiInteractionMenu.Instance == null)
		{
			MultiInteractionMenu.Instance = this;
		}
		this.buttonTemplate.gameObject.SetActive(false);
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000EC2 RID: 3778 RVA: 0x0003AD08 File Offset: 0x00038F08
	private void Setup(MultiInteraction target)
	{
		this.target = target;
		ReadOnlyCollection<InteractableBase> interactables = target.Interactables;
		this.ButtonPool.ReleaseAll();
		foreach (InteractableBase interactableBase in interactables)
		{
			if (!(interactableBase == null))
			{
				MultiInteractionMenuButton multiInteractionMenuButton = this.ButtonPool.Get(null);
				multiInteractionMenuButton.Setup(interactableBase);
				multiInteractionMenuButton.transform.SetAsLastSibling();
			}
		}
	}

	// Token: 0x06000EC3 RID: 3779 RVA: 0x0003AD88 File Offset: 0x00038F88
	private int CreateNewToken()
	{
		this.currentTaskToken = global::UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		return this.currentTaskToken;
	}

	// Token: 0x06000EC4 RID: 3780 RVA: 0x0003ADA5 File Offset: 0x00038FA5
	private bool TokenChanged(int token)
	{
		return token != this.currentTaskToken;
	}

	// Token: 0x06000EC5 RID: 3781 RVA: 0x0003ADB4 File Offset: 0x00038FB4
	public async UniTask SetupAndShow(MultiInteraction target)
	{
		base.gameObject.SetActive(true);
		int token = this.CreateNewToken();
		this.Setup(target);
		ReadOnlyCollection<MultiInteractionMenuButton> activeEntries = this.ButtonPool.ActiveEntries;
		foreach (MultiInteractionMenuButton multiInteractionMenuButton in activeEntries)
		{
			multiInteractionMenuButton.Show();
			await UniTask.WaitForSeconds(this.delayEachButton, true, PlayerLoopTiming.Update, default(CancellationToken), false);
			if (this.TokenChanged(token))
			{
				return;
			}
		}
		IEnumerator<MultiInteractionMenuButton> enumerator = null;
	}

	// Token: 0x06000EC6 RID: 3782 RVA: 0x0003AE00 File Offset: 0x00039000
	public async UniTask Hide()
	{
		int token = this.CreateNewToken();
		ReadOnlyCollection<MultiInteractionMenuButton> activeEntries = this.ButtonPool.ActiveEntries;
		foreach (MultiInteractionMenuButton multiInteractionMenuButton in activeEntries)
		{
			multiInteractionMenuButton.Hide();
			await UniTask.WaitForSeconds(this.delayEachButton, true, PlayerLoopTiming.Update, default(CancellationToken), false);
			if (this.TokenChanged(token))
			{
				return;
			}
		}
		IEnumerator<MultiInteractionMenuButton> enumerator = null;
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000C31 RID: 3121
	[SerializeField]
	private MultiInteractionMenuButton buttonTemplate;

	// Token: 0x04000C32 RID: 3122
	[SerializeField]
	private float delayEachButton = 0.25f;

	// Token: 0x04000C33 RID: 3123
	private PrefabPool<MultiInteractionMenuButton> _buttonPool;

	// Token: 0x04000C34 RID: 3124
	private MultiInteraction target;

	// Token: 0x04000C35 RID: 3125
	private int currentTaskToken;
}
