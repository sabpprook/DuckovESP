using System;
using Duckov;
using Duckov.Scenes;
using Duckov.Weathers;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000194 RID: 404
public class WeatherFxControl : MonoBehaviour
{
	// Token: 0x06000BE1 RID: 3041 RVA: 0x000326D1 File Offset: 0x000308D1
	private void Start()
	{
	}

	// Token: 0x06000BE2 RID: 3042 RVA: 0x000326D4 File Offset: 0x000308D4
	private void Init()
	{
		this.inited = true;
		this.rainingParticleRate = new float[this.rainyFxParticles.Length];
		for (int i = 0; i < this.rainyFxParticles.Length; i++)
		{
			ParticleSystem.EmissionModule emission = this.rainyFxParticles[i].emission;
			this.rainingParticleRate[i] = emission.rateOverTime.constant;
		}
		this.SetFxActive(false);
	}

	// Token: 0x06000BE3 RID: 3043 RVA: 0x0003273A File Offset: 0x0003093A
	private void OnSubSceneChanged()
	{
	}

	// Token: 0x06000BE4 RID: 3044 RVA: 0x0003273C File Offset: 0x0003093C
	private void Update()
	{
		if (!this.inited)
		{
			if (!LevelManager.Instance)
			{
				return;
			}
			if (!LevelManager.LevelInited)
			{
				return;
			}
			this.Init();
			this.SetFxActive(false);
			return;
		}
		else
		{
			if (!TimeOfDayController.Instance)
			{
				return;
			}
			if (!MultiSceneCore.Instance)
			{
				return;
			}
			bool flag = TimeOfDayController.Instance.CurrentWeather == this.targetWeather;
			SubSceneEntry subSceneInfo = MultiSceneCore.Instance.GetSubSceneInfo();
			if (this.onlyOutDoor && subSceneInfo.IsInDoor)
			{
				flag = false;
				this.lerpValue = 0f;
			}
			if (flag)
			{
				this.overTimer = this.deactiveDelay;
				if (!this.fxActive)
				{
					this.SetFxActive(true);
				}
			}
			else if (this.lerpValue <= 0.01f)
			{
				this.overTimer -= Time.deltaTime;
				if (this.overTimer <= 0f)
				{
					this.SetFxActive(false);
				}
			}
			if (!this.fxActive)
			{
				return;
			}
			this.lerpValue = Mathf.MoveTowards(this.lerpValue, flag ? 1f : 0f, Time.deltaTime / this.lerpTime);
			for (int i = 0; i < this.rainyFxParticles.Length; i++)
			{
				ParticleSystem.EmissionModule emission = this.rainyFxParticles[i].emission;
				float num = this.rainingParticleRate[i];
				emission.rateOverTime = Mathf.Lerp(0f, num, this.lerpValue);
			}
			if (flag != this.audioPlaying)
			{
				this.audioPlaying = flag;
				if (flag)
				{
					this.weatherSoundInstace = AudioManager.Post(this.rainSoundKey, base.gameObject);
					return;
				}
				if (this.weatherSoundInstace != null)
				{
					this.weatherSoundInstace.Value.stop(STOP_MODE.ALLOWFADEOUT);
				}
			}
			return;
		}
	}

	// Token: 0x06000BE5 RID: 3045 RVA: 0x000328E8 File Offset: 0x00030AE8
	private void SetFxActive(bool active)
	{
		foreach (ParticleSystem particleSystem in this.rainyFxParticles)
		{
			if (!(particleSystem == null))
			{
				particleSystem.gameObject.SetActive(active);
			}
		}
		this.fxActive = active;
	}

	// Token: 0x06000BE6 RID: 3046 RVA: 0x0003292C File Offset: 0x00030B2C
	private void OnDestroy()
	{
		if (this.weatherSoundInstace != null)
		{
			this.weatherSoundInstace.Value.stop(STOP_MODE.ALLOWFADEOUT);
		}
	}

	// Token: 0x04000A56 RID: 2646
	public ParticleSystem[] rainyFxParticles;

	// Token: 0x04000A57 RID: 2647
	[HideInInspector]
	public float[] rainingParticleRate;

	// Token: 0x04000A58 RID: 2648
	public Weather targetWeather;

	// Token: 0x04000A59 RID: 2649
	private float targetParticleRate;

	// Token: 0x04000A5A RID: 2650
	private float lerpValue;

	// Token: 0x04000A5B RID: 2651
	public float lerpTime = 5f;

	// Token: 0x04000A5C RID: 2652
	public float deactiveDelay = 10f;

	// Token: 0x04000A5D RID: 2653
	private float overTimer;

	// Token: 0x04000A5E RID: 2654
	private bool fxActive;

	// Token: 0x04000A5F RID: 2655
	private bool inited;

	// Token: 0x04000A60 RID: 2656
	private EventInstance? weatherSoundInstace;

	// Token: 0x04000A61 RID: 2657
	public string rainSoundKey = "Amb/amb_rain";

	// Token: 0x04000A62 RID: 2658
	private bool audioPlaying;

	// Token: 0x04000A63 RID: 2659
	[FormerlySerializedAs("onlyInDoor")]
	public bool onlyOutDoor = true;
}
