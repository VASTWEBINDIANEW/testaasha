using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.ViewModel
{
    public class DomainExpirationchecker
    {
        private const int Whois_Server_Default_PortNumber = 43;
        private const string Domain_Record_Type = "domain";
        private const string DotCom_Whois_Server = "whois.verisign-grs.com";

        /// <summary>
        /// Retrieves whois information
        /// </summary>
        /// <param name="domainName">The registrar or domain or name server whose whois information to be retrieved</param>
        /// <param name="recordType">The type of record i.e a domain, nameserver or a registrar</param>
        /// <returns></returns>
        public static string Lookup(string domainName)
        {
            using (TcpClient whoisClient = new TcpClient())
            {
                try
                {
                    //domainName = "vastwebindia.com";


                    whoisClient.Connect(DotCom_Whois_Server, Whois_Server_Default_PortNumber);

                    string domainQuery = Domain_Record_Type + " " + domainName + "\r\n";
                    byte[] domainQueryBytes = Encoding.ASCII.GetBytes(domainQuery.ToCharArray());
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    Stream whoisStream = whoisClient.GetStream();
                    whoisStream.Write(domainQueryBytes, 0, domainQueryBytes.Length);

                    StreamReader whoisStreamReader = new StreamReader(whoisClient.GetStream(), Encoding.ASCII);

                    string streamOutputContent = "";
                    List<string> whoisData = new List<string>();
                    while (null != (streamOutputContent = whoisStreamReader.ReadLine()))
                    {
                        whoisData.Add(streamOutputContent);
                    }

                    whoisClient.Close();
                   // var address = Dns.GetHostAddresses(domainName)[0];
                   // lookupss();
                    return String.Join(Environment.NewLine, whoisData[6]);
                }
                catch(Exception ex)
                {
                    return null;
                }
            }
        }
   
    
    public static string lookupss()
        {
            string IpAddressString = "117.251.115.172"; //eggheadcafe

            try
            {
                IPAddress hostIPAddress = IPAddress.Parse(IpAddressString);
                IPHostEntry hostInfo = Dns.GetHostByAddress(hostIPAddress);
                // Get the IP address list that resolves to the host names contained in 
                // the Alias property.
                IPAddress[] address = hostInfo.AddressList;
                // Get the alias names of the addresses in the IP address list.
                String[] alias = hostInfo.Aliases;

                Console.WriteLine("Host name : " + hostInfo.HostName);
                Console.WriteLine("\nAliases :");
                for (int index = 0; index < alias.Length; index++)
                {
                    Console.WriteLine(alias[index]);
                }
                Console.WriteLine("\nIP address list : ");
                for (int index = 0; index < address.Length; index++)
                {
                    Console.WriteLine(address[index]);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
            catch (FormatException e)
            {
                Console.WriteLine("FormatException caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
            return null;
        }
    
    }
}