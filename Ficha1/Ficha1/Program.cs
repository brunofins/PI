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
        private static Dictionary<String, int> languageOcurs = new Dictionary<String, int>();
        private static Dictionary<String, int> collaboratorOcurs = new Dictionary<String, int>();
        private static int colMaxName;
        private static int colMaxOcc;
        private static int colOccTotal = 0;
        private static int lanMaxName;
        private static int lanMaxOcc;
        private static int lanOccTotal = 0;

        private static readonly String apiLink = "https://api.github.com";
        private static readonly String repos = "/repos";
        private static readonly String orgsLink = "/orgs";

        private static RestClient client;

        private static RestRequest request;

        private static HttpBasicAuthenticator auth;

        public static void Main(string[] args)
        {
            //request.Resource = "/orgs/octokit/repos";
            //request.Resource = "/orgs/flatiron/repos";
            //request.Resource = "/orgs/github/repos";

            if (args.Length != 1)
            {
                throw new ArgumentOutOfRangeException("Invalid arguments number");
            }

            auth = new HttpBasicAuthenticator("brunofins", "terceira6");

            client = new RestClient();

            if (!GetOrganization(args[0]))
                throw new ArgumentException("Invalid Organization name");

            GetRepos(args[0]);

            GetReposcollaborators(args[0]);

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

        private static void GetReposcollaborators(String orgName)
        {
            IRestResponse<List<Repos>> response = null;
            List<Repos> responseData;
            String link = apiLink + orgsLink + "/" + orgName + repos;
            getResponse(link, response, out responseData);

            String colLink;
            foreach (Repos r in responseData)
            {
            colLink = r.collaborators_url;

            colLink = colLink.Remove(colLink.IndexOf('{'));

            IRestResponse<List<Collaborator>> colResponse = null;
            List <Collaborator> colData;
            getResponse<Collaborator>(colLink, colResponse, out colData);

            String loginCol;
            colOccTotal = colData.Count;
            foreach (Collaborator c in colData)
            {
                if (c.login == null)
                    loginCol = "";
                else
                    loginCol = c.login;

                if (loginCol.Length > colMaxName)
                    colMaxName = loginCol.Length;
                
                if (collaboratorOcurs.ContainsKey(loginCol))
                    collaboratorOcurs[loginCol]++;
                else
                    collaboratorOcurs.Add(loginCol, 1);
                
                if (collaboratorOcurs[loginCol] > colMaxOcc)
                    colMaxOcc = collaboratorOcurs[loginCol];
            }
            }

        }

        private static void GetRepos(String orgName)
        {
            IRestResponse<List<Repos>> response = null;
            List<Repos> responseData;
            String link = apiLink + orgsLink + "/" + orgName + repos;
            getResponse<Repos>(link, response, out responseData);

            String lName;
            lanOccTotal = responseData.Count;
            foreach (Repos r in responseData)
            {
                if (r.language == null)
                    lName = "";
                else
                    lName = r.language;

                //Procurar o maior nome
                if (lName.Length > colMaxName)
                    lanMaxName = lName.Length;
                //Verificar se ja apareceu a linguagem se sim incrementar n. de ocorrencias caso contrario colocar no dic
                if (languageOcurs.ContainsKey(lName))
                    languageOcurs[lName]++;
                else
                    languageOcurs.Add(lName, 1);
                //Procurar maior numero de ocorrencias
                if (languageOcurs[lName] > lanMaxOcc)
                    lanMaxOcc = languageOcurs[lName];
            }
        }

        private static void getResponse<T>(String link, IRestResponse<List<T>> response, out List<T> responseData)
        {
            client.BaseUrl = link;
            RestRequest request = new RestRequest();
            response = client.Execute<List<T>>(request);
            responseData = response.Data;
        }

        private static bool GetOrganization(String orgName)
        {

            client.BaseUrl = apiLink + orgsLink + "/" + orgName;
            client.Authenticator = new HttpBasicAuthenticator("brunofins", "terceira6");

            request = new RestRequest();
            
            IRestResponse<Organization> response = client.Execute<Organization>(request);
 
            org = response.Data;
            if (org.message != null)
                return false;

            if (org.name == null)
                org.name = orgName;

            return true;
        }

        private static void HistogramPrint()
        {
            PrintHeader();
            PrintTable(languageOcurs, lanOccTotal, lanMaxOcc, lanMaxOcc);
            Console.WriteLine();
            if(collaboratorOcurs.Count != 0){
            Console.WriteLine("*****************************************************************");
            Console.WriteLine();
            Console.WriteLine("Collaborators");
            Console.WriteLine("-------------------------------------------------------------------");
            PrintTable(collaboratorOcurs, colOccTotal, colMaxName, colMaxOcc); 
            }
            

        }

        private static void PrintTable(Dictionary<String, int> dic, int occTotal, int maxName, int maxOcc)
        {
            foreach(KeyValuePair<String,int> kvpLanguage in dic ){
                int nOcc = kvpLanguage.Value;
                int perc = (int)(((double)nOcc / occTotal) *100);
                String occ = new String('*', nOcc);
                Console.WriteLine(kvpLanguage.Key.PadRight(maxName, ' ') + 
                                  ": " + occ.PadRight(maxOcc + 5, ' ') + "( " +
                                  perc + "%, " + nOcc + " repos)");
            }
        }

        private static void PrintHeader()
        {
            String header = org.name;
           
          if(org.location != null)
            header += " (" + org.location + ")";
            
          Console.WriteLine(header);
          Console.WriteLine("-------------------------------------------------------------------");
        }

    
    }
}
