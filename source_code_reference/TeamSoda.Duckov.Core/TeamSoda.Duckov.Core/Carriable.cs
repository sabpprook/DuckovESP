using System;
using Cysharp.Threading.Tasks;
using Duckov.Scenes;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200009D RID: 157
public class Carriable : MonoBehaviour
{
	// Token: 0x1700011D RID: 285
	// (get) Token: 0x06000540 RID: 1344 RVA: 0x000177C1 File Offset: 0x000159C1
	private Inventory inventory
	{
		get
		{
			if (this.lootbox == null)
			{
				return null;
			}
			return this.lootbox.Inventory;
		}
	}

	// Token: 0x06000541 RID: 1345 RVA: 0x000177DE File Offset: 0x000159DE
	public float GetWeight()
	{
		if (this.inventory)
		{
			return this.inventory.CachedWeight + this.selfWeight;
		}
		return this.selfWeight;
	}

	// Token: 0x06000542 RID: 1346 RVA: 0x00017808 File Offset: 0x00015A08
	public void Take(CA_Carry _carrier)
	{
		if (!_carrier)
		{
			return;
		}
		if (this.carrier)
		{
			this.carrier.StopAction();
		}
		this.droping = false;
		this.carrier = _carrier;
		if (this.inventory)
		{
			this.inventory.RecalculateWeight();
		}
		this.rb.transform.SetParent(this.carrier.characterController.modelRoot);
		this.rb.velocity = Vector3.zero;
		this.rb.transform.position = this.carrier.characterController.modelRoot.TransformPoint(this.carrier.carryPoint);
		this.rb.transform.localRotation = Quaternion.identity;
		this.SetRigidbodyActive(false);
	}

	// Token: 0x06000543 RID: 1347 RVA: 0x000178DC File Offset: 0x00015ADC
	private void SetRigidbodyActive(bool active)
	{
		if (active)
		{
			this.rb.isKinematic = false;
			this.rb.interpolation = RigidbodyInterpolation.Interpolate;
			if (this.lootbox && this.lootbox.interactCollider)
			{
				this.lootbox.interactCollider.isTrigger = false;
				return;
			}
		}
		else
		{
			this.rb.isKinematic = true;
			this.rb.interpolation = RigidbodyInterpolation.None;
			if (this.lootbox && this.lootbox.interactCollider)
			{
				this.lootbox.interactCollider.isTrigger = true;
			}
		}
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x00017980 File Offset: 0x00015B80
	public void Drop()
	{
		if (this.carrier.Running)
		{
			this.carrier.StopAction();
		}
		this.carrier = null;
		MultiSceneCore.MoveToActiveWithScene(this.rb.gameObject, SceneManager.GetActiveScene().buildIndex);
		this.DropTask().Forget();
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x000179D8 File Offset: 0x00015BD8
	public void OnCarriableUpdate(float deltaTime)
	{
		if (!this.carrier)
		{
			return;
		}
		Vector3 vector = this.carrier.characterController.modelRoot.TransformPoint(this.carrier.carryPoint);
		if (this.carrier.characterController.RightHandSocket)
		{
			vector.y = this.carrier.characterController.RightHandSocket.transform.position.y + this.carrier.carryPoint.y;
		}
		this.rb.transform.position = vector;
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x00017A74 File Offset: 0x00015C74
	private async UniTaskVoid DropTask()
	{
		this.startDropTime = Time.time;
		this.droping = true;
		this.SetRigidbodyActive(true);
		this.rb.velocity = base.transform.forward * 1.5f + base.transform.up * 0.5f;
		while (Time.time - this.startDropTime < 3f)
		{
			await UniTask.WaitForEndOfFrame(this);
		}
		this.droping = false;
		this.SetRigidbodyActive(false);
	}

	// Token: 0x040004B4 RID: 1204
	private CA_Carry carrier;

	// Token: 0x040004B5 RID: 1205
	[SerializeField]
	private Rigidbody rb;

	// Token: 0x040004B6 RID: 1206
	[SerializeField]
	private float selfWeight;

	// Token: 0x040004B7 RID: 1207
	public InteractableLootbox lootbox;

	// Token: 0x040004B8 RID: 1208
	private bool droping;

	// Token: 0x040004B9 RID: 1209
	private float startDropTime = -1f;

	// Token: 0x040004BA RID: 1210
	private bool carring;
}
