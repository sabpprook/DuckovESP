using System;
using Cinemachine;
using NodeCanvas.Framework;

// Token: 0x020001B1 RID: 433
public class AT_SetVirtualCamera : ActionTask
{
	// Token: 0x17000257 RID: 599
	// (get) Token: 0x06000CD7 RID: 3287 RVA: 0x000357DA File Offset: 0x000339DA
	protected override string info
	{
		get
		{
			return "Set camera :" + ((this.target.value == null) ? "Empty" : this.target.value.name);
		}
	}

	// Token: 0x06000CD8 RID: 3288 RVA: 0x00035810 File Offset: 0x00033A10
	protected override void OnExecute()
	{
		base.OnExecute();
		if (AT_SetVirtualCamera.cachedVCam != null)
		{
			AT_SetVirtualCamera.cachedVCam.gameObject.SetActive(false);
		}
		if (this.target.value != null)
		{
			this.target.value.gameObject.SetActive(true);
			AT_SetVirtualCamera.cachedVCam = this.target.value;
		}
		else
		{
			AT_SetVirtualCamera.cachedVCam = null;
		}
		base.EndAction();
	}

	// Token: 0x04000B12 RID: 2834
	private static CinemachineVirtualCamera cachedVCam;

	// Token: 0x04000B13 RID: 2835
	public BBParameter<CinemachineVirtualCamera> target;
}
