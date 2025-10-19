using System;
using FOW;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200017A RID: 378
public class FogOfWarManager : MonoBehaviour
{
	// Token: 0x06000B76 RID: 2934 RVA: 0x00030725 File Offset: 0x0002E925
	private void Start()
	{
		LevelManager.OnMainCharacterDead += this.OnCharacterDie;
	}

	// Token: 0x06000B77 RID: 2935 RVA: 0x00030738 File Offset: 0x0002E938
	private void OnDestroy()
	{
		LevelManager.OnMainCharacterDead -= this.OnCharacterDie;
	}

	// Token: 0x06000B78 RID: 2936 RVA: 0x0003074B File Offset: 0x0002E94B
	private void Init()
	{
		this.inited = true;
		if (!LevelManager.Instance.IsRaidMap || !LevelManager.Rule.FogOfWar)
		{
			this.allVision = true;
		}
	}

	// Token: 0x06000B79 RID: 2937 RVA: 0x00030774 File Offset: 0x0002E974
	private void Update()
	{
		if (!LevelManager.LevelInited)
		{
			return;
		}
		if (!this.character)
		{
			this.character = CharacterMainControl.Main;
			if (!this.character)
			{
				return;
			}
		}
		if (!this.inited)
		{
			this.Init();
		}
		if (!this.timeOfDayController)
		{
			this.timeOfDayController = LevelManager.Instance.TimeOfDayController;
			if (!this.timeOfDayController)
			{
				return;
			}
		}
		Vector3 vector = this.character.transform.position + Vector3.up * this.mianVisYOffset;
		this.mainVis.transform.position = vector;
		vector = new Vector3((float)Mathf.RoundToInt(vector.x), (float)Mathf.RoundToInt(vector.y), (float)Mathf.RoundToInt(vector.z));
		this.fogOfWar.UpdateWorldBounds(vector, new Vector3(128f, 1f, 128f));
		Vector3 vector2 = this.character.GetCurrentAimPoint() - this.character.transform.position;
		Debug.DrawLine(this.character.GetCurrentAimPoint(), this.character.GetCurrentAimPoint() + Vector3.up * 2f, Color.green, 0.2f);
		vector2.y = 0f;
		vector2.Normalize();
		float num = Mathf.Clamp01(this.character.NightVisionAbility + (this.character.FlashLight ? 0.3f : 0f));
		float num2 = this.character.ViewAngle;
		float num3 = this.character.SenseRange;
		float num4 = this.character.ViewDistance;
		num2 *= Mathf.Lerp(TimeOfDayController.NightViewAngleFactor, 1f, num);
		num3 *= Mathf.Lerp(TimeOfDayController.NightSenseRangeFactor, 1f, num);
		num4 *= Mathf.Lerp(TimeOfDayController.NightViewDistanceFactor, 1f, num);
		if (num4 < num3 - 2.5f)
		{
			num4 = num3 - 2.5f;
		}
		if (this.allVision)
		{
			num2 = 360f;
			num3 = 50f;
			num4 = 50f;
		}
		if (num2 != this.viewAgnel)
		{
			if (this.viewAgnel < 0f)
			{
				this.viewAgnel = num2;
			}
			this.viewAgnel = Mathf.MoveTowards(this.viewAgnel, num2, 120f * Time.deltaTime);
			this.mainVis.ViewAngle = this.viewAgnel;
		}
		if (num3 != this.senseRange)
		{
			if (this.senseRange < 0f)
			{
				this.senseRange = num3;
			}
			this.senseRange = Mathf.MoveTowards(this.senseRange, num3, 2f * Time.deltaTime);
			this.mainVis.UnobscuredRadius = this.senseRange;
		}
		if (num4 != this.viewDistance)
		{
			if (this.viewDistance < 0f)
			{
				this.viewDistance = num4;
			}
			this.viewDistance = Mathf.MoveTowards(this.viewDistance, num4, 30f * Time.deltaTime);
			this.mainVis.ViewRadius = this.viewDistance;
		}
		this.mainVis.transform.rotation = Quaternion.LookRotation(vector2, Vector3.up);
	}

	// Token: 0x06000B7A RID: 2938 RVA: 0x00030A99 File Offset: 0x0002EC99
	private void OnCharacterDie(DamageInfo dmgInfo)
	{
		LevelManager.OnMainCharacterDead -= this.OnCharacterDie;
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x040009C2 RID: 2498
	[FormerlySerializedAs("mianVis")]
	public FogOfWarRevealer3D mainVis;

	// Token: 0x040009C3 RID: 2499
	public float mianVisYOffset = 1f;

	// Token: 0x040009C4 RID: 2500
	private CharacterMainControl character;

	// Token: 0x040009C5 RID: 2501
	public FogOfWarWorld fogOfWar;

	// Token: 0x040009C6 RID: 2502
	private float viewAgnel = -1f;

	// Token: 0x040009C7 RID: 2503
	private float senseRange = -1f;

	// Token: 0x040009C8 RID: 2504
	private float viewDistance = -1f;

	// Token: 0x040009C9 RID: 2505
	private TimeOfDayController timeOfDayController;

	// Token: 0x040009CA RID: 2506
	private bool allVision;

	// Token: 0x040009CB RID: 2507
	private bool inited;
}
