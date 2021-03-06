using System;

namespace Data.Progress
{
    [Serializable]
    public class LevelProgress
    {
        public bool isCompleted;
        public bool isUnlocked;
        public int countStars;
        public int countPoints;
        public bool[] rewardIsIssued = new bool[3];
    }
}