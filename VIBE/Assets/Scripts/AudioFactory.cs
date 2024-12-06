using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioFactory : MonoBehaviour
{
    public EventInstance CreateEvent(AudioEvent audioEvent)
    {
        return RuntimeManager.CreateInstance(audioEvent.eventPath);
    }

    public void ReleaseEvent(EventInstance eventInstance)
    {
        if (eventInstance.isValid())
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }
}
