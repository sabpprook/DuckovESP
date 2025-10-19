using System;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x020003A3 RID: 931
	public abstract class ManagedUIElement : MonoBehaviour
	{
		// Token: 0x17000661 RID: 1633
		// (get) Token: 0x0600214C RID: 8524 RVA: 0x000746FC File Offset: 0x000728FC
		// (set) Token: 0x0600214D RID: 8525 RVA: 0x00074704 File Offset: 0x00072904
		public bool open { get; private set; }

		// Token: 0x140000E4 RID: 228
		// (add) Token: 0x0600214E RID: 8526 RVA: 0x00074710 File Offset: 0x00072910
		// (remove) Token: 0x0600214F RID: 8527 RVA: 0x00074744 File Offset: 0x00072944
		public static event Action<ManagedUIElement> onOpen;

		// Token: 0x140000E5 RID: 229
		// (add) Token: 0x06002150 RID: 8528 RVA: 0x00074778 File Offset: 0x00072978
		// (remove) Token: 0x06002151 RID: 8529 RVA: 0x000747AC File Offset: 0x000729AC
		public static event Action<ManagedUIElement> onClose;

		// Token: 0x17000662 RID: 1634
		// (get) Token: 0x06002152 RID: 8530 RVA: 0x000747DF File Offset: 0x000729DF
		protected virtual bool ShowOpenCloseButtons
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06002153 RID: 8531 RVA: 0x000747E2 File Offset: 0x000729E2
		protected virtual void Awake()
		{
			this.RegisterEvents();
		}

		// Token: 0x06002154 RID: 8532 RVA: 0x000747EA File Offset: 0x000729EA
		protected virtual void OnDestroy()
		{
			this.UnregisterEvents();
			if (this.open)
			{
				this.Close();
			}
		}

		// Token: 0x06002155 RID: 8533 RVA: 0x00074800 File Offset: 0x00072A00
		public void Open(ManagedUIElement parent = null)
		{
			this.open = true;
			this.parent = parent;
			Action<ManagedUIElement> action = ManagedUIElement.onOpen;
			if (action != null)
			{
				action(this);
			}
			this.OnOpen();
		}

		// Token: 0x06002156 RID: 8534 RVA: 0x00074827 File Offset: 0x00072A27
		public void Close()
		{
			this.open = false;
			this.parent = null;
			Action<ManagedUIElement> action = ManagedUIElement.onClose;
			if (action != null)
			{
				action(this);
			}
			this.OnClose();
		}

		// Token: 0x06002157 RID: 8535 RVA: 0x0007484E File Offset: 0x00072A4E
		private void RegisterEvents()
		{
			ManagedUIElement.onOpen += this.OnManagedUIBehaviorOpen;
			ManagedUIElement.onClose += this.OnManagedUIBehaviorClose;
		}

		// Token: 0x06002158 RID: 8536 RVA: 0x00074872 File Offset: 0x00072A72
		private void UnregisterEvents()
		{
			ManagedUIElement.onOpen -= this.OnManagedUIBehaviorOpen;
			ManagedUIElement.onClose -= this.OnManagedUIBehaviorClose;
		}

		// Token: 0x06002159 RID: 8537 RVA: 0x00074896 File Offset: 0x00072A96
		private void OnManagedUIBehaviorClose(ManagedUIElement obj)
		{
			if (obj != null && obj == this.parent)
			{
				this.Close();
			}
		}

		// Token: 0x0600215A RID: 8538 RVA: 0x000748B5 File Offset: 0x00072AB5
		private void OnManagedUIBehaviorOpen(ManagedUIElement obj)
		{
		}

		// Token: 0x0600215B RID: 8539 RVA: 0x000748B7 File Offset: 0x00072AB7
		protected virtual void OnOpen()
		{
		}

		// Token: 0x0600215C RID: 8540 RVA: 0x000748B9 File Offset: 0x00072AB9
		protected virtual void OnClose()
		{
		}

		// Token: 0x0400169B RID: 5787
		[SerializeField]
		private ManagedUIElement parent;
	}
}
