using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

// Token: 0x02000139 RID: 313
public class PlayableDirectorEvents : MonoBehaviour
{
	// Token: 0x06000A0A RID: 2570 RVA: 0x0002B04C File Offset: 0x0002924C
	private void OnEnable()
	{
		this.playableDirector.played += this.OnPlayed;
		this.playableDirector.paused += this.OnPaused;
		this.playableDirector.stopped += this.OnStopped;
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x0002B0A0 File Offset: 0x000292A0
	private void OnDisable()
	{
		this.playableDirector.played -= this.OnPlayed;
		this.playableDirector.paused -= this.OnPaused;
		this.playableDirector.stopped -= this.OnStopped;
	}

	// Token: 0x06000A0C RID: 2572 RVA: 0x0002B0F2 File Offset: 0x000292F2
	private void OnStopped(PlayableDirector director)
	{
		UnityEvent unityEvent = this.onStopped;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x0002B104 File Offset: 0x00029304
	private void OnPaused(PlayableDirector director)
	{
		UnityEvent unityEvent = this.onPaused;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x0002B116 File Offset: 0x00029316
	private void OnPlayed(PlayableDirector director)
	{
		UnityEvent unityEvent = this.onPlayed;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x040008C8 RID: 2248
	[SerializeField]
	private PlayableDirector playableDirector;

	// Token: 0x040008C9 RID: 2249
	[SerializeField]
	private UnityEvent onPlayed;

	// Token: 0x040008CA RID: 2250
	[SerializeField]
	private UnityEvent onPaused;

	// Token: 0x040008CB RID: 2251
	[SerializeField]
	private UnityEvent onStopped;
}
