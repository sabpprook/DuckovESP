using System;
using UnityEngine;

namespace Duckov.MiniGames
{
	// Token: 0x02000280 RID: 640
	public class MiniGameBehaviour : MonoBehaviour
	{
		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x0600146D RID: 5229 RVA: 0x0004BB60 File Offset: 0x00049D60
		public MiniGame Game
		{
			get
			{
				return this.game;
			}
		}

		// Token: 0x0600146E RID: 5230 RVA: 0x0004BB68 File Offset: 0x00049D68
		public void SetGame(MiniGame game = null)
		{
			if (game == null)
			{
				this.game = base.GetComponentInParent<MiniGame>();
				return;
			}
			this.game = game;
		}

		// Token: 0x0600146F RID: 5231 RVA: 0x0004BB87 File Offset: 0x00049D87
		private void OnUpdateLogic(MiniGame game, float deltaTime)
		{
			if (this == null)
			{
				return;
			}
			if (!base.enabled)
			{
				return;
			}
			if (game == null)
			{
				return;
			}
			if (game != this.game)
			{
				return;
			}
			this.OnUpdate(deltaTime);
		}

		// Token: 0x06001470 RID: 5232 RVA: 0x0004BBBC File Offset: 0x00049DBC
		protected virtual void OnEnable()
		{
			MiniGame.onUpdateLogic = (Action<MiniGame, float>)Delegate.Combine(MiniGame.onUpdateLogic, new Action<MiniGame, float>(this.OnUpdateLogic));
		}

		// Token: 0x06001471 RID: 5233 RVA: 0x0004BBDE File Offset: 0x00049DDE
		protected virtual void OnDisable()
		{
			MiniGame.onUpdateLogic = (Action<MiniGame, float>)Delegate.Remove(MiniGame.onUpdateLogic, new Action<MiniGame, float>(this.OnUpdateLogic));
		}

		// Token: 0x06001472 RID: 5234 RVA: 0x0004BC00 File Offset: 0x00049E00
		private void OnDestroy()
		{
			MiniGame.onUpdateLogic = (Action<MiniGame, float>)Delegate.Remove(MiniGame.onUpdateLogic, new Action<MiniGame, float>(this.OnUpdateLogic));
		}

		// Token: 0x06001473 RID: 5235 RVA: 0x0004BC22 File Offset: 0x00049E22
		protected virtual void Start()
		{
			if (this.game == null)
			{
				this.SetGame(null);
			}
		}

		// Token: 0x06001474 RID: 5236 RVA: 0x0004BC39 File Offset: 0x00049E39
		protected virtual void OnUpdate(float deltaTime)
		{
		}

		// Token: 0x04000EFE RID: 3838
		[SerializeField]
		private MiniGame game;
	}
}
