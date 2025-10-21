using System;
using TMPro;
using UnityEngine;

namespace Duckov
{
	// Token: 0x02000238 RID: 568
	public class GameVersionDisplay : MonoBehaviour
	{
		// Token: 0x0600119C RID: 4508 RVA: 0x00043DD0 File Offset: 0x00041FD0
		private void Start()
		{
			this.text.text = string.Format("v{0}", GameMetaData.Instance.Version);
		}

		// Token: 0x04000DA2 RID: 3490
		[SerializeField]
		private TextMeshProUGUI text;
	}
}
