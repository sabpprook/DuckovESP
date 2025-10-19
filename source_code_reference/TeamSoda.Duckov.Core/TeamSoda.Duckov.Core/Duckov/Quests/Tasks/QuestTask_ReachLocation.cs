using System;
using Duckov.Scenes;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Duckov.Quests.Tasks
{
	// Token: 0x02000350 RID: 848
	public class QuestTask_ReachLocation : Task
	{
		// Token: 0x17000595 RID: 1429
		// (get) Token: 0x06001D7F RID: 7551 RVA: 0x000690F6 File Offset: 0x000672F6
		public string descriptionFormatkey
		{
			get
			{
				return "Task_ReachLocation";
			}
		}

		// Token: 0x17000596 RID: 1430
		// (get) Token: 0x06001D80 RID: 7552 RVA: 0x000690FD File Offset: 0x000672FD
		public string DescriptionFormat
		{
			get
			{
				return this.descriptionFormatkey.ToPlainText();
			}
		}

		// Token: 0x17000597 RID: 1431
		// (get) Token: 0x06001D81 RID: 7553 RVA: 0x0006910A File Offset: 0x0006730A
		public string TargetLocationDisplayName
		{
			get
			{
				return this.location.GetDisplayName();
			}
		}

		// Token: 0x17000598 RID: 1432
		// (get) Token: 0x06001D82 RID: 7554 RVA: 0x00069117 File Offset: 0x00067317
		public override string Description
		{
			get
			{
				return this.DescriptionFormat.Format(new { this.TargetLocationDisplayName });
			}
		}

		// Token: 0x06001D83 RID: 7555 RVA: 0x0006912F File Offset: 0x0006732F
		private void OnEnable()
		{
			SceneLoader.onFinishedLoadingScene += this.OnFinishedLoadingScene;
			MultiSceneCore.OnSubSceneLoaded += this.OnSubSceneLoaded;
		}

		// Token: 0x06001D84 RID: 7556 RVA: 0x00069153 File Offset: 0x00067353
		private void Start()
		{
			this.CacheLocation();
		}

		// Token: 0x06001D85 RID: 7557 RVA: 0x0006915B File Offset: 0x0006735B
		private void OnDisable()
		{
			SceneLoader.onFinishedLoadingScene -= this.OnFinishedLoadingScene;
			MultiSceneCore.OnSubSceneLoaded -= this.OnSubSceneLoaded;
		}

		// Token: 0x06001D86 RID: 7558 RVA: 0x0006917F File Offset: 0x0006737F
		protected override void OnInit()
		{
			base.OnInit();
			if (!base.IsFinished())
			{
				this.SetMapElementVisable(true);
			}
		}

		// Token: 0x06001D87 RID: 7559 RVA: 0x00069196 File Offset: 0x00067396
		private void OnFinishedLoadingScene(SceneLoadingContext context)
		{
			this.CacheLocation();
		}

		// Token: 0x06001D88 RID: 7560 RVA: 0x0006919E File Offset: 0x0006739E
		private void OnSubSceneLoaded(MultiSceneCore core, Scene scene)
		{
			LevelManager.LevelInitializingComment = "Reach location task caching";
			this.CacheLocation();
		}

		// Token: 0x06001D89 RID: 7561 RVA: 0x000691B0 File Offset: 0x000673B0
		private void CacheLocation()
		{
			this.target = this.location.GetLocationTransform();
		}

		// Token: 0x06001D8A RID: 7562 RVA: 0x000691C4 File Offset: 0x000673C4
		private void Update()
		{
			if (this.finished)
			{
				return;
			}
			if (this.target == null)
			{
				return;
			}
			CharacterMainControl main = CharacterMainControl.Main;
			if (main == null)
			{
				return;
			}
			if ((main.transform.position - this.target.position).magnitude <= this.radius)
			{
				this.finished = true;
				this.SetMapElementVisable(false);
			}
			base.ReportStatusChanged();
		}

		// Token: 0x06001D8B RID: 7563 RVA: 0x00069238 File Offset: 0x00067438
		public override object GenerateSaveData()
		{
			return this.finished;
		}

		// Token: 0x06001D8C RID: 7564 RVA: 0x00069245 File Offset: 0x00067445
		protected override bool CheckFinished()
		{
			return this.finished;
		}

		// Token: 0x06001D8D RID: 7565 RVA: 0x00069250 File Offset: 0x00067450
		public override void SetupSaveData(object data)
		{
			if (data is bool)
			{
				bool flag = (bool)data;
				this.finished = flag;
			}
		}

		// Token: 0x06001D8E RID: 7566 RVA: 0x00069274 File Offset: 0x00067474
		private void SetMapElementVisable(bool visable)
		{
			if (!this.mapElement)
			{
				return;
			}
			if (visable)
			{
				this.mapElement.locations.Clear();
				this.mapElement.locations.Add(this.location);
				this.mapElement.range = this.radius;
				this.mapElement.name = base.Master.DisplayName;
			}
			this.mapElement.SetVisibility(visable);
		}

		// Token: 0x04001467 RID: 5223
		[SerializeField]
		private MultiSceneLocation location;

		// Token: 0x04001468 RID: 5224
		[SerializeField]
		private float radius = 1f;

		// Token: 0x04001469 RID: 5225
		[SerializeField]
		private bool finished;

		// Token: 0x0400146A RID: 5226
		[SerializeField]
		private Transform target;

		// Token: 0x0400146B RID: 5227
		[SerializeField]
		private MapElementForTask mapElement;
	}
}
