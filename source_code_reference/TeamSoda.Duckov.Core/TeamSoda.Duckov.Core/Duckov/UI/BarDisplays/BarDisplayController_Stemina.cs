using System;

namespace Duckov.UI.BarDisplays
{
	// Token: 0x020003CC RID: 972
	public class BarDisplayController_Stemina : BarDisplayController
	{
		// Token: 0x06002347 RID: 9031 RVA: 0x0007B658 File Offset: 0x00079858
		private void Update()
		{
			float num = this.Current;
			float max = this.Max;
			if (this.displayingStemina != num || this.displayingMaxStemina != max)
			{
				base.Refresh();
				this.displayingStemina = num;
				this.displayingMaxStemina = max;
			}
		}

		// Token: 0x170006B5 RID: 1717
		// (get) Token: 0x06002348 RID: 9032 RVA: 0x0007B699 File Offset: 0x00079899
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

		// Token: 0x170006B6 RID: 1718
		// (get) Token: 0x06002349 RID: 9033 RVA: 0x0007B6BA File Offset: 0x000798BA
		protected override float Current
		{
			get
			{
				if (this.Target == null)
				{
					return base.Current;
				}
				return this.Target.CurrentStamina;
			}
		}

		// Token: 0x170006B7 RID: 1719
		// (get) Token: 0x0600234A RID: 9034 RVA: 0x0007B6DC File Offset: 0x000798DC
		protected override float Max
		{
			get
			{
				if (this.Target == null)
				{
					return base.Max;
				}
				return this.Target.MaxStamina;
			}
		}

		// Token: 0x040017FD RID: 6141
		private CharacterMainControl _target;

		// Token: 0x040017FE RID: 6142
		private float displayingStemina = -1f;

		// Token: 0x040017FF RID: 6143
		private float displayingMaxStemina = -1f;
	}
}
