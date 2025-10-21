using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Duckov.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Duckov.Quests
{
	// Token: 0x02000331 RID: 817
	[CreateAssetMenu(menuName = "Quest Collection")]
	public class QuestCollection : ScriptableObject, IList<Quest>, ICollection<Quest>, IEnumerable<Quest>, IEnumerable, ISelfValidator
	{
		// Token: 0x1700052C RID: 1324
		// (get) Token: 0x06001BD2 RID: 7122 RVA: 0x00064E46 File Offset: 0x00063046
		public static QuestCollection Instance
		{
			get
			{
				return GameplayDataSettings.QuestCollection;
			}
		}

		// Token: 0x1700052D RID: 1325
		public Quest this[int index]
		{
			get
			{
				return this.list[index];
			}
			set
			{
				this.list[index] = value;
			}
		}

		// Token: 0x1700052E RID: 1326
		// (get) Token: 0x06001BD5 RID: 7125 RVA: 0x00064E6A File Offset: 0x0006306A
		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		// Token: 0x1700052F RID: 1327
		// (get) Token: 0x06001BD6 RID: 7126 RVA: 0x00064E77 File Offset: 0x00063077
		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001BD7 RID: 7127 RVA: 0x00064E7A File Offset: 0x0006307A
		public void Add(Quest item)
		{
			this.list.Add(item);
		}

		// Token: 0x06001BD8 RID: 7128 RVA: 0x00064E88 File Offset: 0x00063088
		public void Clear()
		{
			this.list.Clear();
		}

		// Token: 0x06001BD9 RID: 7129 RVA: 0x00064E95 File Offset: 0x00063095
		public bool Contains(Quest item)
		{
			return this.list.Contains(item);
		}

		// Token: 0x06001BDA RID: 7130 RVA: 0x00064EA3 File Offset: 0x000630A3
		public void CopyTo(Quest[] array, int arrayIndex)
		{
			this.list.CopyTo(array, arrayIndex);
		}

		// Token: 0x06001BDB RID: 7131 RVA: 0x00064EB2 File Offset: 0x000630B2
		public IEnumerator<Quest> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		// Token: 0x06001BDC RID: 7132 RVA: 0x00064EC4 File Offset: 0x000630C4
		public int IndexOf(Quest item)
		{
			return this.list.IndexOf(item);
		}

		// Token: 0x06001BDD RID: 7133 RVA: 0x00064ED2 File Offset: 0x000630D2
		public void Insert(int index, Quest item)
		{
			this.list.Insert(index, item);
		}

		// Token: 0x06001BDE RID: 7134 RVA: 0x00064EE1 File Offset: 0x000630E1
		public bool Remove(Quest item)
		{
			return this.list.Remove(item);
		}

		// Token: 0x06001BDF RID: 7135 RVA: 0x00064EEF File Offset: 0x000630EF
		public void RemoveAt(int index)
		{
			this.list.RemoveAt(index);
		}

		// Token: 0x06001BE0 RID: 7136 RVA: 0x00064EFD File Offset: 0x000630FD
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x06001BE1 RID: 7137 RVA: 0x00064F05 File Offset: 0x00063105
		public void Collect()
		{
		}

		// Token: 0x06001BE2 RID: 7138 RVA: 0x00064F08 File Offset: 0x00063108
		public void Validate(SelfValidationResult result)
		{
			this.list.GroupBy(delegate(Quest e)
			{
				if (e == null)
				{
					return -1;
				}
				return e.ID;
			});
			if (this.list.GroupBy(delegate(Quest e)
			{
				if (e == null)
				{
					return -1;
				}
				return e.ID;
			}).Any((IGrouping<int, Quest> g) => g.Count<Quest>() > 1))
			{
				result.AddError("存在冲突的QuestID。").WithFix("自动重新分配ID", new Action(this.AutoFixID), true);
			}
		}

		// Token: 0x06001BE3 RID: 7139 RVA: 0x00064FB4 File Offset: 0x000631B4
		private void AutoFixID()
		{
			int num = this.list.Max((Quest e) => e.ID) + 1;
			foreach (IEnumerable<Quest> enumerable in from e in this.list
				group e by e.ID into g
				where g.Count<Quest>() > 1
				select g)
			{
				int num2 = 0;
				foreach (Quest quest in enumerable)
				{
					if (!(quest == null) && num2++ != 0)
					{
						quest.ID = num++;
					}
				}
			}
		}

		// Token: 0x06001BE4 RID: 7140 RVA: 0x000650C0 File Offset: 0x000632C0
		public Quest Get(int id)
		{
			return this.list.FirstOrDefault((Quest q) => q != null && q.ID == id);
		}

		// Token: 0x040013A0 RID: 5024
		[SerializeField]
		private List<Quest> list;
	}
}
