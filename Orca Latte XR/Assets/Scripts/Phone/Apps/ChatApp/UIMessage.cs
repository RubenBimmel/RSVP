using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
    public class UIMessage : Division {

        public TMPro.TextMeshPro textMesh;
        public SpriteRenderer spriteRenderer;

        // Use this for initialisation
        public void SetText (string text) {
            textMesh.text = text;
        }

        // Used to scale the message to fit the height of the text
        public void ScaleToFitText () {
            float newHeight = Mathf.Floor(textMesh.mesh.bounds.extents.y);
            spriteRenderer.size = new Vector2(1080f, 170f + 2 * newHeight);
            Resize(1080f, 160f + 2 * newHeight);
        }

    }
}
