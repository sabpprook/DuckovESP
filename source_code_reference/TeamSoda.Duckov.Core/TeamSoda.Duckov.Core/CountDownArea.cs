using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.UI;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200010D RID: 269
public class CountDownArea : MonoBehaviour
{
	// Token: 0x170001EC RID: 492
	// (get) Token: 0x06000930 RID: 2352 RVA: 0x00028B9A File Offset: 0x00026D9A
	public float RequiredExtrationTime
	{
		get
		{
			return this.requiredExtrationTime;
		}
	}

	// Token: 0x170001ED RID: 493
	// (get) Token: 0x06000931 RID: 2353 RVA: 0x00028BA2 File Offset: 0x00026DA2
	private float TimeSinceCountDownBegan
	{
		get
		{
			return Time.time - this.timeWhenCountDownBegan;
		}
	}

	// Token: 0x170001EE RID: 494
	// (get) Token: 0x06000932 RID: 2354 RVA: 0x00028BB0 File Offset: 0x00026DB0
	public float RemainingTime
	{
		get
		{
			return Mathf.Clamp(this.RequiredExtrationTime - this.TimeSinceCountDownBegan, 0f, this.RequiredExtrationTime);
		}
	}

	// Token: 0x170001EF RID: 495
	// (get) Token: 0x06000933 RID: 2355 RVA: 0x00028BCF File Offset: 0x00026DCF
	public float Progress
	{
		get
		{
			if (this.requiredExtrationTime <= 0f)
			{
				return 1f;
			}
			return this.TimeSinceCountDownBegan / this.RequiredExtrationTime;
		}
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x00028BF4 File Offset: 0x00026DF4
	private void OnTriggerEnter(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		CharacterMainControl component = other.GetComponent<CharacterMainControl>();
		if (component == null)
		{
			return;
		}
		if (component.IsMainCharacter())
		{
			this.hoveringMainCharacters.Add(component);
			this.OnHoveringMainCharactersChanged();
		}
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x00028C38 File Offset: 0x00026E38
	private void OnTriggerExit(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		CharacterMainControl component = other.GetComponent<CharacterMainControl>();
		if (component == null)
		{
			return;
		}
		if (component.IsMainCharacter())
		{
			this.hoveringMainCharacters.Remove(component);
			this.OnHoveringMainCharactersChanged();
		}
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x00028C7A File Offset: 0x00026E7A
	private void OnHoveringMainCharactersChanged()
	{
		if (!this.countingDown && this.hoveringMainCharacters.Count > 0)
		{
			this.BeginCountDown();
			return;
		}
		if (this.countingDown && this.hoveringMainCharacters.Count < 1)
		{
			this.AbortCountDown();
		}
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x00028CB5 File Offset: 0x00026EB5
	private void BeginCountDown()
	{
		this.countingDown = true;
		this.timeWhenCountDownBegan = Time.time;
		UnityEvent<CountDownArea> unityEvent = this.onCountDownStarted;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke(this);
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x00028CDA File Offset: 0x00026EDA
	private void AbortCountDown()
	{
		this.countingDown = false;
		this.timeWhenCountDownBegan = float.MaxValue;
		UnityEvent<CountDownArea> unityEvent = this.onCountDownStopped;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke(this);
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x00028D00 File Offset: 0x00026F00
	private void UpdateCountDown()
	{
		if (this.hoveringMainCharacters.All((CharacterMainControl e) => e.Health.IsDead))
		{
			this.AbortCountDown();
		}
		if (this.TimeSinceCountDownBegan >= this.RequiredExtrationTime)
		{
			this.OnCountdownSucceed();
		}
		int num = (int)(this.RemainingTime + Time.deltaTime);
		if ((int)this.RemainingTime != num)
		{
			UnityEvent unityEvent = this.onTickSecond;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x0600093A RID: 2362 RVA: 0x00028D7B File Offset: 0x00026F7B
	private void OnCountdownSucceed()
	{
		UnityEvent<CountDownArea> unityEvent = this.onCountDownStopped;
		if (unityEvent != null)
		{
			unityEvent.Invoke(this);
		}
		UnityEvent unityEvent2 = this.onCountDownSucceed;
		if (unityEvent2 != null)
		{
			unityEvent2.Invoke();
		}
		this.countingDown = false;
		if (this.disableWhenSucceed)
		{
			base.enabled = false;
		}
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x00028DB6 File Offset: 0x00026FB6
	private void Update()
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.countingDown && View.ActiveView == null)
		{
			this.UpdateCountDown();
		}
	}

	// Token: 0x04000834 RID: 2100
	[SerializeField]
	private float requiredExtrationTime = 5f;

	// Token: 0x04000835 RID: 2101
	[SerializeField]
	private bool disableWhenSucceed = true;

	// Token: 0x04000836 RID: 2102
	public UnityEvent onCountDownSucceed;

	// Token: 0x04000837 RID: 2103
	public UnityEvent onTickSecond;

	// Token: 0x04000838 RID: 2104
	public UnityEvent<CountDownArea> onCountDownStarted;

	// Token: 0x04000839 RID: 2105
	public UnityEvent<CountDownArea> onCountDownStopped;

	// Token: 0x0400083A RID: 2106
	private bool countingDown;

	// Token: 0x0400083B RID: 2107
	private float timeWhenCountDownBegan = float.MaxValue;

	// Token: 0x0400083C RID: 2108
	private HashSet<CharacterMainControl> hoveringMainCharacters = new HashSet<CharacterMainControl>();
}
