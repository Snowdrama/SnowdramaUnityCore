using UnityEngine;
using UnityEngine.UI;

public class SaveGame_CreateNewSaveButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(this.OpenModal);
    }

    private void OpenModal()
    {
        Messages.GetOnce<Modal_SaveGameMessage>().Dispatch(-1, "");
    }
}
