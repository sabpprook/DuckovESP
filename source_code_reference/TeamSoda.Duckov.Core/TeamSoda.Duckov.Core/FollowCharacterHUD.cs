using System;
using UnityEngine;

// Token: 0x020000C6 RID: 198
public class FollowCharacterHUD : MonoBehaviour
{
	// Token: 0x0600063F RID: 1599 RVA: 0x0001C1ED File Offset: 0x0001A3ED
	private void Awake()
	{
		GameCamera.OnCameraPosUpdate = (Action<GameCamera, CharacterMainControl>)Delegate.Combine(GameCamera.OnCameraPosUpdate, new Action<GameCamera, CharacterMainControl>(this.UpdatePos));
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x0001C20F File Offset: 0x0001A40F
	private void OnDestroy()
	{
		GameCamera.OnCameraPosUpdate = (Action<GameCamera, CharacterMainControl>)Delegate.Remove(GameCamera.OnCameraPosUpdate, new Action<GameCamera, CharacterMainControl>(this.UpdatePos));
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x0001C234 File Offset: 0x0001A434
	private void UpdatePos(GameCamera gameCamera, CharacterMainControl target)
	{
		Camera renderCamera = gameCamera.renderCamera;
		Vector3 vector = target.transform.position + this.offset;
		this.worldPos = Vector3.SmoothDamp(this.worldPos, vector, ref this.velocityTemp, this.smoothTime);
		if (Vector3.Distance(this.worldPos, vector) > this.maxDistance)
		{
			this.worldPos = (this.worldPos - vector).normalized * this.maxDistance + vector;
		}
		Vector3 vector2 = renderCamera.WorldToScreenPoint(this.worldPos);
		base.transform.position = vector2;
		if (target.gameObject.activeInHierarchy != base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(target.gameObject.activeInHierarchy);
		}
	}

	// Token: 0x040005FA RID: 1530
	public float maxDistance = 2f;

	// Token: 0x040005FB RID: 1531
	public float smoothTime;

	// Token: 0x040005FC RID: 1532
	private Vector3 worldPos;

	// Token: 0x040005FD RID: 1533
	private Vector3 velocityTemp;

	// Token: 0x040005FE RID: 1534
	public Vector3 offset;
}
