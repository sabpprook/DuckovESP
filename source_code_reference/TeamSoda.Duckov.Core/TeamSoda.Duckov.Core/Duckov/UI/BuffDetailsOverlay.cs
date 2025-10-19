using System;
using Duckov.Buffs;
using Duckov.UI.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.UI
{
	// Token: 0x02000373 RID: 883
	public class BuffDetailsOverlay : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x170005E6 RID: 1510
		// (get) Token: 0x06001E7A RID: 7802 RVA: 0x0006B241 File Offset: 0x00069441
		public Buff Target
		{
			get
			{
				return this.target;
			}
		}

		// Token: 0x06001E7B RID: 7803 RVA: 0x0006B249 File Offset: 0x00069449
		private void Awake()
		{
			this.rectTransform = base.transform as RectTransform;
			BuffsDisplayEntry.OnBuffsDisplayEntryClicked += this.OnBuffsDisplayEntryClicked;
		}

		// Token: 0x06001E7C RID: 7804 RVA: 0x0006B26D File Offset: 0x0006946D
		private void OnDestroy()
		{
			BuffsDisplayEntry.OnBuffsDisplayEntryClicked -= this.OnBuffsDisplayEntryClicked;
		}

		// Token: 0x06001E7D RID: 7805 RVA: 0x0006B280 File Offset: 0x00069480
		private void OnBuffsDisplayEntryClicked(BuffsDisplayEntry entry, PointerEventData eventData)
		{
			if (this.fadeGroup.IsShown && this.target == entry.Target)
			{
				this.fadeGroup.Hide();
				this.punchReceiver.Punch();
				return;
			}
			this.Setup(entry);
			this.Show();
			this.punchReceiver.Punch();
		}

		// Token: 0x06001E7E RID: 7806 RVA: 0x0006B2DC File Offset: 0x000694DC
		public void Setup(Buff target)
		{
			this.target = target;
			if (target == null)
			{
				return;
			}
			this.text_BuffName.text = target.DisplayName;
			this.text_BuffDescription.text = target.Description;
			this.RefreshCountDown();
		}

		// Token: 0x06001E7F RID: 7807 RVA: 0x0006B318 File Offset: 0x00069518
		private void Update()
		{
			if (this.fadeGroup.IsShown || this.fadeGroup.IsShowingInProgress)
			{
				if (this.target != null)
				{
					this.RefreshCountDown();
				}
				else
				{
					this.fadeGroup.Hide();
				}
				if (this.TimeSinceShowStarted > this.disappearAfterSeconds)
				{
					this.fadeGroup.Hide();
				}
			}
		}

		// Token: 0x06001E80 RID: 7808 RVA: 0x0006B37C File Offset: 0x0006957C
		public void Setup(BuffsDisplayEntry target)
		{
			if (target == null)
			{
				return;
			}
			this.Setup((target != null) ? target.Target : null);
			RectTransform rectTransform = target.Icon.rectTransform;
			Vector3 vector = rectTransform.TransformPoint(rectTransform.rect.max);
			this.rectTransform.pivot = Vector2.up;
			this.rectTransform.position = vector;
			this.rectTransform.SetAsLastSibling();
		}

		// Token: 0x06001E81 RID: 7809 RVA: 0x0006B3F0 File Offset: 0x000695F0
		private void RefreshCountDown()
		{
			if (this.target == null)
			{
				return;
			}
			if (this.target.LimitedLifeTime)
			{
				float remainingTime = this.target.RemainingTime;
				this.text_CountDown.text = string.Format("{0:0.0}s", remainingTime);
				return;
			}
			this.text_CountDown.text = "";
		}

		// Token: 0x170005E7 RID: 1511
		// (get) Token: 0x06001E82 RID: 7810 RVA: 0x0006B451 File Offset: 0x00069651
		private float TimeSinceShowStarted
		{
			get
			{
				return Time.unscaledTime - this.timeWhenShowStarted;
			}
		}

		// Token: 0x06001E83 RID: 7811 RVA: 0x0006B45F File Offset: 0x0006965F
		public void Show()
		{
			this.fadeGroup.Show();
			this.timeWhenShowStarted = Time.unscaledTime;
		}

		// Token: 0x06001E84 RID: 7812 RVA: 0x0006B477 File Offset: 0x00069677
		public void OnPointerClick(PointerEventData eventData)
		{
			if (this.fadeGroup.IsShown || this.fadeGroup.IsShowingInProgress)
			{
				this.punchReceiver.Punch();
				this.fadeGroup.Hide();
			}
		}

		// Token: 0x040014D9 RID: 5337
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x040014DA RID: 5338
		[SerializeField]
		private TextMeshProUGUI text_BuffName;

		// Token: 0x040014DB RID: 5339
		[SerializeField]
		private TextMeshProUGUI text_BuffDescription;

		// Token: 0x040014DC RID: 5340
		[SerializeField]
		private TextMeshProUGUI text_CountDown;

		// Token: 0x040014DD RID: 5341
		[SerializeField]
		private PunchReceiver punchReceiver;

		// Token: 0x040014DE RID: 5342
		[SerializeField]
		private float disappearAfterSeconds = 5f;

		// Token: 0x040014DF RID: 5343
		private RectTransform rectTransform;

		// Token: 0x040014E0 RID: 5344
		private Buff target;

		// Token: 0x040014E1 RID: 5345
		private float timeWhenShowStarted;
	}
}
