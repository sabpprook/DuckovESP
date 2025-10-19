using System;
using Cysharp.Threading.Tasks;
using Duckov;
using Duckov.UI.Animations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;

// Token: 0x02000168 RID: 360
public class Title : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x06000AEA RID: 2794 RVA: 0x0002EC04 File Offset: 0x0002CE04
	private void Start()
	{
		this.StartTask().Forget();
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x0002EC14 File Offset: 0x0002CE14
	private async UniTask StartTask()
	{
		this.timelineToTitle.Play();
		await this.WaitForTimeline(this.timelineToTitle);
		this.fadeGroup.Show();
		this.timelineToTitle.gameObject.SetActive(false);
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x0002EC58 File Offset: 0x0002CE58
	private async UniTask ContinueTask()
	{
		this.fadeGroup.Hide();
		AudioManager.Post(this.sfx_PressStart);
		this.timelineToTitle.gameObject.SetActive(false);
		this.timelineToMainMenu.gameObject.SetActive(true);
		this.timelineToMainMenu.Play();
		AudioManager.PlayBGM("mus_title");
		await this.WaitForTimeline(this.timelineToMainMenu);
		if (this.timelineToMainMenu != null)
		{
			this.timelineToMainMenu.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x0002EC9C File Offset: 0x0002CE9C
	private async UniTask WaitForTimeline(PlayableDirector timeline)
	{
		while (timeline != null && timeline.state == PlayState.Playing)
		{
			await UniTask.NextFrame();
		}
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x0002ECDF File Offset: 0x0002CEDF
	public void OnPointerClick(PointerEventData eventData)
	{
		if (this.fadeGroup.IsShown)
		{
			this.ContinueTask().Forget();
		}
	}

	// Token: 0x04000967 RID: 2407
	[SerializeField]
	private FadeGroup fadeGroup;

	// Token: 0x04000968 RID: 2408
	[SerializeField]
	private PlayableDirector timelineToTitle;

	// Token: 0x04000969 RID: 2409
	[SerializeField]
	private PlayableDirector timelineToMainMenu;

	// Token: 0x0400096A RID: 2410
	private string sfx_PressStart = "UI/game_start";
}
