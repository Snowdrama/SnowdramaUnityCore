using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.TagSystem
{
    public class Tags : MonoBehaviour
    {
        [Header("Tags"), Tooltip("Assign tags to group this object into a pool related to the tag")]
        public List<string> tags;
        private void OnEnable()
        {
            foreach (var item in tags)
            {
                TagObjects.RegisterTaggedObject(item, this.gameObject);
            }
        }
        private void OnDisable()
        {
            foreach (var item in tags)
            {
                TagObjects.UnregisterTaggedObject(item, this.gameObject);
            }
        }
        /// <summary>
        /// Tag an object with a specific tag 
        /// NOTE: tags are case sensitive!
        /// </summary>
        /// <param name="tag"></param>
        public void TagObject(string tag)
        {
            TagObjects.RegisterTaggedObject(tag, this.gameObject);
        }

        /// <summary>
        /// Untag an object with a specific tag 
        /// NOTE: tags are case sensitive!
        /// </summary>
        /// <param name="tag"></param>
        public void UntagObject(string tag)
        {
            TagObjects.UnregisterTaggedObject(tag, this.gameObject);
        }
    }
}