//------------------------------------------------------------------------------
// <copyright file="CSSqlFunction.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Forms;
using System.Web;
using System.Net.Http;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

public partial class UserDefinedFunctions
{
    private static string oauthToken = string.Empty;
    private static string serviceUrl = string.Empty;
   


    [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString NYP_RestGet(SqlString uri)
    {
        String document;
        System.Net.ServicePointManager.ServerCertificateValidationCallback +=
        delegate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                System.Security.Cryptography.X509Certificates.X509Chain chain,
                                System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true; // **** Always accept
        };

        // Set up the request, including authentication
        WebRequest req = WebRequest.Create(Convert.ToString(uri));
        ((HttpWebRequest)req).UserAgent = "CLR web client on SQL Server";
        req.ContentType = "application/xml";
        ((HttpWebRequest)req).Accept = "application/xml";
       
        // Fire off the request and retrieve the response.
        // We'll put the response in the string variable "document".
        WebResponse resp = req.GetResponse();
        Stream dataStream = resp.GetResponseStream();
        StreamReader rdr = new StreamReader(dataStream);
        document = (String)rdr.ReadToEnd();
        Console.WriteLine(resp);
        Console.WriteLine(rdr);
        Console.WriteLine(dataStream);

        // Close up everything...
        rdr.Close();
        dataStream.Close();
        resp.Close();

        // .. and return the output to the caller.
        return (document);
    }


    [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString NYP_RestPost(SqlString uri, SqlString postData)
    {
        AuthenticateSfdcRestUser();
        string result = null;
        try
        {
           
            byte[] postBytes = Encoding.UTF8.GetBytes(Convert.ToString(postData));

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(serviceUrl + uri.ToString());
            webRequest.Method = "POST";
            webRequest.ContentType = "application/xml";
            webRequest.ContentLength = postBytes.Length;

            String auth = "Authorization:" + "Bearer " + oauthToken;
            webRequest.Headers.Add(auth);

            Stream postStream = webRequest.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Close();

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            Console.WriteLine(webResponse.StatusCode);
            Console.WriteLine(webResponse.Server);

            Stream responseStream = webResponse.GetResponseStream();
            StreamReader responseStreamReader = new StreamReader(responseStream);
            result = responseStreamReader.ReadToEnd();
            //object o = result;
            //SqlString result_SQLString = o as String; 
            return result;
      
        }
        catch(WebException e) {
            Console.WriteLine("Web exception");
            throw e; }
    }

    public static string dictionaryToPostString(Dictionary<string, string> postVariables)
    {
        string postString = "";
        foreach (KeyValuePair<string, string> pair in postVariables)
        {
            postString += HttpUtility.UrlEncode(pair.Key) + "=" +
                HttpUtility.UrlEncode(pair.Value) + "&";
        }

        return postString;
    }

   // [Microsoft.SqlServer.Server.SqlProcedure]
    public static void AuthenticateSfdcRestUser()
    {

        //print message to console
        Console.WriteLine("Authenticating against the OAuth endpoint ...");

        HttpClient authClient = new HttpClient();

        //set OAuth key and secret variables
        string sfdcConsumerKey = "3MVG9KI2HHAq33RzGj0DY8quczupyyl1Es3NGYB3CCGLmAN_84OS7JOsbJxZxjggag71KphahXmocEcJuX1po";
        string sfdcConsumerSecret = "8391159634086460423";

        //set to Force.com user account that has API access enabled
        string sfdcUserName = "gayathri_krishnan18@yahoo.com";
        string sfdcPassword = "Expelliarmus@123";
        string sfdcToken = "XKOwJnGDa1udZnlIvg1sg8e3";

        //create login password value
        string loginPassword = sfdcPassword + sfdcToken;
        try
        {
            string postString = dictionaryToPostString(new Dictionary<string, string>
                {
                    {"grant_type","password"},
                    {"client_id",sfdcConsumerKey},
                    {"client_secret",sfdcConsumerSecret},
                    {"username",sfdcUserName},
                    {"password",loginPassword}
                }
             );
            byte[] reqData = Encoding.ASCII.GetBytes(postString);
            String document_auth;
            HttpWebRequest request_auth = (HttpWebRequest)WebRequest.Create("https://login.salesforce.com/services/oauth2/token");
         
            request_auth.Method = "POST";
            request_auth.ContentType = "application/x-www-form-urlencoded";
     
            request_auth.ContentLength = reqData.Length;
            // Submit the POST data
            Stream dataStream_auth;
            using (dataStream_auth = request_auth.GetRequestStream())
            dataStream_auth.Write(reqData, 0, reqData.Length);
            dataStream_auth.Close();
            // Collect the response, put it in the string variable "document"
            HttpWebResponse response_auth = (HttpWebResponse)request_auth.GetResponse();
            Stream responseStream = response_auth.GetResponseStream();
            StreamReader reader_auth = new StreamReader(responseStream);
            document_auth = (String)reader_auth.ReadToEnd();

            JObject obj = JObject.Parse(document_auth);
            oauthToken = (string)obj["access_token"];
            serviceUrl = (string)obj["instance_url"];

        }
        catch (WebException e) {
            Console.WriteLine("Web exception");
            throw e; }

    }


};


