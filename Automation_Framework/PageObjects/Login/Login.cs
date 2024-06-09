
using Automation_Framework.Utilities;
using OpenQA.Selenium;
namespace Automation_Framework.PageObjects
{
    /// <summary>
    /// Identifies all web elements from Login screen
    /// And all action methods for those elements 
    /// </summary>
    public class Login
    {
        private static By _txtUsername = By.XPath("//*[@id='identifierId']");
        private static By _nextButton = By.XPath("//*[text()='Next']");
        private static By _password = By.XPath("//input[@name='Passwd']");
        private static By _logIn = By.XPath("//button[text()='Log in']");


        //Logs user into the application
        public static void PerformLogin(IWebDriver driver, string loginUsername, string loginPassword)
        {
            IWebElement username = driver.FindElement(_txtUsername);     
                               
            //Refresh the element in case it went stale
            username = driver.FindElement(_txtUsername);          
            username.Clear();
            username.SendKeys(loginUsername);           
            IWebElement nextButton = driver.FindElement(_nextButton);
            nextButton.Click();
            WaitsHelper.waitUntilElementIsVisible(driver, _password, 2000);
            IWebElement password = driver.FindElement(_password);            
            password.Clear();
            password.SendKeys(loginPassword);
            IWebElement nextButton_Pass = driver.FindElement(_nextButton);
            nextButton_Pass.Click();
           /* IWebElement login = driver.FindElement(_logIn);
            login.Click();*/
        }

    }
}
