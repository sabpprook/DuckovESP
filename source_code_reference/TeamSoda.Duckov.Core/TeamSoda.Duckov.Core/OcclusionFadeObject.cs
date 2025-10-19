using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000187 RID: 391
public class OcclusionFadeObject : MonoBehaviour
{
	// Token: 0x06000BA8 RID: 2984 RVA: 0x000317B3 File Offset: 0x0002F9B3
	private void Collect()
	{
		this.CollectTriggers();
		this.CollectRenderers();
	}

	// Token: 0x06000BA9 RID: 2985 RVA: 0x000317C4 File Offset: 0x0002F9C4
	private void CollectTriggers()
	{
		this.triggers = new OcclusionFadeTrigger[0];
		this.triggers = base.GetComponentsInChildren<OcclusionFadeTrigger>();
		if (this.triggers.Length != 0)
		{
			foreach (OcclusionFadeTrigger occlusionFadeTrigger in this.triggers)
			{
				occlusionFadeTrigger.parent = this;
				Collider[] componentsInChildren = occlusionFadeTrigger.GetComponentsInChildren<Collider>(true);
				if (componentsInChildren.Length != 0)
				{
					Collider[] array2 = componentsInChildren;
					for (int j = 0; j < array2.Length; j++)
					{
						array2[j].isTrigger = true;
					}
				}
			}
		}
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x0003183C File Offset: 0x0002FA3C
	private void CollectRenderers()
	{
		this.topTransform = this.FindFirst(base.transform, this.topName);
		if (this.topTransform == null)
		{
			return;
		}
		this.renderers = this.topTransform.GetComponentsInChildren<Renderer>(true);
		this.originMaterials.Clear();
		foreach (Renderer renderer in this.renderers)
		{
			this.originMaterials.AddRange(renderer.sharedMaterials);
		}
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x000318B7 File Offset: 0x0002FAB7
	public void OnEnter()
	{
		this.enterCounter++;
		this.Refresh();
	}

	// Token: 0x06000BAC RID: 2988 RVA: 0x000318CD File Offset: 0x0002FACD
	public void OnLeave()
	{
		this.enterCounter--;
		this.Refresh();
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x000318E4 File Offset: 0x0002FAE4
	private void Refresh()
	{
		this.SyncEnable();
		if (!this.triggerEnabled)
		{
			this.hiding = false;
			this.Sync();
			return;
		}
		if (this.enterCounter > 0 && !this.hiding)
		{
			this.hiding = true;
			this.Sync();
			return;
		}
		if (this.enterCounter <= 0 && this.hiding)
		{
			this.hiding = false;
			this.Sync();
		}
	}

	// Token: 0x06000BAE RID: 2990 RVA: 0x0003194A File Offset: 0x0002FB4A
	private void OnEnable()
	{
		this.SyncEnable();
	}

	// Token: 0x06000BAF RID: 2991 RVA: 0x00031952 File Offset: 0x0002FB52
	private void OnDisable()
	{
		this.SyncEnable();
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x0003195C File Offset: 0x0002FB5C
	private void SyncEnable()
	{
		if (this.triggerEnabled != base.enabled)
		{
			OcclusionFadeTrigger[] array = this.triggers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(base.enabled);
			}
			this.triggerEnabled = base.enabled;
		}
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x000319AC File Offset: 0x0002FBAC
	private void Sync()
	{
		this.SyncEnable();
		OcclusionFadeTypes occlusionFadeTypes = this.fadeType;
		if (occlusionFadeTypes != OcclusionFadeTypes.Fade)
		{
			if (occlusionFadeTypes != OcclusionFadeTypes.ShadowOnly)
			{
				return;
			}
			if (this.hiding)
			{
				foreach (Renderer renderer in this.renderers)
				{
					if (!(renderer == null))
					{
						renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
					}
				}
				return;
			}
			foreach (Renderer renderer2 in this.renderers)
			{
				if (!(renderer2 == null))
				{
					renderer2.shadowCastingMode = ShadowCastingMode.On;
				}
			}
			return;
		}
		else
		{
			if (this.tempMaterials == null)
			{
				this.tempMaterials = new List<Material>();
			}
			if (this.hiding)
			{
				int num = 0;
				foreach (Renderer renderer3 in this.renderers)
				{
					if (!(renderer3 == null))
					{
						this.tempMaterials.Clear();
						for (int j = 0; j < renderer3.materials.Length; j++)
						{
							Material material = this.originMaterials[num];
							Material maskedMaterial = OcclusionFadeManager.Instance.GetMaskedMaterial(material);
							this.tempMaterials.Add(maskedMaterial);
							num++;
						}
						renderer3.SetSharedMaterials(this.tempMaterials);
					}
				}
				return;
			}
			int num2 = 0;
			foreach (Renderer renderer4 in this.renderers)
			{
				if (!(renderer4 == null))
				{
					this.tempMaterials.Clear();
					for (int k = 0; k < renderer4.materials.Length; k++)
					{
						this.tempMaterials.Add(this.originMaterials[num2]);
						num2++;
					}
					renderer4.SetSharedMaterials(this.tempMaterials);
				}
			}
			return;
		}
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x00031B4C File Offset: 0x0002FD4C
	private void Hide()
	{
		for (int i = 0; i < this.renderers.Length; i++)
		{
			Renderer renderer = this.renderers[i];
			if (renderer != null)
			{
				renderer.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000BB3 RID: 2995 RVA: 0x00031B88 File Offset: 0x0002FD88
	private void Show()
	{
		for (int i = 0; i < this.renderers.Length; i++)
		{
			Renderer renderer = this.renderers[i];
			if (renderer != null)
			{
				renderer.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06000BB4 RID: 2996 RVA: 0x00031BC4 File Offset: 0x0002FDC4
	private Transform FindFirst(Transform root, string checkName)
	{
		for (int i = 0; i < root.childCount; i++)
		{
			Transform child = root.GetChild(i);
			if (child.name == checkName)
			{
				return child;
			}
			if (child.childCount > 0)
			{
				Transform transform = this.FindFirst(child, checkName);
				if (transform != null)
				{
					return transform;
				}
			}
		}
		return null;
	}

	// Token: 0x04000A08 RID: 2568
	public OcclusionFadeTypes fadeType;

	// Token: 0x04000A09 RID: 2569
	public string topName = "Fade";

	// Token: 0x04000A0A RID: 2570
	public OcclusionFadeTrigger[] triggers;

	// Token: 0x04000A0B RID: 2571
	public Renderer[] renderers;

	// Token: 0x04000A0C RID: 2572
	public List<Material> originMaterials;

	// Token: 0x04000A0D RID: 2573
	private List<Material> tempMaterials;

	// Token: 0x04000A0E RID: 2574
	private Transform topTransform;

	// Token: 0x04000A0F RID: 2575
	private int enterCounter;

	// Token: 0x04000A10 RID: 2576
	private bool hiding;

	// Token: 0x04000A11 RID: 2577
	private bool triggerEnabled = true;
}
