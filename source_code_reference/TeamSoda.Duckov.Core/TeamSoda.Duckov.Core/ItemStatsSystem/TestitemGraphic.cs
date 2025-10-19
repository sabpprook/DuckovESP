using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ItemStatsSystem
{
	// Token: 0x02000228 RID: 552
	public class TestitemGraphic : MonoBehaviour
	{
		// Token: 0x06001104 RID: 4356 RVA: 0x00041F1F File Offset: 0x0004011F
		private void Start()
		{
		}

		// Token: 0x06001105 RID: 4357 RVA: 0x00041F24 File Offset: 0x00040124
		private void Update()
		{
			if (Keyboard.current.gKey.wasPressedThisFrame)
			{
				if (this.instance)
				{
					global::UnityEngine.Object.Destroy(this.instance.gameObject);
				}
				DuckovItemAgent currentHoldItemAgent = CharacterMainControl.Main.CurrentHoldItemAgent;
				if (!currentHoldItemAgent)
				{
					return;
				}
				this.instance = ItemGraphicInfo.CreateAGraphic(currentHoldItemAgent.Item, base.transform);
			}
		}

		// Token: 0x04000D47 RID: 3399
		private ItemGraphicInfo instance;
	}
}
