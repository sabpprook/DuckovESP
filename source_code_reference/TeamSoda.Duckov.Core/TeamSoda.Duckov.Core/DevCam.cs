using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Token: 0x020001B4 RID: 436
public class DevCam : MonoBehaviour
{
	// Token: 0x06000CE3 RID: 3299 RVA: 0x00035A09 File Offset: 0x00033C09
	private void Awake()
	{
		this.root.gameObject.SetActive(false);
		Shader.SetGlobalFloat("DevCamOn", 0f);
		DevCam.devCamOn = false;
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x00035A34 File Offset: 0x00033C34
	private void Toggle()
	{
		this.active = true;
		DevCam.devCamOn = this.active;
		Shader.SetGlobalFloat("DevCamOn", this.active ? 1f : 0f);
		this.root.gameObject.SetActive(this.active);
		for (int i = 0; i < Display.displays.Length; i++)
		{
			if (i == 1 && this.active)
			{
				Display.displays[i].Activate();
			}
		}
		UniversalRenderPipelineAsset universalRenderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
		if (universalRenderPipelineAsset != null)
		{
			universalRenderPipelineAsset.shadowDistance = 500f;
		}
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x00035AD0 File Offset: 0x00033CD0
	private void OnDestroy()
	{
		DevCam.devCamOn = false;
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x00035AD8 File Offset: 0x00033CD8
	private void Update()
	{
		if (!LevelManager.LevelInited)
		{
			return;
		}
		if (Gamepad.all.Count <= 0)
		{
			return;
		}
		this.timer -= Time.deltaTime;
		if (this.timer <= 0f)
		{
			this.timer = 0f;
			this.pressCounter = 0;
		}
		if (Gamepad.current.leftStickButton.isPressed && Gamepad.current.rightStickButton.wasPressedThisFrame)
		{
			this.pressCounter++;
			this.timer = 1.5f;
			Debug.Log("Toggle Dev Cam");
			if (this.pressCounter >= 2)
			{
				this.pressCounter = 0;
				this.Toggle();
			}
		}
		if (CharacterMainControl.Main != null)
		{
			this.postTarget.position = CharacterMainControl.Main.transform.position;
		}
	}

	// Token: 0x04000B1D RID: 2845
	public Camera devCamera;

	// Token: 0x04000B1E RID: 2846
	public Transform postTarget;

	// Token: 0x04000B1F RID: 2847
	private bool active;

	// Token: 0x04000B20 RID: 2848
	public Transform root;

	// Token: 0x04000B21 RID: 2849
	public static bool devCamOn;

	// Token: 0x04000B22 RID: 2850
	private float timer = 1.5f;

	// Token: 0x04000B23 RID: 2851
	private int pressCounter;
}
