using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;


    public class NotificationPopup : MonoBehaviour
    {
        public TMP_Text Text;
        public Image ForegroundEffect;


        TweenerCore<Quaternion, Vector3, QuaternionOptions> t1;

        public IEnumerator Initialize(string text, int seconds)
        {
            var rectPos = GetComponent<RectTransform>();
            rectPos.anchoredPosition = new Vector2(0, -250);
            rectPos.DOAnchorPos(new Vector2(0, -100), 0.3f);
            this.Text.text = text;

            t1 = ForegroundEffect.transform.DOLocalRotate(new Vector3(0, 0, 360),
                 2, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1);
          
            yield return new WaitForSecondsRealtime(seconds);

            rectPos.DOAnchorPos(new Vector2(0, -250), 0.3f);
            yield return new WaitForSecondsRealtime(0.3f);
            Destroy(gameObject);
        }


        private void OnDestroy()
        {
            t1?.Kill();
        }
    }