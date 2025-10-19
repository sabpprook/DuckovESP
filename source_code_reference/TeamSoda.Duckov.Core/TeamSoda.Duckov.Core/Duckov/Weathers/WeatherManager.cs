using System;
using Saves;
using UnityEngine;

namespace Duckov.Weathers
{
	// Token: 0x0200023F RID: 575
	public class WeatherManager : MonoBehaviour
	{
		// Token: 0x17000322 RID: 802
		// (get) Token: 0x060011DF RID: 4575 RVA: 0x00044649 File Offset: 0x00042849
		// (set) Token: 0x060011E0 RID: 4576 RVA: 0x00044650 File Offset: 0x00042850
		public static WeatherManager Instance { get; private set; }

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x060011E1 RID: 4577 RVA: 0x00044658 File Offset: 0x00042858
		// (set) Token: 0x060011E2 RID: 4578 RVA: 0x00044660 File Offset: 0x00042860
		public bool ForceWeather { get; set; }

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x060011E3 RID: 4579 RVA: 0x00044669 File Offset: 0x00042869
		// (set) Token: 0x060011E4 RID: 4580 RVA: 0x00044671 File Offset: 0x00042871
		public Weather ForceWeatherValue { get; set; }

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x060011E5 RID: 4581 RVA: 0x0004467A File Offset: 0x0004287A
		public Storm Storm
		{
			get
			{
				return this.storm;
			}
		}

		// Token: 0x060011E6 RID: 4582 RVA: 0x00044682 File Offset: 0x00042882
		private void Awake()
		{
			WeatherManager.Instance = this;
			SavesSystem.OnCollectSaveData += this.Save;
			this.Load();
			this._weatherDirty = true;
		}

		// Token: 0x060011E7 RID: 4583 RVA: 0x000446A8 File Offset: 0x000428A8
		private void OnDestroy()
		{
			SavesSystem.OnCollectSaveData -= this.Save;
		}

		// Token: 0x060011E8 RID: 4584 RVA: 0x000446BB File Offset: 0x000428BB
		private void Save()
		{
			SavesSystem.Save<WeatherManager.SaveData>("WeatherManagerData", new WeatherManager.SaveData(this));
		}

		// Token: 0x060011E9 RID: 4585 RVA: 0x000446D0 File Offset: 0x000428D0
		private void Load()
		{
			WeatherManager.SaveData saveData = SavesSystem.Load<WeatherManager.SaveData>("WeatherManagerData");
			if (!saveData.valid)
			{
				this.SetRandomKey();
			}
			else
			{
				saveData.Setup(this);
			}
			this.SetupModules();
		}

		// Token: 0x060011EA RID: 4586 RVA: 0x00044706 File Offset: 0x00042906
		private void SetRandomKey()
		{
			this.seed = global::UnityEngine.Random.Range(0, 100000);
		}

		// Token: 0x060011EB RID: 4587 RVA: 0x00044719 File Offset: 0x00042919
		private void SetupModules()
		{
			this.precipitation.SetSeed(this.seed);
		}

		// Token: 0x060011EC RID: 4588 RVA: 0x0004472C File Offset: 0x0004292C
		private Weather M_GetWeather(TimeSpan dayAndTime)
		{
			if (this.ForceWeather)
			{
				return this.ForceWeatherValue;
			}
			if (!this._weatherDirty && dayAndTime == this._cachedDayAndTime)
			{
				return this._cachedWeather;
			}
			int stormLevel = this.storm.GetStormLevel(dayAndTime);
			Weather weather;
			if (stormLevel > 0)
			{
				if (stormLevel == 1)
				{
					weather = Weather.Stormy_I;
				}
				else
				{
					weather = Weather.Stormy_II;
				}
			}
			else
			{
				float num = this.precipitation.Get(dayAndTime);
				if (num > this.precipitation.RainyThreshold)
				{
					weather = Weather.Rainy;
				}
				else if (num > this.precipitation.CloudyThreshold)
				{
					weather = Weather.Cloudy;
				}
				else
				{
					weather = Weather.Sunny;
				}
			}
			this._cachedDayAndTime = dayAndTime;
			this._cachedWeather = weather;
			this._weatherDirty = false;
			return weather;
		}

		// Token: 0x060011ED RID: 4589 RVA: 0x000447CB File Offset: 0x000429CB
		private void M_SetForceWeather(bool forceWeather, Weather value = Weather.Sunny)
		{
			this.ForceWeather = forceWeather;
			this.ForceWeatherValue = value;
		}

		// Token: 0x060011EE RID: 4590 RVA: 0x000447DB File Offset: 0x000429DB
		public static Weather GetWeather(TimeSpan dayAndTime)
		{
			if (WeatherManager.Instance == null)
			{
				return Weather.Sunny;
			}
			return WeatherManager.Instance.M_GetWeather(dayAndTime);
		}

		// Token: 0x060011EF RID: 4591 RVA: 0x000447F8 File Offset: 0x000429F8
		public static Weather GetWeather()
		{
			TimeSpan now = GameClock.Now;
			if (WeatherManager.Instance && WeatherManager.Instance.ForceWeather)
			{
				return WeatherManager.Instance.ForceWeatherValue;
			}
			return WeatherManager.GetWeather(now);
		}

		// Token: 0x060011F0 RID: 4592 RVA: 0x00044834 File Offset: 0x00042A34
		public static void SetForceWeather(bool forceWeather, Weather value = Weather.Sunny)
		{
			if (WeatherManager.Instance == null)
			{
				return;
			}
			WeatherManager.Instance.M_SetForceWeather(forceWeather, value);
		}

		// Token: 0x04000DC6 RID: 3526
		private int seed = -1;

		// Token: 0x04000DC7 RID: 3527
		[SerializeField]
		private Storm storm = new Storm();

		// Token: 0x04000DC8 RID: 3528
		[SerializeField]
		private Precipitation precipitation = new Precipitation();

		// Token: 0x04000DC9 RID: 3529
		private const string SaveKey = "WeatherManagerData";

		// Token: 0x04000DCA RID: 3530
		private Weather _cachedWeather;

		// Token: 0x04000DCB RID: 3531
		private TimeSpan _cachedDayAndTime;

		// Token: 0x04000DCC RID: 3532
		private bool _weatherDirty;

		// Token: 0x0200052D RID: 1325
		[Serializable]
		private struct SaveData
		{
			// Token: 0x06002793 RID: 10131 RVA: 0x00090E72 File Offset: 0x0008F072
			public SaveData(WeatherManager weatherManager)
			{
				this = default(WeatherManager.SaveData);
				this.seed = weatherManager.seed;
				this.valid = true;
			}

			// Token: 0x06002794 RID: 10132 RVA: 0x00090E8E File Offset: 0x0008F08E
			internal void Setup(WeatherManager weatherManager)
			{
				weatherManager.seed = this.seed;
			}

			// Token: 0x04001E62 RID: 7778
			public bool valid;

			// Token: 0x04001E63 RID: 7779
			public int seed;
		}
	}
}
