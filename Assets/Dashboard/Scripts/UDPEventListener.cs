using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UDPEventListener : MonoBehaviour {

    public int listenAtPort = 7005;
    public bool listenForData = true;

    Thread receiveThread;
    UdpClient udpReceiverClient;
    public float[] valArray;

    public WarningPanel alarmPanel;
    public GearState gearPanel;

    public bool triggerTextureChange = false;
    public int targetTexture = 0;

    void Start() {
        initializeSettings();
    }

    void Update() {
        
        if (triggerTextureChange) {

            alarmPanel.setAlarmTexture(targetTexture);
            triggerTextureChange = false;

        }

    }

    public void initializeSettings() {

        valArray = new float[6];
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

        Debug.Log("UDP Receiver for Alarm Initialized!");

        while (listenForData) {

            try {

                byte[] bData = udpReceiverClient.Receive(ref controlPoint);

                valArray[0] = BitConverter.ToSingle(bData, 0);   // audio
                valArray[1] = BitConverter.ToSingle(bData, 4);   // visual
                valArray[2] = BitConverter.ToSingle(bData, 8);   // tactile
                valArray[3] = BitConverter.ToSingle(bData, 12);  // warningCode
                valArray[4] = BitConverter.ToSingle(bData, 16);  // trigger
                valArray[5] = BitConverter.ToSingle(bData, 20);  // gear

                paramToAction();

            } catch (Exception err) {

                if (!listenForData) {

                    Debug.Log("Not listening for dashboard data anymore.");

                } else {

                    Debug.LogError(err.ToString());

                }

            }

        }

    }

    public void paramToAction() {

        alarmPanel.playAudio = (valArray[0] == 1)?true:false;
        alarmPanel.showVisual = (valArray[1] == 1) ? true : false;
        alarmPanel.playTactile = (valArray[2] == 1) ? true : false;
        targetTexture = (int)valArray[3];
        triggerTextureChange = true;
        alarmPanel.activeWarning = (valArray[4] == 1) ? true : false;
    
    }

}