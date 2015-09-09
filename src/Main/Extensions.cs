using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace SeleniumExtensions.Main
{
    public static class Extensions
    {
        public static void Enter(this IWebElement element,string text)
        {
            element.Clear();
            element.SendKeys(text);
        }

        public static WebDriverWait Wait(this IWebDriver driver, int timeout = 60)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            return wait;
        }

        public static void WaitUntilAjaxIsRunning(this IWebDriver I, bool pageHasJquery = true)
        {
            while (true)
            {
                var ajaxIsComplete = false;

                if (pageHasJquery)
                    ajaxIsComplete = (bool)(I as IJavaScriptExecutor).ExecuteScript("if (!window.jQuery) { return false; } else { return jQuery.active == 0; }");
                else
                    ajaxIsComplete = (bool)(I as IJavaScriptExecutor).ExecuteScript("return document.readyState == 'complete'");

                if (ajaxIsComplete)
                    break;

                Thread.Sleep(200);
            }
        }

        public static void WaitUntilElementIsVisible(this IWebDriver I, By selector)
        {
            I.Wait().Until(d => d.FindElements(selector).Any());

            I.Wait().Until(d => d.FindElements(selector).ElementIsAttached());
        }

        private static bool ElementIsAttached(this ICollection<IWebElement> elements)
        {
            try
            {
                elements.Where(d => d.Displayed).Any();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void WaitAndClick(
            this IWebDriver I,
            By selector)
        {
            I.Wait().Until(d => d.FindElements(selector).Any());
            I.FindElement(selector).TryClick();
        }

        public static bool TryClick(
            this IWebElement element)
        {
            try
            {
                element.Click();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}