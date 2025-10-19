using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Duckov.Tasks
{
	// Token: 0x02000371 RID: 881
	public class PlayTimelineTask : MonoBehaviour, ITaskBehaviour
	{
		// Token: 0x06001E6D RID: 7789 RVA: 0x0006B13C File Offset: 0x0006933C
		private void Awake()
		{
			this.timeline.stopped += this.OnTimelineStopped;
		}

		// Token: 0x06001E6E RID: 7790 RVA: 0x0006B155 File Offset: 0x00069355
		private void OnDestroy()
		{
			if (this.timeline != null)
			{
				this.timeline.stopped -= this.OnTimelineStopped;
			}
		}

		// Token: 0x06001E6F RID: 7791 RVA: 0x0006B17C File Offset: 0x0006937C
		private void OnTimelineStopped(PlayableDirector director)
		{
			this.running = false;
		}

		// Token: 0x06001E70 RID: 7792 RVA: 0x0006B185 File Offset: 0x00069385
		public void Begin()
		{
			this.running = true;
			this.timeline.Play();
		}

		// Token: 0x06001E71 RID: 7793 RVA: 0x0006B199 File Offset: 0x00069399
		public bool IsComplete()
		{
			return this.timeline.time > this.timeline.duration - 0.009999999776482582 || this.timeline.state != PlayState.Playing;
		}

		// Token: 0x06001E72 RID: 7794 RVA: 0x0006B1D0 File Offset: 0x000693D0
		public bool IsPending()
		{
			return this.timeline.time <= this.timeline.duration - 0.009999999776482582 && this.timeline.state == PlayState.Playing;
		}

		// Token: 0x06001E73 RID: 7795 RVA: 0x0006B204 File Offset: 0x00069404
		public void Skip()
		{
			this.timeline.Stop();
		}

		// Token: 0x040014D3 RID: 5331
		[SerializeField]
		private PlayableDirector timeline;

		// Token: 0x040014D4 RID: 5332
		private bool running;
	}
}
