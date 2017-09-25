using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using OpenQA.Selenium;
using System.IO;
using CsvHelper;

namespace _4U_Best_Deal
{
    public partial class UC_Webconnect : UserControl
    {
        string WEBSITE = "http://webconnect.groupnews.com.au/login.aspx?ReturnUrl=%2f";
        string CATEGORYPATH = "webconnect.groupnews.com.txt";
        string SAVEPATH = "webconnect.groupnews.com.csv";
        string SETTINGPATH = "setting.db";
        ThreadStart DataThread;
        Thread DateThread_Thread;
        IWebDriver navigator;

        public UC_Webconnect()
        {
            InitializeComponent();
        }

        private void Btn_Start_Click(object sender, EventArgs e)
        {
            Btn_Start.Enabled = false;
            Btn_Stop.Enabled = true;
            DataThread = new ThreadStart(Start);
            DateThread_Thread = new Thread(DataThread);
            DateThread_Thread.Start();
        }

        private void Btn_Stop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void Stop()
        {
            Btn_Start.Enabled = true;
            Btn_Stop.Enabled = false;
            try { navigator.Quit(); } catch (Exception) { }
            try { DateThread_Thread.Abort(); } catch (Exception) { }
            try
            {
                label3.Text = "";
            }
            catch (Exception)
            {
            }
        }

        private void Start()
        {
            navigator = new CWebBrowser().googleChrome();
            WebLogin(navigator);
            string text = File.ReadAllText(CATEGORYPATH);
            var category = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            int point = int.Parse(File.ReadAllText(SETTINGPATH));
            for (int i = point; i < category.Length; i++)
            {
                File.WriteAllText(SETTINGPATH, i.ToString());
                navigator.Navigate().GoToUrl(category[i]);
                Thread.Sleep(500);
                string ca = navigator.FindElement(By.XPath("//span[@class='breadcrumb']")).Text;
                GetInformation(navigator, ca);                
            }
        }

        private void WebLogin(IWebDriver driver)
        {
            try
            {
                driver.Navigate().GoToUrl(WEBSITE);
                Thread.Sleep(1000);
                driver.FindElement(By.Id("ctl00_contentMain_defaultLogin_loginMain_UserName")).SendKeys(textBox_Username.Text);
                Thread.Sleep(500);
                driver.FindElement(By.Id("ctl00_contentMain_defaultLogin_loginMain_Password")).SendKeys(textBox_Password.Text);
                Thread.Sleep(500);
                driver.FindElement(By.Id("ctl00_contentMain_defaultLogin_loginMain_LoginButton")).Click();
                Thread.Sleep(500);
            }
            catch (Exception)
            {
            }
        }

        // ID, Code, Description, supplierCode, additionalInfo, imageURL, Unit, BulkPrice, 
        // StandardPrice, stockNSW, stockQLD, stockVIC, stockWA, barcode, barcodeInner, brand, categories
        private void GetInformation(IWebDriver driver, string ca)
        {
            NextPage:
            string firstID = "";
            try
            {
                firstID = driver.FindElement(By.XPath("//span[@class='textWhite']")).Text;
            }
            catch (Exception)
            {
                return;
            }
            var more = driver.FindElements(By.XPath("//a"));
            List<string> moreURL = new List<string>();
            foreach (var item in more)
            {
                try
                {
                    if (item.Text == "More Information")
                        moreURL.Add(item.GetAttribute("href"));
                }
                catch (Exception)
                {
                }
            }
            for (int i = 0; i < moreURL.Count; i ++)
            {
                try
                {
                    List<string> list = new List<string>();
                    string ID = driver.FindElement(By.XPath("(//span[@class='textWhite'])[" + (i * 3 + 1).ToString() + "]")).Text;
                    if (CheckID(ID))
                    {
                        string Description = driver.FindElement(By.XPath("(//span[@class='textWhite'])[" + (i * 3 + 3).ToString() + "]")).Text;
                        try
                        {
                            label3.Text = Description;
                        }
                        catch (Exception)
                        {
                        }
                        new CWebBrowser().newTab(driver, moreURL[i]);
                        Thread.Sleep(500);
                        list.Add(ID);  // ID
                        list.Add(driver.FindElement(By.Id("ctl00_contentMain_lblCode")).Text); // Code
                        list.Add(Description.Replace(",", ".")); // Description
                        list.Add(driver.FindElement(By.Id("ctl00_contentMain_lblSupplierCode")).Text); // supplierCode
                        string info = "";
                        try { info = driver.FindElement(By.Id("ctl00_contentMain_LoginView2_lblAdditionalDescription1")).Text; } catch (Exception) { }
                        list.Add(info.Replace(",", ".")); // additionalInfo
                        list.Add(driver.FindElement(By.Id("ctl00_contentMain_imgProduct")).GetAttribute("href")); // imageURL
                        list.Add(driver.FindElement(By.Id("ctl00_contentMain_lblUnit")).Text); // Unit
                        list.Add(driver.FindElement(By.Id("ctl00_contentMain_lblPickupPrice")).Text.Split(' ')[0]); // BulkPrice
                        list.Add(driver.FindElement(By.Id("ctl00_contentMain_lblDeliveredPrice")).Text.Split(' ')[0]); // StandardPrice
                        list.Add(driver.FindElement(By.Id("ctl00_contentMain_lblRealtimeStockNSW")).Text.Split(' ')[0]); // stockNSW
                        list.Add(driver.FindElement(By.Id("ctl00_contentMain_lblRealtimeStockQLD")).Text.Split(' ')[0]); // stockQLD
                        list.Add(driver.FindElement(By.Id("ctl00_contentMain_lblRealtimeStockVIC")).Text.Split(' ')[0]); // stockVIC
                        list.Add(driver.FindElement(By.Id("ctl00_contentMain_lblRealtimeStockWA")).Text.Split(' ')[0]);  // stockWA

                        list.Add(driver.FindElement(By.Id("ctl00_contentMain_lblBarcode")).Text); // barcode
                        list.Add(driver.FindElement(By.Id("ctl00_contentMain_lblbarcodeInner")).Text); // barcodeInner
                        list.Add(driver.FindElement(By.Id("ctl00_contentMain_lblBrand")).Text); // brand
                        list.Add(ca); // categories

                        //                         var csv = new StringBuilder();
                        //                         var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}",
                        //                             list[0], list[1], list[2], list[3], list[4], list[5], list[6], list[7], list[8], list[9], list[10], list[11], list[12], list[13], list[14], list[15], list[16]);
                        //                         csv.AppendLine(newLine);
                        //                         File.AppendAllText(SAVEPATH, csv.ToString());


                        List<Employee> empList = new List<Employee>();

                        using (StreamWriter sw = new StreamWriter(SAVEPATH))
                        using (CsvWriter cw = new CsvWriter(sw))
                        {
                            cw.WriteHeader<Employee>();

                            foreach (Employee emp in empList)
                            {
                                emp.Wage *= 1.1F;
                                cw.WriteRecord<Employee>(emp);
                            }
                        }

                        new CWebBrowser().closeBrowser(driver);
                        Thread.Sleep(500);
                    }
                }
                catch (Exception)
                {
                }
            }

            try
            {
                Thread.Sleep(1000);
                var button = driver.FindElements(By.XPath("//a[@class='pagination']"));
                foreach (var item in button)
                {
                    if (item.Text.Contains("Next"))
                    {
                        item.Click();
                        Thread.Sleep(1000);
                        break;
                    }                     
                }                
                if (firstID != driver.FindElement(By.XPath("//span[@class='textWhite']")).Text)
                  goto NextPage;
            }
            catch (Exception)
            {
            }
        }
        private bool CheckID(string ID)
        {
            string text = File.ReadAllText(SAVEPATH);
            if (text.Contains(ID)) return false;
            else return true;
        }
    }
}
