using System;
using Duckov.Aquariums;
using UnityEngine;

// Token: 0x02000196 RID: 406
public class DummyFish : MonoBehaviour, IAquariumContent
{
	// Token: 0x17000230 RID: 560
	// (get) Token: 0x06000BEA RID: 3050 RVA: 0x0003299A File Offset: 0x00030B9A
	private Vector3 TargetPosition
	{
		get
		{
			return this.target.position;
		}
	}

	// Token: 0x06000BEB RID: 3051 RVA: 0x000329A7 File Offset: 0x00030BA7
	private void Awake()
	{
		this.rigidbody.useGravity = false;
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x000329B5 File Offset: 0x00030BB5
	public void Setup(Aquarium master)
	{
		this.master = master;
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x000329C0 File Offset: 0x00030BC0
	private void FixedUpdate()
	{
		Vector3 up = Vector3.up;
		Vector3 forward = base.transform.forward;
		Vector3 right = base.transform.right;
		Vector3 vector = this.TargetPosition - this.rigidbody.position;
		Vector3 normalized = vector.normalized;
		Vector3 vector2 = Vector3.Cross(up, normalized);
		float num = Vector3.Dot(normalized, forward);
		float num2 = Mathf.Max(0f, num);
		this.swim = ((vector.magnitude > this.deadZone) ? 1f : (vector.magnitude / this.deadZone)) * num2;
		Vector3 vector3 = -(Vector3.Dot(vector2, this.rigidbody.velocity) * vector2);
		this.rigidbody.velocity += forward * this.swimForce * this.swim * Time.deltaTime + vector3 * 0.5f;
		this.rigidbody.angularVelocity = Vector3.zero;
		Vector3 vector4 = vector;
		vector4.y = 0f;
		float num3 = Mathf.Clamp01(vector4.magnitude / this.deadZone - 0.5f);
		Vector3 normalized2 = Vector3.ProjectOnPlane(forward, Vector3.up).normalized;
		this._debug_projectedForward = normalized2;
		Vector3 vector5 = Vector3.Lerp(normalized2, normalized, num3);
		this._debug_idealRotForward = vector5;
		float num4 = Vector3.SignedAngle(forward, vector5, right);
		float num5 = Vector3.SignedAngle(forward, vector5, Vector3.up);
		float num6 = this.rotateForce * num4;
		float num7 = this.rotateForce * num5;
		this.rotVelocityX += num6 * Time.fixedDeltaTime;
		this.rotVelocityY += num7 * Time.fixedDeltaTime * num3;
		this.rotVelocityX *= 1f - this.rotationDamping;
		this.rotVelocityY *= 1f - this.rotationDamping;
		Vector3 eulerAngles = this.rigidbody.rotation.eulerAngles;
		eulerAngles.y += this.rotVelocityY * Time.deltaTime;
		eulerAngles.x += this.rotVelocityX * Time.deltaTime;
		if (eulerAngles.x < -179f)
		{
			eulerAngles.x += 360f;
		}
		if (eulerAngles.x > 179f)
		{
			eulerAngles.x -= 360f;
		}
		eulerAngles.x = Mathf.Clamp(eulerAngles.x, -45f, 45f);
		eulerAngles.z = 0f;
		Quaternion quaternion = Quaternion.Euler(eulerAngles);
		this.rigidbody.MoveRotation(quaternion);
	}

	// Token: 0x06000BEE RID: 3054 RVA: 0x00032C78 File Offset: 0x00030E78
	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(base.transform.position, base.transform.position + this._debug_idealRotForward);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(base.transform.position, base.transform.position + this._debug_projectedForward);
	}

	// Token: 0x04000A64 RID: 2660
	[SerializeField]
	private Rigidbody rigidbody;

	// Token: 0x04000A65 RID: 2661
	[SerializeField]
	private float rotateForce = 10f;

	// Token: 0x04000A66 RID: 2662
	[SerializeField]
	private float swimForce = 10f;

	// Token: 0x04000A67 RID: 2663
	[SerializeField]
	private float deadZone = 2f;

	// Token: 0x04000A68 RID: 2664
	[SerializeField]
	private float rotationDamping = 0.1f;

	// Token: 0x04000A69 RID: 2665
	[Header("Control")]
	[SerializeField]
	private Transform target;

	// Token: 0x04000A6A RID: 2666
	[Range(0f, 1f)]
	[SerializeField]
	private float swim;

	// Token: 0x04000A6B RID: 2667
	private float rotVelocityX;

	// Token: 0x04000A6C RID: 2668
	private float rotVelocityY;

	// Token: 0x04000A6D RID: 2669
	private Aquarium master;

	// Token: 0x04000A6E RID: 2670
	private Vector3 _debug_idealRotForward;

	// Token: 0x04000A6F RID: 2671
	private Vector3 _debug_projectedForward;
}
