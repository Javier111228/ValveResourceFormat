using System;
using GUI.Utils;
using ValveResourceFormat.Serialization;

namespace GUI.Types.ParticleRenderer.Operators
{
    class FadeAndKill : IParticleOperator
    {
        private readonly float startFadeInTime;
        private readonly float endFadeInTime = 0.5f;
        private readonly float startFadeOutTime = 0.5f;
        private readonly float endFadeOutTime = 1f;

        private readonly float startAlpha = 1f;
        private readonly float endAlpha;

        public FadeAndKill(IKeyValueCollection keyValues)
        {
            if (keyValues.ContainsKey("m_flStartFadeInTime"))
            {
                startFadeInTime = keyValues.GetFloatProperty("m_flStartFadeInTime");
            }

            if (keyValues.ContainsKey("m_flEndFadeInTime"))
            {
                endFadeInTime = keyValues.GetFloatProperty("m_flEndFadeInTime");
            }

            if (keyValues.ContainsKey("m_flStartFadeOutTime"))
            {
                startFadeOutTime = keyValues.GetFloatProperty("m_flStartFadeOutTime");
            }

            if (keyValues.ContainsKey("m_flEndFadeOutTime"))
            {
                endFadeOutTime = keyValues.GetFloatProperty("m_flEndFadeOutTime");
            }

            if (keyValues.ContainsKey("m_flStartAlpha"))
            {
                startAlpha = keyValues.GetFloatProperty("m_flStartAlpha");
            }

            if (keyValues.ContainsKey("m_flEndAlpha"))
            {
                endAlpha = keyValues.GetFloatProperty("m_flEndAlpha");
            }
        }

        public void Update(Span<Particle> particles, float frameTime, ParticleSystemRenderState particleSystemState)
        {
            for (var i = 0; i < particles.Length; ++i)
            {
                var time = particles[i].NormalizedAge;

                // If fading in
                if (time >= startFadeInTime && time <= endFadeInTime)
                {
                    var blend = MathUtils.Remap(time, startFadeInTime, endFadeInTime);

                    // Interpolate from startAlpha to constantAlpha
                    particles[i].Alpha = MathUtils.Lerp(blend, startAlpha, particles[i].InitialAlpha);
                }

                // If fading out
                if (time >= startFadeOutTime && time <= endFadeOutTime)
                {
                    var blend = MathUtils.Remap(time, startFadeOutTime, endFadeOutTime);

                    // Interpolate from constantAlpha to end alpha
                    particles[i].Alpha = MathUtils.Lerp(blend, particles[i].InitialAlpha, endAlpha);
                }

                if (time >= endFadeOutTime)
                {
                    particles[i].Kill();
                }
            }
        }
    }
}
