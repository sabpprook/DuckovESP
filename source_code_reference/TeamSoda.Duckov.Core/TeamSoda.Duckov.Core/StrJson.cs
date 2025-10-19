using System;
using System.Collections.Generic;
using System.Text;

// Token: 0x02000096 RID: 150
public class StrJson
{
	// Token: 0x06000520 RID: 1312 RVA: 0x00017310 File Offset: 0x00015510
	private StrJson(params string[] contentPairs)
	{
		this.entries = new List<StrJson.Entry>();
		for (int i = 0; i < contentPairs.Length - 1; i += 2)
		{
			this.entries.Add(new StrJson.Entry(contentPairs[i], contentPairs[i + 1]));
		}
	}

	// Token: 0x06000521 RID: 1313 RVA: 0x00017356 File Offset: 0x00015556
	public StrJson Add(string key, string value)
	{
		this.entries.Add(new StrJson.Entry(key, value));
		return this;
	}

	// Token: 0x06000522 RID: 1314 RVA: 0x0001736B File Offset: 0x0001556B
	public static StrJson Create(params string[] contentPairs)
	{
		return new StrJson(contentPairs);
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x00017374 File Offset: 0x00015574
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		for (int i = 0; i < this.entries.Count; i++)
		{
			StrJson.Entry entry = this.entries[i];
			if (i > 0)
			{
				stringBuilder.Append(",");
			}
			stringBuilder.Append(string.Concat(new string[] { "\"", entry.key, "\":\"", entry.value, "\"" }));
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	// Token: 0x040004A5 RID: 1189
	public List<StrJson.Entry> entries;

	// Token: 0x02000445 RID: 1093
	public struct Entry
	{
		// Token: 0x06002608 RID: 9736 RVA: 0x00084AF2 File Offset: 0x00082CF2
		public Entry(string key, string value)
		{
			this.key = key;
			this.value = value;
		}

		// Token: 0x04001A94 RID: 6804
		public string key;

		// Token: 0x04001A95 RID: 6805
		public string value;
	}
}
