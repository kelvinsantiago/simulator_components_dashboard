using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnIndicator : MonoBehaviour {

    public Material offState;
    public Material onState;

    public UDPBaseDataListener udpSource;
    public int indxIndicator00 = 0;
    public int indxIndicator01 = 0;
    public int indxIndicator02 = 0;

    Renderer indicatorRenderer;
    public float flashingRate = 0.25f;

    private float cycleAnchor = 0;
    private float cyclePos = 0;

    void Start() {

        indicatorRenderer = GetComponent<Renderer>();
        indicatorRenderer.sharedMaterial = offState;

    }

    void Update() {
        
        if (flashing()) {

            if (cycleAnchor == 0) cycleAnchor = Time.time;

            cyclePos = (Time.time - cycleAnchor) / (2 * flashingRate);
            cyclePos = cyclePos - Mathf.Floor(cyclePos);

            indicatorRenderer.sharedMaterial = (cyclePos <= 0.5) ? onState : offState;

        } else {
            
            indicatorRenderer.sharedMaterial = offState;
            cyclePos = 0;
            cycleAnchor = 0;

        }

    }

    public bool flashing() {

        bool state01 = udpSource.valArray[indxIndicator01] > 0;
        bool state02 = udpSource.valArray[indxIndicator02] > 0;

        if (state01 && state02) return false;

        return (indxIndicator00 > 0) ? state01 : state02;

    }

}
