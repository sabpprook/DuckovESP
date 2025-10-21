using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// Token: 0x020001B6 RID: 438
public class FXPool : MonoBehaviour
{
	// Token: 0x17000259 RID: 601
	// (get) Token: 0x06000CF1 RID: 3313 RVA: 0x00036194 File Offset: 0x00034394
	// (set) Token: 0x06000CF2 RID: 3314 RVA: 0x0003619B File Offset: 0x0003439B
	public static FXPool Instance { get; private set; }

	// Token: 0x06000CF3 RID: 3315 RVA: 0x000361A3 File Offset: 0x000343A3
	private void Awake()
	{
		FXPool.Instance = this;
	}

	// Token: 0x06000CF4 RID: 3316 RVA: 0x000361AC File Offset: 0x000343AC
	private void FixedUpdate()
	{
		if (this.poolsDic == null)
		{
			return;
		}
		foreach (FXPool.Pool pool in this.poolsDic.Values)
		{
			pool.Tick();
		}
	}

	// Token: 0x06000CF5 RID: 3317 RVA: 0x0003620C File Offset: 0x0003440C
	private FXPool.Pool GetOrCreatePool(ParticleSystem prefab)
	{
		if (this.poolsDic == null)
		{
			this.poolsDic = new Dictionary<ParticleSystem, FXPool.Pool>();
		}
		FXPool.Pool pool;
		if (this.poolsDic.TryGetValue(prefab, out pool))
		{
			return pool;
		}
		FXPool.Pool pool2 = new FXPool.Pool(prefab, base.transform, null, null, null, null, true, 10, 100);
		this.poolsDic[prefab] = pool2;
		return pool2;
	}

	// Token: 0x06000CF6 RID: 3318 RVA: 0x00036262 File Offset: 0x00034462
	private static ParticleSystem Get(ParticleSystem prefab)
	{
		if (FXPool.Instance == null)
		{
			return null;
		}
		return FXPool.Instance.GetOrCreatePool(prefab).Get();
	}

	// Token: 0x06000CF7 RID: 3319 RVA: 0x00036284 File Offset: 0x00034484
	public static ParticleSystem Play(ParticleSystem prefab, Vector3 postion, Quaternion rotation)
	{
		if (FXPool.Instance == null)
		{
			return null;
		}
		if (prefab == null)
		{
			return null;
		}
		ParticleSystem particleSystem = FXPool.Get(prefab);
		particleSystem.transform.position = postion;
		particleSystem.transform.rotation = rotation;
		particleSystem.gameObject.SetActive(true);
		particleSystem.Play();
		return particleSystem;
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x000362DC File Offset: 0x000344DC
	public static ParticleSystem Play(ParticleSystem prefab, Vector3 postion, Quaternion rotation, Color color)
	{
		if (FXPool.Instance == null)
		{
			return null;
		}
		if (prefab == null)
		{
			return null;
		}
		ParticleSystem particleSystem = FXPool.Get(prefab);
		particleSystem.transform.position = postion;
		particleSystem.transform.rotation = rotation;
		particleSystem.gameObject.SetActive(true);
		particleSystem.main.startColor = color;
		particleSystem.Play();
		return particleSystem;
	}

	// Token: 0x04000B37 RID: 2871
	private Dictionary<ParticleSystem, FXPool.Pool> poolsDic;

	// Token: 0x020004C9 RID: 1225
	private class Pool
	{
		// Token: 0x060026DC RID: 9948 RVA: 0x0008CC74 File Offset: 0x0008AE74
		public Pool(ParticleSystem prefab, Transform parent, Action<ParticleSystem> onCreate = null, Action<ParticleSystem> onGet = null, Action<ParticleSystem> onRelease = null, Action<ParticleSystem> onDestroy = null, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 100)
		{
			this.prefab = prefab;
			this.parent = parent;
			this.pool = new ObjectPool<ParticleSystem>(new Func<ParticleSystem>(this.Create), new Action<ParticleSystem>(this.OnEntryGet), new Action<ParticleSystem>(this.OnEntryRelease), new Action<ParticleSystem>(this.OnEntryDestroy), collectionCheck, defaultCapacity, maxSize);
			this.onCreate = onCreate;
			this.onGet = onGet;
			this.onRelease = onRelease;
			this.onDestroy = onDestroy;
		}

		// Token: 0x060026DD RID: 9949 RVA: 0x0008CD00 File Offset: 0x0008AF00
		private ParticleSystem Create()
		{
			ParticleSystem particleSystem = global::UnityEngine.Object.Instantiate<ParticleSystem>(this.prefab, this.parent);
			Action<ParticleSystem> action = this.onCreate;
			if (action != null)
			{
				action(particleSystem);
			}
			return particleSystem;
		}

		// Token: 0x060026DE RID: 9950 RVA: 0x0008CD32 File Offset: 0x0008AF32
		public void OnEntryGet(ParticleSystem obj)
		{
			this.activeEntries.Add(obj);
		}

		// Token: 0x060026DF RID: 9951 RVA: 0x0008CD40 File Offset: 0x0008AF40
		public void OnEntryRelease(ParticleSystem obj)
		{
			this.activeEntries.Remove(obj);
			obj.gameObject.SetActive(false);
		}

		// Token: 0x060026E0 RID: 9952 RVA: 0x0008CD5B File Offset: 0x0008AF5B
		public void OnEntryDestroy(ParticleSystem obj)
		{
			Action<ParticleSystem> action = this.onDestroy;
			if (action == null)
			{
				return;
			}
			action(obj);
		}

		// Token: 0x060026E1 RID: 9953 RVA: 0x0008CD6E File Offset: 0x0008AF6E
		public ParticleSystem Get()
		{
			return this.pool.Get();
		}

		// Token: 0x060026E2 RID: 9954 RVA: 0x0008CD7B File Offset: 0x0008AF7B
		public void Release(ParticleSystem obj)
		{
			this.pool.Release(obj);
		}

		// Token: 0x060026E3 RID: 9955 RVA: 0x0008CD8C File Offset: 0x0008AF8C
		public void Tick()
		{
			List<ParticleSystem> list = new List<ParticleSystem>();
			foreach (ParticleSystem particleSystem in this.activeEntries)
			{
				if (!particleSystem.isPlaying)
				{
					list.Add(particleSystem);
				}
			}
			foreach (ParticleSystem particleSystem2 in list)
			{
				this.Release(particleSystem2);
			}
		}

		// Token: 0x04001CB7 RID: 7351
		private ParticleSystem prefab;

		// Token: 0x04001CB8 RID: 7352
		private Transform parent;

		// Token: 0x04001CB9 RID: 7353
		private ObjectPool<ParticleSystem> pool;

		// Token: 0x04001CBA RID: 7354
		private Action<ParticleSystem> onCreate;

		// Token: 0x04001CBB RID: 7355
		private Action<ParticleSystem> onGet;

		// Token: 0x04001CBC RID: 7356
		private Action<ParticleSystem> onRelease;

		// Token: 0x04001CBD RID: 7357
		private Action<ParticleSystem> onDestroy;

		// Token: 0x04001CBE RID: 7358
		private List<ParticleSystem> activeEntries = new List<ParticleSystem>();
	}
}
