using System;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.UI.Animations
{
	// Token: 0x020003CF RID: 975
	public class MaterialPropertyFade : FadeElement
	{
		// Token: 0x170006BD RID: 1725
		// (get) Token: 0x0600235C RID: 9052 RVA: 0x0007BA58 File Offset: 0x00079C58
		// (set) Token: 0x0600235D RID: 9053 RVA: 0x0007BA60 File Offset: 0x00079C60
		public AnimationCurve ShowCurve
		{
			get
			{
				return this.showCurve;
			}
			set
			{
				this.showCurve = value;
			}
		}

		// Token: 0x170006BE RID: 1726
		// (get) Token: 0x0600235E RID: 9054 RVA: 0x0007BA69 File Offset: 0x00079C69
		// (set) Token: 0x0600235F RID: 9055 RVA: 0x0007BA71 File Offset: 0x00079C71
		public AnimationCurve HideCurve
		{
			get
			{
				return this.hideCurve;
			}
			set
			{
				this.hideCurve = value;
			}
		}

		// Token: 0x170006BF RID: 1727
		// (get) Token: 0x06002360 RID: 9056 RVA: 0x0007BA7C File Offset: 0x00079C7C
		private Material Material
		{
			get
			{
				if (this._material == null && this.renderer != null)
				{
					this._material = global::UnityEngine.Object.Instantiate<Material>(this.renderer.material);
					this.renderer.material = this._material;
				}
				return this._material;
			}
		}

		// Token: 0x170006C0 RID: 1728
		// (get) Token: 0x06002361 RID: 9057 RVA: 0x0007BAD2 File Offset: 0x00079CD2
		// (set) Token: 0x06002362 RID: 9058 RVA: 0x0007BADA File Offset: 0x00079CDA
		public float Duration
		{
			get
			{
				return this.duration;
			}
			internal set
			{
				this.duration = value;
			}
		}

		// Token: 0x06002363 RID: 9059 RVA: 0x0007BAE3 File Offset: 0x00079CE3
		private void Awake()
		{
			if (this.renderer == null)
			{
				this.renderer = base.GetComponent<Image>();
			}
		}

		// Token: 0x06002364 RID: 9060 RVA: 0x0007BAFF File Offset: 0x00079CFF
		private void OnDestroy()
		{
			if (this._material)
			{
				global::UnityEngine.Object.Destroy(this._material);
			}
		}

		// Token: 0x06002365 RID: 9061 RVA: 0x0007BB1C File Offset: 0x00079D1C
		protected override async UniTask HideTask(int token)
		{
			if (!(this.Material == null))
			{
				if (this.duration <= 0f)
				{
					this.Material.SetFloat(this.propertyName, this.propertyRange.x);
				}
				else
				{
					MaterialPropertyFade.<>c__DisplayClass20_0 CS$<>8__locals1;
					CS$<>8__locals1.timeWhenFadeBegun = Time.unscaledTime;
					float startingValue = this.Material.GetFloat(this.propertyName);
					while (MaterialPropertyFade.<HideTask>g__TimeSinceFadeBegun|20_0(ref CS$<>8__locals1) < this.duration)
					{
						if (token != base.ActiveTaskToken || this.Material == null)
						{
							return;
						}
						float num = MaterialPropertyFade.<HideTask>g__TimeSinceFadeBegun|20_0(ref CS$<>8__locals1) / this.duration;
						this.Material.SetFloat(this.propertyName, Mathf.Lerp(startingValue, this.propertyRange.x, this.hideCurve.Evaluate(num)));
						await UniTask.NextFrame();
					}
					Material material = this.Material;
					if (material != null)
					{
						material.SetFloat(this.propertyName, this.propertyRange.x);
					}
				}
			}
		}

		// Token: 0x06002366 RID: 9062 RVA: 0x0007BB67 File Offset: 0x00079D67
		protected override void OnSkipHide()
		{
			if (this.Material == null)
			{
				return;
			}
			this.Material.SetFloat(this.propertyName, this.propertyRange.x);
		}

		// Token: 0x06002367 RID: 9063 RVA: 0x0007BB94 File Offset: 0x00079D94
		protected override void OnSkipShow()
		{
			if (this.Material == null)
			{
				return;
			}
			this.Material.SetFloat(this.propertyName, this.propertyRange.y);
		}

		// Token: 0x06002368 RID: 9064 RVA: 0x0007BBC4 File Offset: 0x00079DC4
		protected override async UniTask ShowTask(int token)
		{
			if (!(this.Material == null))
			{
				if (this.duration <= 0f)
				{
					this.Material.SetFloat(this.propertyName, this.propertyRange.y);
				}
				else
				{
					MaterialPropertyFade.<>c__DisplayClass23_0 CS$<>8__locals1;
					CS$<>8__locals1.timeWhenFadeBegun = Time.unscaledTime;
					float startingValue = this.Material.GetFloat(this.propertyName);
					while (MaterialPropertyFade.<ShowTask>g__TimeSinceFadeBegun|23_0(ref CS$<>8__locals1) < this.duration)
					{
						if (token != base.ActiveTaskToken || this.Material == null)
						{
							return;
						}
						float num = MaterialPropertyFade.<ShowTask>g__TimeSinceFadeBegun|23_0(ref CS$<>8__locals1) / this.duration;
						this.Material.SetFloat(this.propertyName, Mathf.Lerp(startingValue, this.propertyRange.y, this.showCurve.Evaluate(num)));
						await UniTask.NextFrame();
					}
					Material material = this.Material;
					if (material != null)
					{
						material.SetFloat(this.propertyName, this.propertyRange.y);
					}
				}
			}
		}

		// Token: 0x0600236A RID: 9066 RVA: 0x0007BC8C File Offset: 0x00079E8C
		[CompilerGenerated]
		internal static float <HideTask>g__TimeSinceFadeBegun|20_0(ref MaterialPropertyFade.<>c__DisplayClass20_0 A_0)
		{
			return Time.unscaledTime - A_0.timeWhenFadeBegun;
		}

		// Token: 0x0600236B RID: 9067 RVA: 0x0007BC9A File Offset: 0x00079E9A
		[CompilerGenerated]
		internal static float <ShowTask>g__TimeSinceFadeBegun|23_0(ref MaterialPropertyFade.<>c__DisplayClass23_0 A_0)
		{
			return Time.unscaledTime - A_0.timeWhenFadeBegun;
		}

		// Token: 0x04001809 RID: 6153
		[SerializeField]
		private Image renderer;

		// Token: 0x0400180A RID: 6154
		[SerializeField]
		private string propertyName = "t";

		// Token: 0x0400180B RID: 6155
		[SerializeField]
		private Vector2 propertyRange = new Vector2(0f, 1f);

		// Token: 0x0400180C RID: 6156
		[SerializeField]
		private float duration = 0.5f;

		// Token: 0x0400180D RID: 6157
		[SerializeField]
		private AnimationCurve showCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400180E RID: 6158
		[SerializeField]
		private AnimationCurve hideCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400180F RID: 6159
		private Material _material;
	}
}
