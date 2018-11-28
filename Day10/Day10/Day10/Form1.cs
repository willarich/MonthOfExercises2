using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Day10
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TranslateSentence(textBox1.Text);
        }

        private string TranslateSentence(string sentence)
        {

            string[] words = sentence.Split();
            for (int i = 0; i < words.Length; i++)
            {
                Console.WriteLine(words[i]);
                words[i] = TranslateWord(words[i]);
            }
            label2.Text = "";
            for (int i = 0; i < words.Length; i++)
            {
                label2.Text += words[i] + " ";
            }
                //try
                //{
                //    Regex re = new Regex(@"^[(\w+)\s?]+$");
                //    Console.WriteLine("Test2");
                //    //if (re.IsMatch(sentence))
                //    Match m = re.Match(sentence);
                //    int failSafe = 0;
                //    while (m.Success)
                //    {

                //        Console.WriteLine("Test3 " + re.Match(sentence).Groups.Count);
                //        for (int i = 0; i <= re.Match(sentence).Groups.Count; i++)
                //        {
                //            Console.WriteLine(re.Match(sentence).Groups[i]);
                //        }
                //        Console.WriteLine(re.Match(sentence).Groups);
                //        m = m.NextMatch();
                //        failSafe += 1;
                //        if (failSafe >= 30)
                //        {
                //            break;
                //        }
                //    }

                //}

                //catch (Exception exception)
                //{
                //    Console.WriteLine(exception.Message);

                //}
                return "";
        }

        private string TranslateWord(string word)
        {
            try
            {
                Regex startsWConst = new Regex(@"^([^aeiouAEIOU]+)(\w+)$");
                Regex startsWVowel = new Regex(@"[aeiouAEIOU]+\w*");
                if (startsWConst.IsMatch(word))
                {
                    Match m = startsWConst.Match(word);
                    string plWord = m.Groups[2].ToString() + m.Groups[1].ToString() + "ay";
                    return plWord;
                }
                if (startsWVowel.IsMatch(word))
                {
                    Match m = startsWConst.Match(word);
                    string plWord = word + "yay";
                    return plWord;
                }

            }
            
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);

            }
            return "";
        }
    }
}
