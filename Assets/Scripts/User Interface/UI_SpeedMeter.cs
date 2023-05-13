using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SpeedMeter : MonoBehaviour
{
    [SerializeField] PES_Grounded _groundState;
    TextMeshProUGUI text;

    [SerializeField] Color minColor;
    [SerializeField] Color maxColor;

    float lerpSpeed;
    float maxBoostCol = SF_GameplayValues.FrictionChargeAmount(22);
    float maxBoostFontSize = SF_GameplayValues.FrictionChargeAmount(28);

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        lerpSpeed = 6f * Time.fixedDeltaTime;
        minColor = new Color(61f/255f,76/255f,180,0.75f);
        maxColor = new Color(0f/255f,1,160f/255f,0.9f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float frictionBoost = SF_GameplayValues.FrictionChargeAmount(_groundState.FlatVelocity);
        float km = (_groundState.FlatVelocity*3.6f);
        //float dotten = Mathf.Round(km*10);
        //float dec = (dotten % 10);
        //float part = Mathf.FloorToInt(dotten/10);
        //text.text = part.ToString()+"."+dec.ToString();
        text.text = Mathf.FloorToInt(km) + " km.h\n" + Mathf.FloorToInt(_groundState.GroundHeight)+"."+Mathf.FloorToInt(_groundState.GroundHeight*10)%10 + " meter";
        Color boostColor = Color.Lerp(minColor,maxColor,frictionBoost/maxBoostCol);
        text.color = boostColor;
        text.fontSize = Mathf.Lerp(text.fontSize, 15-Mathf.Min(frictionBoost/maxBoostFontSize, maxBoostFontSize)*5, lerpSpeed);
    }
}
