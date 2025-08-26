using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ContactManager
{
    /// <summary>
    /// کلاس اصلی برنامه مدیریت مخاطبین
    /// این برنامه امکان مدیریت کامل مخاطبان را فراهم می‌کند
    /// </summary>
    class Program
    {
        // لیست مخاطبین
        private static List<Contact> contacts = new List<Contact>();

        // مسیر فایل ذخیره سازی
        private const string DataFilePath = "contacts.json";

        static void Main(string[] args)
        {
            Console.Title = "Advanced Contact Manager"; // تنظیم عنوان کنسول
            Console.OutputEncoding = System.Text.Encoding.UTF8; // پشتیبانی از کاراکترهای خاص

            LoadContacts(); // بارگذاری مخاطبین از فایل

            DisplayWelcomeMessage(); // نمایش پیغام خوشآمدگویی

            bool continueRunning = true;

            while (continueRunning)
            {
                try
                {
                    DisplayMainMenu(); // نمایش منوی اصلی

                    string choice = GetUserInput("Enter your choice (1-10): ").Trim();

                    // پردازش انتخاب کاربر
                    switch (choice)
                    {
                        case "1":
                            AddNewContact();
                            break;
                        case "2":
                            ViewAllContacts();
                            break;
                        case "3":
                            SearchContacts();
                            break;
                        case "4":
                            EditContact();
                            break;
                        case "5":
                            DeleteContact();
                            break;
                        case "6":
                            ExportContacts();
                            break;
                        case "7":
                            ImportContacts();
                            break;
                        case "8":
                            ViewContactStatistics();
                            break;
                        case "9":
                            BackupContacts();
                            break;
                        case "10":
                            continueRunning = false;
                            SaveContacts(); // ذخیره مخاطبین قبل از خروج
                            Console.WriteLine("\nThank you for using Contact Manager!");
                            Console.WriteLine("Goodbye! 👋");
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    Console.WriteLine("Please try again.");
                }

                if (continueRunning)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        /// <summary>
        /// نمایش پیغام خوشآمدگویی به کاربر
        /// </summary>
        static void DisplayWelcomeMessage()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===============================================");
            Console.WriteLine("         ADVANCED CONTACT MANAGER             ");
            Console.WriteLine("===============================================");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// نمایش منوی اصلی برنامه
        /// </summary>
        static void DisplayMainMenu()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("MAIN MENU:");
            Console.ResetColor();

            Console.WriteLine("1. Add New Contact");
            Console.WriteLine("2. View All Contacts");
            Console.WriteLine("3. Search Contacts");
            Console.WriteLine("4. Edit Contact");
            Console.WriteLine("5. Delete Contact");
            Console.WriteLine("6. Export Contacts to CSV");
            Console.WriteLine("7. Import Contacts from CSV");
            Console.WriteLine("8. View Statistics");
            Console.WriteLine("9. Backup Contacts");
            Console.WriteLine("10. Exit");
            Console.WriteLine();
        }

        /// <summary>
        /// اضافه کردن مخاطب جدید
        /// </summary>
        static void AddNewContact()
        {
            Console.WriteLine("\n--- Add New Contact ---");

            Contact newContact = new Contact();

            // دریافت اطلاعات مخاطب
            newContact.FirstName = GetValidatedInput("First Name: ", true, "name");
            newContact.LastName = GetValidatedInput("Last Name: ", false, "name");
            newContact.PhoneNumber = GetValidatedInput("Phone Number: ", true, "phone");
            newContact.Email = GetValidatedInput("Email: ", false, "email");
            newContact.Address = GetValidatedInput("Address: ", false, "text");
            newContact.Company = GetValidatedInput("Company: ", false, "text");
            newContact.JobTitle = GetValidatedInput("Job Title: ", false, "text");
            newContact.Birthday = GetValidatedInput("Birthday (YYYY-MM-DD): ", false, "date");
            newContact.Notes = GetValidatedInput("Notes: ", false, "text");

            // گروه‌ها
            string groupsInput = GetValidatedInput("Groups (comma-separated): ", false, "text");
            if (!string.IsNullOrEmpty(groupsInput))
            {
                newContact.Groups = groupsInput.Split(',')
                    .Select(g => g.Trim())
                    .Where(g => !string.IsNullOrEmpty(g))
                    .ToList();
            }

            // تگ‌ها
            string tagsInput = GetValidatedInput("Tags (comma-separated): ", false, "text");
            if (!string.IsNullOrEmpty(tagsInput))
            {
                newContact.Tags = tagsInput.Split(',')
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t))
                    .ToList();
            }

            // اضافه کردن به لیست
            newContact.Id = contacts.Any() ? contacts.Max(c => c.Id) + 1 : 1;
            newContact.CreatedDate = DateTime.Now;
            newContact.LastModified = DateTime.Now;

            contacts.Add(newContact);
            SaveContacts();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nContact '{newContact.FirstName} {newContact.LastName}' added successfully! ✅");
            Console.ResetColor();
        }

        /// <summary>
        /// مشاهده تمام مخاطبین
        /// </summary>
        static void ViewAllContacts()
        {
            Console.WriteLine("\n--- All Contacts ---");

            if (!contacts.Any())
            {
                Console.WriteLine("No contacts found.");
                return;
            }

            // گزینه‌های مرتب‌سازی
            Console.WriteLine("\nSort by:");
            Console.WriteLine("1. First Name (A-Z)");
            Console.WriteLine("2. Last Name (A-Z)");
            Console.WriteLine("3. Company");
            Console.WriteLine("4. Recently Added");
            Console.WriteLine("5. Recently Modified");

            string sortChoice = GetUserInput("Choose sorting option (1-5, default 1): ").Trim();

            IEnumerable<Contact> sortedContacts = contacts;

            switch (sortChoice)
            {
                case "2":
                    sortedContacts = contacts.OrderBy(c => c.LastName);
                    break;
                case "3":
                    sortedContacts = contacts.OrderBy(c => c.Company);
                    break;
                case "4":
                    sortedContacts = contacts.OrderByDescending(c => c.CreatedDate);
                    break;
                case "5":
                    sortedContacts = contacts.OrderByDescending(c => c.LastModified);
                    break;
                default:
                    sortedContacts = contacts.OrderBy(c => c.FirstName);
                    break;
            }

            // نمایش مخاطبین
            int contactNumber = 1;
            foreach (var contact in sortedContacts)
            {
                DisplayContactSummary(contact, contactNumber);
                contactNumber++;
            }

            Console.WriteLine($"\nTotal contacts: {contacts.Count}");
        }

        /// <summary>
        /// جستجو در مخاطبین
        /// </summary>
        static void SearchContacts()
        {
            Console.WriteLine("\n--- Search Contacts ---");

            if (!contacts.Any())
            {
                Console.WriteLine("No contacts to search.");
                return;
            }

            Console.WriteLine("Search by:");
            Console.WriteLine("1. Name");
            Console.WriteLine("2. Phone Number");
            Console.WriteLine("3. Email");
            Console.WriteLine("4. Company");
            Console.WriteLine("5. Group");
            Console.WriteLine("6. Tag");

            string searchType = GetUserInput("Choose search type (1-6): ").Trim();
            string searchTerm = GetUserInput("Enter search term: ").Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                Console.WriteLine("Search term cannot be empty.");
                return;
            }

            IEnumerable<Contact> searchResults = Enumerable.Empty<Contact>();

            switch (searchType)
            {
                case "1":
                    searchResults = contacts.Where(c =>
                        (c.FirstName + " " + c.LastName).Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                    break;
                case "2":
                    searchResults = contacts.Where(c =>
                        c.PhoneNumber.Contains(searchTerm));
                    break;
                case "3":
                    searchResults = contacts.Where(c =>
                        c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                    break;
                case "4":
                    searchResults = contacts.Where(c =>
                        c.Company.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                    break;
                case "5":
                    searchResults = contacts.Where(c =>
                        c.Groups.Any(g => g.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
                    break;
                case "6":
                    searchResults = contacts.Where(c =>
                        c.Tags.Any(t => t.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
                    break;
                default:
                    Console.WriteLine("Invalid search type.");
                    return;
            }

            // نمایش نتایج
            Console.WriteLine($"\nSearch Results for '{searchTerm}':");

            if (!searchResults.Any())
            {
                Console.WriteLine("No contacts found.");
                return;
            }

            int resultNumber = 1;
            foreach (var contact in searchResults)
            {
                DisplayContactSummary(contact, resultNumber);
                resultNumber++;
            }

            Console.WriteLine($"\nFound {searchResults.Count()} contact(s).");
        }

        /// <summary>
        /// ویرایش مخاطب
        /// </summary>
        static void EditContact()
        {
            Console.WriteLine("\n--- Edit Contact ---");

            if (!contacts.Any())
            {
                Console.WriteLine("No contacts to edit.");
                return;
            }

            int contactId = GetValidContactId();
            if (contactId == -1) return;

            Contact contactToEdit = contacts.First(c => c.Id == contactId);

            Console.WriteLine($"\nEditing contact: {contactToEdit.FirstName} {contactToEdit.LastName}");
            Console.WriteLine("Leave field empty to keep current value.");

            // دریافت اطلاعات جدید
            string newFirstName = GetValidatedInput($"First Name ({contactToEdit.FirstName}): ", false, "name");
            if (!string.IsNullOrEmpty(newFirstName)) contactToEdit.FirstName = newFirstName;

            string newLastName = GetValidatedInput($"Last Name ({contactToEdit.LastName}): ", false, "name");
            if (!string.IsNullOrEmpty(newLastName)) contactToEdit.LastName = newLastName;

            string newPhone = GetValidatedInput($"Phone Number ({contactToEdit.PhoneNumber}): ", false, "phone");
            if (!string.IsNullOrEmpty(newPhone)) contactToEdit.PhoneNumber = newPhone;

            string newEmail = GetValidatedInput($"Email ({contactToEdit.Email}): ", false, "email");
            if (!string.IsNullOrEmpty(newEmail)) contactToEdit.Email = newEmail;

            string newAddress = GetValidatedInput($"Address ({contactToEdit.Address}): ", false, "text");
            if (!string.IsNullOrEmpty(newAddress)) contactToEdit.Address = newAddress;

            string newCompany = GetValidatedInput($"Company ({contactToEdit.Company}): ", false, "text");
            if (!string.IsNullOrEmpty(newCompany)) contactToEdit.Company = newCompany;

            string newJobTitle = GetValidatedInput($"Job Title ({contactToEdit.JobTitle}): ", false, "text");
            if (!string.IsNullOrEmpty(newJobTitle)) contactToEdit.JobTitle = newJobTitle;

            string newBirthday = GetValidatedInput($"Birthday ({contactToEdit.Birthday}): ", false, "date");
            if (!string.IsNullOrEmpty(newBirthday)) contactToEdit.Birthday = newBirthday;

            string newNotes = GetValidatedInput($"Notes ({contactToEdit.Notes}): ", false, "text");
            if (!string.IsNullOrEmpty(newNotes)) contactToEdit.Notes = newNotes;

            // به روز رسانی تاریخ修改
            contactToEdit.LastModified = DateTime.Now;

            SaveContacts();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nContact updated successfully! ✅");
            Console.ResetColor();
        }

        /// <summary>
        /// حذف مخاطب
        /// </summary>
        static void DeleteContact()
        {
            Console.WriteLine("\n--- Delete Contact ---");

            if (!contacts.Any())
            {
                Console.WriteLine("No contacts to delete.");
                return;
            }

            int contactId = GetValidContactId();
            if (contactId == -1) return;

            Contact contactToDelete = contacts.First(c => c.Id == contactId);

            // نمایش اطلاعات مخاطب برای تأیید
            DisplayContactDetails(contactToDelete);

            string confirmation = GetUserInput($"\nAre you sure you want to delete this contact? (y/N): ").Trim().ToLower();

            if (confirmation == "y" || confirmation == "yes")
            {
                contacts.Remove(contactToDelete);
                SaveContacts();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Contact deleted successfully! ✅");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("Deletion cancelled.");
            }
        }

        /// <summary>
        /// صدور مخاطبین به فایل CSV
        /// </summary>
        static void ExportContacts()
        {
            Console.WriteLine("\n--- Export Contacts ---");

            if (!contacts.Any())
            {
                Console.WriteLine("No contacts to export.");
                return;
            }

            string fileName = GetUserInput("Enter export file name (without extension): ").Trim();
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "contacts_export";
            }

            fileName += ".csv";

            try
            {
                using (var writer = new StreamWriter(fileName))
                {
                    // نوشتن هدر
                    writer.WriteLine("ID,FirstName,LastName,PhoneNumber,Email,Address,Company,JobTitle,Birthday,Notes,Groups,Tags,CreatedDate,LastModified");

                    // نوشتن داده‌ها
                    foreach (var contact in contacts)
                    {
                        string groups = string.Join(";", contact.Groups);
                        string tags = string.Join(";", contact.Tags);

                        writer.WriteLine($"\"{contact.Id}\",\"{contact.FirstName}\",\"{contact.LastName}\",\"{contact.PhoneNumber}\",\"{contact.Email}\",\"{contact.Address}\",\"{contact.Company}\",\"{contact.JobTitle}\",\"{contact.Birthday}\",\"{contact.Notes}\",\"{groups}\",\"{tags}\",\"{contact.CreatedDate}\",\"{contact.LastModified}\"");
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Contacts exported successfully to {fileName}! ✅");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting contacts: {ex.Message}");
            }
        }

        /// <summary>
        /// وارد کردن مخاطبین از فایل CSV
        /// </summary>
        static void ImportContacts()
        {
            Console.WriteLine("\n--- Import Contacts ---");

            string fileName = GetUserInput("Enter CSV file name to import: ").Trim();

            if (!File.Exists(fileName))
            {
                Console.WriteLine("File not found.");
                return;
            }

            try
            {
                int importedCount = 0;
                int skippedCount = 0;

                using (var reader = new StreamReader(fileName))
                {
                    // رد کردن هدر
                    string header = reader.ReadLine();

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] values = ParseCsvLine(line);

                        if (values.Length >= 5) // حداقل فیلدهای ضروری
                        {
                            Contact newContact = new Contact();

                            newContact.Id = contacts.Any() ? contacts.Max(c => c.Id) + 1 : 1;
                            newContact.FirstName = values[1];
                            newContact.LastName = values[2];
                            newContact.PhoneNumber = values[3];
                            newContact.Email = values[4];
                            newContact.Address = values.Length > 5 ? values[5] : "";
                            newContact.Company = values.Length > 6 ? values[6] : "";
                            newContact.JobTitle = values.Length > 7 ? values[7] : "";
                            newContact.Birthday = values.Length > 8 ? values[8] : "";
                            newContact.Notes = values.Length > 9 ? values[9] : "";

                            if (values.Length > 10 && !string.IsNullOrEmpty(values[10]))
                                newContact.Groups = values[10].Split(';').ToList();

                            if (values.Length > 11 && !string.IsNullOrEmpty(values[11]))
                                newContact.Tags = values[11].Split(';').ToList();

                            newContact.CreatedDate = DateTime.Now;
                            newContact.LastModified = DateTime.Now;

                            // بررسی تکراری نبودن
                            if (!contacts.Any(c => c.PhoneNumber == newContact.PhoneNumber ||
                                                  (!string.IsNullOrEmpty(newContact.Email) && c.Email == newContact.Email)))
                            {
                                contacts.Add(newContact);
                                importedCount++;
                            }
                            else
                            {
                                skippedCount++;
                            }
                        }
                    }
                }

                SaveContacts();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Import completed! Imported: {importedCount}, Skipped (duplicates): {skippedCount} ✅");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing contacts: {ex.Message}");
            }
        }

        /// <summary>
        /// مشاهده آمار مخاطبین
        /// </summary>
        static void ViewContactStatistics()
        {
            Console.WriteLine("\n--- Contact Statistics ---");

            if (!contacts.Any())
            {
                Console.WriteLine("No contacts available for statistics.");
                return;
            }

            Console.WriteLine($"Total Contacts: {contacts.Count}");
            Console.WriteLine($"Contacts with Email: {contacts.Count(c => !string.IsNullOrEmpty(c.Email))}");
            Console.WriteLine($"Contacts with Company: {contacts.Count(c => !string.IsNullOrEmpty(c.Company))}");

            // گروه‌ها
            var groupStats = contacts
                .SelectMany(c => c.Groups)
                .GroupBy(g => g)
                .OrderByDescending(g => g.Count());

            Console.WriteLine("\nGroups:");
            foreach (var group in groupStats)
            {
                Console.WriteLine($"  {group.Key}: {group.Count()} contacts");
            }

            // تگ‌ها
            var tagStats = contacts
                .SelectMany(c => c.Tags)
                .GroupBy(t => t)
                .OrderByDescending(t => t.Count());

            Console.WriteLine("\nTags:");
            foreach (var tag in tagStats.Take(10)) // نمایش 10 تگ برتر
            {
                Console.WriteLine($"  {tag.Key}: {tag.Count()} contacts");
            }

            // تاریخ ایجاد
            var recentContacts = contacts
                .OrderByDescending(c => c.CreatedDate)
                .Take(5);

            Console.WriteLine("\nRecently Added Contacts:");
            foreach (var contact in recentContacts)
            {
                Console.WriteLine($"  {contact.FirstName} {contact.LastName} - {contact.CreatedDate:yyyy-MM-dd}");
            }
        }

        /// <summary>
        /// پشتیبان‌گیری از مخاطبین
        /// </summary>
        static void BackupContacts()
        {
            Console.WriteLine("\n--- Backup Contacts ---");

            string backupFileName = $"contacts_backup_{DateTime.Now:yyyyMMdd_HHmmss}.json";

            try
            {
                File.Copy(DataFilePath, backupFileName);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Backup created successfully: {backupFileName} ✅");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating backup: {ex.Message}");
            }
        }

        /// <summary>
        /// بارگذاری مخاطبین از فایل
        /// </summary>
        static void LoadContacts()
        {
            try
            {
                if (File.Exists(DataFilePath))
                {
                    string json = File.ReadAllText(DataFilePath);
                    contacts = JsonSerializer.Deserialize<List<Contact>>(json) ?? new List<Contact>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading contacts: {ex.Message}");
                contacts = new List<Contact>();
            }
        }

        /// <summary>
        /// ذخیره مخاطبین در فایل
        /// </summary>
        static void SaveContacts()
        {
            try
            {
                string json = JsonSerializer.Serialize(contacts, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(DataFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving contacts: {ex.Message}");
            }
        }

        /// <summary>
        /// دریافت یک شناسه معتبر از کاربر
        /// </summary>
        /// <returns>شناسه مخاطب یا -1 در صورت لغو</returns>
        static int GetValidContactId()
        {
            ViewAllContacts();

            string input = GetUserInput("\nEnter contact ID: ").Trim();

            if (int.TryParse(input, out int contactId) && contacts.Any(c => c.Id == contactId))
            {
                return contactId;
            }

            Console.WriteLine("Invalid contact ID.");
            return -1;
        }

        /// <summary>
        /// نمایش خلاصه مخاطب
        /// </summary>
        static void DisplayContactSummary(Contact contact, int number)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{number}. ");
            Console.ResetColor();

            Console.Write($"{contact.FirstName} {contact.LastName}");

            if (!string.IsNullOrEmpty(contact.Company))
            {
                Console.Write($" - {contact.Company}");
            }

            Console.WriteLine($" - {contact.PhoneNumber}");
        }

        /// <summary>
        /// نمایش جزئیات کامل مخاطب
        /// </summary>
        static void DisplayContactDetails(Contact contact)
        {
            Console.WriteLine("\n" + new string('=', 40));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("CONTACT DETAILS");
            Console.ResetColor();
            Console.WriteLine(new string('=', 40));

            Console.WriteLine($"ID: {contact.Id}");
            Console.WriteLine($"Name: {contact.FirstName} {contact.LastName}");
            Console.WriteLine($"Phone: {contact.PhoneNumber}");
            Console.WriteLine($"Email: {contact.Email}");
            Console.WriteLine($"Address: {contact.Address}");
            Console.WriteLine($"Company: {contact.Company}");
            Console.WriteLine($"Job Title: {contact.JobTitle}");
            Console.WriteLine($"Birthday: {contact.Birthday}");

            if (contact.Groups.Any())
            {
                Console.WriteLine($"Groups: {string.Join(", ", contact.Groups)}");
            }

            if (contact.Tags.Any())
            {
                Console.WriteLine($"Tags: {string.Join(", ", contact.Tags)}");
            }

            Console.WriteLine($"Notes: {contact.Notes}");
            Console.WriteLine($"Created: {contact.CreatedDate:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"Last Modified: {contact.LastModified:yyyy-MM-dd HH:mm}");
            Console.WriteLine(new string('=', 40));
        }

        /// <summary>
        /// دریافت ورودی با اعتبارسنجی
        /// </summary>
        static string GetValidatedInput(string prompt, bool isRequired, string validationType)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim() ?? "";

                if (isRequired && string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("This field is required.");
                    continue;
                }

                if (!string.IsNullOrEmpty(input))
                {
                    // اعتبارسنجی بر اساس نوع
                    switch (validationType)
                    {
                        case "name":
                            if (!Regex.IsMatch(input, @"^[a-zA-Z\s\-']+$"))
                            {
                                Console.WriteLine("Invalid name format. Use only letters, spaces, hyphens, and apostrophes.");
                                continue;
                            }
                            break;

                        case "phone":
                            if (!Regex.IsMatch(input, @"^[\d\s\-\+\(\)]+$"))
                            {
                                Console.WriteLine("Invalid phone number format.");
                                continue;
                            }
                            break;

                        case "email":
                            if (!Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                            {
                                Console.WriteLine("Invalid email format.");
                                continue;
                            }
                            break;

                        case "date":
                            if (!Regex.IsMatch(input, @"^\d{4}-\d{2}-\d{2}$") || !DateTime.TryParse(input, out _))
                            {
                                Console.WriteLine("Invalid date format. Use YYYY-MM-DD.");
                                continue;
                            }
                            break;
                    }
                }

                return input;
            }
        }

        /// <summary>
        /// تجزیه خط CSV
        /// </summary>
        static string[] ParseCsvLine(string line)
        {
            var values = new List<string>();
            bool inQuotes = false;
            string currentValue = "";

            foreach (char c in line)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(currentValue);
                    currentValue = "";
                }
                else
                {
                    currentValue += c;
                }
            }

            values.Add(currentValue);
            return values.ToArray();
        }

        /// <summary>
        /// دریافت ورودی از کاربر
        /// </summary>
        static string GetUserInput(string message)
        {
            Console.Write(message);
            return Console.ReadLine() ?? "";
        }
    }

    /// <summary>
    /// کلاس مدل مخاطب
    /// شامل تمام ویژگی‌های یک مخاطب
    /// </summary>
    public class Contact
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
        public string Company { get; set; } = "";
        public string JobTitle { get; set; } = "";
        public string Birthday { get; set; } = "";
        public string Notes { get; set; } = "";
        public List<string> Groups { get; set; } = new List<string>();
        public List<string> Tags { get; set; } = new List<string>();
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
    }
}