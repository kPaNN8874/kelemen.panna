using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Memoria
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<int> cardValues;           // Kártyaértékek tárolása
        private List<Button> cards;             // Gombok listája a kártyákhoz
        private Button firstCard, secondCard;   // Az aktuálisan felfordított kártyák
        private DispatcherTimer gameTimer;      // Időzítő a játék idő méréséhez
        private int elapsedTime;                // Eltelt idő másodpercben

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            InitializeCardValues();
            CreateCardButtons();
            StartTimer();
        }

        // Kártyaértékek inicializálása és keverése
        private void InitializeCardValues()
        {
            cardValues = new List<int> { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8 };
            cardValues = cardValues.OrderBy(a => Guid.NewGuid()).ToList(); // Véletlenszerű sorrend
        }

        // Kártyagombok létrehozása és hozzáadása a GameBoard-hoz
        private void CreateCardButtons()
        {
            GameBoard.Children.Clear();
            cards = new List<Button>();

            for (int i = 0; i < cardValues.Count; i++)
            {
                Button btn = new Button
                {
                    Tag = cardValues[i],
                    Style = (Style)FindResource("RoundedButtonStyle"), // A lekerekített stílus alkalmazása
                    Background = new SolidColorBrush(Color.FromRgb(60, 63, 81)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(239, 35, 60)),
                    BorderThickness = new Thickness(2),
                    Padding = new Thickness(5),
                    Cursor = Cursors.Hand,
                    Margin = new Thickness(5)
                };

                btn.Click += Card_Click;
                cards.Add(btn);
                GameBoard.Children.Add(btn); // Hozzáadás a rácshoz
            }
        }

        // Timer elindítása
        private void StartTimer()
        {
            elapsedTime = 0;
            TimerText.Text = "Idő: 0 mp";
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += (s, e) =>
            {
                elapsedTime++;
                TimerText.Text = $"Idő: {elapsedTime} mp";
            };
            gameTimer.Start();
        }

        // Kártyára kattintás kezelése
        private void Card_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == null || clickedButton == firstCard || clickedButton.Tag == null)
                return;

            // Kártya háttérszínének változtatása a felfordított állapotban
            clickedButton.Background = new SolidColorBrush(Color.FromRgb(239, 35, 60)); // Piros szín, amikor felfordítjuk a kártyát

            clickedButton.Content = clickedButton.Tag.ToString(); // Felfordítás

            // Animáció elindítása
            Storyboard flipStoryboard = (Storyboard)FindResource("FlipCardStoryboard");
            flipStoryboard.Begin(clickedButton);

            if (firstCard == null)
            {
                firstCard = clickedButton;
            }
            else
            {
                secondCard = clickedButton;

                if (firstCard.Tag.ToString() == secondCard.Tag.ToString())
                {
                    // Páros: kártyák megtartása felfordítva
                    firstCard.IsEnabled = false;
                    secondCard.IsEnabled = false;
                    firstCard = null;
                    secondCard = null;

                    if (cards.All(c => !c.IsEnabled))
                    {
                        gameTimer.Stop();
                        MessageBox.Show($"Gratulálunk! Teljesítetted a játékot {elapsedTime} másodperc alatt.");
                    }
                }
                else
                {
                    // Nem páros: visszafordítás rövid idő múlva
                    DispatcherTimer flipBackTimer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(500)
                    };
                    flipBackTimer.Tick += (s, args) =>
                    {
                        // Kártya visszafordítása és háttérszín visszaállítása
                        firstCard.Background = new SolidColorBrush(Color.FromRgb(60, 63, 81)); // Eredeti háttérszín
                        secondCard.Background = new SolidColorBrush(Color.FromRgb(60, 63, 81)); // Eredeti háttérszín
                        firstCard.Content = "";
                        secondCard.Content = "";
                        firstCard = null;
                        secondCard = null;
                        flipBackTimer.Stop();
                    };
                    flipBackTimer.Start();
                }
            }
        }

        // Új játék indítása
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            gameTimer.Stop();
            InitializeGame();
        }
    }
}
