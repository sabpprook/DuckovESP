using System;

// Token: 0x0200015D RID: 349
public interface ISingleSelectionMenu<EntryType> where EntryType : class
{
	// Token: 0x06000AAD RID: 2733
	EntryType GetSelection();

	// Token: 0x06000AAE RID: 2734
	bool SetSelection(EntryType selection);
}
