using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboTextControl : MonoBehaviour {
    private Text comboText;

    private void Awake() {
        comboText = gameObject.GetComponent<Text>();
    }

    private void Update() {
        comboText.text = GetScoreText();
    }

    private string GetScoreText() {
        int combo = GamePlayManager.Instance.currentCombo;
        if (combo >= 100) return combo.ToString();
        else if (combo >= 10) return "0" + combo;
        else if (combo >= 1) return "00" + combo;
        else return "000";
    }
}
