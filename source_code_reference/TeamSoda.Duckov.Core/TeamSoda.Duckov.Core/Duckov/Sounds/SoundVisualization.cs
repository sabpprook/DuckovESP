using System;
using System.Collections.Generic;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.Sounds
{
	// Token: 0x02000246 RID: 582
	public class SoundVisualization : MonoBehaviour
	{
		// Token: 0x1700032E RID: 814
		// (get) Token: 0x06001212 RID: 4626 RVA: 0x00044D6C File Offset: 0x00042F6C
		private PrefabPool<SoundDisplay> DisplayPool
		{
			get
			{
				if (this._displayPool == null)
				{
					this._displayPool = new PrefabPool<SoundDisplay>(this.displayTemplate, null, null, null, null, true, 10, 10000, null);
				}
				return this._displayPool;
			}
		}

		// Token: 0x06001213 RID: 4627 RVA: 0x00044DA5 File Offset: 0x00042FA5
		private void Awake()
		{
			AIMainBrain.OnPlayerHearSound += this.OnHeardSound;
			if (this.layoutCenter == null)
			{
				this.layoutCenter = base.transform as RectTransform;
			}
		}

		// Token: 0x06001214 RID: 4628 RVA: 0x00044DD7 File Offset: 0x00042FD7
		private void OnDestroy()
		{
			AIMainBrain.OnPlayerHearSound -= this.OnHeardSound;
		}

		// Token: 0x06001215 RID: 4629 RVA: 0x00044DEC File Offset: 0x00042FEC
		private void Update()
		{
			using (IEnumerator<SoundDisplay> enumerator = this.DisplayPool.ActiveEntries.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SoundDisplay soundDisplay = enumerator.Current;
					if (soundDisplay.Value <= 0f)
					{
						this.releaseBuffer.Enqueue(soundDisplay);
					}
					else
					{
						this.RefreshEntryPosition(soundDisplay);
					}
				}
				goto IL_0071;
			}
			IL_0050:
			SoundDisplay soundDisplay2 = this.releaseBuffer.Dequeue();
			if (!(soundDisplay2 == null))
			{
				this.DisplayPool.Release(soundDisplay2);
			}
			IL_0071:
			if (this.releaseBuffer.Count <= 0)
			{
				return;
			}
			goto IL_0050;
		}

		// Token: 0x06001216 RID: 4630 RVA: 0x00044E88 File Offset: 0x00043088
		private void OnHeardSound(AISound sound)
		{
			this.Trigger(sound);
		}

		// Token: 0x06001217 RID: 4631 RVA: 0x00044E94 File Offset: 0x00043094
		private void Trigger(AISound sound)
		{
			if (GameCamera.Instance == null)
			{
				return;
			}
			SoundDisplay soundDisplay = null;
			if (sound.fromCharacter != null)
			{
				foreach (SoundDisplay soundDisplay2 in this.DisplayPool.ActiveEntries)
				{
					AISound currentSount = soundDisplay2.CurrentSount;
					if (!(currentSount.fromCharacter != sound.fromCharacter) && currentSount.soundType == sound.soundType && Vector3.Distance(currentSount.pos, sound.pos) < this.retriggerDistanceThreshold)
					{
						soundDisplay = soundDisplay2;
					}
				}
			}
			if (soundDisplay == null)
			{
				soundDisplay = this.DisplayPool.Get(null);
			}
			this.RefreshEntryPosition(soundDisplay);
			soundDisplay.Trigger(sound);
		}

		// Token: 0x06001218 RID: 4632 RVA: 0x00044F64 File Offset: 0x00043164
		private void RefreshEntryPosition(SoundDisplay e)
		{
			Vector3 pos = e.CurrentSount.pos;
			Vector2 vector = RectTransformUtility.WorldToScreenPoint(GameCamera.Instance.renderCamera, pos);
			Vector2 vector2;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.layoutCenter, vector, null, out vector2);
			Vector2 normalized = vector2.normalized;
			e.transform.localPosition = normalized * this.displayOffset;
			e.transform.rotation = Quaternion.FromToRotation(Vector2.up, normalized);
		}

		// Token: 0x04000DE9 RID: 3561
		[SerializeField]
		private RectTransform layoutCenter;

		// Token: 0x04000DEA RID: 3562
		[SerializeField]
		private SoundDisplay displayTemplate;

		// Token: 0x04000DEB RID: 3563
		[SerializeField]
		private float retriggerDistanceThreshold = 1f;

		// Token: 0x04000DEC RID: 3564
		[SerializeField]
		private float displayOffset = 400f;

		// Token: 0x04000DED RID: 3565
		private PrefabPool<SoundDisplay> _displayPool;

		// Token: 0x04000DEE RID: 3566
		private Queue<SoundDisplay> releaseBuffer = new Queue<SoundDisplay>();
	}
}
