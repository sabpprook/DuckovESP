using System;
using ItemStatsSystem;
using UnityEngine;

// Token: 0x020000B6 RID: 182
[RequireComponent(typeof(Zone))]
public class ZoneDamage : MonoBehaviour
{
	// Token: 0x060005EE RID: 1518 RVA: 0x0001A621 File Offset: 0x00018821
	private void Start()
	{
		if (this.zone == null)
		{
			this.zone = base.GetComponent<Zone>();
		}
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x0001A640 File Offset: 0x00018840
	private void Update()
	{
		if (!LevelManager.LevelInited)
		{
			return;
		}
		this.timer += Time.deltaTime;
		if (this.timer > this.timeSpace)
		{
			this.timer %= this.timeSpace;
			this.Damage();
		}
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x0001A690 File Offset: 0x00018890
	private void Damage()
	{
		foreach (Health health in this.zone.Healths)
		{
			CharacterMainControl characterMainControl = health.TryGetCharacter();
			if (!(characterMainControl == null))
			{
				if (this.checkGasMask && characterMainControl.HasGasMask)
				{
					Item faceMaskItem = characterMainControl.GetFaceMaskItem();
					if (faceMaskItem && faceMaskItem.GetStat(this.hasMaskHash) != null)
					{
						faceMaskItem.Durability -= 0.1f * this.timeSpace;
					}
				}
				else if ((!this.checkElecProtection || characterMainControl.CharacterItem.GetStat(this.elecProtectionHash).Value <= 0.99f) && (!this.checkFireProtection || characterMainControl.CharacterItem.GetStat(this.fireProtectionHash).Value <= 0.99f))
				{
					this.damageInfo.fromCharacter = null;
					this.damageInfo.damagePoint = health.transform.position + Vector3.up * 0.5f;
					this.damageInfo.damageNormal = Vector3.up;
					health.Hurt(this.damageInfo);
				}
			}
		}
	}

	// Token: 0x04000574 RID: 1396
	public Zone zone;

	// Token: 0x04000575 RID: 1397
	public float timeSpace = 0.5f;

	// Token: 0x04000576 RID: 1398
	private float timer;

	// Token: 0x04000577 RID: 1399
	public DamageInfo damageInfo;

	// Token: 0x04000578 RID: 1400
	public bool checkGasMask;

	// Token: 0x04000579 RID: 1401
	public bool checkElecProtection;

	// Token: 0x0400057A RID: 1402
	public bool checkFireProtection;

	// Token: 0x0400057B RID: 1403
	private int hasMaskHash = "GasMask".GetHashCode();

	// Token: 0x0400057C RID: 1404
	private int elecProtectionHash = "ElecProtection".GetHashCode();

	// Token: 0x0400057D RID: 1405
	private int fireProtectionHash = "FireProtection".GetHashCode();
}
