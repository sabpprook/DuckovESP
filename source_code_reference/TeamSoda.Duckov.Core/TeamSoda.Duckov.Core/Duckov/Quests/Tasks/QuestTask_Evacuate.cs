using System;
using Duckov.Scenes;
using Eflatun.SceneReference;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.Quests.Tasks
{
	// Token: 0x0200034E RID: 846
	public class QuestTask_Evacuate : Task
	{
		// Token: 0x17000579 RID: 1401
		// (get) Token: 0x06001D4B RID: 7499 RVA: 0x00068B0E File Offset: 0x00066D0E
		private SceneInfoEntry RequireSceneInfo
		{
			get
			{
				return SceneInfoCollection.GetSceneInfo(this.requireSceneID);
			}
		}

		// Token: 0x1700057A RID: 1402
		// (get) Token: 0x06001D4C RID: 7500 RVA: 0x00068B1C File Offset: 0x00066D1C
		private SceneReference RequireScene
		{
			get
			{
				SceneInfoEntry requireSceneInfo = this.RequireSceneInfo;
				if (requireSceneInfo == null)
				{
					return null;
				}
				return requireSceneInfo.SceneReference;
			}
		}

		// Token: 0x1700057B RID: 1403
		// (get) Token: 0x06001D4D RID: 7501 RVA: 0x00068B3B File Offset: 0x00066D3B
		private string descriptionFormatKey
		{
			get
			{
				return "Task_Evacuate";
			}
		}

		// Token: 0x1700057C RID: 1404
		// (get) Token: 0x06001D4E RID: 7502 RVA: 0x00068B42 File Offset: 0x00066D42
		private string DescriptionFormat
		{
			get
			{
				return this.descriptionFormatKey.ToPlainText();
			}
		}

		// Token: 0x1700057D RID: 1405
		// (get) Token: 0x06001D4F RID: 7503 RVA: 0x00068B50 File Offset: 0x00066D50
		private string TargetDisplayName
		{
			get
			{
				if (this.RequireScene != null && this.RequireScene.UnsafeReason == SceneReferenceUnsafeReason.None)
				{
					return this.RequireSceneInfo.DisplayName;
				}
				if (base.Master.RequireScene != null && base.Master.RequireScene.UnsafeReason == SceneReferenceUnsafeReason.None)
				{
					return base.Master.RequireSceneInfo.DisplayName;
				}
				return "Scene_Any".ToPlainText();
			}
		}

		// Token: 0x1700057E RID: 1406
		// (get) Token: 0x06001D50 RID: 7504 RVA: 0x00068BB8 File Offset: 0x00066DB8
		public override string Description
		{
			get
			{
				return this.DescriptionFormat.Format(new { this.TargetDisplayName });
			}
		}

		// Token: 0x06001D51 RID: 7505 RVA: 0x00068BD0 File Offset: 0x00066DD0
		private void OnEnable()
		{
			LevelManager.OnEvacuated += this.OnEvacuated;
		}

		// Token: 0x06001D52 RID: 7506 RVA: 0x00068BE3 File Offset: 0x00066DE3
		private void OnDisable()
		{
			LevelManager.OnEvacuated -= this.OnEvacuated;
		}

		// Token: 0x06001D53 RID: 7507 RVA: 0x00068BF8 File Offset: 0x00066DF8
		private void OnEvacuated(EvacuationInfo info)
		{
			if (this.finished)
			{
				return;
			}
			if (this.RequireScene == null || this.RequireScene.UnsafeReason == SceneReferenceUnsafeReason.Empty)
			{
				if (base.Master.SceneRequirementSatisfied)
				{
					this.finished = true;
					base.ReportStatusChanged();
					return;
				}
			}
			else if (this.RequireScene.UnsafeReason == SceneReferenceUnsafeReason.None && this.RequireScene.LoadedScene.isLoaded)
			{
				this.finished = true;
				base.ReportStatusChanged();
			}
		}

		// Token: 0x06001D54 RID: 7508 RVA: 0x00068C6E File Offset: 0x00066E6E
		public override object GenerateSaveData()
		{
			return this.finished;
		}

		// Token: 0x06001D55 RID: 7509 RVA: 0x00068C7B File Offset: 0x00066E7B
		protected override bool CheckFinished()
		{
			return this.finished;
		}

		// Token: 0x06001D56 RID: 7510 RVA: 0x00068C84 File Offset: 0x00066E84
		public override void SetupSaveData(object data)
		{
			if (data is bool)
			{
				bool flag = (bool)data;
				this.finished = flag;
			}
		}

		// Token: 0x0400145A RID: 5210
		[SerializeField]
		[SceneID]
		private string requireSceneID;

		// Token: 0x0400145B RID: 5211
		[SerializeField]
		private bool finished;
	}
}
