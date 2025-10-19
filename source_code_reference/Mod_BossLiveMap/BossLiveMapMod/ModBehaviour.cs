using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Duckov.MiniMaps;
using Duckov.MiniMaps.UI;
using Duckov.Modding;
using Duckov.Scenes;
using Duckov.UI;
using Duckov.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace BossLiveMapMod
{
	// Token: 0x02000002 RID: 2
	public class ModBehaviour : ModBehaviour
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		private void Awake()
		{
			Debug.Log("BossLiveMapMod loaded: live boss markers enabled");
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000205C File Offset: 0x0000025C
		private void OnEnable()
		{
			LevelManager.OnLevelInitialized += this.OnLevelInitialized;
			LevelManager.OnAfterLevelInitialized += this.OnAfterLevelInitialized;
			MultiSceneCore.OnSubSceneLoaded += this.OnSubSceneLoaded;
			View.OnActiveViewChanged += this.OnActiveViewChanged;
			this._activeWhenMapOpen = ModBehaviour.IsMapOpen();
			if (this._activeWhenMapOpen)
			{
				this.HookSpawners();
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020C8 File Offset: 0x000002C8
		private void OnDisable()
		{
			LevelManager.OnLevelInitialized -= this.OnLevelInitialized;
			LevelManager.OnAfterLevelInitialized -= this.OnAfterLevelInitialized;
			MultiSceneCore.OnSubSceneLoaded -= this.OnSubSceneLoaded;
			View.OnActiveViewChanged -= this.OnActiveViewChanged;
			this.ClearAllMarkers();
			this.UnhookSpawners();
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002125 File Offset: 0x00000325
		private void OnLevelInitialized()
		{
			this.ClearAllMarkers();
			if (this._activeWhenMapOpen)
			{
				this.TryEnsureMarkers();
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000213B File Offset: 0x0000033B
		private void OnAfterLevelInitialized()
		{
			if (this._activeWhenMapOpen)
			{
				this.TryEnsureMarkers();
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000214B File Offset: 0x0000034B
		private void OnSubSceneLoaded(MultiSceneCore core, Scene scene)
		{
			if (this._activeWhenMapOpen)
			{
				this.TryEnsureMarkers();
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000215C File Offset: 0x0000035C
		private void OnActiveViewChanged()
		{
			bool nowOpen = ModBehaviour.IsMapOpen();
			if (nowOpen && !this._activeWhenMapOpen)
			{
				this._activeWhenMapOpen = true;
				Health.OnDead += this.OnAnyHealthDead;
				this.HookSpawners();
				this._scanTimer = 0f;
				this.TryEnsureMarkers();
				this.UpdateMarkerPositions();
				return;
			}
			if (!nowOpen && this._activeWhenMapOpen)
			{
				this._activeWhenMapOpen = false;
				Health.OnDead -= this.OnAnyHealthDead;
				this.ClearAllMarkers();
				this.UnhookSpawners();
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000021E0 File Offset: 0x000003E0
		private static bool IsMapOpen()
		{
			MiniMapView mm = MiniMapView.Instance;
			return mm != null && View.ActiveView == mm;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x0000220C File Offset: 0x0000040C
		private void Update()
		{
			if (!this._activeWhenMapOpen)
			{
				return;
			}
			if (!LevelManager.LevelInited)
			{
				return;
			}
			this._scanTimer -= Time.unscaledDeltaTime;
			if (this._scanTimer <= 0f)
			{
				this._scanTimer = 0.75f;
				this.TryEnsureMarkers();
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000225C File Offset: 0x0000045C
		private void TryEnsureMarkers()
		{
			this.PruneMissing();
			List<CharacterMainControl> allChars = ModBehaviour.CollectCharacters();
			if (allChars == null || allChars.Count == 0)
			{
				return;
			}
			LevelManager instance = LevelManager.Instance;
			CharacterMainControl main = ((instance != null) ? instance.MainCharacter : null);
			foreach (CharacterMainControl c in allChars)
			{
				if (!(c == null) && !(c == main) && ModBehaviour.IsBoss(c) && !(c.Health == null) && !c.Health.IsDead && !this._markers.ContainsKey(c))
				{
					GameObject go = new GameObject("BossMarker:" + ModBehaviour.GetBossName(c));
					go.transform.position = c.transform.position;
					SimplePointOfInterest poi = go.AddComponent<SimplePointOfInterest>();
					poi.Color = ModBehaviour.GetRedColor();
					poi.ShadowColor = Color.black;
					poi.ShadowDistance = 0f;
					poi.Setup(ModBehaviour.GetThirdMapMarkerIcon(), ModBehaviour.GetBossName(c), true, null);
					if (MultiSceneCore.MainScene != null)
					{
						SceneManager.MoveGameObjectToScene(go, MultiSceneCore.MainScene.Value);
					}
					this._markers[c] = poi;
				}
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000023D8 File Offset: 0x000005D8
		private void PruneMissing()
		{
			if (this._markers.Count == 0)
			{
				return;
			}
			List<CharacterMainControl> toRemove = new List<CharacterMainControl>();
			foreach (KeyValuePair<CharacterMainControl, SimplePointOfInterest> kv in this._markers)
			{
				CharacterMainControl ch = kv.Key;
				SimplePointOfInterest poi = kv.Value;
				if (ch == null || poi == null || ch.Health == null || ch.Health.IsDead)
				{
					toRemove.Add(ch);
				}
			}
			foreach (CharacterMainControl ch2 in toRemove)
			{
				SimplePointOfInterest poi2;
				if (ch2 != null && this._markers.TryGetValue(ch2, out poi2) && poi2 != null)
				{
					try
					{
						Object.Destroy(poi2.gameObject);
					}
					catch
					{
					}
				}
				this._markers.Remove(ch2);
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002508 File Offset: 0x00000708
		private void ClearAllMarkers()
		{
			foreach (KeyValuePair<CharacterMainControl, SimplePointOfInterest> kv in this._markers)
			{
				SimplePointOfInterest poi = kv.Value;
				if (poi != null)
				{
					try
					{
						Object.Destroy(poi.gameObject);
					}
					catch
					{
					}
				}
			}
			this._markers.Clear();
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000258C File Offset: 0x0000078C
		private void LateUpdate()
		{
			if (!this._activeWhenMapOpen)
			{
				return;
			}
			if (!LevelManager.LevelInited)
			{
				return;
			}
			this.UpdateMarkerPositions();
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000025A8 File Offset: 0x000007A8
		private void UpdateMarkerPositions()
		{
			if (this._markers.Count == 0)
			{
				return;
			}
			List<CharacterMainControl> deadOrMissing = null;
			foreach (KeyValuePair<CharacterMainControl, SimplePointOfInterest> kv in this._markers)
			{
				CharacterMainControl ch = kv.Key;
				SimplePointOfInterest poi = kv.Value;
				if (ch == null || poi == null)
				{
					if (deadOrMissing == null)
					{
						deadOrMissing = new List<CharacterMainControl>();
					}
					deadOrMissing.Add(ch);
				}
				else if (ch.Health != null && ch.Health.IsDead)
				{
					if (deadOrMissing == null)
					{
						deadOrMissing = new List<CharacterMainControl>();
					}
					deadOrMissing.Add(ch);
				}
				else
				{
					poi.transform.position = ch.transform.position;
				}
			}
			if (deadOrMissing != null)
			{
				foreach (CharacterMainControl ch2 in deadOrMissing)
				{
					SimplePointOfInterest poi2;
					if (ch2 != null && this._markers.TryGetValue(ch2, out poi2) && poi2 != null)
					{
						try
						{
							Object.Destroy(poi2.gameObject);
						}
						catch
						{
						}
					}
					this._markers.Remove(ch2);
				}
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002710 File Offset: 0x00000910
		private void OnAnyHealthDead(Health h, DamageInfo info)
		{
			if (!this._activeWhenMapOpen || h == null)
			{
				return;
			}
			try
			{
				CharacterMainControl ch = h.TryGetCharacter();
				if (!(ch == null))
				{
					SimplePointOfInterest poi;
					if (this._markers.TryGetValue(ch, out poi) && poi != null)
					{
						Object.Destroy(poi.gameObject);
						this._markers.Remove(ch);
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002788 File Offset: 0x00000988
		private static List<CharacterMainControl> CollectCharacters()
		{
			HashSet<CharacterMainControl> set = new HashSet<CharacterMainControl>();
			try
			{
				CharacterMainControl[] actives = Object.FindObjectsOfType<CharacterMainControl>();
				if (actives != null)
				{
					foreach (CharacterMainControl c in actives)
					{
						if (c != null)
						{
							set.Add(c);
						}
					}
				}
			}
			catch
			{
			}
			try
			{
				if (ModBehaviour.s_listsOfScenesField == null)
				{
					ModBehaviour.s_listsOfScenesField = typeof(SetActiveByPlayerDistance).GetField("listsOfScenes", BindingFlags.Static | BindingFlags.NonPublic);
				}
				FieldInfo fieldInfo = ModBehaviour.s_listsOfScenesField;
				Dictionary<int, List<GameObject>> dict = ((fieldInfo != null) ? fieldInfo.GetValue(null) : null) as Dictionary<int, List<GameObject>>;
				if (dict != null)
				{
					foreach (KeyValuePair<int, List<GameObject>> kv in dict)
					{
						List<GameObject> list = kv.Value;
						if (list != null)
						{
							foreach (GameObject go in list)
							{
								if (!(go == null))
								{
									CharacterMainControl c2 = go.GetComponent<CharacterMainControl>();
									if (c2 != null)
									{
										set.Add(c2);
									}
								}
							}
						}
					}
				}
			}
			catch
			{
			}
			return new List<CharacterMainControl>(set);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000028E8 File Offset: 0x00000AE8
		private void HookSpawners()
		{
			try
			{
				foreach (CharacterSpawnerRoot root in Object.FindObjectsOfType<CharacterSpawnerRoot>(true))
				{
					if (!(root == null) && !this._rootStartHandlers.ContainsKey(root))
					{
						UnityAction handler = delegate
						{
							if (this._activeWhenMapOpen)
							{
								this.DeferredRescan();
							}
						};
						UnityEvent onStartEvent = root.OnStartEvent;
						if (onStartEvent != null)
						{
							onStartEvent.AddListener(handler);
						}
						this._rootStartHandlers[root] = handler;
					}
				}
			}
			catch
			{
			}
			try
			{
				foreach (RandomCharacterSpawner sp in Object.FindObjectsOfType<RandomCharacterSpawner>(true))
				{
					if (!(sp == null) && !this._randomStartHandlers.ContainsKey(sp))
					{
						UnityAction handler2 = delegate
						{
							if (this._activeWhenMapOpen)
							{
								this.DeferredRescan();
							}
						};
						UnityEvent onStartCreateEvent = sp.OnStartCreateEvent;
						if (onStartCreateEvent != null)
						{
							onStartCreateEvent.AddListener(handler2);
						}
						this._randomStartHandlers[sp] = handler2;
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000029E0 File Offset: 0x00000BE0
		private void UnhookSpawners()
		{
			try
			{
				foreach (KeyValuePair<CharacterSpawnerRoot, UnityAction> kv in this._rootStartHandlers)
				{
					CharacterSpawnerRoot root = kv.Key;
					UnityAction handler = kv.Value;
					if (root != null && handler != null)
					{
						UnityEvent onStartEvent = root.OnStartEvent;
						if (onStartEvent != null)
						{
							onStartEvent.RemoveListener(handler);
						}
					}
				}
				this._rootStartHandlers.Clear();
			}
			catch
			{
			}
			try
			{
				foreach (KeyValuePair<RandomCharacterSpawner, UnityAction> kv2 in this._randomStartHandlers)
				{
					RandomCharacterSpawner sp = kv2.Key;
					UnityAction handler2 = kv2.Value;
					if (sp != null && handler2 != null)
					{
						UnityEvent onStartCreateEvent = sp.OnStartCreateEvent;
						if (onStartCreateEvent != null)
						{
							onStartCreateEvent.RemoveListener(handler2);
						}
					}
				}
				this._randomStartHandlers.Clear();
			}
			catch
			{
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002B04 File Offset: 0x00000D04
		private void DeferredRescan()
		{
			base.StartCoroutine(this.DeferredRescanRoutine());
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002B13 File Offset: 0x00000D13
		private IEnumerator DeferredRescanRoutine()
		{
			ModBehaviour.<DeferredRescanRoutine>d__26 <DeferredRescanRoutine>d__ = new ModBehaviour.<DeferredRescanRoutine>d__26(0);
			<DeferredRescanRoutine>d__.<>4__this = this;
			return <DeferredRescanRoutine>d__;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002B24 File Offset: 0x00000D24
		private static bool IsBoss(CharacterMainControl c)
		{
			bool flag;
			try
			{
				if (c == null)
				{
					flag = false;
				}
				else
				{
					CharacterRandomPreset preset = c.characterPreset;
					if (preset == null)
					{
						flag = false;
					}
					else if (preset.GetCharacterIcon() == GameplayDataSettings.UIStyle.BossCharacterIcon)
					{
						flag = true;
					}
					else
					{
						FieldInfo f = typeof(CharacterRandomPreset).GetField("characterIconType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						if (f != null)
						{
							object v = f.GetValue(preset);
							if (v is CharacterIconTypes)
							{
								CharacterIconTypes enumVal = (CharacterIconTypes)v;
								if (enumVal == CharacterIconTypes.boss)
								{
									return true;
								}
							}
						}
						flag = false;
					}
				}
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002BC8 File Offset: 0x00000DC8
		private static string GetBossName(CharacterMainControl c)
		{
			string text2;
			try
			{
				string text;
				if (c == null)
				{
					text = null;
				}
				else
				{
					CharacterRandomPreset characterPreset = c.characterPreset;
					text = ((characterPreset != null) ? characterPreset.DisplayName : null);
				}
				string i = text;
				text2 = (string.IsNullOrEmpty(i) ? "Boss" : i);
			}
			catch
			{
				text2 = "Boss";
			}
			return text2;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002C1C File Offset: 0x00000E1C
		private static Sprite GetThirdMapMarkerIcon()
		{
			try
			{
				List<Sprite> icons = MapMarkerManager.Icons;
				if (icons != null && icons.Count >= 3 && icons[2] != null)
				{
					return icons[2];
				}
			}
			catch
			{
			}
			try
			{
				return MapMarkerManager.SelectedIcon;
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002C84 File Offset: 0x00000E84
		private static Color GetRedColor()
		{
			return Color.red;
		}

		// Token: 0x04000001 RID: 1
		private readonly Dictionary<CharacterMainControl, SimplePointOfInterest> _markers = new Dictionary<CharacterMainControl, SimplePointOfInterest>();

		// Token: 0x04000002 RID: 2
		private float _scanTimer;

		// Token: 0x04000003 RID: 3
		private const float ScanInterval = 0.75f;

		// Token: 0x04000004 RID: 4
		private bool _activeWhenMapOpen;

		// Token: 0x04000005 RID: 5
		private static FieldInfo s_listsOfScenesField;

		// Token: 0x04000006 RID: 6
		private readonly Dictionary<CharacterSpawnerRoot, UnityAction> _rootStartHandlers = new Dictionary<CharacterSpawnerRoot, UnityAction>();

		// Token: 0x04000007 RID: 7
		private readonly Dictionary<RandomCharacterSpawner, UnityAction> _randomStartHandlers = new Dictionary<RandomCharacterSpawner, UnityAction>();
	}
}
