using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200012A RID: 298
public class SceneLoadingEventsReceiver : MonoBehaviour
{
	// Token: 0x060009C8 RID: 2504 RVA: 0x0002A0FA File Offset: 0x000282FA
	private void OnEnable()
	{
		SceneLoader.onStartedLoadingScene += this.OnStartedLoadingScene;
		SceneLoader.onFinishedLoadingScene += this.OnFinishedLoadingScene;
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x0002A11E File Offset: 0x0002831E
	private void OnDisable()
	{
		SceneLoader.onStartedLoadingScene -= this.OnStartedLoadingScene;
		SceneLoader.onFinishedLoadingScene -= this.OnFinishedLoadingScene;
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x0002A142 File Offset: 0x00028342
	private void OnStartedLoadingScene(SceneLoadingContext context)
	{
		UnityEvent unityEvent = this.onStartLoadingScene;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x0002A154 File Offset: 0x00028354
	private void OnFinishedLoadingScene(SceneLoadingContext context)
	{
		UnityEvent unityEvent = this.onFinishedLoadingScene;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x04000889 RID: 2185
	public UnityEvent onStartLoadingScene;

	// Token: 0x0400088A RID: 2186
	public UnityEvent onFinishedLoadingScene;
}
