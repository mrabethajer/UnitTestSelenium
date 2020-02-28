using System;
using System.IO;
using System.Net;
using System.Threading;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Linq;

namespace UnitTestSelenium
{
    [TestFixture]
    public class UnitTest1
    {
        public IWebDriver driver = null;
        public ExtentReports extent;
        ExtentTest test = null;

        [OneTimeSetUp]
        public void extentStart()
        {

            extent = new ExtentReports();
            var dir = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", "");
            DirectoryInfo di = Directory.CreateDirectory(dir + "\\Test_Execution_Reports");
            var htmlReporter = new ExtentHtmlReporter(dir + "\\Test_Execution_Reports" + "\\Automation_Report" + ".html");

            extent.AttachReporter(htmlReporter);
            string hostname = Dns.GetHostName();
            OperatingSystem system = Environment.OSVersion;

            extent.AddSystemInfo("Operating System", system.ToString());
            extent.AddSystemInfo("hostname", hostname.ToString());
            extent.AddSystemInfo("Browser", "Google Chrome");
        }
        public static string capture(IWebDriver driver, string screenshotname)
        {
            ITakesScreenshot ts = (ITakesScreenshot)driver;
            Screenshot screenshot = ts.GetScreenshot();
            string path = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
            string uptobinpath = path.Substring(0, path.LastIndexOf("bin")) + "ScreenShots\\" + screenshotname + ".png";
            string localpath = new Uri(uptobinpath).LocalPath;
            screenshot.SaveAsFile(localpath, ScreenshotImageFormat.Png);
            return localpath;

        }


        [OneTimeTearDown]
        public void exentClose()
        {
            extent.Flush();
        }


        [TestCase]
        public void TestMethod1()
        {
            try
            {
                JSONReader jsonRead = new JSONReader();
                test = extent.CreateTest("Test1").Info("test started");
                var opts = new DesiredCapabilities();
                opts.SetCapability("app", @"C:\Users\hamrabet\Desktop\75.4.0\Linedata Longview\Build_64800_Linedata_LongView_Smart_Client_AUD\Linedata Longview\75.4.0.64800\LongView.exe");

                test.Log(Status.Info, "LongView started");
                // logger.Info("Test started");
                driver = new RemoteWebDriver(new Uri("http://localhost:9999"), opts);

                Thread.Sleep(3000);
                driver.FindElement(By.Id("8640")).SendKeys(jsonRead.jsonReader("../UnitTestProject/Data/data.json", "username"));
                IWebElement password = driver.FindElement(By.Id("8646"));
                password.SendKeys(jsonRead.jsonReader("../UnitTestProject/Data/data.json", "password"));

                Thread.Sleep(1000);

                driver.FindElement(By.Name("OK")).Click();
                Thread.Sleep(40000);

                driver.FindElement(By.Id("59419")).FindElement(By.ClassName("XTPToolBar")).Click();
                driver.FindElement(By.Name("File")).Click();
                driver.FindElement(By.Name("Open...	Ctrl+O")).Click();
                Thread.Sleep(3000);
                driver.FindElement(By.Name("Appraisal")).Click();
                driver.FindElement(By.Id("22675")).Click();
                driver.FindElement(By.Id("8605")).SendKeys("US1");
                driver.FindElement(By.Id("1")).Click();
                Thread.Sleep(3000);
                driver.FindElement(By.Id("59648")).Click();
                driver.FindElement(By.Name("File")).Click();
                driver.FindElement(By.Name("Export")).Click();
                driver.FindElement(By.Name("Report To Excel")).Click();
                Thread.Sleep(40000);

                var directory = new DirectoryInfo(@"C:/Users/hamrabet/Documents/");
                var myFile = (from f in directory.GetFiles()
                              orderby f.LastWriteTime descending
                              select f).First();

                ExcelUtil.PopulateInCollection(myFile.FullName);

                NUnit.Framework.Assert.AreEqual("100,00000000", ExcelUtil.ReadData(16, "Model Percent"));
                //Read data from Excel Sheet
                NUnit.Framework.Assert.AreEqual("100", ExcelUtil.ReadData(13, "Model Percent"));

                Console.WriteLine(ExcelUtil.ReadData(16, "Model Percent"));
                test.Log(Status.Pass, "test passed");


            }
            catch (Exception e)
            {
                string screenpth = capture(driver, "screenShot");
                test.Log(Status.Fail, e.ToString());

                test.AddScreenCaptureFromPath(screenpth);
                //logger.Info("Test failed");
            }
            finally
            {
                if (driver != null)
                { ///driver.Quit();
                }
            }
        }
    }
}
