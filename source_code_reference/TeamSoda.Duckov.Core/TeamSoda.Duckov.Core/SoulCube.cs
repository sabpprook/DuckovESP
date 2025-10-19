using System;
using UnityEngine;

// Token: 0x020000B1 RID: 177
public class SoulCube : MonoBehaviour
{
	// Token: 0x060005D6 RID: 1494 RVA: 0x0001A038 File Offset: 0x00018238
	public void Init(SoulCollector collectorTarget)
	{
		this.target = collectorTarget;
		this.direction = global::UnityEngine.Random.insideUnitSphere + Vector3.up;
		this.direction.Normalize();
		this.spawnSpeed = global::UnityEngine.Random.Range(this.speedRange.x, this.speedRange.y);
		this.roatePart.transform.localRotation = Quaternion.Euler(global::UnityEngine.Random.insideUnitSphere * 360f);
		this.rotateAxis = global::UnityEngine.Random.insideUnitSphere;
		this.rotateSpeed = global::UnityEngine.Random.Range(this.rotateSpeedRange.x, this.rotateSpeedRange.y);
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x0001A0E0 File Offset: 0x000182E0
	private void Update()
	{
		this.roatePart.Rotate(this.rotateSpeed * this.rotateAxis * Time.deltaTime);
		if (this.target == null)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		this.stateTimer += Time.deltaTime;
		SoulCube.States states = this.currentState;
		if (states != SoulCube.States.spawn)
		{
			if (states != SoulCube.States.goToTarget)
			{
				return;
			}
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.target.transform.position, this.toTargetSpeed * Time.deltaTime);
			if (Vector3.Distance(base.transform.position, this.target.transform.position) < 0.3f)
			{
				this.AddCube();
			}
		}
		else
		{
			this.velocity = this.spawnSpeed * this.direction * this.spawnSpeedCurve.Evaluate(Mathf.Clamp01(this.stateTimer / this.spawnTime));
			base.transform.position += this.velocity * Time.deltaTime;
			if (this.stateTimer > this.spawnTime)
			{
				this.currentState = SoulCube.States.goToTarget;
				return;
			}
		}
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x0001A227 File Offset: 0x00018427
	private void AddCube()
	{
		if (this.target)
		{
			this.target.AddCube();
		}
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x04000554 RID: 1364
	private SoulCube.States currentState;

	// Token: 0x04000555 RID: 1365
	private SoulCollector target;

	// Token: 0x04000556 RID: 1366
	private Vector3 direction;

	// Token: 0x04000557 RID: 1367
	private float stateTimer;

	// Token: 0x04000558 RID: 1368
	public Vector2 speedRange;

	// Token: 0x04000559 RID: 1369
	private float spawnSpeed;

	// Token: 0x0400055A RID: 1370
	public float spawnTime;

	// Token: 0x0400055B RID: 1371
	public float toTargetSpeed;

	// Token: 0x0400055C RID: 1372
	public AnimationCurve spawnSpeedCurve;

	// Token: 0x0400055D RID: 1373
	private Vector3 velocity;

	// Token: 0x0400055E RID: 1374
	public Transform roatePart;

	// Token: 0x0400055F RID: 1375
	public Vector2 rotateSpeedRange = new Vector2(300f, 1000f);

	// Token: 0x04000560 RID: 1376
	private float rotateSpeed;

	// Token: 0x04000561 RID: 1377
	private Vector3 rotateAxis;

	// Token: 0x0200045A RID: 1114
	private enum States
	{
		// Token: 0x04001AF9 RID: 6905
		spawn,
		// Token: 0x04001AFA RID: 6906
		goToTarget
	}
}
