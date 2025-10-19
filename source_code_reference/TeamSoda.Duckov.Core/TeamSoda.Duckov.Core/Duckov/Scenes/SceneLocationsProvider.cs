using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Duckov.Scenes
{
	// Token: 0x0200032D RID: 813
	[ExecuteAlways]
	public class SceneLocationsProvider : MonoBehaviour
	{
		// Token: 0x17000516 RID: 1302
		// (get) Token: 0x06001B86 RID: 7046 RVA: 0x00063DCD File Offset: 0x00061FCD
		public static ReadOnlyCollection<SceneLocationsProvider> ActiveProviders
		{
			get
			{
				if (SceneLocationsProvider._activeProviders_ReadOnly == null)
				{
					SceneLocationsProvider._activeProviders_ReadOnly = new ReadOnlyCollection<SceneLocationsProvider>(SceneLocationsProvider.activeProviders);
				}
				return SceneLocationsProvider._activeProviders_ReadOnly;
			}
		}

		// Token: 0x06001B87 RID: 7047 RVA: 0x00063DEC File Offset: 0x00061FEC
		public static SceneLocationsProvider GetProviderOfScene(SceneReference sceneReference)
		{
			if (sceneReference == null)
			{
				return null;
			}
			return SceneLocationsProvider.ActiveProviders.FirstOrDefault((SceneLocationsProvider e) => e != null && e.gameObject.scene.buildIndex == sceneReference.BuildIndex);
		}

		// Token: 0x06001B88 RID: 7048 RVA: 0x00063E28 File Offset: 0x00062028
		public static SceneLocationsProvider GetProviderOfScene(Scene scene)
		{
			return SceneLocationsProvider.ActiveProviders.FirstOrDefault((SceneLocationsProvider e) => e != null && e.gameObject.scene.buildIndex == scene.buildIndex);
		}

		// Token: 0x06001B89 RID: 7049 RVA: 0x00063E58 File Offset: 0x00062058
		internal static SceneLocationsProvider GetProviderOfScene(int sceneBuildIndex)
		{
			return SceneLocationsProvider.ActiveProviders.FirstOrDefault((SceneLocationsProvider e) => e != null && e.gameObject.scene.buildIndex == sceneBuildIndex);
		}

		// Token: 0x06001B8A RID: 7050 RVA: 0x00063E88 File Offset: 0x00062088
		public static Transform GetLocation(SceneReference scene, string name)
		{
			if (scene.UnsafeReason != SceneReferenceUnsafeReason.None)
			{
				return null;
			}
			return SceneLocationsProvider.GetLocation(scene.BuildIndex, name);
		}

		// Token: 0x06001B8B RID: 7051 RVA: 0x00063EA0 File Offset: 0x000620A0
		public static Transform GetLocation(int sceneBuildIndex, string name)
		{
			SceneLocationsProvider providerOfScene = SceneLocationsProvider.GetProviderOfScene(sceneBuildIndex);
			if (providerOfScene == null)
			{
				return null;
			}
			return providerOfScene.GetLocation(name);
		}

		// Token: 0x06001B8C RID: 7052 RVA: 0x00063EC8 File Offset: 0x000620C8
		public static Transform GetLocation(string sceneID, string name)
		{
			SceneInfoEntry sceneInfo = SceneInfoCollection.GetSceneInfo(sceneID);
			if (sceneInfo == null)
			{
				return null;
			}
			return SceneLocationsProvider.GetLocation(sceneInfo.BuildIndex, name);
		}

		// Token: 0x06001B8D RID: 7053 RVA: 0x00063EED File Offset: 0x000620ED
		private void Awake()
		{
			SceneLocationsProvider.activeProviders.Add(this);
		}

		// Token: 0x06001B8E RID: 7054 RVA: 0x00063EFA File Offset: 0x000620FA
		private void OnDestroy()
		{
			SceneLocationsProvider.activeProviders.Remove(this);
		}

		// Token: 0x06001B8F RID: 7055 RVA: 0x00063F08 File Offset: 0x00062108
		public Transform GetLocation(string path)
		{
			string[] array = path.Split('/', StringSplitOptions.None);
			Transform transform = base.transform;
			foreach (string text in array)
			{
				if (!string.IsNullOrEmpty(text))
				{
					transform = transform.Find(text);
					if (transform == null)
					{
						return null;
					}
				}
			}
			return transform;
		}

		// Token: 0x06001B90 RID: 7056 RVA: 0x00063F54 File Offset: 0x00062154
		public bool TryGetPath(Transform value, out string path)
		{
			path = "";
			Transform transform = value;
			List<Transform> list = new List<Transform>();
			while (transform != null && transform != base.transform)
			{
				list.Insert(0, transform);
				transform = transform.parent;
			}
			if (transform != base.transform)
			{
				return false;
			}
			this.sb.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				if (i > 0)
				{
					this.sb.Append('/');
				}
				this.sb.Append(list[i].name);
			}
			path = this.sb.ToString();
			return true;
		}

		// Token: 0x06001B91 RID: 7057 RVA: 0x00064000 File Offset: 0x00062200
		[return: TupleElementNames(new string[] { "path", "worldPosition", "gameObject" })]
		public List<ValueTuple<string, Vector3, GameObject>> GetAllPathsAndItsPosition()
		{
			List<ValueTuple<string, Vector3, GameObject>> list = new List<ValueTuple<string, Vector3, GameObject>>();
			Stack<Transform> stack = new Stack<Transform>();
			stack.Push(base.transform);
			while (stack.Count > 0)
			{
				Transform transform = stack.Pop();
				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform child = transform.GetChild(i);
					string text;
					if (this.TryGetPath(child, out text))
					{
						list.Add(new ValueTuple<string, Vector3, GameObject>(text, child.transform.position, child.gameObject));
						stack.Push(child);
					}
				}
			}
			return list;
		}

		// Token: 0x06001B92 RID: 7058 RVA: 0x00064090 File Offset: 0x00062290
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			foreach (Transform transform in base.transform.GetComponentsInChildren<Transform>())
			{
				if (transform.childCount == 0)
				{
					Gizmos.DrawSphere(transform.position, 1.5f);
				}
			}
		}

		// Token: 0x04001382 RID: 4994
		private static List<SceneLocationsProvider> activeProviders = new List<SceneLocationsProvider>();

		// Token: 0x04001383 RID: 4995
		private static ReadOnlyCollection<SceneLocationsProvider> _activeProviders_ReadOnly;

		// Token: 0x04001384 RID: 4996
		private StringBuilder sb = new StringBuilder();
	}
}
