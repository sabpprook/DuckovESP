using System;

// Token: 0x0200006D RID: 109
[Serializable]
public struct ElementFactor
{
	// Token: 0x0600041E RID: 1054 RVA: 0x000124BC File Offset: 0x000106BC
	public ElementFactor(ElementTypes _type, float _factor)
	{
		this.elementType = _type;
		this.factor = _factor;
	}

	// Token: 0x0400032F RID: 815
	public ElementTypes elementType;

	// Token: 0x04000330 RID: 816
	public float factor;
}
