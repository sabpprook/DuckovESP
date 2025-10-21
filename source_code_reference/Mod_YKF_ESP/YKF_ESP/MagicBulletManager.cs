using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x02000013 RID: 19
	[NullableContext(1)]
	[Nullable(0)]
	public class MagicBulletManager
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x000059CF File Offset: 0x00003BCF
		public bool IsActive
		{
			get
			{
				return this.settings.MagicBulletActive;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x000059DC File Offset: 0x00003BDC
		public CharacterMainControl CurrentTarget
		{
			get
			{
				return this.currentTarget;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x000059E4 File Offset: 0x00003BE4
		public float Range
		{
			get
			{
				return this.settings.MagicBulletRange;
			}
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x000059F1 File Offset: 0x00003BF1
		public MagicBulletManager(ESPSettings settings, LogManager logManager)
		{
			this.settings = settings;
			this.logManager = logManager;
			this.InitializeReflection();
			this.lastProcessTime = Time.time;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00005A18 File Offset: 0x00003C18
		public void UpdateSettings(ESPSettings newSettings)
		{
			this.settings = newSettings;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00005A24 File Offset: 0x00003C24
		public void ToggleMagicBullet(CharacterMainControl player)
		{
			this.settings.MagicBulletActive = !this.settings.MagicBulletActive;
			if (this.settings.MagicBulletActive)
			{
				if (!this.hasShownActivationMessage && player != null)
				{
					player.PopText("魔法子弹已启动！直接命中目标头部！", 5f);
					this.hasShownActivationMessage = true;
				}
				this.logManager.WriteLog("Magic Bullet activated - Direct Hit Mode", false);
				return;
			}
			this.logManager.WriteLog("Magic Bullet deactivated", false);
			this.currentTarget = null;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00005AAC File Offset: 0x00003CAC
		public void Update(CharacterMainControl player, List<EnemyInfo> enemyInfoList)
		{
			if (!this.IsActive || player == null)
			{
				this.currentTarget = null;
				return;
			}
			this.UpdateTarget(player, enemyInfoList);
			if (Time.time - this.lastProcessTime > 0.02f)
			{
				this.ProcessDirectHits(player);
				this.lastProcessTime = Time.time;
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00005B00 File Offset: 0x00003D00
		private void UpdateTarget(CharacterMainControl player, List<EnemyInfo> enemyInfoList)
		{
			if (Time.time - this.lastRangeUpdate < 0.1f)
			{
				return;
			}
			this.lastRangeUpdate = Time.time;
			this.currentTarget = null;
			float num = float.MaxValue;
			EnemyInfoHelper.GetPlayerPosition(player);
			foreach (EnemyInfo enemyInfo in enemyInfoList)
			{
				if (enemyInfo.Character != null && !enemyInfo.Character.IsDead() && enemyInfo.Distance <= this.settings.MagicBulletRange && enemyInfo.Distance < num)
				{
					this.currentTarget = enemyInfo.Character;
					num = enemyInfo.Distance;
				}
			}
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00005BC4 File Offset: 0x00003DC4
		private void ProcessDirectHits(CharacterMainControl player)
		{
			if (!this.reflectionInitialized || this.currentTarget == null)
			{
				return;
			}
			try
			{
				foreach (Object @object in Object.FindObjectsOfType(this.bulletType))
				{
					if (!(@object == null) && this.IsBulletFromPlayer(@object, player) && !this.HasBulletHit(@object))
					{
						this.SetDirectHit(@object);
					}
				}
			}
			catch (Exception ex)
			{
				this.logManager.WriteLog(string.Format("Error processing direct hits: {0}", ex), true);
			}
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00005C58 File Offset: 0x00003E58
		private bool IsBulletFromPlayer(object bullet, CharacterMainControl player)
		{
			bool flag;
			try
			{
				FieldInfo field = this.bulletType.GetField("context", BindingFlags.Instance | BindingFlags.Public);
				if (field != null)
				{
					object value = field.GetValue(bullet);
					if (value != null)
					{
						FieldInfo field2 = value.GetType().GetField("fromCharacter");
						if (field2 != null)
						{
							return field2.GetValue(value) == player;
						}
					}
				}
				FieldInfo field3 = this.bulletType.GetField("shooter", BindingFlags.Instance | BindingFlags.Public);
				if (field3 != null)
				{
					flag = field3.GetValue(bullet) == player;
				}
				else
				{
					flag = false;
				}
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00005CFC File Offset: 0x00003EFC
		private bool HasBulletHit(object bullet)
		{
			try
			{
				FieldInfo field = this.bulletType.GetField("dead", BindingFlags.Instance | BindingFlags.Public);
				if (field != null && (bool)field.GetValue(bullet))
				{
					return true;
				}
				FieldInfo field2 = this.bulletType.GetField("overMaxDistance", BindingFlags.Instance | BindingFlags.Public);
				if (field2 != null && (bool)field2.GetValue(bullet))
				{
					return true;
				}
				return false;
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00005D80 File Offset: 0x00003F80
		private void SetDirectHit(object bullet)
		{
			try
			{
				Vector3 enemyHeadPosition = EnemyInfoHelper.GetEnemyHeadPosition(this.currentTarget);
				((MonoBehaviour)bullet).transform.position = enemyHeadPosition;
				Rigidbody component = ((MonoBehaviour)bullet).GetComponent<Rigidbody>();
				if (component != null)
				{
					component.velocity = Vector3.zero;
					component.angularVelocity = Vector3.zero;
				}
				FieldInfo field = this.bulletType.GetField("context", BindingFlags.Instance | BindingFlags.Public);
				if (field != null)
				{
					object value = field.GetValue(bullet);
					if (value != null)
					{
						FieldInfo field2 = value.GetType().GetField("hitPoint");
						if (field2 != null)
						{
							field2.SetValue(value, enemyHeadPosition);
						}
					}
				}
				FieldInfo field3 = this.bulletType.GetField("dead", BindingFlags.Instance | BindingFlags.Public);
				if (field3 != null)
				{
					field3.SetValue(bullet, true);
				}
				this.logManager.WriteLog(string.Format("Magic bullet direct hit at {0}", enemyHeadPosition), false);
			}
			catch (Exception ex)
			{
				this.logManager.WriteLog(string.Format("Error setting direct hit: {0}", ex), true);
			}
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00005EA0 File Offset: 0x000040A0
		private void InitializeReflection()
		{
			try
			{
				this.bulletType = typeof(Projectile);
				if (this.bulletType != null)
				{
					this.reflectionInitialized = true;
					this.logManager.WriteLog("Using Projectile type for magic bullet", false);
				}
			}
			catch (Exception ex)
			{
				this.logManager.WriteLog(string.Format("Error initializing reflection: {0}", ex), true);
			}
		}

		// Token: 0x04000056 RID: 86
		private ESPSettings settings;

		// Token: 0x04000057 RID: 87
		private LogManager logManager;

		// Token: 0x04000058 RID: 88
		private bool hasShownActivationMessage;

		// Token: 0x04000059 RID: 89
		private CharacterMainControl currentTarget;

		// Token: 0x0400005A RID: 90
		private Vector3 lastPlayerPosition;

		// Token: 0x0400005B RID: 91
		private float lastRangeUpdate;

		// Token: 0x0400005C RID: 92
		private const float rangeUpdateInterval = 0.1f;

		// Token: 0x0400005D RID: 93
		private float lastProcessTime;

		// Token: 0x0400005E RID: 94
		private const float processInterval = 0.02f;

		// Token: 0x0400005F RID: 95
		private Type bulletType;

		// Token: 0x04000060 RID: 96
		private FieldInfo bulletHitPointField;

		// Token: 0x04000061 RID: 97
		private FieldInfo bulletHasHitField;

		// Token: 0x04000062 RID: 98
		private MethodInfo bulletHitMethod;

		// Token: 0x04000063 RID: 99
		private bool reflectionInitialized;
	}
}
