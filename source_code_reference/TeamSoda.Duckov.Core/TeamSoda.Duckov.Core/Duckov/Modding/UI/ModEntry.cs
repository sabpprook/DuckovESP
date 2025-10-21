using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.Modding.UI
{
	// Token: 0x0200026B RID: 619
	public class ModEntry : MonoBehaviour
	{
		// Token: 0x06001351 RID: 4945 RVA: 0x00047FD4 File Offset: 0x000461D4
		private void Awake()
		{
			this.toggleButton.onClick.AddListener(new UnityAction(this.OnToggleButtonClicked));
			this.uploadButton.onClick.AddListener(new UnityAction(this.OnUploadButtonClicked));
			ModManager.OnModLoadingFailed = (Action<string, string>)Delegate.Combine(ModManager.OnModLoadingFailed, new Action<string, string>(this.OnModLoadingFailed));
			this.failedIndicator.SetActive(false);
		}

		// Token: 0x06001352 RID: 4946 RVA: 0x00048045 File Offset: 0x00046245
		private void OnDestroy()
		{
			ModManager.OnModLoadingFailed = (Action<string, string>)Delegate.Remove(ModManager.OnModLoadingFailed, new Action<string, string>(this.OnModLoadingFailed));
		}

		// Token: 0x06001353 RID: 4947 RVA: 0x00048067 File Offset: 0x00046267
		private void OnModLoadingFailed(string dllPath, string message)
		{
			if (dllPath != this.info.dllPath)
			{
				return;
			}
			Debug.LogError(message);
			this.failedIndicator.SetActive(true);
		}

		// Token: 0x06001354 RID: 4948 RVA: 0x0004808F File Offset: 0x0004628F
		private void OnUploadButtonClicked()
		{
			if (this.master == null)
			{
				return;
			}
			this.master.BeginUpload(this.info).Forget();
		}

		// Token: 0x06001355 RID: 4949 RVA: 0x000480B8 File Offset: 0x000462B8
		private void OnToggleButtonClicked()
		{
			if (ModManager.Instance == null)
			{
				Debug.LogError("ModManager.Instance Not Found");
				return;
			}
			ModBehaviour modBehaviour;
			bool flag = ModManager.IsModActive(this.info, out modBehaviour);
			bool flag2 = flag && modBehaviour.info.path.Trim() == this.info.path.Trim();
			if (flag && flag2)
			{
				ModManager.Instance.DeactivateMod(this.info);
				return;
			}
			ModManager.Instance.ActivateMod(this.info);
		}

		// Token: 0x06001356 RID: 4950 RVA: 0x0004813C File Offset: 0x0004633C
		private void OnEnable()
		{
			ModManager.OnModStatusChanged += this.OnModStatusChanged;
		}

		// Token: 0x06001357 RID: 4951 RVA: 0x0004814F File Offset: 0x0004634F
		private void OnDisable()
		{
			ModManager.OnModStatusChanged -= this.OnModStatusChanged;
		}

		// Token: 0x06001358 RID: 4952 RVA: 0x00048162 File Offset: 0x00046362
		private void OnModStatusChanged()
		{
			this.RefreshStatus();
		}

		// Token: 0x06001359 RID: 4953 RVA: 0x0004816C File Offset: 0x0004636C
		private void RefreshStatus()
		{
			ModBehaviour modBehaviour;
			bool flag = ModManager.IsModActive(this.info, out modBehaviour);
			bool flag2 = flag && modBehaviour.info.path.Trim() == this.info.path.Trim();
			bool flag3 = flag && !flag2;
			this.activeIndicator.SetActive(flag2);
			this.nameCollisionIndicator.SetActive(flag3);
		}

		// Token: 0x0600135A RID: 4954 RVA: 0x000481D4 File Offset: 0x000463D4
		private void RefreshInfo()
		{
			this.textTitle.text = this.info.displayName;
			this.textName.text = this.info.name;
			this.textDescription.text = this.info.description;
			this.preview.texture = this.info.preview;
			this.steamItemIndicator.SetActive(this.info.isSteamItem);
			this.notSteamItemIndicator.SetActive(!this.info.isSteamItem);
			bool flag = SteamWorkshopManager.IsOwner(this.info);
			this.steamItemOwnerIndicator.SetActive(flag);
			bool flag2 = flag || !this.info.isSteamItem;
			this.uploadButton.gameObject.SetActive(flag2);
		}

		// Token: 0x0600135B RID: 4955 RVA: 0x000482A6 File Offset: 0x000464A6
		public void Setup(ModManagerUI master, ModInfo modInfo)
		{
			this.master = master;
			this.info = modInfo;
			this.RefreshInfo();
			this.RefreshStatus();
		}

		// Token: 0x04000E5E RID: 3678
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x04000E5F RID: 3679
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04000E60 RID: 3680
		[SerializeField]
		private TextMeshProUGUI textDescription;

		// Token: 0x04000E61 RID: 3681
		[SerializeField]
		private RawImage preview;

		// Token: 0x04000E62 RID: 3682
		[SerializeField]
		private GameObject activeIndicator;

		// Token: 0x04000E63 RID: 3683
		[SerializeField]
		private GameObject nameCollisionIndicator;

		// Token: 0x04000E64 RID: 3684
		[SerializeField]
		private Button toggleButton;

		// Token: 0x04000E65 RID: 3685
		[SerializeField]
		private GameObject steamItemIndicator;

		// Token: 0x04000E66 RID: 3686
		[SerializeField]
		private GameObject steamItemOwnerIndicator;

		// Token: 0x04000E67 RID: 3687
		[SerializeField]
		private GameObject notSteamItemIndicator;

		// Token: 0x04000E68 RID: 3688
		[SerializeField]
		private Button uploadButton;

		// Token: 0x04000E69 RID: 3689
		[SerializeField]
		private GameObject failedIndicator;

		// Token: 0x04000E6A RID: 3690
		private ModManagerUI master;

		// Token: 0x04000E6B RID: 3691
		private ModInfo info;
	}
}
