using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Duckov.UI.Animations;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x020003BE RID: 958
	public class WeightBarComplex : MonoBehaviour
	{
		// Token: 0x1700069D RID: 1693
		// (get) Token: 0x060022D4 RID: 8916 RVA: 0x00079F6E File Offset: 0x0007816E
		private CharacterMainControl Target
		{
			get
			{
				if (!this.target)
				{
					LevelManager instance = LevelManager.Instance;
					this.target = ((instance != null) ? instance.MainCharacter : null);
				}
				return this.target;
			}
		}

		// Token: 0x1700069E RID: 1694
		// (get) Token: 0x060022D5 RID: 8917 RVA: 0x00079F9A File Offset: 0x0007819A
		private float LightPercentage
		{
			get
			{
				return 0.25f;
			}
		}

		// Token: 0x1700069F RID: 1695
		// (get) Token: 0x060022D6 RID: 8918 RVA: 0x00079FA1 File Offset: 0x000781A1
		private float SuperHeavyPercentage
		{
			get
			{
				return 0.75f;
			}
		}

		// Token: 0x170006A0 RID: 1696
		// (get) Token: 0x060022D7 RID: 8919 RVA: 0x00079FA8 File Offset: 0x000781A8
		private float MaxWeight
		{
			get
			{
				if (this.Target == null)
				{
					return 0f;
				}
				return this.Target.MaxWeight;
			}
		}

		// Token: 0x170006A1 RID: 1697
		// (get) Token: 0x060022D8 RID: 8920 RVA: 0x00079FCC File Offset: 0x000781CC
		private float BarWidth
		{
			get
			{
				if (this.barArea == null)
				{
					return 0f;
				}
				return this.barArea.rect.width;
			}
		}

		// Token: 0x060022D9 RID: 8921 RVA: 0x0007A000 File Offset: 0x00078200
		private void OnEnable()
		{
			ItemUIUtilities.OnSelectionChanged += this.OnItemSelectionChanged;
			if (this.Target)
			{
				this.Target.CharacterItem.onChildChanged += this.OnTargetChildChanged;
			}
			this.RefreshMarkPositions();
			this.ResetMainBar();
			this.Animate().Forget();
		}

		// Token: 0x060022DA RID: 8922 RVA: 0x0007A05E File Offset: 0x0007825E
		private void OnDisable()
		{
			ItemUIUtilities.OnSelectionChanged -= this.OnItemSelectionChanged;
			if (this.Target)
			{
				this.Target.CharacterItem.onChildChanged -= this.OnTargetChildChanged;
			}
		}

		// Token: 0x060022DB RID: 8923 RVA: 0x0007A09C File Offset: 0x0007829C
		private void RefreshMarkPositions()
		{
			if (this.lightMark == null)
			{
				return;
			}
			if (this.superHeavyMark == null)
			{
				return;
			}
			float num = this.BarWidth * this.LightPercentage;
			float num2 = this.BarWidth * this.SuperHeavyPercentage;
			this.lightMark.anchoredPosition = Vector2.right * num;
			this.superHeavyMark.anchoredPosition = Vector2.right * num2;
		}

		// Token: 0x060022DC RID: 8924 RVA: 0x0007A110 File Offset: 0x00078310
		private void RefreshMarkStatus()
		{
			float num = 0f;
			if (this.MaxWeight > 0f)
			{
				num = this.Target.CharacterItem.TotalWeight / this.MaxWeight;
			}
			this.lightMarkToggle.SetToggle(num > this.LightPercentage);
			this.superHeavyMarkToggle.SetToggle(num > this.SuperHeavyPercentage);
		}

		// Token: 0x060022DD RID: 8925 RVA: 0x0007A170 File Offset: 0x00078370
		private void OnTargetChildChanged(Item item)
		{
			this.Animate().Forget();
		}

		// Token: 0x060022DE RID: 8926 RVA: 0x0007A17D File Offset: 0x0007837D
		private void OnItemSelectionChanged()
		{
			this.Animate().Forget();
		}

		// Token: 0x060022DF RID: 8927 RVA: 0x0007A18C File Offset: 0x0007838C
		private async UniTask Animate()
		{
			this.RefreshMarkPositions();
			this.RefreshMarkStatus();
			this.ResetChangeBars();
			int token = global::UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			this.currentToken = token;
			await this.AnimateMainBar(token);
			this.RefreshMarkPositions();
			if (token == this.currentToken)
			{
				UniTask uniTask = this.AnimatePositiveBar(token);
				UniTask uniTask2 = this.AnimateNegativeBar(token);
				this.RefreshMarkPositions();
				await UniTask.WhenAll(new UniTask[] { uniTask, uniTask2 });
				this.RefreshMarkPositions();
			}
		}

		// Token: 0x060022E0 RID: 8928 RVA: 0x0007A1D0 File Offset: 0x000783D0
		private void ResetChangeBars()
		{
			this.positiveBar.DOKill(false);
			this.negativeBar.DOKill(false);
			this.positiveBar.sizeDelta = new Vector2(this.positiveBar.sizeDelta.x, 0f);
			this.negativeBar.sizeDelta = new Vector2(this.negativeBar.sizeDelta.x, 0f);
		}

		// Token: 0x060022E1 RID: 8929 RVA: 0x0007A241 File Offset: 0x00078441
		private void ResetMainBar()
		{
			this.mainBar.DOKill(false);
			this.mainBar.sizeDelta = new Vector2(this.mainBar.sizeDelta.x, 0f);
		}

		// Token: 0x060022E2 RID: 8930 RVA: 0x0007A278 File Offset: 0x00078478
		private async UniTask AnimateMainBar(int token)
		{
			if (this.Target == null)
			{
				this.SetupInvalid();
			}
			else
			{
				await UniTask.NextFrame();
				if (token == this.currentToken)
				{
					this.mainBar.DOKill(false);
					if (!(this.Target == null))
					{
						float totalWeight = this.Target.CharacterItem.TotalWeight;
						float num = this.WeightToRectHeight(totalWeight);
						Color color = this.superLightColor;
						float num2 = 1f;
						if (this.MaxWeight > 0f)
						{
							num2 = totalWeight / this.MaxWeight;
						}
						if (num2 > 1f)
						{
							color = this.overweightColor;
						}
						else if (num2 > this.SuperHeavyPercentage)
						{
							color = this.superHeavyColor;
						}
						else if (num2 > this.LightPercentage)
						{
							color = this.lightColor;
						}
						else
						{
							color = this.superLightColor;
						}
						TweenerCore<Color, Color, ColorOptions> tweenerCore = this.mainBarGraphic.DOColor(color, this.animateDuration);
						TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore2 = this.mainBar.DOSizeDelta(new Vector2(num, this.mainBar.sizeDelta.y), this.animateDuration, false).SetEase(this.animationCurve);
						await UniTask.WhenAll(new UniTask[]
						{
							tweenerCore.ToUniTask(TweenCancelBehaviour.Kill, default(CancellationToken)),
							tweenerCore2.ToUniTask(TweenCancelBehaviour.Kill, default(CancellationToken))
						});
					}
				}
			}
		}

		// Token: 0x060022E3 RID: 8931 RVA: 0x0007A2C4 File Offset: 0x000784C4
		private async UniTask AnimatePositiveBar(int token)
		{
			if (token == this.currentToken)
			{
				Item selectedItem = ItemUIUtilities.SelectedItem;
				float num = 0f;
				if (selectedItem != null && !selectedItem.IsInPlayerCharacter())
				{
					num = this.WeightToRectHeight(selectedItem.TotalWeight);
				}
				this.positiveBar.DOKill(false);
				await this.positiveBar.DOSizeDelta(new Vector2(num, this.positiveBar.sizeDelta.y), this.animateDuration, false).SetEase(this.animationCurve);
			}
		}

		// Token: 0x060022E4 RID: 8932 RVA: 0x0007A310 File Offset: 0x00078510
		private async UniTask AnimateNegativeBar(int token)
		{
			if (token == this.currentToken)
			{
				Item selectedItem = ItemUIUtilities.SelectedItem;
				float num = 0f;
				if (selectedItem != null && selectedItem.IsInPlayerCharacter())
				{
					num = this.WeightToRectHeight(selectedItem.TotalWeight);
				}
				this.negativeBar.DOKill(false);
				await this.negativeBar.DOSizeDelta(new Vector2(num, this.negativeBar.sizeDelta.y), this.animateDuration, false).SetEase(this.animationCurve);
			}
		}

		// Token: 0x060022E5 RID: 8933 RVA: 0x0007A35B File Offset: 0x0007855B
		private void SetupInvalid()
		{
			WeightBarComplex.SetSizeDeltaY(this.mainBar, 0f);
			WeightBarComplex.SetSizeDeltaY(this.positiveBar, 0f);
			WeightBarComplex.SetSizeDeltaY(this.negativeBar, 0f);
		}

		// Token: 0x060022E6 RID: 8934 RVA: 0x0007A390 File Offset: 0x00078590
		private static void SetSizeDeltaY(RectTransform rectTransform, float sizeDelta)
		{
			Vector2 sizeDelta2 = rectTransform.sizeDelta;
			sizeDelta2.y = sizeDelta;
			rectTransform.sizeDelta = sizeDelta2;
		}

		// Token: 0x060022E7 RID: 8935 RVA: 0x0007A3B3 File Offset: 0x000785B3
		private static float GetSizeDeltaY(RectTransform rectTransform)
		{
			return rectTransform.sizeDelta.y;
		}

		// Token: 0x060022E8 RID: 8936 RVA: 0x0007A3C0 File Offset: 0x000785C0
		private float WeightToRectHeight(float weight)
		{
			if (this.MaxWeight <= 0f)
			{
				return 0f;
			}
			float num = weight / this.MaxWeight;
			return this.BarWidth * num;
		}

		// Token: 0x040017B0 RID: 6064
		[SerializeField]
		private CharacterMainControl target;

		// Token: 0x040017B1 RID: 6065
		[SerializeField]
		private RectTransform barArea;

		// Token: 0x040017B2 RID: 6066
		[SerializeField]
		private RectTransform mainBar;

		// Token: 0x040017B3 RID: 6067
		[SerializeField]
		private Graphic mainBarGraphic;

		// Token: 0x040017B4 RID: 6068
		[SerializeField]
		private RectTransform positiveBar;

		// Token: 0x040017B5 RID: 6069
		[SerializeField]
		private RectTransform negativeBar;

		// Token: 0x040017B6 RID: 6070
		[SerializeField]
		private RectTransform lightMark;

		// Token: 0x040017B7 RID: 6071
		[SerializeField]
		private RectTransform superHeavyMark;

		// Token: 0x040017B8 RID: 6072
		[SerializeField]
		private ToggleAnimation lightMarkToggle;

		// Token: 0x040017B9 RID: 6073
		[SerializeField]
		private ToggleAnimation superHeavyMarkToggle;

		// Token: 0x040017BA RID: 6074
		[SerializeField]
		private Color superLightColor;

		// Token: 0x040017BB RID: 6075
		[SerializeField]
		private Color lightColor;

		// Token: 0x040017BC RID: 6076
		[SerializeField]
		private Color superHeavyColor;

		// Token: 0x040017BD RID: 6077
		[SerializeField]
		private Color overweightColor;

		// Token: 0x040017BE RID: 6078
		[SerializeField]
		private float animateDuration = 0.1f;

		// Token: 0x040017BF RID: 6079
		[SerializeField]
		private AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040017C0 RID: 6080
		private float targetRealBarTop;

		// Token: 0x040017C1 RID: 6081
		private int currentToken;
	}
}
