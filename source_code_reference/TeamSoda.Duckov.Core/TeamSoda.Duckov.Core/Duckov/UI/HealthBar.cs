using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Duckov.UI.Animations;
using Duckov.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x020003A0 RID: 928
	public class HealthBar : MonoBehaviour, IPoolable
	{
		// Token: 0x17000658 RID: 1624
		// (get) Token: 0x06002117 RID: 8471 RVA: 0x000735EB File Offset: 0x000717EB
		// (set) Token: 0x06002118 RID: 8472 RVA: 0x000735F3 File Offset: 0x000717F3
		public Health target { get; private set; }

		// Token: 0x17000659 RID: 1625
		// (get) Token: 0x06002119 RID: 8473 RVA: 0x000735FC File Offset: 0x000717FC
		private PrefabPool<HealthBar_DamageBar> DamageBarPool
		{
			get
			{
				if (this._damageBarPool == null)
				{
					this._damageBarPool = new PrefabPool<HealthBar_DamageBar>(this.damageBarTemplate, null, null, null, null, true, 10, 10000, null);
				}
				return this._damageBarPool;
			}
		}

		// Token: 0x0600211A RID: 8474 RVA: 0x00073635 File Offset: 0x00071835
		public void NotifyPooled()
		{
			this.pooled = true;
		}

		// Token: 0x0600211B RID: 8475 RVA: 0x0007363E File Offset: 0x0007183E
		public void NotifyReleased()
		{
			this.UnregisterEvents();
			this.target = null;
			this.pooled = false;
		}

		// Token: 0x0600211C RID: 8476 RVA: 0x00073654 File Offset: 0x00071854
		private void Awake()
		{
			this.rectTransform = base.transform as RectTransform;
		}

		// Token: 0x0600211D RID: 8477 RVA: 0x00073667 File Offset: 0x00071867
		private void OnDestroy()
		{
			this.UnregisterEvents();
			Image image = this.followFill;
			if (image != null)
			{
				image.DOKill(false);
			}
			Image image2 = this.hurtBlink;
			if (image2 == null)
			{
				return;
			}
			image2.DOKill(false);
		}

		// Token: 0x0600211E RID: 8478 RVA: 0x00073694 File Offset: 0x00071894
		private void LateUpdate()
		{
			if (this.target == null || !this.target.isActiveAndEnabled || this.target.Hidden)
			{
				this.Release();
				return;
			}
			this.UpdatePosition();
		}

		// Token: 0x0600211F RID: 8479 RVA: 0x000736CC File Offset: 0x000718CC
		private bool CheckInFrame()
		{
			this.rectTransform.GetWorldCorners(this.cornersBuffer);
			foreach (Vector3 vector in this.cornersBuffer)
			{
				if (vector.x > 0f && vector.x < (float)Screen.width && vector.y > 0f && vector.y < (float)Screen.height)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002120 RID: 8480 RVA: 0x00073745 File Offset: 0x00071945
		private void UpdateFrame()
		{
			if (this.CheckInFrame())
			{
				this.lastTimeInFrame = Time.unscaledTime;
			}
			if (Time.unscaledTime - this.lastTimeInFrame > this.releaseAfterOutOfFrame)
			{
				this.Release();
			}
		}

		// Token: 0x06002121 RID: 8481 RVA: 0x00073774 File Offset: 0x00071974
		private void UpdatePosition()
		{
			Vector3 vector = this.target.transform.position + this.displayOffset;
			Vector3 vector2 = Camera.main.WorldToScreenPoint(vector);
			vector2.y += this.screenYOffset * (float)Screen.height;
			base.transform.position = vector2;
		}

		// Token: 0x06002122 RID: 8482 RVA: 0x000737D0 File Offset: 0x000719D0
		public void Setup(Health target, DamageInfo? damage = null, Action releaseAction = null)
		{
			this.releaseAction = releaseAction;
			this.UnregisterEvents();
			if (target == null)
			{
				this.Release();
				return;
			}
			if (target.IsDead)
			{
				this.Release();
				return;
			}
			this.background.SetActive(true);
			this.deathIndicator.SetActive(false);
			this.fill.gameObject.SetActive(true);
			this.followFill.gameObject.SetActive(true);
			this.target = target;
			this.RefreshOffset();
			this.RegisterEvents();
			this.Refresh();
			this.lastTimeInFrame = Time.unscaledTime;
			this.damageBarTemplate.gameObject.SetActive(false);
			if (damage != null)
			{
				this.OnTargetHurt(damage.Value);
			}
			this.UpdatePosition();
		}

		// Token: 0x06002123 RID: 8483 RVA: 0x00073894 File Offset: 0x00071A94
		public void RefreshOffset()
		{
			if (!this.target)
			{
				return;
			}
			this.displayOffset = Vector3.up * 1.5f;
			CharacterMainControl characterMainControl = this.target.TryGetCharacter();
			if (characterMainControl && characterMainControl.characterModel)
			{
				Transform helmatSocket = characterMainControl.characterModel.HelmatSocket;
				if (helmatSocket)
				{
					this.displayOffset = Vector3.up * (Vector3.Distance(characterMainControl.transform.position, helmatSocket.position) + 0.5f);
				}
			}
		}

		// Token: 0x06002124 RID: 8484 RVA: 0x00073928 File Offset: 0x00071B28
		private void RegisterEvents()
		{
			if (this.target == null)
			{
				return;
			}
			this.RefreshCharacterIcon();
			this.target.OnMaxHealthChange.AddListener(new UnityAction<Health>(this.OnTargetMaxHealthChange));
			this.target.OnHealthChange.AddListener(new UnityAction<Health>(this.OnTargetHealthChange));
			this.target.OnHurtEvent.AddListener(new UnityAction<DamageInfo>(this.OnTargetHurt));
			this.target.OnDeadEvent.AddListener(new UnityAction<DamageInfo>(this.OnTargetDead));
		}

		// Token: 0x06002125 RID: 8485 RVA: 0x000739BC File Offset: 0x00071BBC
		private void RefreshCharacterIcon()
		{
			if (!this.target)
			{
				this.levelIcon.gameObject.SetActive(false);
				this.nameText.gameObject.SetActive(false);
				return;
			}
			CharacterMainControl characterMainControl = this.target.TryGetCharacter();
			if (!characterMainControl)
			{
				this.levelIcon.gameObject.SetActive(false);
				this.nameText.gameObject.SetActive(false);
				return;
			}
			CharacterRandomPreset characterPreset = characterMainControl.characterPreset;
			if (!characterPreset)
			{
				this.levelIcon.gameObject.SetActive(false);
				this.nameText.gameObject.SetActive(false);
				return;
			}
			Sprite characterIcon = characterPreset.GetCharacterIcon();
			if (!characterIcon)
			{
				this.levelIcon.gameObject.SetActive(false);
			}
			else
			{
				this.levelIcon.sprite = characterIcon;
				this.levelIcon.gameObject.SetActive(true);
			}
			if (!characterPreset.showName)
			{
				this.nameText.gameObject.SetActive(false);
				return;
			}
			this.nameText.text = characterPreset.DisplayName;
			this.nameText.gameObject.SetActive(true);
		}

		// Token: 0x06002126 RID: 8486 RVA: 0x00073AE0 File Offset: 0x00071CE0
		private void UnregisterEvents()
		{
			if (this.target == null)
			{
				return;
			}
			this.target.OnMaxHealthChange.RemoveListener(new UnityAction<Health>(this.OnTargetMaxHealthChange));
			this.target.OnHealthChange.RemoveListener(new UnityAction<Health>(this.OnTargetHealthChange));
			this.target.OnHurtEvent.RemoveListener(new UnityAction<DamageInfo>(this.OnTargetHurt));
			this.target.OnDeadEvent.RemoveListener(new UnityAction<DamageInfo>(this.OnTargetDead));
		}

		// Token: 0x06002127 RID: 8487 RVA: 0x00073B6C File Offset: 0x00071D6C
		private void OnTargetMaxHealthChange(Health obj)
		{
			this.Refresh();
		}

		// Token: 0x06002128 RID: 8488 RVA: 0x00073B74 File Offset: 0x00071D74
		private void OnTargetHealthChange(Health obj)
		{
			this.Refresh();
		}

		// Token: 0x06002129 RID: 8489 RVA: 0x00073B7C File Offset: 0x00071D7C
		private void OnTargetHurt(DamageInfo damage)
		{
			Color blinkEndColor = this.blinkColor;
			blinkEndColor.a = 0f;
			if (this.hurtBlink != null)
			{
				this.hurtBlink.DOColor(this.blinkColor, this.blinkDuration).From<TweenerCore<Color, Color, ColorOptions>>().OnKill(delegate
				{
					if (this.hurtBlink != null)
					{
						this.hurtBlink.color = blinkEndColor;
					}
				});
			}
			UnityEvent unityEvent = this.onHurt;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.ShowDamageBar(damage.finalDamage);
		}

		// Token: 0x0600212A RID: 8490 RVA: 0x00073C0C File Offset: 0x00071E0C
		private void OnTargetDead(DamageInfo damage)
		{
			this.UnregisterEvents();
			UnityEvent unityEvent = this.onDead;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			if (!damage.toDamageReceiver || !damage.toDamageReceiver.health)
			{
				return;
			}
			this.DeathTask(damage.toDamageReceiver.health).Forget();
		}

		// Token: 0x0600212B RID: 8491 RVA: 0x00073C68 File Offset: 0x00071E68
		internal void Release()
		{
			if (!this.pooled)
			{
				return;
			}
			if (this.target != null && this.target.IsMainCharacterHealth && !this.target.IsDead && this.target.gameObject.activeInHierarchy)
			{
				return;
			}
			this.UnregisterEvents();
			this.target != null;
			this.target = null;
			Action action = this.releaseAction;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x0600212C RID: 8492 RVA: 0x00073CE4 File Offset: 0x00071EE4
		private void Refresh()
		{
			float currentHealth = this.target.CurrentHealth;
			float maxHealth = this.target.MaxHealth;
			float num = 0f;
			if (maxHealth > 0f)
			{
				num = currentHealth / maxHealth;
			}
			this.fill.fillAmount = num;
			this.fill.color = this.colorOverAmount.Evaluate(num);
			if (this.followFill != null)
			{
				this.followFill.DOKill(false);
				this.followFill.DOFillAmount(num, this.followFillDuration);
			}
		}

		// Token: 0x0600212D RID: 8493 RVA: 0x00073D70 File Offset: 0x00071F70
		private void ShowDamageBar(float damageAmount)
		{
			float num = Mathf.Clamp01(damageAmount / this.target.MaxHealth);
			float num2 = Mathf.Clamp01(this.target.CurrentHealth / this.target.MaxHealth);
			float width = this.fill.rectTransform.rect.width;
			float num3 = width * num;
			float num4 = width * num2;
			HealthBar_DamageBar damageBar = this.DamageBarPool.Get(null);
			damageBar.Animate(num4, num3, delegate
			{
				this.DamageBarPool.Release(damageBar);
			}).Forget();
		}

		// Token: 0x0600212E RID: 8494 RVA: 0x00073E10 File Offset: 0x00072010
		private async UniTask DeathTask(Health health)
		{
			GameObject gameObject = this.background;
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
			GameObject gameObject2 = this.deathIndicator;
			if (gameObject2 != null)
			{
				gameObject2.SetActive(true);
			}
			Image image = this.fill;
			if (image != null)
			{
				image.gameObject.SetActive(false);
			}
			Image image2 = this.followFill;
			if (image2 != null)
			{
				image2.gameObject.SetActive(false);
			}
			PunchReceiver punchReceiver = this.deathIndicatorPunchReceiver;
			if (punchReceiver != null)
			{
				punchReceiver.Punch();
			}
			await UniTask.WaitForSeconds(this.disappearDelay, true, PlayerLoopTiming.Update, default(CancellationToken), false);
			if (health == this.target)
			{
				this.Release();
			}
		}

		// Token: 0x04001676 RID: 5750
		private RectTransform rectTransform;

		// Token: 0x04001677 RID: 5751
		[SerializeField]
		private GameObject background;

		// Token: 0x04001678 RID: 5752
		[SerializeField]
		private Image fill;

		// Token: 0x04001679 RID: 5753
		[SerializeField]
		private Image followFill;

		// Token: 0x0400167A RID: 5754
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x0400167B RID: 5755
		[SerializeField]
		private GameObject deathIndicator;

		// Token: 0x0400167C RID: 5756
		[SerializeField]
		private PunchReceiver deathIndicatorPunchReceiver;

		// Token: 0x0400167D RID: 5757
		[SerializeField]
		private Image hurtBlink;

		// Token: 0x0400167E RID: 5758
		[SerializeField]
		private HealthBar_DamageBar damageBarTemplate;

		// Token: 0x0400167F RID: 5759
		[SerializeField]
		private Gradient colorOverAmount = new Gradient();

		// Token: 0x04001680 RID: 5760
		[SerializeField]
		private float followFillDuration = 0.5f;

		// Token: 0x04001681 RID: 5761
		[SerializeField]
		private float blinkDuration = 0.1f;

		// Token: 0x04001682 RID: 5762
		[SerializeField]
		private Color blinkColor = Color.white;

		// Token: 0x04001683 RID: 5763
		private Vector3 displayOffset = Vector3.zero;

		// Token: 0x04001684 RID: 5764
		[SerializeField]
		private float releaseAfterOutOfFrame = 1f;

		// Token: 0x04001685 RID: 5765
		[SerializeField]
		private float disappearDelay = 0.2f;

		// Token: 0x04001686 RID: 5766
		[SerializeField]
		private Image levelIcon;

		// Token: 0x04001687 RID: 5767
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x04001688 RID: 5768
		[SerializeField]
		private UnityEvent onHurt;

		// Token: 0x04001689 RID: 5769
		[SerializeField]
		private UnityEvent onDead;

		// Token: 0x0400168B RID: 5771
		private Action releaseAction;

		// Token: 0x0400168C RID: 5772
		private float lastTimeInFrame = float.MinValue;

		// Token: 0x0400168D RID: 5773
		private float screenYOffset = 0.02f;

		// Token: 0x0400168E RID: 5774
		private PrefabPool<HealthBar_DamageBar> _damageBarPool;

		// Token: 0x0400168F RID: 5775
		private bool pooled;

		// Token: 0x04001690 RID: 5776
		private Vector3[] cornersBuffer = new Vector3[4];
	}
}
