﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            return new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
        }

        public static void WaitUntilAjaxIsRunning(this IWebDriver driver, bool pageHasJquery = true)
        {
            while (true)
            {
                var ajaxIsComplete = false;

                if (pageHasJquery)
                    ajaxIsComplete = (bool)(driver as IJavaScriptExecutor).ExecuteScript("if (!window.jQuery) { return false; } else { return jQuery.active == 0; }");
                else
                    ajaxIsComplete = (bool)(driver as IJavaScriptExecutor).ExecuteScript("return document.readyState == 'complete'");

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

        public static bool SwitchToWindow(this IWebDriver driver, Expression<Func<IWebDriver, bool>> predicateExp)
        {
            var predicate = predicateExp.Compile();
            foreach (var handle in driver.WindowHandles)
            {
                driver.SwitchTo().Window(handle);
                if (predicate(driver))
                {
                    return true;
                }
            }

            return false;
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

        public static void WaitAndClick(this IWebDriver driver, By selector)
        {
            driver.Wait().Until(d => d.FindElements(selector).Any());
            driver.FindElement(selector).TryClick();
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

        public static IWebElement GetParent(this IWebElement e)
        {
            return e.FindElement(By.XPath(".."));
        }

        public static void ScrollTo(
            this IWebDriver I,
            IWebElement element)
        {
            var yPosition = element.Location.Y - 100;
            var js = (IJavaScriptExecutor)I;

            js.ExecuteScript(string.Format("window.scrollTo(0, {0})", yPosition));
        }

        public static void ScrollAndClick(
            this IWebDriver I,
            IWebElement element)
        {
            ScrollTo(I, element);

            I.Wait().Until(d => element.TryClick());
        }

        public static void Hover(
            this IWebDriver driver,
            IWebElement element)
        {
            var actions = new Actions(driver);
            actions.MoveToElement(element);
            actions.Perform();
        }

        public static string GetCurrentUri(
            this IWebDriver driver)
        {
            return driver.Url;
        }

        public static SelectElement DropDownList(
            this IWebDriver I,
            By selector)
        {
            return new SelectElement(I.FindElement(selector));
        }
    }
}