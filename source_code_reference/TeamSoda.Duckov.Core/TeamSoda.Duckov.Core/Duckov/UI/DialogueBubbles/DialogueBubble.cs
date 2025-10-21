using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.UI.DialogueBubbles
{
	// Token: 0x020003E2 RID: 994
	public class DialogueBubble : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x170006D2 RID: 1746
		// (get) Token: 0x060023EB RID: 9195 RVA: 0x0007D1FA File Offset: 0x0007B3FA
		public Transform Target
		{
			get
			{
				return this.target;
			}
		}

		// Token: 0x170006D3 RID: 1747
		// (get) Token: 0x060023EC RID: 9196 RVA: 0x0007D202 File Offset: 0x0007B402
		private float YOffset
		{
			get
			{
				if (this._yOffset < 0f)
				{
					return this.defaultYOffset;
				}
				return this._yOffset;
			}
		}

		// Token: 0x060023ED RID: 9197 RVA: 0x0007D21E File Offset: 0x0007B41E
		private void LateUpdate()
		{
			this.UpdatePosition();
		}

		// Token: 0x060023EE RID: 9198 RVA: 0x0007D228 File Offset: 0x0007B428
		private void UpdatePosition()
		{
			if (this.target == null)
			{
				return;
			}
			Vector2 vector = RectTransformUtility.WorldToScreenPoint(Camera.main, this.target.position + Vector3.up * this.YOffset);
			vector.y += this.screenYOffset * (float)Screen.height;
			Vector2 vector2;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(base.transform.parent as RectTransform, vector, null, out vector2))
			{
				return;
			}
			base.transform.localPosition = vector2;
		}

		// Token: 0x060023EF RID: 9199 RVA: 0x0007D2B4 File Offset: 0x0007B4B4
		public UniTask Show(string text, Transform target, float yOffset = -1f, bool needInteraction = false, bool skippable = false, float speed = -1f, float duration = 2f)
		{
			this.task = this.ShowTask(text, target, yOffset, needInteraction, skippable, speed, duration);
			return this.task;
		}

		// Token: 0x060023F0 RID: 9200 RVA: 0x0007D2E0 File Offset: 0x0007B4E0
		public async UniTask ShowTask(string text, Transform target, float yOffset = -1f, bool needInteraction = false, bool skippable = false, float speed = -1f, float duration = 2f)
		{
			this._yOffset = yOffset;
			this.target = target;
			this.sustainDuration = duration;
			this.interactIndicator.gameObject.SetActive(false);
			int currentToken = global::UnityEngine.Random.Range(1, int.MaxValue);
			this.taskToken = currentToken;
			TMP_TextInfo textInfo = this.text.GetTextInfo(text);
			if (textInfo.characterCount < 1)
			{
				this.animating = false;
				await this.Hide();
			}
			else
			{
				this.animating = true;
				this.text.text = text;
				this.text.maxVisibleCharacters = 0;
				await this.fadeGroup.ShowAndReturnTask();
				if (this.taskToken == currentToken)
				{
					int characterCount = textInfo.characterCount;
					if (speed <= 0f)
					{
						speed = this.defaultSpeed;
					}
					this.interacted = false;
					for (int i = 0; i <= characterCount; i++)
					{
						this.text.maxVisibleCharacters = i;
						await UniTask.WaitForSeconds(1f / speed, true, PlayerLoopTiming.Update, default(CancellationToken), false);
						if (this.taskToken != currentToken)
						{
							return;
						}
						if (target == null)
						{
							this.Hide().Forget();
							return;
						}
						if (!target.gameObject.activeInHierarchy)
						{
							this.Hide().Forget();
							return;
						}
						if (skippable && this.interacted)
						{
							break;
						}
					}
					this.text.maxVisibleCharacters = characterCount;
					this.animating = false;
					if (!needInteraction)
					{
						float startTime = Time.unscaledTime;
						for (;;)
						{
							await UniTask.NextFrame();
							float num = Time.unscaledTime - startTime;
							if (this.taskToken != currentToken)
							{
								break;
							}
							if (!target || !target.gameObject.activeInHierarchy || num >= this.sustainDuration)
							{
								goto IL_0439;
							}
						}
						return;
					}
					this.interactIndicator.gameObject.SetActive(true);
					await this.WaitForInteraction(currentToken);
					IL_0439:
					if (this.taskToken == currentToken)
					{
						this.Hide().Forget();
					}
				}
			}
		}

		// Token: 0x060023F1 RID: 9201 RVA: 0x0007D360 File Offset: 0x0007B560
		private async UniTask WaitForInteraction(int currentToken)
		{
			this.interacted = false;
			do
			{
				await UniTask.NextFrame();
				if (currentToken != this.taskToken)
				{
					break;
				}
				if (this.interacted)
				{
					break;
				}
			}
			while (this.target.gameObject.activeInHierarchy);
		}

		// Token: 0x060023F2 RID: 9202 RVA: 0x0007D3AB File Offset: 0x0007B5AB
		public void Interact()
		{
			this.interacted = true;
		}

		// Token: 0x060023F3 RID: 9203 RVA: 0x0007D3B4 File Offset: 0x0007B5B4
		private async UniTask Hide()
		{
			this.animating = false;
			await this.fadeGroup.HideAndReturnTask();
		}

		// Token: 0x060023F4 RID: 9204 RVA: 0x0007D3F7 File Offset: 0x0007B5F7
		public void OnPointerClick(PointerEventData eventData)
		{
			this.Interact();
		}

		// Token: 0x060023F5 RID: 9205 RVA: 0x0007D3FF File Offset: 0x0007B5FF
		private void Awake()
		{
			DialogueBubblesManager.onPointerClick += this.OnPointerClick;
		}

		// Token: 0x04001866 RID: 6246
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x04001867 RID: 6247
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x04001868 RID: 6248
		[SerializeField]
		private float defaultSpeed = 10f;

		// Token: 0x04001869 RID: 6249
		[SerializeField]
		private float sustainDuration = 2f;

		// Token: 0x0400186A RID: 6250
		[SerializeField]
		private float defaultYOffset = 2f;

		// Token: 0x0400186B RID: 6251
		[SerializeField]
		private GameObject interactIndicator;

		// Token: 0x0400186C RID: 6252
		private bool interacted;

		// Token: 0x0400186D RID: 6253
		private bool animating;

		// Token: 0x0400186E RID: 6254
		private int taskToken;

		// Token: 0x0400186F RID: 6255
		private Transform target;

		// Token: 0x04001870 RID: 6256
		private float _yOffset;

		// Token: 0x04001871 RID: 6257
		private float screenYOffset = 0.06f;

		// Token: 0x04001872 RID: 6258
		private UniTask task;
	}
}
