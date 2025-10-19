using System;
using UnityEngine;

// Token: 0x020000C7 RID: 199
public class LittleMoveHUD : MonoBehaviour
{
	// Token: 0x06000643 RID: 1603 RVA: 0x0001C314 File Offset: 0x0001A514
	private void LateUpdate()
	{
		if (!this.character)
		{
			if (LevelManager.Instance)
			{
				this.character = LevelManager.Instance.MainCharacter;
			}
			if (!this.character)
			{
				return;
			}
		}
		if (!this.camera)
		{
			this.camera = Camera.main;
			if (!this.camera)
			{
				return;
			}
		}
		Vector3 vector = this.character.transform.position + this.offset;
		this.worldPos = Vector3.SmoothDamp(this.worldPos, vector, ref this.velocityTemp, this.smoothTime);
		if (Vector3.Distance(this.worldPos, vector) > this.maxDistance)
		{
			this.worldPos = (this.worldPos - vector).normalized * this.maxDistance + vector;
		}
		Vector3 vector2 = this.camera.WorldToScreenPoint(this.worldPos);
		base.transform.position = vector2;
	}

	// Token: 0x040005FF RID: 1535
	private Camera camera;

	// Token: 0x04000600 RID: 1536
	private CharacterMainControl character;

	// Token: 0x04000601 RID: 1537
	public float maxDistance = 2f;

	// Token: 0x04000602 RID: 1538
	public float smoothTime;

	// Token: 0x04000603 RID: 1539
	private Vector3 worldPos;

	// Token: 0x04000604 RID: 1540
	private Vector3 velocityTemp;

	// Token: 0x04000605 RID: 1541
	public Vector3 offset;
}
