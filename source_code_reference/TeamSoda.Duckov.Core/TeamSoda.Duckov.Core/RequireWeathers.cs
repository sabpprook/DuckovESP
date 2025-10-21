using System;
using System.Collections.Generic;
using Duckov.Quests;
using Duckov.Weathers;

// Token: 0x0200011E RID: 286
public class RequireWeathers : Condition
{
	// Token: 0x0600097A RID: 2426 RVA: 0x00029548 File Offset: 0x00027748
	public override bool Evaluate()
	{
		if (!LevelManager.LevelInited)
		{
			return false;
		}
		Weather currentWeather = LevelManager.Instance.TimeOfDayController.CurrentWeather;
		return this.weathers.Contains(currentWeather);
	}

	// Token: 0x04000860 RID: 2144
	public List<Weather> weathers;
}
