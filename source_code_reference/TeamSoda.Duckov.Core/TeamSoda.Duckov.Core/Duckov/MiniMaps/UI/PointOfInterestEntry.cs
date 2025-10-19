using System;
using LeTai.TrueShadow;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace Duckov.MiniMaps.UI
{
	// Token: 0x02000278 RID: 632
	public class PointOfInterestEntry : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x0600140E RID: 5134 RVA: 0x0004A2D2 File Offset: 0x000484D2
		public MonoBehaviour Target
		{
			get
			{
				return this.target;
			}
		}

		// Token: 0x0600140F RID: 5135 RVA: 0x0004A2DC File Offset: 0x000484DC
		internal void Setup(MiniMapDisplay master, MonoBehaviour target, MiniMapDisplayEntry minimapEntry)
		{
			this.rectTransform = base.transform as RectTransform;
			this.master = master;
			this.target = target;
			this.minimapEntry = minimapEntry;
			this.pointOfInterest = null;
			this.icon.sprite = this.defaultIcon;
			this.icon.color = this.defaultColor;
			this.areaDisplay.color = this.defaultColor;
			Color color = this.defaultColor;
			color.a *= 0.1f;
			this.areaFill.color = color;
			this.caption = target.name;
			this.icon.gameObject.SetActive(true);
			IPointOfInterest pointOfInterest = target as IPointOfInterest;
			if (pointOfInterest == null)
			{
				return;
			}
			this.pointOfInterest = pointOfInterest;
			this.icon.gameObject.SetActive(!this.pointOfInterest.HideIcon);
			this.icon.sprite = ((pointOfInterest.Icon != null) ? pointOfInterest.Icon : this.defaultIcon);
			this.icon.color = pointOfInterest.Color;
			if (this.shadow)
			{
				this.shadow.Color = pointOfInterest.ShadowColor;
				this.shadow.OffsetDistance = pointOfInterest.ShadowDistance;
			}
			string text = this.pointOfInterest.DisplayName;
			this.caption = pointOfInterest.DisplayName;
			if (string.IsNullOrEmpty(text))
			{
				this.displayName.gameObject.SetActive(false);
			}
			else
			{
				this.displayName.gameObject.SetActive(true);
				this.displayName.text = this.pointOfInterest.DisplayName;
			}
			if (pointOfInterest.IsArea)
			{
				this.areaDisplay.gameObject.SetActive(true);
				this.rectTransform.sizeDelta = this.pointOfInterest.AreaRadius * Vector2.one * 2f;
				this.areaDisplay.color = pointOfInterest.Color;
				color = pointOfInterest.Color;
				color.a *= 0.1f;
				this.areaFill.color = color;
				this.areaDisplay.BorderWidth = this.areaLineThickness / this.ParentLocalScale;
			}
			else
			{
				this.icon.enabled = true;
				this.areaDisplay.gameObject.SetActive(false);
			}
			this.RefreshPosition();
			base.gameObject.SetActive(true);
		}

		// Token: 0x06001410 RID: 5136 RVA: 0x0004A544 File Offset: 0x00048744
		private void RefreshPosition()
		{
			this.cachedWorldPosition = this.target.transform.position;
			Vector3 centerOfObjectScene = MiniMapCenter.GetCenterOfObjectScene(this.target);
			Vector3 vector = this.target.transform.position - centerOfObjectScene;
			Vector3 vector2 = new Vector2(vector.x, vector.z);
			Vector3 vector3 = this.minimapEntry.transform.localToWorldMatrix.MultiplyPoint(vector2);
			base.transform.position = vector3;
			this.UpdateScale();
			this.UpdateRotation();
		}

		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x06001411 RID: 5137 RVA: 0x0004A5D4 File Offset: 0x000487D4
		private float ParentLocalScale
		{
			get
			{
				return base.transform.parent.localScale.x;
			}
		}

		// Token: 0x06001412 RID: 5138 RVA: 0x0004A5EB File Offset: 0x000487EB
		private void Update()
		{
			this.UpdateScale();
			this.UpdatePosition();
			this.UpdateRotation();
		}

		// Token: 0x06001413 RID: 5139 RVA: 0x0004A600 File Offset: 0x00048800
		private void UpdateScale()
		{
			float num = ((this.pointOfInterest != null) ? this.pointOfInterest.ScaleFactor : 1f);
			this.iconContainer.localScale = Vector3.one * num / this.ParentLocalScale;
			if (this.pointOfInterest != null && this.pointOfInterest.IsArea)
			{
				this.areaDisplay.BorderWidth = this.areaLineThickness / this.ParentLocalScale;
				this.areaDisplay.FalloffDistance = 1f / this.ParentLocalScale;
			}
		}

		// Token: 0x06001414 RID: 5140 RVA: 0x0004A68D File Offset: 0x0004888D
		private void UpdatePosition()
		{
			if (this.cachedWorldPosition != this.target.transform.position)
			{
				this.RefreshPosition();
			}
		}

		// Token: 0x06001415 RID: 5141 RVA: 0x0004A6B2 File Offset: 0x000488B2
		private void UpdateRotation()
		{
			base.transform.rotation = Quaternion.identity;
		}

		// Token: 0x06001416 RID: 5142 RVA: 0x0004A6C4 File Offset: 0x000488C4
		public void OnPointerClick(PointerEventData eventData)
		{
			this.pointOfInterest.NotifyClicked(eventData);
			if (CheatMode.Active && UIInputManager.Ctrl && UIInputManager.Alt && UIInputManager.Shift)
			{
				if (MiniMapCenter.GetSceneID(this.target) == null)
				{
					return;
				}
				CharacterMainControl.Main.SetPosition(this.target.transform.position);
			}
		}

		// Token: 0x04000EB5 RID: 3765
		private RectTransform rectTransform;

		// Token: 0x04000EB6 RID: 3766
		private MiniMapDisplay master;

		// Token: 0x04000EB7 RID: 3767
		private MonoBehaviour target;

		// Token: 0x04000EB8 RID: 3768
		private IPointOfInterest pointOfInterest;

		// Token: 0x04000EB9 RID: 3769
		private MiniMapDisplayEntry minimapEntry;

		// Token: 0x04000EBA RID: 3770
		[SerializeField]
		private Transform iconContainer;

		// Token: 0x04000EBB RID: 3771
		[SerializeField]
		private Sprite defaultIcon;

		// Token: 0x04000EBC RID: 3772
		[SerializeField]
		private Color defaultColor = Color.white;

		// Token: 0x04000EBD RID: 3773
		[SerializeField]
		private Image icon;

		// Token: 0x04000EBE RID: 3774
		[SerializeField]
		private TrueShadow shadow;

		// Token: 0x04000EBF RID: 3775
		[SerializeField]
		private TextMeshProUGUI displayName;

		// Token: 0x04000EC0 RID: 3776
		[SerializeField]
		private ProceduralImage areaDisplay;

		// Token: 0x04000EC1 RID: 3777
		[SerializeField]
		private Image areaFill;

		// Token: 0x04000EC2 RID: 3778
		[SerializeField]
		private float areaLineThickness = 1f;

		// Token: 0x04000EC3 RID: 3779
		[SerializeField]
		private string caption;

		// Token: 0x04000EC4 RID: 3780
		private Vector3 cachedWorldPosition = Vector3.zero;
	}
}
