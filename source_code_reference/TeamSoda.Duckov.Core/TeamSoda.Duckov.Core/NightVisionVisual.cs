using System;
using System.Collections.Generic;
using Duckov.Utilities;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

// Token: 0x02000183 RID: 387
public class NightVisionVisual : MonoBehaviour
{
	// Token: 0x06000B96 RID: 2966 RVA: 0x000310FC File Offset: 0x0002F2FC
	public void Awake()
	{
		this.CollectRendererData();
		this.Refresh();
	}

	// Token: 0x06000B97 RID: 2967 RVA: 0x0003110A File Offset: 0x0002F30A
	private void OnDestroy()
	{
		this.nightVisionType = 0;
		this.Refresh();
	}

	// Token: 0x06000B98 RID: 2968 RVA: 0x0003111C File Offset: 0x0002F31C
	private void CollectRendererData()
	{
		if (this.rendererData == null)
		{
			return;
		}
		for (int i = 0; i < this.rendererData.rendererFeatures.Count; i++)
		{
			if (this.rendererData.rendererFeatures[i].name == this.thermalCharacterRednerFeatureKey)
			{
				this.thermalCharacterRednerFeature = this.rendererData.rendererFeatures[i];
			}
			else if (this.rendererData.rendererFeatures[i].name == this.thermalBackgroundRednerFeatureKey)
			{
				this.thermalBackgroundRednerFeature = this.rendererData.rendererFeatures[i];
			}
		}
	}

	// Token: 0x06000B99 RID: 2969 RVA: 0x000311CC File Offset: 0x0002F3CC
	private void Update()
	{
		bool flag = false;
		int num = this.CheckNightVisionType();
		if (num >= this.nightVisionTypes.Length)
		{
			num = 1;
		}
		if (this.nightVisionType != num)
		{
			this.nightVisionType = num;
			flag = true;
		}
		if (LevelManager.LevelInited != this.levelInited)
		{
			this.levelInited = LevelManager.LevelInited;
			flag = true;
		}
		if (flag)
		{
			this.Refresh();
		}
		if (this.character && this.nightVisionLight.gameObject.activeInHierarchy)
		{
			this.nightVisionLight.transform.position = this.character.transform.position + Vector3.up * 2f;
		}
	}

	// Token: 0x06000B9A RID: 2970 RVA: 0x00031277 File Offset: 0x0002F477
	private int CheckNightVisionType()
	{
		if (!this.character)
		{
			if (LevelManager.LevelInited)
			{
				this.character = CharacterMainControl.Main;
			}
			return 0;
		}
		return Mathf.RoundToInt(this.character.NightVisionType);
	}

	// Token: 0x06000B9B RID: 2971 RVA: 0x000312AC File Offset: 0x0002F4AC
	public void Refresh()
	{
		bool flag = this.nightVisionType > 0;
		this.thermalVolume.gameObject.SetActive(flag);
		this.nightVisionLight.gameObject.SetActive(flag);
		NightVisionVisual.NightVisionType nightVisionType = this.nightVisionTypes[this.nightVisionType];
		bool flag2 = nightVisionType.thermalOn && flag;
		bool flag3 = nightVisionType.thermalBackground && flag;
		this.thermalVolume.profile = nightVisionType.profile;
		this.thermalCharacterRednerFeature.SetActive(flag2);
		this.thermalBackgroundRednerFeature.SetActive(flag3);
		Shader.SetGlobalFloat("ThermalOn", flag2 ? 1f : 0f);
		if (LevelManager.LevelInited)
		{
			if (flag2)
			{
				LevelManager.Instance.FogOfWarManager.mainVis.ObstacleMask = GameplayDataSettings.Layers.fowBlockLayersWithThermal;
				return;
			}
			LevelManager.Instance.FogOfWarManager.mainVis.ObstacleMask = GameplayDataSettings.Layers.fowBlockLayers;
		}
	}

	// Token: 0x040009E4 RID: 2532
	private int nightVisionType;

	// Token: 0x040009E5 RID: 2533
	public Volume thermalVolume;

	// Token: 0x040009E6 RID: 2534
	public NightVisionVisual.NightVisionType[] nightVisionTypes;

	// Token: 0x040009E7 RID: 2535
	private CharacterMainControl character;

	// Token: 0x040009E8 RID: 2536
	public ScriptableRendererData rendererData;

	// Token: 0x040009E9 RID: 2537
	public List<string> renderFeatureNames;

	// Token: 0x040009EA RID: 2538
	private ScriptableRendererFeature thermalCharacterRednerFeature;

	// Token: 0x040009EB RID: 2539
	private ScriptableRendererFeature thermalBackgroundRednerFeature;

	// Token: 0x040009EC RID: 2540
	public Transform nightVisionLight;

	// Token: 0x040009ED RID: 2541
	public string thermalCharacterRednerFeatureKey = "ThermalCharacter";

	// Token: 0x040009EE RID: 2542
	public string thermalBackgroundRednerFeatureKey = "ThermalBackground";

	// Token: 0x040009EF RID: 2543
	private bool levelInited;

	// Token: 0x020004B6 RID: 1206
	[Serializable]
	public struct NightVisionType
	{
		// Token: 0x04001C72 RID: 7282
		public string intro;

		// Token: 0x04001C73 RID: 7283
		public VolumeProfile profile;

		// Token: 0x04001C74 RID: 7284
		[FormerlySerializedAs("thermalCharacter")]
		public bool thermalOn;

		// Token: 0x04001C75 RID: 7285
		public bool thermalBackground;
	}
}
