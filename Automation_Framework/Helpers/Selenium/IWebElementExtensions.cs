using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using Xunit;
using Automation_Framework.Utilities;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Threading;
using Automation_Framework.Utilities;

namespace Automation_Framework.Helpers
{
    public static class IWebElementExtensions
    {
        /// TO DO: https://selenide.org/javadoc/current/com/codeborne/selenide/SelenideElement.html#shouldBe(com.codeborne.selenide.Condition...)
        ///Checks that given element meets all of given conditions.
        ///IMPORTANT: If element does not match then conditions immediately, waits up to 4 seconds until element meets the conditions.
        ///It's extremely useful for dynamic content.

        public static IWebElement shouldBe(this IWebElement webElements, Condition condition, string expectedValue)
        {
            return webElements.shouldBe(condition, expectedValue, timeout: TimeSpan.FromSeconds(4));
        }

        public static IWebElement shouldBe(this IWebElement element, Condition condition, string expectedValue, TimeSpan timeout)
        {
            switch (condition)
            {
                case Condition.exactText:
                    string elementValue = element.Text.Trim();
                    if (string.IsNullOrWhiteSpace(elementValue))
                    {
                        elementValue = element.GetDomAttribute("value").Trim();
                    }
                    if (string.IsNullOrWhiteSpace(elementValue))
                    {
                        elementValue = element.GetAttribute("value").Trim();
                    }

                    string userMessage = string.Format("Expected Value is: '{0}', actual value was: '{1}'", expectedValue, elementValue);
                    Assert.True(expectedValue.Equals(elementValue), userMessage);  // remove white space like /r/n from the message retrieved.
                    break;
                case Condition.attribute:
                    //assert that the web element contains this attribute with any value eg. $("#mydiv").shouldHave(attribute("fileId"));
                    break;
                default:
                    break;
            }
            return element;
        }
        public static IWebElement shouldBe(this IWebElement webElement, Condition condition, string name, string expectedValue)
        {
            switch (condition)
            {
                case Condition.attribute:
                    Assert.Equal(webElement.GetAttribute(name), expectedValue);
                    //assert that the web element contains this attribute with specific value eg.  $("#mydiv").shouldHave(attribute("fileId", "12345"));;
                    break;
                default:
                    break;
            }
            return webElement;
        }
        /// TO DO: https://selenide.org/javadoc/current/com/codeborne/selenide/SelenideElement.html#shouldBe(com.codeborne.selenide.Condition...)
        ///Checks that given element meets all of given conditions.
        ///IMPORTANT: If element does not match then conditions immediately, waits up to 4 seconds until element meets the conditions.
        ///It's extremely useful for dynamic content.
        public static IWebElement shouldBe(this IWebElement element, Condition condition)
        {
            return element.shouldBe(condition, TimeSpan.FromSeconds(4));
        }
        public static IWebElement shouldBe(this IWebElement element, Condition condition, TimeSpan timeout)
        {
            DateTime startTime = DateTime.Now;
            while (DateTime.Now - startTime < timeout)
            {
                try
                {
                    switch (condition)
                    {
                        case Condition.exist:
                        case Condition.visible:
                            Assert.NotNull(element);
                            break;
                        case Condition.selected:
                            Assert.True(element.Selected, "Element should have been selected");
                            break;
                        case Condition.enabled:
                            Assert.True(element.Enabled, "Element should have been enabled");
                            break;
                        case Condition.hidden:
                            Assert.True(element.Displayed == false, "Element should have been hidden");
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                catch (Exception ex) when (ex is NoSuchElementException || ex is StaleElementReferenceException)
                {
                    if (condition.Equals(Condition.hidden))
                    {
                        return element;
                    }
                    else
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                }
                catch
                {
                    if (DateTime.Now - startTime > timeout)
                        throw;
                    else
                        continue;
                }
                return element;
            }
            return element;
        }

        public static IWebElement shouldNotBe(this IWebElement element, Condition condition)
        {
            switch (condition)
            {
                case Condition.exist:
                case Condition.visible:
                    Assert.Null(element);
                    break;
                case Condition.selected:
                    Assert.False(element.Selected);
                    break;
                case Condition.empty:
                    string elementValue = element.Text;
                    if (string.IsNullOrWhiteSpace(elementValue))
                    {
                        elementValue = element.GetDomAttribute("value");
                    }
                    if (string.IsNullOrWhiteSpace(elementValue))
                    {
                        elementValue = element.GetAttribute("value");
                    }
                    Assert.False(string.IsNullOrWhiteSpace(elementValue));
                    break;
                default:
                    throw new NotImplementedException();
            }
            return element;
        }

        public static By shouldNotBe(this By locator, IWebDriver driver, Condition condition)
        {
            return locator.shouldNotBe(driver, condition, TimeSpan.FromSeconds(30));
        }
        public static By shouldNotBe(this By locator, IWebDriver driver, Condition condition, TimeSpan timeout)
        {
            WebDriverWait wait = new WebDriverWait(driver, timeout);

            switch (condition)
            {
                case Condition.exist:
                case Condition.visible:
                    WaitsHelper.waitForInvisiblityOfElement(driver, locator, timeout.TotalSeconds);
                    break;
                case Condition.selected:
                    wait.Until(ExpectedConditions.ElementSelectionStateToBe(locator, false));
                    break;
                case Condition.enabled:
                    WaitsHelper.waitUntilElementIsVisible(driver, locator, timeout.TotalSeconds);
                    Assert.False(driver.FindElement(locator).Enabled, string.Format("Element at locator {0} is enabled when it shouldn't be.", locator));
                    break;
                default:
                    throw new NotImplementedException();
            }
            return locator;
        }

        public static bool elementIs(this IWebElement element, Condition condition)
        {
            switch (condition)
            {
                case Condition.visible:
                    if (element is null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                default:
                    break;
            }
            return false;
        }
        public static bool elementIs(this IWebElement element, Condition condition, string value)
        {
            switch (condition)
            {
                case Condition.attribute:
                    string attributeValue = element.GetAttribute(value);
                    if (attributeValue is null)
                    {
                        return false;
                    }

                    else
                    {
                        return true;
                    }
                case Condition.type:
                    attributeValue = element.GetAttribute("type");
                    return attributeValue.Equals(value, StringComparison.OrdinalIgnoreCase);
                case Condition.matchText:
                case Condition.exactText:
                    return (Regex.Matches(element.Text.ToString(), value).Count > 0);
                case Condition.matchesTextIgnoreCase:
                    return (Regex.Matches(element.Text.ToString(), value, RegexOptions.IgnoreCase).Count > 0);
                default:
                    break;
            }
            return false;
        }
        public static bool elementIsNot(this IWebElement element, Condition condition, string value)
        {
            return !elementIs(element, condition, value);
        }

        public static bool elementIs(this IWebElement element, Condition condition, string attribute, string value)
        {
            switch (condition)
            {
                case Condition.attribute:
                    string attributeValue = element.GetAttribute(attribute);
                    if (attributeValue is null)
                    {
                        return false;
                    }
                    else
                    {
                        if (attributeValue.Equals(value, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                default:
                    throw new NotImplementedException();
            }
        }
        public static bool exists(this IWebElement element)
        {
            return !(element is null);
        }

        public static IWebElement Find(this IWebElement element, By locator)
        {
            try
            {
                return element.FindElement(locator);
            }
            catch (NoSuchElementException)
            {
                return null;
                // adding this try catch because FindElement returns Exception if it does not finds any element
            }

        }

        public static IWebElement Find(this By locator, IWebDriver driver)
        {
            return driver.FindElement(locator);
        }

        public static IWebElement Find(this IWebElement element, string cssSelector)
        {
            return element.FindElement(By.CssSelector(cssSelector));
        }

        //public static ReadOnlyCollection<IWebElement> FindAll(this By element, IWebDriver driver)
        //{
        //    return driver.FindElements(element);
        //}



        ///Returns:
        ///list of elements inside given element matching given CSS selector
        public static ReadOnlyCollection<IWebElement> FindAll(this IWebElement thisElement, string cssSelector)
        {
            var webElements = thisElement.FindElements(By.CssSelector(cssSelector));
            return webElements;

            // TO DO need to test implementation
        }

        ///Returns:
        ///list of elements inside given element matching given CSS selector
        public static ReadOnlyCollection<IWebElement> FindAll(this IWebElement thisElement, By locator)
        {
            var webElements = thisElement.FindElements(locator);
            return webElements;

            // TO DO need to test implementation
        }

        public static IWebElement scrollIntoView(this IWebElement element, IWebDriver driver, string options)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript(string.Format("arguments[0].scrollIntoView({0});", options), element);
            return element;
        }

        /// <summary>
        /// Ask browser to scrolls the element on which it's called into the visible area of the browser window.
        ///If alignToTop bool value is true - the top of the element will be aligned to the top.
        ///If alignToTop bool value is false - the bottom of the element will be aligned to the bottom.Usage:
        ///     element.scrollIntoView(true);
        ///     // Corresponds to scrollIntoViewOptions: {block: "start", inline: "nearest"}
        ///
        ///     element.scrollIntoView(false);
        ///     // Corresponds to scrollIntoViewOptions: {block: "end", inline: "nearest"}
        /// </summary>
        /// <param name="element"></param>
        /// <param name="driver"></param>
        /// <param name="alignToTop"></param>
        /// <returns></returns>
        public static IWebElement scrollIntoView(this IWebElement element, IWebDriver driver, bool alignToTop)
        {
            if (alignToTop)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript(string.Format("arguments[0].scrollIntoView({0});", @"{ block: ""start"", inline: ""nearest""}"), element);
            }
            else
            {
                ((IJavaScriptExecutor)driver).ExecuteScript(string.Format("arguments[0].scrollIntoView({0});", @"{ block: ""end"", inline: ""nearest""}"), element);
            }
            return element;
        }

        public static IWebElement scrollTo(this IWebElement element, IWebDriver driver)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
            return element;
        }


        //Get parent element of this element (lazy evaluation) For example, $("td").parent() could give some "tr".
        public static IWebElement parent(this IWebElement element)
        {
            return element.FindElement(By.XPath("./.."));
        }

        //Locates the closest ancestor element matching given criteria.
        //        For example, $("td").ancestor("table") returns the closest "table" element above "td".
        //Same as closest("selector", 0) or closest("selector"). Examples:
        //$("td").ancestor("table") will find the closest ancestor with tag table
        //$("td").ancestor(".container") will find the closest ancestor with CSS class .container
        //$("td").ancestor("[data-testid]") will find the closest ancestor with attribute data-testid
        //$("td").ancestor("[data-testid=test-value]") will find the closest ancestor with attribute and attribute's value data-testid=test-value
        public static IWebElement ancestor(this IWebElement element, string selector)
        {
            return element.FindElement(By.XPath("./ancestor::" + selector));
        }

        public static IWebElement closest(this IWebElement element, string selector)
        {
            return element.FindElement(By.XPath("./ancestor::" + selector));
        }

        ///<summary>
        ///Get the following sibling element of this element For example, $("td").sibling(1) will give the first following sibling element of "td"
        ///</summary>
        public static IWebElement sibling(this IWebElement element, int index)
        {
            ReadOnlyCollection<IWebElement> sib = element.FindElements(By.XPath("following-sibling::span"));

            return sib[index];
        }

        public static bool ancestorExists(this IWebElement element, string selector)
        {
            try
            {
                if (element.FindElement(By.XPath("./ancestor::" + selector)) != null)
                    return true;
            }
            catch { }

            return false;
        }
        /// <summary>
        /// Find (first) selected option from this select field
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IWebElement getSelectedOption(this IWebElement element)
        {
            return element;
        }


        /// <summary>
        /// Get the text code of the element with children.
        /// It can be used to get the text of a hidden element.
        /// 
        /// Short form of getAttribute("textContent") or getAttribute("innerText") depending on browser.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string innerText(this IWebElement element)
        {
            return element.GetAttribute("innerText");
        }

        public static string innerHtml(this IWebElement element)
        {
            return element.GetAttribute("innerHTML");
        }

        public static void selectOptionByValue(this IWebElement element, String dropdownText)
        {
            SelectElement select = new SelectElement(element);
            select.SelectByText(dropdownText);
        }
        public static void selectOptionContainingText(this IWebElement element, String dropdownText)
        {
            SelectElement select = new SelectElement(element);
            select.SelectByText(dropdownText);
        }

    }

    public enum CollectionCondition
    {
        sizeGreaterThanOrEqual,
        sizeGreaterThan,
        size

    };

    public enum Condition
    {
        exactText,
        equalsIgnoreCase,
        visible,
        attribute,
        exist,
        text,
        ownText,
        selected,
        type,
        empty,
        matchesText,
        matchesTextIgnoreCase,
        matchText,
        hidden,
        enabled,
        appear,
        disappear,
        disabled
    }
}
