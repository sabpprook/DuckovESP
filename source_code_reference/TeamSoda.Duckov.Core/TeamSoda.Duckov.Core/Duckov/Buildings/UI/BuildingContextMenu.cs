using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Duckov.Buildings.UI
{
	// Token: 0x02000318 RID: 792
	public class BuildingContextMenu : MonoBehaviour
	{
		// Token: 0x06001A4C RID: 6732 RVA: 0x0005F3A5 File Offset: 0x0005D5A5
		private void Awake()
		{
			this.rectTransform = base.transform as RectTransform;
			this.recycleButton.onPointerClick += this.OnRecycleButtonClicked;
		}

		// Token: 0x06001A4D RID: 6733 RVA: 0x0005F3CF File Offset: 0x0005D5CF
		private void OnRecycleButtonClicked(BuildingContextMenuEntry entry)
		{
			if (this.Target == null)
			{
				return;
			}
			BuildingManager.ReturnBuilding(this.Target.GUID, null).Forget<bool>();
		}

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x06001A4E RID: 6734 RVA: 0x0005F3F6 File Offset: 0x0005D5F6
		// (set) Token: 0x06001A4F RID: 6735 RVA: 0x0005F3FE File Offset: 0x0005D5FE
		public Building Target { get; private set; }

		// Token: 0x06001A50 RID: 6736 RVA: 0x0005F407 File Offset: 0x0005D607
		public void Setup(Building target)
		{
			this.Target = target;
			if (target == null)
			{
				this.Hide();
				return;
			}
			this.nameText.text = target.DisplayName;
			this.Show();
		}

		// Token: 0x06001A51 RID: 6737 RVA: 0x0005F438 File Offset: 0x0005D638
		private void LateUpdate()
		{
			if (this.Target == null)
			{
				this.Hide();
				return;
			}
			Vector2 vector = RectTransformUtility.WorldToScreenPoint(GameCamera.Instance.renderCamera, this.Target.transform.position);
			Vector2 vector2;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(base.transform.parent as RectTransform, vector, null, out vector2);
			this.rectTransform.localPosition = vector2;
		}

		// Token: 0x06001A52 RID: 6738 RVA: 0x0005F4A5 File Offset: 0x0005D6A5
		private void Show()
		{
			base.gameObject.SetActive(true);
		}

		// Token: 0x06001A53 RID: 6739 RVA: 0x0005F4B3 File Offset: 0x0005D6B3
		public void Hide()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x040012E7 RID: 4839
		private RectTransform rectTransform;

		// Token: 0x040012E8 RID: 4840
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x040012E9 RID: 4841
		[SerializeField]
		private BuildingContextMenuEntry recycleButton;
	}
}
