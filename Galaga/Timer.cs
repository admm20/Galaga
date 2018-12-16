using System;
using System.Collections.Generic;
using System.Text;

namespace Galaga
{
    /* Simple timer that invokes method after some amount of time
     * 
     * Examples:
     * // It will write "Test" every 1000ms
     * Timer t1 = new Timer( (t) => {Console.WriteLine("Test");}, 1000, true};
     * 
     * // It will write "Test_2" after 500ms and timer will be destroyed
     * Timer t2 = new Timer( (t) => {Console.WriteLine("Test_2");}, 500, false};
     * 
     */
    class Timer
    {
        // list of all existing timers
        public static List<Timer> CreatedTimers = new List<Timer>();

        // list of timers that will be deleted in next update
        private static List<Timer> TimersToBeDeleted = new List<Timer>();

        public delegate void Action(float totalTimeElapsed);
        private Action action;

        public float TotalTimeElapsed = 0;
        public float Period;
        public float TimeElapsedSinceReset = 0;
        public bool Repeatable;
        
        public Timer(Action action, float period, bool repeatable)
        {
            this.Period = period;
            this.action = (Action)action;
            this.Repeatable = repeatable;
            CreatedTimers.Add(this);
        }

        private void Update(int deltaTime)
        {
            TotalTimeElapsed += deltaTime;
            TimeElapsedSinceReset += deltaTime;

            if (TimeElapsedSinceReset >= Period)
            {
                action(TotalTimeElapsed);
                TimeElapsedSinceReset = 0;

                if(!Repeatable)
                    TimersToBeDeleted.Add(this);
            }
        }

        private void Delete()
        {
            TimersToBeDeleted.Add(this);
        }

        public static void UpdateAllTimers(int deltaTime)
        {
            foreach (Timer t in TimersToBeDeleted)
            {
                CreatedTimers.Remove(t);
            }
            TimersToBeDeleted.Clear();

            foreach (Timer t in CreatedTimers)
            {
                t.Update(deltaTime);
            }
        }

    }
}
