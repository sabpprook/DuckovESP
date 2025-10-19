using System;
using Cysharp.Threading.Tasks;
using ItemStatsSystem;
using UnityEngine;

namespace Duckov.MiniGames.Utilities
{
	// Token: 0x02000284 RID: 644
	public class GamingConsoleGraphics : MonoBehaviour
	{
		// Token: 0x0600149B RID: 5275 RVA: 0x0004C5F8 File Offset: 0x0004A7F8
		private void Awake()
		{
			this.master.onContentChanged += this.OnContentChanged;
			this.master.OnAfterAnimateIn += this.OnAfterAnimateIn;
			this.master.OnBeforeAnimateOut += this.OnBeforeAnimateOut;
		}

		// Token: 0x0600149C RID: 5276 RVA: 0x0004C64A File Offset: 0x0004A84A
		private void Start()
		{
			this.dirty = true;
		}

		// Token: 0x0600149D RID: 5277 RVA: 0x0004C654 File Offset: 0x0004A854
		private void OnContentChanged(GamingConsole console)
		{
			if (console.Monitor != this._cachedMonitor)
			{
				this.OnMonitorChanged();
			}
			if (console.Console != this._cachedConsole)
			{
				this.OnConsoleChanged();
			}
			if (console.Cartridge != this._cachedCartridge)
			{
				this.OnCatridgeChanged();
			}
			this.dirty = true;
		}

		// Token: 0x0600149E RID: 5278 RVA: 0x0004C6B3 File Offset: 0x0004A8B3
		private void Update()
		{
			if (this.dirty)
			{
				this.RefreshDisplays();
				this.dirty = false;
			}
		}

		// Token: 0x0600149F RID: 5279 RVA: 0x0004C6CC File Offset: 0x0004A8CC
		private void RefreshDisplays()
		{
			if (this.isBeingDestroyed)
			{
				return;
			}
			this._cachedMonitor = this.master.Monitor;
			this._cachedConsole = this.master.Console;
			this._cachedCartridge = this.master.Cartridge;
			if (this.monitorGraphic)
			{
				global::UnityEngine.Object.Destroy(this.monitorGraphic.gameObject);
			}
			if (this.consoleGraphic)
			{
				global::UnityEngine.Object.Destroy(this.consoleGraphic.gameObject);
			}
			if (this._cachedMonitor && !this._cachedMonitor.IsBeingDestroyed)
			{
				this.monitorGraphic = ItemGraphicInfo.CreateAGraphic(this._cachedMonitor, this.monitorRoot);
			}
			if (this._cachedConsole && !this._cachedConsole.IsBeingDestroyed)
			{
				this.consoleGraphic = ItemGraphicInfo.CreateAGraphic(this._cachedConsole, this.consoleRoot);
				if (this.consoleGraphic != null)
				{
					this.pickupAnimation = this.consoleGraphic.GetComponent<ControllerPickupAnimation>();
					this.controllerAnimator = this.consoleGraphic.GetComponentInChildren<ControllerAnimator>();
				}
				else
				{
					this.pickupAnimation = null;
					this.controllerAnimator = null;
				}
				if (this.controllerAnimator != null)
				{
					this.controllerAnimator.SetConsole(this.master);
				}
			}
		}

		// Token: 0x060014A0 RID: 5280 RVA: 0x0004C813 File Offset: 0x0004AA13
		private void OnCatridgeChanged()
		{
		}

		// Token: 0x060014A1 RID: 5281 RVA: 0x0004C815 File Offset: 0x0004AA15
		private void OnConsoleChanged()
		{
		}

		// Token: 0x060014A2 RID: 5282 RVA: 0x0004C817 File Offset: 0x0004AA17
		private void OnMonitorChanged()
		{
		}

		// Token: 0x060014A3 RID: 5283 RVA: 0x0004C819 File Offset: 0x0004AA19
		private void OnDestroy()
		{
			this.isBeingDestroyed = true;
		}

		// Token: 0x060014A4 RID: 5284 RVA: 0x0004C822 File Offset: 0x0004AA22
		private void OnBeforeAnimateOut(GamingConsole console)
		{
			if (this.pickupAnimation == null)
			{
				return;
			}
			this.pickupAnimation.PutDown().Forget();
		}

		// Token: 0x060014A5 RID: 5285 RVA: 0x0004C843 File Offset: 0x0004AA43
		private void OnAfterAnimateIn(GamingConsole console)
		{
			if (this.pickupAnimation == null)
			{
				return;
			}
			this.pickupAnimation.PickUp(this.playingControllerPosition).Forget();
		}

		// Token: 0x04000F20 RID: 3872
		[SerializeField]
		private GamingConsole master;

		// Token: 0x04000F21 RID: 3873
		[SerializeField]
		private Transform monitorRoot;

		// Token: 0x04000F22 RID: 3874
		[SerializeField]
		private Transform consoleRoot;

		// Token: 0x04000F23 RID: 3875
		[SerializeField]
		private Transform playingControllerPosition;

		// Token: 0x04000F24 RID: 3876
		private Transform cartridgeRoot;

		// Token: 0x04000F25 RID: 3877
		private Item _cachedMonitor;

		// Token: 0x04000F26 RID: 3878
		private Item _cachedConsole;

		// Token: 0x04000F27 RID: 3879
		private Item _cachedCartridge;

		// Token: 0x04000F28 RID: 3880
		private ItemGraphicInfo monitorGraphic;

		// Token: 0x04000F29 RID: 3881
		private ItemGraphicInfo consoleGraphic;

		// Token: 0x04000F2A RID: 3882
		private ControllerPickupAnimation pickupAnimation;

		// Token: 0x04000F2B RID: 3883
		private ControllerAnimator controllerAnimator;

		// Token: 0x04000F2C RID: 3884
		private bool dirty;

		// Token: 0x04000F2D RID: 3885
		private bool isBeingDestroyed;
	}
}
