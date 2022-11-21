using System.Collections;
using System.IO;
using UnityEngine;

public class WarningPanel : MonoBehaviour {

    public float onTime = 0.50f;
    public float offTime = 0.50f;
    public int cycles = 1;
    private float timeAnchor = 0;

    public bool activeWarning = true;
    public bool playAudio = false;
    public bool showVisual = false;
    public bool playTactile = false;

    public Material alarmTexture;
    private Texture2D[] fileTextures;  
    private Renderer textureRenderer;

    private AudioSource alarm_sound_only;
    private AudioSource alarm_sound_tactile;
    private AudioSource alarm_tactile;

    private float cTime = 0;
    private float elapsed = 0;

    void Start() {

        textureRenderer = gameObject.GetComponent<Renderer>();
        textureRenderer.enabled = false;

        // TODO: Make modifications to support reading audio files from
        // disk thus making the system user-configurable without the need
        // to modify the code.
        // https://docs.unity3d.com/ScriptReference/AudioClip.Create.html
        // https://forum.unity.com/threads/load-audio-files-from-file.439229/

        alarm_sound_only = gameObject.GetComponents<AudioSource>()[0];
        alarm_sound_tactile = gameObject.GetComponents<AudioSource>()[1];
        alarm_tactile = gameObject.GetComponents<AudioSource>()[2];

        textureRenderer.sharedMaterial = alarmTexture;

        populateTextures();
        setAlarmTexture(0);

    }

    void Update() {

        if (activeWarning) {

            cTime = Time.time;

            // A time anchor value equal to zero indicates that whenever the
            // next line of code is executed, a new time anchor needs to be
            // define. This forces the execution of this line only once within
            // a cycle thus allowing calculating the position within the cycle
            // further down. The current anchor position is used to determine
            // the time elapsed within an active alarm (elapsed) which then
            // further down is used to calculate the position within a cycle.

            if (timeAnchor == 0) timeAnchor = cTime;
            elapsed = cTime - timeAnchor;

            // Based on the elapsed time since the start of the active alarm, a
            // calculation is made to determine how far into the current cycle
            // the process is in by caculating a cycle anchor value and then
            // calculating a cycle position value. The cycle position value is
            // used further down to determine if the alarm icon should be on or
            // off based on the position within the cycle. The structure used
            // assumes that the cycle starts by displaying the icon.

            float cyclesElapsed = Mathf.Floor(elapsed / (onTime + offTime));
            float cycleAnchor = cyclesElapsed * (onTime + offTime);
            float cyclePos = elapsed - cycleAnchor;

            // Alarm Trigger: Visual Warning
            // The visual state is determined by the position within the cycle
            // and the duration of the on state within the cycle.

            if (showVisual) {

                textureRenderer.enabled = cyclePos <= onTime;

            }

            // Alarm Trigger: Only Audio
            // The audio file is played once and this is achieved by resetting
            // the audio and tactile variables via resetAudioAlarms. If playing
            // more than once is required, the audio file can be made longer.

            if (playAudio && !playTactile) {

                resetAudioAlarms();
                alarm_sound_only.Play();

            }

            // Alarm Trigger: Audio and Tactile
            // The file that contains the audio alarm on one channel and the
            // tactitle audio on a second channel is played. These are also
            // played only once.

            if (playAudio && playTactile) {

                resetAudioAlarms();
                alarm_sound_tactile.Play();

            }

            // Alarm Trigger: Only Tactice
            // Similar to the "Only Audio" block of code but plays the audio
            // file containing the sound signal to trigger a tactile alarm.

            if (!playAudio && playTactile) {

                resetAudioAlarms();
                alarm_tactile.Play();

            }

            // The decision to end an active alarm is made by taking into
            // consideration if the amount of time elapsed in active alarm mode
            // is greater than or equal to a maximum duration calculated by
            // considering the number of cycles to repeat specified by the user
            // as well as the duration of the on/off intervals specified. These
            // two intervals are only relevant for visual alarms.

            if (elapsed >= (cycles * (onTime + offTime))) {

                showVisual = false;
                resetStates();

            }

        }

    }

    public void populateTextures() {

        // Having issues with Directory.GetFiles pattern search so going back
        // to the basics in the code ahead.

        string imgsPath = Application.dataPath + @"\StreamingAssets\images\";
        var imagesToLoad = Directory.GetFiles(imgsPath);
        int propId = Shader.PropertyToID("_MainTex");

        int imgCount = 0;

        fileTextures = new Texture2D[imagesToLoad.Length];

        for (int i = 0; i < imagesToLoad.Length; i++) {

            string fName = imagesToLoad[i];

            if (fName.Substring(fName.Length - 3).Equals("png")) {

                imgCount++;

            }

        }

        for (int i = 0; i < imgCount; i++) {

            string imgName = "alarm" + i.ToString("00") + ".png";
            string imgPath = imgsPath + imgName;

            Texture2D cTexture = new Texture2D(1024, 1024);
            cTexture.LoadImage(File.ReadAllBytes(imgPath));

            fileTextures[i] = cTexture;

        }

    }

    public void setAlarmTexture(int indx) {

        alarmTexture.SetTexture(Shader.PropertyToID("_MainTex"), fileTextures[indx]);

    }

    void resetAudioAlarms() {

        playAudio = false;
        playTactile = false;

    }

    void resetStates() {

        timeAnchor = 0;
        activeWarning = false;
        textureRenderer.enabled = false;

    }

}