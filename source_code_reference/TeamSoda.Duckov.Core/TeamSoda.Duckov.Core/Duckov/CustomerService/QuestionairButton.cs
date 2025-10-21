using System;
using Duckov.Rules;
using Saves;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.CustomerService
{
	// Token: 0x020003F2 RID: 1010
	public class QuestionairButton : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x06002478 RID: 9336 RVA: 0x0007E770 File Offset: 0x0007C970
		public string GenerateQuestionair()
		{
			SystemLanguage currentLanguage = LocalizationManager.CurrentLanguage;
			string text;
			if (currentLanguage != SystemLanguage.Japanese)
			{
				if (currentLanguage == SystemLanguage.ChineseSimplified)
				{
					text = this.addressCN;
				}
				else
				{
					text = this.addressEN;
				}
			}
			else
			{
				text = this.addressJP;
			}
			int currentSlot = SavesSystem.CurrentSlot;
			string text2 = string.Format("{0}_{1}", PlatformInfo.Platform, PlatformInfo.GetID());
			string text3 = string.Format("{0:0}", GameClock.GetRealTimePlayedOfSaveSlot(currentSlot).TotalMinutes);
			string text4 = string.Format("{0}", EXPManager.Level);
			RuleIndex ruleIndexOfSaveSlot = GameRulesManager.GetRuleIndexOfSaveSlot(currentSlot);
			int num = 0;
			if (ruleIndexOfSaveSlot <= RuleIndex.Easy)
			{
				if (ruleIndexOfSaveSlot != RuleIndex.Standard)
				{
					if (ruleIndexOfSaveSlot != RuleIndex.Custom)
					{
						if (ruleIndexOfSaveSlot == RuleIndex.Easy)
						{
							num = 2;
						}
					}
					else
					{
						num = 0;
					}
				}
				else
				{
					num = 3;
				}
			}
			else if (ruleIndexOfSaveSlot != RuleIndex.ExtraEasy)
			{
				if (ruleIndexOfSaveSlot != RuleIndex.Hard)
				{
					if (ruleIndexOfSaveSlot == RuleIndex.ExtraHard)
					{
						num = 5;
					}
				}
				else
				{
					num = 4;
				}
			}
			else
			{
				num = 1;
			}
			string text5 = string.Format("{0}", num);
			return this.format.Format(new
			{
				address = text,
				id = text2,
				time = text3,
				level = text4,
				difficulty = text5
			});
		}

		// Token: 0x06002479 RID: 9337 RVA: 0x0007E87A File Offset: 0x0007CA7A
		public void OnPointerClick(PointerEventData eventData)
		{
			Application.OpenURL(this.GenerateQuestionair());
		}

		// Token: 0x040018DB RID: 6363
		private string addressCN = "rsmTLx1";

		// Token: 0x040018DC RID: 6364
		private string addressJP = "mHE3yAa";

		// Token: 0x040018DD RID: 6365
		private string addressEN = "YdoJpod";

		// Token: 0x040018DE RID: 6366
		private string format = "https://usersurvey.biligame.com/vm/{address}.aspx?sojumpparm={id}|{difficulty}|{time}|{level}";
	}
}
