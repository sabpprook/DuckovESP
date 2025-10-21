using System;
using System.Collections.Generic;
using Duckov.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200010A RID: 266
public class PlayerPositionBackupManager : MonoBehaviour
{
	// Token: 0x14000045 RID: 69
	// (add) Token: 0x0600091A RID: 2330 RVA: 0x000286D4 File Offset: 0x000268D4
	// (remove) Token: 0x0600091B RID: 2331 RVA: 0x00028708 File Offset: 0x00026908
	private static event Action OnStartRecoverEvent;

	// Token: 0x0600091C RID: 2332 RVA: 0x0002873B File Offset: 0x0002693B
	private void Awake()
	{
		this.backups = new List<PlayerPositionBackupManager.PlayerPositionBackupEntry>();
		MultiSceneCore.OnSubSceneLoaded += this.OnSubSceneLoaded;
		PlayerPositionBackupManager.OnStartRecoverEvent += this.OnStartRecover;
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x0002876A File Offset: 0x0002696A
	private void OnDestroy()
	{
		MultiSceneCore.OnSubSceneLoaded -= this.OnSubSceneLoaded;
		PlayerPositionBackupManager.OnStartRecoverEvent -= this.OnStartRecover;
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x00028790 File Offset: 0x00026990
	private void Update()
	{
		if (!LevelManager.LevelInited)
		{
			return;
		}
		if (!this.mainCharacter)
		{
			this.mainCharacter = CharacterMainControl.Main;
		}
		if (!this.mainCharacter)
		{
			return;
		}
		this.backupTimer -= Time.deltaTime;
		if (this.backupTimer < 0f && this.CheckCanBackup())
		{
			this.BackupCurrentPos();
		}
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x000287F8 File Offset: 0x000269F8
	private bool CheckCanBackup()
	{
		if (!this.mainCharacter)
		{
			return false;
		}
		if (!this.mainCharacter.IsOnGround)
		{
			return false;
		}
		if (Mathf.Abs(this.mainCharacter.Velocity.y) > 2f)
		{
			return false;
		}
		int count = this.backups.Count;
		if (count > 0)
		{
			Vector3 position = this.backups[count - 1].position;
			if (Vector3.Distance(this.mainCharacter.transform.position, position) < this.minBackupDistance)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x00028886 File Offset: 0x00026A86
	private void OnSubSceneLoaded(MultiSceneCore multiSceneCore, Scene scene)
	{
		this.backups.Clear();
		this.backupTimer = this.backupTimeSpace;
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x000288A0 File Offset: 0x00026AA0
	public void BackupCurrentPos()
	{
		if (!LevelManager.LevelInited)
		{
			return;
		}
		if (!this.mainCharacter)
		{
			return;
		}
		this.backupTimer = this.backupTimeSpace;
		PlayerPositionBackupManager.PlayerPositionBackupEntry playerPositionBackupEntry = default(PlayerPositionBackupManager.PlayerPositionBackupEntry);
		playerPositionBackupEntry.position = this.mainCharacter.transform.position;
		playerPositionBackupEntry.sceneID = SceneManager.GetActiveScene().buildIndex;
		this.backups.Add(playerPositionBackupEntry);
		if (this.backups.Count > this.listSize)
		{
			this.backups.RemoveAt(0);
		}
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x0002892D File Offset: 0x00026B2D
	public static void StartRecover()
	{
		Action onStartRecoverEvent = PlayerPositionBackupManager.OnStartRecoverEvent;
		if (onStartRecoverEvent == null)
		{
			return;
		}
		onStartRecoverEvent();
	}

	// Token: 0x06000923 RID: 2339 RVA: 0x00028940 File Offset: 0x00026B40
	private void OnStartRecover()
	{
		if (this.mainCharacter.CurrentAction != null && this.mainCharacter.CurrentAction.Running)
		{
			this.mainCharacter.CurrentAction.StopAction();
		}
		this.mainCharacter.Interact(this.backupInteract);
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x00028994 File Offset: 0x00026B94
	public void SetPlayerToBackupPos()
	{
		if (this.backups.Count <= 0)
		{
			return;
		}
		int buildIndex = SceneManager.GetActiveScene().buildIndex;
		Vector3 position = this.mainCharacter.transform.position;
		ref PlayerPositionBackupManager.PlayerPositionBackupEntry ptr = this.backups[this.backups.Count - 1];
		this.backups.RemoveAt(this.backups.Count - 1);
		Vector3 position2 = ptr.position;
		if (Vector3.Distance(position, position2) > this.minBackupDistance)
		{
			this.mainCharacter.SetPosition(position2);
			return;
		}
		this.SetPlayerToBackupPos();
	}

	// Token: 0x0400082B RID: 2091
	private List<PlayerPositionBackupManager.PlayerPositionBackupEntry> backups;

	// Token: 0x0400082C RID: 2092
	private CharacterMainControl mainCharacter;

	// Token: 0x0400082D RID: 2093
	public float backupTimeSpace = 3f;

	// Token: 0x0400082E RID: 2094
	public float minBackupDistance = 3f;

	// Token: 0x0400082F RID: 2095
	private float backupTimer = 3f;

	// Token: 0x04000830 RID: 2096
	public InteractableBase backupInteract;

	// Token: 0x04000831 RID: 2097
	public int listSize = 20;

	// Token: 0x0200048E RID: 1166
	private struct PlayerPositionBackupEntry
	{
		// Token: 0x04001BBB RID: 7099
		public int sceneID;

		// Token: 0x04001BBC RID: 7100
		public Vector3 position;
	}
}
