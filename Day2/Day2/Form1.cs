using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;


namespace Day2
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String webScrapeURL = textBox1.Text;
            if (webScrapeURL == "" || webScrapeURL == null)
            {
                label1.Text = "Please enter a URL in the text box above";
            }
            else
            {
                try
                {
                    label1.Text = "";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
webScrapeURL);
                    request.Method = "GET";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        Console.WriteLine("Reading from page " + webScrapeURL);
                        string result = reader.ReadToEnd();
                        Console.WriteLine(result);
                        Console.WriteLine("Finished reading from " + webScrapeURL);
                        //for (int i = 0; i < result.Length; i++)
                        //{
                        //    label1.Text = label1.Text + result.Substring(i, 1);
                        //    if (i >= 1000)
                        //    {
                        //        break;
                        //    }
                        //}
                        //label1.Text = result.Substring(0, 1000);
                        label1.Text = "Success. Length: " + result.Length + ", result: ";
                        //label1.Text = result;
                        label1.Refresh();
                        label1.Text += result;
                        Console.WriteLine("Finished writing " + webScrapeURL);
                        //textBox2.Text = result.Substring(0, 1000);
                    }
                }
                catch (System.Net.WebException)
                {
                    label1.Text = System.DateTime.Now + ": Error with URL";
                    //textBox2.Text = System.DateTime.Now + ": Error with URL";
                }
                catch (System.UriFormatException)
                {
                    label1.Text = "Please be sure that what you have entered in the text box is a properly formatted, complete URL";
                    //textBox2.Text = "Please be sure that what you have entered in the text box is a properly formatted, complete URL";

                }
                catch (Exception)
                {
                    label1.Text = "There was an error that I don't know/don't care how to fix";
                    //textBox2.Text = "There was an error that I don't know/don't care how to fix";

                }


            }
            

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
