using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    class Program
    {
        private static Dictionary<String,List<String>> wordsAndURLs= new Dictionary<String,List<String>>();


        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentOutOfRangeException("Invalid arguments number");
            }

            IndexWords(args[0], Convert.ToInt32(args[1]));

            

            GetUrls(ReadWord());


        }

        private static void GetUrls(string word)
        {
            System.Console.WriteLine("Search results:");
            foreach (String url in wordsAndURLs[word])
                System.Console.WriteLine(url);
        }

        private static String ReadWord()
        {
            System.Console.WriteLine("Write a word to search:");
            System.Console.Write(">> ");
            return System.Console.ReadLine(); 
        }

        private static void IndexWords(String url, int depth)
        {
            if (depth == 0)
                return;

            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(url);

            //HtmlAgilityPack.HtmlNode bodyNode = doc.DocumentNode.SelectSingleNode("//a");

            String nodeString = doc.DocumentNode.OuterHtml;
            //foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//a"))//tentativa de ir buscar todo o texto da pagina html mas ainda nao sei como
                //nodeString += node.InnerText;

            IEnumerable<HtmlNode> urls = doc.DocumentNode.Descendants().Where(x => x.Attributes.Contains("href"));

            System.Console.WriteLine(nodeString);
            String[] wordsArray = nodeString.Split(' ');

            foreach (String s in wordsArray)
                if (wordsAndURLs.ContainsKey(s)) { 
                    if (!wordsAndURLs[s].Contains(url))
                        wordsAndURLs[s].Add(url);
                }
                else
                {
                    List<String> urlList = new List<String>();
                    urlList.Add(url);
                    wordsAndURLs.Add(s, urlList);
                }

            /*foreach (var link in urls)
                IndexWords(link.Attributes["href"].Value, --depth);*/
            
        }
    }
}
