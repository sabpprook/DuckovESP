using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001E4 RID: 484
public class Soda_Joysticks : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IDragHandler
{
	// Token: 0x1700029B RID: 667
	// (get) Token: 0x06000E49 RID: 3657 RVA: 0x000396A0 File Offset: 0x000378A0
	public bool Holding
	{
		get
		{
			return this.holding;
		}
	}

	// Token: 0x1700029C RID: 668
	// (get) Token: 0x06000E4A RID: 3658 RVA: 0x000396A8 File Offset: 0x000378A8
	public Vector2 InputValue
	{
		get
		{
			return this.inputValue;
		}
	}

	// Token: 0x06000E4B RID: 3659 RVA: 0x000396B0 File Offset: 0x000378B0
	private void Start()
	{
		this.joyImage.gameObject.SetActive(false);
		if (this.hideWhenNotTouch)
		{
			this.canvasGroup.alpha = 0f;
		}
		if (this.cancleRangeCanvasGroup)
		{
			this.cancleRangeCanvasGroup.alpha = 0f;
		}
	}

	// Token: 0x06000E4C RID: 3660 RVA: 0x00039703 File Offset: 0x00037903
	private void Update()
	{
		if (this.holding && !this.usable)
		{
			this.Revert();
		}
	}

	// Token: 0x06000E4D RID: 3661 RVA: 0x0003971B File Offset: 0x0003791B
	private void OnEnable()
	{
		if (this.cancleRangeCanvasGroup)
		{
			this.cancleRangeCanvasGroup.alpha = 0f;
		}
		this.triggeringCancle = false;
	}

	// Token: 0x06000E4E RID: 3662 RVA: 0x00039744 File Offset: 0x00037944
	public void OnPointerDown(PointerEventData eventData)
	{
		if (!this.usable)
		{
			return;
		}
		if (this.holding)
		{
			return;
		}
		this.holding = true;
		this.currentPointerID = eventData.pointerId;
		this.downPoint = eventData.position;
		this.verticalRes = Screen.height;
		this.joystickRangePixel = (float)this.verticalRes * this.joystickRangePercent;
		this.cancleRangePixel = (float)this.verticalRes * this.cancleRangePercent;
		if (!this.fixedPositon)
		{
			this.backGround.transform.position = this.downPoint;
		}
		this.joyImage.transform.position = this.backGround.transform.position;
		this.backGround.transform.rotation = Quaternion.Euler(Vector3.zero);
		this.joyImage.gameObject.SetActive(true);
		UnityEvent<Vector2, bool> updateValueEvent = this.UpdateValueEvent;
		if (updateValueEvent != null)
		{
			updateValueEvent.Invoke(Vector2.zero, true);
		}
		if (this.hideWhenNotTouch)
		{
			this.canvasGroup.alpha = 1f;
		}
		if (this.canCancle && this.cancleRangeCanvasGroup)
		{
			this.cancleRangeCanvasGroup.alpha = 0.12f;
		}
		this.triggeringCancle = false;
		UnityEvent onTouchEvent = this.OnTouchEvent;
		if (onTouchEvent == null)
		{
			return;
		}
		onTouchEvent.Invoke();
	}

	// Token: 0x06000E4F RID: 3663 RVA: 0x00039890 File Offset: 0x00037A90
	public void OnPointerUp(PointerEventData eventData)
	{
		if (!this.usable)
		{
			return;
		}
		UnityEvent<bool> onUpEvent = this.OnUpEvent;
		if (onUpEvent != null)
		{
			onUpEvent.Invoke(!this.triggeringCancle);
		}
		UnityEvent<Vector2, bool> updateValueEvent = this.UpdateValueEvent;
		if (updateValueEvent != null)
		{
			updateValueEvent.Invoke(Vector2.zero, false);
		}
		if (this.holding && this.currentPointerID == eventData.pointerId)
		{
			this.Revert();
		}
	}

	// Token: 0x06000E50 RID: 3664 RVA: 0x000398F4 File Offset: 0x00037AF4
	private void Revert()
	{
		UnityEvent<Vector2, bool> updateValueEvent = this.UpdateValueEvent;
		if (updateValueEvent != null)
		{
			updateValueEvent.Invoke(Vector2.zero, false);
		}
		if (this.holding)
		{
			UnityEvent<bool> onUpEvent = this.OnUpEvent;
			if (onUpEvent != null)
			{
				onUpEvent.Invoke(false);
			}
		}
		if (!this.usable)
		{
			return;
		}
		this.joyImage.transform.position = this.backGround.transform.position;
		this.inputValue = Vector2.zero;
		this.holding = false;
		this.backGround.transform.rotation = Quaternion.Euler(Vector3.zero);
		if (this.joyImage.gameObject.activeSelf)
		{
			this.joyImage.gameObject.SetActive(false);
		}
		if (this.hideWhenNotTouch)
		{
			this.canvasGroup.alpha = 0f;
		}
		if (this.cancleRangeCanvasGroup)
		{
			this.cancleRangeCanvasGroup.alpha = 0f;
		}
	}

	// Token: 0x06000E51 RID: 3665 RVA: 0x000399DF File Offset: 0x00037BDF
	public void CancleTouch()
	{
		this.Revert();
	}

	// Token: 0x06000E52 RID: 3666 RVA: 0x000399E7 File Offset: 0x00037BE7
	public void OnDisable()
	{
		this.Revert();
	}

	// Token: 0x06000E53 RID: 3667 RVA: 0x000399F0 File Offset: 0x00037BF0
	public void OnDrag(PointerEventData eventData)
	{
		if (this.holding && eventData.pointerId == this.currentPointerID)
		{
			Vector2 vector = eventData.position;
			if (vector == this.downPoint)
			{
				this.inputValue = Vector2.zero;
				return;
			}
			float num = Vector2.Distance(vector, this.downPoint);
			float num2 = num;
			Vector2 normalized = (vector - this.downPoint).normalized;
			if (num > this.joystickRangePixel)
			{
				if (this.followFinger)
				{
					this.downPoint += (num - this.joystickRangePixel) * normalized;
				}
				if (!this.fixedPositon && this.followFinger)
				{
					this.backGround.transform.position = this.downPoint;
				}
				num2 = this.joystickRangePixel;
			}
			vector = this.downPoint + normalized * num2;
			Vector2 vector2 = Vector2.zero;
			if (this.joystickRangePixel > 0f)
			{
				vector2 = normalized * num2 / this.joystickRangePixel;
			}
			this.joyImage.transform.position = this.backGround.transform.position + normalized * num2;
			Vector3 vector3 = Vector3.zero;
			vector3.y = -vector2.x;
			vector3.x = vector2.y;
			vector3 *= this.rotValue;
			this.backGround.transform.rotation = Quaternion.Euler(vector3);
			float num3 = vector2.magnitude;
			num3 = Mathf.InverseLerp(this.deadZone, this.fullZone, num3);
			this.inputValue = num3 * normalized;
			UnityEvent<Vector2, bool> updateValueEvent = this.UpdateValueEvent;
			if (updateValueEvent != null)
			{
				updateValueEvent.Invoke(this.inputValue, true);
			}
			if (this.canCancle && this.cancleRangeCanvasGroup)
			{
				if (num >= this.cancleRangePixel)
				{
					this.cancleRangeCanvasGroup.alpha = 1f;
					this.triggeringCancle = true;
					return;
				}
				this.cancleRangeCanvasGroup.alpha = 0.12f;
				this.triggeringCancle = false;
			}
		}
	}

	// Token: 0x04000BCC RID: 3020
	public bool usable = true;

	// Token: 0x04000BCD RID: 3021
	private int verticalRes;

	// Token: 0x04000BCE RID: 3022
	[Range(0f, 0.5f)]
	public float joystickRangePercent = 0.3f;

	// Token: 0x04000BCF RID: 3023
	[Range(0f, 0.5f)]
	public float cancleRangePercent = 0.4f;

	// Token: 0x04000BD0 RID: 3024
	public bool fixedPositon = true;

	// Token: 0x04000BD1 RID: 3025
	public bool followFinger;

	// Token: 0x04000BD2 RID: 3026
	public bool canCancle;

	// Token: 0x04000BD3 RID: 3027
	private float joystickRangePixel;

	// Token: 0x04000BD4 RID: 3028
	private float cancleRangePixel;

	// Token: 0x04000BD5 RID: 3029
	[SerializeField]
	private Transform backGround;

	// Token: 0x04000BD6 RID: 3030
	[SerializeField]
	private Image joyImage;

	// Token: 0x04000BD7 RID: 3031
	[SerializeField]
	private CanvasGroup cancleRangeCanvasGroup;

	// Token: 0x04000BD8 RID: 3032
	private bool holding;

	// Token: 0x04000BD9 RID: 3033
	private Vector2 downPoint;

	// Token: 0x04000BDA RID: 3034
	private int currentPointerID;

	// Token: 0x04000BDB RID: 3035
	private Vector2 inputValue;

	// Token: 0x04000BDC RID: 3036
	[SerializeField]
	private float rotValue = 10f;

	// Token: 0x04000BDD RID: 3037
	[Range(0f, 1f)]
	public float deadZone;

	// Token: 0x04000BDE RID: 3038
	[Range(0f, 1f)]
	public float fullZone = 1f;

	// Token: 0x04000BDF RID: 3039
	public bool hideWhenNotTouch;

	// Token: 0x04000BE0 RID: 3040
	public CanvasGroup canvasGroup;

	// Token: 0x04000BE1 RID: 3041
	private bool triggeringCancle;

	// Token: 0x04000BE2 RID: 3042
	public UnityEvent<Vector2, bool> UpdateValueEvent;

	// Token: 0x04000BE3 RID: 3043
	public UnityEvent OnTouchEvent;

	// Token: 0x04000BE4 RID: 3044
	public UnityEvent<bool> OnUpEvent;
}
