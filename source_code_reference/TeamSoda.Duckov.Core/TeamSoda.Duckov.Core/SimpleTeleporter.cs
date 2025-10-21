using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

// Token: 0x020000AE RID: 174
public class SimpleTeleporter : InteractableBase
{
	// Token: 0x17000121 RID: 289
	// (get) Token: 0x060005BF RID: 1471 RVA: 0x00019B27 File Offset: 0x00017D27
	public Transform TeleportPoint
	{
		get
		{
			if (!this.selfTeleportPoint)
			{
				return base.transform;
			}
			return this.selfTeleportPoint;
		}
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x00019B43 File Offset: 0x00017D43
	protected override void Awake()
	{
		base.Awake();
		this.teleportVolume.gameObject.SetActive(false);
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x00019B5C File Offset: 0x00017D5C
	protected override void OnInteractFinished()
	{
		if (!this.interactCharacter)
		{
			return;
		}
		this.Teleport(this.interactCharacter).Forget();
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x00019B80 File Offset: 0x00017D80
	private async UniTask Teleport(CharacterMainControl targetCharacter)
	{
		SimpleTeleporter.TransitionTypes transitionTypes = this.transitionType;
		if (transitionTypes != SimpleTeleporter.TransitionTypes.volumeFx)
		{
			if (transitionTypes != SimpleTeleporter.TransitionTypes.blackScreen)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.blackScreen = true;
			BlackScreen.ShowAndReturnTask(null, 0f, this.transitionTime);
		}
		else
		{
			this.VolumeFx(true, this.transitionTime).Forget();
		}
		await UniTask.WaitForSeconds(this.transitionTime + this.delay, true, PlayerLoopTiming.Update, default(CancellationToken), false);
		if (targetCharacter != null)
		{
			targetCharacter.SetPosition(this.target.position);
			if (LevelManager.Instance)
			{
				LevelManager.Instance.GameCamera.ForceSyncPos();
			}
		}
		transitionTypes = this.transitionType;
		if (transitionTypes != SimpleTeleporter.TransitionTypes.volumeFx)
		{
			if (transitionTypes != SimpleTeleporter.TransitionTypes.blackScreen)
			{
				throw new ArgumentOutOfRangeException();
			}
			BlackScreen.HideAndReturnTask(null, 0f, this.transitionTime);
			this.blackScreen = false;
		}
		else
		{
			this.VolumeFx(false, this.transitionTime).Forget();
		}
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x00019BCC File Offset: 0x00017DCC
	private async UniTask VolumeFx(bool show, float time)
	{
		float startTime = Time.time;
		bool end = false;
		this.teleportVolume.priority = 9999f;
		this.teleportVolume.gameObject.SetActive(true);
		while (!end)
		{
			float num = Time.time - startTime;
			float num2 = Mathf.Clamp01(num / time);
			if (!show)
			{
				num2 = 1f - num2;
			}
			this.teleportVolume.weight = num2;
			Shader.SetGlobalFloat(this.fxShaderID, num2);
			if (num > time)
			{
				if (!show)
				{
					this.teleportVolume.gameObject.SetActive(false);
				}
				end = true;
			}
			await UniTask.Yield();
		}
	}

	// Token: 0x04000542 RID: 1346
	public Transform target;

	// Token: 0x04000543 RID: 1347
	[SerializeField]
	private Transform selfTeleportPoint;

	// Token: 0x04000544 RID: 1348
	[SerializeField]
	private SimpleTeleporter.TransitionTypes transitionType;

	// Token: 0x04000545 RID: 1349
	[FormerlySerializedAs("fxTime")]
	public float transitionTime = 0.28f;

	// Token: 0x04000546 RID: 1350
	private float delay = 0.3f;

	// Token: 0x04000547 RID: 1351
	public Volume teleportVolume;

	// Token: 0x04000548 RID: 1352
	private int fxShaderID = Shader.PropertyToID("TeleportFXStrength");

	// Token: 0x04000549 RID: 1353
	private bool blackScreen;

	// Token: 0x02000455 RID: 1109
	public enum TransitionTypes
	{
		// Token: 0x04001ADE RID: 6878
		volumeFx,
		// Token: 0x04001ADF RID: 6879
		blackScreen
	}
}
