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
        private static int lanMaxName;
        private static int lanMaxOcc;
        private static int lanOccTotal = 0;

        private static readonly String apiLink = "https://api.github.com";
        private static readonly String repos = "/repos";
        private static readonly String orgsLink = "/orgs";

        private static RestClient client;

        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentOutOfRangeException("Invalid arguments number");
            }


            client = new RestClient();

            if (!GetOrganization(args[0]))
                throw new ArgumentException("Invalid Organization name");

            IRestResponse<List<Repos>> response = null;
            List<Repos> responseData;
            String curUri = apiLink + orgsLink + "/" + args[0] + repos;

            while (curUri != null) { 

                GetResponse<Repos>(curUri, out response, out responseData);
            
                GetRepos(responseData);

                GetReposcollaborators(responseData);

                curUri = GetUriFromLink(response);

            }
            
            HistogramPrint();
        }

        private static void GetReposcollaborators(List<Repos> responseData)
        {
            String colLink;
            foreach (Repos r in responseData)
            {
                colLink = r.collaborators_url;

                colLink = colLink.Remove(colLink.IndexOf('{'));

                IRestResponse<List<Collaborator>> colResponse = null;
                List<Collaborator> colData;

                String loginCol;
                while (colLink != null)
                {
                    GetResponse<Collaborator>(colLink, out colResponse, out colData);

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
                    colLink = GetUriFromLink(colResponse);
                }
            }
        }

        private static void GetRepos(List<Repos> responseData)
        {
            String lName;
            lanOccTotal += responseData.Count;
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

        private static void GetResponse<T>(String link, out IRestResponse<List<T>> response, out List<T> responseData)
        {
            client.BaseUrl = link;
            RestRequest request = new RestRequest();
            request.AddHeader("Authorization", "token 4f3396d6e9cc5df452c535597bad40f28c6d8ab7");
            response = client.Execute<List<T>>(request);
            responseData = response.Data;
        }

        private static String GetUriFromLink<T>(IRestResponse<T> response)
        {
            foreach (var head in response.Headers)
                if (head.Name.Equals("Link") && ((String)head.Value).Contains("rel=\"next\""))
                    return ((String)head.Value).Substring(1, ((String)head.Value).IndexOf("rel=\"next\"") - 4);
            return null;
        }

        private static bool GetOrganization(String orgName)
        {

            client.BaseUrl = apiLink + orgsLink + "/" + orgName;
            RestRequest request = new RestRequest();
            request.AddHeader("Authorization", "token 4f3396d6e9cc5df452c535597bad40f28c6d8ab7");
            
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
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("Collaborators");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------");
            PrintTable(collaboratorOcurs, lanOccTotal, colMaxName, colMaxOcc); 
            }
            

        }

        private static void PrintTable(Dictionary<String, int> dic, int occTotal, int maxName, int maxOcc)
        {
            foreach(KeyValuePair<String,int> kvpLanguage in dic ){
                int nOcc = kvpLanguage.Value;
                double perc = Math.Round((((double)nOcc / occTotal) *100),1);
                String occ = new String('*', (int)perc);
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
