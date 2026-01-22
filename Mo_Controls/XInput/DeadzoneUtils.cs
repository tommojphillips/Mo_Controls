using UnityEngine;

namespace TommoJProductions.MoControlsV2.XInput {
    internal static class Deadzone_Utils {
        public static float apply_trigger_deadzone(byte x, float deadzone) {
            float fx = x / 255.0f;
            float dz01 = deadzone / 100.0f;
            fx = abs(fx) < dz01 ? 0f : sign(fx) * ((abs(fx) - dz01) / (1f - dz01));
            fx = clamp(fx);
            return fx;
        }

        public static Vector2 apply_stick_deadzone(short x, short y, float deadzone) {
            float fx = x / 32767.0f;
            float fy = y / 32767.0f;
            float dz01 = deadzone / 100.0f;
            float magnitude = Mathf.Sqrt(fx * fx + fy * fy);

            if (magnitude < dz01) {
                return Vector2.zero;
            }

            float new_magnitude = (magnitude - dz01) / (1 - dz01);
            new_magnitude = clamp01(new_magnitude);

            float scale = new_magnitude / magnitude;
            return new Vector2(fx * scale, fy * scale);
        }

        private static float clamp(float value) {
            return value < -1.0f ? -1.0f : (value > 1.0f ? 1.0f : value);
        }
        private static float clamp01(float value) {
            return value < 0f ? 0f : (value > 1.0f ? 1.0f : value);
        }
        private static float abs(float value) {
            return value >= 0 ? value : -value;
        }
        private static float sign(float f) {
            return (f < 0f) ? (-1f) : 1f;
        }
    }
}
