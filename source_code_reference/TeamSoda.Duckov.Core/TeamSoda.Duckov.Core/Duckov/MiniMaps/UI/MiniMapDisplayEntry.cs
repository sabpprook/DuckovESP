using System;
using Duckov.Scenes;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Duckov.MiniMaps.UI
{
	// Token: 0x02000276 RID: 630
	public class MiniMapDisplayEntry : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x170003AF RID: 943
		// (get) Token: 0x060013EA RID: 5098 RVA: 0x00049BFC File Offset: 0x00047DFC
		public SceneReference SceneReference
		{
			get
			{
				SceneInfoEntry sceneInfo = SceneInfoCollection.GetSceneInfo(this.SceneID);
				if (sceneInfo == null)
				{
					return null;
				}
				return sceneInfo.SceneReference;
			}
		}

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x060013EB RID: 5099 RVA: 0x00049C20 File Offset: 0x00047E20
		public string SceneID
		{
			get
			{
				return this.sceneID;
			}
		}

		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x060013EC RID: 5100 RVA: 0x00049C28 File Offset: 0x00047E28
		private RectTransform rectTransform
		{
			get
			{
				if (this._rectTransform == null)
				{
					this._rectTransform = base.transform as RectTransform;
				}
				return this._rectTransform;
			}
		}

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x060013ED RID: 5101 RVA: 0x00049C4F File Offset: 0x00047E4F
		// (set) Token: 0x060013EE RID: 5102 RVA: 0x00049C57 File Offset: 0x00047E57
		public MiniMapDisplay Master { get; private set; }

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x060013EF RID: 5103 RVA: 0x00049C60 File Offset: 0x00047E60
		public bool Hide
		{
			get
			{
				return this.target != null && this.target.Hide;
			}
		}

		// Token: 0x060013F0 RID: 5104 RVA: 0x00049C77 File Offset: 0x00047E77
		private void Awake()
		{
			MultiSceneCore.OnSubSceneLoaded += this.OnSubSceneLoaded;
		}

		// Token: 0x060013F1 RID: 5105 RVA: 0x00049C8A File Offset: 0x00047E8A
		private void OnDestroy()
		{
			MultiSceneCore.OnSubSceneLoaded -= this.OnSubSceneLoaded;
		}

		// Token: 0x060013F2 RID: 5106 RVA: 0x00049C9D File Offset: 0x00047E9D
		private void OnSubSceneLoaded(MultiSceneCore core, Scene scene)
		{
			LevelManager.LevelInitializingComment = "Mapping entries";
			Debug.Log("Mapping entries", this);
			this.RefreshGraphics();
		}

		// Token: 0x060013F3 RID: 5107 RVA: 0x00049CBA File Offset: 0x00047EBA
		public bool NoSignal()
		{
			return this.target != null && this.target.NoSignal;
		}

		// Token: 0x060013F4 RID: 5108 RVA: 0x00049CD4 File Offset: 0x00047ED4
		internal void Setup(MiniMapDisplay master, IMiniMapEntry cur, bool showGraphics = true)
		{
			this.Master = master;
			this.target = cur;
			if (cur.Sprite != null)
			{
				this.image.sprite = cur.Sprite;
				this.rectTransform.sizeDelta = Vector2.one * (float)cur.Sprite.texture.width * cur.PixelSize;
				this.showGraphics = showGraphics;
			}
			else
			{
				this.showGraphics = false;
			}
			if (cur.Hide)
			{
				this.showGraphics = false;
			}
			this.rectTransform.anchoredPosition = cur.Offset;
			this.sceneID = cur.SceneID;
			this.isCombined = false;
			this.RefreshGraphics();
		}

		// Token: 0x060013F5 RID: 5109 RVA: 0x00049D88 File Offset: 0x00047F88
		internal void SetupCombined(MiniMapDisplay master, IMiniMapDataProvider dataProvider)
		{
			this.target = null;
			this.Master = master;
			if (dataProvider == null)
			{
				return;
			}
			if (dataProvider.CombinedSprite == null)
			{
				return;
			}
			this.image.sprite = dataProvider.CombinedSprite;
			this.rectTransform.sizeDelta = Vector2.one * (float)dataProvider.CombinedSprite.texture.width * dataProvider.PixelSize;
			this.rectTransform.anchoredPosition = dataProvider.CombinedCenter;
			this.sceneID = "";
			this.image.enabled = true;
			this.showGraphics = true;
			this.isCombined = true;
			this.RefreshGraphics();
		}

		// Token: 0x060013F6 RID: 5110 RVA: 0x00049E3C File Offset: 0x0004803C
		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Right)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.sceneID))
			{
				return;
			}
			Vector3 vector;
			RectTransformUtility.ScreenPointToWorldPointInRectangle(base.transform as RectTransform, eventData.position, null, out vector);
			Vector3 vector2;
			if (!this.Master.TryConvertToWorldPosition(eventData.position, out vector2))
			{
				return;
			}
			MiniMapView.RequestMarkPOI(vector2);
			eventData.Use();
		}

		// Token: 0x060013F7 RID: 5111 RVA: 0x00049EA4 File Offset: 0x000480A4
		private void RefreshGraphics()
		{
			bool flag = this.ShouldShow();
			if (flag)
			{
				this.image.color = Color.white;
			}
			else
			{
				this.image.color = Color.clear;
			}
			this.image.enabled = flag;
		}

		// Token: 0x060013F8 RID: 5112 RVA: 0x00049EE9 File Offset: 0x000480E9
		public bool ShouldShow()
		{
			if (!this.showGraphics)
			{
				return false;
			}
			if (this.isCombined)
			{
				return this.showGraphics;
			}
			return MultiSceneCore.ActiveSubSceneID == this.SceneID;
		}

		// Token: 0x04000EA2 RID: 3746
		[SerializeField]
		private Image image;

		// Token: 0x04000EA3 RID: 3747
		private string sceneID;

		// Token: 0x04000EA4 RID: 3748
		private RectTransform _rectTransform;

		// Token: 0x04000EA6 RID: 3750
		private bool showGraphics;

		// Token: 0x04000EA7 RID: 3751
		private bool isCombined;

		// Token: 0x04000EA8 RID: 3752
		private IMiniMapEntry target;
	}
}
