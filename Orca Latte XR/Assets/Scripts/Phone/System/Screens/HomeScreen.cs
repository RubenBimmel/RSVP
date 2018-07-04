using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
    public class HomeScreen : Screen {

        private Grid grid;

		// Called on initialisation
        /*void Awake () {
            grid = transform.GetComponentInChildren<Grid>();
        }*/

		// Add apps to this phone
        public void Instantiate (App[] apps, Phone phone) {
            grid = transform.GetComponentInChildren<Grid>();

            foreach (App app in apps) {
                AppButton button = new GameObject().AddComponent<AppButton>();
                button.transform.parent = transform;
				button.transform.localScale = Vector3.one;
                button.normalSprite = app.icon;
                button.app = app;
                button.phone = phone;
                button.Reset();

                grid.cells.Add(button);
            }
            grid.ResetGrid();
        }
    }
}
