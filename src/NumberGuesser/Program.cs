using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace AdvancedNumberGuesser
{
    class Program
    {
        // تنظیمات بازی
        private static int minRange = 1;
        private static int maxRange = 100;
        private static int maxAttempts = 10;
        private static int score = 1000;
        private static int gamesPlayed = 0;
        private static int gamesWon = 0;

        // تاریخچه بازی‌ها
        private static List<GameHistory> gameHistory = new List<GameHistory>();

        // وضعیت بازی فعلی
        private static int targetNumber;
        private static int remainingAttempts;
        private static int currentScore;
        private static DateTime gameStartTime;
        private static List<int> guessHistory = new List<int>();

        // نمادهای نمایشی
        private const string SEPARATOR = "===================================================";

        static void Main(string[] args)
        {
            // تنظیم encoding برای پشتیبانی از کاراکترهای فارسی
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            // نمایش صفحه خوشآمدگویی
            DisplayWelcomeScreen();

            // حلقه اصلی برنامه
            bool exitRequested = false;
            while (!exitRequested)
            {
                DisplayMainMenu();
                string choice = GetUserInput("انتخاب شما: ").ToLower();

                switch (choice)
                {
                    case "1":
                        StartNewGame();
                        break;
                    case "2":
                        DisplaySettings();
                        break;
                    case "3":
                        DisplayStatistics();
                        break;
                    case "4":
                        DisplayGameHistory();
                        break;
                    case "5":
                        DisplayHelp();
                        break;
                    case "6":
                    case "exit":
                        exitRequested = true;
                        break;
                    default:
                        DisplayMessage("لطفاً یک گزینه معتبر انتخاب کنید.", ConsoleColor.Red);
                        Thread.Sleep(1000);
                        break;
                }
            }

            DisplayExitScreen();
        }

        /// <summary>
        /// نمایش صفحه خوشآمدگویی
        /// </summary>
        private static void DisplayWelcomeScreen()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(SEPARATOR);
            Console.WriteLine("          NUMBER GUESSER - بازی حدس عدد           ");
            Console.WriteLine("                 نسخه حرفه ای                     ");
            Console.WriteLine(SEPARATOR);
            Console.ResetColor();
            Console.WriteLine("\nبه بازی حدس عدد خوش آمدید!");
            Console.WriteLine("عدد تصادفی بین محدوده انتخاب شده تولید می‌شود و شما باید آن را حدس بزنید.");
            Console.WriteLine("\nبرای ادامه یک کلید را فشار دهید...");
            Console.ReadKey();
        }

        /// <summary>
        /// نمایش منوی اصلی
        /// </summary>
        private static void DisplayMainMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(SEPARATOR);
            Console.WriteLine("                     منوی اصلی                      ");
            Console.WriteLine(SEPARATOR);
            Console.ResetColor();

            Console.WriteLine(" 1. شروع بازی جدید");
            Console.WriteLine(" 2. تنظیمات بازی");
            Console.WriteLine(" 3. آمار و اطلاعات");
            Console.WriteLine(" 4. تاریخچه بازی‌ها");
            Console.WriteLine(" 5. راهنما");
            Console.WriteLine(" 6. خروج");
            Console.WriteLine();
        }

        /// <summary>
        /// شروع یک بازی جدید
        /// </summary>
        private static void StartNewGame()
        {
            // مقداردهی اولیه بازی جدید
            Random random = new Random();
            targetNumber = random.Next(minRange, maxRange + 1);
            remainingAttempts = maxAttempts;
            currentScore = score;
            guessHistory.Clear();
            gameStartTime = DateTime.Now;

            gamesPlayed++;

            bool gameWon = false;

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(SEPARATOR);
            Console.WriteLine("             شروع بازی جدید            ");
            Console.WriteLine(SEPARATOR);
            Console.ResetColor();

            Console.WriteLine($"من یک عدد بین {minRange} و {maxRange} انتخاب کردم.");
            Console.WriteLine($"شما {maxAttempts} فرصت برای حدس آن دارید.");
            Console.WriteLine("حدس شما چیست؟");
            Console.WriteLine();

            // حلقه اصلی بازی
            while (remainingAttempts > 0 && !gameWon)
            {
                Console.Write($"حدس #{maxAttempts - remainingAttempts + 1}: ");
                string input = Console.ReadLine();

                // بررسی اگر کاربر می‌خواهد بازی را رها کند
                if (input.ToLower() == "quit" || input.ToLower() == "exit")
                {
                    DisplayMessage("بازی متوقف شد.", ConsoleColor.Yellow);
                    return;
                }

                // بررسی اگر کاربر می‌خواهد عدد هدف را ببیند (قابلیت cheat)
                if (input.ToLower() == "reveal")
                {
                    DisplayMessage($"عدد هدف: {targetNumber} (این فقط برای تست است!)", ConsoleColor.Magenta);
                    continue;
                }

                // بررسی ورودی کاربر
                if (!int.TryParse(input, out int guess))
                {
                    DisplayMessage("لطفاً یک عدد معتبر وارد کنید!", ConsoleColor.Red);
                    continue;
                }

                // بررسی اگر عدد خارج از محدوده است
                if (guess < minRange || guess > maxRange)
                {
                    DisplayMessage($"لطفاً عددی بین {minRange} و {maxRange} وارد کنید!", ConsoleColor.Red);
                    continue;
                }

                // افزودن به تاریخچه حدس‌ها
                guessHistory.Add(guess);

                // بررسی حدس کاربر
                if (guess == targetNumber)
                {
                    gameWon = true;
                    gamesWon++;

                    // محاسبه زمان صرف شده
                    TimeSpan timeSpent = DateTime.Now - gameStartTime;

                    // محاسبه امتیاز نهایی (بر اساس زمان و تعداد حدس‌ها)
                    double timeBonus = Math.Max(0, 1 - timeSpent.TotalSeconds / 60); // حداکثر ۱ دقیقه زمان ایده‌آل
                    double attemptsBonus = Math.Max(0, 1 - (double)guessHistory.Count / maxAttempts);

                    currentScore = (int)(currentScore * (0.5 + 0.25 * timeBonus + 0.25 * attemptsBonus));

                    // نمایش پیام پیروزی
                    Console.WriteLine();
                    DisplayMessage("تبریک! شما عدد را correctly حدس زدید!", ConsoleColor.Green);
                    Console.WriteLine($"عدد هدف: {targetNumber}");
                    Console.WriteLine($"تعداد حدس‌ها: {guessHistory.Count}");
                    Console.WriteLine($"زمان صرف شده: {timeSpent:mm\\:ss} دقیقه");
                    Console.WriteLine($"امتیاز شما: {currentScore}");

                    // نمایش نمودار حدس‌ها
                    DisplayGuessDistribution();
                }
                else
                {
                    remainingAttempts--;
                    currentScore = Math.Max(0, currentScore - 100);

                    // ارائه راهنمایی به کاربر
                    if (guess < targetNumber)
                    {
                        DisplayMessage("عدد شما کوچک‌تر از عدد هدف است. ↑", ConsoleColor.Blue);
                    }
                    else
                    {
                        DisplayMessage("عدد شما بزرگ‌تر از عدد هدف است. ↓", ConsoleColor.Blue);
                    }

                    // نمایش محدوده فعلی بر اساس حدس‌های کاربر
                    DisplayCurrentRange();

                    Console.WriteLine($"فرصت‌های باقی‌مانده: {remainingAttempts}");
                    Console.WriteLine();

                    // اگر کاربر شکست خورد
                    if (remainingAttempts == 0)
                    {
                        Console.WriteLine();
                        DisplayMessage("متأسفانه شما شکست خوردید!", ConsoleColor.Red);
                        Console.WriteLine($"عدد هدف: {targetNumber}");

                        // نمایش نمودار حدس‌ها
                        DisplayGuessDistribution();
                    }
                }
            }

            // ذخیره تاریخچه بازی
            gameHistory.Add(new GameHistory
            {
                TargetNumber = targetNumber,
                Attempts = guessHistory.Count,
                Won = gameWon,
                Score = currentScore,
                PlayedDate = DateTime.Now,
                Duration = DateTime.Now - gameStartTime,
                GuessHistory = new List<int>(guessHistory)
            });

            Console.WriteLine();
            Console.WriteLine("برای بازگشت به منوی اصلی، یک کلید را فشار دهید...");
            Console.ReadKey();
        }

        /// <summary>
        /// نمایش محدوده فعلی بر اساس حدس‌های کاربر
        /// </summary>
        private static void DisplayCurrentRange()
        {
            int lowerBound = guessHistory.Where(g => g < targetNumber).DefaultIfEmpty(minRange - 1).Max() + 1;
            int upperBound = guessHistory.Where(g => g > targetNumber).DefaultIfEmpty(maxRange + 1).Min() - 1;

            lowerBound = Math.Max(minRange, lowerBound);
            upperBound = Math.Min(maxRange, upperBound);

            Console.WriteLine($"محدوده ممکن: {lowerBound} - {upperBound}");
        }

        /// <summary>
        /// نمایش توزیع حدس‌ها به صورت نمودار
        /// </summary>
        private static void DisplayGuessDistribution()
        {
            if (guessHistory.Count == 0) return;

            Console.WriteLine();
            Console.WriteLine("توزیع حدس‌های شما:");

            int minGuess = guessHistory.Min();
            int maxGuess = guessHistory.Max();

            // اگر تمام حدس‌ها یکسان باشند
            if (minGuess == maxGuess)
            {
                Console.WriteLine($"همه حدس‌های شما: {minGuess}");
                return;
            }

            // ایجاد یک نمودار ساده
            int chartWidth = 30;
            int range = maxGuess - minGuess;

            foreach (int guess in guessHistory.OrderBy(g => g))
            {
                int position = (int)((double)(guess - minGuess) / range * chartWidth);
                string bar = new string(' ', position) + "X";
                Console.WriteLine($"{guess,4}: {bar}");
            }

            // نمایش عدد هدف روی نمودار
            int targetPosition = (int)((double)(targetNumber - minGuess) / range * chartWidth);
            string targetBar = new string(' ', targetPosition) + "O";
            Console.WriteLine($"هدف: {targetBar} (عدد هدف: {targetNumber})");
        }

        /// <summary>
        /// نمایش تنظیمات بازی
        /// </summary>
        private static void DisplaySettings()
        {
            bool inSettings = true;

            while (inSettings)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(SEPARATOR);
                Console.WriteLine("                  تنظیمات بازی                  ");
                Console.WriteLine(SEPARATOR);
                Console.ResetColor();

                Console.WriteLine($" 1. محدوده اعداد: {minRange} - {maxRange}");
                Console.WriteLine($" 2. حداکثر تعداد حدس: {maxAttempts}");
                Console.WriteLine($" 3. امتیاز اولیه: {score}");
                Console.WriteLine(" 4. بازگشت به منوی اصلی");
                Console.WriteLine();

                string choice = GetUserInput("گزینه مورد نظر برای تغییر: ").ToLower();

                switch (choice)
                {
                    case "1":
                        Console.Write($"حد پایین جدید (فعلی: {minRange}): ");
                        if (int.TryParse(Console.ReadLine(), out int newMin) && newMin < maxRange - 1)
                        {
                            minRange = newMin;
                            DisplayMessage("حد پایین با موفقیت تغییر کرد.", ConsoleColor.Green);
                        }
                        else
                        {
                            DisplayMessage("مقدار نامعتبر است!", ConsoleColor.Red);
                        }
                        Thread.Sleep(1000);
                        break;

                    case "2":
                        Console.Write($"تعداد حدس مجاز جدید (فعلی: {maxAttempts}): ");
                        if (int.TryParse(Console.ReadLine(), out int newAttempts) && newAttempts > 0)
                        {
                            maxAttempts = newAttempts;
                            DisplayMessage("تعداد حدس مجاز با موفقیت تغییر کرد.", ConsoleColor.Green);
                        }
                        else
                        {
                            DisplayMessage("مقدار نامعتبر است!", ConsoleColor.Red);
                        }
                        Thread.Sleep(1000);
                        break;

                    case "3":
                        Console.Write($"امتیاز اولیه جدید (فعلی: {score}): ");
                        if (int.TryParse(Console.ReadLine(), out int newScore) && newScore > 0)
                        {
                            score = newScore;
                            DisplayMessage("امتیاز اولیه با موفقیت تغییر کرد.", ConsoleColor.Green);
                        }
                        else
                        {
                            DisplayMessage("مقدار نامعتبر است!", ConsoleColor.Red);
                        }
                        Thread.Sleep(1000);
                        break;

                    case "4":
                        inSettings = false;
                        break;

                    default:
                        DisplayMessage("گزینه نامعتبر!", ConsoleColor.Red);
                        Thread.Sleep(1000);
                        break;
                }
            }
        }

        /// <summary>
        /// نمایش آمار و اطلاعات
        /// </summary>
        private static void DisplayStatistics()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(SEPARATOR);
            Console.WriteLine("                  آمار و اطلاعات                   ");
            Console.WriteLine(SEPARATOR);
            Console.ResetColor();

            if (gamesPlayed == 0)
            {
                Console.WriteLine("هنوز هیچ بازی انجام نشده است.");
            }
            else
            {
                double winRate = (double)gamesWon / gamesPlayed * 100;
                int totalAttempts = gameHistory.Sum(g => g.Attempts);
                double avgAttempts = gamesPlayed > 0 ? (double)totalAttempts / gamesPlayed : 0;
                int totalScore = gameHistory.Sum(g => g.Score);
                int bestScore = gameHistory.Any() ? gameHistory.Max(g => g.Score) : 0;
                TimeSpan totalTime = TimeSpan.FromSeconds(gameHistory.Sum(g => g.Duration.TotalSeconds));

                Console.WriteLine($"تعداد بازی‌های انجام شده: {gamesPlayed}");
                Console.WriteLine($"تعداد بازی‌های برده: {gamesWon}");
                Console.WriteLine($"نرخ برد: {winRate:F2}%");
                Console.WriteLine($"میانگین تعداد حدس: {avgAttempts:F2}");
                Console.WriteLine($"مجموع امتیازات: {totalScore}");
                Console.WriteLine($"بالاترین امتیاز: {bestScore}");
                Console.WriteLine($"کل زمان بازی: {totalTime:mm\\:ss}");

                // نمایش توزیع تعداد حدس‌ها
                if (gameHistory.Any(g => g.Won))
                {
                    Console.WriteLine();
                    Console.WriteLine("توزیع تعداد حدس‌ها در بازی‌های برده:");

                    var wonGames = gameHistory.Where(g => g.Won).ToList();
                    var attemptGroups = wonGames.GroupBy(g => g.Attempts)
                                              .OrderBy(g => g.Key)
                                              .Select(g => new { Attempts = g.Key, Count = g.Count() });

                    int maxCount = attemptGroups.Max(g => g.Count);

                    foreach (var group in attemptGroups)
                    {
                        int barLength = maxCount > 0 ? (int)((double)group.Count / maxCount * 20) : 0;
                        string bar = new string('█', barLength);
                        Console.WriteLine($"{group.Attempts} حدس: {bar} {group.Count}");
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("برای بازگشت به منوی اصلی، یک کلید را فشار دهید...");
            Console.ReadKey();
        }

        /// <summary>
        /// نمایش تاریخچه بازی‌ها
        /// </summary>
        private static void DisplayGameHistory()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(SEPARATOR);
            Console.WriteLine("                 تاریخچه بازی‌ها                  ");
            Console.WriteLine(SEPARATOR);
            Console.ResetColor();

            if (gameHistory.Count == 0)
            {
                Console.WriteLine("هیچ بازی‌ای در تاریخچه وجود ندارد.");
            }
            else
            {
                Console.WriteLine("{0,-5} {1,-8} {2,-10} {3,-8} {4,-12} {5,-10}",
                                  "ردیف", "نتیجه", "تعداد حدس", "امتیاز", "زمان", "تاریخ");
                Console.WriteLine(new string('-', 60));

                for (int i = 0; i < gameHistory.Count; i++)
                {
                    var game = gameHistory[i];
                    string result = game.Won ? "برد" : "باخت";
                    string index = (i + 1).ToString();

                    Console.WriteLine("{0,-5} {1,-8} {2,-10} {3,-8} {4,-12} {5,-10}",
                                      index,
                                      result,
                                      game.Attempts,
                                      game.Score,
                                      game.Duration.ToString(@"mm\:ss"),
                                      game.PlayedDate.ToString("yyyy-MM-dd"));
                }

                Console.WriteLine();
                Console.Write("برای مشاهده جزئیات یک بازی، شماره آن را وارد کنید (0 برای خروج): ");

                if (int.TryParse(Console.ReadLine(), out int gameIndex) && gameIndex > 0 && gameIndex <= gameHistory.Count)
                {
                    var selectedGame = gameHistory[gameIndex - 1];
                    DisplayGameDetails(selectedGame);
                }
            }

            Console.WriteLine();
            Console.WriteLine("برای بازگشت به منوی اصلی، یک کلید را فشار دهید...");
            Console.ReadKey();
        }

        /// <summary>
        /// نمایش جزئیات یک بازی خاص
        /// </summary>
        private static void DisplayGameDetails(GameHistory game)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(SEPARATOR);
            Console.WriteLine("                 جزئیات بازی                  ");
            Console.WriteLine(SEPARATOR);
            Console.ResetColor();

            Console.WriteLine($"تاریخ: {game.PlayedDate:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"نتیجه: {(game.Won ? "برد" : "باخت")}");
            Console.WriteLine($"عدد هدف: {game.TargetNumber}");
            Console.WriteLine($"تعداد حدس‌ها: {game.Attempts}");
            Console.WriteLine($"امتیاز: {game.Score}");
            Console.WriteLine($"زمان صرف شده: {game.Duration:mm\\:ss}");

            Console.WriteLine();
            Console.WriteLine("تاریخچه حدس‌ها:");
            for (int i = 0; i < game.GuessHistory.Count; i++)
            {
                int guess = game.GuessHistory[i];
                string indicator = guess == game.TargetNumber ? " ✓" :
                                  guess < game.TargetNumber ? " ↑" : " ↓";

                Console.WriteLine($"  حدس {i + 1}: {guess} {indicator}");
            }

            Console.WriteLine();
            Console.WriteLine("برای بازگشت، یک کلید را فشار دهید...");
            Console.ReadKey();
        }

        /// <summary>
        /// نمایش راهنمای بازی
        /// </summary>
        private static void DisplayHelp()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(SEPARATOR);
            Console.WriteLine("                     راهنما                       ");
            Console.WriteLine(SEPARATOR);
            Console.ResetColor();

            Console.WriteLine("هدف بازی:");
            Console.WriteLine("  - شما باید عددی که کامپیوتر انتخاب کرده را در کمترین حدس ممکن پیدا کنید.");
            Console.WriteLine();

            Console.WriteLine("دستورات ویژه:");
            Console.WriteLine("  - quit یا exit: خروج از بازی جاری");
            Console.WriteLine("  - reveal: نمایش عدد هدف (فقط برای تست)");
            Console.WriteLine();

            Console.WriteLine("امتیازدهی:");
            Console.WriteLine("  - امتیاز اولیه: 1000");
            Console.WriteLine("  - به ازای هر حدس: 100 امتیاز کسر می‌شود");
            Console.WriteLine("  - پاداش سرعت: اگر سریعتر بازی را تمام کنید امتیاز بیشتری می‌گیرید");
            Console.WriteLine("  - پاداش دقت: اگر در حدس‌های کمتری برنده شوید امتیاز بیشتری می‌گیرید");
            Console.WriteLine();

            Console.WriteLine("راهکارها:");
            Console.WriteLine("  - از روش تقسیم محدوده استفاده کنید (مثلاً همیشه وسط محدوده را حدس بزنید)");
            Console.WriteLine("  - به راهنمایی‌های '↑' و '↓' توجه کنید");
            Console.WriteLine("  - محدوده ممکن پس از هر حدس به روز می‌شود");
            Console.WriteLine();

            Console.WriteLine("برای بازگشت به منوی اصلی، یک کلید را فشار دهید...");
            Console.ReadKey();
        }

        /// <summary>
        /// نمایش صفحه خروج
        /// </summary>
        private static void DisplayExitScreen()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(SEPARATOR);
            Console.WriteLine("           بازیکردن با شما لذت بخش بود!          ");
            Console.WriteLine(SEPARATOR);
            Console.ResetColor();

            if (gamesPlayed > 0)
            {
                double winRate = (double)gamesWon / gamesPlayed * 100;
                Console.WriteLine($"تعداد بازی‌های انجام شده: {gamesPlayed}");
                Console.WriteLine($"نرخ برد: {winRate:F2}%");
                Console.WriteLine($"بالاترین امتیاز: {gameHistory.Max(g => g.Score)}");
            }

            Console.WriteLine("\nخداحافظ! به امید دیدار مجدد...");
            Thread.Sleep(2000);
        }

        /// <summary>
        /// دریافت ورودی از کاربر با نمایش prompt
        /// </summary>
        private static string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine()?.Trim();
        }

        /// <summary>
        /// نمایش پیام با رنگ خاص
        /// </summary>
        private static void DisplayMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }

    /// <summary>
    /// کلاس برای ذخیره تاریخچه هر بازی
    /// </summary>
    public class GameHistory
    {
        public int TargetNumber { get; set; }
        public int Attempts { get; set; }
        public bool Won { get; set; }
        public int Score { get; set; }
        public DateTime PlayedDate { get; set; }
        public TimeSpan Duration { get; set; }
        public List<int> GuessHistory { get; set; }
    }
}