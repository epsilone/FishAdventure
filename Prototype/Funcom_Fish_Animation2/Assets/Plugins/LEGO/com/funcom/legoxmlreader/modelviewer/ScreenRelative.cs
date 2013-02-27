using UnityEngine;

namespace com.funcom.legoxmlreader.modelviewer
{
    public class ScreenRelative
    {
        public static Rect rect(float x, float y, float w, float h)
        {
            return new Rect(Screen.width * x, Screen.height * y, Screen.width * w, Screen.height * h);
        }
        public static Rect rect(Rect r)
        {
            return new Rect(Screen.width * r.x, Screen.height * r.y, Screen.width * r.width, Screen.height * r.height);
        }
    }
}

