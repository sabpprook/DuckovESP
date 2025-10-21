using System;
using Duckov.UI.Animations;
using TMPro;
using UnityEngine;

// Token: 0x02000161 RID: 353
public class LevelInitializingIndicator : MonoBehaviour
{
	// Token: 0x06000AC1 RID: 2753 RVA: 0x0002E794 File Offset: 0x0002C994
	private void Awake()
	{
		SceneLoader.onBeforeSetSceneActive += this.SceneLoader_onBeforeSetSceneActive;
		SceneLoader.onAfterSceneInitialize += this.SceneLoader_onAfterSceneInitialize;
		LevelManager.OnLevelInitializingCommentChanged += this.OnCommentChanged;
		SceneLoader.OnSetLoadingComment += this.OnSetLoadingComment;
		this.fadeGroup.SkipHide();
	}

	// Token: 0x06000AC2 RID: 2754 RVA: 0x0002E7F0 File Offset: 0x0002C9F0
	private void OnSetLoadingComment(string comment)
	{
		this.levelInitializationCommentText.text = SceneLoader.LoadingComment;
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x0002E802 File Offset: 0x0002CA02
	private void OnCommentChanged(string comment)
	{
		this.levelInitializationCommentText.text = SceneLoader.LoadingComment;
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x0002E814 File Offset: 0x0002CA14
	private void OnDestroy()
	{
		SceneLoader.onBeforeSetSceneActive -= this.SceneLoader_onBeforeSetSceneActive;
		SceneLoader.onAfterSceneInitialize -= this.SceneLoader_onAfterSceneInitialize;
		LevelManager.OnLevelInitializingCommentChanged -= this.OnCommentChanged;
		SceneLoader.OnSetLoadingComment -= this.OnSetLoadingComment;
	}

	// Token: 0x06000AC5 RID: 2757 RVA: 0x0002E865 File Offset: 0x0002CA65
	private void SceneLoader_onBeforeSetSceneActive(SceneLoadingContext obj)
	{
		this.fadeGroup.Show();
		this.levelInitializationCommentText.text = LevelManager.LevelInitializingComment;
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x0002E882 File Offset: 0x0002CA82
	private void SceneLoader_onAfterSceneInitialize(SceneLoadingContext obj)
	{
		this.fadeGroup.Hide();
	}

	// Token: 0x04000951 RID: 2385
	[SerializeField]
	private FadeGroup fadeGroup;

	// Token: 0x04000952 RID: 2386
	[SerializeField]
	private TextMeshProUGUI levelInitializationCommentText;
}
