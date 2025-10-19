using System;
using Duckov;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020001FB RID: 507
public class UI_Bus_Slider : MonoBehaviour
{
	// Token: 0x170002A8 RID: 680
	// (get) Token: 0x06000EDD RID: 3805 RVA: 0x0003B184 File Offset: 0x00039384
	private AudioManager.Bus BusRef
	{
		get
		{
			if (!AudioManager.Initialized)
			{
				return null;
			}
			if (this.busRef == null)
			{
				this.busRef = AudioManager.GetBus(this.busName);
				if (this.busRef == null)
				{
					Debug.LogError("Bus not found:" + this.busName);
				}
			}
			return this.busRef;
		}
	}

	// Token: 0x06000EDE RID: 3806 RVA: 0x0003B1D8 File Offset: 0x000393D8
	private void Initialize()
	{
		if (this.BusRef == null)
		{
			return;
		}
		this.slider.SetValueWithoutNotify(this.BusRef.Volume);
		this.volumeNumberText.text = (this.BusRef.Volume * 100f).ToString("0");
		this.initialized = true;
	}

	// Token: 0x06000EDF RID: 3807 RVA: 0x0003B234 File Offset: 0x00039434
	private void Awake()
	{
		this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnValueChanged));
	}

	// Token: 0x06000EE0 RID: 3808 RVA: 0x0003B252 File Offset: 0x00039452
	private void Start()
	{
		if (!this.initialized)
		{
			this.Initialize();
		}
	}

	// Token: 0x06000EE1 RID: 3809 RVA: 0x0003B262 File Offset: 0x00039462
	private void OnEnable()
	{
		this.Initialize();
	}

	// Token: 0x06000EE2 RID: 3810 RVA: 0x0003B26C File Offset: 0x0003946C
	private void OnValueChanged(float value)
	{
		if (this.BusRef == null)
		{
			return;
		}
		this.BusRef.Volume = value;
		this.BusRef.Mute = value == 0f;
		this.volumeNumberText.text = (this.BusRef.Volume * 100f).ToString("0");
	}

	// Token: 0x04000C3E RID: 3134
	private AudioManager.Bus busRef;

	// Token: 0x04000C3F RID: 3135
	[SerializeField]
	private string busName;

	// Token: 0x04000C40 RID: 3136
	[SerializeField]
	private TextMeshProUGUI volumeNumberText;

	// Token: 0x04000C41 RID: 3137
	[SerializeField]
	private Slider slider;

	// Token: 0x04000C42 RID: 3138
	private bool initialized;
}
