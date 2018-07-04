using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
    public class UIMessageOption : Division {

        public TMPro.TextMeshPro textMesh;
        private int id;
        private Chat chat;

        public override void OnMouseClick() {
            NarrativeHandler.instance.MakeDecision(id, chat);
            base.OnMouseClick();
        }

        public void Initialise(MessageOption option) {
            id = option.id;
            textMesh.text = option.message;
            chat = option.chat;
        }
    }
}
