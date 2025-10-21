using System;
using Duckov.UI;
using Duckov.UI.Animations;
using TMPro;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x020002A1 RID: 673
	public class LevelSettlementUI : MonoBehaviour
	{
		// Token: 0x060015DA RID: 5594 RVA: 0x00050CD1 File Offset: 0x0004EED1
		internal void Reset()
		{
			this.clearIndicator.SetActive(false);
			this.failIndicator.SetActive(false);
			this.money = 0;
			this.score = 0;
			this.factor = 0f;
			this.RefreshTexts();
		}

		// Token: 0x060015DB RID: 5595 RVA: 0x00050D0A File Offset: 0x0004EF0A
		public void SetTargetScore(int targetScore)
		{
			this.targetScore = targetScore;
			this.RefreshTexts();
		}

		// Token: 0x060015DC RID: 5596 RVA: 0x00050D19 File Offset: 0x0004EF19
		public void StepResolveEntity(GoldMinerEntity entity)
		{
		}

		// Token: 0x060015DD RID: 5597 RVA: 0x00050D1B File Offset: 0x0004EF1B
		public void StepResult(bool clear)
		{
			this.clearIndicator.SetActive(clear);
			this.failIndicator.SetActive(!clear);
		}

		// Token: 0x060015DE RID: 5598 RVA: 0x00050D38 File Offset: 0x0004EF38
		public void Step(int money, float factor, int score)
		{
			bool flag = money > this.money;
			bool flag2 = factor > this.factor;
			bool flag3 = score > this.score;
			this.money = money;
			this.factor = factor;
			this.score = score;
			this.RefreshTexts();
			if (flag)
			{
				this.moneyPunch.Punch();
			}
			if (flag2)
			{
				this.factorPunch.Punch();
			}
			if (flag3)
			{
				this.scorePunch.Punch();
			}
		}

		// Token: 0x060015DF RID: 5599 RVA: 0x00050DA8 File Offset: 0x0004EFA8
		private void RefreshTexts()
		{
			this.levelText.text = string.Format("LEVEL {0}", this.goldMiner.run.level + 1);
			this.targetScoreText.text = string.Format("{0}", this.targetScore);
			this.moneyText.text = string.Format("${0}", this.money);
			this.factorText.text = string.Format("{0}", this.factor);
			this.scoreText.text = string.Format("{0}", this.score);
		}

		// Token: 0x060015E0 RID: 5600 RVA: 0x00050E61 File Offset: 0x0004F061
		public void Show()
		{
			this.fadeGroup.Show();
		}

		// Token: 0x060015E1 RID: 5601 RVA: 0x00050E6E File Offset: 0x0004F06E
		public void Hide()
		{
			this.fadeGroup.Hide();
		}

		// Token: 0x04001037 RID: 4151
		[SerializeField]
		private GoldMiner goldMiner;

		// Token: 0x04001038 RID: 4152
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x04001039 RID: 4153
		[SerializeField]
		private PunchReceiver moneyPunch;

		// Token: 0x0400103A RID: 4154
		[SerializeField]
		private PunchReceiver factorPunch;

		// Token: 0x0400103B RID: 4155
		[SerializeField]
		private PunchReceiver scorePunch;

		// Token: 0x0400103C RID: 4156
		[SerializeField]
		private TextMeshProUGUI moneyText;

		// Token: 0x0400103D RID: 4157
		[SerializeField]
		private TextMeshProUGUI factorText;

		// Token: 0x0400103E RID: 4158
		[SerializeField]
		private TextMeshProUGUI scoreText;

		// Token: 0x0400103F RID: 4159
		[SerializeField]
		private TextMeshProUGUI levelText;

		// Token: 0x04001040 RID: 4160
		[SerializeField]
		private TextMeshProUGUI targetScoreText;

		// Token: 0x04001041 RID: 4161
		[SerializeField]
		private GameObject clearIndicator;

		// Token: 0x04001042 RID: 4162
		[SerializeField]
		private GameObject failIndicator;

		// Token: 0x04001043 RID: 4163
		private int targetScore;

		// Token: 0x04001044 RID: 4164
		private int money;

		// Token: 0x04001045 RID: 4165
		private int score;

		// Token: 0x04001046 RID: 4166
		private float factor;
	}
}
