using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ControlObjects;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace TableControlObjectDemoTests
{
    /// <summary>
    /// Tests to verify the Table Control object against the W3C site.
    /// </summary>
    [TestFixture]
    public class TableTests
    {
        protected IWebDriver WebDriver;

        [SetUp]
        public void Setup()
        {
            WebDriver = new FirefoxDriver();
            WebDriver.Navigate().GoToUrl("http://www.thefriendlytester.co.uk/2012/12/table-control-object.html");
            WebDriver.Manage().Window.Maximize();
        }

        [TearDown]
        public void TearDown()
        {
            WebDriver.Dispose();
        }

        [Test]
        public void VerifyColumnHeaders()
        {
            List<string> ExpectedColumnHeaders = new List<string>();

            ExpectedColumnHeaders.Add("Company");
            ExpectedColumnHeaders.Add("Contact");
            ExpectedColumnHeaders.Add("Country");

            Table w3cTable = new Table(WebDriver.FindElement(By.Id("customers")));

            Assert.AreEqual(ExpectedColumnHeaders, w3cTable.ReadAllColumnHeaders());
        }

        [Test]
        public void VerifyColumnCount()
        {
            Table w3cTable = new Table(WebDriver.FindElement(By.Id("customers")));

            Assert.AreEqual(3, w3cTable.ColumnCount());
        }

        /// <summary>
        /// Note there is actually 12 rows, but 
        /// </summary>
        [Test]
        public void VerifyRowCount()
        {
            Table w3cTable = new Table(WebDriver.FindElement(By.Id("customers")));

            Assert.AreEqual(12, w3cTable.RowCount());
        }

        [Test]
        public void VerifyRowContentsUsingKnownColumnValue()
        {
            Table w3cTable = new Table(WebDriver.FindElement(By.Id("customers")));

            Assert.AreEqual("Alfreds Futterkiste Maria Anders Germany", w3cTable.FindRowMatchingColumnData("Company", "Alfreds Futterkiste").Text);
        }

        [Test]
        public void VerifyRowContentContainingKnownValue()
        {
            Table w3cTable = new Table(WebDriver.FindElement(By.Id("customers")));

            Assert.AreEqual("Laughing Bacchus Winecellars Yoshi Tannamuri Canada", w3cTable.FindFirstRowByKnownValue("Canada").Text);
        }

        [Test]
        public void DoesColumnContain()
        {
            Table w3cTable = new Table(WebDriver.FindElement(By.Id("customers")));

            Assert.IsTrue(w3cTable.IsValuePresentWithinColumn("Country", "Denmark"));
            Assert.IsFalse(w3cTable.IsValuePresentWithinColumn("Country", "Greece"));
        }

        [Test]
        public void FindCellByKnownColumnValue()
        {
            Table w3cTable = new Table(WebDriver.FindElement(By.Id("customers")));

            IWebElement matchedCell = w3cTable.FindCellByColumnAndKnownValue("Company", "North/South");

            //You probably wouldn't then compare text, as you used that to find it, but just showing it found element
            Assert.AreEqual("North/South", matchedCell.Text);
        }

        [Test]
        public void FindCellByRowAndColumnName()
        {
            Table w3cTable = new Table(WebDriver.FindElement(By.Id("customers")));

            //You could potentially already have the row element, perhaps by find it a different way, but using a nother table methond here to get a row
            IWebElement requiredRow = w3cTable.FindRowMatchingColumnData("Company", "The Big Cheese");

            Assert.AreEqual("Liz Nixon", w3cTable.FindCellByRowAndColumnName(requiredRow, "Contact").Text);
        }

        [Test]
        public void FindCellByColumnAndRowNumber()
        {
            Table w3cTable = new Table(WebDriver.FindElement(By.Id("customers")));

            Assert.AreEqual("Giovanni Rovelli", w3cTable.FindCellByColumnAndRowNumber("Contact", 8).Text);
        }

        [Test]
        public void VerifyAllColumnData()
        {
            List<string> ExpectedColumnData = new List<string>();

            ExpectedColumnData.Add("Germany");
            ExpectedColumnData.Add("Sweden");
            ExpectedColumnData.Add("Mexico");
            ExpectedColumnData.Add("Austria");
            ExpectedColumnData.Add("UK");
            ExpectedColumnData.Add("Germany");
            ExpectedColumnData.Add("Canada");
            ExpectedColumnData.Add("Italy");
            ExpectedColumnData.Add("UK");
            ExpectedColumnData.Add("France");
            ExpectedColumnData.Add("USA");
            ExpectedColumnData.Add("Denmark");

            Table w3cTable = new Table(WebDriver.FindElement(By.Id("customers")));

            Assert.AreEqual(ExpectedColumnData, w3cTable.ReadAllDataFromAColumn("Country"));
        }

        [Test]
        public void VerifyFranciscoChangCompany()
        {
            Table w3cTable = new Table(WebDriver.FindElement(By.Id("customers")));

            Assert.AreEqual("Centro comercial Moctezuma", w3cTable.ReadAColumnForRowContainingValueInColumn("Company", "Francisco Chang", "Contact"));
        }
    }
}