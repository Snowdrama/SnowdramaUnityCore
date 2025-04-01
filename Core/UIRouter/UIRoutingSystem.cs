using UnityEngine;


namespace Snowdrama.UI
{
    public partial class UIRoutingSystem : MonoBehaviour
    {
        [SerializeField] private UIRouter _uiRouter;
        public UIRouter GetRouter()
        {
            return _uiRouter;
        }
    }

}