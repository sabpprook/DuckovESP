using System;
using ItemStatsSystem;

// Token: 0x02000088 RID: 136
[MenuPath("弱属性")]
public class ElementFactorFilter : EffectFilter
{
	// Token: 0x17000103 RID: 259
	// (get) Token: 0x060004D5 RID: 1237 RVA: 0x00015EBC File Offset: 0x000140BC
	public override string DisplayName
	{
		get
		{
			return string.Format("如果{0}系数{1}{2}", this.element, (this.type == ElementFactorFilter.ElementFactorFilterTypes.GreaterThan) ? "大于" : "小于", this.compareTo);
		}
	}

	// Token: 0x17000104 RID: 260
	// (get) Token: 0x060004D6 RID: 1238 RVA: 0x00015EF2 File Offset: 0x000140F2
	private CharacterMainControl MainControl
	{
		get
		{
			if (this._mainControl == null)
			{
				Effect master = base.Master;
				CharacterMainControl characterMainControl;
				if (master == null)
				{
					characterMainControl = null;
				}
				else
				{
					Item item = master.Item;
					characterMainControl = ((item != null) ? item.GetCharacterMainControl() : null);
				}
				this._mainControl = characterMainControl;
			}
			return this._mainControl;
		}
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x00015F2C File Offset: 0x0001412C
	protected override bool OnEvaluate(EffectTriggerEventContext context)
	{
		if (!this.MainControl)
		{
			return false;
		}
		if (!this.MainControl.Health)
		{
			return false;
		}
		float num = this.MainControl.Health.ElementFactor(this.element);
		if (this.type != ElementFactorFilter.ElementFactorFilterTypes.GreaterThan)
		{
			return num < this.compareTo;
		}
		return num > this.compareTo;
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x00015F8E File Offset: 0x0001418E
	private void OnDestroy()
	{
	}

	// Token: 0x04000410 RID: 1040
	public ElementFactorFilter.ElementFactorFilterTypes type;

	// Token: 0x04000411 RID: 1041
	public float compareTo = 1f;

	// Token: 0x04000412 RID: 1042
	public ElementTypes element;

	// Token: 0x04000413 RID: 1043
	private CharacterMainControl _mainControl;

	// Token: 0x0200043C RID: 1084
	public enum ElementFactorFilterTypes
	{
		// Token: 0x04001A60 RID: 6752
		GreaterThan,
		// Token: 0x04001A61 RID: 6753
		LessThan
	}
}
