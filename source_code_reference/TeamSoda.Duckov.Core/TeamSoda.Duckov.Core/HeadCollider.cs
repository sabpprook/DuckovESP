using System;
using UnityEngine;

// Token: 0x02000063 RID: 99
public class HeadCollider : MonoBehaviour
{
	// Token: 0x0600039C RID: 924 RVA: 0x0000FDB9 File Offset: 0x0000DFB9
	public void Init(CharacterMainControl _character)
	{
		this.character = _character;
		this.character.OnTeamChanged += this.OnSetTeam;
	}

	// Token: 0x0600039D RID: 925 RVA: 0x0000FDD9 File Offset: 0x0000DFD9
	private void OnDestroy()
	{
		if (this.character)
		{
			this.character.OnTeamChanged -= this.OnSetTeam;
		}
	}

	// Token: 0x0600039E RID: 926 RVA: 0x0000FE00 File Offset: 0x0000E000
	private void OnSetTeam(Teams team)
	{
		bool flag = Team.IsEnemy(Teams.player, team);
		this.sphereCollider.enabled = flag;
	}

	// Token: 0x0600039F RID: 927 RVA: 0x0000FE24 File Offset: 0x0000E024
	private void OnDrawGizmos()
	{
		Color yellow = Color.yellow;
		yellow.a = 0.3f;
		Gizmos.color = yellow;
		Gizmos.DrawSphere(base.transform.position, this.sphereCollider.radius * base.transform.lossyScale.x);
	}

	// Token: 0x040002BF RID: 703
	private CharacterMainControl character;

	// Token: 0x040002C0 RID: 704
	[SerializeField]
	private SphereCollider sphereCollider;
}
