using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GamePhone {
    public class AppButton : Button {

        public Phone phone;
        public App app;
        public App instance;

		// Gets called when the user clicks on this division
        public override void OnMouseClick ()
		{
            // Check if app already has an instance, else instantiate a new verison of the app
			if (instance) {
                phone.ActivateScreen(instance);
            }
            else {
                Transform parent = phone.appParent;
                instance = Instantiate(app, parent);
                instance.gameObject.SetActive(false);
                phone.OpenApp(instance);
            }
			base.OnMouseClick ();
        }
    }
}
