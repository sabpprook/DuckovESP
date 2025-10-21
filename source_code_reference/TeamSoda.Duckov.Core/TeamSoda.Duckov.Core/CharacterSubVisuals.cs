using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000060 RID: 96
public class CharacterSubVisuals : MonoBehaviour
{
	// Token: 0x0600038B RID: 907 RVA: 0x0000F8A0 File Offset: 0x0000DAA0
	private void InitLayers()
	{
		if (this.layerInited)
		{
			return;
		}
		this.layerInited = true;
		this.hiddenLayer = LayerMask.NameToLayer("SpecialCamera");
		this.showLayer = LayerMask.NameToLayer("Character");
		this.sodaLightShowLayer = LayerMask.NameToLayer("SodaLight");
	}

	// Token: 0x0600038C RID: 908 RVA: 0x0000F8F0 File Offset: 0x0000DAF0
	private void SetRenderers()
	{
		this.renderers.Clear();
		this.particles.Clear();
		this.lights.Clear();
		this.sodaPointLights.Clear();
		foreach (Renderer renderer in base.GetComponentsInChildren<Renderer>(true))
		{
			ParticleSystem component = renderer.GetComponent<ParticleSystem>();
			if (component)
			{
				this.particles.Add(component);
			}
			else
			{
				SodaPointLight component2 = renderer.GetComponent<SodaPointLight>();
				if (component2)
				{
					this.sodaPointLights.Add(component2);
				}
				else
				{
					this.renderers.Add(renderer);
				}
			}
		}
		foreach (Light light in base.GetComponentsInChildren<Light>(true))
		{
			this.lights.Add(light);
		}
	}

	// Token: 0x0600038D RID: 909 RVA: 0x0000F9B8 File Offset: 0x0000DBB8
	public void AddRenderer(Renderer renderer)
	{
		if (renderer == null || this.renderers.Contains(renderer))
		{
			return;
		}
		this.InitLayers();
		int num = (this.hidden ? this.hiddenLayer : this.showLayer);
		renderer.gameObject.layer = num;
		this.renderers.Add(renderer);
		if (this.character)
		{
			this.character.RemoveVisual(this);
			this.character.AddSubVisuals(this);
		}
	}

	// Token: 0x0600038E RID: 910 RVA: 0x0000FA38 File Offset: 0x0000DC38
	public void SetRenderersHidden(bool _hidden)
	{
		this.hidden = _hidden;
		this.InitLayers();
		int num = (_hidden ? this.hiddenLayer : this.showLayer);
		int num2 = this.renderers.Count;
		for (int i = 0; i < num2; i++)
		{
			if (this.renderers[i] == null)
			{
				this.renderers.RemoveAt(i);
				i--;
				num2--;
			}
			else
			{
				this.renderers[i].gameObject.layer = num;
			}
		}
		int num3 = this.particles.Count;
		for (int j = 0; j < num3; j++)
		{
			if (this.particles[j] == null)
			{
				this.particles.RemoveAt(j);
				j--;
				num3--;
			}
			else
			{
				this.particles[j].gameObject.layer = num;
			}
		}
		int num4 = this.lights.Count;
		for (int k = 0; k < num4; k++)
		{
			Light light = this.lights[k];
			if (light == null)
			{
				this.lights.RemoveAt(k);
				k--;
				num4--;
			}
			else
			{
				light.gameObject.layer = num;
				if (this.hidden)
				{
					light.cullingMask = 0;
				}
				else
				{
					light.cullingMask = -1;
				}
			}
		}
		int num5 = (_hidden ? this.hiddenLayer : this.sodaLightShowLayer);
		int num6 = this.sodaPointLights.Count;
		for (int l = 0; l < this.sodaPointLights.Count; l++)
		{
			if (this.sodaPointLights[l] == null)
			{
				this.sodaPointLights.RemoveAt(l);
				l--;
				num6--;
			}
			else
			{
				this.sodaPointLights[l].gameObject.layer = num5;
			}
		}
	}

	// Token: 0x0600038F RID: 911 RVA: 0x0000FC20 File Offset: 0x0000DE20
	private void OnTransformParentChanged()
	{
		CharacterMainControl componentInParent = base.GetComponentInParent<CharacterMainControl>(true);
		this.SetCharacter(componentInParent);
	}

	// Token: 0x06000390 RID: 912 RVA: 0x0000FC3C File Offset: 0x0000DE3C
	public void SetCharacter(CharacterMainControl newCharacter)
	{
		if (newCharacter != null)
		{
			newCharacter.AddSubVisuals(this);
			this.character = newCharacter;
		}
	}

	// Token: 0x06000391 RID: 913 RVA: 0x0000FC55 File Offset: 0x0000DE55
	private void OnDestroy()
	{
		if (this.character != null)
		{
			this.character.RemoveVisual(this);
		}
	}

	// Token: 0x040002AA RID: 682
	private CharacterMainControl character;

	// Token: 0x040002AB RID: 683
	public List<Renderer> renderers;

	// Token: 0x040002AC RID: 684
	public List<ParticleSystem> particles;

	// Token: 0x040002AD RID: 685
	public List<Light> lights;

	// Token: 0x040002AE RID: 686
	public List<SodaPointLight> sodaPointLights;

	// Token: 0x040002AF RID: 687
	private int hiddenLayer;

	// Token: 0x040002B0 RID: 688
	private int showLayer;

	// Token: 0x040002B1 RID: 689
	private int sodaLightShowLayer;

	// Token: 0x040002B2 RID: 690
	private bool hidden;

	// Token: 0x040002B3 RID: 691
	private bool layerInited;

	// Token: 0x040002B4 RID: 692
	public bool logWhenSetVisual;

	// Token: 0x040002B5 RID: 693
	public CharacterModel mainModel;

	// Token: 0x040002B6 RID: 694
	public bool debug;
}
