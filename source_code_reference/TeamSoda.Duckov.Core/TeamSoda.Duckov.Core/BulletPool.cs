using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// Token: 0x0200010C RID: 268
public class BulletPool : MonoBehaviour
{
	// Token: 0x06000928 RID: 2344 RVA: 0x00028A6F File Offset: 0x00026C6F
	private void Awake()
	{
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x00028A71 File Offset: 0x00026C71
	public Projectile GetABullet(Projectile bulletPrefab)
	{
		return this.GetAPool(bulletPrefab).Get();
	}

	// Token: 0x0600092A RID: 2346 RVA: 0x00028A80 File Offset: 0x00026C80
	private ObjectPool<Projectile> GetAPool(Projectile pfb)
	{
		ObjectPool<Projectile> objectPool;
		if (this.pools.TryGetValue(pfb, out objectPool))
		{
			return objectPool;
		}
		ObjectPool<Projectile> objectPool2 = new ObjectPool<Projectile>(() => this.CreateABulletInPool(pfb), new Action<Projectile>(this.OnGetABulletInPool), new Action<Projectile>(this.OnBulletRelease), null, true, 10, 10000);
		this.pools.Add(pfb, objectPool2);
		return objectPool2;
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x00028B00 File Offset: 0x00026D00
	private Projectile CreateABulletInPool(Projectile pfb)
	{
		Projectile projectile = global::UnityEngine.Object.Instantiate<Projectile>(pfb);
		projectile.transform.SetParent(base.transform);
		ObjectPool<Projectile> apool = this.GetAPool(pfb);
		projectile.SetPool(apool);
		return projectile;
	}

	// Token: 0x0600092C RID: 2348 RVA: 0x00028B33 File Offset: 0x00026D33
	private void OnGetABulletInPool(Projectile bulletToGet)
	{
		bulletToGet.gameObject.SetActive(true);
	}

	// Token: 0x0600092D RID: 2349 RVA: 0x00028B41 File Offset: 0x00026D41
	private void OnBulletRelease(Projectile bulletToGet)
	{
		bulletToGet.transform.SetParent(base.transform);
		bulletToGet.gameObject.SetActive(false);
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x00028B60 File Offset: 0x00026D60
	public bool Release(Projectile instance, Projectile prefab)
	{
		ObjectPool<Projectile> objectPool;
		if (this.pools.TryGetValue(prefab, out objectPool))
		{
			objectPool.Release(prefab);
			return true;
		}
		return false;
	}

	// Token: 0x04000833 RID: 2099
	public Dictionary<Projectile, ObjectPool<Projectile>> pools = new Dictionary<Projectile, ObjectPool<Projectile>>();
}
