using System;
using Saves;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Duckov.Quests
{
	// Token: 0x02000335 RID: 821
	public abstract class Reward : MonoBehaviour, ISelfValidator, ISaveDataProvider
	{
		// Token: 0x140000C8 RID: 200
		// (add) Token: 0x06001C1E RID: 7198 RVA: 0x00065E54 File Offset: 0x00064054
		// (remove) Token: 0x06001C1F RID: 7199 RVA: 0x00065E88 File Offset: 0x00064088
		public static event Action<Reward> OnRewardClaimed;

		// Token: 0x1700053B RID: 1339
		// (get) Token: 0x06001C20 RID: 7200 RVA: 0x00065EBB File Offset: 0x000640BB
		// (set) Token: 0x06001C21 RID: 7201 RVA: 0x00065EC3 File Offset: 0x000640C3
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

		// Token: 0x140000C9 RID: 201
		// (add) Token: 0x06001C22 RID: 7202 RVA: 0x00065ECC File Offset: 0x000640CC
		// (remove) Token: 0x06001C23 RID: 7203 RVA: 0x00065F04 File Offset: 0x00064104
		internal event Action onStatusChanged;

		// Token: 0x1700053C RID: 1340
		// (get) Token: 0x06001C24 RID: 7204 RVA: 0x00065F39 File Offset: 0x00064139
		public bool Claimable
		{
			get
			{
				return this.master.Complete;
			}
		}

		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x06001C25 RID: 7205 RVA: 0x00065F46 File Offset: 0x00064146
		public virtual Sprite Icon
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x06001C26 RID: 7206 RVA: 0x00065F49 File Offset: 0x00064149
		public virtual string Description
		{
			get
			{
				return "未定义奖励描述";
			}
		}

		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x06001C27 RID: 7207
		public abstract bool Claimed { get; }

		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x06001C28 RID: 7208 RVA: 0x00065F50 File Offset: 0x00064150
		public virtual bool Claiming { get; }

		// Token: 0x17000541 RID: 1345
		// (get) Token: 0x06001C29 RID: 7209 RVA: 0x00065F58 File Offset: 0x00064158
		public virtual bool AutoClaim
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x06001C2A RID: 7210 RVA: 0x00065F5B File Offset: 0x0006415B
		// (set) Token: 0x06001C2B RID: 7211 RVA: 0x00065F63 File Offset: 0x00064163
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

		// Token: 0x06001C2C RID: 7212 RVA: 0x00065F6C File Offset: 0x0006416C
		public void Claim()
		{
			if (!this.Claimable || this.Claimed)
			{
				return;
			}
			this.OnClaim();
			this.Master.NotifyRewardClaimed(this);
			Action<Reward> onRewardClaimed = Reward.OnRewardClaimed;
			if (onRewardClaimed == null)
			{
				return;
			}
			onRewardClaimed(this);
		}

		// Token: 0x06001C2D RID: 7213
		public abstract void OnClaim();

		// Token: 0x06001C2E RID: 7214 RVA: 0x00065FA4 File Offset: 0x000641A4
		public virtual void Validate(SelfValidationResult result)
		{
			if (this.master == null)
			{
				result.AddWarning("Reward需要master(Quest)。").WithFix("设为父物体中的Quest。", delegate
				{
					this.master = base.GetComponent<Quest>();
					if (this.master == null)
					{
						this.master = base.GetComponentInParent<Quest>();
					}
				}, true);
			}
			if (this.master != null)
			{
				if (base.transform != this.master.transform && !base.transform.IsChildOf(this.master.transform))
				{
					result.AddError("Task需要存在于master子物体中。").WithFix("设为master子物体", delegate
					{
						base.transform.SetParent(this.master.transform);
					}, true);
				}
				if (!this.master.rewards.Contains(this))
				{
					result.AddError("Master的Task列表中不包含本物体。").WithFix("将本物体添加至master的Task列表中", delegate
					{
						this.master.rewards.Add(this);
					}, true);
				}
			}
		}

		// Token: 0x06001C2F RID: 7215
		public abstract object GenerateSaveData();

		// Token: 0x06001C30 RID: 7216
		public abstract void SetupSaveData(object data);

		// Token: 0x06001C31 RID: 7217 RVA: 0x0006607C File Offset: 0x0006427C
		private void Awake()
		{
			this.Master.onStatusChanged += this.OnMasterStatusChanged;
		}

		// Token: 0x06001C32 RID: 7218 RVA: 0x00066095 File Offset: 0x00064295
		private void OnDestroy()
		{
			this.Master.onStatusChanged -= this.OnMasterStatusChanged;
		}

		// Token: 0x06001C33 RID: 7219 RVA: 0x000660AE File Offset: 0x000642AE
		public void OnMasterStatusChanged(Quest quest)
		{
			Action action = this.onStatusChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06001C34 RID: 7220 RVA: 0x000660C0 File Offset: 0x000642C0
		protected void ReportStatusChanged()
		{
			Action action = this.onStatusChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06001C35 RID: 7221 RVA: 0x000660D2 File Offset: 0x000642D2
		public virtual void NotifyReload(Quest questInstance)
		{
		}

		// Token: 0x040013B9 RID: 5049
		[SerializeField]
		private int id;

		// Token: 0x040013BA RID: 5050
		[SerializeField]
		private Quest master;
	}
}
