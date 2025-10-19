using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Duckov
{
	// Token: 0x0200022A RID: 554
	public class AudioObject : MonoBehaviour
	{
		// Token: 0x17000300 RID: 768
		// (get) Token: 0x06001136 RID: 4406 RVA: 0x00042B4B File Offset: 0x00040D4B
		// (set) Token: 0x06001137 RID: 4407 RVA: 0x00042B53 File Offset: 0x00040D53
		public AudioManager.VoiceType VoiceType
		{
			get
			{
				return this.voiceType;
			}
			set
			{
				this.voiceType = value;
			}
		}

		// Token: 0x06001138 RID: 4408 RVA: 0x00042B5C File Offset: 0x00040D5C
		internal static AudioObject GetOrCreate(GameObject from)
		{
			AudioObject component = from.GetComponent<AudioObject>();
			if (component != null)
			{
				return component;
			}
			return from.AddComponent<AudioObject>();
		}

		// Token: 0x06001139 RID: 4409 RVA: 0x00042B84 File Offset: 0x00040D84
		public EventInstance? PostQuak(string soundKey)
		{
			string text = "Char/Voice/vo_" + this.voiceType.ToString().ToLower() + "_" + soundKey;
			return this.Post(text, true);
		}

		// Token: 0x0600113A RID: 4410 RVA: 0x00042BC0 File Offset: 0x00040DC0
		public EventInstance? Post(string eventName, bool doRelease = true)
		{
			EventInstance eventInstance;
			if (!AudioManager.TryCreateEventInstance(eventName ?? "", out eventInstance))
			{
				return null;
			}
			eventInstance.setCallback(new EVENT_CALLBACK(AudioObject.EventCallback), (EVENT_CALLBACK_TYPE)4294967295U);
			this.events.Add(eventInstance);
			eventInstance.set3DAttributes(base.gameObject.transform.position.To3DAttributes());
			this.ApplyParameters(eventInstance);
			eventInstance.start();
			if (doRelease)
			{
				eventInstance.release();
			}
			return new EventInstance?(eventInstance);
		}

		// Token: 0x0600113B RID: 4411 RVA: 0x00042C48 File Offset: 0x00040E48
		public void Stop(string eventName, global::FMOD.Studio.STOP_MODE mode)
		{
			foreach (EventInstance eventInstance in this.events)
			{
				EventDescription eventDescription;
				string text;
				if (eventInstance.getDescription(out eventDescription) == RESULT.OK && eventDescription.getPath(out text) == RESULT.OK && !("event:/" + text != eventName))
				{
					eventInstance.stop(mode);
					break;
				}
			}
		}

		// Token: 0x0600113C RID: 4412 RVA: 0x00042CC8 File Offset: 0x00040EC8
		private static RESULT EventCallback(EVENT_CALLBACK_TYPE type, IntPtr _event, IntPtr parameters)
		{
			if (type <= EVENT_CALLBACK_TYPE.PLUGIN_DESTROYED)
			{
				if (type <= EVENT_CALLBACK_TYPE.STOPPED)
				{
					if (type <= EVENT_CALLBACK_TYPE.STARTED)
					{
						switch (type)
						{
						case EVENT_CALLBACK_TYPE.CREATED:
						case EVENT_CALLBACK_TYPE.DESTROYED:
						case EVENT_CALLBACK_TYPE.CREATED | EVENT_CALLBACK_TYPE.DESTROYED:
						case EVENT_CALLBACK_TYPE.STARTING:
							break;
						default:
							if (type != EVENT_CALLBACK_TYPE.STARTED)
							{
							}
							break;
						}
					}
					else if (type != EVENT_CALLBACK_TYPE.RESTARTED && type != EVENT_CALLBACK_TYPE.STOPPED)
					{
					}
				}
				else if (type <= EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND)
				{
					if (type != EVENT_CALLBACK_TYPE.START_FAILED && type != EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND)
					{
					}
				}
				else if (type != EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND && type != EVENT_CALLBACK_TYPE.PLUGIN_CREATED && type != EVENT_CALLBACK_TYPE.PLUGIN_DESTROYED)
				{
				}
			}
			else if (type <= EVENT_CALLBACK_TYPE.SOUND_STOPPED)
			{
				if (type <= EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
				{
					if (type != EVENT_CALLBACK_TYPE.TIMELINE_MARKER && type != EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
					{
					}
				}
				else if (type != EVENT_CALLBACK_TYPE.SOUND_PLAYED && type != EVENT_CALLBACK_TYPE.SOUND_STOPPED)
				{
				}
			}
			else if (type <= EVENT_CALLBACK_TYPE.VIRTUAL_TO_REAL)
			{
				if (type != EVENT_CALLBACK_TYPE.REAL_TO_VIRTUAL && type != EVENT_CALLBACK_TYPE.VIRTUAL_TO_REAL)
				{
				}
			}
			else if (type == EVENT_CALLBACK_TYPE.START_EVENT_COMMAND || type != EVENT_CALLBACK_TYPE.NESTED_TIMELINE_BEAT)
			{
			}
			return RESULT.OK;
		}

		// Token: 0x0600113D RID: 4413 RVA: 0x00042DB8 File Offset: 0x00040FB8
		private void FixedUpdate()
		{
			if (this == null)
			{
				return;
			}
			if (base.transform == null)
			{
				return;
			}
			if (this.events == null)
			{
				return;
			}
			foreach (EventInstance eventInstance in this.events)
			{
				if (!eventInstance.isValid())
				{
					this.needCleanup = true;
				}
				else
				{
					eventInstance.set3DAttributes(base.transform.position.To3DAttributes());
				}
			}
			if (this.needCleanup)
			{
				this.events.RemoveAll((EventInstance e) => !e.isValid());
				this.needCleanup = false;
			}
		}

		// Token: 0x0600113E RID: 4414 RVA: 0x00042E8C File Offset: 0x0004108C
		internal void SetParameterByName(string parameter, float value)
		{
			this.parameters[parameter] = value;
			foreach (EventInstance eventInstance in this.events)
			{
				if (!eventInstance.isValid())
				{
					this.needCleanup = true;
				}
				else
				{
					eventInstance.setParameterByName(parameter, value, false);
				}
			}
		}

		// Token: 0x0600113F RID: 4415 RVA: 0x00042F04 File Offset: 0x00041104
		internal void SetParameterByNameWithLabel(string parameter, string label)
		{
			this.strParameters[parameter] = label;
			foreach (EventInstance eventInstance in this.events)
			{
				if (!eventInstance.isValid())
				{
					this.needCleanup = true;
				}
				else
				{
					eventInstance.setParameterByNameWithLabel(parameter, label, false);
				}
			}
		}

		// Token: 0x06001140 RID: 4416 RVA: 0x00042F7C File Offset: 0x0004117C
		private void ApplyParameters(EventInstance eventInstance)
		{
			foreach (KeyValuePair<string, float> keyValuePair in this.parameters)
			{
				eventInstance.setParameterByName(keyValuePair.Key, keyValuePair.Value, false);
			}
			foreach (KeyValuePair<string, string> keyValuePair2 in this.strParameters)
			{
				eventInstance.setParameterByNameWithLabel(keyValuePair2.Key, keyValuePair2.Value, false);
			}
		}

		// Token: 0x06001141 RID: 4417 RVA: 0x00043034 File Offset: 0x00041234
		internal void StopAll(global::FMOD.Studio.STOP_MODE mode = global::FMOD.Studio.STOP_MODE.IMMEDIATE)
		{
			foreach (EventInstance eventInstance in this.events)
			{
				if (!eventInstance.isValid())
				{
					this.needCleanup = true;
				}
				else
				{
					eventInstance.stop(mode);
				}
			}
		}

		// Token: 0x04000D67 RID: 3431
		private Dictionary<string, float> parameters = new Dictionary<string, float>();

		// Token: 0x04000D68 RID: 3432
		private Dictionary<string, string> strParameters = new Dictionary<string, string>();

		// Token: 0x04000D69 RID: 3433
		private AudioManager.VoiceType voiceType;

		// Token: 0x04000D6A RID: 3434
		public List<EventInstance> events = new List<EventInstance>();

		// Token: 0x04000D6B RID: 3435
		private bool needCleanup;
	}
}
