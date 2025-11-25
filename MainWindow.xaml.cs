using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace A1Z26Cipher
{
    public partial class MainWindow : Window
    {
        // русский алфавит
        private const string RUSSIAN_ALPHABET = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        // шифрование
        private void BtnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string inputText = txtInput.Text.ToUpper().Trim();
                string key = txtKey.Text.Trim();

                if (string.IsNullOrEmpty(inputText))
                {
                    MessageBox.Show("Введите текст для шифрования", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(key))
                {
                    MessageBox.Show("Сгенерируйте или введите ключ", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string encryptedText = EncryptA1Z26(inputText, key);
                txtOutput.Text = encryptedText;
                txtEncryptedData.Text = encryptedText;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при шифровании: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string encryptedText = txtEncryptedData.Text.Trim();
                string key = txtKey.Text.Trim();

                if (string.IsNullOrEmpty(encryptedText))
                {
                    MessageBox.Show("Введите зашифрованные данные", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(key))
                {
                    MessageBox.Show("Сгенерируйте или введите ключ", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string decryptedText = DecryptA1Z26(encryptedText, key);
                txtOutput.Text = decryptedText;
                txtInput.Text = decryptedText;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при дешифровании: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // генерация ключа
        private void BtnGenerateKey_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int shift = 0;
                if (int.TryParse(txtShift.Text, out shift))
                {
                    string generatedKey = GenerateKey(shift);
                    txtKey.Text = generatedKey;
                }
                else
                {
                    MessageBox.Show("Введите корректное число для сдвига", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при генерации ключа: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // очистка полей
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            txtInput.Text = "";
            txtOutput.Text = "";
            txtEncryptedData.Text = "";
            txtKey.Text = "";
            txtShift.Text = "0";
        }

        // метод шифрования A1Z26
        private string EncryptA1Z26(string text, string key)
        {
            StringBuilder result = new StringBuilder();
            Dictionary<char, int> charToNumberMap = CreateCharToNumberMap(key);

            foreach (char c in text)
            {
                if (charToNumberMap.ContainsKey(c))
                {
                    result.Append(charToNumberMap[c] + "-");
                }
                else if (c == ' ')
                {
                    result.Append(" ");
                }
                else
                {
                    result.Append(c);
                }
            }

            // убираем последний дефис если он есть
            if (result.Length > 0 && result[result.Length - 1] == '-')
                result.Length--;

            return result.ToString();
        }

        // метод дешифрования A1Z26
        private string DecryptA1Z26(string encryptedText, string key)
        {
            StringBuilder result = new StringBuilder();
            Dictionary<int, char> numberToCharMap = CreateNumberToCharMap(key);

            // разделяем на слова (сохраняем пробелы)
            string[] words = encryptedText.Split(' ');

            foreach (string word in words)
            {
                if (string.IsNullOrWhiteSpace(word))
                {
                    result.Append(" ");
                    continue;
                }

                // разделяем на числа
                string[] numberStrings = word.Split('-');
                foreach (string numberStr in numberStrings)
                {
                    if (int.TryParse(numberStr, out int number))
                    {
                        if (numberToCharMap.ContainsKey(number))
                        {
                            result.Append(numberToCharMap[number]);
                        }
                        else
                        {
                            result.Append($"[{number}]");
                        }
                    }
                    else
                    {
                        result.Append(numberStr);
                    }
                }
                result.Append(" ");
            }

            return result.ToString().Trim();
        }

        // создание карты символов в числа
        private Dictionary<char, int> CreateCharToNumberMap(string key)
        {
            var map = new Dictionary<char, int>();
            string alphabet = ApplyKeyToAlphabet(key);

            for (int i = 0; i < alphabet.Length; i++)
            {
                map[alphabet[i]] = i + 1;
            }

            return map;
        }

        // Создание карты чисел в символы
        private Dictionary<int, char> CreateNumberToCharMap(string key)
        {
            var map = new Dictionary<int, char>();
            string alphabet = ApplyKeyToAlphabet(key);

            for (int i = 0; i < alphabet.Length; i++)
            {
                map[i + 1] = alphabet[i];
            }

            return map;
        }

        // применение ключа к алфавиту
        private string ApplyKeyToAlphabet(string key)
        {
            if (string.IsNullOrEmpty(key))
                return RUSSIAN_ALPHABET;

            // удаляем дубликаты из ключа и преобразуем в верхний регистр
            string uniqueKey = new string(key.ToUpper()
                .Where(c => RUSSIAN_ALPHABET.Contains(c))
                .Distinct()
                .ToArray());

            // добавляем оставшиеся буквы алфавита
            string remainingAlphabet = new string(RUSSIAN_ALPHABET
                .Where(c => !uniqueKey.Contains(c))
                .ToArray());

            return uniqueKey + remainingAlphabet;
        }

        // генерируем ключ
        private string GenerateKey(int shift)
        {
            if (shift == 0)
            {
                // случайная перестановка алфавита
                char[] alphabetArray = RUSSIAN_ALPHABET.ToCharArray();
                for (int i = alphabetArray.Length - 1; i > 0; i--)
                {
                    int j = random.Next(i + 1);
                    char temp = alphabetArray[i];
                    alphabetArray[i] = alphabetArray[j];
                    alphabetArray[j] = temp;
                }
                return new string(alphabetArray);
            }
            else
            {
                shift = shift % RUSSIAN_ALPHABET.Length;
                if (shift < 0)
                    shift += RUSSIAN_ALPHABET.Length;

                return RUSSIAN_ALPHABET.Substring(shift) + RUSSIAN_ALPHABET.Substring(0, shift);
            }
        }
    }
}