using System;
using System.Linq;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x020003A1 RID: 929
	public class HealthBarManager : MonoBehaviour
	{
		// Token: 0x1700065A RID: 1626
		// (get) Token: 0x06002130 RID: 8496 RVA: 0x00073EDE File Offset: 0x000720DE
		public static HealthBarManager Instance
		{
			get
			{
				return HealthBarManager._instance;
			}
		}

		// Token: 0x1700065B RID: 1627
		// (get) Token: 0x06002131 RID: 8497 RVA: 0x00073EE8 File Offset: 0x000720E8
		private PrefabPool<HealthBar> PrefabPool
		{
			get
			{
				if (this._prefabPool == null)
				{
					this._prefabPool = new PrefabPool<HealthBar>(this.healthBarPrefab, base.transform, null, null, null, true, 10, 10000, null);
				}
				return this._prefabPool;
			}
		}

		// Token: 0x06002132 RID: 8498 RVA: 0x00073F26 File Offset: 0x00072126
		private void Awake()
		{
			if (HealthBarManager._instance == null)
			{
				HealthBarManager._instance = this;
			}
			this.UnregisterStaticEvents();
			this.RegisterStaticEvents();
		}

		// Token: 0x06002133 RID: 8499 RVA: 0x00073F47 File Offset: 0x00072147
		private void OnDestroy()
		{
			this.UnregisterStaticEvents();
		}

		// Token: 0x06002134 RID: 8500 RVA: 0x00073F4F File Offset: 0x0007214F
		private void RegisterStaticEvents()
		{
			Health.OnRequestHealthBar += this.Health_OnRequestHealthBar;
		}

		// Token: 0x06002135 RID: 8501 RVA: 0x00073F62 File Offset: 0x00072162
		private void UnregisterStaticEvents()
		{
			Health.OnRequestHealthBar -= this.Health_OnRequestHealthBar;
		}

		// Token: 0x06002136 RID: 8502 RVA: 0x00073F78 File Offset: 0x00072178
		private HealthBar GetActiveHealthBar(Health health)
		{
			if (health == null)
			{
				return null;
			}
			return this.PrefabPool.ActiveEntries.FirstOrDefault((HealthBar e) => e.target == health);
		}

		// Token: 0x06002137 RID: 8503 RVA: 0x00073FC0 File Offset: 0x000721C0
		private HealthBar CreateHealthBarFor(Health health, DamageInfo? damage = null)
		{
			if (health == null)
			{
				return null;
			}
			if (this.PrefabPool.ActiveEntries.FirstOrDefault((HealthBar e) => e.target == health))
			{
				Debug.Log("Health bar for " + health.name + " already exists");
				return null;
			}
			HealthBar newBar = this.PrefabPool.Get(null);
			newBar.Setup(health, damage, delegate
			{
				this.PrefabPool.Release(newBar);
			});
			return newBar;
		}

		// Token: 0x06002138 RID: 8504 RVA: 0x0007406C File Offset: 0x0007226C
		private void Health_OnRequestHealthBar(Health health)
		{
			HealthBar activeHealthBar = this.GetActiveHealthBar(health);
			if (activeHealthBar != null)
			{
				activeHealthBar.RefreshOffset();
				return;
			}
			this.CreateHealthBarFor(health, null);
		}

		// Token: 0x06002139 RID: 8505 RVA: 0x000740A2 File Offset: 0x000722A2
		public static void RequestHealthBar(Health health, DamageInfo? damage = null)
		{
			if (HealthBarManager.Instance == null)
			{
				return;
			}
			HealthBarManager.Instance.CreateHealthBarFor(health, damage);
		}

		// Token: 0x04001691 RID: 5777
		private static HealthBarManager _instance;

		// Token: 0x04001692 RID: 5778
		[SerializeField]
		public HealthBar healthBarPrefab;

		// Token: 0x04001693 RID: 5779
		private PrefabPool<HealthBar> _prefabPool;
	}
}
