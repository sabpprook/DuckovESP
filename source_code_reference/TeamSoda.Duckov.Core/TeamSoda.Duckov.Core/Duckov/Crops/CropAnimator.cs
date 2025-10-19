using System;
using UnityEngine;

namespace Duckov.Crops
{
	// Token: 0x020002E3 RID: 739
	public class CropAnimator : MonoBehaviour
	{
		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x060017B2 RID: 6066 RVA: 0x00056C9A File Offset: 0x00054E9A
		private ParticleSystem PlantFX
		{
			get
			{
				return this.plantFX;
			}
		}

		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x060017B3 RID: 6067 RVA: 0x00056CA2 File Offset: 0x00054EA2
		private ParticleSystem StageChangeFX
		{
			get
			{
				return this.stageChangeFX;
			}
		}

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x060017B4 RID: 6068 RVA: 0x00056CAA File Offset: 0x00054EAA
		private ParticleSystem RipenFX
		{
			get
			{
				return this.ripenFX;
			}
		}

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x060017B5 RID: 6069 RVA: 0x00056CB2 File Offset: 0x00054EB2
		private ParticleSystem WaterFX
		{
			get
			{
				return this.waterFX;
			}
		}

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x060017B6 RID: 6070 RVA: 0x00056CBA File Offset: 0x00054EBA
		private ParticleSystem HarvestFX
		{
			get
			{
				return this.harvestFX;
			}
		}

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x060017B7 RID: 6071 RVA: 0x00056CC2 File Offset: 0x00054EC2
		private ParticleSystem DestroyFX
		{
			get
			{
				return this.destroyFX;
			}
		}

		// Token: 0x060017B8 RID: 6072 RVA: 0x00056CCC File Offset: 0x00054ECC
		private void Awake()
		{
			if (this.crop == null)
			{
				this.crop = base.GetComponent<Crop>();
			}
			Crop crop = this.crop;
			crop.onPlant = (Action<Crop>)Delegate.Combine(crop.onPlant, new Action<Crop>(this.OnPlant));
			Crop crop2 = this.crop;
			crop2.onRipen = (Action<Crop>)Delegate.Combine(crop2.onRipen, new Action<Crop>(this.OnRipen));
			Crop crop3 = this.crop;
			crop3.onWater = (Action<Crop>)Delegate.Combine(crop3.onWater, new Action<Crop>(this.OnWater));
			Crop crop4 = this.crop;
			crop4.onHarvest = (Action<Crop>)Delegate.Combine(crop4.onHarvest, new Action<Crop>(this.OnHarvest));
			Crop crop5 = this.crop;
			crop5.onBeforeDestroy = (Action<Crop>)Delegate.Combine(crop5.onBeforeDestroy, new Action<Crop>(this.OnBeforeDestroy));
		}

		// Token: 0x060017B9 RID: 6073 RVA: 0x00056DB6 File Offset: 0x00054FB6
		private void Update()
		{
			this.RefreshPosition(true);
		}

		// Token: 0x060017BA RID: 6074 RVA: 0x00056DC0 File Offset: 0x00054FC0
		private void RefreshPosition(bool notifyStageChange = true)
		{
			float progress = this.crop.Progress;
			CropAnimator.Stage stage = default(CropAnimator.Stage);
			int? num = this.cachedStage;
			for (int i = 0; i < this.stages.Length; i++)
			{
				CropAnimator.Stage stage2 = this.stages[i];
				if (progress < this.stages[i].progress)
				{
					stage = stage2;
					this.cachedStage = new int?(i);
					break;
				}
			}
			this.displayParent.localPosition = Vector3.up * stage.position;
			if (!notifyStageChange)
			{
				return;
			}
			if (num == null)
			{
				return;
			}
			int value = num.Value;
			int? num2 = this.cachedStage;
			if (!((value == num2.GetValueOrDefault()) & (num2 != null)))
			{
				this.OnStageChange();
			}
		}

		// Token: 0x060017BB RID: 6075 RVA: 0x00056E7F File Offset: 0x0005507F
		private void OnStageChange()
		{
			FXPool.Play(this.StageChangeFX, base.transform.position, base.transform.rotation);
		}

		// Token: 0x060017BC RID: 6076 RVA: 0x00056EA3 File Offset: 0x000550A3
		private void OnWater(Crop crop)
		{
			FXPool.Play(this.WaterFX, base.transform.position, base.transform.rotation);
		}

		// Token: 0x060017BD RID: 6077 RVA: 0x00056EC7 File Offset: 0x000550C7
		private void OnRipen(Crop crop)
		{
			FXPool.Play(this.RipenFX, base.transform.position, base.transform.rotation);
		}

		// Token: 0x060017BE RID: 6078 RVA: 0x00056EEB File Offset: 0x000550EB
		private void OnHarvest(Crop crop)
		{
			FXPool.Play(this.HarvestFX, base.transform.position, base.transform.rotation);
		}

		// Token: 0x060017BF RID: 6079 RVA: 0x00056F0F File Offset: 0x0005510F
		private void OnPlant(Crop crop)
		{
			FXPool.Play(this.PlantFX, base.transform.position, base.transform.rotation);
		}

		// Token: 0x060017C0 RID: 6080 RVA: 0x00056F33 File Offset: 0x00055133
		private void OnBeforeDestroy(Crop crop)
		{
			FXPool.Play(this.DestroyFX, base.transform.position, base.transform.rotation);
		}

		// Token: 0x0400114B RID: 4427
		[SerializeField]
		private Crop crop;

		// Token: 0x0400114C RID: 4428
		[SerializeField]
		private Transform displayParent;

		// Token: 0x0400114D RID: 4429
		[SerializeField]
		private ParticleSystem plantFX;

		// Token: 0x0400114E RID: 4430
		[SerializeField]
		private ParticleSystem stageChangeFX;

		// Token: 0x0400114F RID: 4431
		[SerializeField]
		private ParticleSystem ripenFX;

		// Token: 0x04001150 RID: 4432
		[SerializeField]
		private ParticleSystem waterFX;

		// Token: 0x04001151 RID: 4433
		[SerializeField]
		private ParticleSystem harvestFX;

		// Token: 0x04001152 RID: 4434
		[SerializeField]
		private ParticleSystem destroyFX;

		// Token: 0x04001153 RID: 4435
		[SerializeField]
		private CropAnimator.Stage[] stages = new CropAnimator.Stage[]
		{
			new CropAnimator.Stage(0.333f, -0.4f),
			new CropAnimator.Stage(0.666f, -0.2f),
			new CropAnimator.Stage(0.999f, -0.1f)
		};

		// Token: 0x04001154 RID: 4436
		private int? cachedStage;

		// Token: 0x0200057B RID: 1403
		[Serializable]
		private struct Stage
		{
			// Token: 0x06002836 RID: 10294 RVA: 0x000946B6 File Offset: 0x000928B6
			public Stage(float progress, float position)
			{
				this.progress = progress;
				this.position = position;
			}

			// Token: 0x04001F99 RID: 8089
			public float progress;

			// Token: 0x04001F9A RID: 8090
			public float position;
		}
	}
}
