using UnityEngine;

using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;

public class UDPBaseDataListener : MonoBehaviour {

    public int listenAtPort = 6000;

    public bool listenForData = true;
    public bool listeningForChrono = false;

    Thread receiveThread;
    UdpClient udpReceiverClient;
    
    // Data from Remote Simulator
    private SimulatorMessage currMsgObj = null;
    private bool receivingData = false;

    // For Dashboard
    public float[] valArray;


    // Start is called before the first frame update
    void Start() {

        initializeSettings();

    }

    public void initializeSettings() {

        valArray = new float[11];

        startUdpThread();

    }

    public void startUdpThread() {

        receiveThread = new Thread(new ThreadStart(receiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

    }

    private void receiveData() {

        IPEndPoint controlPoint = new IPEndPoint(IPAddress.Any, listenAtPort);
        udpReceiverClient = new UdpClient(controlPoint);

        Debug.Log("UDP Receiver for Dashboard Initialized!");

        while (listenForData) {

            try {

                byte[] bData = udpReceiverClient.Receive(ref controlPoint);
                currMsgObj = processRawByteData(bData);
                receivingData = true;

            } catch (Exception err) {

                if (!listenForData) {

                    Debug.Log("Not listening for dashboard data anymore.");

                } else {

                    Debug.LogError(err.ToString());

                }

            }

        }

    }

    public SimulatorMessage processRawByteData(byte[] bData) {

        SimulatorMessage toReturn = new SimulatorMessage(bData, listeningForChrono);

        valArray[0] = toReturn.speedVal;
        valArray[1] = toReturn.rpm;
        valArray[2] = toReturn.gear;

        valArray[3] = toReturn.state1;
        valArray[4] = toReturn.state2;
        valArray[5] = toReturn.state3;
        valArray[6] = toReturn.state4;
        valArray[7] = toReturn.state5;
        valArray[8] = toReturn.state6;

        valArray[9] = toReturn.indicator1;
        valArray[10] = toReturn.indicator2;

        return toReturn;

    }

    void OnApplicationQuit() {

        try {

            listenForData = false;
            udpReceiverClient.Close();

        } catch (Exception err) {

            Debug.Log("Dashboard is closing.");
            Debug.LogError(err.ToString());

        }

    }

}

