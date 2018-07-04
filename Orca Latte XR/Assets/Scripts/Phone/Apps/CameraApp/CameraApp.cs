using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
	// QQQ WORK IN PROGRESS
	public class CameraApp : App {

		private Phone phone;
		private bool takingPicture;
		private SpriteRenderer flashRenderer;

		void Awake () {
			System.IO.Directory.CreateDirectory (Application.persistentDataPath + "/Resources/Pictures");

			phone = transform.GetComponentInParent<Phone> ();
			flashRenderer = transform.Find ("Flash").GetComponent<SpriteRenderer> ();
			flashRenderer.gameObject.SetActive (false);
		}

		void OnEnable() {
			if (phone) {
				phone.SetDefaultBackground (false);
				phone.alwaysVisible = true;
			}
		}

		void OnDisable() {
			if (phone) {
				phone.SetDefaultBackground (true);
				phone.alwaysVisible = false;
			}
		}

		public void TakePicture () {
			if (!takingPicture) {
				StartCoroutine (TakePictureRoutine ());
			}
		}

		private IEnumerator TakePictureRoutine () {
			takingPicture = true;
			Material mat = phone.render.GetComponent<Renderer> ().material;
			if (mat) {
				float alpha = 1;
				mat.SetColor ("_Color", new Color (1, 1, 1, 0));

				yield return new WaitForEndOfFrame();
				//Texture screenImage = ScreenCapture.CaptureScreenshotAsTexture();
				Texture2D screenImage = new Texture2D (UnityEngine.Screen.width, UnityEngine.Screen.height, TextureFormat.RGB24, false);
				//Get image from screen
				screenImage.ReadPixels(new Rect(0, 0, UnityEngine.Screen.width, UnityEngine.Screen.height), 0, 0, false);
				screenImage.Apply ();

				byte[] bytes = screenImage.EncodeToPNG();
				string path = Application.persistentDataPath + "/Resources/Pictures/" + GameManager.time + ".png";
				System.IO.File.WriteAllBytes(path, bytes);

				mat.SetColor ("_Color", new Color (1, 1, 1, 1));

				while (alpha > 0) {
					flashRenderer.gameObject.SetActive (true);
					alpha -= Time.deltaTime;
					flashRenderer.color = new Color (1, 1, 1, alpha);
					yield return null;
				}
				flashRenderer.gameObject.SetActive (false);
			}
			takingPicture = false;
		}

		public void StoreTexture (string path, Texture2D tex)
		{
			byte[] bytes = tex.EncodeToPNG();
			System.IO.File.WriteAllBytes(path, bytes);
		}

	}
}
