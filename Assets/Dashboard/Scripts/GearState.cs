using System;
using UnityEngine;

public class GearState : MonoBehaviour {

    Renderer textureRenderer;
    public UDPBaseDataListener udpSource;

    public int gearStateIndx = 2;
    public float gearState = 0;
    public int gearMapping = 0;

    public Material[] stateTextures = new Material[5];

    // Start is called before the first frame update
    void Start() {

        textureRenderer = GetComponent<Renderer>();
        textureRenderer.enabled = true;

    }

    // Update is called once per frame
    void Update() {

        setState();

    }

    public void setState() {

        gearState = udpSource.valArray[gearStateIndx];

        if (gearMapping == 0) {

            textureRenderer.sharedMaterial = stateTextures[(int)gearState];

        }

    }

}
