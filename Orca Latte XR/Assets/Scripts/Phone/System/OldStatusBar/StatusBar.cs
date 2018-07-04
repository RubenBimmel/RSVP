using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
    public class StatusBar : MonoBehaviour {

        /*public int size = 50;

        [System.Flags]
        public enum Symbols {
            Notification = (1 << 0),
            Alarm = (1 << 1),
            Call = (1 << 2),
            Sound = (1 << 3),
            Vibration = (1 << 4),
        }

        public Symbols activeSymbols;

        private Transform bar;
        private Indicator connectionIndicator;
        private Indicator wiFiIndicator;
        private Indicator dataIndicator;
        private Indicator batteryIndicator;
        private Transform timeDisplay;
        private Symbol[] symbols;

        void Awake() {
            bar = Instantiate(Resources.Load<Transform>("Solid"), transform);
            bar.GetComponent<SpriteRenderer>().color = Color.black;

            connectionIndicator = Instantiate(Resources.Load<Indicator>("StatusBar/Symbols/ConnectionIndicator"), transform);
            wiFiIndicator = Instantiate(Resources.Load<Indicator>("StatusBar/Symbols/WiFiIndicator"), transform);
            dataIndicator = Instantiate(Resources.Load<Indicator>("StatusBar/Symbols/DataIndicator"), transform);
            batteryIndicator = Instantiate(Resources.Load<Indicator>("StatusBar/Symbols/BatteryIndicator"), transform);

            timeDisplay = Instantiate(Resources.Load<Transform>("StatusBar/TimeDisplay"), transform);

            symbols = new Symbol[5];
            symbols[0] = Instantiate(Resources.Load<Symbol>("StatusBar/Symbols/Notification"), transform);
            symbols[1] = Instantiate(Resources.Load<Symbol>("StatusBar/Symbols/Alarm"), transform);
            symbols[2] = Instantiate(Resources.Load<Symbol>("StatusBar/Symbols/Call"), transform);
            symbols[3] = Instantiate(Resources.Load<Symbol>("StatusBar/Symbols/Sound"), transform);
            symbols[4] = Instantiate(Resources.Load<Symbol>("StatusBar/Symbols/Vibration"), transform);

            activeSymbols = 0;
        }

        // Use this for initialization
        void Start() {
            ResetPosition();
        }

        public void UpdateIndicators(Phone phone) {
            float position = 50f;
            connectionIndicator.SetValue(phone.signalStrength);
            connectionIndicator.transform.localPosition = Vector3.left * (UnityEngine.Screen.width - position) * .5f + Vector3.back;
            position += 90f;

            if (phone.wifiConnection > 0) {
                wiFiIndicator.gameObject.SetActive(true);
                wiFiIndicator.SetValue(phone.wifiConnection);
				wiFiIndicator.transform.localPosition = Vector3.left * (UnityEngine.Screen.width - position) * .5f + Vector3.back;
                dataIndicator.gameObject.SetActive(false);
            }
            else {
                dataIndicator.gameObject.SetActive(true);
                dataIndicator.SetValue(phone.dataConnection);
				dataIndicator.transform.localPosition = Vector3.left * (UnityEngine.Screen.width - position) * .5f + Vector3.back;
                wiFiIndicator.gameObject.SetActive(false);
            }
            position += 90f;

            // QQQ Add symbols
            foreach (Symbol s in symbols) {
                s.gameObject.SetActive(false);
            }

            batteryIndicator.SetValue(Mathf.FloorToInt(phone.batteryLife / 20f));
        }

        public void AddSymbol(Symbols s) {
            symbols[(int)s].gameObject.SetActive(false);
        }

        public void RemoveSymbol(Symbols s) {
            symbols[(int)s].gameObject.SetActive(false);
        }

        // Reset position on screen
        public void ResetPosition() {
            if (bar) {
                transform.localPosition = Vector3.up * (UnityEngine.Screen.height - size) * .5f;
                bar.localScale = new Vector3(UnityEngine.Screen.width, size, 1f);
				timeDisplay.localPosition = Vector3.right * (UnityEngine.Screen.width - 120f) * .5f + Vector3.back;
				batteryIndicator.transform.localPosition = Vector3.right * (UnityEngine.Screen.width - 260f) * .5f + Vector3.back;
            }
        }*/
    }
}
