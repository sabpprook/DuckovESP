using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Duckov.Buffs
{
	// Token: 0x020003FA RID: 1018
	public class CharacterBuffManager : MonoBehaviour
	{
		// Token: 0x17000725 RID: 1829
		// (get) Token: 0x060024F7 RID: 9463 RVA: 0x0007F903 File Offset: 0x0007DB03
		public CharacterMainControl Master
		{
			get
			{
				return this.master;
			}
		}

		// Token: 0x17000726 RID: 1830
		// (get) Token: 0x060024F8 RID: 9464 RVA: 0x0007F90B File Offset: 0x0007DB0B
		public ReadOnlyCollection<Buff> Buffs
		{
			get
			{
				if (this._readOnlyBuffsCollection == null)
				{
					this._readOnlyBuffsCollection = new ReadOnlyCollection<Buff>(this.buffs);
				}
				return this._readOnlyBuffsCollection;
			}
		}

		// Token: 0x140000F4 RID: 244
		// (add) Token: 0x060024F9 RID: 9465 RVA: 0x0007F92C File Offset: 0x0007DB2C
		// (remove) Token: 0x060024FA RID: 9466 RVA: 0x0007F964 File Offset: 0x0007DB64
		public event Action<CharacterBuffManager, Buff> onAddBuff;

		// Token: 0x140000F5 RID: 245
		// (add) Token: 0x060024FB RID: 9467 RVA: 0x0007F99C File Offset: 0x0007DB9C
		// (remove) Token: 0x060024FC RID: 9468 RVA: 0x0007F9D4 File Offset: 0x0007DBD4
		public event Action<CharacterBuffManager, Buff> onRemoveBuff;

		// Token: 0x060024FD RID: 9469 RVA: 0x0007FA09 File Offset: 0x0007DC09
		private void Awake()
		{
			if (this.master == null)
			{
				this.master = base.GetComponent<CharacterMainControl>();
			}
		}

		// Token: 0x060024FE RID: 9470 RVA: 0x0007FA28 File Offset: 0x0007DC28
		public void AddBuff(Buff buffPrefab, CharacterMainControl fromWho, int overrideWeaponID = 0)
		{
			if (buffPrefab == null)
			{
				return;
			}
			Buff buff = this.buffs.Find((Buff e) => e.ID == buffPrefab.ID);
			if (buff)
			{
				buff.NotifyIncomingBuffWithSameID(buffPrefab);
				return;
			}
			Buff buff2 = global::UnityEngine.Object.Instantiate<Buff>(buffPrefab);
			buff2.Setup(this);
			buff2.fromWho = fromWho;
			if (overrideWeaponID > 0)
			{
				buff2.fromWeaponID = overrideWeaponID;
			}
			this.buffs.Add(buff2);
			Action<CharacterBuffManager, Buff> action = this.onAddBuff;
			if (action == null)
			{
				return;
			}
			action(this, buff2);
		}

		// Token: 0x060024FF RID: 9471 RVA: 0x0007FAC4 File Offset: 0x0007DCC4
		public void RemoveBuff(int buffID, bool removeOneLayer)
		{
			Buff buff = this.buffs.Find((Buff e) => e.ID == buffID);
			if (buff != null)
			{
				this.RemoveBuff(buff, removeOneLayer);
			}
		}

		// Token: 0x06002500 RID: 9472 RVA: 0x0007FB08 File Offset: 0x0007DD08
		public void RemoveBuffsByTag(Buff.BuffExclusiveTags buffTag, bool removeOneLayer)
		{
			if (buffTag == Buff.BuffExclusiveTags.NotExclusive)
			{
				return;
			}
			foreach (Buff buff in this.buffs.FindAll((Buff e) => e.ExclusiveTag == buffTag))
			{
				if (buff != null)
				{
					this.RemoveBuff(buff, removeOneLayer);
				}
			}
		}

		// Token: 0x06002501 RID: 9473 RVA: 0x0007FB8C File Offset: 0x0007DD8C
		public bool HasBuff(int buffID)
		{
			return this.buffs.Find((Buff e) => e.ID == buffID) != null;
		}

		// Token: 0x06002502 RID: 9474 RVA: 0x0007FBC4 File Offset: 0x0007DDC4
		public Buff GetBuffByTag(Buff.BuffExclusiveTags tag)
		{
			if (tag == Buff.BuffExclusiveTags.NotExclusive)
			{
				return null;
			}
			return this.buffs.Find((Buff e) => e.ExclusiveTag == tag);
		}

		// Token: 0x06002503 RID: 9475 RVA: 0x0007FC00 File Offset: 0x0007DE00
		public void RemoveBuff(Buff toRemove, bool oneLayer)
		{
			if (oneLayer && toRemove.CurrentLayers > 1)
			{
				toRemove.CurrentLayers--;
				if (toRemove.CurrentLayers >= 1)
				{
					return;
				}
			}
			if (this.buffs.Remove(toRemove))
			{
				Action<CharacterBuffManager, Buff> action = this.onRemoveBuff;
				if (action != null)
				{
					action(this, toRemove);
				}
				global::UnityEngine.Object.Destroy(toRemove.gameObject);
			}
		}

		// Token: 0x06002504 RID: 9476 RVA: 0x0007FC60 File Offset: 0x0007DE60
		private void Update()
		{
			bool flag = false;
			foreach (Buff buff in this.buffs)
			{
				if (buff == null)
				{
					flag = true;
				}
				else if (buff.IsOutOfTime)
				{
					buff.NotifyOutOfTime();
					this.outOfTimeBuffsBuffer.Add(buff);
				}
				else
				{
					buff.NotifyUpdate();
				}
			}
			if (this.outOfTimeBuffsBuffer.Count > 0)
			{
				foreach (Buff buff2 in this.outOfTimeBuffsBuffer)
				{
					if (buff2 != null)
					{
						this.RemoveBuff(buff2, false);
					}
				}
				this.outOfTimeBuffsBuffer.Clear();
			}
			if (flag)
			{
				this.buffs.RemoveAll((Buff e) => e == null);
			}
		}

		// Token: 0x04001939 RID: 6457
		[SerializeField]
		private CharacterMainControl master;

		// Token: 0x0400193A RID: 6458
		private List<Buff> buffs = new List<Buff>();

		// Token: 0x0400193B RID: 6459
		private ReadOnlyCollection<Buff> _readOnlyBuffsCollection;

		// Token: 0x0400193E RID: 6462
		private List<Buff> outOfTimeBuffsBuffer = new List<Buff>();
	}
}
