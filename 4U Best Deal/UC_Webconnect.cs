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
using System.Net.Mail;

namespace _4U_Best_Deal
{
    public partial class UC_Webconnect : UserControl
    {
        // ID, Code, Description, supplierCode, additionalInfo, imageURL, Unit, BulkPrice, 
        // StandardPrice, stockNSW, stockQLD, stockVIC, stockWA, barcode, barcodeInner, brand, categories
        class Employee
        {
            public string ID { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public string supplierCode { get; set; }
            public string additionalInfo { get; set; }
            public string imageURL { get; set; }
            public string Unit { get; set; }
            public string BulkPrice { get; set; }
            public string StandardPrice { get; set; }
            public string stockNSW { get; set; }
            public string stockQLD { get; set; }
            public string stockVIC { get; set; }
            public string stockWA { get; set; }
            public string barcode { get; set; }
            public string barcodeInner { get; set; }
            public string brand { get; set; }
            public string categories { get; set; }
        }



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
        int total = 0;
        private void Start()
        {
            total = 0;
            navigator = new CWebBrowser().googleChrome();
            WebLogin(navigator);
            string text = File.ReadAllText(CATEGORYPATH);
            var category = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            int point = int.Parse(File.ReadAllText(SETTINGPATH));
            navigator.Manage().Window.Position = new Point(-2000, 0);
            for (int i = point; i < category.Length; i++)
            {
                File.WriteAllText(SETTINGPATH, i.ToString());
                navigator.Navigate().GoToUrl(category[i]);
                Thread.Sleep(500);
                string ca = navigator.FindElement(By.XPath("//span[@class='breadcrumb']")).Text;
                int count = GetInformation(navigator, ca);

                File.AppendAllText("count.txt", category[i] + "      " + count.ToString() + Environment.NewLine);
            }
            File.AppendAllText("count.txt", "   Total count = " + total.ToString());
            Stop();
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
        private int GetInformation(IWebDriver driver, string ca)
        {
            int count = 0;
            NextPage:
            string firstID = "";
            try
            {
                firstID = driver.FindElement(By.XPath("//span[@class='textWhite']")).Text;
            }
            catch (Exception)
            {
                // return;
            }
            
            List<string> moreURL = new List<string>();
            try
            {
                var more = driver.FindElements(By.XPath("//a"));
                foreach (var item in more)
                {
                    if (item.Text == "More Information")
                    {
                        moreURL.Add(item.GetAttribute("href"));
                        count++;
                        total++;
                        try
                        {
                            label4.Text = total.ToString();
                        }
                        catch (Exception)
                        {

                        }
                    }                        
                }
            }
            catch (Exception)
            {
            }
            //for (int i = 0; i < moreURL.Count; i ++)
            //{
            //    try
            //    {
            //        Employee emp = new Employee();
            //        string ID = driver.FindElement(By.XPath("(//span[@class='textWhite'])[" + (i * 3 + 1).ToString() + "]")).Text;
            //        if (CheckID(ID))
            //        {
            //            string Description = driver.FindElement(By.XPath("(//span[@class='textWhite'])[" + (i * 3 + 3).ToString() + "]")).Text;
            //            try
            //            {
            //                label3.Text = Description;
            //            }
            //            catch (Exception)
            //            {
            //            }
            //            new CWebBrowser().newTab(driver, moreURL[i]);
            //            Thread.Sleep(1000);
            //            emp.ID = ID;  // ID
            //            emp.Code = driver.FindElement(By.Id("ctl00_contentMain_lblCode")).Text; // Code
            //            emp.Description = Description; // Description
            //            emp.supplierCode = driver.FindElement(By.Id("ctl00_contentMain_lblSupplierCode")).Text; // supplierCode
            //            string info = "";
            //            try { info = driver.FindElement(By.Id("ctl00_contentMain_LoginView2_lblAdditionalDescription1")).Text; } catch (Exception) { }
            //            emp.additionalInfo = info; // additionalInfo
            //            emp.imageURL = driver.FindElement(By.Id("ctl00_contentMain_imgProduct")).GetAttribute("href"); // imageURL
            //            emp.Unit = driver.FindElement(By.Id("ctl00_contentMain_lblUnit")).Text; // Unit
            //            emp.BulkPrice = driver.FindElement(By.Id("ctl00_contentMain_lblPickupPrice")).Text.Split(' ')[0]; // BulkPrice
            //            emp.StandardPrice = driver.FindElement(By.Id("ctl00_contentMain_lblDeliveredPrice")).Text.Split(' ')[0]; // StandardPrice
            //            emp.stockNSW = driver.FindElement(By.Id("ctl00_contentMain_lblRealtimeStockNSW")).Text.Split(' ')[0]; // stockNSW
            //            emp.stockQLD = driver.FindElement(By.Id("ctl00_contentMain_lblRealtimeStockQLD")).Text.Split(' ')[0]; // stockQLD
            //            emp.stockVIC = driver.FindElement(By.Id("ctl00_contentMain_lblRealtimeStockVIC")).Text.Split(' ')[0]; // stockVIC
            //            emp.stockWA = driver.FindElement(By.Id("ctl00_contentMain_lblRealtimeStockWA")).Text.Split(' ')[0];  // stockWA

            //            emp.barcode = driver.FindElement(By.Id("ctl00_contentMain_lblBarcode")).Text; // barcode
            //            emp.barcodeInner = driver.FindElement(By.Id("ctl00_contentMain_lblbarcodeInner")).Text; // barcodeInner
            //            emp.brand = driver.FindElement(By.Id("ctl00_contentMain_lblBrand")).Text; // brand
            //            emp.categories = ca; // categories

            //            List<Employee> list = new List<Employee>();
            //            try
            //            {
            //                using (var textReader = File.OpenText(SAVEPATH))
            //                {
            //                    var csv = new CsvReader(textReader);
            //                    while (csv.Read())
            //                    {
            //                        var record = csv.GetRecord<Employee>();
            //                        list.Add(record);
            //                    }
            //                    textReader.Close();
            //                }
            //            }
            //            catch (Exception)
            //            {
            //            }
            //            list.Add(emp);
            //            using (StreamWriter sw = new StreamWriter(SAVEPATH))
            //            using (CsvWriter cw = new CsvWriter(sw))
            //            {
            //                cw.WriteHeader<Employee>();
            //                foreach (Employee item in list)
            //                {
            //                    cw.WriteRecord<Employee>(item);
            //                }
            //            }
            //            new CWebBrowser().closeBrowser(driver);
            //            Thread.Sleep(500);
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Console.Write(e.ToString());
            //    }
            //}

            try
            {
                Thread.Sleep(1000);
                new CWebBrowser().pageDown(driver);
                Thread.Sleep(1000);
                var button = driver.FindElements(By.XPath("//a[@class='pagination']"));
                foreach (var item in button)
                {
                    if (item.Text.Contains("Next"))
                    {
                        item.Click();
                        Thread.Sleep(3000);
                        break;
                    }                     
                }                
                if (firstID != driver.FindElement(By.XPath("//span[@class='textWhite']")).Text)
                  goto NextPage;
            }
            catch (Exception)
            {
            }
            return count;
        }
        private bool CheckID(string ID)
        {
            try
            {
                string text = File.ReadAllText(SAVEPATH);
                if (text.Contains(ID)) return false;
                else return true;
            }
            catch (Exception)
            {
                return true; ;
            }
        }
    }
}
