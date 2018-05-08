using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine.UI.Extensions
{
    public class uMath
    {
        /// <summary>
        /// Mathf.Lerp(from, to, Time.deltaTime * strength) is not framerate-independent. This function is.
        /// </summary>

        static public float SpringLerp(float from, float to, float strength, float deltaTime)
        {
            if (deltaTime > 1f) deltaTime = 1f;
            int ms = Mathf.RoundToInt(deltaTime * 1000f);
            deltaTime = 0.001f * strength;
            for (int i = 0; i < ms; ++i) from = Mathf.Lerp(from, to, deltaTime);
            return from;
        }

        /// <summary>
        /// Vector2.Lerp(from, to, Time.deltaTime * strength) is not framerate-independent. This function is.
        /// </summary>

        static public Vector2 SpringLerp(Vector2 from, Vector2 to, float strength, float deltaTime)
        {
            return Vector2.Lerp(from, to, SpringLerp(strength, deltaTime));
        }

        /// <summary>
        /// Vector3.Lerp(from, to, Time.deltaTime * strength) is not framerate-independent. This function is.
        /// </summary>

        static public Vector3 SpringLerp(Vector3 from, Vector3 to, float strength, float deltaTime)
        {
            return Vector3.Lerp(from, to, SpringLerp(strength, deltaTime));
        }

        /// <summary>
        /// Quaternion.Slerp(from, to, Time.deltaTime * strength) is not framerate-independent. This function is.
        /// </summary>

        static public Quaternion SpringLerp(Quaternion from, Quaternion to, float strength, float deltaTime)
        {
            return Quaternion.Slerp(from, to, SpringLerp(strength, deltaTime));
        }

        /// <summary>
        /// Calculate how much to interpolate by.
        /// </summary>

        static public float SpringLerp(float strength, float deltaTime)
        {
            if (deltaTime > 1f) deltaTime = 1f;
            int ms = Mathf.RoundToInt(deltaTime * 1000f);
            deltaTime = 0.001f * strength;
            float cumulative = 0f;
            for (int i = 0; i < ms; ++i) cumulative = Mathf.Lerp(cumulative, 1f, deltaTime);
            return cumulative;
        }


        /// <summary>
        /// This code is not framerate-independent:
        /// 
        /// target.position += velocity;
        /// velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * 9f);
        /// 
        /// But this code is:
        /// 
        /// target.position += NGUIMath.SpringDampen(ref velocity, 9f, Time.deltaTime);
        /// </summary>

        static public Vector3 SpringDampen(ref Vector3 velocity, float strength, float deltaTime)
        {
            if (deltaTime > 1f) deltaTime = 1f;
            float dampeningFactor = 1f - strength * 0.001f;
            int ms = Mathf.RoundToInt(deltaTime * 1000f);
            float totalDampening = Mathf.Pow(dampeningFactor, ms);
            Vector3 vTotal = velocity * ((totalDampening - 1f) / Mathf.Log(dampeningFactor));
            velocity = velocity * totalDampening;
            return vTotal * 0.06f;
        }

        /// <summary>
        /// Same as the Vector3 version, it's a framerate-independent Lerp.
        /// </summary>

        static public Vector2 SpringDampen(ref Vector2 velocity, float strength, float deltaTime)
        {
            if (deltaTime > 1f) deltaTime = 1f;
            float dampeningFactor = 1f - strength * 0.001f;
            int ms = Mathf.RoundToInt(deltaTime * 1000f);
            float totalDampening = Mathf.Pow(dampeningFactor, ms);
            Vector2 vTotal = velocity * ((totalDampening - 1f) / Mathf.Log(dampeningFactor));
            velocity = velocity * totalDampening;
            return vTotal * 0.06f;
        }
    }
}
