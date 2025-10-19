using System;
using Duckov.Utilities;
using UnityEngine;

// Token: 0x02000076 RID: 118
public class AimTargetFinder : MonoBehaviour
{
	// Token: 0x06000457 RID: 1111 RVA: 0x000140B9 File Offset: 0x000122B9
	private void Start()
	{
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x000140BC File Offset: 0x000122BC
	public Transform Find(bool search, Vector3 findPoint, ref CharacterMainControl foundCharacter)
	{
		Transform transform = null;
		if (search)
		{
			transform = this.Search(findPoint, ref foundCharacter);
		}
		return transform;
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x000140D8 File Offset: 0x000122D8
	private Transform Search(Vector3 findPoint, ref CharacterMainControl character)
	{
		character = null;
		if (this.overlapcColliders == null)
		{
			this.overlapcColliders = new Collider[6];
			this.damageReceiverLayers = GameplayDataSettings.Layers.damageReceiverLayerMask;
		}
		int num = Physics.OverlapSphereNonAlloc(findPoint, this.searchRadius, this.overlapcColliders, this.damageReceiverLayers);
		Collider collider = null;
		if (num > 0)
		{
			int i = 0;
			while (i < num)
			{
				DamageReceiver component = this.overlapcColliders[i].GetComponent<DamageReceiver>();
				if (!(component == null) && component.Team != Teams.player)
				{
					collider = this.overlapcColliders[i];
					if (component.health != null)
					{
						character = component.health.GetComponent<CharacterMainControl>();
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
		if (collider)
		{
			return collider.transform;
		}
		return null;
	}

	// Token: 0x040003C2 RID: 962
	private Vector3 searchPoint;

	// Token: 0x040003C3 RID: 963
	public float searchRadius;

	// Token: 0x040003C4 RID: 964
	private LayerMask damageReceiverLayers;

	// Token: 0x040003C5 RID: 965
	private Collider[] overlapcColliders;
}
