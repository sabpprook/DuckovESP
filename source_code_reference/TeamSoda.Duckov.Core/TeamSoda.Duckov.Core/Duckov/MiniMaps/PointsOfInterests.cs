using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Duckov.MiniMaps
{
	// Token: 0x02000271 RID: 625
	public static class PointsOfInterests
	{
		// Token: 0x17000398 RID: 920
		// (get) Token: 0x060013A0 RID: 5024 RVA: 0x00048EDD File Offset: 0x000470DD
		public static ReadOnlyCollection<MonoBehaviour> Points
		{
			get
			{
				if (PointsOfInterests.points_ReadOnly == null)
				{
					PointsOfInterests.points_ReadOnly = new ReadOnlyCollection<MonoBehaviour>(PointsOfInterests.points);
				}
				return PointsOfInterests.points_ReadOnly;
			}
		}

		// Token: 0x1400007E RID: 126
		// (add) Token: 0x060013A1 RID: 5025 RVA: 0x00048EFC File Offset: 0x000470FC
		// (remove) Token: 0x060013A2 RID: 5026 RVA: 0x00048F30 File Offset: 0x00047130
		public static event Action<MonoBehaviour> OnPointRegistered;

		// Token: 0x1400007F RID: 127
		// (add) Token: 0x060013A3 RID: 5027 RVA: 0x00048F64 File Offset: 0x00047164
		// (remove) Token: 0x060013A4 RID: 5028 RVA: 0x00048F98 File Offset: 0x00047198
		public static event Action<MonoBehaviour> OnPointUnregistered;

		// Token: 0x060013A5 RID: 5029 RVA: 0x00048FCB File Offset: 0x000471CB
		public static void Register(MonoBehaviour point)
		{
			PointsOfInterests.points.Add(point);
			Action<MonoBehaviour> onPointRegistered = PointsOfInterests.OnPointRegistered;
			if (onPointRegistered != null)
			{
				onPointRegistered(point);
			}
			PointsOfInterests.CleanUp();
		}

		// Token: 0x060013A6 RID: 5030 RVA: 0x00048FEE File Offset: 0x000471EE
		public static void Unregister(MonoBehaviour point)
		{
			if (PointsOfInterests.points.Remove(point))
			{
				Action<MonoBehaviour> onPointUnregistered = PointsOfInterests.OnPointUnregistered;
				if (onPointUnregistered != null)
				{
					onPointUnregistered(point);
				}
			}
			PointsOfInterests.CleanUp();
		}

		// Token: 0x060013A7 RID: 5031 RVA: 0x00049013 File Offset: 0x00047213
		private static void CleanUp()
		{
			PointsOfInterests.points.RemoveAll((MonoBehaviour e) => e == null);
		}

		// Token: 0x04000E89 RID: 3721
		private static List<MonoBehaviour> points = new List<MonoBehaviour>();

		// Token: 0x04000E8A RID: 3722
		private static ReadOnlyCollection<MonoBehaviour> points_ReadOnly;
	}
}
