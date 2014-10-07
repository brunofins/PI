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
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentOutOfRangeException("Invalid arguments number");
            }

            IndexWords(args[0], Convert.ToInt32(args[1]));

            ReadWord();

            GetUrls();


        }

        private static void GetUrls()
        {
            throw new NotImplementedException();
        }

        private static void ReadWord()
        {
            System.Console.WriteLine("Write a word to search:");
            System.Console.Write(">> ");
            String wordToSearch = System.Console.ReadLine();
            System.Console.WriteLine(wordToSearch);
        }

        private static void IndexWords(String url, int depth)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(url);

            HtmlAgilityPack.HtmlNode bodyNode = doc.DocumentNode.SelectSingleNode("//body");

            String test1 = bodyNode.InnerHtml;//Obtemos o codigo html do body
            System.Console.WriteLine(test1);
            System.Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            String test2 = bodyNode.InnerText;//Obtemos o texto do body sem as tags
            System.Console.WriteLine(test2);
            
            int stop = 0;// so para parar o programa aqui para fazer debug
          
        }
    }
}
