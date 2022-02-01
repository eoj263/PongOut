using System;
using System.Diagnostics.CodeAnalysis;

namespace PongOut
{
    /// <summary>
    /// An item representing a score.
    /// </summary>
    public class HighScoreItem : IComparable<HighScoreItem>
    {
        public string Name { get; private set; }
        public int Score { get; private set; }

        public HighScoreItem(string name, int score)
        {
            Name = name;
            Score = score;
        }

        public static HighScoreItem FromSaveString(string hsiString)
        {
            string[] vals = hsiString.Trim().Split('\t');

            if (vals.Length != 2)
                throw new ArgumentException("The passed string was not in the correct format");

            string name = vals[0];
            int score;
            if(!int.TryParse(vals[1], out score))
                throw new ArgumentException("The passed string's score was not an integer");

            return new HighScoreItem(name, score);
        }

        public string ToSaveString()
        {
            return $"{Name.Trim()}\t{Score}";
        }

        public int CompareTo([AllowNull] HighScoreItem other)
        {
            return other.Score.CompareTo(Score);
        }

        public override string ToString()
        {
            return $"{Score} poäng av: {Name}";
        }

    }
}
