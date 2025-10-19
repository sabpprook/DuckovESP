using System;
using System.Collections.Generic;
using UnityEngine;

namespace FX
{
	// Token: 0x0200020B RID: 523
	public class PopText : MonoBehaviour
	{
		// Token: 0x06000F87 RID: 3975 RVA: 0x0003D105 File Offset: 0x0003B305
		private void Awake()
		{
			PopText.instance = this;
		}

		// Token: 0x06000F88 RID: 3976 RVA: 0x0003D110 File Offset: 0x0003B310
		private PopTextEntity GetOrCreateEntry()
		{
			PopTextEntity popTextEntity;
			if (this.inactiveEntries.Count > 0)
			{
				popTextEntity = this.inactiveEntries[0];
				this.inactiveEntries.RemoveAt(0);
			}
			popTextEntity = global::UnityEngine.Object.Instantiate<PopTextEntity>(this.popTextPrefab, base.transform);
			this.activeEntries.Add(popTextEntity);
			popTextEntity.gameObject.SetActive(true);
			return popTextEntity;
		}

		// Token: 0x06000F89 RID: 3977 RVA: 0x0003D170 File Offset: 0x0003B370
		public void InstancePop(string text, Vector3 worldPosition, Color color, float size, Sprite sprite = null)
		{
			PopTextEntity orCreateEntry = this.GetOrCreateEntry();
			orCreateEntry.Color = color;
			orCreateEntry.size = size;
			orCreateEntry.transform.localScale = Vector3.one * size;
			Transform transform = orCreateEntry.transform;
			transform.position = worldPosition;
			transform.rotation = PopText.LookAtMainCamera(worldPosition);
			float num = global::UnityEngine.Random.Range(-this.randomAngle, this.randomAngle);
			float num2 = global::UnityEngine.Random.Range(-this.randomAngle, this.randomAngle);
			Vector3 vector = Quaternion.Euler(num, 0f, num2) * Vector3.up;
			orCreateEntry.SetupContent(text, sprite);
			orCreateEntry.velocity = vector * this.spawnVelocity;
			orCreateEntry.spawnTime = Time.time;
		}

		// Token: 0x06000F8A RID: 3978 RVA: 0x0003D224 File Offset: 0x0003B424
		private static Quaternion LookAtMainCamera(Vector3 position)
		{
			if (Camera.main)
			{
				Transform transform = Camera.main.transform;
				return Quaternion.LookRotation(-(transform.position - position), transform.up);
			}
			return Quaternion.identity;
		}

		// Token: 0x06000F8B RID: 3979 RVA: 0x0003D26A File Offset: 0x0003B46A
		public void Recycle(PopTextEntity entry)
		{
			entry.gameObject.SetActive(false);
			this.activeEntries.Remove(entry);
			this.inactiveEntries.Add(entry);
		}

		// Token: 0x06000F8C RID: 3980 RVA: 0x0003D294 File Offset: 0x0003B494
		private void Update()
		{
			float deltaTime = Time.deltaTime;
			Vector3 vector = Vector3.up * this.gravityValue;
			bool flag = false;
			foreach (PopTextEntity popTextEntity in this.activeEntries)
			{
				if (popTextEntity == null)
				{
					flag = true;
				}
				else
				{
					Transform transform = popTextEntity.transform;
					transform.position += popTextEntity.velocity * deltaTime;
					transform.rotation = PopText.LookAtMainCamera(transform.position);
					popTextEntity.velocity += vector * deltaTime;
					popTextEntity.transform.localScale = this.sizeOverLife.Evaluate(popTextEntity.timeSinceSpawn / this.lifeTime) * popTextEntity.size * Vector3.one;
					float num = Mathf.Clamp01(popTextEntity.timeSinceSpawn / this.lifeTime * 2f - 1f);
					Color color = Color.Lerp(popTextEntity.Color, popTextEntity.EndColor, num);
					popTextEntity.SetColor(color);
					if (popTextEntity.timeSinceSpawn > this.lifeTime)
					{
						this.recycleList.Add(popTextEntity);
					}
				}
			}
			if (this.recycleList.Count > 0)
			{
				foreach (PopTextEntity popTextEntity2 in this.recycleList)
				{
					this.Recycle(popTextEntity2);
				}
				this.recycleList.Clear();
			}
			if (flag)
			{
				this.activeEntries.RemoveAll((PopTextEntity e) => e == null);
			}
		}

		// Token: 0x06000F8D RID: 3981 RVA: 0x0003D498 File Offset: 0x0003B698
		private void PopTest()
		{
			Vector3 vector = base.transform.position;
			CharacterMainControl main = CharacterMainControl.Main;
			if (main != null)
			{
				vector = main.transform.position + Vector3.up * 2f;
			}
			this.InstancePop("Test", vector, Color.white, 1f, this.debugSprite);
		}

		// Token: 0x06000F8E RID: 3982 RVA: 0x0003D4FC File Offset: 0x0003B6FC
		public static void Pop(string text, Vector3 worldPosition, Color color, float size, Sprite sprite = null)
		{
			if (DevCam.devCamOn)
			{
				return;
			}
			if (PopText.instance)
			{
				PopText.instance.InstancePop(text, worldPosition, color, size, sprite);
			}
		}

		// Token: 0x04000C89 RID: 3209
		public static PopText instance;

		// Token: 0x04000C8A RID: 3210
		public PopTextEntity popTextPrefab;

		// Token: 0x04000C8B RID: 3211
		public List<PopTextEntity> inactiveEntries;

		// Token: 0x04000C8C RID: 3212
		public List<PopTextEntity> activeEntries;

		// Token: 0x04000C8D RID: 3213
		public float spawnVelocity = 5f;

		// Token: 0x04000C8E RID: 3214
		public float gravityValue = -9.8f;

		// Token: 0x04000C8F RID: 3215
		public float lifeTime = 1f;

		// Token: 0x04000C90 RID: 3216
		public AnimationCurve sizeOverLife;

		// Token: 0x04000C91 RID: 3217
		public float randomAngle = 10f;

		// Token: 0x04000C92 RID: 3218
		public Sprite debugSprite;

		// Token: 0x04000C93 RID: 3219
		private List<PopTextEntity> recycleList = new List<PopTextEntity>();
	}
}
