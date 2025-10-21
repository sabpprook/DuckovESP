using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x02000012 RID: 18
	[NullableContext(1)]
	[Nullable(0)]
	public class LogManager : IDisposable
	{
		// Token: 0x0600009A RID: 154 RVA: 0x00005846 File Offset: 0x00003A46
		public LogManager(string modPath, bool enableFileLogging)
		{
			this.logPath = Path.Combine(modPath, "YKF_ESP.log");
			this.logToFile = enableFileLogging;
			this.InitializeLogger();
		}

		// Token: 0x0600009B RID: 155 RVA: 0x0000586C File Offset: 0x00003A6C
		public void UpdateSettings(bool enableFileLogging)
		{
			if (this.logToFile != enableFileLogging)
			{
				this.logToFile = enableFileLogging;
				if (this.logToFile)
				{
					this.InitializeLogger();
					return;
				}
				this.DisposeWriter();
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00005894 File Offset: 0x00003A94
		public void WriteLog(string message, bool isError = false)
		{
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			string text2 = string.Concat(new string[]
			{
				"[",
				text,
				"] ",
				isError ? "ERROR: " : "",
				message
			});
			if (this.logToFile && this.logWriter != null)
			{
				try
				{
					this.logWriter.WriteLine(text2);
					return;
				}
				catch
				{
					this.WriteToUnityConsole(text2, isError);
					return;
				}
			}
			this.WriteToUnityConsole(text2, isError);
		}

		// Token: 0x0600009D RID: 157 RVA: 0x0000592C File Offset: 0x00003B2C
		private void WriteToUnityConsole(string logMessage, bool isError)
		{
			if (isError)
			{
				Debug.LogError(logMessage);
				return;
			}
			Debug.Log(logMessage);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00005940 File Offset: 0x00003B40
		private void InitializeLogger()
		{
			try
			{
				if (this.logToFile)
				{
					this.logWriter = new StreamWriter(this.logPath, true);
					this.logWriter.AutoFlush = true;
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Failed to initialize file logger: {0}", ex));
				this.logToFile = false;
			}
		}

		// Token: 0x0600009F RID: 159 RVA: 0x000059A0 File Offset: 0x00003BA0
		private void DisposeWriter()
		{
			if (this.logWriter != null)
			{
				this.logWriter.Close();
				this.logWriter.Dispose();
				this.logWriter = null;
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x000059C7 File Offset: 0x00003BC7
		public void Dispose()
		{
			this.DisposeWriter();
		}

		// Token: 0x04000053 RID: 83
		private readonly string logPath;

		// Token: 0x04000054 RID: 84
		private StreamWriter logWriter;

		// Token: 0x04000055 RID: 85
		private bool logToFile;
	}
}
