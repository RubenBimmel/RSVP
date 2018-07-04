using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
    public class Phone : MonoBehaviour {

		public bool alwaysVisible;
		public GameObject render;

        public AudioClip[] clips;

		[HideInInspector]
		public Transform appParent;
		private GameObject background;
		private GameObject lockScreen;
		private bool locked = true;

        private HomeScreen homeScreen;
        private MultitaskingScreen multiTaskingScreen;

        private List<App> activeApps;
        private Screen activeScreen;
		private Screen previousScreen;

		// Called on initialisation
        void Awake() {
			background = transform.Find ("SolidBlack").gameObject;
			appParent = transform.Find ("Apps");

			lockScreen = transform.Find ("LockScreen").gameObject;
			homeScreen = transform.Find ("HomeScreen").GetComponent<HomeScreen> ();
			multiTaskingScreen = transform.Find ("MultitaskingScreen").GetComponent<MultitaskingScreen> ();

			activeScreen = homeScreen;

			// Load all apps and add them to the home screen
			activeApps = new List<App>();
			App[] apps = Resources.LoadAll<App>("Apps");
			homeScreen.Instantiate(apps, this);
        }
    
        // Use this for initialization
        void Start() {
            Camera.main.orthographicSize = UnityEngine.Screen.height * .5f;
        }

        // Update is called once per frame
        void Update() {
			if (render) {
				if (GyroControl.eyeView == false || alwaysVisible) {
					render.SetActive (true);
					Input.isEnabled = true;
				} else {
					render.SetActive (false);
					Input.isEnabled = false;
					Lock ();
				}
			}
        }

		// Called when pressing the home button
		public void OpenHomeScreen() {
			ActivateScreen(homeScreen);
		}

		// Called when pressing the multitasking button
		public void OpenMultitaskingScreen() {
            ActivateScreen(multiTaskingScreen);
        }

		// Called when the back button is pressed
		public void GoBack () {
			if (!activeScreen.GoBack ()) {
                if (previousScreen && previousScreen != multiTaskingScreen) {
                    ActivateScreen(previousScreen);
                    previousScreen = homeScreen;
                    return;
                }
                ActivateScreen(homeScreen);
			}
		}

		// Gets calle when a new screen is opened
        public void ActivateScreen (Screen newScreen) {
            previousScreen = activeScreen;
            if (activeScreen != newScreen) {
				activeScreen.Pause();
                activeScreen = newScreen;
				newScreen.Open();
            }
        }

		// Gets called when an app is opened
        public void OpenApp (App app) {
			activeScreen.Pause();
            if (!activeApps.Contains(app)) {
                activeApps.Add(app);
                activeScreen = app;
                StartCoroutine("LoadApp");
            }
            else {
                ActivateScreen(app);
            }
        }

		// Gets called wheb an app is closed
        public void CloseApp (int x) {
            if (x < activeApps.Count) {
				activeApps [x].Close ();
                activeApps.Remove(activeApps[x]);
                multiTaskingScreen.Reset();
            }
        }

		// Load time for opening an app
        public IEnumerator LoadApp () {
            App app = (App)activeScreen;
            if (app) {
                yield return new WaitForSeconds(app.loadTime);
                app.Open();
            }
        }

		// Returns all active apps
        public App[] GetActiveApps () {
            return activeApps.ToArray();
        }

		// Used to make the phone transparent (for camera apps)
		public void SetDefaultBackground (bool enabled) {
			background.SetActive (enabled);
		}

		// Gets called when the lockscreen is swiped up
		public void Unlock () {
			if (locked) {
				locked = false;
			}
		}

		// Gets called when the player leaves the phone view
		public void Lock () {
			if (!locked) {
				lockScreen.SetActive(true);
				locked = true;
			}
		}

		// Default nodification
		public void GetNotified (int audioClip) {
            #if UNITY_ANDROID
			if (audioClip == 0) Handheld.Vibrate ();
            #endif
            AudioSource audio = GetComponent<AudioSource>();
            audio.clip = clips[audioClip];
            audio.Play();
		}
    }
}
