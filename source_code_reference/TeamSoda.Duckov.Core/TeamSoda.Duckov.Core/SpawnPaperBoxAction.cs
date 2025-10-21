using System;
using ItemStatsSystem;
using UnityEngine;

// Token: 0x02000086 RID: 134
public class SpawnPaperBoxAction : EffectAction
{
	// Token: 0x17000100 RID: 256
	// (get) Token: 0x060004CC RID: 1228 RVA: 0x00015D3F File Offset: 0x00013F3F
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

	// Token: 0x060004CD RID: 1229 RVA: 0x00015D7C File Offset: 0x00013F7C
	protected override void OnTriggered(bool positive)
	{
		if (!this.MainControl || !this.MainControl.characterModel)
		{
			return;
		}
		Transform transform = this.MainControl.transform;
		switch (this.socket)
		{
		case SpawnPaperBoxAction.Sockets.root:
			break;
		case SpawnPaperBoxAction.Sockets.helmat:
			transform = this.MainControl.characterModel.HelmatSocket;
			break;
		case SpawnPaperBoxAction.Sockets.armor:
			transform = this.MainControl.characterModel.ArmorSocket;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!transform)
		{
			return;
		}
		if (!this.paperBoxPrefab)
		{
			return;
		}
		this.instance = global::UnityEngine.Object.Instantiate<PaperBox>(this.paperBoxPrefab, transform);
		this.instance.character = this.MainControl;
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x00015E36 File Offset: 0x00014036
	private void OnDestroy()
	{
		if (this.instance)
		{
			global::UnityEngine.Object.Destroy(this.instance.gameObject);
		}
	}

	// Token: 0x0400040B RID: 1035
	public SpawnPaperBoxAction.Sockets socket = SpawnPaperBoxAction.Sockets.helmat;

	// Token: 0x0400040C RID: 1036
	public PaperBox paperBoxPrefab;

	// Token: 0x0400040D RID: 1037
	private PaperBox instance;

	// Token: 0x0400040E RID: 1038
	private CharacterMainControl _mainControl;

	// Token: 0x0200043B RID: 1083
	public enum Sockets
	{
		// Token: 0x04001A5C RID: 6748
		root,
		// Token: 0x04001A5D RID: 6749
		helmat,
		// Token: 0x04001A5E RID: 6750
		armor
	}
}
