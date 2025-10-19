using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020001FE RID: 510
public class DecomposeSlider : MonoBehaviour
{
	// Token: 0x14000069 RID: 105
	// (add) Token: 0x06000EED RID: 3821 RVA: 0x0003B3B0 File Offset: 0x000395B0
	// (remove) Token: 0x06000EEE RID: 3822 RVA: 0x0003B3E8 File Offset: 0x000395E8
	public event Action<float> OnValueChangedEvent;

	// Token: 0x170002A9 RID: 681
	// (get) Token: 0x06000EEF RID: 3823 RVA: 0x0003B41D File Offset: 0x0003961D
	// (set) Token: 0x06000EF0 RID: 3824 RVA: 0x0003B42F File Offset: 0x0003962F
	public int Value
	{
		get
		{
			return Mathf.RoundToInt(this.slider.value);
		}
		set
		{
			this.slider.value = (float)value;
			this.valueText.text = value.ToString();
		}
	}

	// Token: 0x06000EF1 RID: 3825 RVA: 0x0003B450 File Offset: 0x00039650
	private void Awake()
	{
		this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnValueChanged));
	}

	// Token: 0x06000EF2 RID: 3826 RVA: 0x0003B46E File Offset: 0x0003966E
	private void OnDestroy()
	{
		this.slider.onValueChanged.RemoveListener(new UnityAction<float>(this.OnValueChanged));
	}

	// Token: 0x06000EF3 RID: 3827 RVA: 0x0003B48C File Offset: 0x0003968C
	private void OnValueChanged(float value)
	{
		this.OnValueChangedEvent(value);
		this.valueText.text = value.ToString();
	}

	// Token: 0x06000EF4 RID: 3828 RVA: 0x0003B4AC File Offset: 0x000396AC
	public void SetMinMax(int min, int max)
	{
		this.slider.minValue = (float)min;
		this.slider.maxValue = (float)max;
		this.minText.text = min.ToString();
		this.maxText.text = max.ToString();
	}

	// Token: 0x04000C45 RID: 3141
	[SerializeField]
	private Slider slider;

	// Token: 0x04000C46 RID: 3142
	public TextMeshProUGUI minText;

	// Token: 0x04000C47 RID: 3143
	public TextMeshProUGUI maxText;

	// Token: 0x04000C48 RID: 3144
	public TextMeshProUGUI valueText;
}
