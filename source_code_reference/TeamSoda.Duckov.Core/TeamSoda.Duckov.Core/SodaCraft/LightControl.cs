using System;
using Umbra;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SodaCraft
{
	// Token: 0x0200041D RID: 1053
	[VolumeComponentMenu("SodaCraft/LightControl")]
	[Serializable]
	public class LightControl : VolumeComponent, IPostProcessComponent
	{
		// Token: 0x060025C0 RID: 9664 RVA: 0x00082217 File Offset: 0x00080417
		public bool IsActive()
		{
			return this.enable.value;
		}

		// Token: 0x060025C1 RID: 9665 RVA: 0x00082224 File Offset: 0x00080424
		public bool IsTileCompatible()
		{
			return false;
		}

		// Token: 0x060025C2 RID: 9666 RVA: 0x00082228 File Offset: 0x00080428
		public override void Override(VolumeComponent state, float interpFactor)
		{
			LightControl lightControl = state as LightControl;
			base.Override(state, interpFactor);
			RenderSettings.ambientSkyColor = lightControl.skyColor.value;
			RenderSettings.ambientEquatorColor = lightControl.equatorColor.value;
			RenderSettings.ambientGroundColor = lightControl.groundColor.value;
			Shader.SetGlobalColor(this.fowColorID, lightControl.fowColor.value);
			Shader.SetGlobalColor(this.SodaPointLight_EnviromentTintID, lightControl.SodaLightTint.value);
			if (!LightControl.light)
			{
				LightControl.light = RenderSettings.sun;
			}
			if (LightControl.light)
			{
				LightControl.light.color = lightControl.sunColor.value;
				LightControl.light.intensity = lightControl.sunIntensity.value;
				LightControl.light.transform.rotation = Quaternion.Euler(lightControl.sunRotation.value);
				if (!LightControl.lightShadows)
				{
					LightControl.lightShadows = LightControl.light.GetComponent<UmbraSoftShadows>();
				}
				if (LightControl.lightShadows)
				{
					float value = lightControl.sunShadowHardness.value;
					LightControl.lightShadows.profile.contactStrength = value;
				}
			}
		}

		// Token: 0x040019C8 RID: 6600
		public BoolParameter enable = new BoolParameter(false, false);

		// Token: 0x040019C9 RID: 6601
		public ColorParameter skyColor = new ColorParameter(Color.black, true, true, false, false);

		// Token: 0x040019CA RID: 6602
		public ColorParameter equatorColor = new ColorParameter(Color.black, true, true, false, false);

		// Token: 0x040019CB RID: 6603
		public ColorParameter groundColor = new ColorParameter(Color.black, true, true, false, false);

		// Token: 0x040019CC RID: 6604
		public ColorParameter sunColor = new ColorParameter(Color.white, true, true, false, false);

		// Token: 0x040019CD RID: 6605
		public ColorParameter fowColor = new ColorParameter(Color.white, true, true, false, false);

		// Token: 0x040019CE RID: 6606
		public MinFloatParameter sunIntensity = new MinFloatParameter(1f, 0f, false);

		// Token: 0x040019CF RID: 6607
		public ClampedFloatParameter sunShadowHardness = new ClampedFloatParameter(0.96f, 0f, 1f, false);

		// Token: 0x040019D0 RID: 6608
		public Vector3Parameter sunRotation = new Vector3Parameter(new Vector3(59f, 168f, 0f), false);

		// Token: 0x040019D1 RID: 6609
		public ColorParameter SodaLightTint = new ColorParameter(Color.white, true, true, false, false);

		// Token: 0x040019D2 RID: 6610
		private int SodaPointLight_EnviromentTintID = Shader.PropertyToID("SodaPointLight_EnviromentTint");

		// Token: 0x040019D3 RID: 6611
		private int fowColorID = Shader.PropertyToID("_SodaUnknowColor");

		// Token: 0x040019D4 RID: 6612
		private static Light light;

		// Token: 0x040019D5 RID: 6613
		private static UmbraSoftShadows lightShadows;
	}
}
