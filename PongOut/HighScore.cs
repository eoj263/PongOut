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
        public const int MAX_SAVED_SCORES = 500;

        public const int MAX_CHARS = 12;

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
            UpdatePreview();
        }

        void AddScore(string name, int score)
        {
            this.scores.Add(new HighScoreItem(name, score));
            SaveToFile();
        }


        public void SaveToFile()
        {
            StringBuilder builder = new StringBuilder();
            for(int i = 0; i < scores.Length && i < MAX_SAVED_SCORES; i++) {
                builder.AppendLine(scores[i].ToSaveString());
            }

            using(var fs = new StreamWriter(SCORES_SAVE_PATH))
            {
                fs.Write(builder);
            }
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

            if (name.Length > MAX_CHARS)
                GameElements.FlashMessage(new FlashedMessage("Du har redan uppnått max antal tecken"));

            name = name.Substring(0, Math.Min(name.Length, MAX_CHARS));
            UpdatePreview();
            return false;
        }

        /// <summary>
        /// Uppdaterar char count texten och namn previewn
        /// </summary>
        private void UpdatePreview() { 
            charCountText.Text = $"{name.Length}/{MAX_CHARS}";
            charCountText.Color = MAX_CHARS == name.Length ? Color.Red : Color.White;
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

        public void ListDraw(SpriteBatch sb)
        {
            highScoreListText.Text = "";
            for(int i = 0; i < scores.Length; i++)
            {
                highScoreListText.Text += $"{scores[i]}\n";
            }

            highScoreListText.Draw(sb);
        }
    }

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
//class HSItem
//{
//	// Variabler och egenskaper för dem:
//	string name;
//	int points;

//	public string Name { get { return name; } set { name = value; } }

//	public int Points { get { return points; } set { points = value; } }

//	// =======================================================================
//	// HSItem(), klassens konstruktor
//	// =======================================================================
//	public HSItem(string name, int points)
//	{
//		this.name = name;
//		this.points = points;
//	}
//}

// =======================================================================
// HighScore, innehåller en lista med hsItems samt metoder för att
// manipulera listan.
// =======================================================================
//class HighScore
//{
//	int maxInList = 5; // Hur många som får vara i listan
//	List<HSItem> highscore = new List<HSItem>();
//	string name; // Spelarens namn

//	// Används för att skriva ut vilket tecken spelaren har valt just nu:
//	string currentChar;
//	int key_index = 0; // Denna används för att mata in spelarens namn
//					   // Dessa används för att kontrollera när tangenter trycktes in:
//	double lastChange = 0;
//	Keys previousKey;

//	// =======================================================================
//	// HighScore(), klassens konstruktor
//	// =======================================================================
//	public HighScore(int maxInList)
//	{
//		this.maxInList = maxInList;
//	}

//	// =======================================================================
//	// Sort(),  metod som sorterar listan. Metoden
//	// anropas av Add() när en ny person läggs till i
//	// listan. Använder algoritmen bubblesort
//	// =======================================================================
//	void Sort()
//	{
//		int max = highscore.Count - 1;

//		// Den yttre loopen, går igenom hela listan            
//		for (int i = 0; i < max; i++)
//		{
//			// Den inre, går igenom element för element
//			int nrLeft = max - i; // För att se hur många som redan gåtts igenom
//			for (int j = 0; j < nrLeft; j++)
//			{
//				if (highscore[j].Points < highscore[j + 1].Points) // Jämför elementen
//				{
//					// Byt plats!
//					HSItem temp = highscore[j];
//					highscore[j] = highscore[j + 1];
//					highscore[j + 1] = temp;
//				}
//			}
//		}
//	}

//	// =======================================================================
//	// Add(), lägger till en person i highscore-listan.
//	// =======================================================================
//	void Add(int points)
//	{
//		// Skapa en temporär variabel av typen HSItem:
//		HSItem temp = new HSItem(name, points);
//		// Lägg till tmp i listan. Observera att följande Add()
//		// tillhör klassen List (är alltså skapad av Microsoft).
//		// Metoden har endast samma namn, som just denna Add():
//		highscore.Add(temp);
//		Sort(); // Sortera listan efter att vi har lagt till en person!

//		// Är det för många i listan?
//		if (highscore.Count > maxInList)
//		{
//			// Eftersom vi har lagt till endast en person nu, så betyder
//			// det att det är en person för mycket. Index på personen
//			// som är sist i listan, är samma som maxInList. Vi vill ju
//			// att det högsta indexet ska vara maxInList-1. Allstå kan
//			// vi bara ta bort elementet med index maxInList.
//			// Exempel:
//			// maxInList är 5, vi har 6 element i listan. Det sjätte
//			// elementet har index 5. Vi gör highscore.RemoveAt(5):
//			highscore.RemoveAt(maxInList);
//		}
//	}

//	// =======================================================================
//	// CheckKey(), kontrollerar om en viss tangent har tryckts och huruvida
//	// det har gått lagomt lång tid (130ms) sedan tidigare tryck av samma
//	// tangent.
//	// =======================================================================
//	bool CheckKey(Keys key, GameTime gameTime)
//	{
//		KeyboardState keyboardState = Keyboard.GetState();
//		if (keyboardState.IsKeyDown(key))
//		{
//			// Har det gått lagomt lång tid, eller är det en helt annan
//			// tangent som trycks ned denna gång?
//			if (lastChange + 130 < gameTime.TotalGameTime.TotalMilliseconds
//				|| previousKey != key)
//			{
//				// sätt om variablerna inför nästa varv i spelloopen:
//				previousKey = key;
//				lastChange = gameTime.TotalGameTime.TotalMilliseconds;
//				return true;
//			}
//		}
//		// Just den tangenten (key) trycktes INTE ned, eller så trycktes den
//		// ned alldeles nyligen (mindre än 130ms):
//		return false;
//	}

//	// =======================================================================
//	// PrintDraw(), metod för att skriva ut listan. Det finns ingen
//	// PrintUpdate() då det är en helt statisk text som skrivs ut.
//	// =======================================================================
//	public void PrintDraw(SpriteBatch spriteBatch, SpriteFont font)
//	{
//		string text = "HIGHSCORE\n";
//		foreach (HSItem h in highscore)
//			text += h.Name + " " + h.Points + "\n";

//		spriteBatch.DrawString(font, text, Vector2.Zero, Color.White);
//	}

//	// =======================================================================
//	// EnterUpdate(), här matar användaren in sitt användarnamn. Precis som
//	// klassiska gamla arkadspel kan man ha tre tecken A-Z i sitt namn. Detta
//	// är Update-delen i spel-loopen för inmatning av highscore-namn. Metoden
//	// ska fortsätta anropas av Update() så länge true returneras.
//	// =======================================================================
//	public bool EnterUpdate(GameTime gameTime, int points)
//	{
//		// Vilka tecken som är möjliga:
//		char[] key = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K',
//					   'L', 'M', 'N', 'O', 'P',  'Q', 'R', 'S', 'T', 'U',
//					   'V', 'X', 'Y', 'Z'};


//		// Användaren trycker knappen nedåt, stega framlänges i key-vektorn:
//		if (CheckKey(Keys.Down, gameTime))
//		{
//			key_index++;
//			if (key_index >= key.Length)
//				key_index = 0;
//		}

//		// Användaren trycker knappen uppåt, stega baklänges i key-vektorn:
//		if (CheckKey(Keys.Up, gameTime))
//		{
//			key_index--;
//			if (key_index <= 0)
//				key_index = key.Length - 1;
//		}

//		// Användaren trycker ENTER, lägg till det valda tecknet i 
//		if (CheckKey(Keys.Enter, gameTime))
//		{
//			name += key[key_index].ToString();
//			if (name.Length == 3)
//			{
//				// Återställ namnet och allt så att man kan lägga till namnet 
//				// på en ny spelare:
//				Add(points);
//				name = "";
//				currentChar = "";
//				key_index = 0;
//				return true; // Ange att vi är klara
//			}
//		}
//		// Lagra det tecken som nu är valt, så att vi kan skriva ut det i
//		// EnterDraw():
//		currentChar = key[key_index].ToString();
//		// Ange att vi inte är klara, fortsätt anropa denna metod via Update():
//		return false;
//	}

//	// =======================================================================
//	// EnterDraw(), skriver ut de tecken spelaren har matat in av sitt namn
//	// (om något) samt det tecken (av tre) som just nu är valt.
//	// =======================================================================
//	public void EnterDraw(SpriteBatch spriteBatch, SpriteFont font)
//	{
//		string text = "ENTER NAME:" + name + currentChar;
//		spriteBatch.DrawString(font, text, Vector2.Zero, Color.White);
//	}

//	// =======================================================================
//	// SaveToFile(), spara till fil.
//	// =======================================================================
//	public void SaveToFile(string filename)
//	{
//	}

//	// =======================================================================
//	// LoadFromFile(), ladda från fil.
//	// =======================================================================
//	public void LoadFromFile(string filename)
//	{
//	}
//}
