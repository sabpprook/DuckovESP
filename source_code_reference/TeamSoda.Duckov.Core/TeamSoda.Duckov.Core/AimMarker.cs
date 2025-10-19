using System;
using System.Collections.Generic;
using LeTai.TrueShadow;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000075 RID: 117
public class AimMarker : MonoBehaviour
{
	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x0600044A RID: 1098 RVA: 0x00013A54 File Offset: 0x00011C54
	private Camera MainCam
	{
		get
		{
			if (!this._cam)
			{
				if (LevelManager.Instance == null)
				{
					return null;
				}
				if (LevelManager.Instance.GameCamera == null)
				{
					return null;
				}
				this._cam = LevelManager.Instance.GameCamera.renderCamera;
			}
			return this._cam;
		}
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x00013AAC File Offset: 0x00011CAC
	private void Awake()
	{
		if (!this.currentAdsAimMarker)
		{
			this.SwitchAdsAimMarker(this.defaultAdsAimMarker);
		}
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x00013AC7 File Offset: 0x00011CC7
	private void Start()
	{
		this.rootCanvasGroup.alpha = 1f;
		ItemAgent_Gun.OnMainCharacterShootEvent += this.OnMainCharacterShoot;
		Health.OnDead += this.OnKill;
	}

	// Token: 0x0600044D RID: 1101 RVA: 0x00013AFB File Offset: 0x00011CFB
	private void OnDestroy()
	{
		ItemAgent_Gun.OnMainCharacterShootEvent -= this.OnMainCharacterShoot;
		Health.OnDead -= this.OnKill;
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x00013B20 File Offset: 0x00011D20
	private void Update()
	{
		this.aimMarkerAnimator.SetBool(this.inProgressHash, this.reloadProgressBar.InProgress);
		if (this.killMarkerTimer > 0f)
		{
			this.killMarkerTimer -= Time.deltaTime;
			this.aimMarkerAnimator.SetBool(this.killMarkerHash, this.killMarkerTimer > 0f);
		}
		CharacterMainControl main = CharacterMainControl.Main;
		if (main == null)
		{
			return;
		}
		if (main.Health.IsDead)
		{
			this.rootCanvasGroup.alpha = 0f;
			return;
		}
		InputManager inputManager = LevelManager.Instance.InputManager;
		if (inputManager == null)
		{
			return;
		}
		Vector3 inputAimPoint = inputManager.InputAimPoint;
		Vector3 vector = this.MainCam.WorldToScreenPoint(inputAimPoint);
		vector = inputManager.AimScreenPoint;
		this.SetAimMarkerPosScreenSpace(vector);
	}

	// Token: 0x0600044F RID: 1103 RVA: 0x00013BF0 File Offset: 0x00011DF0
	private void LateUpdate()
	{
		CharacterMainControl main = CharacterMainControl.Main;
		if (main == null)
		{
			return;
		}
		InputManager inputManager = LevelManager.Instance.InputManager;
		if (inputManager == null)
		{
			return;
		}
		Vector3 inputAimPoint = inputManager.InputAimPoint;
		ItemAgent_Gun gun = main.GetGun();
		Color color = this.distanceTextColorFull;
		float num;
		if (gun != null)
		{
			if (this.adsValue == 0f && gun.AdsValue > 0f)
			{
				this.OnStartAdsWithGun(gun);
			}
			this.adsValue = gun.AdsValue;
			this.scatter = Mathf.MoveTowards(this.scatter, gun.CurrentScatter, 500f * Time.deltaTime);
			this.minScatter = Mathf.MoveTowards(this.minScatter, gun.MinScatter, 500f * Time.deltaTime);
			this.left.anchoredPosition = Vector3.left * (20f + this.scatter * 5f);
			this.right.anchoredPosition = Vector3.right * (20f + this.scatter * 5f);
			this.up.anchoredPosition = Vector3.up * (20f + this.scatter * 5f);
			this.down.anchoredPosition = Vector3.down * (20f + this.scatter * 5f);
			num = Vector3.Distance(inputAimPoint, gun.muzzle.position);
			float bulletDistance = gun.BulletDistance;
			if (num < bulletDistance * 0.495f)
			{
				color = this.distanceTextColorFull;
			}
			else if (num < bulletDistance)
			{
				color = this.distanceTextColorHalf;
			}
			else
			{
				color = this.distanceTextColorOver;
			}
		}
		else
		{
			this.adsValue = 0f;
			this.scatter = 0f;
			this.minScatter = 0f;
			num = Vector3.Distance(inputAimPoint, main.transform.position + Vector3.up * 0.5f);
			color = this.distanceTextColorFull;
		}
		float num2 = Mathf.Clamp01((0.5f - this.adsValue) * 2f);
		if (this.currentAdsAimMarker)
		{
			this.currentAdsAimMarker.SetScatter(this.scatter, this.minScatter);
			this.currentAdsAimMarker.SetAdsValue(this.adsValue);
			if (!this.currentAdsAimMarker.hideNormalCrosshair)
			{
				num2 = 1f;
			}
		}
		else
		{
			num2 = 1f;
		}
		this.normalAimCanvasGroup.alpha = num2;
		if (this.distanceText)
		{
			this.distanceText.text = num.ToString("00") + " M";
			this.distanceText.color = color;
			this.distanceGlow.Color = color;
		}
	}

	// Token: 0x06000450 RID: 1104 RVA: 0x00013ECB File Offset: 0x000120CB
	public void SetAimMarkerPosScreenSpace(Vector3 pos)
	{
		this.aimMarkerUI.position = pos;
		if (this.currentAdsAimMarker)
		{
			this.currentAdsAimMarker.SetAimMarkerPos(pos);
		}
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x00013EF4 File Offset: 0x000120F4
	private void OnStartAdsWithGun(ItemAgent_Gun gun)
	{
		ADSAimMarker aimMarkerPfb = gun.GetAimMarkerPfb();
		if (!aimMarkerPfb)
		{
			return;
		}
		this.SwitchAdsAimMarker(aimMarkerPfb);
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x00013F18 File Offset: 0x00012118
	private void SwitchAdsAimMarker(ADSAimMarker newAimMarkerPfb)
	{
		if (newAimMarkerPfb == null)
		{
			global::UnityEngine.Object.Destroy(this.currentAdsAimMarker.gameObject);
			this.currentAdsAimMarker = null;
			return;
		}
		if (this.currentAdsAimMarker && newAimMarkerPfb == this.currentAdsAimMarker.selfPrefab)
		{
			return;
		}
		if (this.currentAdsAimMarker)
		{
			global::UnityEngine.Object.Destroy(this.currentAdsAimMarker.gameObject);
		}
		this.currentAdsAimMarker = global::UnityEngine.Object.Instantiate<ADSAimMarker>(newAimMarkerPfb);
		this.currentAdsAimMarker.selfPrefab = newAimMarkerPfb;
		this.currentAdsAimMarker.transform.SetParent(base.transform);
		this.currentAdsAimMarker.parentAimMarker = this;
		RectTransform rectTransform = this.currentAdsAimMarker.transform as RectTransform;
		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.one;
		rectTransform.sizeDelta = Vector2.zero;
		rectTransform.offsetMax = Vector2.zero;
		rectTransform.offsetMin = Vector2.zero;
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x00014004 File Offset: 0x00012204
	private void SetAimMarkerColor(Color col)
	{
		int count = this.aimMarkerImages.Count;
		for (int i = 0; i < count; i++)
		{
			this.aimMarkerImages[i].color = col;
		}
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x0001403B File Offset: 0x0001223B
	private void OnKill(Health _health, DamageInfo dmgInfo)
	{
		if (_health == null || _health.team == Teams.player)
		{
			return;
		}
		this.killMarkerTimer = this.killMarkerTime;
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x0001405B File Offset: 0x0001225B
	private void OnMainCharacterShoot(ItemAgent_Gun gunAgnet)
	{
		UnityEvent unityEvent = this.onShoot;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		if (this.currentAdsAimMarker)
		{
			this.currentAdsAimMarker.OnShoot();
		}
	}

	// Token: 0x040003A8 RID: 936
	public RectTransform aimMarkerUI;

	// Token: 0x040003A9 RID: 937
	public List<Image> aimMarkerImages;

	// Token: 0x040003AA RID: 938
	public RectTransform left;

	// Token: 0x040003AB RID: 939
	public RectTransform right;

	// Token: 0x040003AC RID: 940
	public RectTransform up;

	// Token: 0x040003AD RID: 941
	public RectTransform down;

	// Token: 0x040003AE RID: 942
	private float scatter;

	// Token: 0x040003AF RID: 943
	private float minScatter;

	// Token: 0x040003B0 RID: 944
	public CanvasGroup rootCanvasGroup;

	// Token: 0x040003B1 RID: 945
	public CanvasGroup normalAimCanvasGroup;

	// Token: 0x040003B2 RID: 946
	public Animator aimMarkerAnimator;

	// Token: 0x040003B3 RID: 947
	public ActionProgressHUD reloadProgressBar;

	// Token: 0x040003B4 RID: 948
	public UnityEvent onShoot;

	// Token: 0x040003B5 RID: 949
	private ADSAimMarker currentAdsAimMarker;

	// Token: 0x040003B6 RID: 950
	[SerializeField]
	private ADSAimMarker defaultAdsAimMarker;

	// Token: 0x040003B7 RID: 951
	private readonly int inProgressHash = Animator.StringToHash("InProgress");

	// Token: 0x040003B8 RID: 952
	private readonly int killMarkerHash = Animator.StringToHash("KillMarkerShow");

	// Token: 0x040003B9 RID: 953
	[SerializeField]
	private TextMeshProUGUI distanceText;

	// Token: 0x040003BA RID: 954
	[SerializeField]
	private TrueShadow distanceGlow;

	// Token: 0x040003BB RID: 955
	[SerializeField]
	private Color distanceTextColorFull;

	// Token: 0x040003BC RID: 956
	[SerializeField]
	private Color distanceTextColorHalf;

	// Token: 0x040003BD RID: 957
	[SerializeField]
	private Color distanceTextColorOver;

	// Token: 0x040003BE RID: 958
	private float adsValue;

	// Token: 0x040003BF RID: 959
	private float killMarkerTime = 0.6f;

	// Token: 0x040003C0 RID: 960
	private float killMarkerTimer;

	// Token: 0x040003C1 RID: 961
	private Camera _cam;
}
