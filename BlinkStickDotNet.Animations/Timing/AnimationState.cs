namespace BlinkStickDotNet.Animations
{
    public class State
    {
        public State(uint totalSteps, uint step)
        {
            TotalSteps = totalSteps;
            Step = step;

            if (totalSteps == 0)
            {
                Percentage = 1;
            }
            else
            {
                Percentage = (double)Step / (double)TotalSteps;
            }
        }

        public uint TotalSteps { get; private set; }

        public uint Step { get; private set; }

        public double Percentage { get; private set; }
    }
}