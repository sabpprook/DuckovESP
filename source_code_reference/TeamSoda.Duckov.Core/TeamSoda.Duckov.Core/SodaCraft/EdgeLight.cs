using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SodaCraft
{
	// Token: 0x0200041C RID: 1052
	[VolumeComponentMenu("SodaCraft/EdgeLight")]
	[Serializable]
	public class EdgeLight : VolumeComponent, IPostProcessComponent
	{
		// Token: 0x060025BC RID: 9660 RVA: 0x00082041 File Offset: 0x00080241
		public bool IsActive()
		{
			return this.enable.value;
		}

		// Token: 0x060025BD RID: 9661 RVA: 0x0008204E File Offset: 0x0008024E
		public bool IsTileCompatible()
		{
			return false;
		}

		// Token: 0x060025BE RID: 9662 RVA: 0x00082054 File Offset: 0x00080254
		public override void Override(VolumeComponent state, float interpFactor)
		{
			EdgeLight edgeLight = state as EdgeLight;
			base.Override(state, interpFactor);
			Shader.SetGlobalVector(this.edgeLightDirectionHash, edgeLight.direction.value);
			Shader.SetGlobalFloat(this.widthHash, edgeLight.edgeLightWidth.value);
			Shader.SetGlobalFloat(this.fixHash, edgeLight.edgeLightFix.value);
			Shader.SetGlobalFloat(this.clampDistanceHash, edgeLight.EdgeLightClampDistance.value);
			Shader.SetGlobalColor(this.colorHash, edgeLight.edgeLightColor.value);
			Shader.SetGlobalFloat(this.edgeLightBlendScreenColorHash, edgeLight.blendScreenColor.value);
		}

		// Token: 0x040019BB RID: 6587
		public BoolParameter enable = new BoolParameter(false, false);

		// Token: 0x040019BC RID: 6588
		public Vector2Parameter direction = new Vector2Parameter(new Vector2(-1f, 1f), false);

		// Token: 0x040019BD RID: 6589
		public ClampedFloatParameter edgeLightWidth = new ClampedFloatParameter(0.001f, 0f, 0.05f, false);

		// Token: 0x040019BE RID: 6590
		public ClampedFloatParameter edgeLightFix = new ClampedFloatParameter(0.001f, 0f, 0.05f, false);

		// Token: 0x040019BF RID: 6591
		public FloatParameter EdgeLightClampDistance = new ClampedFloatParameter(0.001f, 0.001f, 1f, false);

		// Token: 0x040019C0 RID: 6592
		public ColorParameter edgeLightColor = new ColorParameter(Color.white, true, false, false, false);

		// Token: 0x040019C1 RID: 6593
		public FloatParameter blendScreenColor = new ClampedFloatParameter(1f, 0f, 1f, false);

		// Token: 0x040019C2 RID: 6594
		private int edgeLightDirectionHash = Shader.PropertyToID("_EdgeLightDirection");

		// Token: 0x040019C3 RID: 6595
		private int widthHash = Shader.PropertyToID("_EdgeLightWidth");

		// Token: 0x040019C4 RID: 6596
		private int colorHash = Shader.PropertyToID("_EdgeLightColor");

		// Token: 0x040019C5 RID: 6597
		private int fixHash = Shader.PropertyToID("_EdgeLightFix");

		// Token: 0x040019C6 RID: 6598
		private int clampDistanceHash = Shader.PropertyToID("_EdgeLightClampDistance");

		// Token: 0x040019C7 RID: 6599
		private int edgeLightBlendScreenColorHash = Shader.PropertyToID("_EdgeLightBlendScreenColor");
	}
}
