using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Cliver;
using Extractor.Helpers;
using HtmlAgilityPack;

namespace Extractor.PreProcessors
{
    static class PubDateFinder
    {
        private static HtmlDocument GetAndParseHtml(string url)
        {
            var doc = new HtmlDocument();
            var wc = new WebClientEx(new CookieContainer());
            var contents = wc.DownloadString(url);
            doc.LoadHtml(contents);

            return doc;
        }

        private static IEnumerable<string> GetTimeElementStrings(HtmlDocument parsedHtml)
        {
            var timeElementStrings = new List<string>();

            var timeElements = parsedHtml.DocumentNode.SelectNodes("//time");

            if (timeElements != null)
            {
                foreach (var element in timeElements)
                {
                    timeElementStrings.Add(element.InnerText);

                    foreach (var attribute in element.Attributes)
                    {
                        timeElementStrings.Add(attribute.Value);
                    }
                }
            }

            return timeElementStrings;
        }

        private static IEnumerable<string> GetMetaElementStrings(HtmlDocument parsedHtml)
        {
            var metaDateAttributes = new[] { "name", "property", "itemprop" };
            var metaDateKeywords = new[] { "publi", "create", "issue", "dateModified", "ptime" };
            
            var metaElements = new List<string>();
            foreach (var keyWord in metaDateKeywords)
            {
                foreach (var attribute in metaDateAttributes)
                {
                    var node = parsedHtml.DocumentNode.SelectSingleNode(String.Format("//meta[@{0}='{1}']", attribute, keyWord));
                    if (node != null) metaElements.Add(node.Attributes["content"].Value);
                }
            }

            return metaElements;
        }

        private static DateTime? ParseForDates(string str)
        {
            try
            {
                var timeParse = DateTime.Parse(str);
                return timeParse;
            }
            catch {}

            try
            {
                DateTimeRoutines.ParsedDateTime pdt;
                str.TryParseDate(DateTimeRoutines.DateTimeFormat.USA_DATE, out pdt);
                return pdt.DateTime;
            }
            catch
            {
                return null;
            }
            
        }

        private static DateTime MostCommonDate(IEnumerable<DateTime> dates)
        {
           return dates.GroupBy(i => i).OrderByDescending(grp => grp.Count())
               .Select(grp => grp.Key).First();
        }

        public static DateTime GetDateUrl(string url)
        {
            var urlTime = ParseForDates(url);
            if (urlTime != null) return DateTime.Parse(urlTime.ToString());

            var parsedHtml = GetAndParseHtml(url);
            var timeStrings = GetTimeElementStrings(parsedHtml);

            var parsedDates = new List<DateTime>();

            var datesInUrl = Regex.Match(url, @".*(\d{4})(?:\/(\d{2}))?(?:\/(\d{2})).*?$");

            if (datesInUrl.Success)
            {
                var parsedDate =
                    DateTime.Parse(datesInUrl.Groups[1].Value + "-" + datesInUrl.Groups[2].Value + "-" +
                                   datesInUrl.Groups[3].Value);
                parsedDates.Add(parsedDate);
            }

            foreach (var timeString in timeStrings)
            {
                var parsedTimeString = ParseForDates(timeString);
                if (parsedTimeString == null) continue;
                parsedDates.Add(DateTime.Parse(parsedTimeString.ToString()));
            }

            parsedDates.RemoveAll(item => item == null);

            if (parsedDates.Count != 0)
            {
                return MostCommonDate(parsedDates);
            }

            var metaStrings = GetMetaElementStrings(parsedHtml);
            parsedDates.Clear();

            foreach (var metaString in metaStrings)
            {
                var parsedMetaString = ParseForDates(metaString);
                if (parsedMetaString == null) continue;
                parsedDates.Add(DateTime.Parse(parsedMetaString.ToString()));
            }

            parsedDates.RemoveAll(item => item == null);

            if (parsedDates.Count == 0) throw new Exception("No publishing date found.");

            return MostCommonDate(parsedDates);
        }
    }
}
