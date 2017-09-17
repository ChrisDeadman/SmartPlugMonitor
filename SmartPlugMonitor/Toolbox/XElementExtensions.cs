﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace SmartPlugMonitor.Toolbox
{
    public static class XElementExtensions
    {
        public static XElement MandatoryElement (this XElement xElement, string elementName)
        {
            if (xElement == null)
                throw new MissingFieldException (string.Format ("Mandatory XML Element '{0}' not present", elementName));

            var element = xElement.Element (elementName);
            if (element == null)
                throw new MissingFieldException (string.Format ("Mandatory XML Element '{0}' not present", elementName));

            return element;
        }

        public static XElement OptionalElement (this XElement xElement, string elementName, XElement defaultValue = null)
        {
            if (xElement == null)
                return defaultValue;

            var element = xElement.Element (elementName);
            if (element == null)
                return defaultValue;

            return element;
        }

        public static IEnumerable<XElement> ElementsOfOptionalElement (this XElement xElement, string elementName)
        {
            var element = xElement.OptionalElement (elementName);
            return element == null ? new XElement[0] : element.Elements ();
        }

        public static TElement MandatoryAttribute<TElement> (this XElement xElement, string attributeName)
        {
            if (xElement == null)
                throw new MissingFieldException (string.Format ("Mandatory XML Attribute '{0}' not present", attributeName));

            var attribute = xElement.Attribute (attributeName);
            if (attribute == null)
                throw new MissingFieldException (string.Format ("Mandatory XML Attribute '{0}' not present", attributeName));

            return (TElement)ParseValue (attribute.Value, (dynamic)default(TElement));
        }

        public static TElement OptionalAttribute<TElement> (
            this XElement xElement,
            string attributeName,
            TElement defaultValue = default(TElement))
        {
            if (xElement == null)
                return defaultValue;

            var attribute = xElement.Attribute (attributeName);
            if (attribute == null)
                return defaultValue;

            return (TElement)ParseValue (attribute.Value, (dynamic)defaultValue);
        }

        public static TElement MandatoryValue<TElement> (this XElement xElement)
        {
            if (xElement == null)
                throw new MissingFieldException ("Mandatory Value of XML Elementnot present");

            if (string.IsNullOrEmpty (xElement.Value))
                throw new MissingFieldException (string.Format ("Mandatory Value of XML Element '{0}' is null", xElement));

            return (TElement)ParseValue (xElement.Value, (dynamic)default(TElement));
        }

        public static TElement OptionalValue<TElement> (this XElement xElement, TElement defaultValue = default(TElement))
        {
            if (xElement == null)
                return defaultValue;

            return (TElement)ParseValue (xElement.Value, (dynamic)defaultValue);
        }

        /// <summary>
        /// Adds or replaces an element.
        /// </summary>
        /// <param name="xElement">The target xElement.</param>
        /// <param name="newElement">The new element.</param>
        public static void AddOrReplaceElement (this XElement xElement, XElement newElement)
        {
            var existingElement = xElement.Element (newElement.Name);

            if (existingElement == null)
                xElement.Add (newElement);
            else {
                existingElement.AddBeforeSelf (newElement);
                existingElement.Remove ();
            }
        }

        #region Parsing

        private static string ParseValue (string value, string defaultValue)
        {
            return value ?? defaultValue;
        }

        private static int ParseValue (string value, int defaultValue)
        {
            int result;
            return int.TryParse (value, NumberStyles.Any, CultureInfo.InvariantCulture, out result)
				? result
				: defaultValue;
        }

        private static double ParseValue (string value, double defaultValue)
        {
            double result;
            return double.TryParse (value, NumberStyles.Any, CultureInfo.InvariantCulture, out result)
				? result
				: defaultValue;
        }

        private static bool ParseValue (string value, bool defaultValue)
        {
            int resultInt;
            bool resultBool;
            return bool.TryParse (value, out resultBool)
				? resultBool
				: int.TryParse (value, NumberStyles.Any, CultureInfo.InvariantCulture, out resultInt)
					? resultInt == 1
					: defaultValue;
        }

        private static object ParseValue (string value, object defaultValue)
        {
            throw new NotSupportedException ();
        }

        #endregion
    }
}
