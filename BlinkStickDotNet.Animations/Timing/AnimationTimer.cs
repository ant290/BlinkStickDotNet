using System;
using System.Threading;

namespace BlinkStickDotNet.Animations.Timing
{
    public class AnimationTimer
    {
        private uint _durationInMs;
        private uint _steps;

        public static AnimationTimer CreateFromHz(uint durationInMs, uint hz)
        {
            var steps = ((double)durationInMs / 1000d) * (double)hz;
            return new AnimationTimer(durationInMs, (uint)steps);
        }

        public AnimationTimer(uint durationInMs, uint steps)
        {
            _durationInMs = durationInMs;
            _steps = steps;
        }

        public void Start(Action<State> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            //single action
            var start = DateTime.Now;
            if (_durationInMs <= 1)
            {
                action(new State(_steps, _steps));
                return;
            }

            //multiple actions
            using (var timer = new System.Timers.Timer())
            {
                using (var gate = new AutoResetEvent(false))
                {
                    timer.Interval = (double)_durationInMs / (double)_steps;
                    timer.Elapsed += (s, e) => gate.Set();
                    timer.Enabled = true;

                    uint previousStep = uint.MaxValue;

                    while (true)
                    {
                        var elapsedTime = DateTime.Now - start;
                        var percentage = elapsedTime.TotalMilliseconds / (double)_durationInMs;
                        var step = (uint)Math.Ceiling(percentage * (double)_steps);

                        if(step > _steps)
                        {
                            step = _steps;
                        }

                        if (previousStep != step)
                        {
                            var state = new State(_steps, step);
                            action(state);
                            previousStep = step;
                        }

                        if (step == _steps)
                        {
                            break;
                        }

                        gate.WaitOne();
                    }

                    timer.Enabled = false;
                }
            }
        }
    }
}