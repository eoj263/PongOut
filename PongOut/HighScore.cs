using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace PongOut
{
    public class HighScore
    {
        public const string SCORES_SAVE_PATH = "highScores.txt";
        public const int MAX_SCORES_TO_DISPLAY = 10;

        public const int MAX_CHARS_IN_NAME = 12;

        ScreenText charCountText;
        ScreenText pointsText;
        ScreenText infoText;

        public HighScore()
        {
            highScoreListText = new ScreenText(new Vector2(200, 200));
            namePrevewText = new ScreenText(new Vector2(300, 350));
            charCountText = new ScreenText(new Vector2(490, 350));
            pointsText = new ScreenText(new Vector2(300, 200));
            infoText = new ScreenText(new Vector2(20, 20));

            GameElements.LoadContentsOf(highScoreListText);
            GameElements.LoadContentsOf(namePrevewText);
            GameElements.LoadContentsOf(charCountText);
            GameElements.LoadContentsOf(pointsText);
            GameElements.LoadContentsOf(infoText);

            infoText.Text = "För å, ä och ö tryck på knapparna 1, 2 och 3\nTryck på ENTER när du är klar";
        }

        void AddScore(string name, int score)
        {
            this.scores.Add(new HighScoreItem(name, score));
            SaveToFile();
        }


        public void SaveToFile()
        {
            StringBuilder builder = new StringBuilder();
            for(int i = 0; i < scores.Length; i++) {
                builder.AppendLine(scores[i].ToSaveString());
            }

            using(var fs = new StreamWriter(SCORES_SAVE_PATH))
            {
                fs.Write(builder);
            }
        }

        public void InitializeEnterName()
        {
            UpdatePreview();

            // We find the current rank and add 1 since "normal people" don't start counting from 0
            int placement = PlacementOfScore(GameElements.World.Score) + 1;

            // Find which letter should follow the ranking(1:a, 2:a, 3:e, 4:e)
            char suffix = placement < 3 ? 'a' : 'e';
            pointsText.Text = $"Du fick {GameElements.World.Score} poäng. (Du kom alltså på {placement}:{suffix} plats av {scores.Length})";
        }

        Keys lastKeyPressed;
        float timeSincePress = 0;
        string name = "";

        readonly string allowedKeys = "ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖ ";

        // Returns true if the name has been entered
        public bool EnterNameUpdate(GameTime gt)
        {
            timeSincePress += gt.ElapsedGameTime.Milliseconds;
            KeyboardState kbs = Keyboard.GetState();
            if(kbs.GetPressedKeyCount() == 0)
            {
                lastKeyPressed = Keys.None;
                return false;
            }

            if (kbs.IsKeyDown(Keys.Enter))
            {
                AddScore(name, GameElements.lastScore);
                return true;
            }

            Keys keyPressed = kbs.GetPressedKeys()[0];
            string keyName = keyPressed.ToString();

            if (keyPressed == lastKeyPressed && timeSincePress < 150)
                return false;

            bool wasValidKey = TryRegisterKey(keyName);
            if (wasValidKey)
            {
                lastKeyPressed = keyPressed;
                timeSincePress = 0;
            }

            if (name.Length > MAX_CHARS_IN_NAME)
                GameElements.FlashMessage(new FlashedMessage("Du har redan uppnått max antal tecken"));

            name = name.Substring(0, Math.Min(name.Length, MAX_CHARS_IN_NAME));
            UpdatePreview();
            return false;
        }


        // What rank a score would get you. E.g If you got the best highscore this method will return 0
        int PlacementOfScore(int score)
        {
            for(int i = 0; i < scores.Length; i++)
            {
                if (scores[i].Score < score)
                    return i;
            }

            return scores.Length - 1;
        }

        /// <summary>
        /// Uppdaterar char count texten och namn previewn
        /// </summary>
        private void UpdatePreview() { 
            charCountText.Text = $"{name.Length}/{MAX_CHARS_IN_NAME}";
            charCountText.Color = MAX_CHARS_IN_NAME == name.Length ? Color.Red : Color.White;
            namePrevewText.Text = $"Ditt namn: {name}";

        }

        public bool TryRegisterKey(string keyName)
        {
            switch (keyName) {
                case "Back":
                    if(name.Length > 0)
                        name = name.Substring(0, name.Length - 1);
                    return true;
                case "Space":
                    name += " ";
                    return true;

                // The following exist because the letters Å, Ä and Ö are not properly registered by monogame
                case "D1": // Digit 1
                    name += "Å";
                    return true;
                case "D2": // Digit 2
                    name += "Ä";
                    return true;
                case "D3": // Digit 3
                    name += "Ö";
                    return true;
            }

            if (keyName.Length != 1)
            {
                // Typ en bug här ish ( Meddelandet skickas väldigt många gånger )
                GameElements.FlashMessage(new FlashedMessage("Okänd knapp :'("));
                return false;
            }

            char key = keyName[0];
            if (!allowedKeys.Contains(key))
                return false;

            name += key;
            return true;
        }


        ScreenText highScoreListText;

        ScreenText namePrevewText;

        public void EnterNameDraw(SpriteBatch sb)
        {
            namePrevewText.Draw(sb);
            charCountText.Draw(sb);
            pointsText.Draw(sb);
            infoText.Draw(sb);
        }


        public void LoadFromFile()
        {
            scores = new SortedVector<HighScoreItem>();
            try
            {
                using (var reader = new StreamReader(SCORES_SAVE_PATH)) {
                    while (!reader.EndOfStream)
                    {
                        scores.Add(HighScoreItem.FromSaveString(reader.ReadLine()));
                    }
                }
            } catch(IOException e) {
                GameElements.WriteDebugLine(e.Message);
            } // Filen finns förmodligen inte.
        }

        SortedVector<HighScoreItem> scores;

        /// <summary>
        /// Draws the highscore board
        /// </summary>
        /// <param name="sb"></param>
        public void ListDraw(SpriteBatch sb)
        {
            highScoreListText.Text = "";
            for(int i = 0; i < Math.Min(scores.Length, MAX_SCORES_TO_DISPLAY); i++)
            {
                highScoreListText.Text += $"{scores[i]}\n";
            }

            highScoreListText.Draw(sb);
        }
    }

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
