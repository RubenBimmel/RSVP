using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
	// QQQ WORK IN PROGRESS
	public class GalleryApp : App {

		public static string[] pictures { get { return System.IO.Directory.GetFiles(Application.persistentDataPath + "/Resources/Pictures");} }
		private Grid grid;
		private Transform openPicture;

		void Awake () {
			System.IO.Directory.CreateDirectory (Application.persistentDataPath + "/Resources/Pictures");

			grid = transform.GetComponentInChildren<Grid> ();
			grid.cells = new List<Division> ();
			openPicture = transform.Find ("OpenPicture");
		}

		public override void Open () {
			foreach (Transform child in grid.transform) {
				if (child != grid.transform) {
					Destroy (child.gameObject);
				}
			}

			grid.ClearGrid ();

			for (int i = grid.cells.Count; i < pictures.Length; i++) {
				Button newPicture = Instantiate(Resources.Load<Button>("Apps/GalleryApp/Prefabs/Picture"), grid.transform);
				Sprite picture = LoadPicture(pictures [i]);
				newPicture.transform.Find("Sprite").GetComponent<SpriteRenderer> ().sprite = picture;
				newPicture.OnClick.AddListener (delegate {
					OpenPicture (picture);
				});
				grid.cells.Add (newPicture);
			}

			grid.ResetGrid ();

			base.Open ();
		}

		public override bool GoBack ()
		{
			if (openPicture.gameObject.activeSelf == true) {
				openPicture.gameObject.SetActive (false);
				return true;
			}

			return base.GoBack ();
		}

		public void OpenPicture (Sprite picture) {
			openPicture.gameObject.SetActive (true);
			openPicture.Find ("Sprite").GetComponent<SpriteRenderer> ().sprite = picture;
		}

		public static Sprite LoadPicture (string path) {
			if( string.IsNullOrEmpty(path) == true) { 
				return null;
			}
			if(System.IO.File.Exists(path) == true)
			{
				byte[] bytes = System.IO.File.ReadAllBytes (path);
				Texture2D texture = new Texture2D (4, 4, TextureFormat.RGBA32, false);
				texture.LoadImage(bytes);
				Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2 (0.5f,0.5f), 1f);
				return sp;
			}
			return null;
		}
	}
}
