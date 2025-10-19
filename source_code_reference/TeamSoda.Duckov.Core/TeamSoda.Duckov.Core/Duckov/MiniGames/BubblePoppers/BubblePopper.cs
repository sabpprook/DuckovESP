using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Duckov.Utilities;
using Saves;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.MiniGames.BubblePoppers
{
	// Token: 0x020002D7 RID: 727
	public class BubblePopper : MiniGameBehaviour
	{
		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x060016EB RID: 5867 RVA: 0x00053B18 File Offset: 0x00051D18
		public int AvaliableColorCount
		{
			get
			{
				return this.colorPallette.Length;
			}
		}

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x060016EC RID: 5868 RVA: 0x00053B22 File Offset: 0x00051D22
		public BubblePopperLayout Layout
		{
			get
			{
				return this.layout;
			}
		}

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x060016ED RID: 5869 RVA: 0x00053B2A File Offset: 0x00051D2A
		public float BubbleRadius
		{
			get
			{
				if (this.bubbleTemplate == null)
				{
					return 8f;
				}
				return this.bubbleTemplate.Radius;
			}
		}

		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x060016EE RID: 5870 RVA: 0x00053B4B File Offset: 0x00051D4B
		public Bubble BubbleTemplate
		{
			get
			{
				return this.bubbleTemplate;
			}
		}

		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x060016EF RID: 5871 RVA: 0x00053B54 File Offset: 0x00051D54
		private PrefabPool<Bubble> BubblePool
		{
			get
			{
				if (this._bubblePool == null)
				{
					this._bubblePool = new PrefabPool<Bubble>(this.bubbleTemplate, null, new Action<Bubble>(this.OnGetBubble), null, null, true, 10, 10000, null);
				}
				return this._bubblePool;
			}
		}

		// Token: 0x060016F0 RID: 5872 RVA: 0x00053B98 File Offset: 0x00051D98
		private void OnGetBubble(Bubble bubble)
		{
			bubble.Rest();
		}

		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x060016F1 RID: 5873 RVA: 0x00053BA0 File Offset: 0x00051DA0
		// (set) Token: 0x060016F2 RID: 5874 RVA: 0x00053BA8 File Offset: 0x00051DA8
		public BubblePopper.Status status { get; private set; }

		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x060016F3 RID: 5875 RVA: 0x00053BB1 File Offset: 0x00051DB1
		// (set) Token: 0x060016F4 RID: 5876 RVA: 0x00053BB9 File Offset: 0x00051DB9
		public int FloorStepETA { get; private set; }

		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x060016F5 RID: 5877 RVA: 0x00053BC2 File Offset: 0x00051DC2
		// (set) Token: 0x060016F6 RID: 5878 RVA: 0x00053BCA File Offset: 0x00051DCA
		public int Score
		{
			get
			{
				return this._score;
			}
			private set
			{
				this._score = value;
				this.RefreshScoreText();
			}
		}

		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x060016F7 RID: 5879 RVA: 0x00053BD9 File Offset: 0x00051DD9
		// (set) Token: 0x060016F8 RID: 5880 RVA: 0x00053BE5 File Offset: 0x00051DE5
		public static int HighScore
		{
			get
			{
				return SavesSystem.Load<int>("MiniGame/BubblePopper/HighScore");
			}
			set
			{
				SavesSystem.Save<int>("MiniGame/BubblePopper/HighScore", value);
			}
		}

		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x060016F9 RID: 5881 RVA: 0x00053BF2 File Offset: 0x00051DF2
		// (set) Token: 0x060016FA RID: 5882 RVA: 0x00053BFE File Offset: 0x00051DFE
		public static int HighLevel
		{
			get
			{
				return SavesSystem.Load<int>("MiniGame/BubblePopper/HighLevel");
			}
			set
			{
				SavesSystem.Save<int>("MiniGame/BubblePopper/HighLevel", value);
			}
		}

		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x060016FB RID: 5883 RVA: 0x00053C0B File Offset: 0x00051E0B
		// (set) Token: 0x060016FC RID: 5884 RVA: 0x00053C13 File Offset: 0x00051E13
		public bool Busy { get; private set; }

		// Token: 0x14000095 RID: 149
		// (add) Token: 0x060016FD RID: 5885 RVA: 0x00053C1C File Offset: 0x00051E1C
		// (remove) Token: 0x060016FE RID: 5886 RVA: 0x00053C50 File Offset: 0x00051E50
		public static event Action<int> OnLevelClear;

		// Token: 0x060016FF RID: 5887 RVA: 0x00053C83 File Offset: 0x00051E83
		protected override void Start()
		{
			base.Start();
			this.RefreshScoreText();
			this.RefreshLevelText();
			this.HideEndScreen();
			this.ShowStartScreen();
		}

		// Token: 0x06001700 RID: 5888 RVA: 0x00053CA4 File Offset: 0x00051EA4
		private void RefreshScoreText()
		{
			this.scoreText.text = string.Format("{0}", this.Score);
			this.highScoreText.text = string.Format("{0}", BubblePopper.HighScore);
		}

		// Token: 0x06001701 RID: 5889 RVA: 0x00053CF0 File Offset: 0x00051EF0
		private void RefreshLevelText()
		{
			this.levelText.text = string.Format("{0}", this.levelIndex);
		}

		// Token: 0x06001702 RID: 5890 RVA: 0x00053D12 File Offset: 0x00051F12
		protected override void OnUpdate(float deltaTime)
		{
			this.UpdateStatus(deltaTime);
			this.HandleInput(deltaTime);
			this.UpdateAimingLine();
		}

		// Token: 0x06001703 RID: 5891 RVA: 0x00053D28 File Offset: 0x00051F28
		private void ShowStartScreen()
		{
			this.startScreen.SetActive(true);
		}

		// Token: 0x06001704 RID: 5892 RVA: 0x00053D36 File Offset: 0x00051F36
		private void HideStartScreen()
		{
			this.startScreen.SetActive(false);
		}

		// Token: 0x06001705 RID: 5893 RVA: 0x00053D44 File Offset: 0x00051F44
		private void ShowEndScreen()
		{
			this.endScreen.SetActive(true);
			this.endScreenLevelText.text = string.Format("LEVEL {0}", this.levelIndex);
			this.endScreenScoreText.text = string.Format("{0}", this.Score);
			this.failIndicator.SetActive(this.fail);
			this.clearIndicator.SetActive(this.clear);
			this.newRecordIndicator.SetActive(this.isHighScore);
			this.allLevelsClearIndicator.SetActive(this.allLevelsClear);
		}

		// Token: 0x06001706 RID: 5894 RVA: 0x00053DE1 File Offset: 0x00051FE1
		private void HideEndScreen()
		{
			this.endScreen.SetActive(false);
		}

		// Token: 0x06001707 RID: 5895 RVA: 0x00053DF0 File Offset: 0x00051FF0
		private void NewGame()
		{
			this.playing = true;
			this.levelIndex = 0;
			this.Score = 0;
			this.isHighScore = false;
			this.HideStartScreen();
			this.HideEndScreen();
			int[] array = this.LoadLevelData(this.levelIndex);
			this.StartNewLevel(array);
			this.RefreshLevelText();
		}

		// Token: 0x06001708 RID: 5896 RVA: 0x00053E40 File Offset: 0x00052040
		private void NextLevel()
		{
			this.levelIndex++;
			this.HideStartScreen();
			this.HideEndScreen();
			int[] array = this.LoadLevelData(this.levelIndex);
			this.StartNewLevel(array);
			this.RefreshLevelText();
		}

		// Token: 0x06001709 RID: 5897 RVA: 0x00053E81 File Offset: 0x00052081
		private int[] LoadLevelData(int levelIndex)
		{
			return this.levelDataProvider.GetData(levelIndex);
		}

		// Token: 0x0600170A RID: 5898 RVA: 0x00053E90 File Offset: 0x00052090
		private Vector2Int LevelDataIndexToCoord(int index)
		{
			int num = this.layout.XCoordBorder.y - this.layout.XCoordBorder.x + 1;
			int num2 = index / num;
			return new Vector2Int(index % num, -num2);
		}

		// Token: 0x0600170B RID: 5899 RVA: 0x00053ED0 File Offset: 0x000520D0
		private void StartNewLevel(int[] levelData)
		{
			this.clear = false;
			this.fail = false;
			this.FloorStepETA = this.floorStepAfterShots;
			this.BubblePool.ReleaseAll();
			this.attachedBubbles.Clear();
			this.ResetFloor();
			for (int i = 0; i < levelData.Length; i++)
			{
				int num = levelData[i];
				if (num >= 0)
				{
					Vector2Int vector2Int = this.LevelDataIndexToCoord(i);
					Bubble bubble = this.BubblePool.Get(null);
					bubble.Setup(this, num);
					this.Set(bubble, vector2Int);
				}
			}
			this.PushRandomColor();
			this.PushRandomColor();
			this.SetStatus(BubblePopper.Status.Loaded);
		}

		// Token: 0x0600170C RID: 5900 RVA: 0x00053F61 File Offset: 0x00052161
		private void ResetFloor()
		{
			this.floorYCoord = this.initialFloorYCoord;
			this.RefreshLayoutPosition();
		}

		// Token: 0x0600170D RID: 5901 RVA: 0x00053F75 File Offset: 0x00052175
		private void StepFloor()
		{
			this.floorYCoord++;
			this.BeginMovingCeiling();
		}

		// Token: 0x0600170E RID: 5902 RVA: 0x00053F8C File Offset: 0x0005218C
		private void RefreshLayoutPosition()
		{
			Vector3 localPosition = this.layout.transform.localPosition;
			localPosition.y = (float)(-(float)(this.floorYCoord - this.initialFloorYCoord)) * this.BubbleRadius * BubblePopperLayout.YOffsetFactor;
			this.layout.transform.localPosition = localPosition;
		}

		// Token: 0x0600170F RID: 5903 RVA: 0x00053FE0 File Offset: 0x000521E0
		private void UpdateStatus(float deltaTime)
		{
			switch (this.status)
			{
			case BubblePopper.Status.Idle:
			case BubblePopper.Status.GameOver:
				if (base.Game.GetButtonDown(MiniGame.Button.Start))
				{
					if (!this.playing || this.fail || this.allLevelsClear)
					{
						this.NewGame();
						return;
					}
					this.NextLevel();
					return;
				}
				break;
			case BubblePopper.Status.Loaded:
				break;
			case BubblePopper.Status.Launched:
				this.UpdateLaunched(deltaTime);
				return;
			case BubblePopper.Status.Settled:
				this.UpdateSettled(deltaTime);
				break;
			default:
				return;
			}
		}

		// Token: 0x06001710 RID: 5904 RVA: 0x00054052 File Offset: 0x00052252
		private void BeginMovingCeiling()
		{
			this.movingCeiling = true;
			this.moveCeilingT = 0f;
			this.originalCeilingPos = this.layout.transform.localPosition;
		}

		// Token: 0x06001711 RID: 5905 RVA: 0x00054084 File Offset: 0x00052284
		private void UpdateMoveCeiling(float deltaTime)
		{
			this.moveCeilingT += deltaTime;
			if (this.moveCeilingT >= this.moveCeilingTime)
			{
				this.movingCeiling = false;
				this.RefreshLayoutPosition();
				return;
			}
			Vector3 vector = this.layout.transform.localPosition;
			Vector2 vector2 = new Vector2(vector.x, (float)(-(float)(this.floorYCoord - this.initialFloorYCoord)) * this.BubbleRadius * BubblePopperLayout.YOffsetFactor);
			float num = this.moveCeilingCurve.Evaluate(this.moveCeilingT / this.moveCeilingTime);
			vector = Vector2.LerpUnclamped(this.originalCeilingPos, vector2, num);
			this.layout.transform.localPosition = vector;
		}

		// Token: 0x06001712 RID: 5906 RVA: 0x00054132 File Offset: 0x00052332
		private void UpdateSettled(float deltaTime)
		{
			if (this.movingCeiling)
			{
				this.UpdateMoveCeiling(deltaTime);
				return;
			}
			if (this.CheckGameOver())
			{
				this.SetStatus(BubblePopper.Status.GameOver);
				return;
			}
			this.SetStatus(BubblePopper.Status.Loaded);
		}

		// Token: 0x06001713 RID: 5907 RVA: 0x0005415C File Offset: 0x0005235C
		private void HandleFloorStep()
		{
			int floorStepETA = this.FloorStepETA;
			this.FloorStepETA = floorStepETA - 1;
			if (this.FloorStepETA <= 0)
			{
				this.StepFloor();
				this.FloorStepETA = this.floorStepAfterShots;
			}
		}

		// Token: 0x06001714 RID: 5908 RVA: 0x00054194 File Offset: 0x00052394
		private bool CheckGameOver()
		{
			if (this.attachedBubbles.Count == 0)
			{
				this.clear = true;
				this.allLevelsClear = this.levelIndex >= this.levelDataProvider.TotalLevels;
				if (this.clear)
				{
					if (this.levelIndex > BubblePopper.HighLevel)
					{
						BubblePopper.HighLevel = this.levelIndex;
					}
					Action<int> onLevelClear = BubblePopper.OnLevelClear;
					if (onLevelClear != null)
					{
						onLevelClear(this.levelIndex);
					}
				}
				return true;
			}
			if (this.attachedBubbles.Keys.Any((Vector2Int e) => e.y <= this.floorYCoord))
			{
				this.fail = true;
				return true;
			}
			return false;
		}

		// Token: 0x06001715 RID: 5909 RVA: 0x00054234 File Offset: 0x00052434
		private void SetStatus(BubblePopper.Status newStatus)
		{
			this.OnExitStatus(this.status);
			this.status = newStatus;
			switch (this.status)
			{
			case BubblePopper.Status.Idle:
			case BubblePopper.Status.Loaded:
			case BubblePopper.Status.Launched:
				break;
			case BubblePopper.Status.Settled:
				this.PushRandomColor();
				this.HandleFloorStep();
				return;
			case BubblePopper.Status.GameOver:
				if (this.Score > BubblePopper.HighScore)
				{
					BubblePopper.HighScore = this.Score;
					this.isHighScore = true;
				}
				this.ShowGameOverScreen();
				break;
			default:
				return;
			}
		}

		// Token: 0x06001716 RID: 5910 RVA: 0x000542A8 File Offset: 0x000524A8
		private void ShowGameOverScreen()
		{
			this.ShowEndScreen();
		}

		// Token: 0x06001717 RID: 5911 RVA: 0x000542B0 File Offset: 0x000524B0
		private void OnExitStatus(BubblePopper.Status status)
		{
			switch (status)
			{
			default:
				return;
			}
		}

		// Token: 0x06001718 RID: 5912 RVA: 0x000542C8 File Offset: 0x000524C8
		private void Set(Bubble bubble, Vector2Int coord)
		{
			this.attachedBubbles[coord] = bubble;
			bubble.NotifyAttached(coord);
		}

		// Token: 0x06001719 RID: 5913 RVA: 0x000542E0 File Offset: 0x000524E0
		private void Attach(Bubble bubble, Vector2Int coord)
		{
			Bubble bubble2;
			if (this.attachedBubbles.TryGetValue(coord, out bubble2))
			{
				Debug.LogError("Target coord is occupied!");
				return;
			}
			this.Set(bubble, coord);
			List<Vector2Int> continousCoords = this.GetContinousCoords(coord);
			if (continousCoords.Count >= 3)
			{
				HashSet<Vector2Int> hashSet = new HashSet<Vector2Int>();
				int num = 0;
				foreach (Vector2Int vector2Int in continousCoords)
				{
					hashSet.AddRange(this.layout.GetAllNeighbourCoords(vector2Int, false));
					this.Explode(vector2Int, coord);
					num++;
				}
				this.PunchCamera();
				HashSet<Vector2Int> looseCoords = this.GetLooseCoords(hashSet);
				foreach (Vector2Int vector2Int2 in looseCoords)
				{
					this.Detach(vector2Int2);
				}
				this.CalculateAndAddScore(looseCoords, continousCoords);
			}
			this.Shockwave(coord, this.shockwaveStrength).Forget();
		}

		// Token: 0x0600171A RID: 5914 RVA: 0x000543F4 File Offset: 0x000525F4
		private void CalculateAndAddScore(HashSet<Vector2Int> detached, List<Vector2Int> exploded)
		{
			float count = (float)exploded.Count;
			int count2 = detached.Count;
			int num = Mathf.FloorToInt(Mathf.Pow(count, 2f)) * (1 + count2);
			this.Score += num;
		}

		// Token: 0x0600171B RID: 5915 RVA: 0x00054434 File Offset: 0x00052634
		private void Explode(Vector2Int coord, Vector2Int origin)
		{
			Bubble bubble;
			if (!this.attachedBubbles.TryGetValue(coord, out bubble))
			{
				return;
			}
			this.attachedBubbles.Remove(coord);
			if (bubble == null)
			{
				return;
			}
			bubble.NotifyExplode(origin);
		}

		// Token: 0x0600171C RID: 5916 RVA: 0x00054470 File Offset: 0x00052670
		private List<Vector2Int> GetContinousCoords(Vector2Int root)
		{
			List<Vector2Int> list = new List<Vector2Int>();
			Bubble bubble;
			if (!this.attachedBubbles.TryGetValue(root, out bubble))
			{
				return list;
			}
			if (bubble == null)
			{
				return list;
			}
			int colorIndex = bubble.ColorIndex;
			BubblePopper.<>c__DisplayClass117_0 CS$<>8__locals1;
			CS$<>8__locals1.visitedCoords = new HashSet<Vector2Int>();
			CS$<>8__locals1.coords = new Stack<Vector2Int>();
			BubblePopper.<GetContinousCoords>g__Push|117_0(root, ref CS$<>8__locals1);
			while (CS$<>8__locals1.coords.Count > 0)
			{
				Vector2Int vector2Int = CS$<>8__locals1.coords.Pop();
				Bubble bubble2;
				if (this.attachedBubbles.TryGetValue(vector2Int, out bubble2) && !(bubble2 == null) && bubble2.ColorIndex == colorIndex)
				{
					list.Add(vector2Int);
					foreach (Vector2Int vector2Int2 in this.layout.GetAllNeighbourCoords(vector2Int, false))
					{
						if (!CS$<>8__locals1.visitedCoords.Contains(vector2Int2))
						{
							BubblePopper.<GetContinousCoords>g__Push|117_0(vector2Int2, ref CS$<>8__locals1);
						}
					}
				}
			}
			return list;
		}

		// Token: 0x0600171D RID: 5917 RVA: 0x00054560 File Offset: 0x00052760
		private HashSet<Vector2Int> GetLooseCoords(HashSet<Vector2Int> roots)
		{
			BubblePopper.<>c__DisplayClass118_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.pendingRoots = roots.ToList<Vector2Int>();
			HashSet<Vector2Int> hashSet = new HashSet<Vector2Int>();
			while (CS$<>8__locals1.pendingRoots.Count > 0)
			{
				Vector2Int vector2Int = this.<GetLooseCoords>g__PopRoot|118_0(ref CS$<>8__locals1);
				List<Vector2Int> list;
				if (this.<GetLooseCoords>g__CheckConnectedLoose|118_1(vector2Int, out list, ref CS$<>8__locals1))
				{
					hashSet.AddRange(list);
				}
			}
			return hashSet;
		}

		// Token: 0x0600171E RID: 5918 RVA: 0x000545B8 File Offset: 0x000527B8
		private void Detach(Vector2Int coord)
		{
			Bubble bubble;
			if (!this.attachedBubbles.TryGetValue(coord, out bubble))
			{
				return;
			}
			this.attachedBubbles.Remove(coord);
			bubble.NotifyDetached();
		}

		// Token: 0x0600171F RID: 5919 RVA: 0x000545EC File Offset: 0x000527EC
		private void UpdateAimingLine()
		{
			this.aimingLine.gameObject.SetActive(this.status == BubblePopper.Status.Loaded);
			Matrix4x4 worldToLocalMatrix = this.layout.transform.worldToLocalMatrix;
			Vector3 vector = worldToLocalMatrix.MultiplyPoint(this.cannon.position);
			Vector3 vector2 = worldToLocalMatrix.MultiplyVector(this.cannon.up);
			Vector3 vector3 = vector2 * this.aimingDistance;
			BubblePopper.CastResult castResult = this.SlideCast(vector, vector3);
			vector.z = 0f;
			this.aimlinePoints[0] = vector;
			this.aimlinePoints[1] = castResult.endPosition;
			if (castResult.touchWall)
			{
				float num = Mathf.Max(this.aimingDistance - (castResult.endPosition - vector).magnitude, 0f);
				Vector2 vector4 = vector2;
				vector4.x *= -1f;
				this.aimlinePoints[2] = castResult.endPosition + vector4 * num;
			}
			else
			{
				this.aimlinePoints[2] = castResult.endPosition;
			}
			this.aimingLine.SetPositions(this.aimlinePoints);
		}

		// Token: 0x06001720 RID: 5920 RVA: 0x0005473B File Offset: 0x0005293B
		private void UpdateLaunched(float deltaTime)
		{
			if (this.activeBubble == null || this.activeBubble.status != Bubble.Status.Moving)
			{
				this.activeBubble = null;
				this.SetStatus(BubblePopper.Status.Settled);
			}
		}

		// Token: 0x06001721 RID: 5921 RVA: 0x00054768 File Offset: 0x00052968
		private void HandleInput(float deltaTime)
		{
			float x = base.Game.GetAxis(0).x;
			this.cannonAngle = Mathf.Clamp(this.cannonAngle - x * this.cannonRotateSpeed * deltaTime, this.cannonAngleRange.x, this.cannonAngleRange.y);
			this.cannon.rotation = Quaternion.Euler(0f, 0f, this.cannonAngle);
			this.duckAnimator.SetInteger("MovementDirection", (x > 0.01f) ? 1 : ((x < -0.01f) ? (-1) : 0));
			this.gear.Rotate(0f, 0f, x * this.cannonRotateSpeed * deltaTime);
			if (base.Game.GetButtonDown(MiniGame.Button.A))
			{
				this.LaunchBubble();
			}
		}

		// Token: 0x06001722 RID: 5922 RVA: 0x00054834 File Offset: 0x00052A34
		public void MoveBubble(Bubble bubble, float deltaTime)
		{
			if (bubble == null)
			{
				return;
			}
			Vector2 moveDirection = bubble.MoveDirection;
			float num = deltaTime * this.bubbleMoveSpeed;
			Matrix4x4 worldToLocalMatrix = this.layout.transform.worldToLocalMatrix;
			Matrix4x4 localToWorldMatrix = this.layout.transform.localToWorldMatrix;
			Vector2 normalized = moveDirection.normalized;
			Vector2 vector = worldToLocalMatrix.MultiplyPoint(bubble.transform.position);
			Vector2 vector2 = worldToLocalMatrix.MultiplyVector(moveDirection.normalized) * num;
			BubblePopper.CastResult castResult = this.SlideCast(vector, vector2);
			bubble.transform.position = localToWorldMatrix.MultiplyPoint(castResult.endPosition);
			if (!castResult.Collide)
			{
				return;
			}
			if (castResult.touchWall && (float)castResult.touchWallDirection * normalized.x > 0f)
			{
				moveDirection.x *= -1f;
				bubble.MoveDirection = moveDirection;
			}
			if (castResult.touchingBubble || castResult.touchCeiling)
			{
				this.Attach(bubble, castResult.endCoord);
			}
		}

		// Token: 0x06001723 RID: 5923 RVA: 0x00054950 File Offset: 0x00052B50
		private Bubble LaunchBubble(Vector2 origin, Vector2 direction, int colorIndex)
		{
			Bubble bubble = this.BubblePool.Get(null);
			bubble.transform.position = this.layout.transform.localToWorldMatrix.MultiplyPoint(origin);
			bubble.MoveDirection = direction;
			bubble.Setup(this, colorIndex);
			bubble.Launch(direction);
			return bubble;
		}

		// Token: 0x06001724 RID: 5924 RVA: 0x000549A8 File Offset: 0x00052BA8
		private void LaunchBubble()
		{
			if (this.status != BubblePopper.Status.Loaded)
			{
				return;
			}
			this.activeBubble = this.LaunchBubble(this.layout.transform.worldToLocalMatrix.MultiplyPoint(this.cannon.transform.position), this.layout.transform.worldToLocalMatrix.MultiplyVector(this.cannon.transform.up), this.loadedColor);
			this.loadedColor = -1;
			this.RefreshColorIndicators();
			this.SetStatus(BubblePopper.Status.Launched);
		}

		// Token: 0x06001725 RID: 5925 RVA: 0x00054A40 File Offset: 0x00052C40
		private void PunchLoadedIndicator()
		{
			this.loadedColorIndicator.transform.DOKill(true);
			this.loadedColorIndicator.transform.localPosition = Vector2.left * 15f;
			this.loadedColorIndicator.transform.DOLocalMove(Vector3.zero, 0.1f, true);
		}

		// Token: 0x06001726 RID: 5926 RVA: 0x00054AA0 File Offset: 0x00052CA0
		private void PunchWaitingIndicator()
		{
			this.waitingColorIndicator.transform.localPosition = Vector2.zero;
			this.waitingColorIndicator.transform.DOKill(true);
			this.waitingColorIndicator.transform.DOPunchPosition(Vector3.down * 5f, 0.5f, 10, 1f, true);
		}

		// Token: 0x06001727 RID: 5927 RVA: 0x00054B08 File Offset: 0x00052D08
		private void PushRandomColor()
		{
			this.loadedColor = this.waitingColor;
			this.waitingColor = global::UnityEngine.Random.Range(0, this.AvaliableColorCount);
			if (this.attachedBubbles.Count <= 0)
			{
				this.waitingColor = global::UnityEngine.Random.Range(0, this.AvaliableColorCount);
			}
			List<int> list = (from e in this.attachedBubbles.Values
				group e by e.ColorIndex into g
				select g.Key).ToList<int>();
			this.waitingColor = list.GetRandom<int>();
			this.RefreshColorIndicators();
			this.PunchLoadedIndicator();
			this.PunchWaitingIndicator();
		}

		// Token: 0x06001728 RID: 5928 RVA: 0x00054BCA File Offset: 0x00052DCA
		private void RefreshColorIndicators()
		{
			this.loadedColorIndicator.color = this.GetDisplayColor(this.loadedColor);
			this.waitingColorIndicator.color = this.GetDisplayColor(this.waitingColor);
		}

		// Token: 0x06001729 RID: 5929 RVA: 0x00054BFA File Offset: 0x00052DFA
		private bool IsCoordOccupied(Vector2Int coord, out Bubble touchingBubble, out bool ceiling)
		{
			ceiling = false;
			if (this.attachedBubbles.TryGetValue(coord, out touchingBubble))
			{
				return true;
			}
			if (coord.y > this.ceilingYCoord)
			{
				ceiling = true;
				return true;
			}
			return false;
		}

		// Token: 0x0600172A RID: 5930 RVA: 0x00054C28 File Offset: 0x00052E28
		public BubblePopper.CastResult SlideCast(Vector2 origin, Vector2 delta)
		{
			float num = delta.magnitude;
			Vector2 normalized = delta.normalized;
			float bubbleRadius = this.BubbleRadius;
			BubblePopper.CastResult castResult = default(BubblePopper.CastResult);
			castResult.origin = origin;
			castResult.castDirection = normalized;
			castResult.castDistance = num;
			Vector2 vector = origin + delta;
			float num2 = 1f;
			float num3 = this.layout.XPositionBorder.x + bubbleRadius;
			float num4 = this.layout.XPositionBorder.y - bubbleRadius;
			if (origin.x < num3 || origin.x > num4)
			{
				Vector2 vector2 = origin;
				vector2.x = Mathf.Clamp(vector2.x, num3 + 0.001f, num4 - 0.001f);
				castResult.endPosition = vector2;
				castResult.clipWall = true;
				castResult.collide = true;
			}
			else
			{
				if (vector.x < num3)
				{
					castResult.touchWall = true;
					num2 = Mathf.Abs(origin.x - num3) / Mathf.Abs(delta.x);
					castResult.touchWallDirection = -1;
				}
				else if (vector.x > num4)
				{
					castResult.touchWall = true;
					num2 = Mathf.Abs(num4 - origin.x) / Mathf.Abs(delta.x);
					castResult.touchWallDirection = 1;
				}
				delta *= num2;
				num = delta.magnitude;
				castResult.endPosition = origin + delta;
				List<Vector2Int> allPassingCoords = this.layout.GetAllPassingCoords(origin, normalized, delta.magnitude);
				float num5 = num;
				foreach (Vector2Int vector2Int in allPassingCoords)
				{
					Bubble bubble;
					bool flag;
					Vector2 vector3;
					if (this.IsCoordOccupied(vector2Int, out bubble, out flag) && this.BubbleCast(this.layout.CoordToLocalPosition(vector2Int), origin, normalized, num, out vector3))
					{
						float magnitude = (vector3 - origin).magnitude;
						if (magnitude < num5)
						{
							castResult.collide = true;
							castResult.touchingBubble = bubble;
							castResult.touchBubbleCoord = vector2Int;
							castResult.endPosition = vector3;
							castResult.touchCeiling = flag;
							num5 = magnitude;
							castResult.touchWall = false;
						}
					}
				}
			}
			castResult.endCoord = this.layout.LocalPositionToCoord(castResult.endPosition);
			return castResult;
		}

		// Token: 0x0600172B RID: 5931 RVA: 0x00054E78 File Offset: 0x00053078
		private bool BubbleCast(Vector2 pos, Vector2 origin, Vector2 direction, float distance, out Vector2 hitCircleCenter)
		{
			float bubbleRadius = this.BubbleRadius;
			hitCircleCenter = origin;
			Vector2 vector = pos - origin;
			float sqrMagnitude = vector.sqrMagnitude;
			float magnitude = vector.magnitude;
			if (magnitude > distance + 2f * bubbleRadius)
			{
				return false;
			}
			if (magnitude <= bubbleRadius * 2f)
			{
				hitCircleCenter = pos - 2f * vector.normalized * bubbleRadius;
				return true;
			}
			if (Vector2.Dot(vector, direction) < 0f)
			{
				return false;
			}
			float num = 0.017453292f * Vector2.Angle(vector, direction);
			float num2 = vector.magnitude * Mathf.Sin(num);
			if (num2 > 2f * bubbleRadius)
			{
				return false;
			}
			float num3 = num2 * num2;
			float num4 = bubbleRadius * bubbleRadius * 2f * 2f;
			float num5 = Mathf.Sqrt(sqrMagnitude - num3) - Mathf.Sqrt(num4 - num3);
			if (num5 > distance)
			{
				return false;
			}
			hitCircleCenter = origin + direction * num5;
			return true;
		}

		// Token: 0x0600172C RID: 5932 RVA: 0x00054F74 File Offset: 0x00053174
		private void OnDrawGizmos()
		{
			if (!this.drawGizmos)
			{
				return;
			}
			float bubbleRadius = this.BubbleRadius;
			Matrix4x4 worldToLocalMatrix = this.layout.transform.worldToLocalMatrix;
			Vector3 vector = worldToLocalMatrix.MultiplyPoint(this.cannon.position);
			Vector3 vector2 = worldToLocalMatrix.MultiplyVector(this.cannon.up);
			BubblePopper.CastResult castResult = this.SlideCast(vector, vector2 * this.distance);
			Gizmos.matrix = this.layout.transform.localToWorldMatrix;
			Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
			for (int i = this.layout.XCoordBorder.x; i <= this.layout.XCoordBorder.y; i++)
			{
				for (int j = this.floorYCoord; j <= this.ceilingYCoord; j++)
				{
					new Vector2Int(i, j);
					this.layout.GizmosDrawCoord(new Vector2Int(i, j), 0.25f);
				}
			}
			Gizmos.color = (castResult.Collide ? Color.red : Color.green);
			Gizmos.DrawWireSphere(vector, bubbleRadius);
			Gizmos.DrawWireSphere(castResult.endPosition, bubbleRadius);
			Gizmos.DrawLine(vector, castResult.endPosition);
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(this.layout.CoordToLocalPosition(castResult.endCoord), bubbleRadius * 0.8f);
			if (castResult.collide)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawWireSphere(this.layout.CoordToLocalPosition(castResult.touchBubbleCoord), bubbleRadius * 0.5f);
			}
		}

		// Token: 0x0600172D RID: 5933 RVA: 0x0005512D File Offset: 0x0005332D
		internal void Release(Bubble bubble)
		{
			this.BubblePool.Release(bubble);
		}

		// Token: 0x0600172E RID: 5934 RVA: 0x0005513B File Offset: 0x0005333B
		internal Color GetDisplayColor(int colorIndex)
		{
			if (colorIndex < 0)
			{
				return Color.clear;
			}
			if (colorIndex >= this.colorPallette.Length)
			{
				return Color.white;
			}
			return this.colorPallette[colorIndex];
		}

		// Token: 0x0600172F RID: 5935 RVA: 0x00055164 File Offset: 0x00053364
		private async UniTask Shockwave(Vector2Int origin, float amplitude)
		{
			HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
			List<Vector2Int> next = new List<Vector2Int>();
			visited.Add(origin);
			next.Add(origin);
			List<Vector2Int> buffer = new List<Vector2Int>();
			while (next.Count > 0)
			{
				buffer.Clear();
				foreach (Vector2Int vector2Int in next)
				{
					Bubble bubble;
					this.attachedBubbles.TryGetValue(vector2Int, out bubble);
					if (bubble != null)
					{
						bubble.Impact((vector2Int - origin).normalized * amplitude);
					}
					foreach (Vector2Int vector2Int2 in this.layout.GetAllNeighbourCoords(vector2Int, false))
					{
						if (!visited.Contains(vector2Int2) && vector2Int2.x >= this.layout.XCoordBorder.x && vector2Int2.x <= this.layout.XCoordBorder.y && vector2Int2.y <= this.ceilingYCoord && vector2Int2.y >= this.floorYCoord)
						{
							buffer.Add(vector2Int2);
						}
					}
				}
				next.Clear();
				visited.AddRange(buffer);
				next.AddRange(buffer);
				await UniTask.WaitForSeconds(0.025f, false, PlayerLoopTiming.Update, default(CancellationToken), false);
				amplitude *= 0.5f;
				if (base.gameObject == null)
				{
					return;
				}
			}
		}

		// Token: 0x06001730 RID: 5936 RVA: 0x000551B8 File Offset: 0x000533B8
		private void PunchCamera()
		{
			this.cameraParent.DOKill(true);
			this.cameraParent.DOShakePosition(0.4f, 1f, 10, 90f, false, true);
			this.cameraParent.DOShakeRotation(0.4f, Vector3.forward, 10, 90f, true);
		}

		// Token: 0x06001733 RID: 5939 RVA: 0x000552B0 File Offset: 0x000534B0
		[CompilerGenerated]
		internal static void <GetContinousCoords>g__Push|117_0(Vector2Int coord, ref BubblePopper.<>c__DisplayClass117_0 A_1)
		{
			A_1.coords.Push(coord);
			A_1.visitedCoords.Add(coord);
		}

		// Token: 0x06001734 RID: 5940 RVA: 0x000552CB File Offset: 0x000534CB
		[CompilerGenerated]
		private Vector2Int <GetLooseCoords>g__PopRoot|118_0(ref BubblePopper.<>c__DisplayClass118_0 A_1)
		{
			Vector2Int vector2Int = A_1.pendingRoots[0];
			A_1.pendingRoots.RemoveAt(0);
			return vector2Int;
		}

		// Token: 0x06001735 RID: 5941 RVA: 0x000552E8 File Offset: 0x000534E8
		[CompilerGenerated]
		private bool <GetLooseCoords>g__CheckConnectedLoose|118_1(Vector2Int root, out List<Vector2Int> connected, ref BubblePopper.<>c__DisplayClass118_0 A_3)
		{
			connected = new List<Vector2Int>();
			bool flag = true;
			Stack<Vector2Int> stack = new Stack<Vector2Int>();
			HashSet<Vector2Int> hashSet = new HashSet<Vector2Int>();
			stack.Push(root);
			hashSet.Add(root);
			while (stack.Count > 0)
			{
				Vector2Int vector2Int = stack.Pop();
				A_3.pendingRoots.Remove(vector2Int);
				if (this.attachedBubbles.ContainsKey(vector2Int))
				{
					if (vector2Int.y >= this.ceilingYCoord)
					{
						flag = false;
					}
					connected.Add(vector2Int);
					foreach (Vector2Int vector2Int2 in this.layout.GetAllNeighbourCoords(vector2Int, false))
					{
						if (!hashSet.Contains(vector2Int2))
						{
							stack.Push(vector2Int2);
							hashSet.Add(vector2Int2);
						}
					}
				}
			}
			return flag;
		}

		// Token: 0x040010C8 RID: 4296
		[SerializeField]
		private Bubble bubbleTemplate;

		// Token: 0x040010C9 RID: 4297
		[SerializeField]
		private BubblePopperLayout layout;

		// Token: 0x040010CA RID: 4298
		[SerializeField]
		private Image waitingColorIndicator;

		// Token: 0x040010CB RID: 4299
		[SerializeField]
		private Image loadedColorIndicator;

		// Token: 0x040010CC RID: 4300
		[SerializeField]
		private Transform cannon;

		// Token: 0x040010CD RID: 4301
		[SerializeField]
		private LineRenderer aimingLine;

		// Token: 0x040010CE RID: 4302
		[SerializeField]
		private Transform cameraParent;

		// Token: 0x040010CF RID: 4303
		[SerializeField]
		private Animator duckAnimator;

		// Token: 0x040010D0 RID: 4304
		[SerializeField]
		private Transform gear;

		// Token: 0x040010D1 RID: 4305
		[SerializeField]
		private TextMeshProUGUI scoreText;

		// Token: 0x040010D2 RID: 4306
		[SerializeField]
		private TextMeshProUGUI levelText;

		// Token: 0x040010D3 RID: 4307
		[SerializeField]
		private TextMeshProUGUI highScoreText;

		// Token: 0x040010D4 RID: 4308
		[SerializeField]
		private GameObject startScreen;

		// Token: 0x040010D5 RID: 4309
		[SerializeField]
		private GameObject endScreen;

		// Token: 0x040010D6 RID: 4310
		[SerializeField]
		private GameObject failIndicator;

		// Token: 0x040010D7 RID: 4311
		[SerializeField]
		private GameObject clearIndicator;

		// Token: 0x040010D8 RID: 4312
		[SerializeField]
		private GameObject newRecordIndicator;

		// Token: 0x040010D9 RID: 4313
		[SerializeField]
		private GameObject allLevelsClearIndicator;

		// Token: 0x040010DA RID: 4314
		[SerializeField]
		private TextMeshProUGUI endScreenLevelText;

		// Token: 0x040010DB RID: 4315
		[SerializeField]
		private TextMeshProUGUI endScreenScoreText;

		// Token: 0x040010DC RID: 4316
		[SerializeField]
		private BubblePopperLevelDataProvider levelDataProvider;

		// Token: 0x040010DD RID: 4317
		[SerializeField]
		private Color[] colorPallette;

		// Token: 0x040010DE RID: 4318
		[SerializeField]
		private float aimingDistance = 100f;

		// Token: 0x040010DF RID: 4319
		[SerializeField]
		private Vector2 cannonAngleRange = new Vector2(-45f, 45f);

		// Token: 0x040010E0 RID: 4320
		[SerializeField]
		private float cannonRotateSpeed = 20f;

		// Token: 0x040010E1 RID: 4321
		[SerializeField]
		private int ceilingYCoord;

		// Token: 0x040010E2 RID: 4322
		[SerializeField]
		private int initialFloorYCoord = -18;

		// Token: 0x040010E3 RID: 4323
		[SerializeField]
		private int floorStepAfterShots = 4;

		// Token: 0x040010E4 RID: 4324
		[SerializeField]
		private float bubbleMoveSpeed = 100f;

		// Token: 0x040010E5 RID: 4325
		private float shockwaveStrength = 2f;

		// Token: 0x040010E6 RID: 4326
		[SerializeField]
		private float moveCeilingTime = 1f;

		// Token: 0x040010E7 RID: 4327
		[SerializeField]
		private AnimationCurve moveCeilingCurve;

		// Token: 0x040010E8 RID: 4328
		private PrefabPool<Bubble> _bubblePool;

		// Token: 0x040010E9 RID: 4329
		private Dictionary<Vector2Int, Bubble> attachedBubbles = new Dictionary<Vector2Int, Bubble>();

		// Token: 0x040010EA RID: 4330
		private float cannonAngle;

		// Token: 0x040010EB RID: 4331
		private int waitingColor;

		// Token: 0x040010EC RID: 4332
		private int loadedColor;

		// Token: 0x040010ED RID: 4333
		private Bubble activeBubble;

		// Token: 0x040010EF RID: 4335
		private bool clear;

		// Token: 0x040010F0 RID: 4336
		private bool fail;

		// Token: 0x040010F1 RID: 4337
		private bool allLevelsClear;

		// Token: 0x040010F2 RID: 4338
		private bool playing;

		// Token: 0x040010F3 RID: 4339
		[SerializeField]
		private int floorYCoord;

		// Token: 0x040010F5 RID: 4341
		private int levelIndex;

		// Token: 0x040010F6 RID: 4342
		private int _score;

		// Token: 0x040010F7 RID: 4343
		private bool isHighScore;

		// Token: 0x040010F8 RID: 4344
		private const string HighScoreSaveKey = "MiniGame/BubblePopper/HighScore";

		// Token: 0x040010F9 RID: 4345
		private const string HighLevelSaveKey = "MiniGame/BubblePopper/HighLevel";

		// Token: 0x040010FB RID: 4347
		private const int CriticalCount = 3;

		// Token: 0x040010FD RID: 4349
		private bool movingCeiling;

		// Token: 0x040010FE RID: 4350
		private float moveCeilingT;

		// Token: 0x040010FF RID: 4351
		private Vector2 originalCeilingPos;

		// Token: 0x04001100 RID: 4352
		private Vector3[] aimlinePoints = new Vector3[3];

		// Token: 0x04001101 RID: 4353
		[SerializeField]
		private bool drawGizmos = true;

		// Token: 0x04001102 RID: 4354
		[SerializeField]
		private float distance;

		// Token: 0x0200056E RID: 1390
		public enum Status
		{
			// Token: 0x04001F63 RID: 8035
			Idle,
			// Token: 0x04001F64 RID: 8036
			Loaded,
			// Token: 0x04001F65 RID: 8037
			Launched,
			// Token: 0x04001F66 RID: 8038
			Settled,
			// Token: 0x04001F67 RID: 8039
			GameOver
		}

		// Token: 0x0200056F RID: 1391
		public struct CastResult
		{
			// Token: 0x17000763 RID: 1891
			// (get) Token: 0x06002824 RID: 10276 RVA: 0x000941E6 File Offset: 0x000923E6
			public bool Collide
			{
				get
				{
					return this.collide || this.clipWall || this.touchWall || this.touchingBubble;
				}
			}

			// Token: 0x04001F68 RID: 8040
			public Vector2 origin;

			// Token: 0x04001F69 RID: 8041
			public Vector2 castDirection;

			// Token: 0x04001F6A RID: 8042
			public float castDistance;

			// Token: 0x04001F6B RID: 8043
			public bool clipWall;

			// Token: 0x04001F6C RID: 8044
			public bool touchWall;

			// Token: 0x04001F6D RID: 8045
			public int touchWallDirection;

			// Token: 0x04001F6E RID: 8046
			public bool collide;

			// Token: 0x04001F6F RID: 8047
			public Bubble touchingBubble;

			// Token: 0x04001F70 RID: 8048
			public Vector2Int touchBubbleCoord;

			// Token: 0x04001F71 RID: 8049
			public bool touchCeiling;

			// Token: 0x04001F72 RID: 8050
			public Vector2 endPosition;

			// Token: 0x04001F73 RID: 8051
			public Vector2Int endCoord;
		}
	}
}
