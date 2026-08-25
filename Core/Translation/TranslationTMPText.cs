using TMPro;
using UnityEngine;

namespace Snowdrama
{
    /// <summary>
    /// A script that lives on a static TMP_Text and translates
    /// some key to the tranlsation for that key using messages
    /// to update the text when changed
    /// 
    /// 
    /// 
    /// NOT FINISHED!
    /// NOT FINISHED!
    /// NOT FINISHED!
    /// NOT FINISHED!
    /// NOT FINISHED!
    /// THIS IS UNFINISHED BUT PUSHED DUE TO NEEDING TO SHIP A BUGFIX
    /// NOT FINISHED!
    /// NOT FINISHED! 
    /// NOT FINISHED!
    /// NOT FINISHED!
    /// NOT FINISHED!
    /// 
    /// 
    /// 
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class TranslationTMPText : MonoBehaviour
    {
        [SerializeField] private string key = "[DEFAULT_KEY]";
        [SerializeField] private TMP_Text text;

        private TranslationChangedMesage TranslationChangedMesage;
        private void OnEnable()
        {
            this.UpdateTransltation();
            TranslationChangedMesage = Messages.Get<TranslationChangedMesage>();
            TranslationChangedMesage.AddListener(this.UpdateTransltation);
        }

        private void OnDisable()
        {
            TranslationChangedMesage.RemoveListener(this.UpdateTransltation);
            Messages.Return<TranslationChangedMesage>();
        }

        private void UpdateTransltation()
        {
            text = this.GetComponent<TMP_Text>();
            text.text = TranslationSystem.TR(key);
        }
    }
}
