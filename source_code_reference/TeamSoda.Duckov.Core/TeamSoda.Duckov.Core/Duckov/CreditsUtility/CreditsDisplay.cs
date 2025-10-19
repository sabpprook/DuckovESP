using System;
using System.Collections.Generic;
using UnityEngine;

namespace Duckov.CreditsUtility
{
	// Token: 0x020002F6 RID: 758
	public class CreditsDisplay : MonoBehaviour
	{
		// Token: 0x060018A7 RID: 6311 RVA: 0x000599D8 File Offset: 0x00057BD8
		private void ParseAndDisplay()
		{
			this.Reset();
			CreditsLexer creditsLexer = new CreditsLexer(this.content.text);
			this.BeginVerticalLayout(Array.Empty<string>());
			foreach (Token token in creditsLexer)
			{
				if (this.status.records.Count > 0)
				{
					Token token2 = this.status.records[this.status.records.Count - 1];
				}
				this.status.records.Add(token);
				switch (token.type)
				{
				case TokenType.Invalid:
					Debug.LogError("Invalid Token: " + token.text);
					break;
				case TokenType.End:
					goto IL_00F4;
				case TokenType.String:
					this.DoText(token.text);
					break;
				case TokenType.Instructor:
					this.DoInstructor(token.text);
					break;
				case TokenType.EmptyLine:
					this.EndItem();
					break;
				}
			}
			IL_00F4:
			this.EndLayout(Array.Empty<string>());
		}

		// Token: 0x060018A8 RID: 6312 RVA: 0x00059AF4 File Offset: 0x00057CF4
		private void EndItem()
		{
			if (this.status.activeItem)
			{
				this.status.activeItem = null;
				this.EndLayout(Array.Empty<string>());
			}
		}

		// Token: 0x060018A9 RID: 6313 RVA: 0x00059B20 File Offset: 0x00057D20
		private void BeginItem()
		{
			this.status.activeItem = this.BeginVerticalLayout(Array.Empty<string>());
			this.status.activeItem.SetLayoutSpacing(this.internalItemSpacing);
			this.status.activeItem.SetPreferredWidth(this.itemWidth);
		}

		// Token: 0x060018AA RID: 6314 RVA: 0x00059B6F File Offset: 0x00057D6F
		private void DoEmpty(params string[] elements)
		{
			global::UnityEngine.Object.Instantiate<EmptyEntry>(this.emptyPrefab, this.CurrentTransform).Setup(elements);
		}

		// Token: 0x060018AB RID: 6315 RVA: 0x00059B88 File Offset: 0x00057D88
		private void DoInstructor(string text)
		{
			string[] array = text.Split(' ', StringSplitOptions.None);
			if (array.Length < 1)
			{
				return;
			}
			string text2 = array[0];
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text2);
			if (num <= 3008443898U)
			{
				if (num <= 1811125385U)
				{
					if (num != 1031692888U)
					{
						if (num != 1811125385U)
						{
							return;
						}
						if (!(text2 == "Horizontal"))
						{
							return;
						}
						this.BeginHorizontalLayout(array);
						return;
					}
					else
					{
						if (!(text2 == "color"))
						{
							return;
						}
						this.DoColor(array);
						return;
					}
				}
				else if (num != 2163944795U)
				{
					if (num != 3008443898U)
					{
						return;
					}
					if (!(text2 == "image"))
					{
						return;
					}
					this.DoImage(array);
					return;
				}
				else
				{
					if (!(text2 == "Vertical"))
					{
						return;
					}
					this.BeginVerticalLayout(array);
					return;
				}
			}
			else if (num <= 3482547786U)
			{
				if (num != 3250860581U)
				{
					if (num != 3482547786U)
					{
						return;
					}
					if (!(text2 == "End"))
					{
						return;
					}
					this.EndLayout(Array.Empty<string>());
					return;
				}
				else
				{
					if (!(text2 == "Space"))
					{
						return;
					}
					this.DoEmpty(array);
					return;
				}
			}
			else if (num != 3876335077U)
			{
				if (num != 3909890315U)
				{
					if (num != 4127999362U)
					{
						return;
					}
					if (!(text2 == "s"))
					{
						return;
					}
					this.status.s = true;
					return;
				}
				else
				{
					if (!(text2 == "l"))
					{
						return;
					}
					this.status.l = true;
					return;
				}
			}
			else
			{
				if (!(text2 == "b"))
				{
					return;
				}
				this.status.b = true;
				return;
			}
		}

		// Token: 0x060018AC RID: 6316 RVA: 0x00059CF8 File Offset: 0x00057EF8
		private void DoImage(string[] elements)
		{
			if (this.status.activeItem == null)
			{
				this.BeginItem();
			}
			global::UnityEngine.Object.Instantiate<ImageEntry>(this.imagePrefab, this.CurrentTransform).Setup(elements);
		}

		// Token: 0x060018AD RID: 6317 RVA: 0x00059D2C File Offset: 0x00057F2C
		private void DoColor(string[] elements)
		{
			if (elements.Length < 2)
			{
				return;
			}
			Color color;
			ColorUtility.TryParseHtmlString(elements[1], out color);
			this.status.color = color;
		}

		// Token: 0x060018AE RID: 6318 RVA: 0x00059D58 File Offset: 0x00057F58
		private void DoText(string text)
		{
			if (this.status.activeItem == null)
			{
				this.BeginItem();
			}
			TextEntry textEntry = global::UnityEngine.Object.Instantiate<TextEntry>(this.textPrefab, this.CurrentTransform);
			int num = 30;
			if (this.status.s)
			{
				num = 20;
			}
			if (this.status.l)
			{
				num = 40;
			}
			bool b = this.status.b;
			textEntry.Setup(text, this.status.color, num, b);
			this.status.Flush();
		}

		// Token: 0x060018AF RID: 6319 RVA: 0x00059DDC File Offset: 0x00057FDC
		private Transform GetCurrentTransform()
		{
			if (this.status == null)
			{
				return this.rootContentTransform;
			}
			if (this.status.transforms.Count == 0)
			{
				return this.rootContentTransform;
			}
			return this.status.transforms.Peek();
		}

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x060018B0 RID: 6320 RVA: 0x00059E16 File Offset: 0x00058016
		private Transform CurrentTransform
		{
			get
			{
				return this.GetCurrentTransform();
			}
		}

		// Token: 0x060018B1 RID: 6321 RVA: 0x00059E1E File Offset: 0x0005801E
		public void PushTransform(Transform trans)
		{
			if (this.status == null)
			{
				Debug.LogError("Status not found. Credits Display functions should be called after initialization.", this);
				return;
			}
			this.status.transforms.Push(trans);
		}

		// Token: 0x060018B2 RID: 6322 RVA: 0x00059E48 File Offset: 0x00058048
		public Transform PopTransform()
		{
			if (this.status == null)
			{
				Debug.LogError("Status not found. Credits Display functions should be called after initialization.", this);
				return null;
			}
			if (this.status.transforms.Count == 0)
			{
				Debug.LogError("Nothing to pop. Makesure to match push and pop.", this);
				return null;
			}
			return this.status.transforms.Pop();
		}

		// Token: 0x060018B3 RID: 6323 RVA: 0x00059E99 File Offset: 0x00058099
		private void Awake()
		{
			if (this.setupOnAwake)
			{
				this.ParseAndDisplay();
			}
		}

		// Token: 0x060018B4 RID: 6324 RVA: 0x00059EAC File Offset: 0x000580AC
		private void Reset()
		{
			while (base.transform.childCount > 0)
			{
				Transform child = base.transform.GetChild(0);
				child.SetParent(null);
				if (Application.isPlaying)
				{
					global::UnityEngine.Object.Destroy(child.gameObject);
				}
				else
				{
					global::UnityEngine.Object.DestroyImmediate(child.gameObject);
				}
			}
			this.status = new CreditsDisplay.GenerationStatus();
		}

		// Token: 0x060018B5 RID: 6325 RVA: 0x00059F08 File Offset: 0x00058108
		private VerticalEntry BeginVerticalLayout(params string[] args)
		{
			VerticalEntry verticalEntry = global::UnityEngine.Object.Instantiate<VerticalEntry>(this.verticalPrefab, this.CurrentTransform);
			verticalEntry.Setup(args);
			verticalEntry.SetLayoutSpacing(this.mainSpacing);
			this.PushTransform(verticalEntry.transform);
			return verticalEntry;
		}

		// Token: 0x060018B6 RID: 6326 RVA: 0x00059F47 File Offset: 0x00058147
		private void EndLayout(params string[] args)
		{
			if (this.status.activeItem != null)
			{
				this.EndItem();
			}
			this.PopTransform();
		}

		// Token: 0x060018B7 RID: 6327 RVA: 0x00059F6C File Offset: 0x0005816C
		private HorizontalEntry BeginHorizontalLayout(params string[] args)
		{
			HorizontalEntry horizontalEntry = global::UnityEngine.Object.Instantiate<HorizontalEntry>(this.horizontalPrefab, this.CurrentTransform);
			horizontalEntry.Setup(args);
			this.PushTransform(horizontalEntry.transform);
			return horizontalEntry;
		}

		// Token: 0x040011F0 RID: 4592
		[SerializeField]
		private bool setupOnAwake;

		// Token: 0x040011F1 RID: 4593
		[SerializeField]
		private TextAsset content;

		// Token: 0x040011F2 RID: 4594
		[SerializeField]
		private Transform rootContentTransform;

		// Token: 0x040011F3 RID: 4595
		[SerializeField]
		private float internalItemSpacing = 8f;

		// Token: 0x040011F4 RID: 4596
		[SerializeField]
		private float mainSpacing = 16f;

		// Token: 0x040011F5 RID: 4597
		[SerializeField]
		private float itemWidth = 350f;

		// Token: 0x040011F6 RID: 4598
		[Header("Prefabs")]
		[SerializeField]
		private HorizontalEntry horizontalPrefab;

		// Token: 0x040011F7 RID: 4599
		[SerializeField]
		private VerticalEntry verticalPrefab;

		// Token: 0x040011F8 RID: 4600
		[SerializeField]
		private EmptyEntry emptyPrefab;

		// Token: 0x040011F9 RID: 4601
		[SerializeField]
		private TextEntry textPrefab;

		// Token: 0x040011FA RID: 4602
		[SerializeField]
		private ImageEntry imagePrefab;

		// Token: 0x040011FB RID: 4603
		private CreditsDisplay.GenerationStatus status;

		// Token: 0x02000587 RID: 1415
		private class GenerationStatus
		{
			// Token: 0x0600284E RID: 10318 RVA: 0x00094BAE File Offset: 0x00092DAE
			public void Flush()
			{
				this.s = false;
				this.l = false;
				this.b = false;
				this.color = Color.white;
			}

			// Token: 0x04001FB6 RID: 8118
			public List<Token> records = new List<Token>();

			// Token: 0x04001FB7 RID: 8119
			public Stack<Transform> transforms = new Stack<Transform>();

			// Token: 0x04001FB8 RID: 8120
			public bool s;

			// Token: 0x04001FB9 RID: 8121
			public bool l;

			// Token: 0x04001FBA RID: 8122
			public bool b;

			// Token: 0x04001FBB RID: 8123
			public Color color = Color.white;

			// Token: 0x04001FBC RID: 8124
			public VerticalEntry activeItem;
		}
	}
}
