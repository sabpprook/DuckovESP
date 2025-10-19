using System;
using Duckov.Scenes;
using UnityEngine;

// Token: 0x020000AB RID: 171
public class SetActiveByChance : MonoBehaviour
{
	// Token: 0x060005B6 RID: 1462 RVA: 0x000198E8 File Offset: 0x00017AE8
	private void Awake()
	{
		bool flag = global::UnityEngine.Random.Range(0f, 1f) < this.activeChange;
		if (this.saveInLevel && MultiSceneCore.Instance)
		{
			object obj;
			if (MultiSceneCore.Instance.inLevelData.TryGetValue(this.keyCached, out obj) && obj is bool)
			{
				bool flag2 = (bool)obj;
				Debug.Log(string.Format("存在门存档信息：{0}", flag2));
				flag = flag2;
			}
			MultiSceneCore.Instance.inLevelData[this.keyCached] = flag;
		}
		base.gameObject.SetActive(flag);
	}

	// Token: 0x060005B7 RID: 1463 RVA: 0x00019988 File Offset: 0x00017B88
	private int GetKey()
	{
		Vector3 vector = base.transform.position * 10f;
		int num = Mathf.RoundToInt(vector.x);
		int num2 = Mathf.RoundToInt(vector.y);
		int num3 = Mathf.RoundToInt(vector.z);
		Vector3Int vector3Int = new Vector3Int(num, num2, num3);
		return string.Format("Door_{0}", vector3Int).GetHashCode();
	}

	// Token: 0x04000538 RID: 1336
	public bool saveInLevel;

	// Token: 0x04000539 RID: 1337
	private int keyCached;

	// Token: 0x0400053A RID: 1338
	[Range(0f, 1f)]
	public float activeChange = 0.5f;
}
