using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x0200000F RID: 15
	[NullableContext(1)]
	[Nullable(0)]
	public class LogManager : IDisposable
	{
		// Token: 0x0600006B RID: 107 RVA: 0x00003FC6 File Offset: 0x000021C6
		public LogManager(string modPath, bool enableFileLogging)
		{
			this.logPath = Path.Combine(modPath, "YKF_ESP.log");
			this.logToFile = enableFileLogging;
			this.InitializeLogger();
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00003FEC File Offset: 0x000021EC
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

		// Token: 0x0600006D RID: 109 RVA: 0x00004014 File Offset: 0x00002214
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

		// Token: 0x0600006E RID: 110 RVA: 0x000040AC File Offset: 0x000022AC
		private void WriteToUnityConsole(string logMessage, bool isError)
		{
			if (isError)
			{
				Debug.LogError(logMessage);
				return;
			}
			Debug.Log(logMessage);
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000040C0 File Offset: 0x000022C0
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

		// Token: 0x06000070 RID: 112 RVA: 0x00004120 File Offset: 0x00002320
		private void DisposeWriter()
		{
			if (this.logWriter != null)
			{
				this.logWriter.Close();
				this.logWriter.Dispose();
				this.logWriter = null;
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00004147 File Offset: 0x00002347
		public void Dispose()
		{
			this.DisposeWriter();
		}

		// Token: 0x04000039 RID: 57
		private readonly string logPath;

		// Token: 0x0400003A RID: 58
		private StreamWriter logWriter;

		// Token: 0x0400003B RID: 59
		private bool logToFile;
	}
}
