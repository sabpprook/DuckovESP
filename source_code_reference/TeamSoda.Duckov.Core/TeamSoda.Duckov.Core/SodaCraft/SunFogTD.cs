using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SodaCraft
{
	// Token: 0x0200041A RID: 1050
	[VolumeComponentMenu("SodaCraft/SunFogTD")]
	[Serializable]
	public class SunFogTD : VolumeComponent, IPostProcessComponent
	{
		// Token: 0x060025B5 RID: 9653 RVA: 0x00081D25 File Offset: 0x0007FF25
		public bool IsActive()
		{
			return this.enable.value;
		}

		// Token: 0x060025B6 RID: 9654 RVA: 0x00081D32 File Offset: 0x0007FF32
		public bool IsTileCompatible()
		{
			return false;
		}

		// Token: 0x060025B7 RID: 9655 RVA: 0x00081D38 File Offset: 0x0007FF38
		public override void Override(VolumeComponent state, float interpFactor)
		{
			SunFogTD sunFogTD = state as SunFogTD;
			base.Override(state, interpFactor);
			Shader.SetGlobalColor(this.fogColorHash, sunFogTD.fogColor.value);
			Shader.SetGlobalColor(this.sunColorHash, sunFogTD.sunColor.value);
			Shader.SetGlobalFloat(this.nearDistanceHash, sunFogTD.clipPlanes.value.x);
			Shader.SetGlobalFloat(this.farDistanceHash, sunFogTD.clipPlanes.value.y);
			Shader.SetGlobalFloat(this.sunSizeHash, sunFogTD.sunSize.value);
			Shader.SetGlobalFloat(this.sunPowerHash, sunFogTD.sunPower.value);
			Shader.SetGlobalVector(this.sunPointHash, sunFogTD.sunPoint.value);
			Shader.SetGlobalFloat(this.sunAlphaGainHash, sunFogTD.sunAlphaGain.value);
		}

		// Token: 0x040019A7 RID: 6567
		public BoolParameter enable = new BoolParameter(false, false);

		// Token: 0x040019A8 RID: 6568
		public ColorParameter fogColor = new ColorParameter(new Color(0.68718916f, 1.070217f, 1.3615336f, 0f), true, true, false, false);

		// Token: 0x040019A9 RID: 6569
		public ColorParameter sunColor = new ColorParameter(new Color(4.061477f, 2.5092788f, 1.7816858f, 0f), true, true, false, false);

		// Token: 0x040019AA RID: 6570
		public FloatRangeParameter clipPlanes = new FloatRangeParameter(new Vector2(41f, 72f), 0.3f, 1000f, false);

		// Token: 0x040019AB RID: 6571
		public Vector2Parameter sunPoint = new Vector2Parameter(new Vector2(-2.63f, 1.23f), false);

		// Token: 0x040019AC RID: 6572
		public FloatParameter sunSize = new ClampedFloatParameter(1.85f, 0f, 10f, false);

		// Token: 0x040019AD RID: 6573
		public ClampedFloatParameter sunPower = new ClampedFloatParameter(1f, 0.1f, 10f, false);

		// Token: 0x040019AE RID: 6574
		public ClampedFloatParameter sunAlphaGain = new ClampedFloatParameter(0.001f, 0f, 0.25f, false);

		// Token: 0x040019AF RID: 6575
		private int fogColorHash = Shader.PropertyToID("SunFogColor");

		// Token: 0x040019B0 RID: 6576
		private int sunColorHash = Shader.PropertyToID("SunFogSunColor");

		// Token: 0x040019B1 RID: 6577
		private int nearDistanceHash = Shader.PropertyToID("SunFogNearDistance");

		// Token: 0x040019B2 RID: 6578
		private int farDistanceHash = Shader.PropertyToID("SunFogFarDistance");

		// Token: 0x040019B3 RID: 6579
		private int sunPointHash = Shader.PropertyToID("SunFogSunPoint");

		// Token: 0x040019B4 RID: 6580
		private int sunSizeHash = Shader.PropertyToID("SunFogSunSize");

		// Token: 0x040019B5 RID: 6581
		private int sunPowerHash = Shader.PropertyToID("SunFogSunPower");

		// Token: 0x040019B6 RID: 6582
		private int sunAlphaGainHash = Shader.PropertyToID("SunFogSunAplhaGain");
	}
}
