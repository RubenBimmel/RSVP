using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GamePhone
{
    public class DateDisplay : MonoBehaviour
    {

        private TextMeshPro text;
        private string suffix = "th";
        private string[] days = {"Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

        // Use this for initialization
        void Start()
        {
            text = transform.GetComponentInChildren<TextMeshPro>();
            if (!text)
            {
                Debug.LogWarning("Time display is missing a text object.");
            }
        }

        // Update is called once per frame
        void Update()
        {
            int day = GameManager.day;
            if (text)
            {
                switch (day) {
                    case 1: case 21: case 31:
                        suffix = "st";
                        break;
                    case 2: case 22:
                        suffix = "nd";
                        break;
                    case 3: case 23:
                        suffix = "rd";
                        break;
                    default:
                        suffix = "th";
                        break;
                }
                text.text = days[(day - 4) % 7]  + " " + GameManager.day.ToString() + suffix + " June";
            }
        }
    }
}
