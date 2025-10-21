using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Duckov;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x02000005 RID: 5
	[NullableContext(1)]
	[Nullable(0)]
	public class AudioController : IDisposable
	{
		// Token: 0x06000005 RID: 5 RVA: 0x0000208E File Offset: 0x0000028E
		public AudioController(LogManager logManager)
		{
			this.logManager = logManager;
			this.loadedSounds = new Dictionary<string, Sound>();
			this.activeChannels = new List<Channel>();
			this.InitializeAudio();
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020BC File Offset: 0x000002BC
		private void InitializeAudio()
		{
			try
			{
				foreach (string text in AudioStorage.GetAvailableAudioNames())
				{
					this.LoadAudioFromMemory(text);
				}
				LogManager logManager = this.logManager;
				if (logManager != null)
				{
					logManager.WriteLog(string.Format("AudioController initialized successfully. Loaded {0} custom sounds.", this.loadedSounds.Count), false);
				}
			}
			catch (Exception ex)
			{
				LogManager logManager2 = this.logManager;
				if (logManager2 != null)
				{
					logManager2.WriteLog(string.Format("Failed to initialize AudioController: {0}", ex), true);
				}
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002164 File Offset: 0x00000364
		private bool LoadAudioFromMemory(string audioName)
		{
			bool flag;
			try
			{
				string audioData = AudioStorage.GetAudioData(audioName);
				if (string.IsNullOrEmpty(audioData))
				{
					LogManager logManager = this.logManager;
					if (logManager != null)
					{
						logManager.WriteLog("Audio data not found for: " + audioName, true);
					}
					flag = false;
				}
				else
				{
					byte[] array = Convert.FromBase64String(audioData);
					CREATESOUNDEXINFO createsoundexinfo = default(CREATESOUNDEXINFO);
					createsoundexinfo.cbsize = Marshal.SizeOf(typeof(CREATESOUNDEXINFO));
					createsoundexinfo.length = (uint)array.Length;
					Sound sound;
					RESULT result = RuntimeManager.CoreSystem.createSound(array, 2049, ref createsoundexinfo, ref sound);
					if (result == null)
					{
						this.loadedSounds[audioName] = sound;
						LogManager logManager2 = this.logManager;
						if (logManager2 != null)
						{
							logManager2.WriteLog("Successfully loaded custom audio: " + audioName, false);
						}
						flag = true;
					}
					else
					{
						LogManager logManager3 = this.logManager;
						if (logManager3 != null)
						{
							logManager3.WriteLog(string.Format("FMOD failed to create sound for {0}: {1}", audioName, result), true);
						}
						flag = false;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager logManager4 = this.logManager;
				if (logManager4 != null)
				{
					logManager4.WriteLog(string.Format("Failed to load audio {0}: {1}", audioName, ex), true);
				}
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002280 File Offset: 0x00000480
		public bool PlayCustomAudio(string audioName, float volume = 1f, string busPath = "bus:/Master/SFX")
		{
			if (this.isDisposed)
			{
				LogManager logManager = this.logManager;
				if (logManager != null)
				{
					logManager.WriteLog("AudioController is disposed, cannot play audio.", true);
				}
				return false;
			}
			bool flag;
			try
			{
				Sound sound;
				if (this.loadedSounds.TryGetValue(audioName, out sound))
				{
					ChannelGroup channelGroup;
					RuntimeManager.GetBus(busPath).getChannelGroup(ref channelGroup);
					Channel channel;
					RESULT result = RuntimeManager.CoreSystem.playSound(sound, channelGroup, false, ref channel);
					if (result == null)
					{
						channel.setVolume(Mathf.Clamp01(volume));
						this.activeChannels.Add(channel);
						LogManager logManager2 = this.logManager;
						if (logManager2 != null)
						{
							logManager2.WriteLog(string.Format("Successfully played custom audio: {0} (Volume: {1})", audioName, volume), false);
						}
						flag = true;
					}
					else
					{
						LogManager logManager3 = this.logManager;
						if (logManager3 != null)
						{
							logManager3.WriteLog(string.Format("FMOD failed to play sound {0}: {1}", audioName, result), true);
						}
						flag = false;
					}
				}
				else
				{
					LogManager logManager4 = this.logManager;
					if (logManager4 != null)
					{
						logManager4.WriteLog("Custom audio not found: " + audioName + ". Falling back to built-in sound.", true);
					}
					flag = this.PlayBuiltInAudio(audioName, volume);
				}
			}
			catch (Exception ex)
			{
				LogManager logManager5 = this.logManager;
				if (logManager5 != null)
				{
					logManager5.WriteLog(string.Format("Failed to play custom audio {0}: {1}", audioName, ex), true);
				}
				flag = this.PlayBuiltInAudio(audioName, volume);
			}
			return flag;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000023C8 File Offset: 0x000005C8
		private bool PlayBuiltInAudio(string audioName, float volume)
		{
			bool flag;
			try
			{
				string builtInSoundPath = this.GetBuiltInSoundPath(audioName);
				EventInstance eventInstance;
				if (AudioManager.TryCreateEventInstance(builtInSoundPath, out eventInstance))
				{
					eventInstance.setVolume(volume);
					eventInstance.start();
					eventInstance.release();
					LogManager logManager = this.logManager;
					if (logManager != null)
					{
						logManager.WriteLog("Played built-in audio: " + builtInSoundPath, false);
					}
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			catch (Exception ex)
			{
				LogManager logManager2 = this.logManager;
				if (logManager2 != null)
				{
					logManager2.WriteLog(string.Format("Failed to play built-in audio: {0}", ex), true);
				}
				flag = false;
			}
			return flag;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002458 File Offset: 0x00000658
		private string GetBuiltInSoundPath(string audioName)
		{
			if (audioName != null)
			{
				int length = audioName.Length;
				if (length <= 16)
				{
					if (length != 9)
					{
						switch (length)
						{
						case 12:
						{
							char c = audioName[0];
							if (c != 'n')
							{
								if (c != 't')
								{
									goto IL_00D7;
								}
								if (!(audioName == "trader_alert"))
								{
									goto IL_00D7;
								}
								goto IL_00C5;
							}
							else if (!(audioName == "notification"))
							{
								goto IL_00D7;
							}
							break;
						}
						case 13:
						case 15:
							goto IL_00D7;
						case 14:
							if (!(audioName == "enemy_detected"))
							{
								goto IL_00D7;
							}
							break;
						case 16:
							if (!(audioName == "high_value_alert"))
							{
								goto IL_00D7;
							}
							goto IL_00C5;
						default:
							goto IL_00D7;
						}
						return "UI/hover";
					}
					if (!(audioName == "pmc_alert"))
					{
						goto IL_00D7;
					}
					IL_00C5:
					return "UI/game_start";
				}
				if (length != 21)
				{
					if (length != 23)
					{
						goto IL_00D7;
					}
					if (!(audioName == "magic_bullet_deactivate"))
					{
						goto IL_00D7;
					}
				}
				else if (!(audioName == "magic_bullet_activate"))
				{
					goto IL_00D7;
				}
				return "UI/sceneloader_click";
			}
			IL_00D7:
			return "UI/notification";
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002544 File Offset: 0x00000744
		public void StopAllAudio()
		{
			try
			{
				foreach (Channel channel in this.activeChannels)
				{
					if (channel.hasHandle())
					{
						channel.stop();
					}
				}
				this.activeChannels.Clear();
				LogManager logManager = this.logManager;
				if (logManager != null)
				{
					logManager.WriteLog("Stopped all audio channels.", false);
				}
			}
			catch (Exception ex)
			{
				LogManager logManager2 = this.logManager;
				if (logManager2 != null)
				{
					logManager2.WriteLog(string.Format("Failed to stop all audio: {0}", ex), true);
				}
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000025F4 File Offset: 0x000007F4
		public void PlayESPAudio(ESPAudioType audioType, float volume = 1f)
		{
			string text;
			switch (audioType)
			{
			case ESPAudioType.PMCAlert:
				text = "pmc_alert";
				break;
			case ESPAudioType.TraderAlert:
				text = "trader_alert";
				break;
			case ESPAudioType.HighValueAlert:
				text = "high_value_alert";
				break;
			case ESPAudioType.MagicBulletActivate:
				text = "magic_bullet_activate";
				break;
			case ESPAudioType.MagicBulletDeactivate:
				text = "magic_bullet_deactivate";
				break;
			case ESPAudioType.EnemyDetected:
				text = "enemy_detected";
				break;
			case ESPAudioType.Notification:
				text = "notification";
				break;
			default:
				text = "notification";
				break;
			}
			string text2 = text;
			this.PlayCustomAudio(text2, volume, "bus:/Master/SFX");
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002674 File Offset: 0x00000874
		public void Dispose()
		{
			if (this.isDisposed)
			{
				return;
			}
			try
			{
				this.StopAllAudio();
				foreach (Sound sound in this.loadedSounds.Values)
				{
					sound.release();
				}
				this.loadedSounds.Clear();
				LogManager logManager = this.logManager;
				if (logManager != null)
				{
					logManager.WriteLog("AudioController disposed successfully.", false);
				}
			}
			catch (Exception ex)
			{
				LogManager logManager2 = this.logManager;
				if (logManager2 != null)
				{
					logManager2.WriteLog(string.Format("Error disposing AudioController: {0}", ex), true);
				}
			}
			finally
			{
				this.isDisposed = true;
			}
		}

		// Token: 0x04000003 RID: 3
		private readonly LogManager logManager;

		// Token: 0x04000004 RID: 4
		private readonly Dictionary<string, Sound> loadedSounds;

		// Token: 0x04000005 RID: 5
		private readonly List<Channel> activeChannels;

		// Token: 0x04000006 RID: 6
		private bool isDisposed;
	}
}
