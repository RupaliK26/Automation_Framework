using System.Threading;
using Xunit;
using Automation_Framework.Utilities;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
//using /*psqa.testfw.config.RuntimeConfig;*/


//using static com.codeborne.selenide.WebDriverRunner.getWebDriver;

/**
 * Wait helpers.  NOTE: some of these are a sledgehammer approach.
 * When possible, it's better to wait for specific elements on the page to appear/disappear.
 *
 * @author kpace
 */

namespace Automation_Framework.Helpers.Selenium
{
    public class WaitFor
    {
        public static By getMainCPLoadingBar = By.CssSelector(".pds-main-loader .pds-overlay-blocking");


        private WaitFor()
        {
        }


        /**
         * Wait for angular processing to finish.
         */
        public static void waitForAngularToFinish(IWebDriver driver)
        {

            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(30);
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            bool isAngular = (Boolean)executor.ExecuteScript("return window.angular != undefined");
            if (isAngular)
            {
                try
                {
                    executor.ExecuteAsyncScript("var callback = arguments[arguments.Length - 1];" +
                            "angular.element(document.body).injector().get('$browser').notifyWhenNoOutstandingRequests(callback);");
                }
                catch (WebDriverException e)
                {
                    //meh
                }
            }
            //small sleep to let it wrap things up
            Thread.Sleep(200);
        }

        /**
         * Wait for jquery processing to finish
         *
         * @deprecated on Nov 1. jquery.active simply means there are outstanding ajax requests running. Since that's difficult
         * to determine in a test the moment they fire and when they're done, it's not a great strategy to rely on
         */
        [Obsolete()]
        public static void waitForJqueryToFinish(IWebDriver driver)
        {

            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            if ((Boolean)executor.ExecuteScript("return window.jQuery != undefined"))
            {
                while (!(Boolean)executor.ExecuteScript("return jQuery.active == 0"))
                {
                    Thread.Sleep(500);
                }
            }
        }

        /**
         * Wait for document.readyState to be complete
         *
         * @param secondsToWait number of seconds to wait; leave off for default 30
         */
        public static void waitForJavascriptToFinish(IWebDriver driver, params int[] secondsToWait)
        {
            //Depending on when the timing of this is called, it's possible the js gets executed before the intended page action.
            //Meaning: Do some action => wait for JS, it returns => Now the action is actually started
            //Sleep a little bit up front to alleviate that
            Thread.Sleep(300);

            Func<IWebDriver, bool> expect = driver =>
                    ("complete").Equals(((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState"));

            int seconds = secondsToWait.Length > 0 ? secondsToWait[0] : 120;

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));
            try
            {
                wait.Until(expect);
            }
            catch (WebDriverException e)
            {
                //meh
            }
        }

        /**
         * Wait for admin loading bar to be hidden
         */
        public static void waitForAdminLoadingBarToDisappear(IWebDriver driver)
        {
            // Returned to 2000, just like below overloaded version. This method is used many places and every second adds up
            Thread.Sleep(2000);

            // If tests fail after the implicit wait timeout, then adding more time here is probably not the best approach
            // better to use the overloaded version of the method below if more time is needed
            driver.FindElement(By.CssSelector("#loading")).shouldBe(Condition.hidden);
            waitForJavascriptToFinish(driver);
            waitForAngularToFinish(driver);
        }

        /**
         * Wait for admin loading bar to be hidden for waitTime duration
         * <p>
         * ken aug 6 2018: this used to have a try/catch and the exception was being swallowed. any selenide
         * failures results in a screenshot being taken, so this was really cluttering up the results log.
         * changed to have an up front sleep time to allow for the bar to appear, then a normal wait for the bar to be hidden
         */
        public static void waitForAdminLoadingBarToDisappear(IWebDriver driver, DurationInMilliseconds waitTime)
        {
            Thread.Sleep(2000);
            WaitsHelper.waitForInvisiblityOfElement(driver, By.CssSelector("#loading"), waitTime.inSeconds());
            waitForJavascriptToFinish(driver);
        }

        /**
         * Wait for the TT loading bar to disappear. Possible to get timing problems if this code is executed too quickly
         * (meaning it gets called in the gap between a click and the UI responding with the loading bar). For now, have
         * a small up-front sleep time.
         * <p>
         * kpace 10/17/2017: setting the up front sleep to 500ms. Someone had changed it to 2000. The PTPro loading bar will
         * appear after it detects that angular has been busy for 200ms. So 500ms is plenty of time to allow the bar to appear.
         */
        public static void waitForTTLoadingBarToDisappear(IWebDriver driver)
        {
            Thread.Sleep(500);
            WaitsHelper.waitForInvisiblityOfElement(driver, By.CssSelector(".loading-lg"), new DurationInMilliseconds(duration.THREE_MINUTES).inSeconds());
            //dialog disappears, but in the few ms right after, a click might not happen. Have a small sleep to account for that.
            Thread.Sleep(500);
        }

        /**
         * There are two types of loading bars in PTP. This method will wait for the smaller one to disappear.
         * cabreraj 6/21/2021 increasing initial and post sleep as the smaller loading bars sometimes take longer to appear on the page.
         */
        public static void waitForSmallTTLoadingBarToDisappear(IWebDriver driver)
        {
            //want to make sure we wait first for the bar to appear, before we wait for it to disappear
            if (waitForIsDisplayed(driver, By.CssSelector(".loading"), 5))
            {
                //If we are in here then that means the mini load bar has appeared and now we will wait for it to disappear.

                WaitsHelper.waitForInvisiblityOfElement(driver, By.CssSelector("div[class='loading']"), new DurationInMilliseconds(duration.THREE_MINUTES).inMS());
                //dialog disappears, but in the few ms right after, a click might not happen. Have a small sleep to account for that.
                Thread.Sleep(500);
            }
        }

        /**
         * Wait for a TT alert banner (see AlertBanners in psqa.shared.sis.pages.teachers.ptpro.fragments) to disappear before continuing
         * This method is still being tested and may need modifications. Please use this 'at your own risk'
         */
        public static void waitForTTAlertBannerToDisappear(IWebDriver driver)
        {
            Thread.Sleep(2000);
            waitForTTLoadingBarToDisappear(driver);
            WaitsHelper.waitForInvisiblityOfElement(driver, By.CssSelector(".alert-dismissible"), driver.Manage().Timeouts().ImplicitWait.TotalSeconds * 1000L);
        }

        /**
         * Wait for DOM to be in the ready state. Helpful with YUI pages that load and sometimes have visible components
         * before the page can actually be acted on. Has timeout of 60 seconds.
         *
         * @deprecated same as waitForJavascriptToFinish
         */
        [Obsolete()]
        public static void waitForDOMReadyState(IWebDriver driver)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60000));
            wait.Until(webDriver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }



        /**
         * Waits for the main CP loading bar to disappear
         *
         * @param secondsToWait seconds to wait (optional, will default to 60)
         */
        public static void waitForMainCPLoadingBarToDisappear(IWebDriver driver, params int[] secondsToWait)
        {
            int seconds = secondsToWait.Length > 0 ? secondsToWait[0] : 60;
            WaitsHelper.waitForInvisiblityOfElement(driver, getMainCPLoadingBar, (long)seconds * 1000);
            //Ken Nov 12: There's a very slight delay between the bar disappearing and the page elements being ready
            //to interact with. I don't like putting in a sleep in such a high-traffic area, but let's see if this
            //clears up some flakiness
            Thread.Sleep(750);
        }

        /**
         * Wait for the main CP loading bar to disappear, optionally after making sure it displayed first
         *
         * @param waitForBarToAppearFirst Should the test wait for the loading bar to appear in the first place?
         * @param secondsToWait           seconds to wait (optional, will default to 60)
         */
        public static void waitForMainCPLoadingBarToDisappear(IWebDriver driver, bool waitForBarToAppearFirst, params int[] secondsToWait)
        {
            if (waitForBarToAppearFirst)
            {
                waitForMainCPLoadingBarToAppear(driver, secondsToWait);
            }
            waitForMainCPLoadingBarToDisappear(driver, secondsToWait);
        }

        /**
         * wait for CP's loading bar to appear.  It is helpful in waiting while page is still blank, waiting for any portion
         * of page to load.  Once the loading bar appears, waitForMainCPLoadingBarToDisappear(driver) can be called to wait further
         * for the loading bar to disappear.
         *
         * @param secondsToWait seconds to wait (optional, will default to 60)
         */
        public static void waitForMainCPLoadingBarToAppear(IWebDriver driver, params int[] secondsToWait)
        {
            int seconds = secondsToWait.Length > 0 ? secondsToWait[0] : 60;
            WaitsHelper.waitUntilElementIsVisible(driver, getMainCPLoadingBar, (long)seconds * 1000);
        }

        /**
         * Overloaded method that will default switchTo parameter to true
         * as it is expected to be the primary use case.
         *
         * @param expectedWindows Count of the number of windows you expect to see, start value 1
         */
        public static void waitForNewWindowLoad(IWebDriver driver, int expectedWindows)
        {
            waitForNewWindowLoad(driver, expectedWindows, true);
        }

        /**
         * Verifies that a tab or new window has been loaded.  Can be used
         * to verify that things are opening in new windows properly or to
         * ensure that content in a tab is loaded before verifying content
         *
         * @param expectedWindows Count of the number of windows you expect to see, start value 1
         * @param switchTo        Switch to the highest indexed window when loaded.
         */
        public static void waitForNewWindowLoad(IWebDriver driver, int expectedWindows, bool switchTo)
        {
            int returnValue;
            bool boolReturnValue;
            int maxRetryCount = 100;


            try
            {
                for (int i = 0; i < maxRetryCount; Thread.Sleep(100), i++)
                {
                    returnValue = driver.WindowHandles.Count;
                    boolReturnValue = returnValue == expectedWindows ? true : false;
                    if (boolReturnValue && switchTo)
                    {
                        driver.SwitchTo().Window(driver.WindowHandles[expectedWindows - 1]);
                        Thread.Sleep(1000);
                        return;
                    }
                    else if (boolReturnValue)
                    {
                        Thread.Sleep(1000);
                        return;
                    }

                }
            }
            catch (Exception e)
            {
                Serilog.Log.Error("Interrupted exception while attempting to sleep...", e);
                Thread.CurrentThread.Interrupt();
            }

            Assert.True(false, "Expected window count: " + expectedWindows + ", actual count: " + driver.WindowHandles.Count);

        }

        /**
         * Wait for admin loading bar to be hidden
         */
        public static void waitForProgressSpinnerToDisappear(IWebDriver driver)
        {
            Thread.Sleep(1000);
            WaitsHelper.waitForInvisiblityOfElement(driver, By.CssSelector(".progress"), driver.Manage().Timeouts().ImplicitWait.TotalSeconds * 1000L);
            //baffling and very intermittent problem with interacting with elements right after the bar disappears. Try a small sleep
            Thread.Sleep(1000);
        }

        /**
         * This Method makes sure that the page is loaded completely
         */
        public static void waitForPDSLoaderToDisappear(IWebDriver driver)
        {
            By.ClassName("pds-loader").shouldNotBe(driver, Condition.visible);
        }

        /**
         * This Method makes sure that the page is loaded completely
         *
         * @param milliSeconds - Waits until a given period and polls the pds loader to check if it disappears within that time.
         *                     To be used generally when the pds loader appears for more than 60 seconds. Else, use {@link #waitForPDSLoaderToDisappear(driver)}
         */
        public static void waitForPDSLoaderToDisappear(IWebDriver driver, long milliSeconds)
        {
            WaitsHelper.waitForInvisiblityOfElement(driver, By.ClassName("pds-loader"), milliSeconds);
        }

        /**
         * Selenide doesn't have a timed wait for isVisible. I put in a request and it was denied :(. So here's an attempt.
         * <p>
         * Example: Let's say you have an element that may or may not be visible, but you don't know right away if it will
         * be or not. An example would be a "Stay signed in?" message, as the first time (with a new browser session) it
         * will be, but subsequent tests with the same browser it may not show that. The Condition classes throw exceptions,
         * and this is a case where we just need a bool returned without exception handling, but be able to poll the page
         * for an interval to make sure it's there or not.
         * <p>
         * There is a similar method in the Registration project, but that has the seconds to wait as a mandatory param and
         * a true/false param, and I'd rather not modify that one.
         *
         * @param locator       element locator
         * @param secondsToWait (optional) how long to wait for the element to be visible. Default is 5 seconds
         * @return true if the element is found, false if not
         */
        public static bool waitForIsDisplayed(IWebDriver driver, By locator, params int[] secondsToWait)
        {
            /**
            * Function will wait for the element to be visible as per the time provided in the parameter. Default is 5 Seconds.
            */
            int seconds = secondsToWait.Length > 0 ? secondsToWait[0] : 5;
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));
            wait.PollingInterval = TimeSpan.FromMilliseconds(200);
            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(locator));
            }
            catch
            {
                return false;
            }
            return true;
        }

        /**
         *
         * @param element Selenide element
         * @param secondsToWait (optional) how long to wait for the element to be visible. Default is 5 seconds
         * @return true if the element is found, false if not
         */
        public static bool waitForIsDisplayed(IWebDriver driver, IWebElement element, params int[] secondsToWait)
        {
            int seconds = secondsToWait.Length > 0 ? secondsToWait[0] : 5;
            int ms = seconds * 1000;
            int iterations = ms / 250;
            for (int i = 0; i <= iterations; i++)
            {
                if (element.Displayed)
                {
                    return true;
                }
                else
                {
                    Thread.Sleep(250);
                }
            }
            return false;
        }


        /**
         * Waits up to 5 seconds for a dialog (drawer) to open. It does this by looking for the right style attribute and waits
         * for it to be 0px.
         *
         * Works with any dialog in /admin with the ".ui-dialog" class that slides out from the right.
         */
        public static void waitForUIDialog(IWebDriver driver)
        {
            string attr = driver.FindElement(By.CssSelector(".ui-dialog")).GetAttribute("style");
            if (attr != null)
            {
                string val = StringUtils.substringBetween(attr, "right:", ";");
                Serilog.Log.Debug("val initial: {}", val);
                int totalWaitTime = 5000;
                int currentWaitTime = 0;
                while (!val.Contains("0px") && currentWaitTime < totalWaitTime)
                {
                    Thread.Sleep(500);
                    currentWaitTime += 500;
                    attr = driver.FindElement(By.CssSelector(".ui-dialog")).GetAttribute("style");
                    val = StringUtils.substringBetween(attr, "right:", ";");
                    Serilog.Log.Debug("val: {}", val);
                }
                Thread.Sleep(100);
            }
        }

        /**
         * This is an excessive wait-for method, but given the
         * flaky nature of the element-readiness, this method is used
         * for extreme measures.
         *
         * Example usage: waitFor(IWebElement).Click();
         *
         * @param element   Element to wait for
         */

        public static IWebElement waitForReady(IWebDriver driver, IWebElement element)
        {
            waitForJavascriptToFinish(driver);
            waitForIsDisplayed(driver, element);
            element.shouldBe(Condition.visible)
                    .shouldBe(Condition.enabled);
            Thread.Sleep(300);
            return element;
        }

        /**
         * This is an excessive wait-for method, but given the
         * flaky nature of the element-readiness, this method is used
         * for extreme measures.
         *
         * @param locator By locator to wait for
         */

        public static IWebElement waitForReady(IWebDriver driver, By locator)
        {
            waitForJavascriptToFinish(driver);
            if (waitForIsDisplayed(driver, locator))
            {
                IWebElement element = driver.FindElement(locator);
                element.shouldBe(Condition.visible)
                        .shouldBe(Condition.enabled);
                return element;
            }
            return driver.FindElement(locator);
        }

    }

    /**
      * Constants used for wait times.
      */
    public enum duration
    {
        ONE_QUARTER_SECOND = 250,
        ONE_THIRD_SECOND = 300,
        ONE_HALF_SECOND = 500,
        ONE_SECOND = 1000,
        TWO_SECONDS = 2000,
        FIVE_SECONDS = 5000,
        TEN_SECONDS = 10000,
        TWELVE_SECONDS = 12000,
        FIFTEEN_SECONDS = 15000,
        TWENTY_SECONDS = 20000,
        THIRTY_SECONDS = 30000,
        ONE_MINUTE = 60000,
        TWO_MINUTES = 120000,
        THREE_MINUTES = 180000,
        FIVE_MINUTES = 300000
    };
    //Put the methods of the enum into their own class: DurationInMilliseconds
    public class DurationInMilliseconds
    {
        private double time;

        public DurationInMilliseconds(long time)
        {
            this.time = time;
        }

        public DurationInMilliseconds(duration time)
        {
            this.time = (double)time;
        }

        public int inMS()
        {
            return (int)time;
        }

        /**
         * Convert the duration to seconds.
         *
         * @return the duration in seconds. Will be 0 if you ask for a sub-second value.
         */
        public double inSeconds()
        {
            return TimeSpan.FromMilliseconds(time).TotalSeconds;
        }

    }
}
