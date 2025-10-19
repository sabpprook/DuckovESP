using System;
using SodaCraft.Localizations;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x02000288 RID: 648
	public class GoldMinerArtifact : MiniGameBehaviour
	{
		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x060014E8 RID: 5352 RVA: 0x0004D9B9 File Offset: 0x0004BBB9
		// (set) Token: 0x060014E9 RID: 5353 RVA: 0x0004D9CB File Offset: 0x0004BBCB
		[LocalizationKey("Default")]
		private string displayNameKey
		{
			get
			{
				return "GoldMiner_" + this.id;
			}
			set
			{
			}
		}

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x060014EA RID: 5354 RVA: 0x0004D9CD File Offset: 0x0004BBCD
		// (set) Token: 0x060014EB RID: 5355 RVA: 0x0004D9E4 File Offset: 0x0004BBE4
		[LocalizationKey("Default")]
		private string descriptionKey
		{
			get
			{
				return "GoldMiner_" + this.id + "_Desc";
			}
			set
			{
			}
		}

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x060014EC RID: 5356 RVA: 0x0004D9E6 File Offset: 0x0004BBE6
		public bool AllowMultiple
		{
			get
			{
				return this.allowMultiple;
			}
		}

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x060014ED RID: 5357 RVA: 0x0004D9EE File Offset: 0x0004BBEE
		public string DisplayName
		{
			get
			{
				return this.displayNameKey.ToPlainText();
			}
		}

		// Token: 0x170003DB RID: 987
		// (get) Token: 0x060014EE RID: 5358 RVA: 0x0004D9FB File Offset: 0x0004BBFB
		public string Description
		{
			get
			{
				return this.descriptionKey.ToPlainText();
			}
		}

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x060014EF RID: 5359 RVA: 0x0004DA08 File Offset: 0x0004BC08
		public int Quality
		{
			get
			{
				return this.quality;
			}
		}

		// Token: 0x170003DD RID: 989
		// (get) Token: 0x060014F0 RID: 5360 RVA: 0x0004DA10 File Offset: 0x0004BC10
		public int BasePrice
		{
			get
			{
				return this.basePrice;
			}
		}

		// Token: 0x170003DE RID: 990
		// (get) Token: 0x060014F1 RID: 5361 RVA: 0x0004DA18 File Offset: 0x0004BC18
		public string ID
		{
			get
			{
				return this.id;
			}
		}

		// Token: 0x170003DF RID: 991
		// (get) Token: 0x060014F2 RID: 5362 RVA: 0x0004DA20 File Offset: 0x0004BC20
		public Sprite Icon
		{
			get
			{
				return this.icon;
			}
		}

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x060014F3 RID: 5363 RVA: 0x0004DA28 File Offset: 0x0004BC28
		public GoldMiner Master
		{
			get
			{
				return this.master;
			}
		}

		// Token: 0x060014F4 RID: 5364 RVA: 0x0004DA30 File Offset: 0x0004BC30
		public void Attach(GoldMiner master)
		{
			this.master = master;
			base.transform.SetParent(master.transform);
			Action<GoldMinerArtifact> onAttached = this.OnAttached;
			if (onAttached == null)
			{
				return;
			}
			onAttached(this);
		}

		// Token: 0x060014F5 RID: 5365 RVA: 0x0004DA5B File Offset: 0x0004BC5B
		public void Detatch(GoldMiner master)
		{
			Action<GoldMinerArtifact> onDetached = this.OnDetached;
			if (onDetached != null)
			{
				onDetached(this);
			}
			if (master != this.master)
			{
				Debug.LogError("Artifact is being notified detach by a different GoldMiner instance.", master.gameObject);
			}
			this.master = null;
		}

		// Token: 0x060014F6 RID: 5366 RVA: 0x0004DA94 File Offset: 0x0004BC94
		private void OnDestroy()
		{
			this.Detatch(this.master);
		}

		// Token: 0x04000F60 RID: 3936
		[SerializeField]
		private string id;

		// Token: 0x04000F61 RID: 3937
		[SerializeField]
		private Sprite icon;

		// Token: 0x04000F62 RID: 3938
		[SerializeField]
		private bool allowMultiple;

		// Token: 0x04000F63 RID: 3939
		[SerializeField]
		private int basePrice;

		// Token: 0x04000F64 RID: 3940
		[SerializeField]
		private int quality;

		// Token: 0x04000F65 RID: 3941
		private GoldMiner master;

		// Token: 0x04000F66 RID: 3942
		public Action<GoldMinerArtifact> OnAttached;

		// Token: 0x04000F67 RID: 3943
		public Action<GoldMinerArtifact> OnDetached;
	}
}
