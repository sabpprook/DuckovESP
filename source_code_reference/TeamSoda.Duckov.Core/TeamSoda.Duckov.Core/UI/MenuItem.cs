using System;
using UnityEngine;

namespace UI
{
	// Token: 0x0200020E RID: 526
	public class MenuItem : MonoBehaviour
	{
		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x06000FB3 RID: 4019 RVA: 0x0003DBEB File Offset: 0x0003BDEB
		// (set) Token: 0x06000FB4 RID: 4020 RVA: 0x0003DC1E File Offset: 0x0003BE1E
		public Menu Master
		{
			get
			{
				if (this._master == null)
				{
					Transform parent = base.transform.parent;
					this._master = ((parent != null) ? parent.GetComponent<Menu>() : null);
				}
				return this._master;
			}
			set
			{
				this._master = value;
			}
		}

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x06000FB5 RID: 4021 RVA: 0x0003DC27 File Offset: 0x0003BE27
		// (set) Token: 0x06000FB6 RID: 4022 RVA: 0x0003DC3E File Offset: 0x0003BE3E
		public bool Selectable
		{
			get
			{
				return base.gameObject.activeSelf && this.selectable;
			}
			set
			{
				this.selectable = value;
			}
		}

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x06000FB7 RID: 4023 RVA: 0x0003DC47 File Offset: 0x0003BE47
		public bool IsSelected
		{
			get
			{
				return this.cacheSelected;
			}
		}

		// Token: 0x06000FB8 RID: 4024 RVA: 0x0003DC4F File Offset: 0x0003BE4F
		private void OnTransformParentChanged()
		{
			if (this.Master == null)
			{
				return;
			}
			this.Master.Register(this);
		}

		// Token: 0x06000FB9 RID: 4025 RVA: 0x0003DC6C File Offset: 0x0003BE6C
		private void OnEnable()
		{
			if (this.Master == null)
			{
				return;
			}
			this.Master.Register(this);
		}

		// Token: 0x06000FBA RID: 4026 RVA: 0x0003DC89 File Offset: 0x0003BE89
		private void OnDisable()
		{
			if (this.Master == null)
			{
				return;
			}
			this.Master.Unegister(this);
		}

		// Token: 0x06000FBB RID: 4027 RVA: 0x0003DCA6 File Offset: 0x0003BEA6
		public void Select()
		{
			if (this.Master == null)
			{
				Debug.LogError("Menu Item " + base.name + " 没有Master。");
				return;
			}
			this.Master.Select(this);
		}

		// Token: 0x06000FBC RID: 4028 RVA: 0x0003DCDD File Offset: 0x0003BEDD
		internal void NotifySelected()
		{
			this.cacheSelected = true;
			Action<MenuItem> action = this.onSelected;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06000FBD RID: 4029 RVA: 0x0003DCF7 File Offset: 0x0003BEF7
		internal void NotifyDeselected()
		{
			this.cacheSelected = false;
			Action<MenuItem> action = this.onDeselected;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06000FBE RID: 4030 RVA: 0x0003DD11 File Offset: 0x0003BF11
		internal void NotifyConfirmed()
		{
			Action<MenuItem> action = this.onConfirmed;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06000FBF RID: 4031 RVA: 0x0003DD24 File Offset: 0x0003BF24
		internal void NotifyCanceled()
		{
			Action<MenuItem> action = this.onCanceled;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06000FC0 RID: 4032 RVA: 0x0003DD37 File Offset: 0x0003BF37
		internal void NotifyMasterFocusStatusChanged()
		{
			Action<MenuItem, bool> action = this.onFocusStatusChanged;
			if (action == null)
			{
				return;
			}
			action(this, this.Master.Focused);
		}

		// Token: 0x04000CA3 RID: 3235
		private Menu _master;

		// Token: 0x04000CA4 RID: 3236
		[SerializeField]
		private bool selectable = true;

		// Token: 0x04000CA5 RID: 3237
		private bool cacheSelected;

		// Token: 0x04000CA6 RID: 3238
		public Action<MenuItem> onSelected;

		// Token: 0x04000CA7 RID: 3239
		public Action<MenuItem> onDeselected;

		// Token: 0x04000CA8 RID: 3240
		public Action<MenuItem> onConfirmed;

		// Token: 0x04000CA9 RID: 3241
		public Action<MenuItem> onCanceled;

		// Token: 0x04000CAA RID: 3242
		public Action<MenuItem, bool> onFocusStatusChanged;
	}
}
