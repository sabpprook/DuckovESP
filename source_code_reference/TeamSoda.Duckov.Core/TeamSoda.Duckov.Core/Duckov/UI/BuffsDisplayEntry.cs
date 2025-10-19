using System;
using Cysharp.Threading.Tasks;
using Duckov.Buffs;
using Duckov.UI.Animations;
using Duckov.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x02000375 RID: 885
	public class BuffsDisplayEntry : MonoBehaviour, IPoolable, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x140000D0 RID: 208
		// (add) Token: 0x06001E92 RID: 7826 RVA: 0x0006B6E4 File Offset: 0x000698E4
		// (remove) Token: 0x06001E93 RID: 7827 RVA: 0x0006B718 File Offset: 0x00069918
		public static event Action<BuffsDisplayEntry, PointerEventData> OnBuffsDisplayEntryClicked;

		// Token: 0x170005E9 RID: 1513
		// (get) Token: 0x06001E94 RID: 7828 RVA: 0x0006B74B File Offset: 0x0006994B
		public Image Icon
		{
			get
			{
				return this.icon;
			}
		}

		// Token: 0x06001E95 RID: 7829 RVA: 0x0006B754 File Offset: 0x00069954
		public void Setup(BuffsDisplay master, Buff target)
		{
			this.master = master;
			this.target = target;
			this.icon.sprite = target.Icon;
			if (this.displayName)
			{
				this.displayName.text = target.DisplayName;
			}
			this.fadeGroup.Show();
		}

		// Token: 0x06001E96 RID: 7830 RVA: 0x0006B7A9 File Offset: 0x000699A9
		private void Update()
		{
			this.Refresh();
		}

		// Token: 0x06001E97 RID: 7831 RVA: 0x0006B7B4 File Offset: 0x000699B4
		private void Refresh()
		{
			if (this.target == null)
			{
				this.Release();
				return;
			}
			if (this.target.LimitedLifeTime)
			{
				this.remainingTimeText.text = string.Format(this.timeFormat, this.target.RemainingTime);
			}
			else
			{
				this.remainingTimeText.text = "";
			}
			if (this.target.MaxLayers > 1)
			{
				this.layersText.text = this.target.CurrentLayers.ToString();
				return;
			}
			this.layersText.text = "";
		}

		// Token: 0x06001E98 RID: 7832 RVA: 0x0006B858 File Offset: 0x00069A58
		public void Release()
		{
			if (this.releasing)
			{
				return;
			}
			this.releasing = true;
			this.ReleaseTask().Forget();
		}

		// Token: 0x06001E99 RID: 7833 RVA: 0x0006B878 File Offset: 0x00069A78
		private async UniTask ReleaseTask()
		{
			await this.fadeGroup.HideAndReturnTask();
			if (this.pooled)
			{
				this.master.ReleaseEntry(this);
			}
		}

		// Token: 0x170005EA RID: 1514
		// (get) Token: 0x06001E9A RID: 7834 RVA: 0x0006B8BB File Offset: 0x00069ABB
		public Buff Target
		{
			get
			{
				return this.target;
			}
		}

		// Token: 0x06001E9B RID: 7835 RVA: 0x0006B8C3 File Offset: 0x00069AC3
		public void NotifyPooled()
		{
			this.pooled = true;
			this.releasing = false;
		}

		// Token: 0x06001E9C RID: 7836 RVA: 0x0006B8D3 File Offset: 0x00069AD3
		public void NotifyReleased()
		{
			this.pooled = false;
			this.target = null;
			this.releasing = false;
		}

		// Token: 0x06001E9D RID: 7837 RVA: 0x0006B8EA File Offset: 0x00069AEA
		public void OnPointerClick(PointerEventData eventData)
		{
			PunchReceiver punchReceiver = this.punchReceiver;
			if (punchReceiver != null)
			{
				punchReceiver.Punch();
			}
			Action<BuffsDisplayEntry, PointerEventData> onBuffsDisplayEntryClicked = BuffsDisplayEntry.OnBuffsDisplayEntryClicked;
			if (onBuffsDisplayEntryClicked == null)
			{
				return;
			}
			onBuffsDisplayEntryClicked(this, eventData);
		}

		// Token: 0x040014E7 RID: 5351
		[SerializeField]
		private Image icon;

		// Token: 0x040014E8 RID: 5352
		[SerializeField]
		private TextMeshProUGUI remainingTimeText;

		// Token: 0x040014E9 RID: 5353
		[SerializeField]
		private TextMeshProUGUI layersText;

		// Token: 0x040014EA RID: 5354
		[SerializeField]
		private TextMeshProUGUI displayName;

		// Token: 0x040014EB RID: 5355
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x040014EC RID: 5356
		[SerializeField]
		private PunchReceiver punchReceiver;

		// Token: 0x040014ED RID: 5357
		[SerializeField]
		private string timeFormat = "{0:0}s";

		// Token: 0x040014EE RID: 5358
		private BuffsDisplay master;

		// Token: 0x040014EF RID: 5359
		private Buff target;

		// Token: 0x040014F0 RID: 5360
		private bool releasing;

		// Token: 0x040014F1 RID: 5361
		private bool pooled;
	}
}
