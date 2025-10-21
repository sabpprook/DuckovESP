using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x0200000D RID: 13
	[NullableContext(1)]
	[Nullable(0)]
	public static class EnemyInfoHelper
	{
		// Token: 0x0600004F RID: 79 RVA: 0x00004620 File Offset: 0x00002820
		public static string GetEnemyName(CharacterMainControl enemy)
		{
			Teams team = enemy.Team;
			switch (team)
			{
			case Teams.scav:
				return "Scav";
			case (Teams)2:
				break;
			case Teams.usec:
				return "USEC";
			case Teams.bear:
				return "BEAR";
			default:
				if (team == Teams.wolf)
				{
					return "Wolf";
				}
				break;
			}
			return "Enemy";
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00004670 File Offset: 0x00002870
		public static string GetWeaponName(CharacterMainControl enemy)
		{
			try
			{
				ItemAgent_Gun gun = enemy.GetGun();
				if (((gun != null) ? gun.Item : null) != null)
				{
					return gun.Item.DisplayName;
				}
				ItemAgent_MeleeWeapon meleeWeapon = enemy.GetMeleeWeapon();
				if (((meleeWeapon != null) ? meleeWeapon.Item : null) != null)
				{
					return meleeWeapon.Item.DisplayName;
				}
			}
			catch
			{
			}
			return "无";
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000046EC File Offset: 0x000028EC
		public static Color GetSpecialColor(CharacterMainControl enemy, long enemyValue, long highValueThreshold)
		{
			if (enemyValue >= highValueThreshold)
			{
				return new Color(1f, 0.843f, 0f);
			}
			if (enemy.Team == Teams.usec || enemy.Team == Teams.bear)
			{
				return new Color(1f, 0.843f, 0f);
			}
			string weaponName = EnemyInfoHelper.GetWeaponName(enemy);
			if (EnemyInfoHelper.GetEnemyName(enemy) == "Enemy" && weaponName == "无")
			{
				return Color.yellow;
			}
			return Color.green;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x0000476A File Offset: 0x0000296A
		public static Vector3 GetPlayerPosition(CharacterMainControl player)
		{
			if (!(((player != null) ? player.transform : null) != null))
			{
				return Vector3.zero;
			}
			return player.transform.position + Vector3.up * 1f;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x000047A8 File Offset: 0x000029A8
		public static Vector3 GetEnemyHeadPosition(CharacterMainControl enemy)
		{
			try
			{
				CharacterModel characterModel = enemy.characterModel;
				if (((characterModel != null) ? characterModel.HelmatSocket : null) != null)
				{
					return enemy.characterModel.HelmatSocket.position;
				}
			}
			catch
			{
			}
			if (!(enemy.transform != null))
			{
				return Vector3.zero;
			}
			return enemy.transform.position + Vector3.up * 1.8f;
		}
	}
}
