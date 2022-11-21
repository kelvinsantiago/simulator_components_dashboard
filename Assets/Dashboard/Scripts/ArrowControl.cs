using UnityEngine;

public class ArrowControl : MonoBehaviour {

    public UDPBaseDataListener udpSource;

    public float sensorVal;
    public bool isAbsoluteValue = true;

    public float leftRotation = -120;
    public float rightRotation = 120;
    public float conversionFactor = 2.23693629f;
    public float valByDegree = 2;
    public int dataPointIndex = 0;

    private Rigidbody arrowObj;

    // Start is called before the first frame update
    void Start() {

        arrowObj = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update() {

        if (isAbsoluteValue) {

            sensorVal = Mathf.Abs(udpSource.valArray[dataPointIndex] * conversionFactor);

        } else {

            sensorVal = udpSource.valArray[dataPointIndex] * conversionFactor;

        }

        arrowObj.transform.eulerAngles = new Vector3(0, leftRotation + sensorVal * valByDegree, 0);

    }

}
