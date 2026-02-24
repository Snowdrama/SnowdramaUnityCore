using Snowdrama.Spring;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Snowdrama.Core
{
    public class SpringyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public SpringConfiguration springConfig;
        public Spring2D spring2D;
        public Vector2 hoverScale = new Vector2(1.25f, 1.25f);

        protected void Start()
        {
            spring2D = new Spring2D(springConfig, Vector2.one);
        }

        public void Update()
        {
            if (spring2D == null) { return; }
            spring2D.Update(Time.deltaTime);
            this.transform.localScale = spring2D.Value;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            spring2D.Target = hoverScale;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            spring2D.Target = Vector2.one;
        }
    }

}
