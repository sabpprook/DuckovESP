using System;
using Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x020000A6 RID: 166
[Obsolete]
public class InvisibleTeleporter : MonoBehaviour, IDrawGizmos
{
	// Token: 0x1700011F RID: 287
	// (get) Token: 0x060005A6 RID: 1446 RVA: 0x00019459 File Offset: 0x00017659
	private bool UsePosition
	{
		get
		{
			return this.target == null;
		}
	}

	// Token: 0x17000120 RID: 288
	// (get) Token: 0x060005A7 RID: 1447 RVA: 0x00019468 File Offset: 0x00017668
	private Vector3 TargetWorldPosition
	{
		get
		{
			if (this.target != null)
			{
				return this.target.transform.position;
			}
			Space space = this.space;
			if (space == Space.World)
			{
				return this.position;
			}
			if (space != Space.Self)
			{
				return default(Vector3);
			}
			return base.transform.TransformPoint(this.position);
		}
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x000194C8 File Offset: 0x000176C8
	public void Teleport()
	{
		CharacterMainControl main = CharacterMainControl.Main;
		if (main == null)
		{
			return;
		}
		GameCamera instance = GameCamera.Instance;
		Vector3 vector = instance.transform.position - main.transform.position;
		main.SetPosition(this.TargetWorldPosition);
		Vector3 vector2 = main.transform.position + vector;
		instance.transform.position = vector2;
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x0001952F File Offset: 0x0001772F
	private void LateUpdate()
	{
		if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
		{
			this.Teleport();
		}
	}

	// Token: 0x060005AA RID: 1450 RVA: 0x00019550 File Offset: 0x00017750
	public void DrawGizmos()
	{
		if (!GizmoContext.InActiveSelection(this))
		{
			return;
		}
		CharacterMainControl main = CharacterMainControl.Main;
		if (main == null)
		{
			Draw.Arrow(base.transform.position, this.TargetWorldPosition);
			return;
		}
		Draw.Arrow(main.transform.position, this.TargetWorldPosition);
	}

	// Token: 0x04000524 RID: 1316
	[SerializeField]
	private Transform target;

	// Token: 0x04000525 RID: 1317
	[SerializeField]
	private Vector3 position;

	// Token: 0x04000526 RID: 1318
	[SerializeField]
	private Space space;
}
