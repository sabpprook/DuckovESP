using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x020003BD RID: 957
	public abstract class View : ManagedUIElement
	{
		// Token: 0x1700069C RID: 1692
		// (get) Token: 0x060022C3 RID: 8899 RVA: 0x00079C9D File Offset: 0x00077E9D
		// (set) Token: 0x060022C4 RID: 8900 RVA: 0x00079CA4 File Offset: 0x00077EA4
		public static View ActiveView
		{
			get
			{
				return View._activeView;
			}
			private set
			{
				global::UnityEngine.Object activeView = View._activeView;
				View._activeView = value;
				if (activeView != View._activeView)
				{
					Action onActiveViewChanged = View.OnActiveViewChanged;
					if (onActiveViewChanged == null)
					{
						return;
					}
					onActiveViewChanged();
				}
			}
		}

		// Token: 0x140000EA RID: 234
		// (add) Token: 0x060022C5 RID: 8901 RVA: 0x00079CCC File Offset: 0x00077ECC
		// (remove) Token: 0x060022C6 RID: 8902 RVA: 0x00079D00 File Offset: 0x00077F00
		public static event Action OnActiveViewChanged;

		// Token: 0x060022C7 RID: 8903 RVA: 0x00079D34 File Offset: 0x00077F34
		protected override void Awake()
		{
			base.Awake();
			if (this.exitButton != null)
			{
				this.exitButton.onClick.AddListener(new UnityAction(base.Close));
			}
			UIInputManager.OnNavigate += this.OnNavigate;
			UIInputManager.OnConfirm += this.OnConfirm;
			UIInputManager.OnCancel += this.OnCancel;
			this.viewTabs = base.transform.parent.parent.GetComponent<ViewTabs>();
			if (this.autoClose)
			{
				base.Close();
			}
		}

		// Token: 0x060022C8 RID: 8904 RVA: 0x00079DCD File Offset: 0x00077FCD
		protected override void OnDestroy()
		{
			base.OnDestroy();
			UIInputManager.OnNavigate -= this.OnNavigate;
			UIInputManager.OnConfirm -= this.OnConfirm;
			UIInputManager.OnCancel -= this.OnCancel;
		}

		// Token: 0x060022C9 RID: 8905 RVA: 0x00079E08 File Offset: 0x00078008
		protected override void OnOpen()
		{
			this.autoClose = false;
			if (View.ActiveView != null && View.ActiveView != this)
			{
				View.ActiveView.Close();
			}
			View.ActiveView = this;
			ItemUIUtilities.Select(null);
			if (this.viewTabs != null)
			{
				this.viewTabs.Show();
			}
			if (base.gameObject == null)
			{
				Debug.LogError("GameObject不存在", base.gameObject);
			}
			InputManager.DisableInput(base.gameObject);
			AudioManager.Post(this.sfx_Open);
		}

		// Token: 0x060022CA RID: 8906 RVA: 0x00079E9A File Offset: 0x0007809A
		protected override void OnClose()
		{
			if (View.ActiveView == this)
			{
				View.ActiveView = null;
			}
			InputManager.ActiveInput(base.gameObject);
			AudioManager.Post(this.sfx_Close);
		}

		// Token: 0x060022CB RID: 8907 RVA: 0x00079EC6 File Offset: 0x000780C6
		internal virtual void TryQuit()
		{
			base.Close();
		}

		// Token: 0x060022CC RID: 8908 RVA: 0x00079ECE File Offset: 0x000780CE
		public void OnNavigate(UIInputEventData eventData)
		{
			if (eventData.Used)
			{
				return;
			}
			if (View.ActiveView != this)
			{
				return;
			}
			this.OnNavigate(eventData.vector);
		}

		// Token: 0x060022CD RID: 8909 RVA: 0x00079EF3 File Offset: 0x000780F3
		public void OnConfirm(UIInputEventData eventData)
		{
			if (eventData.Used)
			{
				return;
			}
			if (View.ActiveView != this)
			{
				return;
			}
			this.OnConfirm();
		}

		// Token: 0x060022CE RID: 8910 RVA: 0x00079F12 File Offset: 0x00078112
		public void OnCancel(UIInputEventData eventData)
		{
			if (eventData.Used)
			{
				return;
			}
			if (View.ActiveView == null || View.ActiveView != this)
			{
				return;
			}
			this.OnCancel();
			if (!eventData.Used)
			{
				this.TryQuit();
				eventData.Use();
			}
		}

		// Token: 0x060022CF RID: 8911 RVA: 0x00079F52 File Offset: 0x00078152
		protected virtual void OnNavigate(Vector2 vector)
		{
		}

		// Token: 0x060022D0 RID: 8912 RVA: 0x00079F54 File Offset: 0x00078154
		protected virtual void OnConfirm()
		{
		}

		// Token: 0x060022D1 RID: 8913 RVA: 0x00079F56 File Offset: 0x00078156
		protected virtual void OnCancel()
		{
		}

		// Token: 0x060022D2 RID: 8914 RVA: 0x00079F58 File Offset: 0x00078158
		protected static T GetViewInstance<T>() where T : View
		{
			return GameplayUIManager.GetViewInstance<T>();
		}

		// Token: 0x040017A9 RID: 6057
		[HideInInspector]
		private static View _activeView;

		// Token: 0x040017AB RID: 6059
		[SerializeField]
		private ViewTabs viewTabs;

		// Token: 0x040017AC RID: 6060
		[SerializeField]
		private Button exitButton;

		// Token: 0x040017AD RID: 6061
		[SerializeField]
		private string sfx_Open;

		// Token: 0x040017AE RID: 6062
		[SerializeField]
		private string sfx_Close;

		// Token: 0x040017AF RID: 6063
		private bool autoClose = true;
	}
}
