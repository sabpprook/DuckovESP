using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x0200000A RID: 10
	[NullableContext(1)]
	[Nullable(0)]
	public static class EnemyInfoHelper
	{
		// Token: 0x06000035 RID: 53 RVA: 0x000031C0 File Offset: 0x000013C0
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

		// Token: 0x06000036 RID: 54 RVA: 0x00003210 File Offset: 0x00001410
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

		// Token: 0x06000037 RID: 55 RVA: 0x0000328C File Offset: 0x0000148C
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

		// Token: 0x06000038 RID: 56 RVA: 0x0000330A File Offset: 0x0000150A
		public static Vector3 GetPlayerPosition(CharacterMainControl player)
		{
			if (!(((player != null) ? player.transform : null) != null))
			{
				return Vector3.zero;
			}
			return player.transform.position + Vector3.up * 1f;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003348 File Offset: 0x00001548
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
