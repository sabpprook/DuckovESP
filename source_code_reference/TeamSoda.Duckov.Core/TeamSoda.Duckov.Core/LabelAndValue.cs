using System;
using ItemStatsSystem;
using TMPro;
using UnityEngine;

// Token: 0x02000156 RID: 342
public class LabelAndValue : MonoBehaviour
{
	// Token: 0x06000A7E RID: 2686 RVA: 0x0002DDE8 File Offset: 0x0002BFE8
	public void Setup(string label, string value, Polarity valuePolarity)
	{
		this.labelText.text = label;
		this.valueText.text = value;
		Color color = this.colorNeutral;
		switch (valuePolarity)
		{
		case Polarity.Negative:
			color = this.colorNegative;
			break;
		case Polarity.Neutral:
			color = this.colorNeutral;
			break;
		case Polarity.Positive:
			color = this.colorPositive;
			break;
		}
		this.valueText.color = color;
	}

	// Token: 0x04000923 RID: 2339
	[SerializeField]
	private TextMeshProUGUI labelText;

	// Token: 0x04000924 RID: 2340
	[SerializeField]
	private TextMeshProUGUI valueText;

	// Token: 0x04000925 RID: 2341
	[SerializeField]
	private Color colorNeutral;

	// Token: 0x04000926 RID: 2342
	[SerializeField]
	private Color colorPositive;

	// Token: 0x04000927 RID: 2343
	[SerializeField]
	private Color colorNegative;
}
