using System;
using System.Diagnostics;
using SodaCraft.StringUtilities;
using UnityEngine;

// Token: 0x0200010F RID: 271
public class NamedFormatTest : MonoBehaviour
{
	// Token: 0x06000942 RID: 2370 RVA: 0x00028EDC File Offset: 0x000270DC
	private void Test()
	{
		string text = "";
		Stopwatch stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < this.loopCount; i++)
		{
			text = this.format.Format(this.content);
		}
		stopwatch.Stop();
		global::UnityEngine.Debug.Log("Time Consumed 1:" + stopwatch.ElapsedMilliseconds.ToString());
		stopwatch = Stopwatch.StartNew();
		for (int j = 0; j < this.loopCount; j++)
		{
			text = string.Format(this.format2, this.content.textA, this.content.textB);
		}
		stopwatch.Stop();
		global::UnityEngine.Debug.Log("Time Consumed 2:" + stopwatch.ElapsedMilliseconds.ToString());
		global::UnityEngine.Debug.Log(text);
	}

	// Token: 0x06000943 RID: 2371 RVA: 0x00028FA8 File Offset: 0x000271A8
	private void Test2()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		string text = this.format.Format(new
		{
			this.content.textA,
			this.content.textB
		});
		stopwatch.Stop();
		global::UnityEngine.Debug.Log("Time Consumed:" + stopwatch.ElapsedMilliseconds.ToString());
		global::UnityEngine.Debug.Log(text);
	}

	// Token: 0x04000840 RID: 2112
	public string format = "Displaying {textA} {textB}";

	// Token: 0x04000841 RID: 2113
	public string format2 = "Displaying {0} {1}";

	// Token: 0x04000842 RID: 2114
	public NamedFormatTest.Content content;

	// Token: 0x04000843 RID: 2115
	[SerializeField]
	private int loopCount = 100;

	// Token: 0x02000491 RID: 1169
	[Serializable]
	public struct Content
	{
		// Token: 0x04001BC1 RID: 7105
		public string textA;

		// Token: 0x04001BC2 RID: 7106
		public string textB;
	}
}
