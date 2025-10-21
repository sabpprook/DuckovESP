using System;
using System.Collections.Generic;
using DG.Tweening;
using Duckov.Utilities;
using Saves;
using TMPro;
using UnityEngine;

namespace Duckov.MiniGames.SnakeForces
{
	// Token: 0x02000286 RID: 646
	public class SnakeForce : MiniGameBehaviour
	{
		// Token: 0x170003CF RID: 975
		// (get) Token: 0x060014B7 RID: 5303 RVA: 0x0004CBF0 File Offset: 0x0004ADF0
		public List<SnakeForce.Part> Snake
		{
			get
			{
				return this.snake;
			}
		}

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x060014B8 RID: 5304 RVA: 0x0004CBF8 File Offset: 0x0004ADF8
		public List<Vector2Int> Foods
		{
			get
			{
				return this.foods;
			}
		}

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x060014B9 RID: 5305 RVA: 0x0004CC00 File Offset: 0x0004AE00
		// (set) Token: 0x060014BA RID: 5306 RVA: 0x0004CC08 File Offset: 0x0004AE08
		public int Score
		{
			get
			{
				return this._score;
			}
			private set
			{
				this._score = value;
				Action<SnakeForce> onScoreChanged = this.OnScoreChanged;
				if (onScoreChanged == null)
				{
					return;
				}
				onScoreChanged(this);
			}
		}

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x060014BB RID: 5307 RVA: 0x0004CC22 File Offset: 0x0004AE22
		// (set) Token: 0x060014BC RID: 5308 RVA: 0x0004CC2E File Offset: 0x0004AE2E
		public static int HighScore
		{
			get
			{
				return SavesSystem.Load<int>("MiniGame/Snake/HighScore");
			}
			private set
			{
				SavesSystem.Save<int>("MiniGame/Snake/HighScore", value);
			}
		}

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x060014BD RID: 5309 RVA: 0x0004CC3B File Offset: 0x0004AE3B
		public SnakeForce.Part Head
		{
			get
			{
				if (this.snake.Count <= 0)
				{
					return null;
				}
				return this.snake[0];
			}
		}

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x060014BE RID: 5310 RVA: 0x0004CC59 File Offset: 0x0004AE59
		public SnakeForce.Part Tail
		{
			get
			{
				if (this.snake.Count <= 0)
				{
					return null;
				}
				List<SnakeForce.Part> list = this.snake;
				return list[list.Count - 1];
			}
		}

		// Token: 0x14000086 RID: 134
		// (add) Token: 0x060014BF RID: 5311 RVA: 0x0004CC80 File Offset: 0x0004AE80
		// (remove) Token: 0x060014C0 RID: 5312 RVA: 0x0004CCB8 File Offset: 0x0004AEB8
		public event Action<SnakeForce.Part> OnAddPart;

		// Token: 0x14000087 RID: 135
		// (add) Token: 0x060014C1 RID: 5313 RVA: 0x0004CCF0 File Offset: 0x0004AEF0
		// (remove) Token: 0x060014C2 RID: 5314 RVA: 0x0004CD28 File Offset: 0x0004AF28
		public event Action<SnakeForce.Part> OnRemovePart;

		// Token: 0x14000088 RID: 136
		// (add) Token: 0x060014C3 RID: 5315 RVA: 0x0004CD60 File Offset: 0x0004AF60
		// (remove) Token: 0x060014C4 RID: 5316 RVA: 0x0004CD98 File Offset: 0x0004AF98
		public event Action<SnakeForce> OnAfterTick;

		// Token: 0x14000089 RID: 137
		// (add) Token: 0x060014C5 RID: 5317 RVA: 0x0004CDD0 File Offset: 0x0004AFD0
		// (remove) Token: 0x060014C6 RID: 5318 RVA: 0x0004CE08 File Offset: 0x0004B008
		public event Action<SnakeForce> OnScoreChanged;

		// Token: 0x1400008A RID: 138
		// (add) Token: 0x060014C7 RID: 5319 RVA: 0x0004CE40 File Offset: 0x0004B040
		// (remove) Token: 0x060014C8 RID: 5320 RVA: 0x0004CE78 File Offset: 0x0004B078
		public event Action<SnakeForce> OnGameStart;

		// Token: 0x1400008B RID: 139
		// (add) Token: 0x060014C9 RID: 5321 RVA: 0x0004CEB0 File Offset: 0x0004B0B0
		// (remove) Token: 0x060014CA RID: 5322 RVA: 0x0004CEE8 File Offset: 0x0004B0E8
		public event Action<SnakeForce> OnGameOver;

		// Token: 0x1400008C RID: 140
		// (add) Token: 0x060014CB RID: 5323 RVA: 0x0004CF20 File Offset: 0x0004B120
		// (remove) Token: 0x060014CC RID: 5324 RVA: 0x0004CF58 File Offset: 0x0004B158
		public event Action<SnakeForce, Vector2Int> OnFoodEaten;

		// Token: 0x060014CD RID: 5325 RVA: 0x0004CF8D File Offset: 0x0004B18D
		protected override void Start()
		{
			base.Start();
			this.titleScreen.SetActive(true);
		}

		// Token: 0x060014CE RID: 5326 RVA: 0x0004CFA4 File Offset: 0x0004B1A4
		private void Restart()
		{
			this.Clear();
			this.gameOverScreen.SetActive(false);
			for (int i = this.borderXMin; i <= this.borderXMax; i++)
			{
				for (int j = this.borderYMin; j <= this.borderYMax; j++)
				{
					this.allCoords.Add(new Vector2Int(i, j));
				}
			}
			this.AddPart(new Vector2Int((this.borderXMax + this.borderXMin) / 2, (this.borderYMax + this.borderYMin) / 2), Vector2Int.up);
			this.Grow();
			this.Grow();
			this.AddFood(3);
			this.PunchCamera();
			this.playing = true;
			this.RefreshScoreText();
			this.highScoreText.text = string.Format("{0}", SnakeForce.HighScore);
			Action<SnakeForce> onGameStart = this.OnGameStart;
			if (onGameStart == null)
			{
				return;
			}
			onGameStart(this);
		}

		// Token: 0x060014CF RID: 5327 RVA: 0x0004D088 File Offset: 0x0004B288
		private void AddFood(int count = 3)
		{
			List<Vector2Int> list = new List<Vector2Int>(this.allCoords);
			foreach (SnakeForce.Part part in this.snake)
			{
				list.Remove(part.coord);
			}
			if (list.Count <= 0)
			{
				this.Win();
				return;
			}
			foreach (Vector2Int vector2Int in list.GetRandomSubSet(count))
			{
				this.foods.Add(vector2Int);
			}
		}

		// Token: 0x060014D0 RID: 5328 RVA: 0x0004D130 File Offset: 0x0004B330
		private void GameOver()
		{
			Action<SnakeForce> onGameOver = this.OnGameOver;
			if (onGameOver != null)
			{
				onGameOver(this);
			}
			bool flag = this.Score > SnakeForce.HighScore;
			if (this.Score > SnakeForce.HighScore)
			{
				SnakeForce.HighScore = this.Score;
			}
			this.highScoreIndicator.SetActive(flag);
			this.winIndicator.SetActive(this.won);
			this.scoreTextGameOver.text = string.Format("{0}", this.Score);
			this.gameOverScreen.SetActive(true);
			this.PunchCamera();
		}

		// Token: 0x060014D1 RID: 5329 RVA: 0x0004D1C4 File Offset: 0x0004B3C4
		private void Win()
		{
			this.won = true;
			this.GameOver();
		}

		// Token: 0x060014D2 RID: 5330 RVA: 0x0004D1D4 File Offset: 0x0004B3D4
		protected override void OnUpdate(float deltaTime)
		{
			Vector2 axis = base.Game.GetAxis(0);
			if (axis.sqrMagnitude > 0.1f)
			{
				Vector2Int vector2Int = default(Vector2Int);
				if (axis.x > 0f)
				{
					vector2Int = Vector2Int.right;
				}
				else if (axis.x < 0f)
				{
					vector2Int = Vector2Int.left;
				}
				else if (axis.y > 0f)
				{
					vector2Int = Vector2Int.up;
				}
				else if (axis.y < 0f)
				{
					vector2Int = Vector2Int.down;
				}
				if (this.lastFrameAxis != vector2Int)
				{
					this.axisInput = true;
				}
				this.lastFrameAxis = vector2Int;
			}
			else
			{
				this.lastFrameAxis = Vector2Int.zero;
			}
			if (this.freezeCountDown > 0.0)
			{
				this.freezeCountDown -= (double)Time.unscaledDeltaTime;
				return;
			}
			if (this.dead || this.won || !this.playing)
			{
				if (base.Game.GetButtonDown(MiniGame.Button.Start))
				{
					this.Restart();
				}
				return;
			}
			this.RefreshScoreText();
			bool flag = base.Game.GetButton(MiniGame.Button.B) || base.Game.GetButton(MiniGame.Button.A);
			this.tickETA -= deltaTime * (flag ? 10f : 1f);
			float num = ((this.playTick < (ulong)((long)this.maxSpeedTick)) ? (this.playTick / (float)this.maxSpeedTick) : 1f);
			float num2 = Mathf.Lerp(this.tickIntervalFrom, this.tickIntervalTo, this.speedCurve.Evaluate(num));
			if (this.tickETA <= 0f || this.axisInput)
			{
				this.Tick();
				this.tickETA = num2;
				this.axisInput = false;
			}
		}

		// Token: 0x060014D3 RID: 5331 RVA: 0x0004D387 File Offset: 0x0004B587
		private void RefreshScoreText()
		{
			this.scoreText.text = string.Format("{0}", this.Score);
		}

		// Token: 0x060014D4 RID: 5332 RVA: 0x0004D3A9 File Offset: 0x0004B5A9
		private void Tick()
		{
			this.playTick += 1UL;
			if (this.Head == null)
			{
				return;
			}
			this.HandleMovement();
			this.DetectDeath();
			this.HandleEatAndGrow();
			Action<SnakeForce> onAfterTick = this.OnAfterTick;
			if (onAfterTick == null)
			{
				return;
			}
			onAfterTick(this);
		}

		// Token: 0x060014D5 RID: 5333 RVA: 0x0004D3E8 File Offset: 0x0004B5E8
		private void HandleMovement()
		{
			Vector2Int vector2Int = this.lastFrameAxis;
			if ((!(vector2Int == -this.Head.direction) || this.snake.Count <= 1) && vector2Int != Vector2Int.zero)
			{
				this.Head.direction = vector2Int;
			}
			for (int i = this.snake.Count - 1; i >= 0; i--)
			{
				SnakeForce.Part part = this.snake[i];
				Vector2Int vector2Int2 = ((i > 0) ? this.snake[i - 1].coord : (part.coord + part.direction));
				if (i > 0)
				{
					part.direction = this.snake[i - 1].direction;
				}
				if (vector2Int2.x > this.borderXMax)
				{
					vector2Int2.x = this.borderXMin;
				}
				if (vector2Int2.y > this.borderYMax)
				{
					vector2Int2.y = this.borderYMin;
				}
				if (vector2Int2.x < this.borderXMin)
				{
					vector2Int2.x = this.borderXMax;
				}
				if (vector2Int2.y < this.borderYMin)
				{
					vector2Int2.y = this.borderYMax;
				}
				part.MoveTo(vector2Int2);
			}
		}

		// Token: 0x060014D6 RID: 5334 RVA: 0x0004D528 File Offset: 0x0004B728
		private void HandleEatAndGrow()
		{
			Vector2Int coord = this.Head.coord;
			if (this.foods.Remove(coord))
			{
				this.Grow();
				int score = this.Score;
				this.Score = score + 1;
				int num = 3 + Mathf.FloorToInt(Mathf.Log((float)this.Score, 2f));
				int num2 = Mathf.Max(0, num - this.foods.Count);
				this.AddFood(num2);
				Action<SnakeForce, Vector2Int> onFoodEaten = this.OnFoodEaten;
				if (onFoodEaten != null)
				{
					onFoodEaten(this, coord);
				}
				this.PunchCamera();
			}
		}

		// Token: 0x060014D7 RID: 5335 RVA: 0x0004D5B4 File Offset: 0x0004B7B4
		private void DetectDeath()
		{
			Vector2Int coord = this.Head.coord;
			for (int i = 1; i < this.snake.Count; i++)
			{
				if (this.snake[i].coord == coord)
				{
					this.dead = true;
					this.GameOver();
					return;
				}
			}
		}

		// Token: 0x060014D8 RID: 5336 RVA: 0x0004D60C File Offset: 0x0004B80C
		private SnakeForce.Part Grow()
		{
			if (this.snake.Count == 0)
			{
				Debug.LogError("Cannot grow the snake! It haven't been created yet.");
				return null;
			}
			SnakeForce.Part tail = this.Tail;
			Vector2Int vector2Int = tail.coord - tail.direction;
			return this.AddPart(vector2Int, tail.direction);
		}

		// Token: 0x060014D9 RID: 5337 RVA: 0x0004D658 File Offset: 0x0004B858
		private SnakeForce.Part AddPart(Vector2Int coord, Vector2Int direction)
		{
			SnakeForce.Part part = new SnakeForce.Part(this, coord, direction);
			this.snake.Add(part);
			Action<SnakeForce.Part> onAddPart = this.OnAddPart;
			if (onAddPart != null)
			{
				onAddPart(part);
			}
			return part;
		}

		// Token: 0x060014DA RID: 5338 RVA: 0x0004D68D File Offset: 0x0004B88D
		private bool RemovePart(SnakeForce.Part part)
		{
			if (!this.snake.Remove(part))
			{
				return false;
			}
			Action<SnakeForce.Part> onRemovePart = this.OnRemovePart;
			if (onRemovePart != null)
			{
				onRemovePart(part);
			}
			return true;
		}

		// Token: 0x060014DB RID: 5339 RVA: 0x0004D6B4 File Offset: 0x0004B8B4
		private void Clear()
		{
			this.titleScreen.SetActive(false);
			this.won = false;
			this.dead = false;
			this.Score = 0;
			this.playTick = 0UL;
			this.allCoords.Clear();
			this.foods.Clear();
			for (int i = this.snake.Count - 1; i >= 0; i--)
			{
				SnakeForce.Part part = this.snake[i];
				if (part == null)
				{
					this.snake.RemoveAt(i);
				}
				else
				{
					this.RemovePart(part);
				}
			}
		}

		// Token: 0x060014DC RID: 5340 RVA: 0x0004D740 File Offset: 0x0004B940
		private void PunchCamera()
		{
			this.freezeCountDown = 0.10000000149011612;
			this.cameraParent.DOKill(true);
			this.cameraParent.DOShakePosition(0.4f, 1f, 10, 90f, false, true);
			this.cameraParent.DOShakeRotation(0.4f, Vector3.forward, 10, 90f, true);
		}

		// Token: 0x04000F38 RID: 3896
		[SerializeField]
		private GameObject gameOverScreen;

		// Token: 0x04000F39 RID: 3897
		[SerializeField]
		private GameObject titleScreen;

		// Token: 0x04000F3A RID: 3898
		[SerializeField]
		private GameObject winIndicator;

		// Token: 0x04000F3B RID: 3899
		[SerializeField]
		private TextMeshProUGUI scoreText;

		// Token: 0x04000F3C RID: 3900
		[SerializeField]
		private TextMeshProUGUI highScoreText;

		// Token: 0x04000F3D RID: 3901
		[SerializeField]
		private GameObject highScoreIndicator;

		// Token: 0x04000F3E RID: 3902
		[SerializeField]
		private TextMeshProUGUI scoreTextGameOver;

		// Token: 0x04000F3F RID: 3903
		[SerializeField]
		private Transform cameraParent;

		// Token: 0x04000F40 RID: 3904
		[SerializeField]
		private float tickIntervalFrom = 0.5f;

		// Token: 0x04000F41 RID: 3905
		[SerializeField]
		private float tickIntervalTo = 0.01f;

		// Token: 0x04000F42 RID: 3906
		[SerializeField]
		private int maxSpeedTick = 4096;

		// Token: 0x04000F43 RID: 3907
		[SerializeField]
		private AnimationCurve speedCurve;

		// Token: 0x04000F44 RID: 3908
		[SerializeField]
		private int borderXMin = -10;

		// Token: 0x04000F45 RID: 3909
		[SerializeField]
		private int borderXMax = 10;

		// Token: 0x04000F46 RID: 3910
		[SerializeField]
		private int borderYMin = -10;

		// Token: 0x04000F47 RID: 3911
		[SerializeField]
		private int borderYMax = 10;

		// Token: 0x04000F48 RID: 3912
		private bool playing;

		// Token: 0x04000F49 RID: 3913
		private bool dead;

		// Token: 0x04000F4A RID: 3914
		private bool won;

		// Token: 0x04000F4B RID: 3915
		private List<SnakeForce.Part> snake = new List<SnakeForce.Part>();

		// Token: 0x04000F4C RID: 3916
		private List<Vector2Int> foods = new List<Vector2Int>();

		// Token: 0x04000F4D RID: 3917
		private int _score;

		// Token: 0x04000F4E RID: 3918
		public const string HighScoreKey = "MiniGame/Snake/HighScore";

		// Token: 0x04000F56 RID: 3926
		private float tickETA;

		// Token: 0x04000F57 RID: 3927
		private List<Vector2Int> allCoords = new List<Vector2Int>();

		// Token: 0x04000F58 RID: 3928
		private ulong playTick;

		// Token: 0x04000F59 RID: 3929
		private Vector2Int lastFrameAxis;

		// Token: 0x04000F5A RID: 3930
		private double freezeCountDown;

		// Token: 0x04000F5B RID: 3931
		private bool axisInput;

		// Token: 0x02000558 RID: 1368
		public class Part
		{
			// Token: 0x060027EE RID: 10222 RVA: 0x00092C2E File Offset: 0x00090E2E
			public Part(SnakeForce master, Vector2Int coord, Vector2Int direction)
			{
				this.Master = master;
				this.coord = coord;
				this.direction = direction;
			}

			// Token: 0x1700075F RID: 1887
			// (get) Token: 0x060027EF RID: 10223 RVA: 0x00092C4B File Offset: 0x00090E4B
			public bool IsHead
			{
				get
				{
					return this == this.Master.Head;
				}
			}

			// Token: 0x17000760 RID: 1888
			// (get) Token: 0x060027F0 RID: 10224 RVA: 0x00092C5B File Offset: 0x00090E5B
			public bool IsTail
			{
				get
				{
					return this == this.Master.Tail;
				}
			}

			// Token: 0x060027F1 RID: 10225 RVA: 0x00092C6B File Offset: 0x00090E6B
			internal void MoveTo(Vector2Int coord)
			{
				this.coord = coord;
				Action<SnakeForce.Part> onMove = this.OnMove;
				if (onMove == null)
				{
					return;
				}
				onMove(this);
			}

			// Token: 0x140000F6 RID: 246
			// (add) Token: 0x060027F2 RID: 10226 RVA: 0x00092C88 File Offset: 0x00090E88
			// (remove) Token: 0x060027F3 RID: 10227 RVA: 0x00092CC0 File Offset: 0x00090EC0
			public event Action<SnakeForce.Part> OnMove;

			// Token: 0x04001EFD RID: 7933
			public Vector2Int coord;

			// Token: 0x04001EFE RID: 7934
			public Vector2Int direction;

			// Token: 0x04001EFF RID: 7935
			public readonly SnakeForce Master;
		}
	}
}
