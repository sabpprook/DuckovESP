using System;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.Buildings.UI
{
	// Token: 0x02000319 RID: 793
	public class BuildingContextMenuEntry : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x06001A55 RID: 6741 RVA: 0x0005F4C9 File Offset: 0x0005D6C9
		private void OnEnable()
		{
			this.text.text = this.textKey.ToPlainText();
		}

		// Token: 0x140000AC RID: 172
		// (add) Token: 0x06001A56 RID: 6742 RVA: 0x0005F4E4 File Offset: 0x0005D6E4
		// (remove) Token: 0x06001A57 RID: 6743 RVA: 0x0005F51C File Offset: 0x0005D71C
		public event Action<BuildingContextMenuEntry> onPointerClick;

		// Token: 0x06001A58 RID: 6744 RVA: 0x0005F551 File Offset: 0x0005D751
		public void OnPointerClick(PointerEventData eventData)
		{
			Action<BuildingContextMenuEntry> action = this.onPointerClick;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x040012EB RID: 4843
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x040012EC RID: 4844
		[SerializeField]
		[LocalizationKey("Default")]
		private string textKey;
	}
}
