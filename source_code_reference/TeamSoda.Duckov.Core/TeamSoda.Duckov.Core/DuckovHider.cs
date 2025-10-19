using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FOW;

// Token: 0x02000179 RID: 377
public class DuckovHider : HiderBehavior
{
	// Token: 0x06000B6F RID: 2927 RVA: 0x0003064F File Offset: 0x0002E84F
	protected override void Awake()
	{
		base.Awake();
		LevelManager.OnMainCharacterDead += this.OnMainCharacterDie;
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x00030668 File Offset: 0x0002E868
	private void OnDestroy()
	{
		LevelManager.OnMainCharacterDead -= this.OnMainCharacterDie;
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x0003067B File Offset: 0x0002E87B
	protected override void OnHide()
	{
		if (!LevelManager.Instance || !LevelManager.Instance.IsRaidMap || this.mainCharacterDied)
		{
			return;
		}
		this.targetHide = true;
		this.HideDelay();
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x000306AC File Offset: 0x0002E8AC
	protected override void OnReveal()
	{
		this.targetHide = false;
		this.character.Show();
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x000306C0 File Offset: 0x0002E8C0
	private async UniTask HideDelay()
	{
		await UniTask.WaitForSeconds(this.hideDelay, false, PlayerLoopTiming.Update, default(CancellationToken), false);
		if (this.targetHide)
		{
			if (this.character != null)
			{
				this.character.Hide();
			}
		}
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x00030703 File Offset: 0x0002E903
	private void OnMainCharacterDie(DamageInfo damageInfo)
	{
		this.mainCharacterDied = true;
		this.OnReveal();
	}

	// Token: 0x040009BE RID: 2494
	public CharacterMainControl character;

	// Token: 0x040009BF RID: 2495
	private float hideDelay = 0.2f;

	// Token: 0x040009C0 RID: 2496
	private bool targetHide;

	// Token: 0x040009C1 RID: 2497
	private bool mainCharacterDied;
}
