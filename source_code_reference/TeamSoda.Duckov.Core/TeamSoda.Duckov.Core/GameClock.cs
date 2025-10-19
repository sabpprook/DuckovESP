using System;
using Saves;
using UnityEngine;

// Token: 0x020001B7 RID: 439
public class GameClock : MonoBehaviour
{
	// Token: 0x1700025A RID: 602
	// (get) Token: 0x06000CFA RID: 3322 RVA: 0x0003634F File Offset: 0x0003454F
	// (set) Token: 0x06000CFB RID: 3323 RVA: 0x00036356 File Offset: 0x00034556
	public static GameClock Instance { get; private set; }

	// Token: 0x14000062 RID: 98
	// (add) Token: 0x06000CFC RID: 3324 RVA: 0x00036360 File Offset: 0x00034560
	// (remove) Token: 0x06000CFD RID: 3325 RVA: 0x00036394 File Offset: 0x00034594
	public static event Action OnGameClockStep;

	// Token: 0x06000CFE RID: 3326 RVA: 0x000363C7 File Offset: 0x000345C7
	private void Awake()
	{
		if (GameClock.Instance != null)
		{
			Debug.LogError("检测到多个Game Clock");
			return;
		}
		GameClock.Instance = this;
		SavesSystem.OnCollectSaveData += this.Save;
		this.Load();
	}

	// Token: 0x06000CFF RID: 3327 RVA: 0x000363FE File Offset: 0x000345FE
	private void OnDestroy()
	{
		SavesSystem.OnCollectSaveData -= this.Save;
	}

	// Token: 0x1700025B RID: 603
	// (get) Token: 0x06000D00 RID: 3328 RVA: 0x00036411 File Offset: 0x00034611
	private static string SaveKey
	{
		get
		{
			return "GameClock";
		}
	}

	// Token: 0x06000D01 RID: 3329 RVA: 0x00036418 File Offset: 0x00034618
	private void Save()
	{
		SavesSystem.Save<GameClock.SaveData>(GameClock.SaveKey, new GameClock.SaveData
		{
			days = this.days,
			secondsOfDay = this.secondsOfDay,
			realTimePlayedTicks = this.RealTimePlayed.Ticks
		});
	}

	// Token: 0x06000D02 RID: 3330 RVA: 0x00036468 File Offset: 0x00034668
	private void Load()
	{
		GameClock.SaveData saveData = SavesSystem.Load<GameClock.SaveData>(GameClock.SaveKey);
		this.days = saveData.days;
		this.secondsOfDay = saveData.secondsOfDay;
		this.realTimePlayed = saveData.RealTimePlayed;
		Action onGameClockStep = GameClock.OnGameClockStep;
		if (onGameClockStep == null)
		{
			return;
		}
		onGameClockStep();
	}

	// Token: 0x06000D03 RID: 3331 RVA: 0x000364B4 File Offset: 0x000346B4
	public static TimeSpan GetRealTimePlayedOfSaveSlot(int saveSlot)
	{
		return SavesSystem.Load<GameClock.SaveData>(GameClock.SaveKey, saveSlot).RealTimePlayed;
	}

	// Token: 0x1700025C RID: 604
	// (get) Token: 0x06000D04 RID: 3332 RVA: 0x000364D4 File Offset: 0x000346D4
	private TimeSpan RealTimePlayed
	{
		get
		{
			return this.realTimePlayed;
		}
	}

	// Token: 0x1700025D RID: 605
	// (get) Token: 0x06000D05 RID: 3333 RVA: 0x000364DC File Offset: 0x000346DC
	private static double SecondsOfDay
	{
		get
		{
			if (GameClock.Instance == null)
			{
				return 0.0;
			}
			return GameClock.Instance.secondsOfDay;
		}
	}

	// Token: 0x1700025E RID: 606
	// (get) Token: 0x06000D06 RID: 3334 RVA: 0x00036500 File Offset: 0x00034700
	[TimeSpan]
	private long _TimeOfDayTicks
	{
		get
		{
			return GameClock.TimeOfDay.Ticks;
		}
	}

	// Token: 0x1700025F RID: 607
	// (get) Token: 0x06000D07 RID: 3335 RVA: 0x0003651A File Offset: 0x0003471A
	public static TimeSpan TimeOfDay
	{
		get
		{
			return TimeSpan.FromSeconds(GameClock.SecondsOfDay);
		}
	}

	// Token: 0x17000260 RID: 608
	// (get) Token: 0x06000D08 RID: 3336 RVA: 0x00036526 File Offset: 0x00034726
	public static long Day
	{
		get
		{
			if (GameClock.Instance == null)
			{
				return 0L;
			}
			return GameClock.Instance.days;
		}
	}

	// Token: 0x17000261 RID: 609
	// (get) Token: 0x06000D09 RID: 3337 RVA: 0x00036542 File Offset: 0x00034742
	public static TimeSpan Now
	{
		get
		{
			return GameClock.TimeOfDay + TimeSpan.FromDays((double)GameClock.Day);
		}
	}

	// Token: 0x17000262 RID: 610
	// (get) Token: 0x06000D0A RID: 3338 RVA: 0x0003655C File Offset: 0x0003475C
	public static int Hour
	{
		get
		{
			return GameClock.TimeOfDay.Hours;
		}
	}

	// Token: 0x17000263 RID: 611
	// (get) Token: 0x06000D0B RID: 3339 RVA: 0x00036578 File Offset: 0x00034778
	public static int Minut
	{
		get
		{
			return GameClock.TimeOfDay.Minutes;
		}
	}

	// Token: 0x17000264 RID: 612
	// (get) Token: 0x06000D0C RID: 3340 RVA: 0x00036594 File Offset: 0x00034794
	public static int Seconds
	{
		get
		{
			return GameClock.TimeOfDay.Seconds;
		}
	}

	// Token: 0x17000265 RID: 613
	// (get) Token: 0x06000D0D RID: 3341 RVA: 0x000365B0 File Offset: 0x000347B0
	public static int Milliseconds
	{
		get
		{
			return GameClock.TimeOfDay.Milliseconds;
		}
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x000365CA File Offset: 0x000347CA
	private void Update()
	{
		this.StepTime(Time.deltaTime * this.clockTimeScale);
		this.realTimePlayed += TimeSpan.FromSeconds((double)Time.unscaledDeltaTime);
	}

	// Token: 0x06000D0F RID: 3343 RVA: 0x000365FC File Offset: 0x000347FC
	private void StepTime(float deltaTime)
	{
		this.secondsOfDay += (double)deltaTime;
		while (this.secondsOfDay > 86300.0)
		{
			this.days += 1L;
			this.secondsOfDay -= 86300.0;
		}
		Action onGameClockStep = GameClock.OnGameClockStep;
		if (onGameClockStep == null)
		{
			return;
		}
		onGameClockStep();
	}

	// Token: 0x06000D10 RID: 3344 RVA: 0x00036660 File Offset: 0x00034860
	public void StepTimeTil(TimeSpan time)
	{
		if (time.Days > 0)
		{
			time = new TimeSpan(time.Hours, time.Minutes, time.Seconds);
		}
		TimeSpan timeSpan;
		if (time > GameClock.TimeOfDay)
		{
			timeSpan = time - GameClock.TimeOfDay;
		}
		else
		{
			timeSpan = time + TimeSpan.FromDays(1.0) - GameClock.TimeOfDay;
		}
		this.StepTime((float)timeSpan.TotalSeconds);
	}

	// Token: 0x06000D11 RID: 3345 RVA: 0x000366DB File Offset: 0x000348DB
	internal static void Step(float seconds)
	{
		if (GameClock.Instance == null)
		{
			return;
		}
		GameClock.Instance.StepTime(seconds);
	}

	// Token: 0x04000B3A RID: 2874
	public float clockTimeScale = 60f;

	// Token: 0x04000B3B RID: 2875
	private long days;

	// Token: 0x04000B3C RID: 2876
	private double secondsOfDay;

	// Token: 0x04000B3D RID: 2877
	private TimeSpan realTimePlayed;

	// Token: 0x04000B3E RID: 2878
	private const double SecondsPerDay = 86300.0;

	// Token: 0x020004CA RID: 1226
	[Serializable]
	private struct SaveData
	{
		// Token: 0x1700073F RID: 1855
		// (get) Token: 0x060026E4 RID: 9956 RVA: 0x0008CE2C File Offset: 0x0008B02C
		public TimeSpan RealTimePlayed
		{
			get
			{
				return TimeSpan.FromTicks(this.realTimePlayedTicks);
			}
		}

		// Token: 0x04001CBF RID: 7359
		public long days;

		// Token: 0x04001CC0 RID: 7360
		public double secondsOfDay;

		// Token: 0x04001CC1 RID: 7361
		public long realTimePlayedTicks;
	}
}
