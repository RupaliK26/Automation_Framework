  using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Automation_Framework.Utilities
{
    public class WaitsHelper
    {



        public static void waitUntilElementIsVisible(IWebDriver driver, By element, Double timeInSeconds)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeInSeconds));
            wait.PollingInterval = TimeSpan.FromMilliseconds(200);
            wait.Until(ExpectedConditions.ElementIsVisible(element));
            
        }

        public static void implicitWait(IWebDriver driver, Double timeInSeconds)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeInSeconds);
        }

        public static void waitForPageLoad(IWebDriver driver, Double timeInSeconds)
        {
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(timeInSeconds);
        }

        public static void waitUntilElementClickable(IWebDriver driver, By element)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until(ExpectedConditions.ElementToBeClickable(driver.FindElement(element)));

        }

        public static bool waitForInvisibilityOfLoading(IWebDriver driver, By loading)
        {
            //This method waits for loading overlay to vanish
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(90));
            bool invisible = wait.Until(ExpectedConditions.InvisibilityOfElementLocated(loading));
            return invisible;
        }

        public static void waitForInvisiblityOfElement(IWebDriver driver, By element)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(element));
        }

        public static void waitForInvisiblityOfElement(IWebDriver driver, By element, Double timeInSeconds)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeInSeconds));
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(element));
        }

        public static void waitUntilElementExists(IWebDriver driver, By element, Double timeInSeconds)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeInSeconds));
            wait.Until(ExpectedConditions.ElementExists(element));
        }

        public static IWebElement waitUntilElementExistsAndReturnElement(IWebDriver driver, By element, Double timeInSeconds)
        {
            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(driver);
            fluentWait.Timeout = TimeSpan.FromSeconds(timeInSeconds);
            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
            /* Ignore the exception - NoSuchElementException that indicates that the element is not present */
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            fluentWait.Message = "Element to be searched not found";
            IWebElement searchResult = fluentWait.Until(x => x.FindElement(element));

            return searchResult;
        }

        public static void waitForElementToContainNoText(IWebDriver driver, By element, Double timeInSeconds=30)
        {
            IWebElement ele = driver.FindElement(element);
            Serilog.Log.Information("The text in the element is : " + ele.GetDomProperty("value"));
            while (ele.GetDomProperty("value") != null)
            {
                waitUntilElementIsVisible(driver, element, timeInSeconds);
            }
        }
        public static void waitForElementToContainText(IWebDriver driver, By element, Double timeInSeconds, string text)
        {
            IWebElement elementInput = driver.FindElement(element);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeInSeconds));
            wait.Until(ExpectedConditions.TextToBePresentInElement(elementInput, text));
        }

        
    }
}