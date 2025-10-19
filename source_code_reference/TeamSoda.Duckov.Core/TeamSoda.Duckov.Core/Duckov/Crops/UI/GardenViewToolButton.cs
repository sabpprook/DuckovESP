using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.Crops.UI
{
	// Token: 0x020002EF RID: 751
	public class GardenViewToolButton : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x06001856 RID: 6230 RVA: 0x00058F6B File Offset: 0x0005716B
		public void OnPointerClick(PointerEventData eventData)
		{
			this.master.SetTool(this.tool);
		}

		// Token: 0x06001857 RID: 6231 RVA: 0x00058F7E File Offset: 0x0005717E
		private void Awake()
		{
			this.master.onToolChanged += this.OnToolChanged;
		}

		// Token: 0x06001858 RID: 6232 RVA: 0x00058F97 File Offset: 0x00057197
		private void Start()
		{
			this.Refresh();
		}

		// Token: 0x06001859 RID: 6233 RVA: 0x00058F9F File Offset: 0x0005719F
		private void Refresh()
		{
			this.indicator.SetActive(this.tool == this.master.Tool);
		}

		// Token: 0x0600185A RID: 6234 RVA: 0x00058FBF File Offset: 0x000571BF
		private void OnToolChanged()
		{
			this.Refresh();
		}

		// Token: 0x040011C6 RID: 4550
		[SerializeField]
		private GardenView master;

		// Token: 0x040011C7 RID: 4551
		[SerializeField]
		private GardenView.ToolType tool;

		// Token: 0x040011C8 RID: 4552
		[SerializeField]
		private GameObject indicator;
	}
}
