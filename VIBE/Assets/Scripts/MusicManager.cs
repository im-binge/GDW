using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;
    public static MusicManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MusicManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("MusicManager");
                    _instance = obj.AddComponent<MusicManager>();
                }
            }
            return _instance;
        }
    }

    public event Action OnHit;
    public event Action OnMiss;

    public AudioEvent mistimedPressEvent;

    public List<AudioEvent> audioEvents;
    private EventInstance musicEvent;
    private AudioFactory audioFactory;

    private int currentEventIndex;

    private const float TimingWindow = 0.3f;
    private float timer = 0f;
    private bool isTiming = false;
    private float markerTime;

    [SerializeField] private Button startButton;

    [StructLayout(LayoutKind.Sequential)]
    private struct TimelineMarker
    {
        public IntPtr name;
        public int position;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        audioFactory = gameObject.AddComponent<AudioFactory>();
    }

    private void Start()
    {
        
    }

    public void StartMusic()
    {
        Debug.Log($"Current Event Index: {currentEventIndex}, Total Events: {audioEvents.Count}");

        if (currentEventIndex < audioEvents.Count)
        {
            musicEvent = audioFactory.CreateEvent(audioEvents[currentEventIndex]);
            musicEvent.setCallback(OnMarkerReached, EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

            musicEvent.start();

            startButton.interactable = false;
            startButton.gameObject.SetActive(false);
            StartCoroutine(MonitorPlaybackState());
        }
        else
        {
            Debug.LogWarning("No audio events available to play.");
        }
    }

    private void LoadNextEvent()
    {
        if (currentEventIndex < audioEvents.Count)
        {
            musicEvent = audioFactory.CreateEvent(audioEvents[currentEventIndex]);
            musicEvent.setCallback(OnMarkerReached, EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

            musicEvent.start();
        }
        else
        {
            Debug.Log("No more events to play.");

            startButton.interactable = true;
            startButton.gameObject.SetActive(true);
        }
    }

    public void PlayNextEvent()
    {
        if (musicEvent.isValid())
        {
            audioFactory.ReleaseEvent(musicEvent);
        }

        currentEventIndex++;
        LoadNextEvent();
    }

    private static FMOD.RESULT OnMarkerReached(EVENT_CALLBACK_TYPE type, IntPtr eventInstance, IntPtr parameter)
    {
        if (type == EVENT_CALLBACK_TYPE.TIMELINE_MARKER && Instance != null)
        {
            Instance.ProcessMarker(parameter);
        }
        return FMOD.RESULT.OK;
    }

    private void ProcessMarker(IntPtr parameter)
    {
        TimelineMarker marker = Marshal.PtrToStructure<TimelineMarker>(parameter);
        markerTime = marker.position / 1000f;
        timer = TimingWindow;
        isTiming = true;
    }

    private void Update()
    {
        if (!isTiming)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Debug.Log("Missed the beat!");
            OnMiss?.Invoke();
            isTiming = false;
        }
    }

    public void CheckHit()
    {
        float timeDifference = Mathf.Abs(markerTime - GetCurrentPlaybackTime());

        if (timeDifference <= TimingWindow)
        {
            Debug.Log("Hit at the right time!");
            OnHit?.Invoke();
        }
        else
        {
            Debug.Log("Mistimed hit!");
            OnMiss?.Invoke();

            // Play the mistimed sound effect
            EventInstance mistimedInstance = audioFactory.CreateEvent(mistimedPressEvent);
            mistimedInstance.start();
            mistimedInstance.release();
        }

        isTiming = false;
    }



    private float GetCurrentPlaybackTime()
    {
        musicEvent.getTimelinePosition(out int position);
        return position / 1000f;
    }

    private IEnumerator MonitorPlaybackState()
    {
        while (true)
        {
            musicEvent.getPlaybackState(out PLAYBACK_STATE state);

            if (state == PLAYBACK_STATE.STOPPED)
            {
                if (currentEventIndex + 1 >= audioEvents.Count)
                {
                    startButton.interactable = true;
                    startButton.gameObject.SetActive(true);
                }
                else
                {
                    PlayNextEvent();
                }
                break;
            }

            yield return null;
        }
    }


    private void OnDestroy()
    {
        if (musicEvent.isValid())
        {
            musicEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicEvent.release();
        }

        if (_instance == this)
        {
            _instance = null;
        }
    }
}
