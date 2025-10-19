using System;
using TMPro;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x020002A6 RID: 678
	public class ScoreDisplay : MonoBehaviour
	{
		// Token: 0x06001610 RID: 5648 RVA: 0x00051644 File Offset: 0x0004F844
		private void Awake()
		{
			GoldMiner goldMiner = this.master;
			goldMiner.onLevelBegin = (Action<GoldMiner>)Delegate.Combine(goldMiner.onLevelBegin, new Action<GoldMiner>(this.OnLevelBegin));
			GoldMiner goldMiner2 = this.master;
			goldMiner2.onAfterResolveEntity = (Action<GoldMiner, GoldMinerEntity>)Delegate.Combine(goldMiner2.onAfterResolveEntity, new Action<GoldMiner, GoldMinerEntity>(this.OnAfterResolveEntity));
		}

		// Token: 0x06001611 RID: 5649 RVA: 0x0005169F File Offset: 0x0004F89F
		private void OnAfterResolveEntity(GoldMiner miner, GoldMinerEntity entity)
		{
			this.Refresh();
		}

		// Token: 0x06001612 RID: 5650 RVA: 0x000516A7 File Offset: 0x0004F8A7
		private void OnLevelBegin(GoldMiner miner)
		{
			this.Refresh();
		}

		// Token: 0x06001613 RID: 5651 RVA: 0x000516B0 File Offset: 0x0004F8B0
		private void Refresh()
		{
			GoldMinerRunData run = this.master.run;
			if (run == null)
			{
				return;
			}
			int num = 0;
			float num2 = run.scoreFactorBase.Value + run.levelScoreFactor;
			int targetScore = run.targetScore;
			foreach (GoldMinerEntity goldMinerEntity in this.master.resolvedEntities)
			{
				int num3 = Mathf.CeilToInt((float)goldMinerEntity.Value * run.charm.Value);
				if (num3 != 0)
				{
					num += num3;
				}
			}
			this.moneyText.text = string.Format("${0}", num);
			this.factorText.text = string.Format("{0}", num2);
			this.scoreText.text = string.Format("{0}", Mathf.CeilToInt((float)num * num2));
			this.targetScoreText.text = string.Format("{0}", targetScore);
		}

		// Token: 0x0400105F RID: 4191
		[SerializeField]
		private GoldMiner master;

		// Token: 0x04001060 RID: 4192
		[SerializeField]
		private TextMeshProUGUI moneyText;

		// Token: 0x04001061 RID: 4193
		[SerializeField]
		private TextMeshProUGUI factorText;

		// Token: 0x04001062 RID: 4194
		[SerializeField]
		private TextMeshProUGUI scoreText;

		// Token: 0x04001063 RID: 4195
		[SerializeField]
		private TextMeshProUGUI targetScoreText;
	}
}
