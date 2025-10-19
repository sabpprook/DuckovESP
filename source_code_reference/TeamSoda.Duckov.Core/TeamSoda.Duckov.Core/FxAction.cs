using System;
using ItemStatsSystem;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000082 RID: 130
public class FxAction : EffectAction
{
	// Token: 0x170000FD RID: 253
	// (get) Token: 0x060004BE RID: 1214 RVA: 0x000159CF File Offset: 0x00013BCF
	private CharacterMainControl MainControl
	{
		get
		{
			if (this._mainControl == null)
			{
				Effect master = base.Master;
				CharacterMainControl characterMainControl;
				if (master == null)
				{
					characterMainControl = null;
				}
				else
				{
					Item item = master.Item;
					characterMainControl = ((item != null) ? item.GetCharacterMainControl() : null);
				}
				this._mainControl = characterMainControl;
			}
			return this._mainControl;
		}
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x00015A0C File Offset: 0x00013C0C
	protected override void OnTriggered(bool positive)
	{
		if (!this.MainControl || !this.MainControl.characterModel)
		{
			return;
		}
		Transform transform = this.MainControl.transform;
		switch (this.socket)
		{
		case FxAction.Sockets.root:
			break;
		case FxAction.Sockets.helmat:
			transform = this.MainControl.characterModel.HelmatSocket;
			break;
		case FxAction.Sockets.armor:
			transform = this.MainControl.characterModel.ArmorSocket;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!transform)
		{
			return;
		}
		if (!this.fxPfb)
		{
			return;
		}
		global::UnityEngine.Object.Instantiate<GameObject>(this.fxPfb, transform.position, quaternion.identity);
	}

	// Token: 0x040003FB RID: 1019
	public FxAction.Sockets socket = FxAction.Sockets.helmat;

	// Token: 0x040003FC RID: 1020
	public GameObject fxPfb;

	// Token: 0x040003FD RID: 1021
	private CharacterMainControl _mainControl;

	// Token: 0x0200043A RID: 1082
	public enum Sockets
	{
		// Token: 0x04001A58 RID: 6744
		root,
		// Token: 0x04001A59 RID: 6745
		helmat,
		// Token: 0x04001A5A RID: 6746
		armor
	}
}
