using System;

namespace ItemStatsSystem
{
	// Token: 0x02000019 RID: 25
	[MenuPath("General/Update")]
	public class UpdateTrigger : EffectTrigger
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x00003FA8 File Offset: 0x000021A8
		public override string DisplayName
		{
			get
			{
				return "Update";
			}
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00003FAF File Offset: 0x000021AF
		private void Update()
		{
			base.Trigger(true);
		}
	}
}
