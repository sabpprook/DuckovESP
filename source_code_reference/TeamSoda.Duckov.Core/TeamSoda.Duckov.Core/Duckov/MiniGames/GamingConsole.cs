using System;
using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using ItemStatsSystem;
using ItemStatsSystem.Data;
using ItemStatsSystem.Items;
using Saves;
using SodaCraft.Localizations;
using UnityEngine;

namespace Duckov.MiniGames
{
	// Token: 0x0200027C RID: 636
	public class GamingConsole : InteractableBase
	{
		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x06001428 RID: 5160 RVA: 0x0004ACEF File Offset: 0x00048EEF
		public MiniGame SelectedGame
		{
			get
			{
				if (this.CatridgeGameID == null)
				{
					return null;
				}
				return this.possibleGames.Find((MiniGame e) => e != null && e.ID == this.CatridgeGameID);
			}
		}

		// Token: 0x170003BA RID: 954
		// (get) Token: 0x06001429 RID: 5161 RVA: 0x0004AD12 File Offset: 0x00048F12
		public MiniGame Game
		{
			get
			{
				return this.game;
			}
		}

		// Token: 0x170003BB RID: 955
		// (get) Token: 0x0600142A RID: 5162 RVA: 0x0004AD1A File Offset: 0x00048F1A
		public Slot MonitorSlot
		{
			get
			{
				return this.mainItem.Slots["Monitor"];
			}
		}

		// Token: 0x170003BC RID: 956
		// (get) Token: 0x0600142B RID: 5163 RVA: 0x0004AD31 File Offset: 0x00048F31
		public Slot ConsoleSlot
		{
			get
			{
				return this.mainItem.Slots["Console"];
			}
		}

		// Token: 0x170003BD RID: 957
		// (get) Token: 0x0600142C RID: 5164 RVA: 0x0004AD48 File Offset: 0x00048F48
		public bool controllerConnected
		{
			get
			{
				if (this.mainItem == null)
				{
					return false;
				}
				if (this.ConsoleSlot == null)
				{
					return false;
				}
				Item content = this.ConsoleSlot.Content;
				if (content == null)
				{
					return false;
				}
				Slot slot = content.Slots["FcController"];
				return slot != null && slot.Content != null;
			}
		}

		// Token: 0x14000081 RID: 129
		// (add) Token: 0x0600142D RID: 5165 RVA: 0x0004ADA8 File Offset: 0x00048FA8
		// (remove) Token: 0x0600142E RID: 5166 RVA: 0x0004ADE0 File Offset: 0x00048FE0
		public event Action<GamingConsole> onContentChanged;

		// Token: 0x14000082 RID: 130
		// (add) Token: 0x0600142F RID: 5167 RVA: 0x0004AE18 File Offset: 0x00049018
		// (remove) Token: 0x06001430 RID: 5168 RVA: 0x0004AE50 File Offset: 0x00049050
		public event Action<GamingConsole> OnAfterAnimateIn;

		// Token: 0x14000083 RID: 131
		// (add) Token: 0x06001431 RID: 5169 RVA: 0x0004AE88 File Offset: 0x00049088
		// (remove) Token: 0x06001432 RID: 5170 RVA: 0x0004AEC0 File Offset: 0x000490C0
		public event Action<GamingConsole> OnBeforeAnimateOut;

		// Token: 0x14000084 RID: 132
		// (add) Token: 0x06001433 RID: 5171 RVA: 0x0004AEF8 File Offset: 0x000490F8
		// (remove) Token: 0x06001434 RID: 5172 RVA: 0x0004AF2C File Offset: 0x0004912C
		public static event Action<bool> OnGamingConsoleInteractChanged;

		// Token: 0x170003BE RID: 958
		// (get) Token: 0x06001435 RID: 5173 RVA: 0x0004AF5F File Offset: 0x0004915F
		public Item Monitor
		{
			get
			{
				if (this.MonitorSlot == null)
				{
					return null;
				}
				return this.MonitorSlot.Content;
			}
		}

		// Token: 0x170003BF RID: 959
		// (get) Token: 0x06001436 RID: 5174 RVA: 0x0004AF76 File Offset: 0x00049176
		public Item Console
		{
			get
			{
				if (this.ConsoleSlot == null)
				{
					return null;
				}
				return this.ConsoleSlot.Content;
			}
		}

		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x06001437 RID: 5175 RVA: 0x0004AF90 File Offset: 0x00049190
		public Item Cartridge
		{
			get
			{
				if (this.Console == null)
				{
					return null;
				}
				if (!this.Console.Slots)
				{
					Debug.LogError(this.Console.DisplayName + " has no catridge slot");
					return null;
				}
				Slot slot = this.Console.Slots["Cartridge"];
				if (slot == null)
				{
					Debug.LogError(this.Console.DisplayName + " has no catridge slot");
					return null;
				}
				return slot.Content;
			}
		}

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x06001438 RID: 5176 RVA: 0x0004B016 File Offset: 0x00049216
		public string CatridgeGameID
		{
			get
			{
				if (this.Cartridge == null)
				{
					return null;
				}
				return this.Cartridge.Constants.GetString("GameID", null);
			}
		}

		// Token: 0x06001439 RID: 5177 RVA: 0x0004B040 File Offset: 0x00049240
		private async UniTask Load()
		{
			if (this.loading)
			{
				Debug.LogError("Component is loading in progress, aborting.");
			}
			else
			{
				while (!LevelManager.LevelInited)
				{
					await UniTask.Yield();
				}
				GamingConsole.SaveData data = SavesSystem.Load<GamingConsole.SaveData>(this.SaveKey);
				if (data == null)
				{
					this.loaded = true;
				}
				else
				{
					if (data.monitorData != null)
					{
						Item item = await ItemTreeData.InstantiateAsync(data.monitorData);
						if (item != null)
						{
							Item item2;
							if (!this.MonitorSlot.Plug(item, out item2))
							{
								ItemUtilities.SendToPlayer(item, false, true);
							}
							if (item2 != null)
							{
								item2.DestroyTree();
							}
						}
					}
					if (data.consoleData != null)
					{
						Item item3 = await ItemTreeData.InstantiateAsync(data.consoleData);
						if (item3 != null)
						{
							Item item4;
							if (!this.ConsoleSlot.Plug(item3, out item4))
							{
								ItemUtilities.SendToPlayer(item3, false, true);
							}
							if (item4 != null)
							{
								item4.DestroyTree();
							}
						}
					}
					this.loading = false;
					this.loaded = true;
					Action<GamingConsole> action = this.onContentChanged;
					if (action != null)
					{
						action(this);
					}
				}
			}
		}

		// Token: 0x0600143A RID: 5178 RVA: 0x0004B084 File Offset: 0x00049284
		private void Save()
		{
			if (this.loading)
			{
				return;
			}
			if (!this.loaded)
			{
				return;
			}
			GamingConsole.SaveData saveData = new GamingConsole.SaveData();
			if (this.Console != null)
			{
				saveData.consoleData = ItemTreeData.FromItem(this.Console);
			}
			if (this.Monitor != null)
			{
				saveData.monitorData = ItemTreeData.FromItem(this.Monitor);
			}
			SavesSystem.Save<GamingConsole.SaveData>(this.SaveKey, saveData);
		}

		// Token: 0x0600143B RID: 5179 RVA: 0x0004B0F4 File Offset: 0x000492F4
		protected override void Awake()
		{
			base.Awake();
			UIInputManager.OnCancel += this.OnUICancel;
			SavesSystem.OnCollectSaveData += this.Save;
			this.inputHandler.enabled = false;
			this.mainItem.onItemTreeChanged += this.OnContentChanged;
		}

		// Token: 0x0600143C RID: 5180 RVA: 0x0004B14C File Offset: 0x0004934C
		protected override void OnDestroy()
		{
			base.OnDestroy();
			Action<bool> onGamingConsoleInteractChanged = GamingConsole.OnGamingConsoleInteractChanged;
			if (onGamingConsoleInteractChanged != null)
			{
				onGamingConsoleInteractChanged(false);
			}
			UIInputManager.OnCancel -= this.OnUICancel;
			SavesSystem.OnCollectSaveData -= this.Save;
			this.isBeingDestroyed = true;
		}

		// Token: 0x0600143D RID: 5181 RVA: 0x0004B199 File Offset: 0x00049399
		private void OnDisable()
		{
			Action<bool> onGamingConsoleInteractChanged = GamingConsole.OnGamingConsoleInteractChanged;
			if (onGamingConsoleInteractChanged == null)
			{
				return;
			}
			onGamingConsoleInteractChanged(false);
		}

		// Token: 0x0600143E RID: 5182 RVA: 0x0004B1AB File Offset: 0x000493AB
		protected override void Start()
		{
			base.Start();
			this.Load().Forget();
		}

		// Token: 0x0600143F RID: 5183 RVA: 0x0004B1BE File Offset: 0x000493BE
		private void OnContentChanged(Item item)
		{
			Action<GamingConsole> action = this.onContentChanged;
			if (action != null)
			{
				action(this);
			}
			this.RefreshGame();
		}

		// Token: 0x06001440 RID: 5184 RVA: 0x0004B1D8 File Offset: 0x000493D8
		private void OnUICancel(UIInputEventData data)
		{
			if (data.Used)
			{
				return;
			}
			if (base.Interacting)
			{
				base.StopInteract();
				data.Use();
			}
		}

		// Token: 0x06001441 RID: 5185 RVA: 0x0004B1F8 File Offset: 0x000493F8
		protected override void OnInteractStart(CharacterMainControl interactCharacter)
		{
			base.OnInteractStart(interactCharacter);
			Action<bool> onGamingConsoleInteractChanged = GamingConsole.OnGamingConsoleInteractChanged;
			if (onGamingConsoleInteractChanged != null)
			{
				onGamingConsoleInteractChanged(this);
			}
			if (this.Console == null || this.Monitor == null || this.Cartridge == null)
			{
				NotificationText.Push(this.incompleteNotificationText.ToPlainText());
				base.StopInteract();
				return;
			}
			if (this.SelectedGame == null)
			{
				NotificationText.Push(this.noGameNotificationText.ToPlainText());
				base.StopInteract();
				return;
			}
			this.RefreshGame();
			this.inputHandler.enabled = this.controllerConnected;
			this.AnimateCameraIn().Forget();
			HUDManager.RegisterHideToken(this);
			CharacterMainControl.Main.SetPosition(this.teleportToPositionWhenBegin.position);
			GamingConsoleHUD.Show();
		}

		// Token: 0x06001442 RID: 5186 RVA: 0x0004B2CC File Offset: 0x000494CC
		private async UniTask AnimateCameraIn()
		{
			int token = global::UnityEngine.Random.Range(0, int.MaxValue);
			this.animateToken = token;
			Vector3 toPos = this.vcamEndPosition.position;
			Quaternion quaternion = this.vcamEndPosition.rotation;
			float toFov = this.activeFov;
			if (!(GameCamera.Instance == null) && !(GameCamera.Instance.mainVCam == null))
			{
				CinemachineVirtualCamera mainVCam = GameCamera.Instance.mainVCam;
				Vector3 fromPos = mainVCam.transform.position;
				Quaternion fromRot = mainVCam.transform.rotation;
				float fromFov = mainVCam.m_Lens.FieldOfView;
				this.virtualCamera.transform.position = fromPos;
				this.virtualCamera.transform.rotation = fromRot;
				this.virtualCamera.Priority = 10;
				float time = 0f;
				while (time < this.transitionTime)
				{
					time += Time.deltaTime;
					float num = time / this.transitionTime;
					float num2 = this.posCurve.Evaluate(num);
					float num3 = this.rotCurve.Evaluate(num);
					float num4 = this.fovCurve.Evaluate(num);
					Vector3 vector = Vector3.Lerp(fromPos, toPos, num2);
					quaternion = Quaternion.LookRotation(this.vcamLookTarget.position - vector, Vector3.up);
					Quaternion quaternion2 = Quaternion.Lerp(fromRot, quaternion, num3);
					float num5 = Mathf.Lerp(fromFov, toFov, num4);
					this.virtualCamera.transform.SetPositionAndRotation(vector, quaternion2);
					this.virtualCamera.m_Lens.FieldOfView = num5;
					await UniTask.Yield();
					if (this.animateToken != token)
					{
						return;
					}
				}
				quaternion = Quaternion.LookRotation(this.vcamLookTarget.position - toPos, Vector3.up);
			}
			this.virtualCamera.transform.SetPositionAndRotation(toPos, quaternion);
			this.virtualCamera.m_Lens.FieldOfView = toFov;
			Action<GamingConsole> onAfterAnimateIn = this.OnAfterAnimateIn;
			if (onAfterAnimateIn != null)
			{
				onAfterAnimateIn(this);
			}
		}

		// Token: 0x06001443 RID: 5187 RVA: 0x0004B310 File Offset: 0x00049510
		private async UniTask AnimateCameraOut()
		{
			Action<GamingConsole> onBeforeAnimateOut = this.OnBeforeAnimateOut;
			if (onBeforeAnimateOut != null)
			{
				onBeforeAnimateOut(this);
			}
			int token = global::UnityEngine.Random.Range(0, int.MaxValue);
			this.animateToken = token;
			GameCamera instance = GameCamera.Instance;
			if (!(instance == null))
			{
				CinemachineVirtualCamera mainVCam = instance.mainVCam;
				if (!(mainVCam == null))
				{
					Vector3 fromPos = this.virtualCamera.transform.position;
					float fromFov = this.activeFov;
					float time = 0f;
					while (time < this.transitionTime)
					{
						if (mainVCam == null)
						{
							return;
						}
						time += Time.deltaTime;
						float num = 1f - time / this.transitionTime;
						float num2 = 1f - this.posCurve.Evaluate(num);
						float num3 = 1f - this.rotCurve.Evaluate(num);
						float num4 = 1f - this.fovCurve.Evaluate(num);
						Vector3 position = mainVCam.transform.position;
						Quaternion rotation = mainVCam.transform.rotation;
						float fieldOfView = mainVCam.m_Lens.FieldOfView;
						Vector3 vector = Vector3.Lerp(fromPos, position, num2);
						Quaternion quaternion = Quaternion.Lerp(Quaternion.LookRotation(this.vcamLookTarget.position - vector, Vector3.up), rotation, num3);
						float num5 = Mathf.Lerp(fromFov, fieldOfView, num4);
						this.virtualCamera.transform.SetPositionAndRotation(vector, quaternion);
						this.virtualCamera.m_Lens.FieldOfView = num5;
						await UniTask.Yield();
						if (this.animateToken != token)
						{
							return;
						}
					}
				}
			}
			this.virtualCamera.Priority = -1;
		}

		// Token: 0x06001444 RID: 5188 RVA: 0x0004B353 File Offset: 0x00049553
		protected override void OnInteractStop()
		{
			base.OnInteractStop();
			Action<bool> onGamingConsoleInteractChanged = GamingConsole.OnGamingConsoleInteractChanged;
			if (onGamingConsoleInteractChanged != null)
			{
				onGamingConsoleInteractChanged(false);
			}
			this.inputHandler.enabled = false;
			this.AnimateCameraOut().Forget();
			HUDManager.UnregisterHideToken(this);
			GamingConsoleHUD.Hide();
		}

		// Token: 0x06001445 RID: 5189 RVA: 0x0004B390 File Offset: 0x00049590
		private void RefreshGame()
		{
			if (this.game == null)
			{
				this.CreateGame(this.SelectedGame);
				return;
			}
			if (this.SelectedGame == null || this.SelectedGame.ID != this.game.ID)
			{
				this.CreateGame(this.SelectedGame);
			}
		}

		// Token: 0x06001446 RID: 5190 RVA: 0x0004B3F0 File Offset: 0x000495F0
		private void CreateGame(MiniGame prefab)
		{
			if (this.isBeingDestroyed)
			{
				return;
			}
			if (this.game != null)
			{
				global::UnityEngine.Object.Destroy(this.game.gameObject);
			}
			if (prefab == null)
			{
				return;
			}
			this.game = global::UnityEngine.Object.Instantiate<MiniGame>(prefab);
			this.game.transform.SetParent(base.transform, true);
			this.game.SetRenderTexture(this.rt);
			this.game.SetConsole(this);
			this.inputHandler.SetGame(this.game);
		}

		// Token: 0x04000ED4 RID: 3796
		[SerializeField]
		private List<MiniGame> possibleGames;

		// Token: 0x04000ED5 RID: 3797
		[SerializeField]
		private RenderTexture rt;

		// Token: 0x04000ED6 RID: 3798
		[SerializeField]
		private MiniGameInputHandler inputHandler;

		// Token: 0x04000ED7 RID: 3799
		[SerializeField]
		private CinemachineVirtualCamera virtualCamera;

		// Token: 0x04000ED8 RID: 3800
		[SerializeField]
		private float transitionTime = 1f;

		// Token: 0x04000ED9 RID: 3801
		[SerializeField]
		private Transform vcamEndPosition;

		// Token: 0x04000EDA RID: 3802
		[SerializeField]
		private Transform vcamLookTarget;

		// Token: 0x04000EDB RID: 3803
		[SerializeField]
		private AnimationCurve posCurve;

		// Token: 0x04000EDC RID: 3804
		[SerializeField]
		private AnimationCurve rotCurve;

		// Token: 0x04000EDD RID: 3805
		[SerializeField]
		private AnimationCurve fovCurve;

		// Token: 0x04000EDE RID: 3806
		[SerializeField]
		private float activeFov = 45f;

		// Token: 0x04000EDF RID: 3807
		[SerializeField]
		private Transform teleportToPositionWhenBegin;

		// Token: 0x04000EE0 RID: 3808
		[SerializeField]
		private Item mainItem;

		// Token: 0x04000EE1 RID: 3809
		[SerializeField]
		[LocalizationKey("Default")]
		private string incompleteNotificationText = "GamingConsole_Incomplete";

		// Token: 0x04000EE2 RID: 3810
		[SerializeField]
		[LocalizationKey("Default")]
		private string noGameNotificationText = "GamingConsole_NoGame";

		// Token: 0x04000EE3 RID: 3811
		private MiniGame game;

		// Token: 0x04000EE8 RID: 3816
		private string SaveKey = "GamingConsoleData";

		// Token: 0x04000EE9 RID: 3817
		private bool loading;

		// Token: 0x04000EEA RID: 3818
		private bool loaded;

		// Token: 0x04000EEB RID: 3819
		private bool isBeingDestroyed;

		// Token: 0x04000EEC RID: 3820
		private int animateToken;

		// Token: 0x0200054C RID: 1356
		[Serializable]
		private class SaveData
		{
			// Token: 0x04001EAE RID: 7854
			public ItemTreeData monitorData;

			// Token: 0x04001EAF RID: 7855
			public ItemTreeData consoleData;
		}
	}
}
