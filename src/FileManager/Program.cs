using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdvancedFileManager
{
    class Program
    {
        // مسیر جاری در فایل منیجر
        private static string currentDirectory = Directory.GetCurrentDirectory();

        // تاریخچه دستورات برای قابلیت تکرار دستورات
        private static List<string> commandHistory = new List<string>();

        // نمادهای نمایشی برای رابط کاربری
        private const string INDICATOR = "> ";
        private const string SEPARATOR = "---------------------------------------------------";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            DisplayWelcomeMessage();

            // حلقه اصلی برنامه
            while (true)
            {
                try
                {
                    DisplayCurrentDirectory();
                    string command = GetUserInput();

                    if (string.IsNullOrWhiteSpace(command))
                        continue;

                    commandHistory.Add(command);
                    ProcessCommand(command);
                }
                catch (Exception ex)
                {
                    DisplayError($"خطا: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// نمایش پیغام خوشآمدگویی
        /// </summary>
        private static void DisplayWelcomeMessage()
        {
            Console.WriteLine(SEPARATOR);
            Console.WriteLine("       Advanced Console File Manager       ");
            Console.WriteLine("            Version 2.0 - 2023            ");
            Console.WriteLine(SEPARATOR);
            Console.WriteLine("برای مشاهده دستورات موجود از 'help' استفاده کنید");
            Console.WriteLine();
        }

        /// <summary>
        /// نمایش مسیر جاری
        /// </summary>
        private static void DisplayCurrentDirectory()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{currentDirectory}{INDICATOR}");
            Console.ResetColor();
        }

        /// <summary>
        /// دریافت ورودی از کاربر
        /// </summary>
        private static string GetUserInput()
        {
            return Console.ReadLine()?.Trim();
        }

        /// <summary>
        /// پردازش دستور وارد شده توسط کاربر
        /// </summary>
        private static void ProcessCommand(string command)
        {
            string[] parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string action = parts[0].ToLower();

            switch (action)
            {
                case "cd":
                    ChangeDirectory(parts.Length > 1 ? parts[1] : "");
                    break;
                case "dir":
                case "ls":
                    ListDirectoryContents(parts.Length > 1 ? parts[1] : "");
                    break;
                case "mkdir":
                    if (parts.Length > 1) CreateDirectory(parts[1]);
                    else DisplayError("نام دایرکتوری را مشخص کنید");
                    break;
                case "rmdir":
                    if (parts.Length > 1) DeleteDirectory(parts[1]);
                    else DisplayError("نام دایرکتوری را مشخص کنید");
                    break;
                case "type":
                case "cat":
                    if (parts.Length > 1) DisplayFileContent(parts[1]);
                    else DisplayError("نام فایل را مشخص کنید");
                    break;
                case "copy":
                    if (parts.Length > 2) CopyFile(parts[1], parts[2]);
                    else DisplayError("مقصد و منبع را مشخص کنید");
                    break;
                case "move":
                    if (parts.Length > 2) MoveFile(parts[1], parts[2]);
                    else DisplayError("مقصد و منبع را مشخص کنید");
                    break;
                case "del":
                case "rm":
                    if (parts.Length > 1) DeleteFile(parts[1]);
                    else DisplayError("نام فایل را مشخص کنید");
                    break;
                case "rename":
                    if (parts.Length > 2) RenameFile(parts[1], parts[2]);
                    else DisplayError("نام جدید و قدیم را مشخص کنید");
                    break;
                case "find":
                    if (parts.Length > 2) FindInFiles(parts[1], parts[2]);
                    else DisplayError("الگو و متن جستجو را مشخص کنید");
                    break;
                case "size":
                    if (parts.Length > 1) ShowFileSize(parts[1]);
                    else DisplayError("نام فایل یا دایرکتوری را مشخص کنید");
                    break;
                case "history":
                    ShowCommandHistory();
                    break;
                case "clear":
                    Console.Clear();
                    break;
                case "help":
                    ShowHelp();
                    break;
                case "exit":
                    Environment.Exit(0);
                    break;
                default:
                    DisplayError($"دستور ناشناخته: {action}");
                    break;
            }
        }

        /// <summary>
        /// تغییر دایرکتوری جاری
        /// </summary>
        private static void ChangeDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                // نمایش دایرکتوری جاری اگر مسیری مشخص نشده
                Console.WriteLine(currentDirectory);
                return;
            }

            if (path == "..")
            {
                // بازگشت به دایرکتوری والد
                currentDirectory = Directory.GetParent(currentDirectory)?.FullName ?? currentDirectory;
                return;
            }

            if (path == ".")
            {
                // ماندن در دایرکتوری جاری
                return;
            }

            string newPath = Path.Combine(currentDirectory, path);

            if (Directory.Exists(newPath))
            {
                currentDirectory = Path.GetFullPath(newPath);
            }
            else
            {
                DisplayError($"دایرکتوری '{newPath}' یافت نشد");
            }
        }

        /// <summary>
        /// نمایش محتوای دایرکتوری
        /// </summary>
        private static void ListDirectoryContents(string path)
        {
            string targetDir = string.IsNullOrEmpty(path) ? currentDirectory : Path.Combine(currentDirectory, path);

            if (!Directory.Exists(targetDir))
            {
                DisplayError($"دایرکتوری '{targetDir}' یافت نشد");
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"محتوای دایرکتوری: {targetDir}");
            Console.WriteLine(SEPARATOR);

            // نمایش دایرکتوری‌ها
            Console.ForegroundColor = ConsoleColor.Blue;
            foreach (var dir in Directory.GetDirectories(targetDir))
            {
                var dirInfo = new DirectoryInfo(dir);
                Console.WriteLine($"{dirInfo.LastWriteTime:yyyy-MM-dd HH:mm}    <DIR>          {dirInfo.Name}");
            }
            Console.ResetColor();

            // نمایش فایل‌ها
            foreach (var file in Directory.GetFiles(targetDir))
            {
                var fileInfo = new FileInfo(file);
                Console.WriteLine($"{fileInfo.LastWriteTime:yyyy-MM-dd HH:mm}    {fileInfo.Length,15:N0} {fileInfo.Name}");
            }

            Console.WriteLine(SEPARATOR);

            // نمایش آمار
            int dirCount = Directory.GetDirectories(targetDir).Length;
            int fileCount = Directory.GetFiles(targetDir).Length;
            long totalSize = Directory.GetFiles(targetDir).Sum(f => new FileInfo(f).Length);

            Console.WriteLine($"{dirCount} دایرکتوری، {fileCount} فایل، حجم کل: {FormatFileSize(totalSize)}");
            Console.WriteLine();
        }

        /// <summary>
        /// ایجاد دایرکتوری جدید
        /// </summary>
        private static void CreateDirectory(string dirName)
        {
            string fullPath = Path.Combine(currentDirectory, dirName);

            if (Directory.Exists(fullPath))
            {
                DisplayError($"دایرکتوری '{dirName}' از قبل وجود دارد");
                return;
            }

            Directory.CreateDirectory(fullPath);
            Console.WriteLine($"دایرکتوری '{dirName}' ایجاد شد");
        }

        /// <summary>
        /// حذف دایرکتوری
        /// </summary>
        private static void DeleteDirectory(string dirName)
        {
            string fullPath = Path.Combine(currentDirectory, dirName);

            if (!Directory.Exists(fullPath))
            {
                DisplayError($"دایرکتوری '{dirName}' یافت نشد");
                return;
            }

            // بررسی خالی بودن دایرکتوری
            if (Directory.GetFiles(fullPath).Length > 0 || Directory.GetDirectories(fullPath).Length > 0)
            {
                Console.Write($"دایرکتوری '{dirName}' خالی نیست. آیا مطمئنید که می‌خواهید آن را حذف کنید؟ (y/n): ");
                string response = Console.ReadLine()?.ToLower();

                if (response != "y" && response != "yes")
                {
                    Console.WriteLine("عملیات لغو شد");
                    return;
                }
            }

            Directory.Delete(fullPath, true);
            Console.WriteLine($"دایرکتوری '{dirName}' حذف شد");
        }

        /// <summary>
        /// نمایش محتوای فایل
        /// </summary>
        private static void DisplayFileContent(string fileName)
        {
            string fullPath = Path.Combine(currentDirectory, fileName);

            if (!File.Exists(fullPath))
            {
                DisplayError($"فایل '{fileName}' یافت نشد");
                return;
            }

            try
            {
                Console.WriteLine();
                Console.WriteLine(SEPARATOR);
                Console.WriteLine($"محتوای فایل: {fileName}");
                Console.WriteLine(SEPARATOR);

                using (StreamReader reader = new StreamReader(fullPath))
                {
                    string line;
                    int lineNumber = 1;

                    while ((line = reader.ReadLine()) != null)
                    {
                        Console.WriteLine($"{lineNumber:0000}: {line}");
                        lineNumber++;

                        // توقف بعد از هر 20 خط برای امکان ادامه نمایش
                        if (lineNumber % 20 == 0)
                        {
                            Console.Write("-- ادامه -- (Enter برای ادامه، q برای خروج): ");
                            string response = Console.ReadLine();
                            if (response?.ToLower() == "q") break;
                        }
                    }
                }

                Console.WriteLine(SEPARATOR);
            }
            catch (Exception ex)
            {
                DisplayError($"خطا در خواندن فایل: {ex.Message}");
            }
        }

        /// <summary>
        /// کپی فایل
        /// </summary>
        private static void CopyFile(string source, string destination)
        {
            string sourcePath = Path.Combine(currentDirectory, source);
            string destPath = Path.Combine(currentDirectory, destination);

            if (!File.Exists(sourcePath))
            {
                DisplayError($"فایل مبدأ '{source}' یافت نشد");
                return;
            }

            // اگر مقصد یک دایرکتوری است، نام فایل را حفظ کن
            if (Directory.Exists(destPath))
            {
                destPath = Path.Combine(destPath, Path.GetFileName(source));
            }

            try
            {
                File.Copy(sourcePath, destPath, true);
                Console.WriteLine($"فایل '{source}' به '{destination}' کپی شد");
            }
            catch (Exception ex)
            {
                DisplayError($"خطا در کپی فایل: {ex.Message}");
            }
        }

        /// <summary>
        /// انتقال یا تغییر نام فایل
        /// </summary>
        private static void MoveFile(string source, string destination)
        {
            string sourcePath = Path.Combine(currentDirectory, source);
            string destPath = Path.Combine(currentDirectory, destination);

            if (!File.Exists(sourcePath))
            {
                DisplayError($"فایل مبدأ '{source}' یافت نشد");
                return;
            }

            try
            {
                File.Move(sourcePath, destPath);
                Console.WriteLine($"فایل '{source}' به '{destination}' انتقال یافت");
            }
            catch (Exception ex)
            {
                DisplayError($"خطا در انتقال فایل: {ex.Message}");
            }
        }

        /// <summary>
        /// حذف فایل
        /// </summary>
        private static void DeleteFile(string fileName)
        {
            string fullPath = Path.Combine(currentDirectory, fileName);

            if (!File.Exists(fullPath))
            {
                DisplayError($"فایل '{fileName}' یافت نشد");
                return;
            }

            try
            {
                File.Delete(fullPath);
                Console.WriteLine($"فایل '{fileName}' حذف شد");
            }
            catch (Exception ex)
            {
                DisplayError($"خطا در حذف فایل: {ex.Message}");
            }
        }

        /// <summary>
        /// تغییر نام فایل
        /// </summary>
        private static void RenameFile(string oldName, string newName)
        {
            string oldPath = Path.Combine(currentDirectory, oldName);
            string newPath = Path.Combine(currentDirectory, newName);

            if (!File.Exists(oldPath))
            {
                DisplayError($"فایل '{oldName}' یافت نشد");
                return;
            }

            if (File.Exists(newPath))
            {
                DisplayError($"فایل '{newName}' از قبل وجود دارد");
                return;
            }

            try
            {
                File.Move(oldPath, newPath);
                Console.WriteLine($"فایل '{oldName}' به '{newName}' تغییر نام یافت");
            }
            catch (Exception ex)
            {
                DisplayError($"خطا در تغییر نام فایل: {ex.Message}");
            }
        }

        /// <summary>
        /// جستجوی متن در فایل‌ها
        /// </summary>
        private static void FindInFiles(string searchPattern, string searchText)
        {
            string[] files;

            try
            {
                files = Directory.GetFiles(currentDirectory, searchPattern);
            }
            catch
            {
                DisplayError("الگوی جستجوی نامعتبر");
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"جستجوی '{searchText}' در فایل‌های {searchPattern}");
            Console.WriteLine(SEPARATOR);

            int totalMatches = 0;

            foreach (string file in files)
            {
                try
                {
                    string[] lines = File.ReadAllLines(file);
                    string fileName = Path.GetFileName(file);
                    int matchesInFile = 0;

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Contains(searchText, StringComparison.OrdinalIgnoreCase))
                        {
                            if (matchesInFile == 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"\nفایل: {fileName}");
                                Console.ResetColor();
                            }

                            Console.WriteLine($"{i + 1:0000}: {lines[i]}");
                            matchesInFile++;
                            totalMatches++;
                        }
                    }

                    if (matchesInFile > 0)
                    {
                        Console.WriteLine($"   {matchesInFile} مورد یافت شد در {fileName}");
                    }
                }
                catch
                {
                    // در صورت خطا در خواندن فایل، از آن عبور کن
                }
            }

            Console.WriteLine(SEPARATOR);
            Console.WriteLine($"تعداد کل موارد یافت شده: {totalMatches}");
            Console.WriteLine();
        }

        /// <summary>
        /// نمایش حجم فایل یا دایرکتوری
        /// </summary>
        private static void ShowFileSize(string path)
        {
            string fullPath = Path.Combine(currentDirectory, path);

            if (File.Exists(fullPath))
            {
                FileInfo fileInfo = new FileInfo(fullPath);
                Console.WriteLine($"حجم فایل '{path}': {FormatFileSize(fileInfo.Length)}");
            }
            else if (Directory.Exists(fullPath))
            {
                long size = CalculateDirectorySize(fullPath);
                Console.WriteLine($"حجم دایرکتوری '{path}': {FormatFileSize(size)}");
            }
            else
            {
                DisplayError($"فایل یا دایرکتوری '{path}' یافت نشد");
            }
        }

        /// <summary>
        /// محاسبه حجم دایرکتوری
        /// </summary>
        private static long CalculateDirectorySize(string directoryPath)
        {
            long size = 0;

            // افزودن حجم تمام فایل‌ها
            foreach (string file in Directory.GetFiles(directoryPath))
            {
                size += new FileInfo(file).Length;
            }

            // افزودن حجم تمام زیردایرکتوری‌ها
            foreach (string dir in Directory.GetDirectories(directoryPath))
            {
                size += CalculateDirectorySize(dir);
            }

            return size;
        }

        /// <summary>
        /// نمایش تاریخچه دستورات
        /// </summary>
        private static void ShowCommandHistory()
        {
            Console.WriteLine();
            Console.WriteLine("تاریخچه دستورات:");
            Console.WriteLine(SEPARATOR);

            for (int i = 0; i < commandHistory.Count; i++)
            {
                Console.WriteLine($"{i + 1:000}: {commandHistory[i]}");
            }

            Console.WriteLine(SEPARATOR);
            Console.WriteLine();
        }

        /// <summary>
        /// نمایش راهنمای دستورات
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine();
            Console.WriteLine("دستورات موجود:");
            Console.WriteLine(SEPARATOR);
            Console.WriteLine("cd [directory]     - تغییر دایرکتوری جاری");
            Console.WriteLine("dir [pattern]      - نمایش محتوای دایرکتوری");
            Console.WriteLine("ls [pattern]       - همان دستور dir");
            Console.WriteLine("mkdir <name>       - ایجاد دایرکتوری جدید");
            Console.WriteLine("rmdir <name>       - حذف دایرکتوری");
            Console.WriteLine("type <file>        - نمایش محتوای فایل");
            Console.WriteLine("cat <file>         - همان دستور type");
            Console.WriteLine("copy <src> <dest>  - کپی فایل");
            Console.WriteLine("move <src> <dest>  - انتقال فایل");
            Console.WriteLine("del <file>         - حذف فایل");
            Console.WriteLine("rm <file>          - همان دستور del");
            Console.WriteLine("rename <old> <new> - تغییر نام فایل");
            Console.WriteLine("find <pattern> <text> - جستجوی متن در فایل‌ها");
            Console.WriteLine("size <path>        - نمایش حجم فایل یا دایرکتوری");
            Console.WriteLine("history            - نمایش تاریخچه دستورات");
            Console.WriteLine("clear              - پاک کردن صفحه کنسول");
            Console.WriteLine("help               - نمایش این راهنما");
            Console.WriteLine("exit               - خروج از برنامه");
            Console.WriteLine(SEPARATOR);
            Console.WriteLine();
        }

        /// <summary>
        /// نمایش خطا با رنگ قرمز
        /// </summary>
        private static void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// فرمت کردن حجم فایل به صورت خوانا
        /// </summary>
        private static string FormatFileSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;

            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }

            return $"{number:n2} {suffixes[counter]}";
        }
    }
}