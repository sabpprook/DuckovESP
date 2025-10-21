using System;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Duckov.Scenes;
using Duckov.UI.Animations;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x020003AD RID: 941
	public class ClosureView : View
	{
		// Token: 0x17000676 RID: 1654
		// (get) Token: 0x060021C2 RID: 8642 RVA: 0x0007592B File Offset: 0x00073B2B
		public static ClosureView Instance
		{
			get
			{
				return View.GetViewInstance<ClosureView>();
			}
		}

		// Token: 0x17000677 RID: 1655
		// (get) Token: 0x060021C3 RID: 8643 RVA: 0x00075932 File Offset: 0x00073B32
		private string EvacuatedTitleText
		{
			get
			{
				return this.evacuatedTitleTextKey.ToPlainText();
			}
		}

		// Token: 0x17000678 RID: 1656
		// (get) Token: 0x060021C4 RID: 8644 RVA: 0x0007593F File Offset: 0x00073B3F
		private string FailedTitleText
		{
			get
			{
				return this.failedTitleTextKey.ToPlainText();
			}
		}

		// Token: 0x060021C5 RID: 8645 RVA: 0x0007594C File Offset: 0x00073B4C
		protected override void Awake()
		{
			base.Awake();
			this.continueButton.onClick.AddListener(new UnityAction(this.OnContinueButtonClicked));
		}

		// Token: 0x060021C6 RID: 8646 RVA: 0x00075970 File Offset: 0x00073B70
		private void OnContinueButtonClicked()
		{
			if (!this.canContinue)
			{
				return;
			}
			this.continueButtonClicked = true;
			this.contentFadeGroup.Hide();
		}

		// Token: 0x060021C7 RID: 8647 RVA: 0x0007598D File Offset: 0x00073B8D
		protected override void OnOpen()
		{
			base.OnOpen();
			this.fadeGroup.Show();
			this.contentFadeGroup.Show();
		}

		// Token: 0x060021C8 RID: 8648 RVA: 0x000759AB File Offset: 0x00073BAB
		protected override void OnClose()
		{
			base.OnClose();
			this.fadeGroup.Hide();
		}

		// Token: 0x060021C9 RID: 8649 RVA: 0x000759C0 File Offset: 0x00073BC0
		public static async UniTask ShowAndReturnTask(float duration = 0.5f)
		{
			if (!(ClosureView.Instance == null))
			{
				ClosureView.Instance.canContinue = false;
				await BlackScreen.ShowAndReturnTask(null, 1f, duration);
				if (MultiSceneCore.Instance)
				{
					MultiSceneCore.Instance.PlayStinger();
				}
				ClosureView.Instance.Open(null);
				ClosureView.Instance.SetupTitle(false);
				ClosureView.Instance.SetupBeginning();
				ClosureView.Instance.damageInfoContainer.SetActive(false);
				await BlackScreen.HideAndReturnTask(null, 0f, duration);
				ClosureView.Instance.canContinue = true;
				await ClosureView.Instance.ClosureTask();
			}
		}

		// Token: 0x060021CA RID: 8650 RVA: 0x00075A04 File Offset: 0x00073C04
		public static async UniTask ShowAndReturnTask(DamageInfo dmgInfo, float duration = 0.5f)
		{
			if (!(ClosureView.Instance == null))
			{
				ClosureView.Instance.canContinue = false;
				await BlackScreen.ShowAndReturnTask(null, 1f, duration);
				if (!(ClosureView.Instance == null))
				{
					ClosureView.Instance.Open(null);
					ClosureView.Instance.SetupTitle(true);
					ClosureView.Instance.SetupBeginning();
					ClosureView.Instance.SetupDamageInfo(dmgInfo);
					await BlackScreen.HideAndReturnTask(null, 0f, duration);
					if (!(ClosureView.Instance == null))
					{
						ClosureView.Instance.canContinue = true;
						await ClosureView.Instance.ClosureTask();
					}
				}
			}
		}

		// Token: 0x060021CB RID: 8651 RVA: 0x00075A4F File Offset: 0x00073C4F
		private void SetupDamageInfo(DamageInfo dmgInfo)
		{
			this.damageSourceText.text = dmgInfo.GenerateDescription();
			this.damageInfoContainer.gameObject.SetActive(true);
		}

		// Token: 0x060021CC RID: 8652 RVA: 0x00075A74 File Offset: 0x00073C74
		private async UniTask ClosureTask()
		{
			this.continueButtonClicked = false;
			long cachedExp = EXPManager.CachedExp;
			long exp = EXPManager.EXP;
			await this.AnimateExpBar(cachedExp, exp);
			this.continueButton.gameObject.SetActive(true);
			this.continueButtonPunchReceiver.Punch();
			AudioManager.Post(this.sfx_Pop);
			while (!this.continueButtonClicked)
			{
				await UniTask.NextFrame();
			}
			AudioManager.Post("UI/confirm");
		}

		// Token: 0x060021CD RID: 8653 RVA: 0x00075AB8 File Offset: 0x00073CB8
		private void SetupBeginning()
		{
			long cachedExp = EXPManager.CachedExp;
			long exp = EXPManager.EXP;
			this.Refresh(0f, cachedExp, exp);
			this.continueButton.gameObject.SetActive(false);
		}

		// Token: 0x060021CE RID: 8654 RVA: 0x00075AF0 File Offset: 0x00073CF0
		private void SetupTitle(bool dead)
		{
			if (dead)
			{
				this.titleText.color = this.failedTitleTextColor;
				this.titleText.text = this.FailedTitleText;
				return;
			}
			this.titleText.color = this.evacuatedTitleTextColor;
			this.titleText.text = this.EvacuatedTitleText;
		}

		// Token: 0x060021CF RID: 8655 RVA: 0x00075B48 File Offset: 0x00073D48
		private async UniTask AnimateExpBar(long fromExp, long toExp)
		{
			if (fromExp != toExp)
			{
				float time = 0f;
				long displayingExp = fromExp;
				while (time < this.expBarAnimationTime && fromExp != toExp)
				{
					float num = time / this.expBarAnimationTime;
					long num2 = this.Refresh(this.expBarAnimationCurve.Evaluate(num), fromExp, toExp);
					if (num2 != displayingExp)
					{
						this.SpitExpUpSfx((float)(num2 - fromExp) / (float)(toExp - fromExp));
					}
					displayingExp = num2;
					time += Time.unscaledDeltaTime;
					await UniTask.NextFrame();
				}
				this.SpitExpUpSfx(1f);
			}
			this.SetExpDisplay(toExp, fromExp);
			this.SetLevelDisplay(this.cachedLevel);
		}

		// Token: 0x060021D0 RID: 8656 RVA: 0x00075B9C File Offset: 0x00073D9C
		private void SpitExpUpSfx(float expDelta)
		{
			float unscaledTime = Time.unscaledTime;
			if (unscaledTime - this.lastTimeExpUpSfxPlayed < 0.05f)
			{
				return;
			}
			this.lastTimeExpUpSfxPlayed = unscaledTime;
			AudioManager.SetRTPC("ExpDelta", expDelta, null);
			AudioManager.Post(this.sfx_ExpUp);
		}

		// Token: 0x060021D1 RID: 8657 RVA: 0x00075BE0 File Offset: 0x00073DE0
		private long Refresh(float t, long fromExp, long toExp)
		{
			long num = this.LongLerp(fromExp, toExp, t);
			this.SetExpDisplay(num, fromExp);
			this.SetLevelDisplay(this.cachedLevel);
			return num;
		}

		// Token: 0x060021D2 RID: 8658 RVA: 0x00075C0C File Offset: 0x00073E0C
		private long LongLerp(long from, long to, float t)
		{
			return (long)((float)(to - from) * t) + from;
		}

		// Token: 0x060021D3 RID: 8659 RVA: 0x00075C18 File Offset: 0x00073E18
		private void CacheLevelInfo(int level)
		{
			if (level == this.cachedLevel)
			{
				return;
			}
			this.cachedLevel = level;
			this.cachedLevelRange = EXPManager.Instance.GetLevelExpRange(level);
			this.cachedLevelLength = this.cachedLevelRange.Item2 - this.cachedLevelRange.Item1;
		}

		// Token: 0x060021D4 RID: 8660 RVA: 0x00075C64 File Offset: 0x00073E64
		private void SetExpDisplay(long currentExp, long oldExp)
		{
			int num = EXPManager.Instance.LevelFromExp(currentExp);
			this.CacheLevelInfo(num);
			float num2 = 0f;
			if (oldExp >= this.cachedLevelRange.Item1 && oldExp <= this.cachedLevelRange.Item2)
			{
				num2 = (float)(oldExp - this.cachedLevelRange.Item1) / (float)this.cachedLevelLength;
			}
			float num3 = (float)(currentExp - this.cachedLevelRange.Item1) / (float)this.cachedLevelLength;
			this.expBar_OldFill.fillAmount = num2;
			this.expBar_CurrentFill.fillAmount = num3;
			this.expDisplay.text = string.Format(this.expFormat, currentExp, this.cachedLevelRange.Item2);
		}

		// Token: 0x060021D5 RID: 8661 RVA: 0x00075D18 File Offset: 0x00073F18
		private void SetLevelDisplay(int level)
		{
			if (this.displayingLevel > 0 && level != this.displayingLevel)
			{
				this.LevelUpPunch();
			}
			this.displayingLevel = level;
			this.levelDisplay.text = string.Format(this.levelFormat, level);
		}

		// Token: 0x060021D6 RID: 8662 RVA: 0x00075D55 File Offset: 0x00073F55
		private void LevelUpPunch()
		{
			PunchReceiver punchReceiver = this.levelDisplayPunchReceiver;
			if (punchReceiver != null)
			{
				punchReceiver.Punch();
			}
			PunchReceiver punchReceiver2 = this.barPunchReceiver;
			if (punchReceiver2 != null)
			{
				punchReceiver2.Punch();
			}
			AudioManager.Post(this.sfx_LvUp);
		}

		// Token: 0x060021D7 RID: 8663 RVA: 0x00075D85 File Offset: 0x00073F85
		internal override void TryQuit()
		{
		}

		// Token: 0x040016D6 RID: 5846
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x040016D7 RID: 5847
		[SerializeField]
		private FadeGroup contentFadeGroup;

		// Token: 0x040016D8 RID: 5848
		[SerializeField]
		private TextMeshProUGUI titleText;

		// Token: 0x040016D9 RID: 5849
		[SerializeField]
		[LocalizationKey("Default")]
		private string evacuatedTitleTextKey = "UI_Closure_Escaped";

		// Token: 0x040016DA RID: 5850
		[SerializeField]
		private Color evacuatedTitleTextColor = Color.white;

		// Token: 0x040016DB RID: 5851
		[SerializeField]
		[LocalizationKey("Default")]
		private string failedTitleTextKey = "UI_Closure_Dead";

		// Token: 0x040016DC RID: 5852
		[SerializeField]
		private Color failedTitleTextColor = Color.red;

		// Token: 0x040016DD RID: 5853
		[SerializeField]
		private GameObject damageInfoContainer;

		// Token: 0x040016DE RID: 5854
		[SerializeField]
		private TextMeshProUGUI damageSourceText;

		// Token: 0x040016DF RID: 5855
		[SerializeField]
		private Image expBar_OldFill;

		// Token: 0x040016E0 RID: 5856
		[SerializeField]
		private Image expBar_CurrentFill;

		// Token: 0x040016E1 RID: 5857
		[SerializeField]
		private AnimationCurve expBarAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040016E2 RID: 5858
		[SerializeField]
		private float expBarAnimationTime = 3f;

		// Token: 0x040016E3 RID: 5859
		[SerializeField]
		private TextMeshProUGUI expDisplay;

		// Token: 0x040016E4 RID: 5860
		[SerializeField]
		private string expFormat = "{0}/<sub>{1}</sub>";

		// Token: 0x040016E5 RID: 5861
		[SerializeField]
		private TextMeshProUGUI levelDisplay;

		// Token: 0x040016E6 RID: 5862
		[SerializeField]
		private string levelFormat = "Lv.{0}";

		// Token: 0x040016E7 RID: 5863
		[SerializeField]
		private PunchReceiver levelDisplayPunchReceiver;

		// Token: 0x040016E8 RID: 5864
		[SerializeField]
		private PunchReceiver barPunchReceiver;

		// Token: 0x040016E9 RID: 5865
		[SerializeField]
		private Button continueButton;

		// Token: 0x040016EA RID: 5866
		[SerializeField]
		private PunchReceiver continueButtonPunchReceiver;

		// Token: 0x040016EB RID: 5867
		private string sfx_Pop = "UI/pop";

		// Token: 0x040016EC RID: 5868
		private string sfx_ExpUp = "UI/exp_up";

		// Token: 0x040016ED RID: 5869
		private string sfx_LvUp = "UI/level_up";

		// Token: 0x040016EE RID: 5870
		private bool continueButtonClicked;

		// Token: 0x040016EF RID: 5871
		private bool canContinue;

		// Token: 0x040016F0 RID: 5872
		private float lastTimeExpUpSfxPlayed = float.MinValue;

		// Token: 0x040016F1 RID: 5873
		private const float minIntervalForExpUpSfx = 0.05f;

		// Token: 0x040016F2 RID: 5874
		private int cachedLevel = -1;

		// Token: 0x040016F3 RID: 5875
		[TupleElementNames(new string[] { "from", "to" })]
		private ValueTuple<long, long> cachedLevelRange;

		// Token: 0x040016F4 RID: 5876
		private long cachedLevelLength;

		// Token: 0x040016F5 RID: 5877
		private int displayingLevel = -1;
	}
}
