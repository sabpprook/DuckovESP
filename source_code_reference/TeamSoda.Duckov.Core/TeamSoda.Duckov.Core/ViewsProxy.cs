using System;
using Duckov.BlackMarkets.UI;
using Duckov.Crops;
using Duckov.Crops.UI;
using Duckov.Endowment.UI;
using Duckov.MasterKeys.UI;
using Duckov.MiniGames;
using Duckov.MiniMaps.UI;
using Duckov.Quests.UI;
using Duckov.UI;
using UnityEngine;

// Token: 0x02000113 RID: 275
public class ViewsProxy : MonoBehaviour
{
	// Token: 0x0600094E RID: 2382 RVA: 0x00029109 File Offset: 0x00027309
	public void ShowInventoryView()
	{
		if (LevelManager.Instance.IsBaseLevel && PlayerStorage.Instance)
		{
			PlayerStorage.Instance.InteractableLootBox.InteractWithMainCharacter();
			return;
		}
		InventoryView.Show();
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x00029138 File Offset: 0x00027338
	public void ShowQuestView()
	{
		QuestView.Show();
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x0002913F File Offset: 0x0002733F
	public void ShowMapView()
	{
		MiniMapView.Show();
	}

	// Token: 0x06000951 RID: 2385 RVA: 0x00029146 File Offset: 0x00027346
	public void ShowKeyView()
	{
		MasterKeysView.Show();
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x0002914D File Offset: 0x0002734D
	public void ShowPlayerStats()
	{
		PlayerStatsView.Instance.Open(null);
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x0002915A File Offset: 0x0002735A
	public void ShowEndowmentView()
	{
		EndowmentSelectionPanel.Show();
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x00029161 File Offset: 0x00027361
	public void ShowMapSelectionView()
	{
		MapSelectionView.Instance.Open(null);
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x0002916E File Offset: 0x0002736E
	public void ShowRepairView()
	{
		ItemRepairView.Instance.Open(null);
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x0002917B File Offset: 0x0002737B
	public void ShowFormulasIndexView()
	{
		FormulasIndexView.Show();
	}

	// Token: 0x06000957 RID: 2391 RVA: 0x00029182 File Offset: 0x00027382
	public void ShowBitcoinView()
	{
		BitcoinMinerView.Show();
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x00029189 File Offset: 0x00027389
	public void ShowStorageDock()
	{
		StorageDock.Show();
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x00029190 File Offset: 0x00027390
	public void ShowBlackMarket_Demands()
	{
		BlackMarketView.Show(BlackMarketView.Mode.Demand);
	}

	// Token: 0x0600095A RID: 2394 RVA: 0x00029198 File Offset: 0x00027398
	public void ShowBlackMarket_Supplies()
	{
		BlackMarketView.Show(BlackMarketView.Mode.Supply);
	}

	// Token: 0x0600095B RID: 2395 RVA: 0x000291A0 File Offset: 0x000273A0
	public void ShowSleepView()
	{
		SleepView.Show();
	}

	// Token: 0x0600095C RID: 2396 RVA: 0x000291A7 File Offset: 0x000273A7
	public void ShowATMView()
	{
		ATMView.Show();
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x000291AE File Offset: 0x000273AE
	public void ShowDecomposeView()
	{
		ItemDecomposeView.Show();
	}

	// Token: 0x0600095E RID: 2398 RVA: 0x000291B5 File Offset: 0x000273B5
	public void ShowGardenView(Garden garnden)
	{
		GardenView.Show(garnden);
	}

	// Token: 0x0600095F RID: 2399 RVA: 0x000291BD File Offset: 0x000273BD
	public void ShowGamingConsoleView(GamingConsole console)
	{
		GamingConsoleView.Show(console);
	}
}
