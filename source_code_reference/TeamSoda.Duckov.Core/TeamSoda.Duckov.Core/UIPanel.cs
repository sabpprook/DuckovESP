using System;
using Duckov.UI.Animations;
using UnityEngine;

// Token: 0x02000153 RID: 339
public class UIPanel : MonoBehaviour
{
	// Token: 0x06000A6E RID: 2670 RVA: 0x0002DBC9 File Offset: 0x0002BDC9
	protected virtual void OnOpen()
	{
	}

	// Token: 0x06000A6F RID: 2671 RVA: 0x0002DBCB File Offset: 0x0002BDCB
	protected virtual void OnClose()
	{
	}

	// Token: 0x06000A70 RID: 2672 RVA: 0x0002DBCD File Offset: 0x0002BDCD
	protected virtual void OnChildOpened(UIPanel child)
	{
	}

	// Token: 0x06000A71 RID: 2673 RVA: 0x0002DBCF File Offset: 0x0002BDCF
	protected virtual void OnChildClosed(UIPanel child)
	{
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x0002DBD1 File Offset: 0x0002BDD1
	internal void Open(UIPanel parent = null, bool controlFadeGroup = true)
	{
		this.parent = parent;
		this.OnOpen();
		if (controlFadeGroup)
		{
			FadeGroup fadeGroup = this.fadeGroup;
			if (fadeGroup == null)
			{
				return;
			}
			fadeGroup.Show();
		}
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x0002DBF4 File Offset: 0x0002BDF4
	public void Close()
	{
		if (this.activeChild != null)
		{
			this.activeChild.Close();
		}
		this.OnClose();
		UIPanel uipanel = this.parent;
		if (uipanel != null)
		{
			uipanel.NotifyChildClosed(this);
		}
		FadeGroup fadeGroup = this.fadeGroup;
		if (fadeGroup == null)
		{
			return;
		}
		fadeGroup.Hide();
	}

	// Token: 0x06000A74 RID: 2676 RVA: 0x0002DC44 File Offset: 0x0002BE44
	public void OpenChild(UIPanel childPanel)
	{
		if (childPanel == null)
		{
			return;
		}
		if (this.activeChild != null)
		{
			this.activeChild.Close();
		}
		this.activeChild = childPanel;
		childPanel.Open(this, true);
		this.OnChildOpened(childPanel);
		if (this.hideWhenChildActive)
		{
			FadeGroup fadeGroup = this.fadeGroup;
			if (fadeGroup == null)
			{
				return;
			}
			fadeGroup.Hide();
		}
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x0002DCA2 File Offset: 0x0002BEA2
	private void NotifyChildClosed(UIPanel child)
	{
		this.OnChildClosed(child);
		if (this.hideWhenChildActive)
		{
			FadeGroup fadeGroup = this.fadeGroup;
			if (fadeGroup == null)
			{
				return;
			}
			fadeGroup.Show();
		}
	}

	// Token: 0x0400091B RID: 2331
	[SerializeField]
	protected FadeGroup fadeGroup;

	// Token: 0x0400091C RID: 2332
	[SerializeField]
	private bool hideWhenChildActive;

	// Token: 0x0400091D RID: 2333
	private UIPanel parent;

	// Token: 0x0400091E RID: 2334
	private UIPanel activeChild;
}
