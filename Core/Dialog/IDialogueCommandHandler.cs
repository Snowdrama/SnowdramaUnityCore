using System.Collections;
using System.Threading.Tasks;

namespace DialogueSystem
{
    public interface IDialogueCommandHandler
    {
        Task Handle(DialogLine instruction);
    }
}
