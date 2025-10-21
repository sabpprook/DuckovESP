using System;
using Duckov.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.CreditsUtility
{
	// Token: 0x020002FC RID: 764
	public class ImageEntry : MonoBehaviour
	{
		// Token: 0x060018C7 RID: 6343 RVA: 0x0005A428 File Offset: 0x00058628
		internal void Setup(string[] elements)
		{
			if (elements.Length < 2)
			{
				return;
			}
			for (int i = 0; i < elements.Length; i++)
			{
				float num2;
				if (i == 1)
				{
					string text = elements[1];
					Sprite sprite = GameplayDataSettings.GetSprite(text);
					if (sprite == null)
					{
						Debug.LogError("Cannot find sprite:" + text);
					}
					else
					{
						this.image.sprite = sprite;
					}
				}
				else if (i == 2)
				{
					float num;
					if (float.TryParse(elements[2], out num))
					{
						this.layoutElement.preferredHeight = num;
					}
				}
				else if (i == 3 && float.TryParse(elements[2], out num2))
				{
					this.layoutElement.preferredWidth = num2;
				}
			}
		}

		// Token: 0x0400120B RID: 4619
		[SerializeField]
		private Image image;

		// Token: 0x0400120C RID: 4620
		[SerializeField]
		private LayoutElement layoutElement;
	}
}
