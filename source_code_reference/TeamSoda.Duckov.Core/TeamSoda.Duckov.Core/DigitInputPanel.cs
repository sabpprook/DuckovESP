using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200019B RID: 411
public class DigitInputPanel : MonoBehaviour
{
	// Token: 0x14000061 RID: 97
	// (add) Token: 0x06000C25 RID: 3109 RVA: 0x000334D0 File Offset: 0x000316D0
	// (remove) Token: 0x06000C26 RID: 3110 RVA: 0x00033508 File Offset: 0x00031708
	public event Action<string> onInputFieldValueChanged;

	// Token: 0x17000234 RID: 564
	// (get) Token: 0x06000C27 RID: 3111 RVA: 0x00033540 File Offset: 0x00031740
	public long Value
	{
		get
		{
			string text = this.inputField.text;
			if (string.IsNullOrEmpty(text))
			{
				return 0L;
			}
			long num;
			if (!long.TryParse(text, out num))
			{
				return 0L;
			}
			return num;
		}
	}

	// Token: 0x06000C28 RID: 3112 RVA: 0x00033574 File Offset: 0x00031774
	private void Awake()
	{
		this.inputField.onValueChanged.AddListener(new UnityAction<string>(this.OnInputFieldValueChanged));
		for (int i = 0; i < this.numKeys.Length; i++)
		{
			int v = i;
			this.numKeys[i].onClick.AddListener(delegate
			{
				this.OnNumKeyClicked((long)v);
			});
		}
		this.clearButton.onClick.AddListener(new UnityAction(this.OnClearButtonClicked));
		this.backspaceButton.onClick.AddListener(new UnityAction(this.OnBackspaceButtonClicked));
		this.maximumButton.onClick.AddListener(new UnityAction(this.Max));
	}

	// Token: 0x06000C29 RID: 3113 RVA: 0x00033638 File Offset: 0x00031838
	private void OnBackspaceButtonClicked()
	{
		if (string.IsNullOrEmpty(this.inputField.text))
		{
			return;
		}
		this.inputField.text = this.inputField.text.Substring(0, this.inputField.text.Length - 1);
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x00033686 File Offset: 0x00031886
	private void OnClearButtonClicked()
	{
		this.inputField.text = string.Empty;
	}

	// Token: 0x06000C2B RID: 3115 RVA: 0x00033698 File Offset: 0x00031898
	private void OnNumKeyClicked(long v)
	{
		this.inputField.text = string.Format("{0}{1}", this.inputField.text, v);
	}

	// Token: 0x06000C2C RID: 3116 RVA: 0x000336C0 File Offset: 0x000318C0
	private void OnInputFieldValueChanged(string value)
	{
		long num;
		if (long.TryParse(value, out num) && num == 0L)
		{
			this.inputField.SetTextWithoutNotify(string.Empty);
		}
		Action<string> action = this.onInputFieldValueChanged;
		if (action == null)
		{
			return;
		}
		action(value);
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x000336FB File Offset: 0x000318FB
	public void Setup(long value, Func<long> maxFunc = null)
	{
		this.maxFunction = maxFunc;
		this.inputField.text = string.Format("{0}", value);
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x00033720 File Offset: 0x00031920
	public void Max()
	{
		if (this.maxFunction == null)
		{
			return;
		}
		long num = this.maxFunction();
		this.inputField.text = string.Format("{0}", num);
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x0003375D File Offset: 0x0003195D
	internal void Clear()
	{
		this.inputField.text = string.Empty;
	}

	// Token: 0x04000A8B RID: 2699
	[SerializeField]
	private TMP_InputField inputField;

	// Token: 0x04000A8C RID: 2700
	[SerializeField]
	private Button clearButton;

	// Token: 0x04000A8D RID: 2701
	[SerializeField]
	private Button backspaceButton;

	// Token: 0x04000A8E RID: 2702
	[SerializeField]
	private Button maximumButton;

	// Token: 0x04000A8F RID: 2703
	[SerializeField]
	private Button[] numKeys;

	// Token: 0x04000A90 RID: 2704
	public Func<long> maxFunction;
}
