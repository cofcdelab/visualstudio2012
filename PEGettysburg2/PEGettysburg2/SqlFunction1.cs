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
using System.IO;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.Web;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;



public partial class UserDefinedFunctions
{
    private static string oauthToken = string.Empty;
    private static string serviceUrl = string.Empty;

    [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString SF_RestPost(SqlString uri, SqlString postData)
    {
       // String returnVal=AuthenticateSfdcRestUser();
        //string[] tokens = returnVal.Split(',');
       // oauthToken = tokens[1];
       // serviceUrl = tokens[0];
        
        AuthenticateSfdcRestUser();
        string result = null;
        try
        {

            byte[] postBytes = Encoding.UTF8.GetBytes(Convert.ToString(postData));
            System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
                return true; // **** Always accept
            };

            Debug.WriteLine(System.Net.ServicePointManager.CheckCertificateRevocationList);



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
            return result;

        }
        catch (WebException e)
        {
            Console.WriteLine("Web exception");
            throw e;
        }
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

       /* //cofc salesforce creds
        string sfdcConsumerKey = "3MVG9KI2HHAq33Rw9fPHP7lVyvlxEhvfOasMWtORAXzjCVB8SzyYiB.1fyU.LrxpLHamQjtlSEc9wpgBnCsBa";
        string sfdcConsumerSecret = "244377947656650971";*/

        //gettysburg production creds
        string sfdcConsumerKey = "3MVG9KI2HHAq33RxKK9IFuZ8Bo2E7YALviC1kaBRCIwWDQzICzChrGvUf_GfEeqazG8CWiJFaR0mIFVV0_7Jb";
        string sfdcConsumerSecret = "8687622246589661312";
       


        /*//cofc salesforce user creds
        //set to Force.com user account that has API access enabled
        string sfdcUserName = "santhanakrishnang@cofc.edu";
        string sfdcPassword = "Alohamora@123";
        string sfdcToken = "5YEbcOS6Vgg979mWBjdRrRQ6";
        */

        //gettysburg production salesforce user creds
        string sfdcUserName = "santhanakrishnang@g.cofc.edu";
        string sfdcPassword = "amfo_122117";
        string sfdcToken = "FCfPRbqkGR28PzqRjzKUnsrc4";



        //create login password value
        string loginPassword = sfdcPassword + sfdcToken;
        try
        {
            Dictionary<String, String> dictionary = new Dictionary<String, String>();
            dictionary.Add("grant_type", "password");
            dictionary.Add("client_id", sfdcConsumerKey);
            dictionary.Add("client_secret", sfdcConsumerSecret);
            dictionary.Add("username", sfdcUserName);
            dictionary.Add("password", loginPassword);
            string postString = dictionaryToPostString(dictionary);

            byte[] reqData = Encoding.ASCII.GetBytes(postString);
            String document_auth;
            System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
                return true; // **** Always accept
            };


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
        catch (WebException e)
        {
            Console.WriteLine("Web exception");
            throw e;
        }

    }
    

};


