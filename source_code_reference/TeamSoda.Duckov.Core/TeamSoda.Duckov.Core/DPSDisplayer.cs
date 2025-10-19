using System;
using TMPro;
using UnityEngine;

// Token: 0x02000178 RID: 376
public class DPSDisplayer : MonoBehaviour
{
	// Token: 0x06000B68 RID: 2920 RVA: 0x00030515 File Offset: 0x0002E715
	private void Awake()
	{
		Health.OnHurt += this.OnHurt;
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x00030528 File Offset: 0x0002E728
	private void Update()
	{
		if (Time.time - this.lastTimeMarker > 3f)
		{
			this.empty = true;
			this.totalDamage = 0f;
			this.RefreshDisplay();
		}
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x00030555 File Offset: 0x0002E755
	private void OnDestroy()
	{
		Health.OnHurt -= this.OnHurt;
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x00030568 File Offset: 0x0002E768
	private void OnHurt(Health health, DamageInfo dmgInfo)
	{
		if (!dmgInfo.fromCharacter || !dmgInfo.fromCharacter.IsMainCharacter)
		{
			return;
		}
		this.totalDamage += dmgInfo.finalDamage;
		if (this.empty)
		{
			this.firstTimeMarker = Time.time;
			this.lastTimeMarker = Time.time;
			this.empty = false;
			return;
		}
		this.lastTimeMarker = Time.time;
		this.RefreshDisplay();
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x000305DC File Offset: 0x0002E7DC
	private void RefreshDisplay()
	{
		float num = this.CalculateDPS();
		this.dpsText.text = num.ToString("00000");
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x00030608 File Offset: 0x0002E808
	private float CalculateDPS()
	{
		if (this.empty)
		{
			return 0f;
		}
		float num = this.lastTimeMarker - this.firstTimeMarker;
		if (num <= 0f)
		{
			return 0f;
		}
		return this.totalDamage / num;
	}

	// Token: 0x040009B9 RID: 2489
	[SerializeField]
	private TextMeshPro dpsText;

	// Token: 0x040009BA RID: 2490
	private bool empty;

	// Token: 0x040009BB RID: 2491
	private float totalDamage;

	// Token: 0x040009BC RID: 2492
	private float firstTimeMarker;

	// Token: 0x040009BD RID: 2493
	private float lastTimeMarker;
}
