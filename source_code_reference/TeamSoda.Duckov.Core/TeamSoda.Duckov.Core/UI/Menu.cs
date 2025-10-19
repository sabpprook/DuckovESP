using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	// Token: 0x0200020D RID: 525
	public class Menu : MonoBehaviour
	{
		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x06000F9C RID: 3996 RVA: 0x0003D69B File Offset: 0x0003B89B
		// (set) Token: 0x06000F9D RID: 3997 RVA: 0x0003D6A3 File Offset: 0x0003B8A3
		public bool Focused
		{
			get
			{
				return this.focused;
			}
			set
			{
				this.SetFocused(value);
			}
		}

		// Token: 0x1400006A RID: 106
		// (add) Token: 0x06000F9E RID: 3998 RVA: 0x0003D6AC File Offset: 0x0003B8AC
		// (remove) Token: 0x06000F9F RID: 3999 RVA: 0x0003D6E4 File Offset: 0x0003B8E4
		public event Action<Menu, MenuItem> onSelectionChanged;

		// Token: 0x1400006B RID: 107
		// (add) Token: 0x06000FA0 RID: 4000 RVA: 0x0003D71C File Offset: 0x0003B91C
		// (remove) Token: 0x06000FA1 RID: 4001 RVA: 0x0003D754 File Offset: 0x0003B954
		public event Action<Menu, MenuItem> onConfirmed;

		// Token: 0x1400006C RID: 108
		// (add) Token: 0x06000FA2 RID: 4002 RVA: 0x0003D78C File Offset: 0x0003B98C
		// (remove) Token: 0x06000FA3 RID: 4003 RVA: 0x0003D7C4 File Offset: 0x0003B9C4
		public event Action<Menu, MenuItem> onCanceled;

		// Token: 0x06000FA4 RID: 4004 RVA: 0x0003D7F9 File Offset: 0x0003B9F9
		private void SetFocused(bool value)
		{
			this.focused = value;
			if (this.focused && this.cursor == null)
			{
				this.SelectDefault();
			}
			MenuItem menuItem = this.cursor;
			if (menuItem == null)
			{
				return;
			}
			menuItem.NotifyMasterFocusStatusChanged();
		}

		// Token: 0x06000FA5 RID: 4005 RVA: 0x0003D82E File Offset: 0x0003BA2E
		public MenuItem GetSelected()
		{
			return this.cursor;
		}

		// Token: 0x06000FA6 RID: 4006 RVA: 0x0003D838 File Offset: 0x0003BA38
		public T GetSelected<T>() where T : Component
		{
			if (this.cursor == null)
			{
				return default(T);
			}
			return this.cursor.GetComponent<T>();
		}

		// Token: 0x06000FA7 RID: 4007 RVA: 0x0003D868 File Offset: 0x0003BA68
		public void Select(MenuItem toSelect)
		{
			if (toSelect.transform.parent != base.transform)
			{
				Debug.LogError("正在尝试选中不属于此菜单的项目。已取消。");
				return;
			}
			if (!this.items.Contains(toSelect))
			{
				this.items.Add(toSelect);
			}
			if (!toSelect.Selectable)
			{
				return;
			}
			if (this.cursor != null)
			{
				this.DeselectCurrent();
			}
			this.cursor = toSelect;
			this.cursor.NotifySelected();
			this.OnSelectionChanged();
		}

		// Token: 0x06000FA8 RID: 4008 RVA: 0x0003D8E8 File Offset: 0x0003BAE8
		public void SelectDefault()
		{
			MenuItem[] componentsInChildren = base.GetComponentsInChildren<MenuItem>(false);
			if (componentsInChildren == null)
			{
				return;
			}
			foreach (MenuItem menuItem in componentsInChildren)
			{
				if (!(menuItem == null) && menuItem.Selectable)
				{
					this.Select(menuItem);
				}
			}
		}

		// Token: 0x06000FA9 RID: 4009 RVA: 0x0003D92B File Offset: 0x0003BB2B
		public void Confirm()
		{
			if (this.cursor != null)
			{
				this.cursor.NotifyConfirmed();
			}
			Action<Menu, MenuItem> action = this.onConfirmed;
			if (action == null)
			{
				return;
			}
			action(this, this.cursor);
		}

		// Token: 0x06000FAA RID: 4010 RVA: 0x0003D95D File Offset: 0x0003BB5D
		public void Cancel()
		{
			if (this.cursor != null)
			{
				this.cursor.NotifyCanceled();
			}
			Action<Menu, MenuItem> action = this.onCanceled;
			if (action == null)
			{
				return;
			}
			action(this, this.cursor);
		}

		// Token: 0x06000FAB RID: 4011 RVA: 0x0003D98F File Offset: 0x0003BB8F
		private void DeselectCurrent()
		{
			this.cursor.NotifyDeselected();
		}

		// Token: 0x06000FAC RID: 4012 RVA: 0x0003D99C File Offset: 0x0003BB9C
		private void OnSelectionChanged()
		{
			Action<Menu, MenuItem> action = this.onSelectionChanged;
			if (action == null)
			{
				return;
			}
			action(this, this.cursor);
		}

		// Token: 0x06000FAD RID: 4013 RVA: 0x0003D9B8 File Offset: 0x0003BBB8
		public void Navigate(Vector2 direction)
		{
			if (this.cursor == null)
			{
				this.SelectDefault();
			}
			if (this.cursor == null)
			{
				return;
			}
			if (Mathf.Approximately(direction.sqrMagnitude, 0f))
			{
				return;
			}
			MenuItem menuItem = this.FindClosestEntryInDirection(this.cursor, direction);
			if (menuItem == null)
			{
				return;
			}
			this.Select(menuItem);
		}

		// Token: 0x06000FAE RID: 4014 RVA: 0x0003DA1C File Offset: 0x0003BC1C
		private MenuItem FindClosestEntryInDirection(MenuItem cursor, Vector2 direction)
		{
			if (cursor == null)
			{
				return null;
			}
			direction = direction.normalized;
			float num = Mathf.Cos(0.7853982f);
			Menu.<>c__DisplayClass26_0 CS$<>8__locals1;
			CS$<>8__locals1.bestMatch = null;
			CS$<>8__locals1.bestSqrDist = float.MaxValue;
			CS$<>8__locals1.bestDot = num;
			foreach (MenuItem menuItem in this.items)
			{
				Menu.<>c__DisplayClass26_1 CS$<>8__locals2;
				CS$<>8__locals2.cur = menuItem;
				if (!(CS$<>8__locals2.cur == null) && !(CS$<>8__locals2.cur == cursor) && CS$<>8__locals2.cur.Selectable)
				{
					Vector3 vector = CS$<>8__locals2.cur.transform.localPosition - cursor.transform.localPosition;
					Vector3 normalized = vector.normalized;
					Menu.<>c__DisplayClass26_2 CS$<>8__locals3;
					CS$<>8__locals3.dot = Vector3.Dot(normalized, direction);
					if (CS$<>8__locals3.dot >= num)
					{
						CS$<>8__locals3.sqrDist = vector.magnitude;
						if (CS$<>8__locals3.sqrDist <= CS$<>8__locals1.bestSqrDist)
						{
							if (CS$<>8__locals3.sqrDist < CS$<>8__locals1.bestSqrDist)
							{
								Menu.<FindClosestEntryInDirection>g__SetBestAsCur|26_0(ref CS$<>8__locals1, ref CS$<>8__locals2, ref CS$<>8__locals3);
							}
							else if (CS$<>8__locals3.sqrDist == CS$<>8__locals1.bestSqrDist && CS$<>8__locals3.dot > CS$<>8__locals1.bestDot)
							{
								Menu.<FindClosestEntryInDirection>g__SetBestAsCur|26_0(ref CS$<>8__locals1, ref CS$<>8__locals2, ref CS$<>8__locals3);
							}
						}
					}
				}
			}
			return CS$<>8__locals1.bestMatch;
		}

		// Token: 0x06000FAF RID: 4015 RVA: 0x0003DB94 File Offset: 0x0003BD94
		internal void Register(MenuItem menuItem)
		{
			this.items.Add(menuItem);
		}

		// Token: 0x06000FB0 RID: 4016 RVA: 0x0003DBA3 File Offset: 0x0003BDA3
		internal void Unegister(MenuItem menuItem)
		{
			this.items.Remove(menuItem);
		}

		// Token: 0x06000FB2 RID: 4018 RVA: 0x0003DBC5 File Offset: 0x0003BDC5
		[CompilerGenerated]
		internal static void <FindClosestEntryInDirection>g__SetBestAsCur|26_0(ref Menu.<>c__DisplayClass26_0 A_0, ref Menu.<>c__DisplayClass26_1 A_1, ref Menu.<>c__DisplayClass26_2 A_2)
		{
			A_0.bestMatch = A_1.cur;
			A_0.bestSqrDist = A_2.sqrDist;
			A_0.bestDot = A_2.dot;
		}

		// Token: 0x04000C9C RID: 3228
		[SerializeField]
		private bool focused;

		// Token: 0x04000C9D RID: 3229
		[SerializeField]
		private MenuItem cursor;

		// Token: 0x04000C9E RID: 3230
		[SerializeField]
		private LayoutGroup layout;

		// Token: 0x04000CA2 RID: 3234
		private HashSet<MenuItem> items = new HashSet<MenuItem>();
	}
}
