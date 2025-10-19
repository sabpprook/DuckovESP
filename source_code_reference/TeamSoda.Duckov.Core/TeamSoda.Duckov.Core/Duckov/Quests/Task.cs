using System;
using Saves;
using UnityEngine;

namespace Duckov.Quests
{
	// Token: 0x02000337 RID: 823
	[Serializable]
	public abstract class Task : MonoBehaviour, ISaveDataProvider
	{
		// Token: 0x17000548 RID: 1352
		// (get) Token: 0x06001C45 RID: 7237 RVA: 0x0006624B File Offset: 0x0006444B
		// (set) Token: 0x06001C46 RID: 7238 RVA: 0x00066253 File Offset: 0x00064453
		public Quest Master
		{
			get
			{
				return this.master;
			}
			internal set
			{
				this.master = value;
			}
		}

		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x06001C47 RID: 7239 RVA: 0x0006625C File Offset: 0x0006445C
		// (set) Token: 0x06001C48 RID: 7240 RVA: 0x00066264 File Offset: 0x00064464
		public int ID
		{
			get
			{
				return this.id;
			}
			internal set
			{
				this.id = value;
			}
		}

		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x06001C49 RID: 7241 RVA: 0x0006626D File Offset: 0x0006446D
		public virtual string Description
		{
			get
			{
				return "未定义Task描述。";
			}
		}

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x06001C4A RID: 7242 RVA: 0x00066274 File Offset: 0x00064474
		public virtual string[] ExtraDescriptsions
		{
			get
			{
				return new string[0];
			}
		}

		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x06001C4B RID: 7243 RVA: 0x0006627C File Offset: 0x0006447C
		public virtual Sprite Icon
		{
			get
			{
				return null;
			}
		}

		// Token: 0x140000CA RID: 202
		// (add) Token: 0x06001C4C RID: 7244 RVA: 0x00066280 File Offset: 0x00064480
		// (remove) Token: 0x06001C4D RID: 7245 RVA: 0x000662B8 File Offset: 0x000644B8
		public event Action<Task> onStatusChanged;

		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x06001C4E RID: 7246 RVA: 0x000662ED File Offset: 0x000644ED
		public virtual bool Interactable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700054E RID: 1358
		// (get) Token: 0x06001C4F RID: 7247 RVA: 0x000662F0 File Offset: 0x000644F0
		public virtual bool PossibleValidInteraction
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700054F RID: 1359
		// (get) Token: 0x06001C50 RID: 7248 RVA: 0x000662F3 File Offset: 0x000644F3
		public virtual bool NeedInspection
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x06001C51 RID: 7249 RVA: 0x000662F6 File Offset: 0x000644F6
		public virtual string InteractText
		{
			get
			{
				return "交互";
			}
		}

		// Token: 0x06001C52 RID: 7250 RVA: 0x000662FD File Offset: 0x000644FD
		public virtual void Interact()
		{
			Debug.LogWarning(string.Format("{0}可能未定义交互行为", base.GetType()));
		}

		// Token: 0x06001C53 RID: 7251 RVA: 0x00066314 File Offset: 0x00064514
		public bool IsFinished()
		{
			return this.forceFinish || this.CheckFinished();
		}

		// Token: 0x06001C54 RID: 7252
		protected abstract bool CheckFinished();

		// Token: 0x06001C55 RID: 7253
		public abstract object GenerateSaveData();

		// Token: 0x06001C56 RID: 7254
		public abstract void SetupSaveData(object data);

		// Token: 0x06001C57 RID: 7255 RVA: 0x00066326 File Offset: 0x00064526
		protected void ReportStatusChanged()
		{
			Action<Task> action = this.onStatusChanged;
			if (action != null)
			{
				action(this);
			}
			if (this.IsFinished())
			{
				Quest quest = this.Master;
				if (quest == null)
				{
					return;
				}
				quest.NotifyTaskFinished(this);
			}
		}

		// Token: 0x06001C58 RID: 7256 RVA: 0x00066353 File Offset: 0x00064553
		internal void Init()
		{
			if (this.IsFinished())
			{
				base.enabled = false;
				return;
			}
			this.OnInit();
		}

		// Token: 0x06001C59 RID: 7257 RVA: 0x0006636B File Offset: 0x0006456B
		protected virtual void OnInit()
		{
		}

		// Token: 0x06001C5A RID: 7258 RVA: 0x0006636D File Offset: 0x0006456D
		internal void ForceFinish()
		{
			this.forceFinish = true;
			Action<Task> action = this.onStatusChanged;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x040013C2 RID: 5058
		[SerializeField]
		private Quest master;

		// Token: 0x040013C3 RID: 5059
		[SerializeField]
		private int id;

		// Token: 0x040013C5 RID: 5061
		[SerializeField]
		private bool forceFinish;
	}
}
