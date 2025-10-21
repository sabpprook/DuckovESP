using System;
using Duckov.UI.Animations;
using TMPro;
using UnityEngine;

// Token: 0x020000D5 RID: 213
public class CostTakerHUD_Entry : MonoBehaviour
{
	// Token: 0x17000135 RID: 309
	// (get) Token: 0x06000699 RID: 1689 RVA: 0x0001DA44 File Offset: 0x0001BC44
	// (set) Token: 0x0600069A RID: 1690 RVA: 0x0001DA4C File Offset: 0x0001BC4C
	public CostTaker Target { get; private set; }

	// Token: 0x0600069B RID: 1691 RVA: 0x0001DA55 File Offset: 0x0001BC55
	private void Awake()
	{
		this.rectTransform = base.transform as RectTransform;
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x0001DA68 File Offset: 0x0001BC68
	private void LateUpdate()
	{
		this.UpdatePosition();
		this.UpdateFadeGroup();
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x0001DA76 File Offset: 0x0001BC76
	internal void Setup(CostTaker cur)
	{
		this.Target = cur;
		this.nameText.text = cur.InteractName;
		this.costDisplay.Setup(cur.Cost, 1);
		this.UpdatePosition();
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x0001DAA8 File Offset: 0x0001BCA8
	private void UpdatePosition()
	{
		this.rectTransform.MatchWorldPosition(this.Target.transform.TransformPoint(this.Target.interactMarkerOffset), Vector3.up * 0.5f);
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x0001DAE0 File Offset: 0x0001BCE0
	private void UpdateFadeGroup()
	{
		CharacterMainControl main = CharacterMainControl.Main;
		bool flag = false;
		if (!(this.Target == null) && !(main == null))
		{
			Vector3 vector = main.transform.position - this.Target.transform.position;
			if (Mathf.Abs(vector.y) <= 2.5f && vector.magnitude <= 10f)
			{
				flag = true;
			}
		}
		if (flag && !this.fadeGroup.IsShown)
		{
			this.fadeGroup.Show();
			return;
		}
		if (!flag && this.fadeGroup.IsShown)
		{
			this.fadeGroup.Hide();
		}
	}

	// Token: 0x0400065C RID: 1628
	private RectTransform rectTransform;

	// Token: 0x0400065D RID: 1629
	[SerializeField]
	private TextMeshProUGUI nameText;

	// Token: 0x0400065E RID: 1630
	[SerializeField]
	private CostDisplay costDisplay;

	// Token: 0x0400065F RID: 1631
	[SerializeField]
	private FadeGroup fadeGroup;

	// Token: 0x04000660 RID: 1632
	private const float HideDistance = 10f;

	// Token: 0x04000661 RID: 1633
	private const float HideDistanceYLimit = 2.5f;
}
