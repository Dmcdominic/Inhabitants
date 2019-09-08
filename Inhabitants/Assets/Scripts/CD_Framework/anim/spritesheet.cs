using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;

[CreateAssetMenu(menuName = "anim/spritesheet")]
public class spritesheet : ScriptableObject {
	public List<Sprite> sprites;

#if UNITY_EDITOR
	// Generate a spritesheet for every texture that is selected.
	[MenuItem("Custom/Generate Spritesheet Objects %g")]
	public static void generate_spritesheets() {
		Object[] selected_objs = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
		if (selected_objs.Length <= 0) {
			Debug.LogWarning("To generate spritesheet objects, you must first select at least one multi-sprite texture in the Assets Folder");
			return;
		}
		Debug.Log("Generating spritesheet objects...");

		foreach (Object obj in selected_objs) {
			if (!try_generate_spritesheet(obj)) {
				Debug.LogWarning("Generation failed for object: " + obj + ". Each selected object must be a multi-sprite texture in the Assets Folder.");
			}
		}
	}

	// Try to generate a spritesheet object from the given object, which is expected to be a multi-sprite texture.
	// Return true iff generation is successful.
	protected static bool try_generate_spritesheet(Object obj) {
		Debug.Log("Generating a spritesheet for: " + obj);
		string path = AssetDatabase.GetAssetPath(obj);
		if (path.Length <= 0 || !File.Exists(path)) {
			Debug.LogWarning("Invalid path for asset");
			return false;
		}
		Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);

		// Alphanumeric sorting fix!
		//IOrderedEnumerable<Object> ordered_sprites = sprites.OrderBy(sprite => sprite.name);
		IOrderedEnumerable<Object> ordered_sprites = sprites.OrderBy(sprite => string_util.PadNumbers(sprite.name));

		// Try to find an existing asset for this spritesheet, and repopulate it.
		// Otherwise, generate a new spritesheet asset.
		string new_path = save_util.remove_extension(path) + ".asset";
		spritesheet spritesheet_found = AssetDatabase.LoadAssetAtPath<spritesheet>(new_path);
		if (spritesheet_found != null) {
			spritesheet_found.sprites.Clear();

			foreach (Object sprite in ordered_sprites) {
				if (sprite is Sprite) {
					spritesheet_found.sprites.Add((Sprite)sprite);
				}
			}

			AssetDatabase.SaveAssets();
			Debug.Log("Spritesheet for: " + obj + " successfully saved to existing asset at: " + new_path);
		} else {
			spritesheet new_spritesheet = ScriptableObject.CreateInstance<spritesheet>();
			new_spritesheet.sprites = new List<Sprite>();

			foreach (Object sprite in ordered_sprites) {
				if (sprite is Sprite) {
					new_spritesheet.sprites.Add((Sprite)sprite);
				}
			}

			AssetDatabase.CreateAsset(new_spritesheet, new_path);
			Debug.Log("Spritesheet for: " + obj + " successfully saved to new asset at: " + new_path);
		}
		return true;
	}
#endif
}
