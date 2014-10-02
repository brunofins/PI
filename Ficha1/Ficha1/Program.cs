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

        private readonly String apiLink = "https://api.github.com";
        private readonly String repos = "/repos";
        private readonly String orgs = "/orgs";

        private static RestClient client;

        private static RestRequest request;

        public static void Main(string[] args)
        {
            //request.Resource = "/orgs/octokit/repos";
            //request.Resource = "/orgs/flatiron/repos";
            //request.Resource = "/orgs/github/repos";

            if (args.Length != 1)
            {
                throw new ArgumentException();
            }

            client = new RestClient();
           
             IRestResponse<List<Organization>> responseOrg = GetOrganization();
             
             IRestResponse<List<Repos>> responseRepos = GetRepos();

             IRestResponse<List<Repos>> responseCollaborators = GetReposcollaborators();

            /* HEADER

            String linkValue;
            foreach(var head in response.Headers)//é assim que vamos buscar os headers, onde está o link para obtermos os outros repositorios
                if(head.Name.Equals("Link"))
                   linkValue = (String)head.Value;
            
            Console.WriteLine(response.Data.Count);

            String name = "GitHub", local = "San Francisco, CA";
            org = new Organization(name, local);
            */
            HistogramPrint();
        }

        private static IRestResponse<List<Repos>> GetReposcollaborators()
        {
            throw new NotImplementedException();
        }

        private static IRestResponse<List<Repos>> GetRepos()
        {
            RestRequest request = new RestRequest();
            IRestResponse<List<Repos>> response = client.Execute<List<Repos>>(request);
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

            return null;
        }

        private static IRestResponse<List<Organization>> GetOrganization()
        {

            client.BaseUrl = "https://api.github.com";
            client.Authenticator = new HttpBasicAuthenticator("brunofins", "terceira6");

            return null;
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
