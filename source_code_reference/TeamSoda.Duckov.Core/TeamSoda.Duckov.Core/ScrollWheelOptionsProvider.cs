using System;

// Token: 0x020001C4 RID: 452
public class ScrollWheelOptionsProvider : OptionsProviderBase
{
	// Token: 0x1700027D RID: 637
	// (get) Token: 0x06000D73 RID: 3443 RVA: 0x0003776A File Offset: 0x0003596A
	public override string Key
	{
		get
		{
			return "Input_ScrollWheelBehaviour";
		}
	}

	// Token: 0x06000D74 RID: 3444 RVA: 0x00037771 File Offset: 0x00035971
	public override string GetCurrentOption()
	{
		return ScrollWheelBehaviour.GetDisplayName(ScrollWheelBehaviour.CurrentBehaviour);
	}

	// Token: 0x06000D75 RID: 3445 RVA: 0x00037780 File Offset: 0x00035980
	public override string[] GetOptions()
	{
		ScrollWheelBehaviour.Behaviour[] array = (ScrollWheelBehaviour.Behaviour[])Enum.GetValues(typeof(ScrollWheelBehaviour.Behaviour));
		string[] array2 = new string[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = ScrollWheelBehaviour.GetDisplayName(array[i]);
		}
		return array2;
	}

	// Token: 0x06000D76 RID: 3446 RVA: 0x000377C5 File Offset: 0x000359C5
	public override void Set(int index)
	{
		ScrollWheelBehaviour.CurrentBehaviour = ((ScrollWheelBehaviour.Behaviour[])Enum.GetValues(typeof(ScrollWheelBehaviour.Behaviour)))[index];
	}
}
