using System;
using System.Collections.Generic;
using System.Linq;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Duckov.Options.UI
{
	// Token: 0x0200025C RID: 604
	public class OptionsUIEntry_Dropdown : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
	{
		// Token: 0x17000367 RID: 871
		// (get) Token: 0x060012BE RID: 4798 RVA: 0x000467CC File Offset: 0x000449CC
		private string optionKey
		{
			get
			{
				if (this.provider == null)
				{
					return "";
				}
				return this.provider.Key;
			}
		}

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x060012BF RID: 4799 RVA: 0x000467ED File Offset: 0x000449ED
		// (set) Token: 0x060012C0 RID: 4800 RVA: 0x000467FF File Offset: 0x000449FF
		[LocalizationKey("Options")]
		public string LabelKey
		{
			get
			{
				return "Options_" + this.optionKey;
			}
			set
			{
			}
		}

		// Token: 0x060012C1 RID: 4801 RVA: 0x00046801 File Offset: 0x00044A01
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.SetupDropdown();
		}

		// Token: 0x060012C2 RID: 4802 RVA: 0x0004680C File Offset: 0x00044A0C
		private void SetupDropdown()
		{
			if (!this.provider)
			{
				return;
			}
			List<string> list = this.provider.GetOptions().ToList<string>();
			string currentOption = this.provider.GetCurrentOption();
			int num = list.IndexOf(currentOption);
			if (num < 0)
			{
				list.Insert(0, currentOption);
				num = 0;
			}
			this.dropdown.ClearOptions();
			this.dropdown.AddOptions(list.ToList<string>());
			this.dropdown.SetValueWithoutNotify(num);
		}

		// Token: 0x060012C3 RID: 4803 RVA: 0x00046884 File Offset: 0x00044A84
		private void Awake()
		{
			LocalizationManager.OnSetLanguage += this.OnSetLanguage;
			this.dropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnDropdownValueChanged));
			this.label.text = this.LabelKey.ToPlainText();
		}

		// Token: 0x060012C4 RID: 4804 RVA: 0x000468D4 File Offset: 0x00044AD4
		private void Start()
		{
			this.SetupDropdown();
		}

		// Token: 0x060012C5 RID: 4805 RVA: 0x000468DC File Offset: 0x00044ADC
		private void OnDestroy()
		{
			LocalizationManager.OnSetLanguage -= this.OnSetLanguage;
		}

		// Token: 0x060012C6 RID: 4806 RVA: 0x000468EF File Offset: 0x00044AEF
		private void OnSetLanguage(SystemLanguage language)
		{
			this.SetupDropdown();
			this.label.text = this.LabelKey.ToPlainText();
		}

		// Token: 0x060012C7 RID: 4807 RVA: 0x00046910 File Offset: 0x00044B10
		private void OnDropdownValueChanged(int index)
		{
			if (!this.provider)
			{
				return;
			}
			int num = this.provider.GetOptions().ToList<string>().IndexOf(this.dropdown.options[index].text);
			if (num >= 0)
			{
				this.provider.Set(num);
			}
			this.SetupDropdown();
		}

		// Token: 0x060012C8 RID: 4808 RVA: 0x0004696D File Offset: 0x00044B6D
		private void OnValidate()
		{
			if (this.label)
			{
				this.label.text = this.LabelKey.ToPlainText();
			}
		}

		// Token: 0x04000E20 RID: 3616
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x04000E21 RID: 3617
		[SerializeField]
		private OptionsProviderBase provider;

		// Token: 0x04000E22 RID: 3618
		[SerializeField]
		private TMP_Dropdown dropdown;
	}
}
