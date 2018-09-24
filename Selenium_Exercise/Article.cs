using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace ExerciseSelenium
{

    [TestClass]
    public class Article
    {
        IWebDriver driver;
        string title;
        [TestInitialize]
        public void TestIntitialize()
        {
            title = "Article "+ Random(1,1000);
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("http://192.168.190.247/joomlatest/administrator/");
            login("quy", "quyquy");
        }

        [TestMethod]
        public void TestCase1()
        {
            AddArticle("Article 1", "Unpublished", "this is article content", "Sample Data-Articles", "save and close");
            CheckArticleSaved("Article saved.", "Article 1", "this is article content", "Sample Data-Articles", "Unpublished");
        }

        [TestMethod]
        public void TestCase2()
        {
            AddArticle("Article 2", "Unpublished", "this is article", "Sample Data-Articles", "save and close");
            CheckOnRecentlyArticle();
            EditArticle("Test Article 123", "- Park Site", "this is modified content", "save and close");
            CheckArticleSaved("Article saved.", "Test Article 123", "this is modified content", "- Park Site", "Unpublished");
        }

        [TestMethod]
        public void TestCase3()
        {
            AddArticle("Article 3", "Unpublished", "this is article", "Sample Data-Articles", "save and close");
            CheckOnRecentlyArticle();
            ChangeStatusArticle("Publish");
            CheckArticleChangeStatus("1 article published.", "publish");
        }

        [TestMethod]
        public void TestCase4()
        {
            AddArticle("Article 4", "Published", "this is article", "Sample Data-Articles", "save and close");
            CheckOnRecentlyArticle();
            ChangeStatusArticle("Unpublish");
            CheckArticleChangeStatus("1 article unpublished.", "unpublish");
        }

        [TestMethod]
        public void TestCase5()
        {
            AddArticle("Article 5", null, "this is article", "Sample Data-Articles", "save and close");
            CheckOnRecentlyArticle();
            ChangeStatusArticle("Archive");
            FilterArticleStatus("Archived");
            CheckArticleDisplayed("Article 5");
        }

        public void login(string username, string password)
        {
            driver.FindElement(By.Id("mod-login-username")).SendKeys(username);
            driver.FindElement(By.Id("mod-login-password")).SendKeys(password);
            driver.FindElement(By.ClassName("login-button")).Click();
        }

        public void AddArticle(string title, string status, string text, string category, string typesave)
        {
            // Click on New Article buton
            driver.FindElement(By.ClassName("j-links-link")).Click();

            // 
            //driver.FindElement(By.ClassName("btn-success")).Click();
            driver.FindElement(By.Id("jform_title")).SendKeys(title);
            driver.FindElement(By.Id("jform_state_chzn")).Click();
            driver.FindElement(By.XPath("//li[contains(.,\"" + status + "\")]")).Click();
            driver.FindElement(By.XPath("//div[@id='jform_catid_chzn']//b")).Click();
            driver.FindElement(By.XPath("//div[@id='jform_catid_chzn']//li[text()=\"" + category + "\"]")).Click();
            driver.SwitchTo().Frame("jform_articletext_ifr");
            driver.FindElement(By.Id("tinymce")).SendKeys(text);
            driver.SwitchTo().DefaultContent();            
        }

        public void ChooseTypeSave(string typesave)
        {
            switch (typesave)
            {
                case "save":
                    driver.FindElement(By.ClassName("btn-success")).Click();
                    break;
                case "save and close":
                    driver.FindElement(By.ClassName("button-save")).Click();
                    break;
            }
        }

        public void CheckArticleSaved(string message, string title, string text, string category, string status)
        {
            //string actualMessage = driver.FindElement(By.XPath("//div[@class = 'alert alert-success']/div[@class = 'alert-message']")).Text;
            Assert.AreEqual(message, driver.FindElement(By.XPath("//div[@class = 'alert alert-success']/div[@class = 'alert-message']")).Text);
            driver.FindElement(By.XPath("//a[contains(.,\"" + title + "\")]")).Click();
            driver.SwitchTo().Frame("jform_articletext_ifr");
            Assert.AreEqual(text, driver.FindElement(By.XPath("//body[@id='tinymce']/p")).Text);
            driver.SwitchTo().DefaultContent();
            Assert.AreEqual(driver.FindElement(By.XPath("//div[@id='jform_catid_chzn']//span")).Text, category);
            Assert.AreEqual(driver.FindElement(By.XPath("//div[@id='jform_state_chzn']//span")).Text, status);
            //loai bo khoang trang
            //a[normalize-space(" abd a12 ")="Article Test 1"]
        }

        public void CheckArticleChangeStatus(string message, string status)
        {
            Assert.AreEqual(message, driver.FindElement(By.XPath("//div[@class = 'alert alert-success']/div[@class = 'alert-message']")).Text);
            Assert.IsTrue(driver.FindElement(By.XPath("//div[@class='btn-group']//span[@class = 'icon-"+status+"']")).Displayed);
        }

        public void CheckOnRecentlyArticle()
        {
            driver.FindElement(By.XPath("//div[@id='list_fullordering_chzn']//b")).Click();
            driver.FindElement(By.XPath("//li[contains(.,'ID descending')]")).Click();
            driver.FindElement(By.XPath("//input[@id='cb0']")).Click();
        }

        public void EditArticle(string newtitle, string newcategory, string newtext, string typesave)
        {
            driver.FindElement(By.ClassName("button-edit")).Click();
            driver.FindElement(By.Id("jform_title")).Clear();
            driver.FindElement(By.Id("jform_title")).SendKeys(newtitle);
            driver.FindElement(By.XPath("//div[@id='jform_catid_chzn']//b")).Click();
            driver.FindElement(By.XPath("//div[@id='jform_catid_chzn']//li[text()=\""+newcategory+"\"]")).Click();
            driver.SwitchTo().Frame("jform_articletext_ifr");
            driver.FindElement(By.Id("tinymce")).Clear();
            driver.FindElement(By.Id("tinymce")).SendKeys(newtext);
            driver.SwitchTo().DefaultContent();
            if (typesave == "save")
            {
                driver.FindElement(By.ClassName("btn-success")).Click();
            }
            else
                if (typesave == "save and close")
            {
                driver.FindElement(By.ClassName("button-save")).Click();
            }
        }

        public void ChangeStatusArticle(string newstatus)
        {
            driver.FindElement(By.XPath("//div[@id='toolbar']//button[normalize-space()=\"" + newstatus + "\"]")).Click();
        }

        public void FilterArticleStatus(string status)
        {
            driver.FindElement(By.XPath("//button[contains(.,'Search Tools')]")).Click();
            System.Threading.Thread.Sleep(1000);
            driver.FindElement(By.XPath("//div[@id='filter_published_chzn']//b")).Click();
            driver.FindElement(By.XPath("//div[@id='filter_published_chzn']//li[contains(.,\"" + status + "\")]")).Click();
        }

        public void CheckArticleDisplayed(string title)
        {
            Assert.IsTrue(driver.FindElement(By.XPath("//table[@id='articleList']//a[normalize-space()=\"" + title + "\"]")).Displayed);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            driver.Close();
            driver.Quit();
        }
    }
}
