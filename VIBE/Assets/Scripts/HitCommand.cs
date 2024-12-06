using UnityEngine;

public class HitCommand : ICommand
{
    private MusicManager musicManager;

    public HitCommand(MusicManager musicManager)
    {
        this.musicManager = musicManager;
    }

    public void Execute()
    {
        musicManager.CheckHit();
    }
}
