using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.MiniMaps
{
	// Token: 0x02000272 RID: 626
	public interface IPointOfInterest
	{
		// Token: 0x17000399 RID: 921
		// (get) Token: 0x060013A9 RID: 5033 RVA: 0x0004904B File Offset: 0x0004724B
		int OverrideScene
		{
			get
			{
				return -1;
			}
		}

		// Token: 0x1700039A RID: 922
		// (get) Token: 0x060013AA RID: 5034 RVA: 0x0004904E File Offset: 0x0004724E
		Sprite Icon
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x060013AB RID: 5035 RVA: 0x00049051 File Offset: 0x00047251
		Color Color
		{
			get
			{
				return Color.white;
			}
		}

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x060013AC RID: 5036 RVA: 0x00049058 File Offset: 0x00047258
		string DisplayName
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700039D RID: 925
		// (get) Token: 0x060013AD RID: 5037 RVA: 0x0004905B File Offset: 0x0004725B
		Color ShadowColor
		{
			get
			{
				return Color.white;
			}
		}

		// Token: 0x1700039E RID: 926
		// (get) Token: 0x060013AE RID: 5038 RVA: 0x00049062 File Offset: 0x00047262
		float ShadowDistance
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x060013AF RID: 5039 RVA: 0x00049069 File Offset: 0x00047269
		bool IsArea
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x060013B0 RID: 5040 RVA: 0x0004906C File Offset: 0x0004726C
		float AreaRadius
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x060013B1 RID: 5041 RVA: 0x00049073 File Offset: 0x00047273
		bool HideIcon
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x060013B2 RID: 5042 RVA: 0x00049076 File Offset: 0x00047276
		float ScaleFactor
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x060013B3 RID: 5043 RVA: 0x0004907D File Offset: 0x0004727D
		void NotifyClicked(PointerEventData eventData)
		{
		}
	}
}
