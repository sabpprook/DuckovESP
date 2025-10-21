using System;
using System.Linq;
using Sirenix.Utilities;

// Token: 0x020001A7 RID: 423
public class InteractCrafter : InteractableBase
{
	// Token: 0x06000C7F RID: 3199 RVA: 0x000349AD File Offset: 0x00032BAD
	protected override void Awake()
	{
		base.Awake();
		this.finishWhenTimeOut = true;
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x000349BC File Offset: 0x00032BBC
	protected override void OnInteractFinished()
	{
		base.OnInteractFinished();
		CraftView.SetupAndOpenView(new Predicate<CraftingFormula>(this.FilterCraft));
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x000349D5 File Offset: 0x00032BD5
	private bool FilterCraft(CraftingFormula formula)
	{
		return this.requireTag.IsNullOrWhitespace() || formula.tags.Contains(this.requireTag);
	}

	// Token: 0x04000AD6 RID: 2774
	public string requireTag;
}
