using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePhone {
    public class MultitaskingScreen : Screen {

        public Phone phone;
        public Sprite homePagePreview;

        private Panel division;
        private List<Button> buttons;

		// Called on initialisation
        void Awake () {
            division = transform.GetComponentInChildren<Panel>();
            buttons = new List<Button>();
        }

		// Called on activation
        void OnEnable () {
            Reset();
        }

		// Regenerates all of the content for this screen
        public void Reset () {
            if (phone) {
                ClearScreen();

				// Get all active apps
                App[] apps = phone.GetActiveApps();

				// Set the new width of the multitasking screen
                float screenWidth = UnityEngine.Screen.width;
				Vector3 appScale = Vector3.one * screenWidth / 1080;
                Vector3 position = new Vector3(-apps.Length * screenWidth * .3f, 0, -1);

				division.bounds.size = new Vector3(Mathf.FloorToInt(screenWidth + screenWidth * .6f * apps.Length), 1920, 0);
                Transform solid = division.transform.Find("Solid");
                solid.localScale = division.bounds.size;

				// Add all new thumbnails
				AddHomepageThumbnail (position, appScale);

                for (int i = 0; i < apps.Length; i++) {
                    position.x += screenWidth * .6f;

					Transform thumbnail = AddThumbnail (position, appScale, apps [i]);
					AddCloseButton (thumbnail, appScale, i);
                }
            }
        }

		// Generates the homepage thumbnial
		private void AddHomepageThumbnail (Vector3 position, Vector3 appScale) {
			Button button = new GameObject().AddComponent<Button>();
			button.transform.parent = division.transform;
			button.transform.localPosition = position;
			button.transform.localScale = appScale;
			button.name = "HomeScreen";

			button.normalSprite = homePagePreview;
			button.Reset();
			button.OnClick.AddListener(phone.OpenHomeScreen);

			buttons.Add(button);
		}

		// Generates an app thumbnail
		private Transform AddThumbnail (Vector3 position, Vector3 appScale, App app) {
			AppButton appButton = new GameObject().AddComponent<AppButton>();
			appButton.transform.parent = division.transform;
			appButton.transform.localPosition = position;
			appButton.transform.localScale = appScale;
			appButton.name = app.name;

			appButton.normalSprite = app.preview;
			appButton.phone = phone;
			appButton.instance = app;
			appButton.Reset();

			buttons.Add(appButton);
			return appButton.transform;
		}

		// Adds a close button to the app thumbnail
		private void AddCloseButton (Transform appButton, Vector3 appScale, int index) {
			Button closeButton = Instantiate(Resources.Load<Button>("Buttons/CloseButton"), appButton);
			Vector3 closeButtonPosition = new Vector3(270, 480);
			closeButtonPosition.Scale(appScale);
			closeButtonPosition -= new Vector3(25, 25, 1);
			closeButton.transform.localPosition = closeButtonPosition;
			closeButton.OnClick.AddListener(delegate{phone.CloseApp(index);});
		}

		// Removes all thumbnails from the screen
        private void ClearScreen() {
            foreach (Button button in buttons) {
                Destroy(button.gameObject);
            }
            buttons = new List<Button>();
        }
    }
}
