using System;
using Duckov.Scenes;
using SodaCraft.Localizations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Duckov.MiniMaps
{
	// Token: 0x02000273 RID: 627
	public class SimplePointOfInterest : MonoBehaviour, IPointOfInterest
	{
		// Token: 0x14000080 RID: 128
		// (add) Token: 0x060013B4 RID: 5044 RVA: 0x00049080 File Offset: 0x00047280
		// (remove) Token: 0x060013B5 RID: 5045 RVA: 0x000490B8 File Offset: 0x000472B8
		public event Action<PointerEventData> OnClicked;

		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x060013B6 RID: 5046 RVA: 0x000490ED File Offset: 0x000472ED
		// (set) Token: 0x060013B7 RID: 5047 RVA: 0x000490F5 File Offset: 0x000472F5
		public float ScaleFactor
		{
			get
			{
				return this.scaleFactor;
			}
			set
			{
				this.scaleFactor = value;
			}
		}

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x060013B8 RID: 5048 RVA: 0x000490FE File Offset: 0x000472FE
		// (set) Token: 0x060013B9 RID: 5049 RVA: 0x00049106 File Offset: 0x00047306
		public Color Color
		{
			get
			{
				return this.color;
			}
			set
			{
				this.color = value;
			}
		}

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x060013BA RID: 5050 RVA: 0x0004910F File Offset: 0x0004730F
		// (set) Token: 0x060013BB RID: 5051 RVA: 0x00049117 File Offset: 0x00047317
		public Color ShadowColor
		{
			get
			{
				return this.shadowColor;
			}
			set
			{
				this.shadowColor = value;
			}
		}

		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x060013BC RID: 5052 RVA: 0x00049120 File Offset: 0x00047320
		// (set) Token: 0x060013BD RID: 5053 RVA: 0x00049128 File Offset: 0x00047328
		public float ShadowDistance
		{
			get
			{
				return this.shadowDistance;
			}
			set
			{
				this.shadowDistance = value;
			}
		}

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x060013BE RID: 5054 RVA: 0x00049131 File Offset: 0x00047331
		public string DisplayName
		{
			get
			{
				return this.displayName.ToPlainText();
			}
		}

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x060013BF RID: 5055 RVA: 0x0004913E File Offset: 0x0004733E
		public Sprite Icon
		{
			get
			{
				return this.icon;
			}
		}

		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x060013C0 RID: 5056 RVA: 0x00049148 File Offset: 0x00047348
		public int OverrideScene
		{
			get
			{
				if (this.followActiveScene && MultiSceneCore.ActiveSubScene != null)
				{
					return MultiSceneCore.ActiveSubScene.Value.buildIndex;
				}
				if (!string.IsNullOrEmpty(this.overrideSceneID))
				{
					return SceneInfoCollection.GetBuildIndex(this.overrideSceneID);
				}
				return -1;
			}
		}

		// Token: 0x170003AA RID: 938
		// (get) Token: 0x060013C1 RID: 5057 RVA: 0x0004919C File Offset: 0x0004739C
		// (set) Token: 0x060013C2 RID: 5058 RVA: 0x000491A4 File Offset: 0x000473A4
		public bool IsArea
		{
			get
			{
				return this.isArea;
			}
			set
			{
				this.isArea = value;
			}
		}

		// Token: 0x170003AB RID: 939
		// (get) Token: 0x060013C3 RID: 5059 RVA: 0x000491AD File Offset: 0x000473AD
		// (set) Token: 0x060013C4 RID: 5060 RVA: 0x000491B5 File Offset: 0x000473B5
		public float AreaRadius
		{
			get
			{
				return this.areaRadius;
			}
			set
			{
				this.areaRadius = value;
			}
		}

		// Token: 0x170003AC RID: 940
		// (get) Token: 0x060013C5 RID: 5061 RVA: 0x000491BE File Offset: 0x000473BE
		// (set) Token: 0x060013C6 RID: 5062 RVA: 0x000491C6 File Offset: 0x000473C6
		public bool HideIcon
		{
			get
			{
				return this.hideIcon;
			}
			set
			{
				this.hideIcon = value;
			}
		}

		// Token: 0x060013C7 RID: 5063 RVA: 0x000491CF File Offset: 0x000473CF
		private void OnEnable()
		{
			PointsOfInterests.Register(this);
		}

		// Token: 0x060013C8 RID: 5064 RVA: 0x000491D7 File Offset: 0x000473D7
		private void OnDisable()
		{
			PointsOfInterests.Unregister(this);
		}

		// Token: 0x060013C9 RID: 5065 RVA: 0x000491DF File Offset: 0x000473DF
		public void Setup(Sprite icon = null, string displayName = null, bool followActiveScene = false, string overrideSceneID = null)
		{
			if (icon != null)
			{
				this.icon = icon;
			}
			this.displayName = displayName;
			this.followActiveScene = followActiveScene;
			this.overrideSceneID = overrideSceneID;
			PointsOfInterests.Unregister(this);
			PointsOfInterests.Register(this);
		}

		// Token: 0x060013CA RID: 5066 RVA: 0x00049213 File Offset: 0x00047413
		public void SetColor(Color color)
		{
			this.color = color;
		}

		// Token: 0x060013CB RID: 5067 RVA: 0x0004921C File Offset: 0x0004741C
		public bool SetupMultiSceneLocation(MultiSceneLocation location, bool moveToMainScene = true)
		{
			Vector3 vector;
			if (!location.TryGetLocationPosition(out vector))
			{
				return false;
			}
			base.transform.position = vector;
			this.overrideSceneID = location.SceneID;
			if (moveToMainScene && MultiSceneCore.MainScene != null)
			{
				SceneManager.MoveGameObjectToScene(base.gameObject, MultiSceneCore.MainScene.Value);
			}
			return true;
		}

		// Token: 0x060013CC RID: 5068 RVA: 0x0004927C File Offset: 0x0004747C
		public static SimplePointOfInterest Create(Vector3 position, string sceneID, string displayName, Sprite icon = null, bool hideIcon = false)
		{
			GameObject gameObject = new GameObject("POI_" + displayName);
			gameObject.transform.position = position;
			SimplePointOfInterest simplePointOfInterest = gameObject.AddComponent<SimplePointOfInterest>();
			simplePointOfInterest.overrideSceneID = sceneID;
			simplePointOfInterest.displayName = displayName;
			simplePointOfInterest.hideIcon = hideIcon;
			simplePointOfInterest.icon = icon;
			SceneManager.MoveGameObjectToScene(gameObject, MultiSceneCore.MainScene.Value);
			return simplePointOfInterest;
		}

		// Token: 0x060013CD RID: 5069 RVA: 0x000492DC File Offset: 0x000474DC
		public void NotifyClicked(PointerEventData pointerEventData)
		{
			Action<PointerEventData> onClicked = this.OnClicked;
			if (onClicked == null)
			{
				return;
			}
			onClicked(pointerEventData);
		}

		// Token: 0x04000E8D RID: 3725
		[SerializeField]
		private Sprite icon;

		// Token: 0x04000E8E RID: 3726
		[SerializeField]
		private Color color = Color.white;

		// Token: 0x04000E8F RID: 3727
		[SerializeField]
		private Color shadowColor = Color.white;

		// Token: 0x04000E90 RID: 3728
		[SerializeField]
		private float shadowDistance;

		// Token: 0x04000E91 RID: 3729
		[LocalizationKey("Default")]
		[SerializeField]
		private string displayName = "";

		// Token: 0x04000E92 RID: 3730
		[SerializeField]
		private bool followActiveScene;

		// Token: 0x04000E93 RID: 3731
		[SceneID]
		[SerializeField]
		private string overrideSceneID;

		// Token: 0x04000E94 RID: 3732
		[SerializeField]
		private bool isArea;

		// Token: 0x04000E95 RID: 3733
		[SerializeField]
		private float areaRadius;

		// Token: 0x04000E96 RID: 3734
		[SerializeField]
		private float scaleFactor = 1f;

		// Token: 0x04000E98 RID: 3736
		[SerializeField]
		private bool hideIcon;
	}
}
