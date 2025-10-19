using System;

namespace Duckov.UI.BarDisplays
{
	// Token: 0x020003CD RID: 973
	public class BarDisplayController_Thurst : BarDisplayController
	{
		// Token: 0x0600234C RID: 9036 RVA: 0x0007B71C File Offset: 0x0007991C
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

		// Token: 0x170006B8 RID: 1720
		// (get) Token: 0x0600234D RID: 9037 RVA: 0x0007B75D File Offset: 0x0007995D
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

		// Token: 0x170006B9 RID: 1721
		// (get) Token: 0x0600234E RID: 9038 RVA: 0x0007B77E File Offset: 0x0007997E
		protected override float Current
		{
			get
			{
				if (this.Target == null)
				{
					return base.Current;
				}
				return this.Target.CurrentWater;
			}
		}

		// Token: 0x170006BA RID: 1722
		// (get) Token: 0x0600234F RID: 9039 RVA: 0x0007B7A0 File Offset: 0x000799A0
		protected override float Max
		{
			get
			{
				if (this.Target == null)
				{
					return base.Max;
				}
				return this.Target.MaxWater;
			}
		}

		// Token: 0x04001800 RID: 6144
		private CharacterMainControl _target;

		// Token: 0x04001801 RID: 6145
		private float displayingCurrent = -1f;

		// Token: 0x04001802 RID: 6146
		private float displayingMax = -1f;
	}
}
