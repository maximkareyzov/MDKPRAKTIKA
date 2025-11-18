using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace VigenereCipher
{
    public partial class MainWindow : Window
    {
        private const string RUSSIAN_ALPHABET = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string originalText = OriginalTextTextBox.Text.Trim();
                string vigenereKey = VigenereKeyTextBox.Text.Trim();
                string caesarKey = CaesarKeyTextBox.Text.Trim();


                if (string.IsNullOrEmpty(originalText))
                {
                    MessageBox.Show("Введите текст для шифрования", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(vigenereKey))
                {
                    MessageBox.Show("Введите ключ для шифра Виженера", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(caesarKey) || !int.TryParse(caesarKey, out int caesarShift))
                {
                    MessageBox.Show("Для шифра Цезаря ключ должен быть числом", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                string afterVigenere = VigenereEncrypt(originalText, vigenereKey);
                AfterVigenereTextBox.Text = afterVigenere;


                string afterCaesar = CaesarEncrypt(afterVigenere, caesarShift);
                AfterCaesarTextBox.Text = afterCaesar;


                EncryptedTextTextBox.Text = afterCaesar;


                EncryptedForDecryptTextBox.Text = afterCaesar;
                DecryptVigenereKeyTextBox.Text = vigenereKey;
                DecryptCaesarKeyTextBox.Text = caesarKey;

                MessageBox.Show($"Текст успешно зашифрован двумя методами!\n\n" +
                              $"Длина исходного текста: {originalText.Length} символов\n" +
                              $"Длина зашифрованного текста: {afterCaesar.Length} символов",
                              "Успешное шифрование",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при шифровании: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string encryptedText = EncryptedForDecryptTextBox.Text.Trim();
                string vigenereKey = DecryptVigenereKeyTextBox.Text.Trim();
                string caesarKey = DecryptCaesarKeyTextBox.Text.Trim();


                if (string.IsNullOrEmpty(encryptedText))
                {
                    MessageBox.Show("Введите зашифрованный текст", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(vigenereKey))
                {
                    MessageBox.Show("Введите ключ для шифра Виженера", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(caesarKey) || !int.TryParse(caesarKey, out int caesarShift))
                {
                    MessageBox.Show("Для шифра Цезаря ключ должен быть числом", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                string afterCaesarDecrypt = CaesarDecrypt(encryptedText, caesarShift);
                AfterCaesarDecryptTextBox.Text = afterCaesarDecrypt;

                string afterVigenereDecrypt = VigenereDecrypt(afterCaesarDecrypt, vigenereKey);
                AfterVigenereDecryptTextBox.Text = afterVigenereDecrypt;


                DecryptedTextTextBox.Text = afterVigenereDecrypt;

                MessageBox.Show($"Текст успешно расшифрован!\n\n" +
                              $"Проверка: {(afterVigenereDecrypt.Equals(OriginalTextTextBox.Text.Trim()) ? "✓ Текст совпадает с исходным" : "⚠ Текст не совпадает с исходным")}",
                              "Успешное дешифрование",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при расшифровке: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private string VigenereEncrypt(string text, string key)
        {
            return VigenereProcess(text, key, true);
        }

        private string VigenereDecrypt(string text, string key)
        {
            return VigenereProcess(text, key, false);
        }

        private string CaesarEncrypt(string text, int shift)
        {
            return CaesarProcess(text, shift, true);
        }

        private string CaesarDecrypt(string text, int shift)
        {
            return CaesarProcess(text, shift, false);
        }

        private string VigenereProcess(string text, string key, bool encrypt)
        {
            StringBuilder result = new StringBuilder();
            int keyIndex = 0;

            string cleanKey = CleanText(key);
            if (string.IsNullOrEmpty(cleanKey))
            {
                throw new Exception("Ключ Виженера должен содержать русские буквы");
            }

            for (int i = 0; i < text.Length; i++)
            {
                char currentChar = text[i];
                char lowerChar = char.ToLower(currentChar);

                if (RUSSIAN_ALPHABET.Contains(lowerChar.ToString()))
                {
                    int textIndex = RUSSIAN_ALPHABET.IndexOf(lowerChar);
                    int keyCharIndex = RUSSIAN_ALPHABET.IndexOf(cleanKey[keyIndex % cleanKey.Length]);

                    int newIndex;
                    if (encrypt)
                    {
                        newIndex = (textIndex + keyCharIndex) % RUSSIAN_ALPHABET.Length;
                    }
                    else
                    {
                        newIndex = (textIndex - keyCharIndex + RUSSIAN_ALPHABET.Length) % RUSSIAN_ALPHABET.Length;
                    }

                    char newChar = RUSSIAN_ALPHABET[newIndex];

                    if (char.IsUpper(currentChar))
                        result.Append(char.ToUpper(newChar));
                    else
                        result.Append(newChar);

                    keyIndex++;
                }
                else
                {
                    result.Append(currentChar);
                }
            }

            return result.ToString();
        }

        private string CaesarProcess(string text, int shift, bool encrypt)
        {
            StringBuilder result = new StringBuilder();

            if (!encrypt)
            {
                shift = -shift;
            }

            foreach (char currentChar in text)
            {
                char lowerChar = char.ToLower(currentChar);

                if (RUSSIAN_ALPHABET.Contains(lowerChar.ToString()))
                {
                    int textIndex = RUSSIAN_ALPHABET.IndexOf(lowerChar);
                    int newIndex = (textIndex + shift + RUSSIAN_ALPHABET.Length) % RUSSIAN_ALPHABET.Length;

                    char newChar = RUSSIAN_ALPHABET[newIndex];

                    if (char.IsUpper(currentChar))
                        result.Append(char.ToUpper(newChar));
                    else
                        result.Append(newChar);
                }
                else
                {
                    result.Append(currentChar);
                }
            }

            return result.ToString();
        }

        private string CleanText(string text)
        {
            StringBuilder clean = new StringBuilder();
            foreach (char c in text.ToLower())
            {
                if (RUSSIAN_ALPHABET.Contains(c.ToString()))
                {
                    clean.Append(c);
                }
            }
            return clean.ToString();
        }


        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {

            OriginalTextTextBox.Clear();
            VigenereKeyTextBox.Clear();
            CaesarKeyTextBox.Clear();
            AfterVigenereTextBox.Clear();
            AfterCaesarTextBox.Clear();
            EncryptedTextTextBox.Clear();
            EncryptedForDecryptTextBox.Clear();
            DecryptVigenereKeyTextBox.Clear();
            DecryptCaesarKeyTextBox.Clear();
            AfterCaesarDecryptTextBox.Clear();
            AfterVigenereDecryptTextBox.Clear();
            DecryptedTextTextBox.Clear();
        }

        private void CopyEncryptedButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(EncryptedTextTextBox.Text))
            {
                Clipboard.SetText(EncryptedTextTextBox.Text);
                MessageBox.Show("Зашифрованный текст скопирован в буфер обмена!", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Нет зашифрованного текста для копирования", "Информация",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void PasteForDecryptButton_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText();
                if (!string.IsNullOrEmpty(clipboardText))
                {
                    EncryptedForDecryptTextBox.Text = clipboardText;
                    MessageBox.Show("Текст из буфера обмена вставлен!", "Успех",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Буфер обмена пуст", "Информация",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

  


        private void CaesarKeyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(CaesarKeyTextBox.Text) && !int.TryParse(CaesarKeyTextBox.Text, out _))
            {
                CaesarKeyTextBox.Background = System.Windows.Media.Brushes.LightPink;
            }
            else
            {
                CaesarKeyTextBox.Background = System.Windows.Media.Brushes.White;
            }
        }

        private void DecryptCaesarKeyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(DecryptCaesarKeyTextBox.Text) && !int.TryParse(DecryptCaesarKeyTextBox.Text, out _))
            {
                DecryptCaesarKeyTextBox.Background = System.Windows.Media.Brushes.LightPink;
            }
            else
            {
                DecryptCaesarKeyTextBox.Background = System.Windows.Media.Brushes.White;
            }
        }
    }
}