using System;
using System.Collections.Generic;
using Duckov.UI.Animations;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x020003C3 RID: 963
	public class KontextMenu : MonoBehaviour
	{
		// Token: 0x170006AA RID: 1706
		// (get) Token: 0x0600230B RID: 8971 RVA: 0x0007A9E0 File Offset: 0x00078BE0
		private Transform ContentRoot
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x170006AB RID: 1707
		// (get) Token: 0x0600230C RID: 8972 RVA: 0x0007A9E8 File Offset: 0x00078BE8
		private PrefabPool<KontextMenuEntry> EntryPool
		{
			get
			{
				if (this._entryPool == null)
				{
					this._entryPool = new PrefabPool<KontextMenuEntry>(this.entryPrefab, this.ContentRoot, null, null, null, true, 10, 10000, null);
				}
				return this._entryPool;
			}
		}

		// Token: 0x0600230D RID: 8973 RVA: 0x0007AA26 File Offset: 0x00078C26
		private void Awake()
		{
			if (KontextMenu.instance == null)
			{
				KontextMenu.instance = this;
			}
			this.rectTransform = base.transform as RectTransform;
		}

		// Token: 0x0600230E RID: 8974 RVA: 0x0007AA4C File Offset: 0x00078C4C
		private void OnDestroy()
		{
		}

		// Token: 0x0600230F RID: 8975 RVA: 0x0007AA50 File Offset: 0x00078C50
		private void Update()
		{
			if (this.watchRectTransform)
			{
				if ((this.cachedTransformPosition - this.watchRectTransform.position).magnitude > this.positionMoveCloseThreshold)
				{
					KontextMenu.Hide(null);
					return;
				}
			}
			else if (this.isWatchingRectTransform)
			{
				KontextMenu.Hide(null);
			}
		}

		// Token: 0x06002310 RID: 8976 RVA: 0x0007AAA8 File Offset: 0x00078CA8
		public void InstanceShow(object target, RectTransform targetRectTransform, params KontextMenuDataEntry[] entries)
		{
			this.target = target;
			this.watchRectTransform = targetRectTransform;
			this.isWatchingRectTransform = true;
			this.cachedTransformPosition = this.watchRectTransform.position;
			Vector3[] array = new Vector3[4];
			targetRectTransform.GetWorldCorners(array);
			float num = Mathf.Min(new float[]
			{
				array[0].x,
				array[1].x,
				array[2].x,
				array[3].x
			});
			float num2 = Mathf.Max(new float[]
			{
				array[0].x,
				array[1].x,
				array[2].x,
				array[3].x
			});
			float num3 = Mathf.Min(new float[]
			{
				array[0].y,
				array[1].y,
				array[2].y,
				array[3].y
			});
			float num4 = Mathf.Max(new float[]
			{
				array[0].y,
				array[1].y,
				array[2].y,
				array[3].y
			});
			float num5 = num;
			float num6 = (float)Screen.width - num2;
			float num7 = num3;
			float num8 = (float)Screen.height - num4;
			float num9 = ((num5 > num6) ? num : num2);
			float num10 = ((num7 > num8) ? num3 : num4);
			Vector2 vector = new Vector2(num9, num10);
			if (entries.Length < 1)
			{
				this.InstanceHide();
				return;
			}
			Vector2 vector2 = new Vector2(vector.x / (float)Screen.width, vector.y / (float)Screen.height);
			float num11 = (float)((vector2.x < 0.5f) ? 0 : 1);
			float num12 = (float)((vector2.y < 0.5f) ? 0 : 1);
			this.rectTransform.pivot = new Vector2(num11, num12);
			base.gameObject.SetActive(true);
			this.fadeGroup.SkipHide();
			this.Setup(entries);
			this.fadeGroup.Show();
			base.transform.position = vector;
		}

		// Token: 0x06002311 RID: 8977 RVA: 0x0007ACEC File Offset: 0x00078EEC
		public void InstanceShow(object target, Vector2 screenPoint, params KontextMenuDataEntry[] entries)
		{
			this.target = target;
			this.watchRectTransform = null;
			this.isWatchingRectTransform = false;
			if (entries.Length < 1)
			{
				this.InstanceHide();
				return;
			}
			Vector2 vector = new Vector2(screenPoint.x / (float)Screen.width, screenPoint.y / (float)Screen.height);
			float num = (float)((vector.x < 0.5f) ? 0 : 1);
			float num2 = (float)((vector.y < 0.5f) ? 0 : 1);
			this.rectTransform.pivot = new Vector2(num, num2);
			base.gameObject.SetActive(true);
			this.fadeGroup.SkipHide();
			this.Setup(entries);
			this.fadeGroup.Show();
			base.transform.position = screenPoint;
		}

		// Token: 0x06002312 RID: 8978 RVA: 0x0007ADAC File Offset: 0x00078FAC
		private void Clear()
		{
			this.EntryPool.ReleaseAll();
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < this.ContentRoot.childCount; i++)
			{
				Transform child = this.ContentRoot.GetChild(i);
				if (child.gameObject.activeSelf)
				{
					list.Add(child.gameObject);
				}
			}
			foreach (GameObject gameObject in list)
			{
				global::UnityEngine.Object.Destroy(gameObject);
			}
		}

		// Token: 0x06002313 RID: 8979 RVA: 0x0007AE44 File Offset: 0x00079044
		private void Setup(IEnumerable<KontextMenuDataEntry> entries)
		{
			this.Clear();
			int num = 0;
			foreach (KontextMenuDataEntry kontextMenuDataEntry in entries)
			{
				if (kontextMenuDataEntry != null)
				{
					KontextMenuEntry kontextMenuEntry = this.EntryPool.Get(this.ContentRoot);
					num++;
					kontextMenuEntry.Setup(this, num, kontextMenuDataEntry);
					kontextMenuEntry.transform.SetAsLastSibling();
				}
			}
		}

		// Token: 0x06002314 RID: 8980 RVA: 0x0007AEB8 File Offset: 0x000790B8
		public void InstanceHide()
		{
			this.target = null;
			this.watchRectTransform = null;
			this.fadeGroup.Hide();
		}

		// Token: 0x06002315 RID: 8981 RVA: 0x0007AED3 File Offset: 0x000790D3
		public static void Show(object target, RectTransform watchRectTransform, params KontextMenuDataEntry[] entries)
		{
			if (KontextMenu.instance == null)
			{
				return;
			}
			KontextMenu.instance.InstanceShow(target, watchRectTransform, entries);
		}

		// Token: 0x06002316 RID: 8982 RVA: 0x0007AEF0 File Offset: 0x000790F0
		public static void Show(object target, Vector2 position, params KontextMenuDataEntry[] entries)
		{
			if (KontextMenu.instance == null)
			{
				return;
			}
			KontextMenu.instance.InstanceShow(target, position, entries);
		}

		// Token: 0x06002317 RID: 8983 RVA: 0x0007AF0D File Offset: 0x0007910D
		public static void Hide(object target)
		{
			if (KontextMenu.instance == null)
			{
				return;
			}
			if (target != null && target != KontextMenu.instance.target)
			{
				return;
			}
			if (KontextMenu.instance.fadeGroup.IsHidingInProgress)
			{
				return;
			}
			KontextMenu.instance.InstanceHide();
		}

		// Token: 0x040017D6 RID: 6102
		private static KontextMenu instance;

		// Token: 0x040017D7 RID: 6103
		private RectTransform rectTransform;

		// Token: 0x040017D8 RID: 6104
		[SerializeField]
		private KontextMenuEntry entryPrefab;

		// Token: 0x040017D9 RID: 6105
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x040017DA RID: 6106
		[SerializeField]
		private float positionMoveCloseThreshold = 10f;

		// Token: 0x040017DB RID: 6107
		private object target;

		// Token: 0x040017DC RID: 6108
		private bool isWatchingRectTransform;

		// Token: 0x040017DD RID: 6109
		private RectTransform watchRectTransform;

		// Token: 0x040017DE RID: 6110
		private Vector3 cachedTransformPosition;

		// Token: 0x040017DF RID: 6111
		private PrefabPool<KontextMenuEntry> _entryPool;
	}
}
