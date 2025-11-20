using UnityEngine;
using UnityEngine.UI;

public class SaveGame_CreateNewSaveButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(OpenModal);
    }

    private void OpenModal()
    {
        Messages.GetOnce<SaveGame_CreateNewSaveModalMessage>().Dispatch();
    }
}
