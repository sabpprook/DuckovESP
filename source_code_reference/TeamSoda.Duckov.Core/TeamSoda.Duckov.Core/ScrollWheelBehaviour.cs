using System;
using Duckov.Options;
using SodaCraft.Localizations;

// Token: 0x020001C5 RID: 453
public static class ScrollWheelBehaviour
{
	// Token: 0x06000D78 RID: 3448 RVA: 0x000377EA File Offset: 0x000359EA
	public static string GetDisplayName(ScrollWheelBehaviour.Behaviour behaviour)
	{
		return string.Format("ScrollWheelBehaviour_{0}", behaviour).ToPlainText();
	}

	// Token: 0x1700027E RID: 638
	// (get) Token: 0x06000D79 RID: 3449 RVA: 0x00037801 File Offset: 0x00035A01
	// (set) Token: 0x06000D7A RID: 3450 RVA: 0x0003780E File Offset: 0x00035A0E
	public static ScrollWheelBehaviour.Behaviour CurrentBehaviour
	{
		get
		{
			return OptionsManager.Load<ScrollWheelBehaviour.Behaviour>("ScrollWheelBehaviour", ScrollWheelBehaviour.Behaviour.AmmoAndInteract);
		}
		set
		{
			OptionsManager.Save<ScrollWheelBehaviour.Behaviour>("ScrollWheelBehaviour", value);
		}
	}

	// Token: 0x020004D2 RID: 1234
	public enum Behaviour
	{
		// Token: 0x04001CE5 RID: 7397
		AmmoAndInteract,
		// Token: 0x04001CE6 RID: 7398
		Weapon
	}
}
