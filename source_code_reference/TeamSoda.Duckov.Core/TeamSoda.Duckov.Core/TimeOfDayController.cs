using System;
using Duckov;
using Duckov.Scenes;
using Duckov.UI;
using Duckov.Weathers;
using SodaCraft.Localizations;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200018E RID: 398
public class TimeOfDayController : MonoBehaviour
{
	// Token: 0x17000229 RID: 553
	// (get) Token: 0x06000BC8 RID: 3016 RVA: 0x00032054 File Offset: 0x00030254
	public static TimeOfDayController Instance
	{
		get
		{
			if (!LevelManager.Instance)
			{
				return null;
			}
			return LevelManager.Instance.TimeOfDayController;
		}
	}

	// Token: 0x1700022A RID: 554
	// (get) Token: 0x06000BC9 RID: 3017 RVA: 0x0003206E File Offset: 0x0003026E
	public bool AtNight
	{
		get
		{
			return this.atNight;
		}
	}

	// Token: 0x1700022B RID: 555
	// (get) Token: 0x06000BCA RID: 3018 RVA: 0x00032076 File Offset: 0x00030276
	public TimeOfDayPhase CurrentPhase
	{
		get
		{
			return this.currentPhase;
		}
	}

	// Token: 0x1700022C RID: 556
	// (get) Token: 0x06000BCB RID: 3019 RVA: 0x0003207E File Offset: 0x0003027E
	public Weather CurrentWeather
	{
		get
		{
			return this.currentWeather;
		}
	}

	// Token: 0x1700022D RID: 557
	// (get) Token: 0x06000BCC RID: 3020 RVA: 0x00032086 File Offset: 0x00030286
	public float Time
	{
		get
		{
			return this.time;
		}
	}

	// Token: 0x06000BCD RID: 3021 RVA: 0x00032090 File Offset: 0x00030290
	private void Start()
	{
		this.config = LevelConfig.Instance.timeOfDayConfig;
		if (this.config.forceSetTime)
		{
			TimeSpan timeSpan = new TimeSpan(0, this.config.forceSetTimeTo, 0, 0);
			GameClock.Instance.StepTimeTil(timeSpan);
		}
		if (this.config.forceSetWeather)
		{
			WeatherManager.SetForceWeather(true, this.config.forceSetWeatherTo);
		}
		this.time = (float)GameClock.TimeOfDay.TotalHours % 24f;
		TimePhaseTags timePhaseTagByTime = this.GetTimePhaseTagByTime(this.time);
		this.atNight = timePhaseTagByTime == TimePhaseTags.night;
		this.currentWeather = WeatherManager.GetWeather();
		this.OnWeatherChanged(this.currentWeather);
		this.currentPhase = this.config.GetCurrentEntry(this.CurrentWeather).GetPhase(timePhaseTagByTime);
		this.weatherVolumeControl.ForceSetProfile(this.currentPhase.volumeProfile);
	}

	// Token: 0x06000BCE RID: 3022 RVA: 0x00032174 File Offset: 0x00030374
	private void Update()
	{
		this.time = (float)GameClock.TimeOfDay.TotalHours % 24f;
		TimePhaseTags timePhaseTagByTime = this.GetTimePhaseTagByTime(this.time);
		this.atNight = timePhaseTagByTime == TimePhaseTags.night;
		Weather weather = WeatherManager.GetWeather();
		if (weather != this.currentWeather)
		{
			this.currentWeather = weather;
			this.OnWeatherChanged(this.currentWeather);
		}
		this.currentPhase = this.config.GetCurrentEntry(this.CurrentWeather).GetPhase(timePhaseTagByTime);
		if (this.weatherVolumeControl.CurrentProfile != this.currentPhase.volumeProfile && this.weatherVolumeControl.BufferTargetProfile != this.currentPhase.volumeProfile)
		{
			this.weatherVolumeControl.SetTargetProfile(this.currentPhase.volumeProfile);
		}
	}

	// Token: 0x06000BCF RID: 3023 RVA: 0x00032244 File Offset: 0x00030444
	private void OnWeatherChanged(Weather newWeather)
	{
		bool flag = false;
		if (MultiSceneCore.Instance)
		{
			SubSceneEntry subSceneInfo = MultiSceneCore.Instance.GetSubSceneInfo();
			if (subSceneInfo != null)
			{
				flag = subSceneInfo.IsInDoor;
			}
		}
		if (newWeather == Weather.Stormy_I)
		{
			this.stormIObject.SetActive(true);
			this.stormIIObject.SetActive(false);
			NotificationText.Push("Weather_Storm_I".ToPlainText());
			if (!flag && LevelManager.AfterInit)
			{
				AudioManager.Post(this.stormPhaseISoundKey, base.gameObject);
				return;
			}
		}
		else if (newWeather == Weather.Stormy_II)
		{
			this.stormIObject.SetActive(false);
			this.stormIIObject.SetActive(true);
			NotificationText.Push("Weather_Storm_II".ToPlainText());
			if (!flag && LevelManager.AfterInit)
			{
				AudioManager.Post(this.stormPhaseIISoundKey, base.gameObject);
				return;
			}
		}
		else
		{
			this.stormIObject.SetActive(false);
			this.stormIIObject.SetActive(false);
		}
	}

	// Token: 0x06000BD0 RID: 3024 RVA: 0x0003231C File Offset: 0x0003051C
	private TimePhaseTags GetTimePhaseTagByTime(float hourTime)
	{
		hourTime %= 24f;
		if (hourTime < this.morningStart || hourTime >= this.nightStart)
		{
			return TimePhaseTags.night;
		}
		if (hourTime >= this.morningStart && hourTime < this.dawnStart)
		{
			return TimePhaseTags.day;
		}
		if (hourTime >= this.dawnStart && hourTime < this.nightStart)
		{
			return TimePhaseTags.dawn;
		}
		return TimePhaseTags.day;
	}

	// Token: 0x06000BD1 RID: 3025 RVA: 0x00032370 File Offset: 0x00030570
	public static string GetTimePhaseNameByPhaseTag(TimePhaseTags phaseTag)
	{
		TimeOfDayController instance = TimeOfDayController.Instance;
		if (!instance)
		{
			return string.Empty;
		}
		switch (phaseTag)
		{
		case TimePhaseTags.day:
			return instance.timePhaseKey_Day.ToPlainText();
		case TimePhaseTags.dawn:
			return instance.timePhaseKey_Dawn.ToPlainText();
		case TimePhaseTags.night:
			return instance.timePhaseKey_Night.ToPlainText();
		default:
			return instance.timePhaseKey_Day.ToPlainText();
		}
	}

	// Token: 0x06000BD2 RID: 3026 RVA: 0x000323D4 File Offset: 0x000305D4
	public static string GetWeatherNameByWeather(Weather weather)
	{
		TimeOfDayController instance = TimeOfDayController.Instance;
		if (!instance)
		{
			return string.Empty;
		}
		switch (weather)
		{
		case Weather.Sunny:
			return instance.WeatherKey_Sunny.ToPlainText();
		case Weather.Cloudy:
			return instance.WeatherKey_Cloudy.ToPlainText();
		case Weather.Rainy:
			return instance.WeatherKey_Rainy.ToPlainText();
		case Weather.Stormy_I:
			return instance.WeatherKey_Storm_I.ToPlainText();
		case Weather.Stormy_II:
			return instance.WeatherKey_Storm_II.ToPlainText();
		default:
			return instance.WeatherKey_Sunny.ToPlainText();
		}
	}

	// Token: 0x04000A2B RID: 2603
	private TimeOfDayConfig config;

	// Token: 0x04000A2C RID: 2604
	private bool atNight;

	// Token: 0x04000A2D RID: 2605
	[FormerlySerializedAs("volumeControl")]
	[SerializeField]
	private TimeOfDayVolumeControl weatherVolumeControl;

	// Token: 0x04000A2E RID: 2606
	private TimeOfDayPhase currentPhase;

	// Token: 0x04000A2F RID: 2607
	private Weather currentWeather;

	// Token: 0x04000A30 RID: 2608
	public float morningStart = 5f;

	// Token: 0x04000A31 RID: 2609
	public float dawnStart = 16f;

	// Token: 0x04000A32 RID: 2610
	public float nightStart = 19f;

	// Token: 0x04000A33 RID: 2611
	public static float NightViewAngleFactor;

	// Token: 0x04000A34 RID: 2612
	public static float NightViewDistanceFactor;

	// Token: 0x04000A35 RID: 2613
	public static float NightSenseRangeFactor;

	// Token: 0x04000A36 RID: 2614
	[LocalizationKey("Default")]
	public string timePhaseKey_Day;

	// Token: 0x04000A37 RID: 2615
	[LocalizationKey("Default")]
	public string timePhaseKey_Dawn;

	// Token: 0x04000A38 RID: 2616
	[LocalizationKey("Default")]
	public string timePhaseKey_Night;

	// Token: 0x04000A39 RID: 2617
	[LocalizationKey("Default")]
	public string WeatherKey_Sunny;

	// Token: 0x04000A3A RID: 2618
	[LocalizationKey("Default")]
	public string WeatherKey_Cloudy;

	// Token: 0x04000A3B RID: 2619
	[LocalizationKey("Default")]
	public string WeatherKey_Rainy;

	// Token: 0x04000A3C RID: 2620
	[LocalizationKey("Default")]
	public string WeatherKey_Storm_I;

	// Token: 0x04000A3D RID: 2621
	[LocalizationKey("Default")]
	public string WeatherKey_Storm_II;

	// Token: 0x04000A3E RID: 2622
	private string stormPhaseISoundKey = "Music/Stinger/stg_storm_1";

	// Token: 0x04000A3F RID: 2623
	private string stormPhaseIISoundKey = "Music/Stinger/stg_storm_2";

	// Token: 0x04000A40 RID: 2624
	public GameObject stormIObject;

	// Token: 0x04000A41 RID: 2625
	public GameObject stormIIObject;

	// Token: 0x04000A42 RID: 2626
	private float time;
}
