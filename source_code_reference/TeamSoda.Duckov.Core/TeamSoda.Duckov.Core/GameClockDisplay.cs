using System;
using TMPro;
using UnityEngine;

// Token: 0x020001B8 RID: 440
public class GameClockDisplay : MonoBehaviour
{
	// Token: 0x06000D13 RID: 3347 RVA: 0x00036709 File Offset: 0x00034909
	private void Awake()
	{
		this.Refresh();
	}

	// Token: 0x06000D14 RID: 3348 RVA: 0x00036711 File Offset: 0x00034911
	private void OnEnable()
	{
		GameClock.OnGameClockStep += this.Refresh;
	}

	// Token: 0x06000D15 RID: 3349 RVA: 0x00036724 File Offset: 0x00034924
	private void OnDisable()
	{
		GameClock.OnGameClockStep -= this.Refresh;
	}

	// Token: 0x06000D16 RID: 3350 RVA: 0x00036738 File Offset: 0x00034938
	private void Refresh()
	{
		string text;
		if (GameClock.Instance == null)
		{
			text = "--:--";
		}
		else
		{
			text = string.Format("{0:00}:{1:00}", GameClock.Hour, GameClock.Minut);
		}
		this.text.text = text;
	}

	// Token: 0x04000B3F RID: 2879
	[SerializeField]
	private TextMeshProUGUI text;
}
