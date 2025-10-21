using System;
using Duckov.Weathers;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200018D RID: 397
public class TimeOfDayConfig : MonoBehaviour
{
	// Token: 0x06000BC5 RID: 3013 RVA: 0x00031EFC File Offset: 0x000300FC
	public TimeOfDayEntry GetCurrentEntry(Weather weather)
	{
		switch (weather)
		{
		case Weather.Sunny:
			return this.defaultEntry;
		case Weather.Cloudy:
			return this.cloudyEntry;
		case Weather.Rainy:
			return this.rainyEntry;
		case Weather.Stormy_I:
			return this.stormIEntry;
		case Weather.Stormy_II:
			return this.stormIIEntry;
		default:
			return this.defaultEntry;
		}
	}

	// Token: 0x06000BC6 RID: 3014 RVA: 0x00031F50 File Offset: 0x00030150
	public void InvokeDebug()
	{
		TimeOfDayEntry currentEntry = this.GetCurrentEntry(this.debugWeather);
		if (!currentEntry)
		{
			Debug.Log("No entry found");
			return;
		}
		TimeOfDayPhase phase = currentEntry.GetPhase(this.debugPhase);
		if (!Application.isPlaying)
		{
			if (this.lookDevVolume && this.lookDevVolume.profile != phase.volumeProfile)
			{
				this.lookDevVolume.profile = phase.volumeProfile;
				return;
			}
		}
		else
		{
			int num;
			switch (this.debugPhase)
			{
			case TimePhaseTags.day:
				num = 9;
				break;
			case TimePhaseTags.dawn:
				num = 17;
				break;
			case TimePhaseTags.night:
				num = 22;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			WeatherManager.SetForceWeather(true, this.debugWeather);
			TimeSpan timeSpan = new TimeSpan(num, 10, 0);
			GameClock.Instance.StepTimeTil(timeSpan);
			Debug.Log(string.Format("Set Weather to {0},and time to {1}", this.debugWeather, num));
		}
	}

	// Token: 0x04000A1F RID: 2591
	[SerializeField]
	private TimeOfDayEntry defaultEntry;

	// Token: 0x04000A20 RID: 2592
	[SerializeField]
	private TimeOfDayEntry cloudyEntry;

	// Token: 0x04000A21 RID: 2593
	[SerializeField]
	private TimeOfDayEntry rainyEntry;

	// Token: 0x04000A22 RID: 2594
	[SerializeField]
	private TimeOfDayEntry stormIEntry;

	// Token: 0x04000A23 RID: 2595
	[SerializeField]
	private TimeOfDayEntry stormIIEntry;

	// Token: 0x04000A24 RID: 2596
	public bool forceSetTime;

	// Token: 0x04000A25 RID: 2597
	[Range(0f, 24f)]
	public int forceSetTimeTo = 8;

	// Token: 0x04000A26 RID: 2598
	public bool forceSetWeather;

	// Token: 0x04000A27 RID: 2599
	public Weather forceSetWeatherTo;

	// Token: 0x04000A28 RID: 2600
	[SerializeField]
	private Volume lookDevVolume;

	// Token: 0x04000A29 RID: 2601
	[SerializeField]
	private TimePhaseTags debugPhase;

	// Token: 0x04000A2A RID: 2602
	[SerializeField]
	private Weather debugWeather;
}
