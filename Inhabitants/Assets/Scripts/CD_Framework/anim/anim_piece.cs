using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class anim_piece : MonoBehaviour {

	public anim_palettes_bundle anim_Palettes_Bundle;

	public int animation_index;
	public int palette_index;
	public int sprite_index;
	private SpriteRenderer sprite_renderer;


	protected virtual void Awake() {
		sprite_renderer = GetComponent<SpriteRenderer>();
	}

	private void Start() {
		refresh_sprite();
	}

	public virtual void refresh_sprite() {
		if (sprite_renderer == null) {
			return;
		}
		if (0 <= palette_index && palette_index < anim_Palettes_Bundle.sets.Count) {
			anim_palettes_set set = anim_Palettes_Bundle.sets[palette_index];
			if (0 <= animation_index && animation_index < set.palettes.Count) {
				spritesheet sheet = set.palettes[animation_index];
				if (0 <= sprite_index && sprite_index < sheet.sprites.Count) {
					sprite_renderer.sprite = sheet.sprites[sprite_index];
				}
			}
		}
	}

}
