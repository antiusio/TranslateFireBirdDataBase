using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Translate
{
    public class Translator
    {
        IWebDriver driver;
        public Translator()
        {
            StartBrowser();
            driver.Url = "https://translate.google.com/#view=home&op=translate&sl=ru&tl=uk&text=";
        }
        [SetUp]
        private void StartBrowser()
        {
            driver = new ChromeDriver();
        }
        [Test]
        public void test()
        {
            driver.Url = "http://www.google.co.in";
        }

        [TearDown]
        public void CloseBrowser()
        {
            driver.Close();
            driver.Dispose();
        }
        public string Translate(string str)
        {
            var el=driver.FindElement(By.Id("source"));
            el.SendKeys(str);
            string rez = "";
            //add time limit, reload page if limit expires
            while(rez.Equals(""))
            {
                try { rez = driver.FindElement(By.XPath("//span[@lang='uk']")).Text; }
                catch { }
            }
            el.Clear();
            string s = "a";
            while (!s.Equals(""))
            {
                try { s = driver.FindElement(By.XPath("//span[@lang='uk']")).Text; }
                catch { break; }
            }
            //var rez = driver.FindElement(By.XPath("//span[@lang='uk']"));

            return rez;
        }
    }
}
