using System;
using UnityEngine;

namespace Duckov.MiniGames.BubblePoppers
{
	// Token: 0x020002D9 RID: 729
	public class BubblePopperLevelDataProvider : MonoBehaviour
	{
		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x06001740 RID: 5952 RVA: 0x000557D0 File Offset: 0x000539D0
		public int TotalLevels
		{
			get
			{
				return this.totalLevels;
			}
		}

		// Token: 0x06001741 RID: 5953 RVA: 0x000557D8 File Offset: 0x000539D8
		internal int[] GetData(int levelIndex)
		{
			int num = this.seed + levelIndex;
			int[] array = new int[60 + 10 * (levelIndex / 2)];
			global::System.Random random = new global::System.Random(num);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = random.Next(0, this.master.AvaliableColorCount);
			}
			return array;
		}

		// Token: 0x0400110B RID: 4363
		[SerializeField]
		private BubblePopper master;

		// Token: 0x0400110C RID: 4364
		[SerializeField]
		private int totalLevels = 10;

		// Token: 0x0400110D RID: 4365
		[SerializeField]
		public int seed;
	}
}
