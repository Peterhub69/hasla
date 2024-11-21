using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Text.RegularExpressions;
using System.IO;

namespace hasla
{
    public partial class MainWindow : Window
    {
        private static readonly char[] dozwoloneZnakiSpecjalne = { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')' };
        private static readonly char[] zabronioneZnakiSpecjalne = { '<', '>', '{', '}', '[', ']', '\\', '/', '|', '`' };
        private static string[] commonPasswords;

        public MainWindow()
        {
            InitializeComponent();
            LoadCommonPasswords();
        }

        private void LoadCommonPasswords()
        {
            try
            {
                commonPasswords = File.ReadAllLines("C:\\Users\\user\\Desktop\\wordlist_pl.txt");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd w wczytywaniu pliku: " + ex.Message);
            }
        }

        private void CheckPasswordStrength_Click(object sender, RoutedEventArgs e)
        {
            string password1 = passwordBox1.Password;
            string password2 = passwordBox2.Password;

            if (password1 != password2)
            {
                passwordStrengthText.Text = "Hasła nie są zgodne.";
                return;
            }

            if (IsPasswordStrong(password1))
            {
                passwordStrengthText.Text = "Hasło jest silne.";
            }
            else
            {
                passwordStrengthText.Text = "Hasło jest słabe.";
            }
        }

        private bool IsPasswordStrong(string password)
        {
            if (password.Length < 8 || password.Length > 20) return false;
            if (password.Length >= 12) return true;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => dozwoloneZnakiSpecjalne.Contains(ch));
            bool hasForbiddenSpecial = password.Any(ch => zabronioneZnakiSpecjalne.Contains(ch));

            if (hasForbiddenSpecial) return false;
            if (commonPasswords.Contains(password)) return false;

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        private void GeneratePassword_Click(object sender, RoutedEventArgs e)
        {
            string password = GenerateStrongPassword();
            generatedPasswordText.Text = "Wygenerowane hasło: " + password;
        }

        private string GenerateStrongPassword()
        {
            Random random = new Random();
            StringBuilder password = new StringBuilder();
            password.Append((char)random.Next(65, 91)); 
            password.Append((char)random.Next(97, 123)); 
            password.Append((char)random.Next(48, 58)); 
            password.Append(dozwoloneZnakiSpecjalne[random.Next(dozwoloneZnakiSpecjalne.Length)]);

            for (int i = 4; i < 12; i++)
            {
                char nextChar = (char)random.Next(33, 126);
                if (!char.IsLetterOrDigit(nextChar) && !dozwoloneZnakiSpecjalne.Contains(nextChar)) continue;
                password.Append(nextChar);
            }

            return new string(password.ToString().OrderBy(c => random.Next()).ToArray());
        }
    }
}

