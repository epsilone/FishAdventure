using UnityEngine;
using System.Collections;

public static class Interpolation
{	
//	public enum EaseType 
//	{
//       Linear,
//       EaseInQuad,
//       EaseOutQuad,
//       EaseInOutQuad,
//       EaseInCubic,
//       EaseOutCubic,
//       EaseInOutCubic,
//       EaseInQuart,
//       EaseOutQuart,
//       EaseInOutQuart,
//       EaseInQuint,
//       EaseOutQuint,
//       EaseInOutQuint,
//       EaseInSine,
//       EaseOutSine,
//       EaseInOutSine,
//       EaseInExpo,
//       EaseOutExpo,
//       EaseInOutExpo,
//       EaseInCirc,
//       EaseOutCirc,
//       EaseInOutCirc
//   }
//	
//	public static Function Ease(EaseType type) 
//	{
//		Function f = null;
//		switch (type) 
//		{
//			case EaseType.Linear: f = Interpolate.Linear; break;
//			case EaseType.EaseInQuad: f = Interpolate.EaseInQuad; break;
//			case EaseType.EaseOutQuad: f = Interpolate.EaseOutQuad; break;
//			case EaseType.EaseInOutQuad: f = Interpolate.EaseInOutQuad; break;
//			case EaseType.EaseInCubic: f = Interpolate.EaseInCubic; break;
//			case EaseType.EaseOutCubic: f = Interpolate.EaseOutCubic; break;
//			case EaseType.EaseInOutCubic: f = Interpolate.EaseInOutCubic; break;
//			case EaseType.EaseInQuart: f = Interpolate.EaseInQuart; break;
//			case EaseType.EaseOutQuart: f = Interpolate.EaseOutQuart; break;
//			case EaseType.EaseInOutQuart: f = Interpolate.EaseInOutQuart; break;
//			case EaseType.EaseInQuint: f = Interpolate.EaseInQuint; break;
//			case EaseType.EaseOutQuint: f = Interpolate.EaseOutQuint; break;
//			case EaseType.EaseInOutQuint: f = Interpolate.EaseInOutQuint; break;
//			case EaseType.EaseInSine: f = Interpolate.EaseInSine; break;
//			case EaseType.EaseOutSine: f = Interpolate.EaseOutSine; break;
//			case EaseType.EaseInOutSine: f = Interpolate.EaseInOutSine; break;
//			case EaseType.EaseInExpo: f = Interpolate.EaseInExpo; break;
//			case EaseType.EaseOutExpo: f = Interpolate.EaseOutExpo; break;
//			case EaseType.EaseInOutExpo: f = Interpolate.EaseInOutExpo; break;
//			case EaseType.EaseInCirc: f = Interpolate.EaseInCirc; break;
//			case EaseType.EaseOutCirc: f = Interpolate.EaseOutCirc; break;
//			case EaseType.EaseInOutCirc: f = Interpolate.EaseInOutCirc; break;
//		}
//		return f;
//	}
	
   /**
     * Linear interpolation (same as Mathf.Lerp)
     */
    public  static float Linear(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
        return distance * (elapsedTime / duration) + start;
    }

    /**
     * quadratic easing in - accelerating from zero velocity
     */
    public  static float EaseInQuad(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return distance * elapsedTime * elapsedTime + start;
    }

    /**
     * quadratic easing out - decelerating to zero velocity
     */
    public  static float EaseOutQuad(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return -distance * elapsedTime * (elapsedTime - 2) + start;
    }

    /**
     * quadratic easing in/out - acceleration until halfway, then deceleration
     */
    public  static float EaseInOutQuad(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
        if (elapsedTime < 1) return distance / 2 * elapsedTime * elapsedTime + start;
        elapsedTime--;
        return -distance / 2 * (elapsedTime * (elapsedTime - 2) - 1) + start;
    }

    /**
     * cubic easing in - accelerating from zero velocity
     */
    public  static float EaseInCubic(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return distance * elapsedTime * elapsedTime * elapsedTime + start;
    }

    /**
     * cubic easing out - decelerating to zero velocity
     */
    public  static float EaseOutCubic(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        elapsedTime--;
        return distance * (elapsedTime * elapsedTime * elapsedTime + 1) + start;
    }

    /**
     * cubic easing in/out - acceleration until halfway, then deceleration
     */
    public  static float EaseInOutCubic(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
        if (elapsedTime < 1) return distance / 2 * elapsedTime * elapsedTime * elapsedTime + start;
        elapsedTime -= 2;
        return distance / 2 * (elapsedTime * elapsedTime * elapsedTime + 2) + start;
    }

    /**
     * quartic easing in - accelerating from zero velocity
     */
    public  static float EaseInQuart(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
    }

    /**
     * quartic easing out - decelerating to zero velocity
     */
    public  static float EaseOutQuart(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        elapsedTime--;
        return -distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 1) + start;
    }

    /**
     * quartic easing in/out - acceleration until halfway, then deceleration
     */
    public  static float EaseInOutQuart(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
        if (elapsedTime < 1) return distance / 2 * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
        elapsedTime -= 2;
        return -distance / 2 * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 2) + start;
    }


    /**
     * quintic easing in - accelerating from zero velocity
     */
    public static float EaseInQuint(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
    }

    /**
     * quintic easing out - decelerating to zero velocity
     */
    public  static float EaseOutQuint(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        elapsedTime--;
        return distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 1) + start;
    }

    /**
     * quintic easing in/out - acceleration until halfway, then deceleration
     */
    public  static float EaseInOutQuint(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2f);
        if (elapsedTime < 1) return distance / 2 * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
        elapsedTime -= 2;
        return distance / 2 * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 2) + start;
    }

    /**
     * sinusoidal easing in - accelerating from zero velocity
     */
    public static float EaseInSine(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
        return -distance * Mathf.Cos(elapsedTime / duration * (Mathf.PI / 2)) + distance + start;
    }

    /**
     * sinusoidal easing out - decelerating to zero velocity
     */
    static float EaseOutSine(float start, float distance, float elapsedTime, float duration) {
        if (elapsedTime > duration) { elapsedTime = duration; }
        return distance * Mathf.Sin(elapsedTime / duration * (Mathf.PI / 2)) + start;
    }

    /**
     * sinusoidal easing in/out - accelerating until halfway, then decelerating
     */
    static float EaseInOutSine(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
        return -distance / 2 * (Mathf.Cos(Mathf.PI * elapsedTime / duration) - 1) + start;
    }

    /**
     * exponential easing in - accelerating from zero velocity
     */
    public static float EaseInExpo(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
        return distance * Mathf.Pow(2, 10 * (elapsedTime / duration - 1)) + start;
    }

    /**
     * exponential easing out - decelerating to zero velocity
     */
    public static float EaseOutExpo(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
        return distance * (-Mathf.Pow(2, -10 * elapsedTime / duration) + 1) + start;
    }

    /**
     * exponential easing in/out - accelerating until halfway, then decelerating
     */
    public static float EaseInOutExpo(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
        if (elapsedTime < 1) return distance / 2 *  Mathf.Pow(2, 10 * (elapsedTime - 1)) + start;
        elapsedTime--;
        return distance / 2 * (-Mathf.Pow(2, -10 * elapsedTime) + 2) + start;
    }

    /**
     * circular easing in - accelerating from zero velocity
     */
    public static float EaseInCirc(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return -distance * (Mathf.Sqrt(1 - elapsedTime * elapsedTime) - 1) + start;
    }

    /**
     * circular easing out - decelerating to zero velocity
     */
    public static float EaseOutCirc(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        elapsedTime--;
        return distance * Mathf.Sqrt(1 - elapsedTime * elapsedTime) + start;
    }

    /**
     * circular easing in/out - acceleration until halfway, then deceleration
     */
    public static float EaseInOutCirc(float start, float distance, float
                         elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
        if (elapsedTime < 1) return -distance / 2 * (Mathf.Sqrt(1 - elapsedTime * elapsedTime) - 1) + start;
        elapsedTime -= 2;
        return distance / 2 * (Mathf.Sqrt(1 - elapsedTime * elapsedTime) + 1) + start;
    }
}

