using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000185 RID: 389
public class OcclusionFadeManager : MonoBehaviour
{
	// Token: 0x17000227 RID: 551
	// (get) Token: 0x06000BA0 RID: 2976 RVA: 0x000313FF File Offset: 0x0002F5FF
	public static OcclusionFadeManager Instance
	{
		get
		{
			if (!OcclusionFadeManager.instance)
			{
				OcclusionFadeManager.instance = global::UnityEngine.Object.FindFirstObjectByType<OcclusionFadeManager>();
			}
			return OcclusionFadeManager.instance;
		}
	}

	// Token: 0x17000228 RID: 552
	// (get) Token: 0x06000BA1 RID: 2977 RVA: 0x0003141C File Offset: 0x0002F61C
	public float startFadeHeight
	{
		get
		{
			CharacterMainControl main = CharacterMainControl.Main;
			if (!main || !main.gameObject.activeInHierarchy)
			{
				return 0.25f;
			}
			return main.transform.position.y + 0.25f;
		}
	}

	// Token: 0x06000BA2 RID: 2978 RVA: 0x00031460 File Offset: 0x0002F660
	private void Awake()
	{
		this.materialDic = new Dictionary<Material, Material>();
		this.aimOcclusionFadeChecker.gameObject.layer = LayerMask.NameToLayer("VisualOcclusion");
		this.characterOcclusionFadeChecker.gameObject.layer = LayerMask.NameToLayer("VisualOcclusion");
		this.SetShader();
		Shader.SetGlobalTexture("FadeNoiseTexture", this.fadeNoiseTexture);
	}

	// Token: 0x06000BA3 RID: 2979 RVA: 0x000314C2 File Offset: 0x0002F6C2
	private void OnValidate()
	{
		this.SetShader();
	}

	// Token: 0x06000BA4 RID: 2980 RVA: 0x000314CC File Offset: 0x0002F6CC
	private void SetShader()
	{
		Shader.SetGlobalFloat(this.viewRangeHash, this.viewRange);
		Shader.SetGlobalFloat(this.viewFadeRangeHash, this.viewFadeRange);
		Shader.SetGlobalFloat(this.startFadeHeightHash, this.startFadeHeight);
		Shader.SetGlobalFloat(this.heightFadeRangeHash, this.heightFadeRange);
	}

	// Token: 0x06000BA5 RID: 2981 RVA: 0x00031520 File Offset: 0x0002F720
	private void Update()
	{
		if (this.character)
		{
			this.aimOcclusionFadeChecker.transform.position = LevelManager.Instance.InputManager.InputAimPoint;
			Vector3 normalized = (this.aimOcclusionFadeChecker.transform.position - this.cam.transform.position).normalized;
			this.aimOcclusionFadeChecker.transform.rotation = Quaternion.LookRotation(-normalized);
			Shader.SetGlobalVector(this.aimViewDirHash, normalized);
			Shader.SetGlobalVector(this.aimPosHash, this.aimOcclusionFadeChecker.transform.position);
			this.characterOcclusionFadeChecker.transform.position = this.character.transform.position;
			Vector3 normalized2 = (this.characterOcclusionFadeChecker.transform.position - this.cam.transform.position).normalized;
			this.characterOcclusionFadeChecker.transform.rotation = Quaternion.LookRotation(-normalized2);
			Shader.SetGlobalVector(this.characterViewDirHash, normalized2);
			Shader.SetGlobalFloat(this.startFadeHeightHash, this.startFadeHeight);
			Shader.SetGlobalVector(this.charactetrPosHash, this.character.transform.position);
			return;
		}
		if (!LevelManager.Instance)
		{
			return;
		}
		this.character = LevelManager.Instance.MainCharacter;
		this.cam = LevelManager.Instance.GameCamera.renderCamera;
	}

	// Token: 0x06000BA6 RID: 2982 RVA: 0x000316B4 File Offset: 0x0002F8B4
	public Material GetMaskedMaterial(Material mat)
	{
		if (mat == null)
		{
			return null;
		}
		if (!this.supportedShaders.Contains(mat.shader))
		{
			return mat;
		}
		if (this.materialDic.ContainsKey(mat))
		{
			return this.materialDic[mat];
		}
		Material material = new Material(this.maskedShader);
		material.CopyPropertiesFromMaterial(mat);
		this.materialDic.Add(mat, material);
		return material;
	}

	// Token: 0x040009F0 RID: 2544
	private static OcclusionFadeManager instance;

	// Token: 0x040009F1 RID: 2545
	public OcclusionFadeChecker aimOcclusionFadeChecker;

	// Token: 0x040009F2 RID: 2546
	public OcclusionFadeChecker characterOcclusionFadeChecker;

	// Token: 0x040009F3 RID: 2547
	private CharacterMainControl character;

	// Token: 0x040009F4 RID: 2548
	private Camera cam;

	// Token: 0x040009F5 RID: 2549
	public Dictionary<Material, Material> materialDic;

	// Token: 0x040009F6 RID: 2550
	public List<Shader> supportedShaders;

	// Token: 0x040009F7 RID: 2551
	public Shader maskedShader;

	// Token: 0x040009F8 RID: 2552
	public Material testMat;

	// Token: 0x040009F9 RID: 2553
	[Range(0f, 4f)]
	public float viewRange;

	// Token: 0x040009FA RID: 2554
	[Range(0f, 8f)]
	public float viewFadeRange;

	// Token: 0x040009FB RID: 2555
	public Texture2D fadeNoiseTexture;

	// Token: 0x040009FC RID: 2556
	public float heightFadeRange;

	// Token: 0x040009FD RID: 2557
	private int aimViewDirHash = Shader.PropertyToID("OC_AimViewDir");

	// Token: 0x040009FE RID: 2558
	private int aimPosHash = Shader.PropertyToID("OC_AimPos");

	// Token: 0x040009FF RID: 2559
	private int characterViewDirHash = Shader.PropertyToID("OC_CharacterViewDir");

	// Token: 0x04000A00 RID: 2560
	private int charactetrPosHash = Shader.PropertyToID("OC_CharacterPos");

	// Token: 0x04000A01 RID: 2561
	private int viewRangeHash = Shader.PropertyToID("ViewRange");

	// Token: 0x04000A02 RID: 2562
	private int viewFadeRangeHash = Shader.PropertyToID("ViewFadeRange");

	// Token: 0x04000A03 RID: 2563
	private int startFadeHeightHash = Shader.PropertyToID("StartFadeHeight");

	// Token: 0x04000A04 RID: 2564
	private int heightFadeRangeHash = Shader.PropertyToID("HeightFadeRange");
}
