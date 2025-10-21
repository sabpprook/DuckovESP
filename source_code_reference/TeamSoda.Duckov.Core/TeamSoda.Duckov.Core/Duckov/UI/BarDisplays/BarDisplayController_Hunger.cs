using System;

namespace Duckov.UI.BarDisplays
{
	// Token: 0x020003CB RID: 971
	public class BarDisplayController_Hunger : BarDisplayController
	{
		// Token: 0x06002342 RID: 9026 RVA: 0x0007B594 File Offset: 0x00079794
		private void Update()
		{
			float num = this.Current;
			float max = this.Max;
			if (this.displayingCurrent != num || this.displayingMax != max)
			{
				base.Refresh();
				this.displayingCurrent = num;
				this.displayingMax = max;
			}
		}

		// Token: 0x170006B2 RID: 1714
		// (get) Token: 0x06002343 RID: 9027 RVA: 0x0007B5D5 File Offset: 0x000797D5
		private CharacterMainControl Target
		{
			get
			{
				if (this._target == null)
				{
					this._target = CharacterMainControl.Main;
				}
				return this._target;
			}
		}

		// Token: 0x170006B3 RID: 1715
		// (get) Token: 0x06002344 RID: 9028 RVA: 0x0007B5F6 File Offset: 0x000797F6
		protected override float Current
		{
			get
			{
				if (this.Target == null)
				{
					return base.Current;
				}
				return this.Target.CurrentEnergy;
			}
		}

		// Token: 0x170006B4 RID: 1716
		// (get) Token: 0x06002345 RID: 9029 RVA: 0x0007B618 File Offset: 0x00079818
		protected override float Max
		{
			get
			{
				if (this.Target == null)
				{
					return base.Max;
				}
				return this.Target.MaxEnergy;
			}
		}

		// Token: 0x040017FA RID: 6138
		private CharacterMainControl _target;

		// Token: 0x040017FB RID: 6139
		private float displayingCurrent = -1f;

		// Token: 0x040017FC RID: 6140
		private float displayingMax = -1f;
	}
}
