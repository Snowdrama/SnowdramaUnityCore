using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace DialogueSystem
{
    public class MusicHandler : MonoBehaviour, IDialogueCommandHandler
    {

        public async Task Handle(DialogLine instr)
        {
            Debug.Log($"Play music: {string.Join(' ', instr.text)}");
            await Task.Delay(50);
        }
    }
}
