using UnityEngine;
using UnityEngine.UI;
public class SaveGameDataNewSlotButton : MonoBehaviour
{
    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(Save);
    }

    private void Save()
    {
        if (SaveManager.SaveGameToNewSlot(GameData.GetGameData(), true))
        {
            Messages.GetOnce<OpenNoticeModalMessage>().Dispatch(
                $"Game Saved!",
                new ModalButtonData()
                {
                    text = "Ok",
                    pressCallback = null,
                    disableTime = 2.0f,
                }
            );
        }
        else
        {
            Messages.GetOnce<OpenNoticeModalMessage>().Dispatch(
                $"Game was unable to be saved",
                new ModalButtonData()
                {
                    text = "Ok",
                    pressCallback = null,
                    disableTime = 2.0f,
                }
            );
        }
    }
}
