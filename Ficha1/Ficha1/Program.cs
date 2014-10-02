using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ficha1
{
    public class Program
    {
        private static Organization org;
        private static Dictionary<String, int> occs;
        private static int MaxName;
        private static int MaxOcc;
        private static int occTotal =0;
        

        public static void Main(string[] args)
        {
            //var client = new RestClient("https://github.com/octokit");

            var client = new RestClient();
            client.BaseUrl = "https://api.github.com";
            client.Authenticator = new HttpBasicAuthenticator("brunofins", "terceira6");

            var request = new RestRequest();
            request.Resource = "/orgs/octokit/repos";
            //request.Resource = "/orgs/flatiron/repos";
            //request.Resource = "/orgs/github/repos";
            
            IRestResponse<List<Repos>> response = client.Execute<List<Repos>>(request);

            String linkValue;
            foreach(var head in response.Headers)//é assim que vamos buscar os headers, onde está o link para obtermos os outros repositorios
                if(head.Name.Equals("Link"))
                   linkValue = (String)head.Value;
            
            Console.WriteLine(response.Data.Count);
            List<Repos> responseData = response.Data;

            String lName;
            occs = new Dictionary<string, int>();
            foreach (Repos r in responseData)
            {
                if (r.language == null)
                    lName = "";
                else
                    lName = r.language;

                //Procurar o maior nome
                if (lName.Length > MaxName)
                    MaxName = lName.Length;
                //Verificar se ja apareceu a linguagem se sim incrementar n. de ocorrencias caso contrario colocar no dic
                if (occs.ContainsKey(lName))
                    occs[lName]++;
                else
                    occs.Add(lName, 1); 
                //Procurar maior numero de ocorrencias
                if (occs[lName] > MaxOcc)
                    MaxOcc = occs[lName];
                occTotal++;
            }

            var res = occs.ToList();
            foreach(var r in res){
                Console.WriteLine("K:" + r.Key + " V:" + r.Value);
            }

            //Console.WriteLine();


            //string str = "forty-two";
            // char pad = '\t';

            // Console.WriteLine(str.PadLeft(15, pad));
            //Console.WriteLine(str.PadLeft(10, pad));
            String name = "GitHub", local = "San Francisco, CA";
            org = new Organization(name, local);
            //histObj = new List<LanguageOccurrences>();

            //histObj.Add(new LanguageOccurrences("", 6));
            //histObj.Add(new LanguageOccurrences("Ruby", 16));
            //histObj.Add(new LanguageOccurrences("CSS", 2));
            //histObj.Add(new LanguageOccurrences("JavaScript", 3));
            //histObj.Add(new LanguageOccurrences("Python", 1));
            //histObj.Add(new LanguageOccurrences("C", 1));
            //histObj.Add(new LanguageOccurrences("Java", 1));

            HistogramPrint();
        }

        private static void HistogramPrint()
        {
            PrintHeader();
            foreach(KeyValuePair<String,int> kvp in occs ){
                int nOcc = kvp.Value;
                int perc = (int)(((double)nOcc / occTotal) *100);
                String occ = new String('*', nOcc);
                Console.WriteLine(kvp.Key.PadRight(MaxName, ' ') + 
                                  ": " + occ.PadRight(MaxOcc + 5, ' ') + "( " +
                                  perc + "%, " + nOcc + " repos)");
                

            }

        }

        private static void PrintHeader()
        {
          Console.WriteLine(org.name +" (" + org.location + ")");
          Console.WriteLine("-------------------------------------------------------------------");
        }

    
    }
}
