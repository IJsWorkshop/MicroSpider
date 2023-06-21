using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MicroSpider
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // chain multi url's for crawler
            foreach (var url in LoadUrl()) 
            {
                // lets go all spidery :D
                var carList = StartCrawler(url);

                // iterate list of cars
                foreach (var c in carList.Result)
                {
                    // write to debug window
                    Debug.WriteLine($"{c.Model}");
                    Debug.WriteLine($"{c.Price}");
                    Debug.WriteLine($"{c.Link}");
                    Debug.WriteLine($"{c.ImageUrl}");
                }
            }
            // wait for a keypress
            Console.ReadLine();
        }

        static List<string> LoadUrl() 
        {
            // init list of url's as string
            var urlList = new List<string>();
            // add urls to list
            urlList.Add("https://www.automobile.tn/neuf/bmw.3/");
            urlList.Add("https://www.automobile.tn/fr/neuf/ford");
            urlList.Add("https://www.automobile.tn/fr/neuf/audi");
            urlList.Add("https://www.automobile.tn/fr/neuf/honda");
            // return list
            return urlList;
        }

        static async Task<List<Car>> StartCrawler(string url) 
        {
            // buid httlp client
            var httpClient = new HttpClient();
            // load web site
            var html = await httpClient.GetStringAsync(url);
            // build dom
            var htmlDocumtent = new HtmlDocument();
            // load html
            htmlDocumtent.LoadHtml(html);
            // navigate dom collect descendants of 'div'
            var divs = htmlDocumtent.DocumentNode.Descendants("div").Where(s => s.GetAttributeValue("class", "").Equals("versions-item")).ToList();
            // init list of type Car
            var cars = new List<Car>();

            // iterate collected objects of div
            foreach (var div in divs) 
            {
                // init type Car
                var car = new Car();
                // get car price
                var carprice = div.Descendants("div").Where(d => d.GetAttributeValue("class", "").Contains("price-ht") && d != null).FirstOrDefault();
                // make sure not null
                if (carprice != null)
                    car.Price = carprice.InnerText.Trim('\n', ' ');
                else
                    car.Price = "0 € HT";
                // get model
                car.Model = div.Descendants("h2").FirstOrDefault().InnerText;
                // get Link
                car.Link = div.Descendants("a").FirstOrDefault().ChildAttributes("href").FirstOrDefault().Value;
                // get image url
                car.ImageUrl = div.Descendants("img").Select(s => s.GetAttributeValue("src", "")).Where(w => !String.IsNullOrEmpty(w)).FirstOrDefault().ToString();
                // build collection
                cars.Add(car);
            }
            // return list of cars
            return cars;
        }

        class Car 
        {
            public string Model { get; set; } = default;
            public string Price { get; set; } = default;
            public string Link { get; set; } = default;
            public string ImageUrl { get; set; } = default;
        }
    }
}
