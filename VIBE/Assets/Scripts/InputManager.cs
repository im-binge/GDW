using System.Windows.Input;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private ICommand hitCommand;

    private void Start()
    {
        MusicManager musicManager = MusicManager.Instance;
        hitCommand = new HitCommand(musicManager);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hitCommand.Execute();
        }
    }
}
