using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Duckov.Modding;
using Duckov.Utilities;
using ItemStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace CheatMenu
{
	// Token: 0x02000005 RID: 5
	[NullableContext(1)]
	[Nullable(0)]
	public class ModBehaviour : ModBehaviour
	{
		// Token: 0x06000005 RID: 5 RVA: 0x0000208E File Offset: 0x0000028E
		private void Awake()
		{
			this.LoadSettings();
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002096 File Offset: 0x00000296
		private void OnDestroy()
		{
			this.RestoreMovementUnlockEffects();
			this.DestroyOverlayResources();
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020A4 File Offset: 0x000002A4
		private void OnEnable()
		{
			LevelManager.OnAfterLevelInitialized += this.OnLevelInitialized;
			this.TryApplyNoRecoil();
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000020C0 File Offset: 0x000002C0
		private void OnDisable()
		{
			LevelManager.OnAfterLevelInitialized -= this.OnLevelInitialized;
			this.UnhookCharacterEvents();
			this.RestoreOriginalCharacterRecoil();
			this.RestoreGunStatMultipliers();
			this.RestoreItemStatOverrides();
			this.RestoreMovementUnlockEffects();
			this.DestroyOverlayResources();
			this._menuVisible = true;
			this._rangeVisible = true;
			this._lastTriggerInput = false;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002117 File Offset: 0x00000317
		private void OnLevelInitialized()
		{
			this.TryApplyNoRecoil();
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002120 File Offset: 0x00000320
		private void TryApplyNoRecoil()
		{
			CharacterMainControl main = CharacterMainControl.Main;
			if (main == null || main.CharacterItem == null)
			{
				return;
			}
			this.HookCharacterEvents(main);
			this.ApplyInvincibility(main);
			this.ApplyNoRecoilToCharacter(main);
			ItemAgent_Gun itemAgent_Gun = ((main.agentHolder != null) ? main.agentHolder.CurrentHoldGun : null);
			this.ApplyNoRecoilToGun(itemAgent_Gun);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002184 File Offset: 0x00000384
		private void HookCharacterEvents(CharacterMainControl character)
		{
			if (character == null)
			{
				return;
			}
			if (this._trackedCharacter != character)
			{
				this.UnhookCharacterEvents();
				this._trackedCharacter = character;
			}
			if (this._trackedCharacter != null && !this._characterEventsHooked)
			{
				if (this._trackedCharacter.agentHolder != null)
				{
					this._trackedCharacter.agentHolder.OnHoldAgentChanged += this.OnHoldAgentChanged;
				}
				CharacterMainControl.OnMainCharacterInventoryChangedEvent = (Action<CharacterMainControl, Inventory, int>)Delegate.Combine(CharacterMainControl.OnMainCharacterInventoryChangedEvent, new Action<CharacterMainControl, Inventory, int>(this.OnMainCharacterInventoryChanged));
				this._characterEventsHooked = true;
			}
			if (this._trackedCharacter != null)
			{
				ItemAgent_Gun itemAgent_Gun = ((this._trackedCharacter.agentHolder != null) ? this._trackedCharacter.agentHolder.CurrentHoldGun : null);
				this.HookGunEvents(itemAgent_Gun);
			}
			this.ApplyInvincibility(this._trackedCharacter);
			this._lastTriggerInput = false;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002274 File Offset: 0x00000474
		private void UnhookCharacterEvents()
		{
			if (this._trackedCharacter != null && this._characterEventsHooked && this._trackedCharacter.agentHolder != null)
			{
				this._trackedCharacter.agentHolder.OnHoldAgentChanged -= this.OnHoldAgentChanged;
			}
			CharacterMainControl.OnMainCharacterInventoryChangedEvent = (Action<CharacterMainControl, Inventory, int>)Delegate.Remove(CharacterMainControl.OnMainCharacterInventoryChangedEvent, new Action<CharacterMainControl, Inventory, int>(this.OnMainCharacterInventoryChanged));
			this._characterEventsHooked = false;
			this.HookGunEvents(null);
			this.RestoreMovementUnlockEffects();
			this.RestoreCharacterInvincibility();
			this._trackedCharacter = null;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002307 File Offset: 0x00000507
		private void OnHoldAgentChanged(DuckovItemAgent agent)
		{
			this.ApplyNoRecoilToGun(agent as ItemAgent_Gun);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002318 File Offset: 0x00000518
		private void OnMainCharacterInventoryChanged(CharacterMainControl character, Inventory inventory, int slotIndex)
		{
			if (character != this._trackedCharacter || inventory == null)
			{
				return;
			}
			if (slotIndex < 0 || slotIndex >= inventory.Content.Count)
			{
				return;
			}
			Item item = inventory.Content[slotIndex];
			if (item == null)
			{
				return;
			}
			this.ApplyNoRecoilToItem(item);
			if (this._trackedCharacter != null && this._trackedCharacter.agentHolder != null)
			{
				ItemAgent_Gun currentHoldGun = this._trackedCharacter.agentHolder.CurrentHoldGun;
				if (currentHoldGun != null && currentHoldGun.Item == item)
				{
					this.ApplyNoRecoilToGun(currentHoldGun);
				}
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000023C0 File Offset: 0x000005C0
		private void ApplyNoRecoilToCharacter(CharacterMainControl character)
		{
			if (!this._noRecoilEnabled)
			{
				this.RestoreOriginalCharacterRecoil();
				return;
			}
			Stat stat = character.CharacterItem.GetStat(ModBehaviour.RecoilControlStatHash);
			if (stat == null)
			{
				return;
			}
			if (this._originalCharacterRecoilControl == null)
			{
				this._originalCharacterRecoilControl = new float?(stat.BaseValue);
			}
			stat.BaseValue = 9999f;
			this.ForceDisableTriggerTimer(character, 0f);
			ModBehaviour.TryRefillStamina(character);
			this.TryEnableDashControl(character);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002434 File Offset: 0x00000634
		[NullableContext(2)]
		private void ApplyNoRecoilToGun(ItemAgent_Gun gun)
		{
			if (gun == null || gun.Item == null)
			{
				return;
			}
			if (this._noRecoilEnabled)
			{
				this.ApplyNoRecoilToItem(gun.Item);
				this.ResetGunInternals(gun);
			}
			if (this._infiniteAmmoEnabled)
			{
				this.RestoreGunAmmo(gun);
			}
			this.HookGunEvents(gun);
			this.TryOverrideTriggerRestrictions(gun);
			this.ApplyGunStatMultipliers(gun);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002498 File Offset: 0x00000698
		private void ApplyNoRecoilToItem(Item item)
		{
			foreach (int num in ModBehaviour.GunStatHashes)
			{
				this.OverrideItemStat(item, num, 0f);
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000024CC File Offset: 0x000006CC
		[NullableContext(2)]
		private void HookGunEvents(ItemAgent_Gun gun)
		{
			if (this._trackedGun == gun)
			{
				return;
			}
			if (this._trackedGun != null)
			{
				this._trackedGun.OnShootEvent -= this.OnGunShoot;
			}
			this._trackedGun = gun;
			if (this._trackedGun != null)
			{
				this._trackedGun.OnShootEvent += this.OnGunShoot;
				this.TryOverrideTriggerRestrictions(this._trackedGun);
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002548 File Offset: 0x00000748
		private void Update()
		{
			this.HandleOverlayInput();
			CharacterMainControl trackedCharacter = this._trackedCharacter;
			if (trackedCharacter != null)
			{
				this.MaintainTriggerAccess(trackedCharacter);
				if (this._obstaclePenetrationEnabled && this._penetrationActive)
				{
					this.MaintainPenetratingProjectiles();
				}
			}
			else
			{
				this._lastTriggerInput = false;
			}
			if (trackedCharacter == null)
			{
				this.SetMenuVisibility(false);
				this.HideAutoAimRange();
				return;
			}
			LevelManager instance = LevelManager.Instance;
			if (!LevelManager.LevelInited || instance == null || instance.GameCamera == null || instance.GameCamera.renderCamera == null)
			{
				this.SetMenuVisibility(false);
				this.HideAutoAimRange();
				return;
			}
			InputManager inputManager = instance.InputManager;
			Vector2 vector = ((inputManager != null) ? inputManager.AimScreenPoint : new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f));
			this._latestAimScreenPoint = vector;
			if (this._menuVisible)
			{
				this.InitializeMenuWindowIfNeeded();
				this.RefreshMenuContent(trackedCharacter, vector);
			}
			else
			{
				this.SetMenuVisibility(false);
			}
			if (this._rangeVisible && this._autoAimEnabled)
			{
				Camera renderCamera = instance.GameCamera.renderCamera;
				float num = trackedCharacter.transform.position.y + 0.5f;
				this.UpdateAutoAimRangeVisualization(renderCamera, vector, num);
				return;
			}
			this.HideAutoAimRange();
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002690 File Offset: 0x00000890
		private void OnGunShoot()
		{
			if (this._trackedGun == null)
			{
				return;
			}
			this.TryAutoAimShot(this._trackedGun);
			if (this._noRecoilEnabled)
			{
				this.ResetGunInternals(this._trackedGun);
				if (this._trackedGun.Item != null)
				{
					this.ApplyNoRecoilToItem(this._trackedGun.Item);
				}
			}
			if (this._infiniteAmmoEnabled)
			{
				this.RestoreGunAmmo(this._trackedGun);
			}
			this.TryOverrideTriggerRestrictions(this._trackedGun);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002710 File Offset: 0x00000910
		[NullableContext(2)]
		private void TryOverrideTriggerRestrictions(ItemAgent_Gun gun)
		{
			if (!this._movementUnlockEnabled)
			{
				return;
			}
			if (gun == null)
			{
				return;
			}
			CharacterMainControl holder = gun.Holder;
			if (holder == null)
			{
				return;
			}
			this.ForceDisableTriggerTimer(holder, 0f);
			ModBehaviour.TryRefillStamina(holder);
			this.TryEnableDashControl(holder);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x0000275C File Offset: 0x0000095C
		private static void TrySetDisableTriggerTimer(CharacterMainControl character, float value)
		{
			if (ModBehaviour.DisableTriggerTimerField == null)
			{
				return;
			}
			try
			{
				ModBehaviour.DisableTriggerTimerField.SetValue(character, value);
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000027A0 File Offset: 0x000009A0
		private static void TryRefillStamina(CharacterMainControl character)
		{
			if (ModBehaviour.CurrentStaminaField == null)
			{
				return;
			}
			try
			{
				ModBehaviour.CurrentStaminaField.SetValue(character, character.MaxStamina);
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000027E8 File Offset: 0x000009E8
		private static float GetDisableTriggerTimer(CharacterMainControl character)
		{
			if (ModBehaviour.DisableTriggerTimerField == null)
			{
				return 0f;
			}
			try
			{
				object value = ModBehaviour.DisableTriggerTimerField.GetValue(character);
				if (value is float)
				{
					return (float)value;
				}
			}
			catch (Exception)
			{
			}
			return 0f;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002844 File Offset: 0x00000A44
		private void MaintainTriggerAccess(CharacterMainControl character)
		{
			if (!this._movementUnlockEnabled)
			{
				return;
			}
			this.ForceDisableTriggerTimer(character, 0f);
			ModBehaviour.TryRefillStamina(character);
			this.TryEnableDashControl(character);
			ItemAgentHolder agentHolder = character.agentHolder;
			ItemAgent_Gun itemAgent_Gun = ((agentHolder != null) ? agentHolder.CurrentHoldGun : null);
			if (agentHolder == null || itemAgent_Gun == null)
			{
				return;
			}
			InputManager inputManager = ((LevelManager.Instance != null) ? LevelManager.Instance.InputManager : null);
			if (inputManager == null)
			{
				return;
			}
			bool triggerInput = inputManager.TriggerInput;
			bool flag = triggerInput && !this._lastTriggerInput;
			bool flag2 = !triggerInput && this._lastTriggerInput;
			this._lastTriggerInput = triggerInput;
			if (!triggerInput)
			{
				return;
			}
			CharacterActionBase currentAction = character.CurrentAction;
			bool flag3 = currentAction != null && currentAction.Running;
			bool flag4 = flag3 && character.dashAction != null && currentAction == character.dashAction;
			if (!character.Running && ModBehaviour.GetDisableTriggerTimer(character) <= 0f && (!flag3 || flag4))
			{
				return;
			}
			Movement movementControl = character.movementControl;
			if (movementControl != null)
			{
				movementControl.ForceSetAimDirectionToAimPoint();
			}
			agentHolder.SetTrigger(triggerInput, flag, flag2);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x0000297C File Offset: 0x00000B7C
		private void TryEnableDashControl(CharacterMainControl character)
		{
			if (!this._movementUnlockEnabled)
			{
				return;
			}
			if (character == null || character.CharacterItem == null)
			{
				return;
			}
			this.EnsureMovementUnlockSnapshot(character);
			this.TryRemoveDashCooldown(character);
			if (character.DashCanControl)
			{
				return;
			}
			try
			{
				character.DashCanControl = true;
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000029E0 File Offset: 0x00000BE0
		private void TryRemoveDashCooldown(CharacterMainControl character)
		{
			if (!this._movementUnlockEnabled)
			{
				return;
			}
			if (character == null)
			{
				return;
			}
			CA_Dash dashAction = character.dashAction;
			if (dashAction == null)
			{
				return;
			}
			this.EnsureMovementUnlockSnapshot(character);
			if (Mathf.Abs(dashAction.coolTime) <= 0.0001f)
			{
				return;
			}
			dashAction.coolTime = 0f;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002A38 File Offset: 0x00000C38
		private void EnsureMovementUnlockSnapshot(CharacterMainControl character)
		{
			if (!this._dashControlOriginalStates.ContainsKey(character))
			{
				this._dashControlOriginalStates[character] = character.DashCanControl;
			}
			if (!this._disableTriggerOriginalValues.ContainsKey(character))
			{
				this._disableTriggerOriginalValues[character] = ModBehaviour.GetDisableTriggerTimer(character);
			}
			CA_Dash dashAction = character.dashAction;
			if (dashAction != null && !this._dashCooldownOriginalValues.ContainsKey(dashAction))
			{
				this._dashCooldownOriginalValues[dashAction] = dashAction.coolTime;
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002AB5 File Offset: 0x00000CB5
		private void ForceDisableTriggerTimer(CharacterMainControl character, float value)
		{
			if (!this._movementUnlockEnabled)
			{
				return;
			}
			this.EnsureMovementUnlockSnapshot(character);
			ModBehaviour.TrySetDisableTriggerTimer(character, value);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002ACE File Offset: 0x00000CCE
		private void ForceRestoreDisableTriggerTimer(CharacterMainControl character, float originalValue)
		{
			ModBehaviour.TrySetDisableTriggerTimer(character, originalValue);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002AD8 File Offset: 0x00000CD8
		private void RestoreMovementUnlockEffects()
		{
			if (this._dashControlOriginalStates.Count == 0 && this._disableTriggerOriginalValues.Count == 0 && this._dashCooldownOriginalValues.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<CharacterMainControl, bool> keyValuePair in this._dashControlOriginalStates.ToArray<KeyValuePair<CharacterMainControl, bool>>())
			{
				CharacterMainControl key = keyValuePair.Key;
				if (!(key == null))
				{
					try
					{
						key.DashCanControl = keyValuePair.Value;
					}
					catch (Exception)
					{
					}
				}
			}
			this._dashControlOriginalStates.Clear();
			foreach (KeyValuePair<CharacterMainControl, float> keyValuePair2 in this._disableTriggerOriginalValues.ToArray<KeyValuePair<CharacterMainControl, float>>())
			{
				CharacterMainControl key2 = keyValuePair2.Key;
				if (!(key2 == null))
				{
					this.ForceRestoreDisableTriggerTimer(key2, keyValuePair2.Value);
				}
			}
			this._disableTriggerOriginalValues.Clear();
			foreach (KeyValuePair<CA_Dash, float> keyValuePair3 in this._dashCooldownOriginalValues.ToArray<KeyValuePair<CA_Dash, float>>())
			{
				CA_Dash key3 = keyValuePair3.Key;
				if (!(key3 == null))
				{
					key3.coolTime = keyValuePair3.Value;
				}
			}
			this._dashCooldownOriginalValues.Clear();
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002C14 File Offset: 0x00000E14
		private void HandleOverlayInput()
		{
			bool flag = ModBehaviour.IsControlModifierActive();
			if (flag && Input.GetKeyDown(KeyCode.F8))
			{
				this.SetMenuVisible(!this._menuVisible);
			}
			if (flag && Input.GetKeyDown(KeyCode.F9))
			{
				this.SetRangeVisible(!this._rangeVisible);
			}
			if (flag && Input.GetKeyDown(KeyCode.F1))
			{
				this.SetAutoAimEnabled(!this._autoAimEnabled);
			}
			if (flag && Input.GetKeyDown(KeyCode.F2))
			{
				this.SetNoRecoilEnabled(!this._noRecoilEnabled);
			}
			if (flag && Input.GetKeyDown(KeyCode.F3))
			{
				this.SetInfiniteAmmoEnabled(!this._infiniteAmmoEnabled);
			}
			if (flag && Input.GetKeyDown(KeyCode.F4))
			{
				this.SetMovementUnlockEnabled(!this._movementUnlockEnabled);
			}
			if (flag && Input.GetKeyDown(KeyCode.F5))
			{
				this.SetObstaclePenetrationEnabled(!this._obstaclePenetrationEnabled);
			}
			if (flag && Input.GetKeyDown(KeyCode.F6))
			{
				this.SetInvincibilityEnabled(!this._invincibilityEnabled);
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002D15 File Offset: 0x00000F15
		private static bool IsControlModifierActive()
		{
			return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002D2F File Offset: 0x00000F2F
		private void SetMenuVisible(bool visible)
		{
			if (this._menuVisible == visible)
			{
				return;
			}
			this._menuVisible = visible;
			if (!visible)
			{
				this.SetMenuVisibility(false);
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002D4C File Offset: 0x00000F4C
		private void SetRangeVisible(bool visible)
		{
			if (this._rangeVisible == visible)
			{
				return;
			}
			this._rangeVisible = visible;
			if (!visible)
			{
				this.HideAutoAimRange();
			}
			this.SaveSettings();
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002D6E File Offset: 0x00000F6E
		private void SetAutoAimEnabled(bool enabled)
		{
			if (this._autoAimEnabled == enabled)
			{
				return;
			}
			this._autoAimEnabled = enabled;
			if (!enabled)
			{
				this.HideAutoAimRange();
			}
			this.SaveSettings();
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002D90 File Offset: 0x00000F90
		private void SetNoRecoilEnabled(bool enabled)
		{
			if (this._noRecoilEnabled == enabled)
			{
				return;
			}
			this._noRecoilEnabled = enabled;
			if (this._noRecoilEnabled)
			{
				this.TryApplyNoRecoil();
			}
			else
			{
				this.RestoreOriginalCharacterRecoil();
				this.RestoreItemStatOverrides();
			}
			this.SaveSettings();
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002DC5 File Offset: 0x00000FC5
		private void SetInfiniteAmmoEnabled(bool enabled)
		{
			if (this._infiniteAmmoEnabled == enabled)
			{
				return;
			}
			this._infiniteAmmoEnabled = enabled;
			this.SaveSettings();
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002DE0 File Offset: 0x00000FE0
		private void SetMovementUnlockEnabled(bool enabled)
		{
			if (this._movementUnlockEnabled == enabled)
			{
				return;
			}
			this._movementUnlockEnabled = enabled;
			if (this._movementUnlockEnabled)
			{
				if (this._trackedCharacter != null)
				{
					this.TryEnableDashControl(this._trackedCharacter);
				}
			}
			else
			{
				this.RestoreMovementUnlockEffects();
			}
			this.SaveSettings();
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002E2E File Offset: 0x0000102E
		private void SetObstaclePenetrationEnabled(bool enabled)
		{
			this._obstaclePenetrationEnabled = enabled;
			this.SaveSettings();
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002E3D File Offset: 0x0000103D
		private void SetInvincibilityEnabled(bool enabled)
		{
			if (this._invincibilityEnabled == enabled)
			{
				return;
			}
			this._invincibilityEnabled = enabled;
			if (this._invincibilityEnabled)
			{
				this.ApplyInvincibility(this._trackedCharacter);
			}
			else
			{
				this.RestoreCharacterInvincibility();
			}
			this.SaveSettings();
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002E72 File Offset: 0x00001072
		private void SetRangeMultiplierFromSlider(float value)
		{
			this.HandleMultiplierSliderChanged(ref this._rangeMultiplier, value, null, null);
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002E83 File Offset: 0x00001083
		private void SetFireRateMultiplierFromSlider(float value)
		{
			this.HandleMultiplierSliderChanged(ref this._fireRateMultiplier, value, null, null);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002E94 File Offset: 0x00001094
		private void SetBulletSpeedMultiplierFromSlider(float value)
		{
			this.HandleMultiplierSliderChanged(ref this._bulletSpeedMultiplier, value, null, null);
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002EA5 File Offset: 0x000010A5
		private void SetDamageMultiplierFromSlider(float value)
		{
			this.HandleMultiplierSliderChanged(ref this._damageMultiplier, value, null, null);
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002EB8 File Offset: 0x000010B8
		private void SetAutoAimScreenRadius(float radius)
		{
			float num = Mathf.Clamp(Mathf.Round(radius / 5f) * 5f, 60f, 2048f);
			if (Mathf.Abs(this._autoAimScreenRadius - num) <= 0.001f)
			{
				return;
			}
			this._autoAimScreenRadius = num;
			this._autoAimCircleRadiusPixels = this._autoAimScreenRadius;
			this.UpdateAutoAimSection();
			this.SaveSettings();
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002F1C File Offset: 0x0000111C
		private void InitializeMenuWindowIfNeeded()
		{
			if (this._menuWindowInitialized)
			{
				return;
			}
			float num = Mathf.Clamp((float)Screen.width * 0.32f, 320f, 420f);
			float num2 = Mathf.Clamp((float)Screen.height * 0.55f, 360f, Mathf.Max(360f, (float)Screen.height - 120f));
			float num3 = (float)Screen.width - num - 40f;
			float num4 = 40f;
			this._menuWindowRect = new Rect(num3, num4, num, num2);
			this._menuWindowInitialized = true;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002FA8 File Offset: 0x000011A8
		private void EnsureAutoAimCircleTexture()
		{
			if (this._autoAimCircleTexture != null)
			{
				return;
			}
			Texture2D texture2D = new Texture2D(256, 256, TextureFormat.ARGB32, false)
			{
				wrapMode = TextureWrapMode.Clamp,
				filterMode = FilterMode.Bilinear,
				hideFlags = HideFlags.DontSave
			};
			Color32[] array = new Color32[65536];
			float num = 128f;
			float num2 = num - 0.5f;
			float num3 = num * 0.94f;
			Color32 color = new Color32(51, 191, byte.MaxValue, byte.MaxValue);
			for (int i = 0; i < 256; i++)
			{
				float num4 = (float)i - num + 0.5f;
				for (int j = 0; j < 256; j++)
				{
					float num5 = (float)j - num + 0.5f;
					float num6 = Mathf.Sqrt(num5 * num5 + num4 * num4);
					int num7 = i * 256 + j;
					if (num6 > num2 || num6 < num3)
					{
						array[num7] = new Color32(0, 0, 0, 0);
					}
					else
					{
						float num8 = Mathf.InverseLerp(num2, num3, num6);
						byte b = (byte)Mathf.Lerp(72f, 216f, num8);
						array[num7] = new Color32(color.r, color.g, color.b, b);
					}
				}
			}
			texture2D.SetPixels32(array);
			texture2D.Apply(false, true);
			this._autoAimCircleTexture = texture2D;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003108 File Offset: 0x00001308
		private void EnsureOverlayCanvas()
		{
			if (this._overlayCanvasObject == null)
			{
				this._overlayCanvasObject = new GameObject("CheatMenuOverlayCanvas");
				this._overlayCanvasObject.transform.SetParent(base.transform, false);
				Canvas canvas = this._overlayCanvasObject.AddComponent<Canvas>();
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				canvas.sortingOrder = 5000;
				CanvasScaler canvasScaler = this._overlayCanvasObject.AddComponent<CanvasScaler>();
				canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
				canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
				canvasScaler.matchWidthOrHeight = 0.5f;
				this._overlayCanvasObject.AddComponent<GraphicRaycaster>();
			}
			if (!this._overlayCanvasObject.activeSelf)
			{
				this._overlayCanvasObject.SetActive(true);
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000031C0 File Offset: 0x000013C0
		private void EnsureOverlayUI()
		{
			this.EnsureOverlayCanvas();
			if (this._overlayCanvasObject == null)
			{
				return;
			}
			if (this._menuPanel == null)
			{
				GameObject gameObject = new GameObject("CheatMenuPanel");
				gameObject.transform.SetParent(this._overlayCanvasObject.transform, false);
				this._menuPanel = gameObject.AddComponent<RectTransform>();
				this._menuPanel.anchorMin = new Vector2(1f, 1f);
				this._menuPanel.anchorMax = new Vector2(1f, 1f);
				this._menuPanel.pivot = new Vector2(1f, 1f);
				this._menuPanel.anchoredPosition = new Vector2(-32f, -32f);
				this._menuPanel.sizeDelta = new Vector2(580f, 440f);
			}
			if (this._menuPanel != null)
			{
				Image component = this._menuPanel.GetComponent<Image>();
				if (component != null)
				{
					Object.Destroy(component);
				}
			}
			this.EnsureMenuHierarchy();
			this.EnsureSidebarButtons();
			this.EnsureSectionRoot(ModBehaviour.MenuSection.FunctionToggles);
			this.EnsureSectionRoot(ModBehaviour.MenuSection.Multipliers);
			this.EnsureSectionRoot(ModBehaviour.MenuSection.AutoAim);
			this.EnsureSectionRoot(ModBehaviour.MenuSection.Weapon);
			this.EnsureSectionRoot(ModBehaviour.MenuSection.Shortcuts);
			this.EnsureMultiplierSliders();
			this.UpdateSidebarSelectionVisuals();
			this.UpdateSectionVisibility();
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003310 File Offset: 0x00001510
		private void EnsureMenuHierarchy()
		{
			if (this._menuPanel == null)
			{
				return;
			}
			if (this._menuRoot == null)
			{
				GameObject gameObject = new GameObject("CheatMenuRoot");
				gameObject.transform.SetParent(this._menuPanel, false);
				this._menuRoot = gameObject.AddComponent<RectTransform>();
				this._menuRoot.anchorMin = Vector2.zero;
				this._menuRoot.anchorMax = Vector2.one;
				this._menuRoot.offsetMin = Vector2.zero;
				this._menuRoot.offsetMax = Vector2.zero;
				HorizontalLayoutGroup horizontalLayoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
				horizontalLayoutGroup.spacing = 18f;
				horizontalLayoutGroup.padding = new RectOffset(18, 18, 18, 18);
				horizontalLayoutGroup.childAlignment = 0;
				horizontalLayoutGroup.childControlWidth = true;
				horizontalLayoutGroup.childForceExpandWidth = true;
				horizontalLayoutGroup.childControlHeight = true;
				horizontalLayoutGroup.childForceExpandHeight = true;
			}
			if (this._sidebarContainer == null && this._menuRoot != null)
			{
				GameObject gameObject2 = new GameObject("CheatMenuSidebar");
				gameObject2.transform.SetParent(this._menuRoot, false);
				this._sidebarContainer = gameObject2.AddComponent<RectTransform>();
				LayoutElement layoutElement = gameObject2.AddComponent<LayoutElement>();
				layoutElement.preferredWidth = 188f;
				layoutElement.minWidth = 170f;
				VerticalLayoutGroup verticalLayoutGroup = gameObject2.AddComponent<VerticalLayoutGroup>();
				verticalLayoutGroup.spacing = 10f;
				verticalLayoutGroup.padding = new RectOffset(0, 0, 4, 4);
				verticalLayoutGroup.childAlignment = 0;
				verticalLayoutGroup.childControlWidth = true;
				verticalLayoutGroup.childForceExpandWidth = true;
				verticalLayoutGroup.childControlHeight = false;
				verticalLayoutGroup.childForceExpandHeight = false;
			}
			if (this._contentContainer == null && this._menuRoot != null)
			{
				GameObject gameObject3 = new GameObject("CheatMenuContent");
				gameObject3.transform.SetParent(this._menuRoot, false);
				this._contentContainer = gameObject3.AddComponent<RectTransform>();
				this._contentContainer.anchorMin = Vector2.zero;
				this._contentContainer.anchorMax = Vector2.one;
				this._contentContainer.offsetMin = Vector2.zero;
				this._contentContainer.offsetMax = Vector2.zero;
				LayoutElement layoutElement2 = gameObject3.AddComponent<LayoutElement>();
				layoutElement2.flexibleWidth = 1f;
				layoutElement2.flexibleHeight = 1f;
				layoutElement2.minWidth = 240f;
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00003548 File Offset: 0x00001748
		private void EnsureSidebarButtons()
		{
			if (this._sidebarContainer == null)
			{
				return;
			}
			foreach (object obj in Enum.GetValues(typeof(ModBehaviour.MenuSection)))
			{
				ModBehaviour.MenuSection menuSection = (ModBehaviour.MenuSection)obj;
				this.EnsureSidebarButton(menuSection);
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000035BC File Offset: 0x000017BC
		private void EnsureSidebarButton(ModBehaviour.MenuSection section)
		{
			if (this._sidebarContainer == null)
			{
				return;
			}
			if (this._menuSectionButtons.ContainsKey(section))
			{
				return;
			}
			GameObject gameObject = new GameObject(string.Format("{0}Button", section));
			gameObject.transform.SetParent(this._sidebarContainer, false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.sizeDelta = new Vector2(0f, 44f);
			Image image = gameObject.AddComponent<Image>();
			image.color = this._sidebarButtonColor;
			Button button = gameObject.AddComponent<Button>();
			button.targetGraphic = image;
			button.transition = Selectable.Transition.None;
			button.navigation = new Navigation
			{
				mode = Navigation.Mode.None
			};
			LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
			layoutElement.minHeight = 44f;
			layoutElement.preferredHeight = 44f;
			TextMeshProUGUI textMeshProUGUI = Object.Instantiate<TextMeshProUGUI>(GameplayDataSettings.UIStyle.TemplateTextUGUI, rectTransform);
			RectTransform rectTransform2 = textMeshProUGUI.rectTransform;
			rectTransform2.anchorMin = Vector2.zero;
			rectTransform2.anchorMax = Vector2.one;
			rectTransform2.offsetMin = new Vector2(14f, 6f);
			rectTransform2.offsetMax = new Vector2(-14f, -6f);
			textMeshProUGUI.alignment = 4097;
			textMeshProUGUI.fontSize = 22f;
			textMeshProUGUI.enableWordWrapping = false;
			textMeshProUGUI.text = ModBehaviour.GetMenuSectionLabel(section);
			ModBehaviour.MenuSection captured = section;
			button.onClick.AddListener(delegate
			{
				this.SwitchMenuSection(captured);
			});
			this._menuSectionButtons[section] = button;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x0000373C File Offset: 0x0000193C
		private static string GetMenuSectionLabel(ModBehaviour.MenuSection section)
		{
			switch (section)
			{
			case ModBehaviour.MenuSection.FunctionToggles:
				return "功能开关";
			case ModBehaviour.MenuSection.Multipliers:
				return "数值倍率";
			case ModBehaviour.MenuSection.AutoAim:
				return "自瞄参数";
			case ModBehaviour.MenuSection.Weapon:
				return "武器信息";
			case ModBehaviour.MenuSection.Shortcuts:
				return "快捷键";
			default:
				return section.ToString();
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003790 File Offset: 0x00001990
		private void EnsureSectionRoot(ModBehaviour.MenuSection section)
		{
			if (this._contentContainer == null)
			{
				return;
			}
			if (this._menuSectionRoots.ContainsKey(section))
			{
				return;
			}
			GameObject gameObject = new GameObject(string.Format("{0}Section", section));
			gameObject.transform.SetParent(this._contentContainer, false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
			layoutElement.flexibleWidth = 1f;
			layoutElement.flexibleHeight = 1f;
			layoutElement.minHeight = 0f;
			this._menuSectionRoots[section] = gameObject;
			switch (section)
			{
			case ModBehaviour.MenuSection.FunctionToggles:
				this._functionToggleContent = this.CreateSectionText(rectTransform);
				break;
			case ModBehaviour.MenuSection.Multipliers:
				this.SetupMultipliersSection(rectTransform);
				break;
			case ModBehaviour.MenuSection.AutoAim:
				this._autoAimContent = this.CreateSectionText(rectTransform);
				break;
			case ModBehaviour.MenuSection.Weapon:
				this._weaponInfoContent = this.CreateSectionText(rectTransform);
				break;
			case ModBehaviour.MenuSection.Shortcuts:
				this._shortcutsContent = this.CreateSectionText(rectTransform);
				break;
			}
			gameObject.SetActive(section == this._activeMenuSection);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000038B8 File Offset: 0x00001AB8
		private TextMeshProUGUI CreateSectionText(RectTransform parent)
		{
			TextMeshProUGUI textMeshProUGUI = Object.Instantiate<TextMeshProUGUI>(GameplayDataSettings.UIStyle.TemplateTextUGUI, parent);
			RectTransform rectTransform = textMeshProUGUI.rectTransform;
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = new Vector2(18f, 18f);
			rectTransform.offsetMax = new Vector2(-18f, -18f);
			textMeshProUGUI.alignment = 257;
			textMeshProUGUI.fontSize = 22f;
			textMeshProUGUI.enableWordWrapping = true;
			textMeshProUGUI.richText = true;
			return textMeshProUGUI;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003940 File Offset: 0x00001B40
		private void SetupMultipliersSection(RectTransform sectionRect)
		{
			VerticalLayoutGroup verticalLayoutGroup = sectionRect.gameObject.AddComponent<VerticalLayoutGroup>();
			verticalLayoutGroup.spacing = 8f;
			verticalLayoutGroup.padding = new RectOffset(14, 14, 14, 14);
			verticalLayoutGroup.childAlignment = 0;
			verticalLayoutGroup.childControlWidth = true;
			verticalLayoutGroup.childForceExpandWidth = true;
			verticalLayoutGroup.childControlHeight = true;
			verticalLayoutGroup.childForceExpandHeight = false;
			this._multipliersSummary = Object.Instantiate<TextMeshProUGUI>(GameplayDataSettings.UIStyle.TemplateTextUGUI, sectionRect);
			this._multipliersSummary.alignment = 4097;
			this._multipliersSummary.fontSize = 20f;
			this._multipliersSummary.enableWordWrapping = true;
			this._multipliersSummary.richText = true;
			LayoutElement layoutElement = this._multipliersSummary.gameObject.AddComponent<LayoutElement>();
			layoutElement.preferredHeight = 64f;
			layoutElement.minHeight = 48f;
			layoutElement.flexibleHeight = 0f;
			if (this._sliderContainer == null)
			{
				GameObject gameObject = new GameObject("CheatMenuSliderContainer");
				gameObject.transform.SetParent(sectionRect, false);
				this._sliderContainer = gameObject.AddComponent<RectTransform>();
				VerticalLayoutGroup verticalLayoutGroup2 = gameObject.AddComponent<VerticalLayoutGroup>();
				verticalLayoutGroup2.spacing = 8f;
				verticalLayoutGroup2.padding = new RectOffset(0, 0, 18, 0);
				verticalLayoutGroup2.childAlignment = 0;
				verticalLayoutGroup2.childControlWidth = true;
				verticalLayoutGroup2.childForceExpandWidth = true;
				verticalLayoutGroup2.childControlHeight = true;
				verticalLayoutGroup2.childForceExpandHeight = false;
			}
			else
			{
				this._sliderContainer.SetParent(sectionRect, false);
			}
			LayoutElement layoutElement2 = this._sliderContainer.gameObject.GetComponent<LayoutElement>();
			if (layoutElement2 == null)
			{
				layoutElement2 = this._sliderContainer.gameObject.AddComponent<LayoutElement>();
			}
			layoutElement2.flexibleHeight = 1f;
			layoutElement2.minHeight = 180f;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003ADB File Offset: 0x00001CDB
		private void SwitchMenuSection(ModBehaviour.MenuSection section)
		{
			if (this._activeMenuSection == section)
			{
				return;
			}
			this._activeMenuSection = section;
			this.UpdateSidebarSelectionVisuals();
			this.UpdateSectionVisibility();
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003AFC File Offset: 0x00001CFC
		private void UpdateSidebarSelectionVisuals()
		{
			foreach (KeyValuePair<ModBehaviour.MenuSection, Button> keyValuePair in this._menuSectionButtons)
			{
				Button value = keyValuePair.Value;
				if (!(value == null))
				{
					Image component = value.GetComponent<Image>();
					if (component != null)
					{
						component.color = ((keyValuePair.Key == this._activeMenuSection) ? this._sidebarButtonSelectedColor : this._sidebarButtonColor);
					}
				}
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003B8C File Offset: 0x00001D8C
		private void UpdateSectionVisibility()
		{
			foreach (KeyValuePair<ModBehaviour.MenuSection, GameObject> keyValuePair in this._menuSectionRoots)
			{
				if (!(keyValuePair.Value == null))
				{
					keyValuePair.Value.SetActive(keyValuePair.Key == this._activeMenuSection);
				}
			}
			if (this._sliderContainer != null)
			{
				this._sliderContainer.gameObject.SetActive(this._activeMenuSection == ModBehaviour.MenuSection.Multipliers);
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003C2C File Offset: 0x00001E2C
		private void EnsureMultiplierSliders()
		{
			if (this._sliderContainer == null)
			{
				return;
			}
			this.EnsureMultiplierSlider(ref this._rangeSlider, ref this._rangeValueText, "RangeMultiplierRow", "射程倍率", this._rangeMultiplier, new UnityAction<float>(this.OnRangeSliderChanged));
			this.EnsureMultiplierSlider(ref this._fireRateSlider, ref this._fireRateValueText, "FireRateMultiplierRow", "射速倍率", this._fireRateMultiplier, new UnityAction<float>(this.OnFireRateSliderChanged));
			this.EnsureMultiplierSlider(ref this._bulletSpeedSlider, ref this._bulletSpeedValueText, "BulletSpeedMultiplierRow", "子弹速度倍率", this._bulletSpeedMultiplier, new UnityAction<float>(this.OnBulletSpeedSliderChanged));
			this.EnsureMultiplierSlider(ref this._damageSlider, ref this._damageValueText, "DamageMultiplierRow", "伤害倍率", this._damageMultiplier, new UnityAction<float>(this.OnDamageSliderChanged));
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003D00 File Offset: 0x00001F00
		private void EnsureMultiplierSlider([Nullable(2)] ref Slider sliderField, [Nullable(2)] ref TextMeshProUGUI valueLabel, string objectName, string labelText, float currentValue, UnityAction<float> handler)
		{
			if (this._sliderContainer == null)
			{
				return;
			}
			if (sliderField != null)
			{
				sliderField.SetValueWithoutNotify(currentValue);
				this.UpdateSliderValueDisplay(valueLabel, currentValue);
				return;
			}
			GameObject gameObject = new GameObject(objectName);
			gameObject.transform.SetParent(this._sliderContainer, false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			HorizontalLayoutGroup horizontalLayoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
			horizontalLayoutGroup.spacing = 10f;
			horizontalLayoutGroup.childAlignment = 3;
			horizontalLayoutGroup.childControlWidth = true;
			horizontalLayoutGroup.childForceExpandWidth = true;
			horizontalLayoutGroup.childControlHeight = true;
			horizontalLayoutGroup.childForceExpandHeight = false;
			LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
			layoutElement.minHeight = 48f;
			layoutElement.preferredHeight = 48f;
			TextMeshProUGUI textMeshProUGUI = Object.Instantiate<TextMeshProUGUI>(GameplayDataSettings.UIStyle.TemplateTextUGUI, rectTransform);
			textMeshProUGUI.text = labelText;
			textMeshProUGUI.fontSize = 20f;
			textMeshProUGUI.alignment = 4097;
			textMeshProUGUI.enableWordWrapping = false;
			LayoutElement layoutElement2 = textMeshProUGUI.gameObject.AddComponent<LayoutElement>();
			layoutElement2.preferredWidth = 132f;
			layoutElement2.minWidth = 116f;
			Slider slider = this.CreateSliderControl(rectTransform);
			slider.minValue = 1f;
			slider.maxValue = 10f;
			slider.wholeNumbers = false;
			slider.SetValueWithoutNotify(currentValue);
			slider.onValueChanged.AddListener(handler);
			LayoutElement layoutElement3 = slider.gameObject.AddComponent<LayoutElement>();
			layoutElement3.flexibleWidth = 1f;
			layoutElement3.minWidth = 220f;
			layoutElement3.preferredHeight = 28f;
			TextMeshProUGUI textMeshProUGUI2 = Object.Instantiate<TextMeshProUGUI>(GameplayDataSettings.UIStyle.TemplateTextUGUI, rectTransform);
			textMeshProUGUI2.fontSize = 20f;
			textMeshProUGUI2.alignment = 4100;
			textMeshProUGUI2.enableWordWrapping = false;
			LayoutElement layoutElement4 = textMeshProUGUI2.gameObject.AddComponent<LayoutElement>();
			layoutElement4.preferredWidth = 84f;
			layoutElement4.minWidth = 72f;
			sliderField = slider;
			valueLabel = textMeshProUGUI2;
			this.UpdateSliderValueDisplay(textMeshProUGUI2, currentValue);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003EBC File Offset: 0x000020BC
		private Slider CreateSliderControl(RectTransform parent)
		{
			GameObject gameObject = new GameObject("Slider", new Type[] { typeof(RectTransform) });
			gameObject.transform.SetParent(parent, false);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.anchorMin = new Vector2(0f, 0.5f);
			component.anchorMax = new Vector2(1f, 0.5f);
			component.pivot = new Vector2(0.5f, 0.5f);
			component.sizeDelta = new Vector2(0f, 26f);
			Slider slider = gameObject.AddComponent<Slider>();
			slider.direction = Slider.Direction.LeftToRight;
			slider.navigation = new Navigation
			{
				mode = Navigation.Mode.None
			};
			GameObject gameObject2 = new GameObject("Background", new Type[] { typeof(RectTransform) });
			gameObject2.transform.SetParent(component, false);
			RectTransform component2 = gameObject2.GetComponent<RectTransform>();
			component2.anchorMin = Vector2.zero;
			component2.anchorMax = Vector2.one;
			component2.offsetMin = Vector2.zero;
			component2.offsetMax = Vector2.zero;
			gameObject2.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.7f);
			GameObject gameObject3 = new GameObject("Fill Area", new Type[] { typeof(RectTransform) });
			gameObject3.transform.SetParent(component, false);
			RectTransform component3 = gameObject3.GetComponent<RectTransform>();
			component3.anchorMin = new Vector2(0f, 0f);
			component3.anchorMax = new Vector2(1f, 1f);
			component3.offsetMin = new Vector2(4f, 4f);
			component3.offsetMax = new Vector2(-14f, -4f);
			GameObject gameObject4 = new GameObject("Fill", new Type[] { typeof(RectTransform) });
			gameObject4.transform.SetParent(component3, false);
			RectTransform component4 = gameObject4.GetComponent<RectTransform>();
			component4.anchorMin = new Vector2(0f, 0f);
			component4.anchorMax = new Vector2(1f, 1f);
			component4.offsetMin = Vector2.zero;
			component4.offsetMax = Vector2.zero;
			gameObject4.AddComponent<Image>().color = new Color(0.2f, 0.75f, 1f, 0.85f);
			slider.fillRect = component4;
			GameObject gameObject5 = new GameObject("Handle Slide Area", new Type[] { typeof(RectTransform) });
			gameObject5.transform.SetParent(component, false);
			RectTransform component5 = gameObject5.GetComponent<RectTransform>();
			component5.anchorMin = new Vector2(0f, 0f);
			component5.anchorMax = new Vector2(1f, 1f);
			component5.offsetMin = new Vector2(4f, 4f);
			component5.offsetMax = new Vector2(-4f, -4f);
			GameObject gameObject6 = new GameObject("Handle", new Type[] { typeof(RectTransform) });
			gameObject6.transform.SetParent(component5, false);
			RectTransform component6 = gameObject6.GetComponent<RectTransform>();
			component6.sizeDelta = new Vector2(16f, 24f);
			Image image = gameObject6.AddComponent<Image>();
			image.color = Color.white;
			slider.handleRect = component6;
			slider.targetGraphic = image;
			return slider;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00004206 File Offset: 0x00002406
		private static float SnapMultiplier(float value)
		{
			return Mathf.Clamp(Mathf.Round(value / 0.1f) * 0.1f, 1f, 10f);
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00004229 File Offset: 0x00002429
		private static string FormatMultiplierValue(float value)
		{
			return string.Format("x{0:F1}", value);
		}

		// Token: 0x06000042 RID: 66 RVA: 0x0000423B File Offset: 0x0000243B
		private static string FormatShortcutLabel(KeyCode key)
		{
			return string.Format("{0} + {1}", "Ctrl", key);
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00004252 File Offset: 0x00002452
		[NullableContext(2)]
		private void UpdateSliderValueDisplay(TextMeshProUGUI valueText, float value)
		{
			if (valueText == null)
			{
				return;
			}
			valueText.SetText(ModBehaviour.FormatMultiplierValue(value), true);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x0000426B File Offset: 0x0000246B
		private void OnRangeSliderChanged(float value)
		{
			if (this._suppressSliderEvents)
			{
				return;
			}
			this.HandleMultiplierSliderChanged(ref this._rangeMultiplier, value, this._rangeSlider, this._rangeValueText);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x0000428F File Offset: 0x0000248F
		private void OnFireRateSliderChanged(float value)
		{
			if (this._suppressSliderEvents)
			{
				return;
			}
			this.HandleMultiplierSliderChanged(ref this._fireRateMultiplier, value, this._fireRateSlider, this._fireRateValueText);
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000042B3 File Offset: 0x000024B3
		private void OnBulletSpeedSliderChanged(float value)
		{
			if (this._suppressSliderEvents)
			{
				return;
			}
			this.HandleMultiplierSliderChanged(ref this._bulletSpeedMultiplier, value, this._bulletSpeedSlider, this._bulletSpeedValueText);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000042D7 File Offset: 0x000024D7
		private void OnDamageSliderChanged(float value)
		{
			if (this._suppressSliderEvents)
			{
				return;
			}
			this.HandleMultiplierSliderChanged(ref this._damageMultiplier, value, this._damageSlider, this._damageValueText);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000042FC File Offset: 0x000024FC
		[NullableContext(2)]
		private void HandleMultiplierSliderChanged(ref float multiplierField, float sliderValue, Slider slider, TextMeshProUGUI valueLabel)
		{
			float num = ModBehaviour.SnapMultiplier(sliderValue);
			if (Mathf.Abs(num - sliderValue) > 0.0001f)
			{
				this._suppressSliderEvents = true;
				if (slider != null)
				{
					slider.SetValueWithoutNotify(num);
				}
				this._suppressSliderEvents = false;
			}
			if (Mathf.Abs(multiplierField - num) <= 0.0001f)
			{
				this.UpdateSliderValueDisplay(valueLabel, multiplierField);
				return;
			}
			multiplierField = num;
			this.UpdateSliderValueDisplay(valueLabel, multiplierField);
			this.ApplyGunStatMultipliers(this._trackedGun);
			this.SaveSettings();
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00004374 File Offset: 0x00002574
		[NullableContext(2)]
		private void ApplyGunStatMultipliers(ItemAgent_Gun gun)
		{
			if (gun == null || gun.Item == null)
			{
				return;
			}
			this.ApplyStatMultiplier(gun.Item, ModBehaviour.BulletDistanceStatHash, this._rangeMultiplier);
			this.ApplyStatMultiplier(gun.Item, ModBehaviour.ShootSpeedStatHash, this._fireRateMultiplier);
			this.ApplyStatMultiplier(gun.Item, ModBehaviour.BulletSpeedStatHash, this._bulletSpeedMultiplier);
			this.ApplyStatMultiplier(gun.Item, ModBehaviour.DamageStatHash, this._damageMultiplier);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000043F8 File Offset: 0x000025F8
		private void ApplyStatMultiplier(Item item, int statHash, float multiplier)
		{
			if (item == null)
			{
				return;
			}
			Stat stat = item.GetStat(statHash);
			if (stat == null)
			{
				return;
			}
			Dictionary<int, float> dictionary;
			if (!this._statMultiplierOriginalValues.TryGetValue(item, out dictionary))
			{
				dictionary = new Dictionary<int, float>();
				this._statMultiplierOriginalValues[item] = dictionary;
			}
			float baseValue;
			if (!dictionary.TryGetValue(statHash, out baseValue))
			{
				baseValue = stat.BaseValue;
				dictionary[statHash] = baseValue;
			}
			float num = baseValue * multiplier;
			stat.BaseValue = num;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00004464 File Offset: 0x00002664
		private void RestoreGunStatMultipliers()
		{
			foreach (KeyValuePair<Item, Dictionary<int, float>> keyValuePair in this._statMultiplierOriginalValues)
			{
				Item key = keyValuePair.Key;
				if (!(key == null))
				{
					foreach (KeyValuePair<int, float> keyValuePair2 in keyValuePair.Value)
					{
						Stat stat = key.GetStat(keyValuePair2.Key);
						if (stat != null)
						{
							stat.BaseValue = keyValuePair2.Value;
						}
					}
				}
			}
			this._statMultiplierOriginalValues.Clear();
		}

		// Token: 0x0600004C RID: 76 RVA: 0x0000452C File Offset: 0x0000272C
		private void RefreshMenuContent(CharacterMainControl character, Vector2 aimScreenPoint)
		{
			ItemAgent_Gun itemAgent_Gun = ((character.agentHolder != null) ? character.agentHolder.CurrentHoldGun : null);
			this.UpdateFunctionToggleSection();
			this.UpdateMultipliersSummary();
			this.UpdateAutoAimSection();
			this.UpdateWeaponInfoSection(character, itemAgent_Gun);
			this.UpdateShortcutsSection();
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00004578 File Offset: 0x00002778
		private void UpdateFunctionToggleSection()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			stringBuilder.AppendLine("<b>功能开关</b>");
			stringBuilder.Append("  • 自瞄：").AppendLine(ModBehaviour.FormatToggleStatus(this._autoAimEnabled));
			stringBuilder.Append("  • 自瞄范围显示：").AppendLine(ModBehaviour.FormatToggleStatus(this._autoAimEnabled && this._rangeVisible));
			stringBuilder.Append("  • 无后坐力：").AppendLine(ModBehaviour.FormatToggleStatus(this._noRecoilEnabled));
			stringBuilder.Append("  • 无限弹药：").AppendLine(ModBehaviour.FormatToggleStatus(this._infiniteAmmoEnabled));
			stringBuilder.Append("  • 动作解锁射击：").AppendLine(ModBehaviour.FormatToggleStatus(this._movementUnlockEnabled));
			stringBuilder.Append("  • 子弹穿透障碍：").AppendLine(ModBehaviour.FormatToggleStatus(this._obstaclePenetrationEnabled));
			stringBuilder.Append("  • 主角无敌：").AppendLine(ModBehaviour.FormatToggleStatus(this._invincibilityEnabled));
			this._functionToggleText = stringBuilder.ToString();
			if (this._functionToggleContent != null)
			{
				this._functionToggleContent.SetText(this._functionToggleText, true);
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004698 File Offset: 0x00002898
		private void UpdateMultipliersSummary()
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			stringBuilder.AppendLine("<b>当前倍率</b>");
			stringBuilder.Append("  • 射程：").AppendLine(ModBehaviour.FormatMultiplierValue(this._rangeMultiplier));
			stringBuilder.Append("  • 射速：").AppendLine(ModBehaviour.FormatMultiplierValue(this._fireRateMultiplier));
			stringBuilder.Append("  • 子弹速度：").AppendLine(ModBehaviour.FormatMultiplierValue(this._bulletSpeedMultiplier));
			stringBuilder.Append("  • 伤害：").AppendLine(ModBehaviour.FormatMultiplierValue(this._damageMultiplier));
			this._multipliersSummaryText = stringBuilder.ToString();
			if (this._multipliersSummary != null)
			{
				this._multipliersSummary.SetText(this._multipliersSummaryText, true);
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00004758 File Offset: 0x00002958
		private void UpdateAutoAimSection()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			stringBuilder.AppendLine("<b>自瞄参数</b>");
			stringBuilder.Append("  • 自瞄状态：").AppendLine(ModBehaviour.FormatToggleStatus(this._autoAimEnabled));
			stringBuilder.Append("  • 自瞄半径：").Append(this._autoAimScreenRadius.ToString("F0")).AppendLine(" px");
			stringBuilder.Append("  • 最大射程：").Append(80f.ToString("F0")).AppendLine(" m");
			stringBuilder.Append("  • 穿透开关：").AppendLine(ModBehaviour.FormatToggleStatus(this._obstaclePenetrationEnabled));
			stringBuilder.Append("  • 穿透激活：").AppendLine(ModBehaviour.FormatToggleStatus(this._penetrationActive));
			this._autoAimContentText = stringBuilder.ToString();
			if (this._autoAimContent != null)
			{
				this._autoAimContent.SetText(this._autoAimContentText, true);
			}
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00004854 File Offset: 0x00002A54
		private void UpdateWeaponInfoSection(CharacterMainControl character, [Nullable(2)] ItemAgent_Gun gun)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			stringBuilder.AppendLine("<b>武器信息</b>");
			if (gun != null)
			{
				float bulletDistance = gun.BulletDistance;
				stringBuilder.Append("  • 当前武器：").AppendLine(gun.name);
				stringBuilder.Append("  • 理论射程：").Append(bulletDistance.ToString("F1")).AppendLine(" m");
				stringBuilder.Append("  • 射程倍率：").Append(character.GunDistanceMultiplier.ToString("F2")).AppendLine();
				stringBuilder.Append("  • 射速：").Append(gun.ShootSpeed.ToString("F2")).AppendLine(" 发/秒");
				stringBuilder.Append("  • 子弹速度：").Append(gun.BulletSpeed.ToString("F1")).AppendLine(" 米/秒");
				stringBuilder.Append("  • 伤害：").Append(gun.Damage.ToString("F1")).AppendLine();
			}
			else
			{
				stringBuilder.AppendLine("  • 当前武器：无");
			}
			this._weaponInfoText = stringBuilder.ToString();
			if (this._weaponInfoContent != null)
			{
				this._weaponInfoContent.SetText(this._weaponInfoText, true);
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000049B4 File Offset: 0x00002BB4
		private void UpdateShortcutsSection()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			stringBuilder.AppendLine("<b>快捷键</b>");
			stringBuilder.Append("  • ").Append(ModBehaviour.FormatShortcutLabel(KeyCode.F1)).AppendLine(" 切换自瞄");
			stringBuilder.Append("  • ").Append(ModBehaviour.FormatShortcutLabel(KeyCode.F2)).AppendLine(" 切换无后坐力");
			stringBuilder.Append("  • ").Append(ModBehaviour.FormatShortcutLabel(KeyCode.F3)).AppendLine(" 切换无限弹药");
			stringBuilder.Append("  • ").Append(ModBehaviour.FormatShortcutLabel(KeyCode.F4)).AppendLine(" 切换动作解锁射击");
			stringBuilder.Append("  • ").Append(ModBehaviour.FormatShortcutLabel(KeyCode.F5)).AppendLine(" 切换子弹穿透障碍");
			stringBuilder.Append("  • ").Append(ModBehaviour.FormatShortcutLabel(KeyCode.F6)).AppendLine(" 切换主角无敌");
			stringBuilder.Append("  • ").Append(ModBehaviour.FormatShortcutLabel(KeyCode.F8)).AppendLine(" 切换菜单");
			stringBuilder.Append("  • ").Append(ModBehaviour.FormatShortcutLabel(KeyCode.F9)).AppendLine(" 切换自瞄范围显示");
			this._shortcutsText = stringBuilder.ToString();
			if (this._shortcutsContent != null)
			{
				this._shortcutsContent.SetText(this._shortcutsText, true);
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00004B2C File Offset: 0x00002D2C
		private void SetMenuVisibility(bool visible)
		{
			if (visible)
			{
				this.EnsureOverlayUI();
				if (this._overlayCanvasObject != null && !this._overlayCanvasObject.activeSelf)
				{
					this._overlayCanvasObject.SetActive(true);
				}
				if (this._menuPanel != null && !this._menuPanel.gameObject.activeSelf)
				{
					this._menuPanel.gameObject.SetActive(true);
					return;
				}
			}
			else if (this._menuPanel != null && this._menuPanel.gameObject.activeSelf)
			{
				this._menuPanel.gameObject.SetActive(false);
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00004BCC File Offset: 0x00002DCC
		private void EnsureAutoAimRangeRenderer()
		{
			this.EnsureOverlayCanvas();
			GameObject overlayCanvasObject = this._overlayCanvasObject;
			if (overlayCanvasObject == null)
			{
				return;
			}
			if (this._autoAimRangeRect == null)
			{
				GameObject gameObject = new GameObject("CheatMenuAutoAimRange", new Type[] { typeof(RectTransform) });
				gameObject.transform.SetParent(overlayCanvasObject.transform, false);
				RectTransform component = gameObject.GetComponent<RectTransform>();
				component.anchorMin = new Vector2(0.5f, 0.5f);
				component.anchorMax = new Vector2(0.5f, 0.5f);
				component.pivot = new Vector2(0.5f, 0.5f);
				component.sizeDelta = Vector2.zero;
				this._autoAimRangeRect = component;
				LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
				lineRenderer.loop = true;
				lineRenderer.useWorldSpace = false;
				lineRenderer.textureMode = LineTextureMode.Stretch;
				lineRenderer.numCornerVertices = 8;
				lineRenderer.numCapVertices = 0;
				lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
				lineRenderer.receiveShadows = false;
				lineRenderer.startColor = new Color(0.2f, 0.75f, 1f, 0.9f);
				lineRenderer.endColor = new Color(0.2f, 0.75f, 1f, 0.45f);
				lineRenderer.widthMultiplier = 3.5f;
				lineRenderer.positionCount = 64;
				this._autoAimRangeRenderer = lineRenderer;
			}
			if (this._autoAimRangeRenderer == null)
			{
				return;
			}
			ModBehaviour.ConfigureLineRendererForOverlay(this._autoAimRangeRenderer);
			if (this._autoAimRangeMaterial == null)
			{
				Shader shader = Shader.Find("UI/Default");
				if (shader == null)
				{
					shader = Shader.Find("Sprites/Default");
				}
				if (shader == null)
				{
					shader = Shader.Find("Hidden/Internal-Colored");
				}
				if (shader != null)
				{
					this._autoAimRangeMaterial = new Material(shader)
					{
						color = new Color(0.2f, 0.75f, 1f, 0.85f),
						renderQueue = 4000
					};
					ModBehaviour.ConfigureOverlayMaterial(this._autoAimRangeMaterial);
				}
			}
			if (this._autoAimRangeMaterial != null)
			{
				ModBehaviour.ConfigureOverlayMaterial(this._autoAimRangeMaterial);
				if (this._autoAimRangeRenderer.material != this._autoAimRangeMaterial)
				{
					this._autoAimRangeRenderer.material = this._autoAimRangeMaterial;
				}
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00004E00 File Offset: 0x00003000
		private static void ConfigureOverlayMaterial(Material material)
		{
			if (material == null)
			{
				return;
			}
			try
			{
				material.renderQueue = 4000;
				if (material.HasProperty("_ZWrite"))
				{
					material.SetInt("_ZWrite", 0);
				}
				if (material.HasProperty("_ZTest"))
				{
					material.SetInt("_ZTest", 8);
				}
				if (material.HasProperty("unity_GUIZTestMode"))
				{
					material.SetInt("unity_GUIZTestMode", 8);
				}
				if (material.HasProperty("_SrcBlend"))
				{
					material.SetInt("_SrcBlend", 5);
				}
				if (material.HasProperty("_DstBlend"))
				{
					material.SetInt("_DstBlend", 10);
				}
				if (material.HasProperty("_Cull"))
				{
					material.SetInt("_Cull", 0);
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00004ED0 File Offset: 0x000030D0
		private static void ConfigureLineRendererForOverlay(LineRenderer renderer)
		{
			if (renderer == null)
			{
				return;
			}
			renderer.useWorldSpace = false;
			renderer.allowOcclusionWhenDynamic = false;
			renderer.sortingOrder = 4096;
			renderer.alignment = LineAlignment.Local;
			renderer.numCapVertices = 8;
			renderer.numCornerVertices = 8;
			renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
			renderer.lightProbeUsage = LightProbeUsage.Off;
			renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			renderer.forceRenderingOff = false;
			renderer.shadowCastingMode = ShadowCastingMode.Off;
			renderer.receiveShadows = false;
			try
			{
				renderer.sortingLayerName = "UI";
			}
			catch (ArgumentException)
			{
				renderer.sortingLayerID = 0;
			}
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00004F68 File Offset: 0x00003168
		private void UpdateAutoAimRangeVisualization(Camera camera, Vector2 aimScreenPoint, float planeHeight)
		{
			this.EnsureAutoAimCircleTexture();
			this._autoAimCircleVisible = true;
			this._autoAimCircleCenter = aimScreenPoint;
			this._autoAimCircleRadiusPixels = this._autoAimScreenRadius;
			Vector3 vector = ModBehaviour.EvaluateAimPlaneIntersection(camera.ScreenPointToRay(new Vector3(aimScreenPoint.x, aimScreenPoint.y, 0f)), planeHeight);
			float num = 0f;
			for (int i = 0; i < 64; i++)
			{
				float num2 = 6.2831855f * (float)i / 64f;
				float num3 = Mathf.Cos(num2) * this._autoAimScreenRadius;
				float num4 = Mathf.Sin(num2) * this._autoAimScreenRadius;
				Vector3 vector2 = ModBehaviour.EvaluateAimPlaneIntersection(camera.ScreenPointToRay(new Vector3(aimScreenPoint.x + num3, aimScreenPoint.y + num4, 0f)), planeHeight);
				this._autoAimRangePoints[i] = vector2;
				num += Vector3.Distance(vector, vector2);
			}
			this._lastAutoAimWorldRadius = num / 64f;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00005044 File Offset: 0x00003244
		private void HideAutoAimRange()
		{
			this._autoAimCircleVisible = false;
			if (this._autoAimRangeRenderer != null && this._autoAimRangeRenderer.enabled)
			{
				this._autoAimRangeRenderer.enabled = false;
			}
			if (this._autoAimRangeRect != null && this._autoAimRangeRect.gameObject.activeSelf)
			{
				this._autoAimRangeRect.gameObject.SetActive(false);
			}
			this._lastAutoAimWorldRadius = 0f;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000050BC File Offset: 0x000032BC
		private void DestroyOverlayResources()
		{
			if (this._menuPanel != null)
			{
				Object.Destroy(this._menuPanel.gameObject);
				this._menuPanel = null;
			}
			this._menuRoot = null;
			this._sidebarContainer = null;
			this._contentContainer = null;
			this._functionToggleContent = null;
			this._autoAimContent = null;
			this._weaponInfoContent = null;
			this._shortcutsContent = null;
			this._multipliersSummary = null;
			this._menuSectionButtons.Clear();
			this._menuSectionRoots.Clear();
			this._activeMenuSection = ModBehaviour.MenuSection.FunctionToggles;
			this._sliderContainer = null;
			this._rangeSlider = null;
			this._fireRateSlider = null;
			this._damageSlider = null;
			this._rangeValueText = null;
			this._fireRateValueText = null;
			this._damageValueText = null;
			if (this._overlayCanvasObject != null)
			{
				Object.Destroy(this._overlayCanvasObject);
				this._overlayCanvasObject = null;
			}
			if (this._autoAimRangeRenderer != null)
			{
				Object.Destroy(this._autoAimRangeRenderer.gameObject);
				this._autoAimRangeRenderer = null;
				this._autoAimRangeRect = null;
			}
			if (this._autoAimRangeMaterial != null)
			{
				Object.Destroy(this._autoAimRangeMaterial);
				this._autoAimRangeMaterial = null;
			}
			if (this._autoAimCircleTexture != null)
			{
				Object.Destroy(this._autoAimCircleTexture);
				this._autoAimCircleTexture = null;
			}
			this._autoAimCircleVisible = false;
			this._menuWindowInitialized = false;
			this._lastAutoAimWorldRadius = 0f;
			this._headshotOffsets.Clear();
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00005224 File Offset: 0x00003424
		private void OnGUI()
		{
			this.DrawAutoAimCircleOverlay();
			if (!this._menuVisible)
			{
				return;
			}
			if (Event.current.type == 8)
			{
				this.InitializeMenuWindowIfNeeded();
			}
			bool richText = GUI.skin.label.richText;
			bool richText2 = GUI.skin.box.richText;
			GUI.skin.label.richText = true;
			GUI.skin.box.richText = true;
			this._menuWindowRect = GUILayout.Window(this._menuWindowId, this._menuWindowRect, new GUI.WindowFunction(this.DrawMenuWindowContents), "cheatmenu 控制面板", Array.Empty<GUILayoutOption>());
			GUI.skin.label.richText = richText;
			GUI.skin.box.richText = richText2;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000052E0 File Offset: 0x000034E0
		private void DrawAutoAimCircleOverlay()
		{
			if (!this._autoAimCircleVisible || Event.current.type != 7)
			{
				return;
			}
			if (this._autoAimCircleTexture == null)
			{
				this.EnsureAutoAimCircleTexture();
				if (this._autoAimCircleTexture == null)
				{
					return;
				}
			}
			float autoAimCircleRadiusPixels = this._autoAimCircleRadiusPixels;
			float num = autoAimCircleRadiusPixels * 2f;
			float num2 = (float)Screen.height - this._autoAimCircleCenter.y;
			Rect rect = new Rect(this._autoAimCircleCenter.x - autoAimCircleRadiusPixels, num2 - autoAimCircleRadiusPixels, num, num);
			Color color = GUI.color;
			GUI.color = new Color(0.2f, 0.75f, 1f, 0.8f);
			GUI.DrawTexture(rect, this._autoAimCircleTexture);
			GUI.color = color;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00005394 File Offset: 0x00003594
		private void DrawMenuWindowContents(int windowId)
		{
			GUILayout.Space(4f);
			this._menuScrollPosition = GUILayout.BeginScrollView(this._menuScrollPosition, false, true, Array.Empty<GUILayoutOption>());
			CharacterMainControl trackedCharacter = this._trackedCharacter;
			if (trackedCharacter != null)
			{
				this.RefreshMenuContent(trackedCharacter, this._latestAimScreenPoint);
			}
			this.DrawFunctionSection();
			GUILayout.Space(6f);
			this.DrawMultipliersSection();
			GUILayout.Space(6f);
			this.DrawAutoAimInfoSection();
			GUILayout.Space(6f);
			this.DrawWeaponInfoSection();
			GUILayout.Space(6f);
			this.DrawShortcutsSection();
			GUILayout.EndScrollView();
			GUI.DragWindow(new Rect(0f, 0f, this._menuWindowRect.width, 24f));
		}

		// Token: 0x0600005C RID: 92 RVA: 0x0000544F File Offset: 0x0000364F
		private bool DrawSectionHeader(string title, bool expanded)
		{
			if (GUILayout.Button((expanded ? "[-] " : "[+] ") + title, new GUILayoutOption[] { GUILayout.Height(28f) }))
			{
				expanded = !expanded;
			}
			return expanded;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00005488 File Offset: 0x00003688
		private void DrawFunctionSection()
		{
			this._functionSectionExpanded = this.DrawSectionHeader("功能开关", this._functionSectionExpanded);
			if (!this._functionSectionExpanded)
			{
				return;
			}
			GUILayout.BeginVertical(GUI.skin.box, Array.Empty<GUILayoutOption>());
			bool flag = GUILayout.Toggle(this._autoAimEnabled, "启用自瞄 (" + ModBehaviour.FormatShortcutLabel(KeyCode.F1) + ")", Array.Empty<GUILayoutOption>());
			if (flag != this._autoAimEnabled)
			{
				this.SetAutoAimEnabled(flag);
			}
			bool flag2 = GUILayout.Toggle(this._rangeVisible, "显示自瞄范围 (" + ModBehaviour.FormatShortcutLabel(KeyCode.F9) + ")", Array.Empty<GUILayoutOption>());
			if (flag2 != this._rangeVisible)
			{
				this.SetRangeVisible(flag2);
			}
			bool flag3 = GUILayout.Toggle(this._noRecoilEnabled, "无后坐力 (" + ModBehaviour.FormatShortcutLabel(KeyCode.F2) + ")", Array.Empty<GUILayoutOption>());
			if (flag3 != this._noRecoilEnabled)
			{
				this.SetNoRecoilEnabled(flag3);
			}
			bool flag4 = GUILayout.Toggle(this._infiniteAmmoEnabled, "无限弹药 (" + ModBehaviour.FormatShortcutLabel(KeyCode.F3) + ")", Array.Empty<GUILayoutOption>());
			if (flag4 != this._infiniteAmmoEnabled)
			{
				this.SetInfiniteAmmoEnabled(flag4);
			}
			bool flag5 = GUILayout.Toggle(this._movementUnlockEnabled, "动作解锁射击 (" + ModBehaviour.FormatShortcutLabel(KeyCode.F4) + ")", Array.Empty<GUILayoutOption>());
			if (flag5 != this._movementUnlockEnabled)
			{
				this.SetMovementUnlockEnabled(flag5);
			}
			bool flag6 = GUILayout.Toggle(this._obstaclePenetrationEnabled, "子弹穿透障碍 (" + ModBehaviour.FormatShortcutLabel(KeyCode.F5) + ")", Array.Empty<GUILayoutOption>());
			if (flag6 != this._obstaclePenetrationEnabled)
			{
				this.SetObstaclePenetrationEnabled(flag6);
			}
			bool flag7 = GUILayout.Toggle(this._invincibilityEnabled, "主角无敌 (" + ModBehaviour.FormatShortcutLabel(KeyCode.F6) + ")", Array.Empty<GUILayoutOption>());
			if (flag7 != this._invincibilityEnabled)
			{
				this.SetInvincibilityEnabled(flag7);
			}
			GUILayout.EndVertical();
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00005670 File Offset: 0x00003870
		private void DrawMultipliersSection()
		{
			this._multipliersSectionExpanded = this.DrawSectionHeader("数值倍率", this._multipliersSectionExpanded);
			if (!this._multipliersSectionExpanded)
			{
				return;
			}
			GUILayout.BeginVertical(GUI.skin.box, Array.Empty<GUILayoutOption>());
			this.DrawMultiplierSliderRow("射程倍率", () => this._rangeMultiplier, new Action<float>(this.SetRangeMultiplierFromSlider));
			this.DrawMultiplierSliderRow("射速倍率", () => this._fireRateMultiplier, new Action<float>(this.SetFireRateMultiplierFromSlider));
			this.DrawMultiplierSliderRow("子弹速度倍率", () => this._bulletSpeedMultiplier, new Action<float>(this.SetBulletSpeedMultiplierFromSlider));
			this.DrawMultiplierSliderRow("伤害倍率", () => this._damageMultiplier, new Action<float>(this.SetDamageMultiplierFromSlider));
			GUILayout.EndVertical();
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00005744 File Offset: 0x00003944
		private void DrawAutoAimInfoSection()
		{
			this._autoAimSectionExpanded = this.DrawSectionHeader("自瞄参数", this._autoAimSectionExpanded);
			if (!this._autoAimSectionExpanded)
			{
				return;
			}
			GUILayout.BeginVertical(GUI.skin.box, Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("屏幕半径", new GUILayoutOption[] { GUILayout.Width(132f) });
			float num = GUILayout.HorizontalSlider(this._autoAimScreenRadius, 60f, 2048f, Array.Empty<GUILayoutOption>());
			if (Mathf.Abs(num - this._autoAimScreenRadius) > 0.001f)
			{
				this.SetAutoAimScreenRadius(num);
			}
			GUILayout.Space(8f);
			GUILayout.Label(string.Format("{0:F0} px", this._autoAimScreenRadius), new GUILayoutOption[] { GUILayout.Width(80f) });
			GUILayout.EndHorizontal();
			GUILayout.Space(6f);
			if (!string.IsNullOrEmpty(this._autoAimContentText))
			{
				GUILayout.Label(this._autoAimContentText, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
			}
			else
			{
				GUILayout.Label("暂无自瞄信息", new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
			}
			GUILayout.EndVertical();
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00005870 File Offset: 0x00003A70
		private void DrawWeaponInfoSection()
		{
			this._weaponSectionExpanded = this.DrawSectionHeader("武器信息", this._weaponSectionExpanded);
			if (!this._weaponSectionExpanded)
			{
				return;
			}
			GUILayout.BeginVertical(GUI.skin.box, Array.Empty<GUILayoutOption>());
			if (!string.IsNullOrEmpty(this._weaponInfoText))
			{
				GUILayout.Label(this._weaponInfoText, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
			}
			else
			{
				GUILayout.Label("暂无武器信息", new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
			}
			GUILayout.EndVertical();
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000058F8 File Offset: 0x00003AF8
		private void DrawShortcutsSection()
		{
			this._shortcutsSectionExpanded = this.DrawSectionHeader("快捷键", this._shortcutsSectionExpanded);
			if (!this._shortcutsSectionExpanded)
			{
				return;
			}
			GUILayout.BeginVertical(GUI.skin.box, Array.Empty<GUILayoutOption>());
			if (!string.IsNullOrEmpty(this._shortcutsText))
			{
				GUILayout.Label(this._shortcutsText, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
			}
			else
			{
				GUILayout.Label("暂无快捷键信息", new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
			}
			GUILayout.EndVertical();
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00005980 File Offset: 0x00003B80
		private void DrawMultiplierSliderRow(string label, Func<float> getter, Action<float> setter)
		{
			float num = getter();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(label, new GUILayoutOption[] { GUILayout.Width(132f) });
			float num2 = GUILayout.HorizontalSlider(num, 1f, 10f, Array.Empty<GUILayoutOption>());
			if (Mathf.Abs(num2 - num) > 0.0001f)
			{
				setter(num2);
				num = getter();
			}
			GUILayout.Space(8f);
			GUILayout.Label(ModBehaviour.FormatMultiplierValue(num), new GUILayoutOption[] { GUILayout.Width(64f) });
			GUILayout.EndHorizontal();
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00005A17 File Offset: 0x00003C17
		private static string FormatToggleStatus(bool enabled)
		{
			if (!enabled)
			{
				return "<color=#FF6666>关闭</color>";
			}
			return "<color=#5CFF5C>开启</color>";
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00005A28 File Offset: 0x00003C28
		private void ResetGunInternals(ItemAgent_Gun gun)
		{
			ModBehaviour.TrySetFieldValue(ModBehaviour.ScatterBeforeControlField, gun, 0f);
			ModBehaviour.TrySetFieldValue(ModBehaviour.ScatterFactorHipsField, gun, 0f);
			ModBehaviour.TrySetFieldValue(ModBehaviour.ScatterFactorAdsField, gun, 0f);
			ModBehaviour.TrySetFieldValue(ModBehaviour.RecoilMoveValueField, gun, 0f);
			ModBehaviour.TrySetFieldValue(ModBehaviour.RecoilBackField, gun, false);
			ModBehaviour.TrySetFieldValue(ModBehaviour.RecoilDistanceField, gun, 0f);
			ModBehaviour.TrySetFieldValue(ModBehaviour.RecoilBackSpeedField, gun, 0f);
			ModBehaviour.TrySetFieldValue(ModBehaviour.RecoilRecoverSpeedField, gun, 0f);
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00005ADC File Offset: 0x00003CDC
		private void RestoreGunAmmo(ItemAgent_Gun gun)
		{
			ItemSetting_Gun gunItemSetting = gun.GunItemSetting;
			if (gunItemSetting == null)
			{
				return;
			}
			int bulletCount = gunItemSetting.BulletCount;
			int num = Mathf.Max(0, gunItemSetting.Capacity);
			int num2 = bulletCount + 1;
			if (num > 0)
			{
				num2 = Mathf.Clamp(num2, 0, num);
			}
			this.ApplyBulletCount(gunItemSetting, num2);
			this.EnsureMagazineStack(gunItemSetting, num2);
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00005B30 File Offset: 0x00003D30
		private void ApplyBulletCount(ItemSetting_Gun setting, int desiredCount)
		{
			if (ModBehaviour.GunBulletCountProperty == null)
			{
				return;
			}
			try
			{
				ModBehaviour.GunBulletCountProperty.SetValue(setting, desiredCount, null);
			}
			catch (Exception)
			{
			}
			bool infiniteAmmoEnabled = this._infiniteAmmoEnabled;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00005B7C File Offset: 0x00003D7C
		private void EnsureMagazineStack(ItemSetting_Gun setting, int desiredCount)
		{
			Inventory inventory = ((setting.Item != null) ? setting.Item.Inventory : null);
			if (inventory == null)
			{
				return;
			}
			Item item = null;
			foreach (Item item2 in inventory)
			{
				if (item2 != null)
				{
					item = item2;
					break;
				}
			}
			if (item == null)
			{
				return;
			}
			if (item.StackCount < desiredCount)
			{
				item.StackCount = desiredCount;
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00005C10 File Offset: 0x00003E10
		private void TryAutoAimShot(ItemAgent_Gun gun)
		{
			if (!this._autoAimEnabled)
			{
				return;
			}
			if (gun == null)
			{
				return;
			}
			this.CleanupRetargetedProjectiles();
			this._applyPenetrationThisShot = false;
			CharacterMainControl holder = gun.Holder;
			if (holder == null)
			{
				ModBehaviour.AutoAimLog("Skipped auto-aim: gun holder missing");
				return;
			}
			if (!LevelManager.LevelInited || LevelManager.Instance == null)
			{
				ModBehaviour.AutoAimLog("Skipped auto-aim: level not initialized");
				return;
			}
			GameCamera gameCamera = LevelManager.Instance.GameCamera;
			if (gameCamera == null || gameCamera.renderCamera == null)
			{
				ModBehaviour.AutoAimLog("Skipped auto-aim: camera unavailable");
				return;
			}
			Camera renderCamera = gameCamera.renderCamera;
			InputManager inputManager = LevelManager.Instance.InputManager;
			Vector2 vector = ((inputManager != null) ? inputManager.AimScreenPoint : new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f));
			Ray ray = renderCamera.ScreenPointToRay(new Vector3(vector.x, vector.y, 0f));
			Vector3 vector2 = ((gun.muzzle != null) ? gun.muzzle.position : gun.transform.position);
			ModBehaviour.AutoAimCandidate autoAimCandidate;
			if (!this.TryFindAutoAimTarget(gun, holder.Team, ray, vector, vector2, holder.transform.position.y + 0.5f, out autoAimCandidate))
			{
				this.SetPenetrationActive(false);
				this._applyPenetrationThisShot = false;
				ModBehaviour.AutoAimLog(string.Format("No auto-aim target found for {0} (screenPoint={1})", gun.name, vector));
				return;
			}
			bool requiresPenetration = autoAimCandidate.RequiresPenetration;
			bool flag = this._obstaclePenetrationEnabled && (requiresPenetration || this._penetratingProjectiles.Count > 0);
			this.SetPenetrationActive(flag);
			this._applyPenetrationThisShot = this._obstaclePenetrationEnabled && requiresPenetration;
			if (this._obstaclePenetrationEnabled)
			{
				if (requiresPenetration)
				{
					string text = "Penetration enabled for occluded target ";
					DamageReceiver receiver = autoAimCandidate.Receiver;
					ModBehaviour.AutoAimLog(text + (((receiver != null) ? receiver.name : null) ?? "?"));
				}
				else
				{
					ModBehaviour.AutoAimLog("Target has clear line of sight; penetration not required this shot");
				}
			}
			Projectile projectile = this.RetargetImmediateProjectile(gun, vector2, autoAimCandidate.AimPoint);
			if (projectile == null)
			{
				this.CleanupRetargetedProjectiles();
				projectile = this.ApplyAutoAimToProjectiles(gun, vector2, autoAimCandidate.AimPoint);
			}
			if (projectile == null)
			{
				ModBehaviour.AutoAimLog(string.Format("No projectiles retargeted for {0} toward {1}", gun.name, autoAimCandidate.AimPoint));
				return;
			}
			string text2 = "Auto-aim applied: gun={0}, target={1}, aimPoint={2}, screenDist={3:F1}, rayDist={4:F1}";
			object[] array = new object[5];
			array[0] = gun.name;
			int num = 1;
			DamageReceiver receiver2 = autoAimCandidate.Receiver;
			array[num] = ((receiver2 != null) ? receiver2.name : null) ?? "?";
			array[2] = autoAimCandidate.AimPoint;
			array[3] = autoAimCandidate.ScreenDistance;
			array[4] = autoAimCandidate.RayDistance;
			ModBehaviour.AutoAimLog(string.Format(text2, array));
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00005ED0 File Offset: 0x000040D0
		private bool TryFindAutoAimTarget(ItemAgent_Gun gun, Teams playerTeam, Ray aimRay, Vector2 aimScreenPoint, Vector3 muzzlePosition, float aimPlaneHeight, out ModBehaviour.AutoAimCandidate candidate)
		{
			candidate = default(ModBehaviour.AutoAimCandidate);
			LevelManager instance = LevelManager.Instance;
			Camera camera;
			if (instance == null)
			{
				camera = null;
			}
			else
			{
				GameCamera gameCamera = instance.GameCamera;
				camera = ((gameCamera != null) ? gameCamera.renderCamera : null);
			}
			Camera camera2 = camera;
			if (camera2 == null)
			{
				ModBehaviour.AutoAimLog("Skipping target scan: render camera unavailable");
				return false;
			}
			DamageReceiver[] array = Object.FindObjectsOfType<DamageReceiver>();
			if (array == null || array.Length == 0)
			{
				ModBehaviour.AutoAimLog("No damage receivers found when scanning for targets");
				return false;
			}
			ModBehaviour.AutoAimLog(string.Format("Scanning {0} damage receivers for auto-aim", array.Length));
			bool flag = false;
			bool flag2 = false;
			float maxValue = float.MaxValue;
			Vector3 origin = aimRay.origin;
			Vector3 normalized = aimRay.direction.normalized;
			float num = Mathf.Max(2f, gun.BulletDistance);
			HashSet<int> hashSet = new HashSet<int>();
			foreach (DamageReceiver damageReceiver in array)
			{
				Vector3 vector;
				if (!(damageReceiver == null) && damageReceiver.enabled && !(damageReceiver.gameObject == null) && damageReceiver.gameObject.activeInHierarchy && this.IsCandidateValid(playerTeam, damageReceiver, out vector))
				{
					hashSet.Add(damageReceiver.GetInstanceID());
					if (Vector3.Distance(muzzlePosition, vector) <= num)
					{
						float num2 = Vector3.Dot(vector - origin, normalized);
						if (num2 > 0f)
						{
							float num3 = num2;
							Vector3 vector2 = camera2.WorldToScreenPoint(vector);
							if (vector2.z > 0f)
							{
								float num4 = Vector2.Distance(new Vector2(vector2.x, vector2.y), aimScreenPoint);
								if (num4 <= this._autoAimScreenRadius)
								{
									flag2 = true;
									if (this.EvaluateCandidate(gun, muzzlePosition, vector, damageReceiver, num3, num4, ref candidate, ref maxValue))
									{
										flag = true;
									}
								}
							}
						}
					}
				}
			}
			if (!flag2)
			{
				ModBehaviour.AutoAimLog("No enemy inside auto-aim circle; skipping auto-aim");
				candidate = default(ModBehaviour.AutoAimCandidate);
				this.CleanupUnusedHeadshotOffsets(hashSet);
				return false;
			}
			if (!flag)
			{
				ModBehaviour.AutoAimLog("No suitable enemy found after full scan");
			}
			this.CleanupUnusedHeadshotOffsets(hashSet);
			return flag;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000060C0 File Offset: 0x000042C0
		private void CleanupUnusedHeadshotOffsets(HashSet<int> seenReceivers)
		{
			if (this._headshotOffsets.Count == 0)
			{
				return;
			}
			List<int> list = null;
			foreach (int num in this._headshotOffsets.Keys)
			{
				if (!seenReceivers.Contains(num))
				{
					if (list == null)
					{
						list = new List<int>();
					}
					list.Add(num);
				}
			}
			if (list != null)
			{
				foreach (int num2 in list)
				{
					this._headshotOffsets.Remove(num2);
				}
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00006184 File Offset: 0x00004384
		private bool EvaluateCandidate(ItemAgent_Gun gun, Vector3 muzzlePosition, Vector3 aimPoint, [Nullable(2)] DamageReceiver receiver, float rayDistance, float screenDistance, ref ModBehaviour.AutoAimCandidate candidate, ref float bestScore)
		{
			if (receiver == null)
			{
				return false;
			}
			Vector3 vector = aimPoint - muzzlePosition;
			if (vector.sqrMagnitude < 0.04f)
			{
				return false;
			}
			float num;
			bool flag = this.HasLineOfSight(muzzlePosition, aimPoint, receiver, out num);
			if (!flag && !this._obstaclePenetrationEnabled)
			{
				return false;
			}
			float num2 = Vector3.Angle((gun.muzzle != null) ? gun.muzzle.forward : gun.transform.forward, vector);
			float num3 = screenDistance + num2 * 2.25f + Mathf.Max(0f, rayDistance) * 0.1f;
			if (!flag)
			{
				float num4 = 35f + Mathf.Max(0f, num) * 1.75f;
				num3 += num4;
			}
			if (num3 < bestScore)
			{
				bool flag2 = !flag;
				candidate = new ModBehaviour.AutoAimCandidate(receiver, aimPoint, screenDistance, rayDistance, flag2);
				if (flag2)
				{
					ModBehaviour.AutoAimLog(string.Format("Candidate updated (penetration): receiver={0}, screenDist={1:F1}, angle={2:F1}, score={3:F1}, obstacle={4:F2}", new object[] { receiver.name, screenDistance, num2, num3, num }));
				}
				else
				{
					ModBehaviour.AutoAimLog(string.Format("Candidate updated: receiver={0}, screenDist={1:F1}, angle={2:F1}, score={3:F1}", new object[] { receiver.name, screenDistance, num2, num3 }));
				}
				bestScore = num3;
				return true;
			}
			return false;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x000062F4 File Offset: 0x000044F4
		private bool HasLineOfSight(Vector3 origin, Vector3 target, DamageReceiver receiver, out float obstructionDistance)
		{
			obstructionDistance = 0f;
			Vector3 vector = target - origin;
			float magnitude = vector.magnitude;
			if (magnitude <= 0.2f)
			{
				return true;
			}
			vector.Normalize();
			RaycastHit raycastHit;
			if (!Physics.Raycast(origin, vector, out raycastHit, magnitude - 0.2f, ModBehaviour.ObstacleLayerMaskValue, QueryTriggerInteraction.Ignore))
			{
				return true;
			}
			DamageReceiver componentInParent = raycastHit.collider.GetComponentInParent<DamageReceiver>();
			if (componentInParent != null && componentInParent == receiver)
			{
				return true;
			}
			obstructionDistance = raycastHit.distance;
			return false;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00006370 File Offset: 0x00004570
		[NullableContext(2)]
		private bool IsCandidateValid(Teams playerTeam, DamageReceiver receiver, out Vector3 aimPoint)
		{
			aimPoint = Vector3.zero;
			if (receiver == null)
			{
				return false;
			}
			if (receiver.IsDead || receiver.IsMainCharacter || !ModBehaviour.ReceiverHasHealth(receiver))
			{
				return false;
			}
			if (!Team.IsEnemy(playerTeam, receiver.Team))
			{
				return false;
			}
			if (receiver.Team == Teams.all)
			{
				return false;
			}
			aimPoint = this.GetReceiverAimPoint(receiver);
			return true;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x000063D8 File Offset: 0x000045D8
		[NullableContext(2)]
		private void ApplyInvincibility(CharacterMainControl character)
		{
			if (!this._invincibilityEnabled)
			{
				this.RestoreCharacterInvincibility();
				return;
			}
			if (character == null)
			{
				return;
			}
			Health health = character.Health;
			if (health == null)
			{
				return;
			}
			if (this._invincibilityTargetHealth != null && this._invincibilityTargetHealth != health)
			{
				this.RestoreCharacterInvincibility();
			}
			if (this._originalCharacterInvincible == null)
			{
				this._originalCharacterInvincible = new bool?(health.Invincible);
			}
			if (!health.Invincible)
			{
				health.SetInvincible(true);
			}
			this._invincibilityTargetHealth = health;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00006468 File Offset: 0x00004668
		private void RestoreCharacterInvincibility()
		{
			if (this._invincibilityTargetHealth != null && this._originalCharacterInvincible != null)
			{
				this._invincibilityTargetHealth.SetInvincible(this._originalCharacterInvincible.Value);
			}
			this._invincibilityTargetHealth = null;
			this._originalCharacterInvincible = null;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000064BC File Offset: 0x000046BC
		private void LoadSettings()
		{
			this._autoAimEnabled = ModBehaviour.LoadBoolSetting("AutoAimEnabled", this._autoAimEnabled);
			this._rangeVisible = ModBehaviour.LoadBoolSetting("RangeVisible", this._rangeVisible);
			this._noRecoilEnabled = ModBehaviour.LoadBoolSetting("NoRecoilEnabled", this._noRecoilEnabled);
			this._infiniteAmmoEnabled = ModBehaviour.LoadBoolSetting("InfiniteAmmoEnabled", this._infiniteAmmoEnabled);
			this._movementUnlockEnabled = ModBehaviour.LoadBoolSetting("MovementUnlockEnabled", this._movementUnlockEnabled);
			this._obstaclePenetrationEnabled = ModBehaviour.LoadBoolSetting("ObstaclePenetrationEnabled", this._obstaclePenetrationEnabled);
			this._invincibilityEnabled = ModBehaviour.LoadBoolSetting("InvincibilityEnabled", this._invincibilityEnabled);
			this._rangeMultiplier = ModBehaviour.LoadFloatSetting("RangeMultiplier", this._rangeMultiplier, 1f, 10f);
			this._fireRateMultiplier = ModBehaviour.LoadFloatSetting("FireRateMultiplier", this._fireRateMultiplier, 1f, 10f);
			this._bulletSpeedMultiplier = ModBehaviour.LoadFloatSetting("BulletSpeedMultiplier", this._bulletSpeedMultiplier, 1f, 10f);
			this._damageMultiplier = ModBehaviour.LoadFloatSetting("DamageMultiplier", this._damageMultiplier, 1f, 10f);
			this._autoAimScreenRadius = ModBehaviour.LoadFloatSetting("AutoAimScreenRadius", this._autoAimScreenRadius, 60f, 2048f);
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00006604 File Offset: 0x00004804
		private void SaveSettings()
		{
			ModBehaviour.SaveBoolSetting("AutoAimEnabled", this._autoAimEnabled);
			ModBehaviour.SaveBoolSetting("RangeVisible", this._rangeVisible);
			ModBehaviour.SaveBoolSetting("NoRecoilEnabled", this._noRecoilEnabled);
			ModBehaviour.SaveBoolSetting("InfiniteAmmoEnabled", this._infiniteAmmoEnabled);
			ModBehaviour.SaveBoolSetting("MovementUnlockEnabled", this._movementUnlockEnabled);
			ModBehaviour.SaveBoolSetting("ObstaclePenetrationEnabled", this._obstaclePenetrationEnabled);
			ModBehaviour.SaveBoolSetting("InvincibilityEnabled", this._invincibilityEnabled);
			ModBehaviour.SaveFloatSetting("RangeMultiplier", this._rangeMultiplier);
			ModBehaviour.SaveFloatSetting("FireRateMultiplier", this._fireRateMultiplier);
			ModBehaviour.SaveFloatSetting("BulletSpeedMultiplier", this._bulletSpeedMultiplier);
			ModBehaviour.SaveFloatSetting("DamageMultiplier", this._damageMultiplier);
			ModBehaviour.SaveFloatSetting("AutoAimScreenRadius", this._autoAimScreenRadius);
			PlayerPrefs.Save();
		}

		// Token: 0x06000072 RID: 114 RVA: 0x000066D6 File Offset: 0x000048D6
		private static bool ReceiverHasHealth(DamageReceiver receiver)
		{
			if (receiver.useSimpleHealth)
			{
				return receiver.simpleHealth != null;
			}
			return receiver.health != null;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000066FC File Offset: 0x000048FC
		private static bool LoadBoolSetting(string name, bool defaultValue)
		{
			int num = ((defaultValue > false) ? 1 : 0);
			return PlayerPrefs.GetInt(ModBehaviour.GetSettingKey(name), num) != 0;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x0000671D File Offset: 0x0000491D
		private static float LoadFloatSetting(string name, float defaultValue, float min, float max)
		{
			return Mathf.Clamp(PlayerPrefs.GetFloat(ModBehaviour.GetSettingKey(name), defaultValue), min, max);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00006732 File Offset: 0x00004932
		private static void SaveBoolSetting(string name, bool value)
		{
			PlayerPrefs.SetInt(ModBehaviour.GetSettingKey(name), (value > false) ? 1 : 0);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00006743 File Offset: 0x00004943
		private static void SaveFloatSetting(string name, float value)
		{
			PlayerPrefs.SetFloat(ModBehaviour.GetSettingKey(name), value);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00006751 File Offset: 0x00004951
		private static string GetSettingKey(string name)
		{
			return "CheatMenu." + name;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00006760 File Offset: 0x00004960
		private Vector3 GetReceiverAimPoint(DamageReceiver receiver)
		{
			ModBehaviour.HeadshotInfo headshotInfo = this.GetHeadshotInfo(receiver);
			Collider collider;
			if (receiver.TryGetComponent<Collider>(out collider))
			{
				return collider.bounds.center;
			}
			if (headshotInfo.HasHead)
			{
				return headshotInfo.HeadPosition;
			}
			return receiver.transform.position + Vector3.up * 0.5f;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000067BC File Offset: 0x000049BC
		private ModBehaviour.HeadshotInfo GetHeadshotInfo(DamageReceiver receiver)
		{
			Transform transform = ModBehaviour.TryGetHeadTransform(receiver);
			if (transform == null)
			{
				return default(ModBehaviour.HeadshotInfo);
			}
			int instanceID = receiver.GetInstanceID();
			Vector3 vector;
			if (!this._headshotOffsets.TryGetValue(instanceID, out vector))
			{
				vector = new Vector3(Random.Range(-0.03f, 0.03f), Random.Range(0.02f, 0.06f), Random.Range(-0.03f, 0.03f));
				this._headshotOffsets[instanceID] = vector;
			}
			return new ModBehaviour.HeadshotInfo
			{
				HasHead = true,
				HeadPosition = transform.position,
				RandomOffset = vector
			};
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00006864 File Offset: 0x00004A64
		[return: Nullable(2)]
		private static Transform TryGetHeadTransform(DamageReceiver receiver)
		{
			HeadCollider componentInChildren = receiver.GetComponentInChildren<HeadCollider>();
			if (componentInChildren != null)
			{
				return componentInChildren.transform;
			}
			if (receiver.health != null)
			{
				CharacterMainControl characterMainControl = receiver.health.TryGetCharacter();
				if (characterMainControl != null)
				{
					if (characterMainControl.characterModel != null)
					{
						if (characterMainControl.characterModel.HelmatSocket != null)
						{
							return characterMainControl.characterModel.HelmatSocket;
						}
						Transform faceMaskSocket = characterMainControl.characterModel.FaceMaskSocket;
						if (faceMaskSocket != null)
						{
							return faceMaskSocket;
						}
					}
					return characterMainControl.transform;
				}
			}
			if (receiver.simpleHealth != null)
			{
				return receiver.simpleHealth.transform;
			}
			return null;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00006914 File Offset: 0x00004B14
		private static Vector3 EvaluateAimPlaneIntersection(Ray aimRay, float planeHeight)
		{
			Plane plane = new Plane(Vector3.up, new Vector3(0f, planeHeight, 0f));
			float num;
			if (plane.Raycast(aimRay, out num))
			{
				return aimRay.origin + aimRay.direction * num;
			}
			return aimRay.origin + aimRay.direction.normalized * 5f;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00006988 File Offset: 0x00004B88
		[return: Nullable(2)]
		private Projectile RetargetImmediateProjectile(ItemAgent_Gun gun, Vector3 muzzlePosition, Vector3 targetPoint)
		{
			if (ModBehaviour.GunProjectileField == null)
			{
				return null;
			}
			Projectile projectile = ModBehaviour.GunProjectileField.GetValue(gun) as Projectile;
			if (projectile == null)
			{
				ModBehaviour.AutoAimLog("Immediate projectile not available via reflection");
				return null;
			}
			if (projectile.context.fromCharacter != gun.Holder)
			{
				ModBehaviour.AutoAimLog("Immediate projectile owner mismatch; skipping retarget");
				return null;
			}
			Vector3 vector = targetPoint - muzzlePosition;
			if (vector.sqrMagnitude < 0.0001f)
			{
				return null;
			}
			if (ModBehaviour.GetProjectileTraveledDistance(projectile) > 0.06f)
			{
				ModBehaviour.AutoAimLog("Immediate projectile already traveled; fallback to pool search");
				return null;
			}
			if (!this.TryRetargetProjectile(projectile, vector, targetPoint, true))
			{
				ModBehaviour.AutoAimLog("Failed to retarget immediate projectile");
				return null;
			}
			this._retargetedProjectiles.Add(projectile.GetInstanceID());
			ModBehaviour.AutoAimLog(string.Format("Retargeted immediate projectile {0} toward {1}", projectile.name, targetPoint));
			return projectile;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00006A68 File Offset: 0x00004C68
		[return: Nullable(2)]
		private Projectile ApplyAutoAimToProjectiles(ItemAgent_Gun gun, Vector3 muzzlePosition, Vector3 targetPoint)
		{
			Projectile[] array = Object.FindObjectsOfType<Projectile>();
			if (array == null || array.Length == 0)
			{
				return null;
			}
			Vector3 vector = targetPoint - muzzlePosition;
			if (vector.sqrMagnitude < 0.0001f)
			{
				return null;
			}
			vector.Normalize();
			int num = Mathf.Max(1, gun.ShotCount);
			int num2 = 0;
			Projectile projectile = null;
			IEnumerable<Projectile> enumerable = array;
			Func<Projectile, bool> <>9__0;
			Func<Projectile, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (Projectile p) => p != null && p.context.fromCharacter == gun.Holder);
			}
			foreach (Projectile projectile2 in enumerable.Where(func).OrderBy(new Func<Projectile, float>(ModBehaviour.GetProjectileTraveledDistance)))
			{
				if (num2 >= num)
				{
					break;
				}
				int instanceID = projectile2.GetInstanceID();
				if (!this._retargetedProjectiles.Contains(instanceID) && ModBehaviour.GetProjectileTraveledDistance(projectile2) <= 0.06f && this.TryRetargetProjectile(projectile2, vector, targetPoint, true))
				{
					this._retargetedProjectiles.Add(instanceID);
					num2++;
					if (projectile == null)
					{
						projectile = projectile2;
					}
				}
			}
			return projectile;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00006B94 File Offset: 0x00004D94
		private bool TryRetargetProjectile(Projectile projectile, Vector3 direction, Vector3 targetPoint, bool forceCritical)
		{
			if (projectile == null)
			{
				return false;
			}
			if (direction.sqrMagnitude < 0.0001f)
			{
				return false;
			}
			direction.Normalize();
			bool flag;
			try
			{
				projectile.context.direction = direction;
				projectile.context.firstFrameCheckStartPoint = projectile.transform.position - direction * 0.1f;
				float num = Vector3.Distance(projectile.transform.position, targetPoint) + 2f;
				if (projectile.context.distance < num)
				{
					projectile.context.distance = num;
				}
				if (forceCritical)
				{
					projectile.context.critRate = 1f;
					projectile.context.ignoreHalfObsticle = true;
				}
				if (ModBehaviour.ProjectileDirectionField != null)
				{
					ModBehaviour.ProjectileDirectionField.SetValue(projectile, direction);
				}
				if (ModBehaviour.ProjectileVelocityField != null)
				{
					Vector3 vector = direction * projectile.context.speed;
					ModBehaviour.ProjectileVelocityField.SetValue(projectile, vector);
				}
				projectile.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
				if (this._obstaclePenetrationEnabled)
				{
					this.TryApplyObstaclePenetration(projectile);
				}
				flag = true;
			}
			catch (Exception)
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00006CD4 File Offset: 0x00004ED4
		private static float GetProjectileTraveledDistance(Projectile projectile)
		{
			if (ModBehaviour.ProjectileTraveledDistanceField == null)
			{
				return 0f;
			}
			float num;
			try
			{
				num = (float)ModBehaviour.ProjectileTraveledDistanceField.GetValue(projectile);
			}
			catch
			{
				num = 0f;
			}
			return num;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00006D24 File Offset: 0x00004F24
		private void TryApplyObstaclePenetration(Projectile projectile)
		{
			if (!this._obstaclePenetrationEnabled || !this._penetrationActive)
			{
				return;
			}
			try
			{
				bool flag = false;
				LayerMask layerMask = default(LayerMask);
				int instanceID = projectile.GetInstanceID();
				bool flag2 = this._penetratingProjectiles.Contains(instanceID);
				if (this._applyPenetrationThisShot || flag2)
				{
					if (ModBehaviour.ProjectileHitLayersField != null && !this._originalProjectileMasks.ContainsKey(instanceID))
					{
						object value = ModBehaviour.ProjectileHitLayersField.GetValue(projectile);
						if (value is LayerMask)
						{
							LayerMask layerMask2 = (LayerMask)value;
							layerMask = layerMask2;
							flag = true;
						}
					}
					projectile.context.ignoreHalfObsticle = true;
					projectile.context.penetrate = Mathf.Max(projectile.context.penetrate, 6);
					this.ConfigureProjectileHitMask(projectile);
					this.RegisterPenetratingProjectile(projectile, flag ? new LayerMask?(layerMask) : null);
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00006E10 File Offset: 0x00005010
		private void ConfigureProjectileHitMask(Projectile projectile)
		{
			if (ModBehaviour.ProjectileHitLayersField == null)
			{
				return;
			}
			try
			{
				int num = GameplayDataSettings.Layers.damageReceiverLayerMask.value;
				int num2 = GameplayDataSettings.Layers.wallLayerMask.value | GameplayDataSettings.Layers.groundLayerMask.value | GameplayDataSettings.Layers.halfObsticleLayer.value;
				num &= ~num2;
				if (num == 0)
				{
					int num3 = LayerMask.NameToLayer("DamageReceiver");
					if (num3 >= 0)
					{
						num |= 1 << num3;
					}
					int num4 = LayerMask.NameToLayer("HeadCollider");
					if (num4 >= 0)
					{
						num |= 1 << num4;
					}
				}
				LayerMask layerMask = new LayerMask
				{
					value = num
				};
				ModBehaviour.ProjectileHitLayersField.SetValue(projectile, layerMask);
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00006EE0 File Offset: 0x000050E0
		private void RegisterPenetratingProjectile(Projectile projectile, LayerMask? originalMask)
		{
			if (projectile == null)
			{
				return;
			}
			int instanceID = projectile.GetInstanceID();
			this._penetratingProjectiles.Add(instanceID);
			if (originalMask != null && !this._originalProjectileMasks.ContainsKey(instanceID))
			{
				this._originalProjectileMasks[instanceID] = originalMask.Value;
			}
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00006F38 File Offset: 0x00005138
		private static void TryUpdateInputAimPoint(InputManager inputManager, Vector3 aimPoint)
		{
			if (ModBehaviour.InputAimPointField == null)
			{
				return;
			}
			try
			{
				ModBehaviour.InputAimPointField.SetValue(inputManager, aimPoint);
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00006F7C File Offset: 0x0000517C
		private static void TrySetAimingEnemyHead(InputManager inputManager, bool value)
		{
			if (ModBehaviour.InputAimingEnemyHeadField == null)
			{
				return;
			}
			try
			{
				ModBehaviour.InputAimingEnemyHeadField.SetValue(inputManager, value);
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00006FC0 File Offset: 0x000051C0
		private void CleanupRetargetedProjectiles()
		{
			if (this._retargetedProjectiles.Count == 0)
			{
				return;
			}
			HashSet<int> activeIds = new HashSet<int>();
			foreach (Projectile projectile in Object.FindObjectsOfType<Projectile>())
			{
				if (projectile != null)
				{
					activeIds.Add(projectile.GetInstanceID());
				}
			}
			this._retargetedProjectiles.RemoveWhere((int id) => !activeIds.Contains(id));
			this._penetratingProjectiles.RemoveWhere((int id) => !activeIds.Contains(id));
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00007050 File Offset: 0x00005250
		private void MaintainPenetratingProjectiles()
		{
			if (this._penetratingProjectiles.Count == 0)
			{
				return;
			}
			foreach (Projectile projectile in Object.FindObjectsOfType<Projectile>())
			{
				if (!(projectile == null))
				{
					int instanceID = projectile.GetInstanceID();
					if (this._penetratingProjectiles.Contains(instanceID))
					{
						try
						{
							projectile.context.ignoreHalfObsticle = true;
							projectile.context.penetrate = Mathf.Max(projectile.context.penetrate, 6);
							this.ConfigureProjectileHitMask(projectile);
							this.EnsureDamagedObjectsInitialized(projectile);
						}
						catch (Exception)
						{
						}
					}
				}
			}
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000070F0 File Offset: 0x000052F0
		private void SetPenetrationActive(bool active)
		{
			if (this._penetrationActive == active)
			{
				return;
			}
			this._penetrationActive = active;
			if (!active)
			{
				this._applyPenetrationThisShot = false;
				this.RestorePenetratingProjectiles();
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00007114 File Offset: 0x00005314
		private void RestorePenetratingProjectiles()
		{
			if (this._penetratingProjectiles.Count == 0)
			{
				return;
			}
			foreach (Projectile projectile in Object.FindObjectsOfType<Projectile>())
			{
				if (!(projectile == null))
				{
					int instanceID = projectile.GetInstanceID();
					if (this._penetratingProjectiles.Contains(instanceID))
					{
						try
						{
							LayerMask layerMask;
							if (this._originalProjectileMasks.TryGetValue(instanceID, out layerMask) && ModBehaviour.ProjectileHitLayersField != null)
							{
								ModBehaviour.ProjectileHitLayersField.SetValue(projectile, layerMask);
							}
							projectile.context.ignoreHalfObsticle = false;
						}
						catch (Exception)
						{
						}
					}
				}
			}
			this._penetratingProjectiles.Clear();
			this._originalProjectileMasks.Clear();
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000071CC File Offset: 0x000053CC
		private void EnsureDamagedObjectsInitialized(Projectile projectile)
		{
			if (ModBehaviour.ProjectileDamagedObjectsField == null)
			{
				return;
			}
			try
			{
				List<GameObject> list = ModBehaviour.ProjectileDamagedObjectsField.GetValue(projectile) as List<GameObject>;
				if (list != null)
				{
					for (int i = list.Count - 1; i >= 0; i--)
					{
						GameObject gameObject = list[i];
						if (gameObject == null)
						{
							list.RemoveAt(i);
						}
						else if (gameObject.GetComponent<DamageReceiver>() == null)
						{
							list.RemoveAt(i);
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00007254 File Offset: 0x00005454
		private static void AutoAimLog(string message)
		{
			bool enableAutoAimDebugLogs = ModBehaviour.EnableAutoAimDebugLogs;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000725C File Offset: 0x0000545C
		private static void TrySetFieldValue([Nullable(2)] FieldInfo field, ItemAgent_Gun gun, object value)
		{
			if (field == null)
			{
				return;
			}
			try
			{
				field.SetValue(gun, value);
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00007294 File Offset: 0x00005494
		private void OverrideItemStat(Item item, int statHash, float newValue)
		{
			if (item == null)
			{
				return;
			}
			Stat stat = item.GetStat(statHash);
			if (stat == null)
			{
				return;
			}
			Dictionary<int, float> dictionary;
			if (!this._originalItemStatValues.TryGetValue(item, out dictionary))
			{
				dictionary = new Dictionary<int, float>();
				this._originalItemStatValues[item] = dictionary;
			}
			if (!dictionary.ContainsKey(statHash))
			{
				dictionary[statHash] = stat.BaseValue;
			}
			stat.BaseValue = newValue;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x000072F8 File Offset: 0x000054F8
		private void RestoreOriginalCharacterRecoil()
		{
			if (this._originalCharacterRecoilControl == null)
			{
				return;
			}
			CharacterMainControl main = CharacterMainControl.Main;
			if (main == null || main.CharacterItem == null)
			{
				this._originalCharacterRecoilControl = null;
				return;
			}
			Stat stat = main.CharacterItem.GetStat(ModBehaviour.RecoilControlStatHash);
			if (stat != null)
			{
				stat.BaseValue = this._originalCharacterRecoilControl.Value;
			}
			this._originalCharacterRecoilControl = null;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00007370 File Offset: 0x00005570
		private void RestoreItemStatOverrides()
		{
			this.HookGunEvents(null);
			foreach (KeyValuePair<Item, Dictionary<int, float>> keyValuePair in this._originalItemStatValues)
			{
				Item key = keyValuePair.Key;
				if (!(key == null))
				{
					foreach (KeyValuePair<int, float> keyValuePair2 in keyValuePair.Value)
					{
						Stat stat = key.GetStat(keyValuePair2.Key);
						if (stat != null)
						{
							stat.BaseValue = keyValuePair2.Value;
						}
					}
				}
			}
			this._originalItemStatValues.Clear();
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00007440 File Offset: 0x00005640
		private static string GetItemDebugName([Nullable(2)] Item item)
		{
			if (item == null)
			{
				return "<null item>";
			}
			string text = item.TypeID.ToString();
			return item.name + " (TypeID: " + text + ")";
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00007481 File Offset: 0x00005681
		private static string GetCharacterDebugName([Nullable(2)] CharacterMainControl character)
		{
			if (character == null)
			{
				return "<null character>";
			}
			return character.name;
		}

		// Token: 0x04000003 RID: 3
		private const string LogPrefix = "[CheatMenu]";

		// Token: 0x04000004 RID: 4
		private static readonly int RecoilControlStatHash = "RecoilControl".GetHashCode();

		// Token: 0x04000005 RID: 5
		private static readonly int[] GunStatHashes = new int[]
		{
			"RecoilVMin".GetHashCode(),
			"RecoilVMax".GetHashCode(),
			"RecoilHMin".GetHashCode(),
			"RecoilHMax".GetHashCode(),
			"RecoilScaleV".GetHashCode(),
			"RecoilScaleH".GetHashCode(),
			"ScatterFactor".GetHashCode(),
			"ScatterFactorADS".GetHashCode(),
			"DefaultScatter".GetHashCode(),
			"DefaultScatterADS".GetHashCode(),
			"MaxScatter".GetHashCode(),
			"MaxScatterADS".GetHashCode(),
			"ScatterGrow".GetHashCode(),
			"ScatterGrowADS".GetHashCode(),
			"ScatterRecover".GetHashCode(),
			"ScatterRecoverADS".GetHashCode(),
			"ShotAngle".GetHashCode()
		};

		// Token: 0x04000006 RID: 6
		private const float NoRecoilControlValue = 9999f;

		// Token: 0x04000007 RID: 7
		private const float NoRecoilStatValue = 0f;

		// Token: 0x04000008 RID: 8
		private readonly Dictionary<Item, Dictionary<int, float>> _originalItemStatValues = new Dictionary<Item, Dictionary<int, float>>();

		// Token: 0x04000009 RID: 9
		private float? _originalCharacterRecoilControl;

		// Token: 0x0400000A RID: 10
		[Nullable(2)]
		private CharacterMainControl _trackedCharacter;

		// Token: 0x0400000B RID: 11
		private bool _characterEventsHooked;

		// Token: 0x0400000C RID: 12
		[Nullable(2)]
		private ItemAgent_Gun _trackedGun;

		// Token: 0x0400000D RID: 13
		private bool _lastTriggerInput;

		// Token: 0x0400000E RID: 14
		private const int AutoAimCircleSegments = 64;

		// Token: 0x0400000F RID: 15
		private const KeyCode MenuToggleKey = KeyCode.F8;

		// Token: 0x04000010 RID: 16
		private const KeyCode RangeToggleKey = KeyCode.F9;

		// Token: 0x04000011 RID: 17
		private const KeyCode ToggleAutoAimKey = KeyCode.F1;

		// Token: 0x04000012 RID: 18
		private const KeyCode ToggleNoRecoilKey = KeyCode.F2;

		// Token: 0x04000013 RID: 19
		private const KeyCode ToggleInfiniteAmmoKey = KeyCode.F3;

		// Token: 0x04000014 RID: 20
		private const KeyCode ToggleMovementUnlockKey = KeyCode.F4;

		// Token: 0x04000015 RID: 21
		private const KeyCode ToggleObstaclePenetrationKey = KeyCode.F5;

		// Token: 0x04000016 RID: 22
		private const KeyCode ToggleInvincibilityKey = KeyCode.F6;

		// Token: 0x04000017 RID: 23
		private const string ShortcutModifierLabel = "Ctrl";

		// Token: 0x04000018 RID: 24
		private const string SettingsKeyPrefix = "CheatMenu.";

		// Token: 0x04000019 RID: 25
		private readonly Vector3[] _autoAimRangePoints = new Vector3[64];

		// Token: 0x0400001A RID: 26
		private bool _menuVisible = true;

		// Token: 0x0400001B RID: 27
		private bool _rangeVisible = true;

		// Token: 0x0400001C RID: 28
		private bool _autoAimEnabled = true;

		// Token: 0x0400001D RID: 29
		private bool _noRecoilEnabled = true;

		// Token: 0x0400001E RID: 30
		private bool _infiniteAmmoEnabled = true;

		// Token: 0x0400001F RID: 31
		private bool _movementUnlockEnabled = true;

		// Token: 0x04000020 RID: 32
		private bool _obstaclePenetrationEnabled = true;

		// Token: 0x04000021 RID: 33
		private bool _invincibilityEnabled = true;

		// Token: 0x04000022 RID: 34
		private float _rangeMultiplier = 1f;

		// Token: 0x04000023 RID: 35
		private float _fireRateMultiplier = 1f;

		// Token: 0x04000024 RID: 36
		private float _bulletSpeedMultiplier = 1f;

		// Token: 0x04000025 RID: 37
		private float _damageMultiplier = 1f;

		// Token: 0x04000026 RID: 38
		[Nullable(2)]
		private GameObject _overlayCanvasObject;

		// Token: 0x04000027 RID: 39
		[Nullable(2)]
		private RectTransform _menuPanel;

		// Token: 0x04000028 RID: 40
		[Nullable(2)]
		private RectTransform _menuRoot;

		// Token: 0x04000029 RID: 41
		[Nullable(2)]
		private RectTransform _sidebarContainer;

		// Token: 0x0400002A RID: 42
		[Nullable(2)]
		private RectTransform _contentContainer;

		// Token: 0x0400002B RID: 43
		[Nullable(2)]
		private RectTransform _sliderContainer;

		// Token: 0x0400002C RID: 44
		private readonly Dictionary<ModBehaviour.MenuSection, Button> _menuSectionButtons = new Dictionary<ModBehaviour.MenuSection, Button>();

		// Token: 0x0400002D RID: 45
		private readonly Dictionary<ModBehaviour.MenuSection, GameObject> _menuSectionRoots = new Dictionary<ModBehaviour.MenuSection, GameObject>();

		// Token: 0x0400002E RID: 46
		[Nullable(2)]
		private TextMeshProUGUI _functionToggleContent;

		// Token: 0x0400002F RID: 47
		[Nullable(2)]
		private TextMeshProUGUI _autoAimContent;

		// Token: 0x04000030 RID: 48
		[Nullable(2)]
		private TextMeshProUGUI _weaponInfoContent;

		// Token: 0x04000031 RID: 49
		[Nullable(2)]
		private TextMeshProUGUI _shortcutsContent;

		// Token: 0x04000032 RID: 50
		[Nullable(2)]
		private TextMeshProUGUI _multipliersSummary;

		// Token: 0x04000033 RID: 51
		[Nullable(2)]
		private TextMeshProUGUI _rangeValueText;

		// Token: 0x04000034 RID: 52
		[Nullable(2)]
		private TextMeshProUGUI _fireRateValueText;

		// Token: 0x04000035 RID: 53
		[Nullable(2)]
		private TextMeshProUGUI _bulletSpeedValueText;

		// Token: 0x04000036 RID: 54
		[Nullable(2)]
		private TextMeshProUGUI _damageValueText;

		// Token: 0x04000037 RID: 55
		[Nullable(2)]
		private Slider _rangeSlider;

		// Token: 0x04000038 RID: 56
		[Nullable(2)]
		private Slider _fireRateSlider;

		// Token: 0x04000039 RID: 57
		[Nullable(2)]
		private Slider _bulletSpeedSlider;

		// Token: 0x0400003A RID: 58
		[Nullable(2)]
		private Slider _damageSlider;

		// Token: 0x0400003B RID: 59
		private bool _suppressSliderEvents;

		// Token: 0x0400003C RID: 60
		[Nullable(2)]
		private LineRenderer _autoAimRangeRenderer;

		// Token: 0x0400003D RID: 61
		[Nullable(2)]
		private RectTransform _autoAimRangeRect;

		// Token: 0x0400003E RID: 62
		[Nullable(2)]
		private Material _autoAimRangeMaterial;

		// Token: 0x0400003F RID: 63
		private Rect _menuWindowRect;

		// Token: 0x04000040 RID: 64
		private Vector2 _menuScrollPosition;

		// Token: 0x04000041 RID: 65
		private bool _menuWindowInitialized;

		// Token: 0x04000042 RID: 66
		private readonly int _menuWindowId = "CheatMenuWindow".GetHashCode();

		// Token: 0x04000043 RID: 67
		private bool _functionSectionExpanded = true;

		// Token: 0x04000044 RID: 68
		private bool _multipliersSectionExpanded = true;

		// Token: 0x04000045 RID: 69
		private bool _autoAimSectionExpanded = true;

		// Token: 0x04000046 RID: 70
		private bool _weaponSectionExpanded = true;

		// Token: 0x04000047 RID: 71
		private bool _shortcutsSectionExpanded = true;

		// Token: 0x04000048 RID: 72
		[Nullable(2)]
		private Texture2D _autoAimCircleTexture;

		// Token: 0x04000049 RID: 73
		private bool _autoAimCircleVisible;

		// Token: 0x0400004A RID: 74
		private Vector2 _autoAimCircleCenter;

		// Token: 0x0400004B RID: 75
		private float _autoAimCircleRadiusPixels;

		// Token: 0x0400004C RID: 76
		private Vector2 _latestAimScreenPoint;

		// Token: 0x0400004D RID: 77
		private string _functionToggleText = string.Empty;

		// Token: 0x0400004E RID: 78
		private string _multipliersSummaryText = string.Empty;

		// Token: 0x0400004F RID: 79
		private string _autoAimContentText = string.Empty;

		// Token: 0x04000050 RID: 80
		private string _weaponInfoText = string.Empty;

		// Token: 0x04000051 RID: 81
		private string _shortcutsText = string.Empty;

		// Token: 0x04000052 RID: 82
		private ModBehaviour.MenuSection _activeMenuSection;

		// Token: 0x04000053 RID: 83
		private readonly Color _sidebarButtonColor = new Color(0.16f, 0.16f, 0.16f, 0.88f);

		// Token: 0x04000054 RID: 84
		private readonly Color _sidebarButtonSelectedColor = new Color(0.24f, 0.55f, 0.96f, 0.9f);

		// Token: 0x04000055 RID: 85
		private float _lastAutoAimWorldRadius;

		// Token: 0x04000056 RID: 86
		private readonly Dictionary<Item, Dictionary<int, float>> _statMultiplierOriginalValues = new Dictionary<Item, Dictionary<int, float>>();

		// Token: 0x04000057 RID: 87
		private readonly Dictionary<CharacterMainControl, bool> _dashControlOriginalStates = new Dictionary<CharacterMainControl, bool>();

		// Token: 0x04000058 RID: 88
		private readonly Dictionary<CA_Dash, float> _dashCooldownOriginalValues = new Dictionary<CA_Dash, float>();

		// Token: 0x04000059 RID: 89
		private readonly Dictionary<CharacterMainControl, float> _disableTriggerOriginalValues = new Dictionary<CharacterMainControl, float>();

		// Token: 0x0400005A RID: 90
		private bool? _originalCharacterInvincible;

		// Token: 0x0400005B RID: 91
		[Nullable(2)]
		private Health _invincibilityTargetHealth;

		// Token: 0x0400005C RID: 92
		[Nullable(2)]
		private static readonly FieldInfo ScatterBeforeControlField = typeof(ItemAgent_Gun).GetField("scatterBeforeControl", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400005D RID: 93
		[Nullable(2)]
		private static readonly FieldInfo ScatterFactorHipsField = typeof(ItemAgent_Gun).GetField("scatterFactorHips", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400005E RID: 94
		[Nullable(2)]
		private static readonly FieldInfo ScatterFactorAdsField = typeof(ItemAgent_Gun).GetField("scatterFactorAds", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400005F RID: 95
		[Nullable(2)]
		private static readonly FieldInfo RecoilMoveValueField = typeof(ItemAgent_Gun).GetField("_recoilMoveValue", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000060 RID: 96
		[Nullable(2)]
		private static readonly FieldInfo RecoilBackField = typeof(ItemAgent_Gun).GetField("_recoilBack", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000061 RID: 97
		[Nullable(2)]
		private static readonly FieldInfo RecoilDistanceField = typeof(ItemAgent_Gun).GetField("_recoilDistance", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000062 RID: 98
		[Nullable(2)]
		private static readonly FieldInfo RecoilBackSpeedField = typeof(ItemAgent_Gun).GetField("_recoilBackSpeed", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000063 RID: 99
		[Nullable(2)]
		private static readonly FieldInfo RecoilRecoverSpeedField = typeof(ItemAgent_Gun).GetField("_recoilRecoverSpeed", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000064 RID: 100
		[Nullable(2)]
		private static readonly PropertyInfo GunBulletCountProperty = typeof(ItemSetting_Gun).GetProperty("bulletCount", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000065 RID: 101
		[Nullable(2)]
		private static readonly FieldInfo ProjectileVelocityField = typeof(Projectile).GetField("velocity", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000066 RID: 102
		[Nullable(2)]
		private static readonly FieldInfo ProjectileDirectionField = typeof(Projectile).GetField("direction", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000067 RID: 103
		[Nullable(2)]
		private static readonly FieldInfo ProjectileTraveledDistanceField = typeof(Projectile).GetField("traveledDistance", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000068 RID: 104
		[Nullable(2)]
		private static readonly FieldInfo InputAimPointField = typeof(InputManager).GetField("inputAimPoint", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000069 RID: 105
		[Nullable(2)]
		private static readonly FieldInfo GunProjectileField = typeof(ItemAgent_Gun).GetField("projInst", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400006A RID: 106
		[Nullable(2)]
		private static readonly FieldInfo InputAimingEnemyHeadField = typeof(InputManager).GetField("aimingEnemyHead", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400006B RID: 107
		[Nullable(2)]
		private static readonly FieldInfo ProjectileHitLayersField = typeof(Projectile).GetField("hitLayers", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400006C RID: 108
		[Nullable(2)]
		private static readonly FieldInfo ProjectileDamagedObjectsField = typeof(Projectile).GetField("damagedObjects", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400006D RID: 109
		[Nullable(2)]
		private static readonly FieldInfo ProjectileDeadField = typeof(Projectile).GetField("dead", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400006E RID: 110
		private static readonly int BulletDistanceStatHash = "BulletDistance".GetHashCode();

		// Token: 0x0400006F RID: 111
		private static readonly int ShootSpeedStatHash = "ShootSpeed".GetHashCode();

		// Token: 0x04000070 RID: 112
		private static readonly int BulletSpeedStatHash = "BulletSpeed".GetHashCode();

		// Token: 0x04000071 RID: 113
		private static readonly int DamageStatHash = "Damage".GetHashCode();

		// Token: 0x04000072 RID: 114
		private const float StatMultiplierMin = 1f;

		// Token: 0x04000073 RID: 115
		private const float StatMultiplierMax = 10f;

		// Token: 0x04000074 RID: 116
		private const float StatMultiplierStep = 0.1f;

		// Token: 0x04000075 RID: 117
		[Nullable(2)]
		private static readonly FieldInfo DisableTriggerTimerField = typeof(CharacterMainControl).GetField("disableTriggerTimer", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000076 RID: 118
		[Nullable(2)]
		private static readonly FieldInfo CurrentStaminaField = typeof(CharacterMainControl).GetField("currentStamina", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000077 RID: 119
		private static readonly bool EnableAutoAimDebugLogs = true;

		// Token: 0x04000078 RID: 120
		private static readonly int DamageReceiverLayerMaskValue = GameplayDataSettings.Layers.damageReceiverLayerMask;

		// Token: 0x04000079 RID: 121
		private static readonly int ObstacleLayerMaskValue = GameplayDataSettings.Layers.wallLayerMask | GameplayDataSettings.Layers.groundLayerMask | GameplayDataSettings.Layers.halfObsticleLayer;

		// Token: 0x0400007A RID: 122
		private const float AutoAimRayRadius = 0.6f;

		// Token: 0x0400007B RID: 123
		private const float AutoAimFallbackRadius = 3.5f;

		// Token: 0x0400007C RID: 124
		private float _autoAimScreenRadius = 140f;

		// Token: 0x0400007D RID: 125
		private const float AutoAimScreenRadiusMin = 60f;

		// Token: 0x0400007E RID: 126
		private const float AutoAimScreenRadiusMax = 2048f;

		// Token: 0x0400007F RID: 127
		private const float AutoAimScreenRadiusStep = 5f;

		// Token: 0x04000080 RID: 128
		private const float AutoAimMaxDistance = 80f;

		// Token: 0x04000081 RID: 129
		private const float AutoAimAngleWeight = 2.25f;

		// Token: 0x04000082 RID: 130
		private const float AutoAimDistanceWeight = 0.1f;

		// Token: 0x04000083 RID: 131
		private const float AutoAimPenetrationBasePenalty = 35f;

		// Token: 0x04000084 RID: 132
		private const float AutoAimPenetrationDistanceWeight = 1.75f;

		// Token: 0x04000085 RID: 133
		private const float AutoAimObstaclePadding = 0.2f;

		// Token: 0x04000086 RID: 134
		private const float AutoAimDefaultAimHeight = 1.35f;

		// Token: 0x04000087 RID: 135
		private const float ProjectileSelectionTravelThreshold = 0.06f;

		// Token: 0x04000088 RID: 136
		private readonly RaycastHit[] _autoAimRaycastHits = new RaycastHit[24];

		// Token: 0x04000089 RID: 137
		private readonly Collider[] _autoAimOverlapResults = new Collider[24];

		// Token: 0x0400008A RID: 138
		private readonly HashSet<int> _retargetedProjectiles = new HashSet<int>();

		// Token: 0x0400008B RID: 139
		private readonly HashSet<int> _penetratingProjectiles = new HashSet<int>();

		// Token: 0x0400008C RID: 140
		private readonly Dictionary<int, LayerMask> _originalProjectileMasks = new Dictionary<int, LayerMask>();

		// Token: 0x0400008D RID: 141
		private bool _penetrationActive;

		// Token: 0x0400008E RID: 142
		private bool _applyPenetrationThisShot;

		// Token: 0x0400008F RID: 143
		private readonly Dictionary<int, Vector3> _headshotOffsets = new Dictionary<int, Vector3>();

		// Token: 0x02000006 RID: 6
		[NullableContext(0)]
		private enum MenuSection
		{
			// Token: 0x04000091 RID: 145
			FunctionToggles,
			// Token: 0x04000092 RID: 146
			Multipliers,
			// Token: 0x04000093 RID: 147
			AutoAim,
			// Token: 0x04000094 RID: 148
			Weapon,
			// Token: 0x04000095 RID: 149
			Shortcuts
		}

		// Token: 0x02000007 RID: 7
		[NullableContext(0)]
		private struct HeadshotInfo
		{
			// Token: 0x04000096 RID: 150
			public bool HasHead;

			// Token: 0x04000097 RID: 151
			public Vector3 HeadPosition;

			// Token: 0x04000098 RID: 152
			public Vector3 RandomOffset;
		}

		// Token: 0x02000008 RID: 8
		[Nullable(0)]
		private readonly struct AutoAimCandidate
		{
			// Token: 0x06000097 RID: 151 RVA: 0x00007A37 File Offset: 0x00005C37
			public AutoAimCandidate(DamageReceiver receiver, Vector3 aimPoint, float screenDistance, float rayDistance, bool requiresPenetration)
			{
				this.Receiver = receiver;
				this.AimPoint = aimPoint;
				this.ScreenDistance = screenDistance;
				this.RayDistance = rayDistance;
				this.RequiresPenetration = requiresPenetration;
			}

			// Token: 0x17000001 RID: 1
			// (get) Token: 0x06000098 RID: 152 RVA: 0x00007A5E File Offset: 0x00005C5E
			public DamageReceiver Receiver { get; }

			// Token: 0x17000002 RID: 2
			// (get) Token: 0x06000099 RID: 153 RVA: 0x00007A66 File Offset: 0x00005C66
			public Vector3 AimPoint { get; }

			// Token: 0x17000003 RID: 3
			// (get) Token: 0x0600009A RID: 154 RVA: 0x00007A6E File Offset: 0x00005C6E
			public float ScreenDistance { get; }

			// Token: 0x17000004 RID: 4
			// (get) Token: 0x0600009B RID: 155 RVA: 0x00007A76 File Offset: 0x00005C76
			public float RayDistance { get; }

			// Token: 0x17000005 RID: 5
			// (get) Token: 0x0600009C RID: 156 RVA: 0x00007A7E File Offset: 0x00005C7E
			public bool RequiresPenetration { get; }
		}
	}
}
