using System;
using System.Linq;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.MiniGames.SnakeForces
{
	// Token: 0x02000285 RID: 645
	public class SnakeDisplay : MiniGameBehaviour
	{
		// Token: 0x170003CD RID: 973
		// (get) Token: 0x060014A7 RID: 5287 RVA: 0x0004C874 File Offset: 0x0004AA74
		private PrefabPool<SnakePartDisplay> PartPool
		{
			get
			{
				if (this._partPool == null)
				{
					this._partPool = new PrefabPool<SnakePartDisplay>(this.partDisplayTemplate, null, null, null, null, true, 10, 10000, null);
				}
				return this._partPool;
			}
		}

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x060014A8 RID: 5288 RVA: 0x0004C8B0 File Offset: 0x0004AAB0
		private PrefabPool<Transform> FoodPool
		{
			get
			{
				if (this._foodPool == null)
				{
					this._foodPool = new PrefabPool<Transform>(this.foodDisplayTemplate, null, null, null, null, true, 10, 10000, null);
				}
				return this._foodPool;
			}
		}

		// Token: 0x060014A9 RID: 5289 RVA: 0x0004C8EC File Offset: 0x0004AAEC
		private void Awake()
		{
			this.master.OnAddPart += this.OnAddPart;
			this.master.OnGameStart += this.OnGameStart;
			this.master.OnRemovePart += this.OnRemovePart;
			this.master.OnAfterTick += this.OnAfterTick;
			this.master.OnFoodEaten += this.OnFoodEaten;
			this.partDisplayTemplate.gameObject.SetActive(false);
		}

		// Token: 0x060014AA RID: 5290 RVA: 0x0004C97D File Offset: 0x0004AB7D
		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			this.HandlePunchColor();
		}

		// Token: 0x060014AB RID: 5291 RVA: 0x0004C98C File Offset: 0x0004AB8C
		private void HandlePunchColor()
		{
			if (!this.punchingColor)
			{
				return;
			}
			if (this.punchColorIndex >= this.master.Snake.Count)
			{
				this.punchingColor = false;
				return;
			}
			SnakePartDisplay snakePartDisplay = this.PartPool.ActiveEntries.First((SnakePartDisplay e) => e.Target == this.master.Snake[this.punchColorIndex]);
			if (snakePartDisplay)
			{
				snakePartDisplay.PunchColor(Color.HSVToRGB((float)this.punchColorIndex % 12f / 12f, 1f, 1f));
			}
			this.punchColorIndex++;
		}

		// Token: 0x060014AC RID: 5292 RVA: 0x0004CA1D File Offset: 0x0004AC1D
		private void OnGameStart(SnakeForce force)
		{
			this.RefreshFood();
		}

		// Token: 0x060014AD RID: 5293 RVA: 0x0004CA28 File Offset: 0x0004AC28
		private void OnFoodEaten(SnakeForce force, Vector2Int coord)
		{
			FXPool.Play(this.eatFXPrefab, this.GetWorldPosition(coord), Quaternion.LookRotation((Vector3Int)this.master.Head.direction, Vector3.forward));
			foreach (SnakePartDisplay snakePartDisplay in this.PartPool.ActiveEntries)
			{
				snakePartDisplay.Punch();
			}
			this.StartPunchingColor();
		}

		// Token: 0x060014AE RID: 5294 RVA: 0x0004CAB4 File Offset: 0x0004ACB4
		private void StartPunchingColor()
		{
			this.punchingColor = true;
			this.punchColorIndex = 0;
		}

		// Token: 0x060014AF RID: 5295 RVA: 0x0004CAC4 File Offset: 0x0004ACC4
		private void OnAfterTick(SnakeForce force)
		{
			this.RefreshFood();
		}

		// Token: 0x060014B0 RID: 5296 RVA: 0x0004CACC File Offset: 0x0004ACCC
		private void RefreshFood()
		{
			this.FoodPool.ReleaseAll();
			foreach (Vector2Int vector2Int in this.master.Foods)
			{
				this.FoodPool.Get(null).localPosition = this.GetPosition(vector2Int);
			}
		}

		// Token: 0x060014B1 RID: 5297 RVA: 0x0004CB40 File Offset: 0x0004AD40
		private void OnRemovePart(SnakeForce.Part part)
		{
			this.PartPool.ReleaseAll((SnakePartDisplay e) => e.Target == part);
		}

		// Token: 0x060014B2 RID: 5298 RVA: 0x0004CB72 File Offset: 0x0004AD72
		private void OnAddPart(SnakeForce.Part part)
		{
			this.PartPool.Get(null).Setup(this, part);
		}

		// Token: 0x060014B3 RID: 5299 RVA: 0x0004CB87 File Offset: 0x0004AD87
		internal Vector3 GetPosition(Vector2Int coord)
		{
			return coord * this.gridSize;
		}

		// Token: 0x060014B4 RID: 5300 RVA: 0x0004CBA0 File Offset: 0x0004ADA0
		internal Vector3 GetWorldPosition(Vector2Int coord)
		{
			Vector3 position = this.GetPosition(coord);
			return base.transform.TransformPoint(position);
		}

		// Token: 0x04000F2E RID: 3886
		[SerializeField]
		private SnakeForce master;

		// Token: 0x04000F2F RID: 3887
		[SerializeField]
		private SnakePartDisplay partDisplayTemplate;

		// Token: 0x04000F30 RID: 3888
		[SerializeField]
		private Transform foodDisplayTemplate;

		// Token: 0x04000F31 RID: 3889
		[SerializeField]
		private Transform exitDisplayTemplte;

		// Token: 0x04000F32 RID: 3890
		[SerializeField]
		private ParticleSystem eatFXPrefab;

		// Token: 0x04000F33 RID: 3891
		[SerializeField]
		private int gridSize = 8;

		// Token: 0x04000F34 RID: 3892
		private PrefabPool<SnakePartDisplay> _partPool;

		// Token: 0x04000F35 RID: 3893
		private PrefabPool<Transform> _foodPool;

		// Token: 0x04000F36 RID: 3894
		private bool punchingColor;

		// Token: 0x04000F37 RID: 3895
		private int punchColorIndex;
	}
}
