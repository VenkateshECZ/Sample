using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;
using System.Xml;
//using System.Xml.Linq;
//using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Data.SqlClient;
using DSMData;
using DSM;


namespace DSM
{
    public class Webservicekia
    {


        public string Url { get; private set; }
        public string Method { get; private set; }
        public Dictionary<string, string> Params = new Dictionary<string, string>();
        public XDocument ResponseSOAP = XDocument.Parse("<root/>");
        public XDocument ResultXML = XDocument.Parse("<root/>");
        public string ResultString = String.Empty;
        DSMModelData objDSMModelData = new DSMModelData();
        string msg = "";
        private Cursor InitialCursorState;

        public Webservicekia()
        {
            Url = String.Empty;
            Method = String.Empty;
        }
        public Webservicekia(string baseUrl)
        {
            Url = baseUrl;
            Method = String.Empty;
        }
        public Webservicekia(string baseUrl, string methodName)
        {
            Url = baseUrl;
            Method = methodName;
        }

        // Public API

        /// <summary>
        /// Adds a parameter to the WebMethod invocation.
        /// </summary>
        /// <param name="name">Name of the WebMethod parameter (case sensitive)</param>
        /// <param name="value">Value to pass to the paramenter</param>
        public void AddParameter(string name, string value)
        {
            Params.Add(name, value);
        }

        public void Invoke()
        {
            Invoke(Method, true);
        }

        /// <summary>
        /// Using the base url, invokes the WebMethod with the given name
        /// </summary>
        /// <param name="methodName">Web Method name</param>
        public void Invoke(string methodName)
        {
            Invoke(methodName, true);
        }

        /// <summary>
        /// Cleans all internal data used in the last invocation, except the WebService's URL.
        /// This avoids creating a new WebService object when the URL you want to use is the same.
        /// </summary>
        public void CleanLastInvoke()
        {
            ResponseSOAP = ResultXML = null;
            ResultString = Method = String.Empty;
            Params = new Dictionary<string, string>();
        }

        #region Helper Methods

        /// <summary>
        /// Checks if the WebService's URL and the WebMethod's name are valid. If not, throws ArgumentNullException.
        /// </summary>
        /// <param name="methodName">Web Method name (optional)</param>
        private void AssertCanInvoke(string methodName = "")
        {
            if (Url == String.Empty)
                throw new ArgumentNullException("You tried to invoke a webservice without specifying the WebService's URL.");
            if ((methodName == "") && (Method == String.Empty))
                throw new ArgumentNullException("You tried to invoke a webservice without specifying the WebMethod.");
        }

        private void ExtractResult(string methodName)
        {
            //MessageBox.Show("Coming ExtractResult");
            // Selects just the elements with namespace http://tempuri.org/ (i.e. ignores SOAP namespace)
            XmlNamespaceManager namespMan = new XmlNamespaceManager(new NameTable());
            namespMan.AddNamespace("foo", "http://tempuri.org/");

            //XElement webMethodResult = ResponseSOAP.XPathSelectElement("//foo:" + methodName + "Result", namespMan);
            XElement webMethodResult = ResponseSOAP.XPathSelectElement("//foo:" + methodName + "Result", namespMan);
            //res
            // If the result is an XML, return it and convert it to string
            if (webMethodResult.FirstNode.NodeType == XmlNodeType.Element)
            {
                ResultXML = XDocument.Parse(webMethodResult.FirstNode.ToString());
                ResultXML = Utils.RemoveNamespaces(ResultXML);
                ResultString = ResultXML.ToString();
            }
            // If the result is a string, return it and convert it to XML (creating a root node to wrap the result)
            else
            {
                ResultString = webMethodResult.FirstNode.ToString();
                ResultXML = XDocument.Parse("<root>" + ResultString + "</root>");
            }
        }

        /// <summary>
        /// Invokes a Web Method, with its parameters encoded or not.
        /// </summary>
        /// <param name="methodName">Name of the web method you want to call (case sensitive)</param>
        /// <param name="encode">Do you want to encode your parameters? (default: true)</param>
        private void Invoke(string methodName, bool encode)
        {
            try
            {
                AssertCanInvoke(methodName);
                byte[] file;
                string hexvalue = "";
                objDSMModelData = new DSMModelData();
                file = objDSMModelData.GetHexValueByInvoiceNumber(Global.APIInvoiceNumber);
                string pdfBase64file = Convert.ToBase64String(file);
                hexvalue = BitConverter.ToString(file).Replace("-", "");

                //Correct code for PHA invoice

                int count_rec = objDSMModelData.GetInvoiceDetailsByInvoiceNumber(Global.APIInvoiceNumber).Count;

                string soapStr = "";

                if (count_rec > 0)
                {

                    if (count_rec == 1)
                    {
                        //int i = 0;

                        soapStr =
                   @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                              <soap:Body>
                              <getData xmlns=""{APISOAP}"">
      <HEXADECIMAL>{0}</HEXADECIMAL>
      <data01>
        <GET_DATA>
          <TAB_DATA_HEADER>
            
            <IVNUM>{1}</IVNUM>
            <IVDAT>{2}</IVDAT>
            <LIFNR>{3}</LIFNR>
            <MATNR>{4}</MATNR>
            <ZSHOP>{5}</ZSHOP>
            <EBELN>{6}</EBELN>
            <IVQTY>{7}</IVQTY>
            <ZAIVAMT>{8}</ZAIVAMT>
            <ZANETPR>{9}</ZANETPR>
            <ZANETWR>{10}</ZANETWR>
            <ZCGST>{11}</ZCGST>
            <ZSGST>{12}</ZSGST>
            <ZIGST>{13}</ZIGST>
            <ZUGST>{14}</ZUGST>
            <ZATCS>{TCS1}</ZATCS>
            <ZHSNSAC>{15}</ZHSNSAC>
            <ZGSTIN>{16}</ZGSTIN>
            <VEHNO>{17}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{1N1}</ZNUM1>
            <ZNUM2>{1N2}</ZNUM2>
            <ZNUM3>{1N3}</ZNUM3>
            <ZNUM4>{1N4}</ZNUM4>
            <ZNUM5>{1N5}</ZNUM5>
            <ZCHAR2>{1C2}</ZCHAR2>
            <ZCHAR3>{1C3}</ZCHAR3>
            <ZCHAR4>{1C4}</ZCHAR4>
            <ZCHAR5>{1C5}</ZCHAR5>

          </TAB_DATA_HEADER>
        </GET_DATA>
      </data01>
    </getData>
                              </soap:Body>
                            </soap:Envelope>";
                    }

                    else if (count_rec == 2)
                    {
                        soapStr =
                        @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                              <soap:Body>
                              <getData xmlns=""{APISOAP}"">
      <HEXADECIMAL>{0}</HEXADECIMAL>
      <data01>
        <GET_DATA>
          <TAB_DATA_HEADER>
            <IVNUM>{1}</IVNUM>
            <IVDAT>{2}</IVDAT>
            <LIFNR>{3}</LIFNR>
            <MATNR>{4}</MATNR>
            <ZSHOP>{5}</ZSHOP>
            <EBELN>{6}</EBELN>
            <IVQTY>{7}</IVQTY>
            <ZAIVAMT>{8}</ZAIVAMT>
            <ZANETPR>{9}</ZANETPR>
            <ZANETWR>{10}</ZANETWR>
            <ZCGST>{11}</ZCGST>
            <ZSGST>{12}</ZSGST>
            <ZIGST>{13}</ZIGST>
            <ZUGST>{14}</ZUGST>
            <ZATCS>{TCS1}</ZATCS>
            <ZHSNSAC>{15}</ZHSNSAC>
            <ZGSTIN>{16}</ZGSTIN>
            <VEHNO>{17}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{1N1}</ZNUM1>
            <ZNUM2>{1N2}</ZNUM2>
            <ZNUM3>{1N3}</ZNUM3>
            <ZNUM4>{1N4}</ZNUM4>
            <ZNUM5>{1N5}</ZNUM5>
            <ZCHAR2>{1C2}</ZCHAR2>
            <ZCHAR3>{1C3}</ZCHAR3>
            <ZCHAR4>{1C4}</ZCHAR4>
            <ZCHAR5>{1C5}</ZCHAR5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
             <IVNUM>{18}</IVNUM>
            <IVDAT>{19}</IVDAT>
            <LIFNR>{20}</LIFNR>
            <MATNR>{21}</MATNR>
            <ZSHOP>{22}</ZSHOP>
            <EBELN>{23}</EBELN>
            <IVQTY>{24}</IVQTY>
            <ZAIVAMT>{25}</ZAIVAMT>
            <ZANETPR>{26}</ZANETPR>
            <ZANETWR>{27}</ZANETWR>
            <ZCGST>{28}</ZCGST>
            <ZSGST>{29}</ZSGST>
            <ZIGST>{30}</ZIGST>
            <ZUGST>{31}</ZUGST>
            <ZATCS>{TCS2}</ZATCS>
            <ZHSNSAC>{32}</ZHSNSAC>
            <ZGSTIN>{33}</ZGSTIN>
            <VEHNO>{34}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{2N1}</ZNUM1>
            <ZNUM2>{2N2}</ZNUM2>
            <ZNUM3>{2N3}</ZNUM3>
            <ZNUM4>{2N4}</ZNUM4>
            <ZNUM5>{2N5}</ZNUM5>
            <ZCHAR2>{2C2}</ZCHAR2>
            <ZCHAR3>{2C3}</ZCHAR3>
            <ZCHAR4>{2C4}</ZCHAR4>
            <ZCHAR5>{2C5}</ZCHAR5>
          </TAB_DATA_HEADER>
         </GET_DATA>
      </data01>
    </getData>
                              </soap:Body>
                            </soap:Envelope>";
                    }
                    else if (count_rec == 3)
                    {
                        soapStr =
                        @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                              <soap:Body>
                              <getData xmlns=""{APISOAP}"">
      <HEXADECIMAL>{0}</HEXADECIMAL>
      <data01>
        <GET_DATA>
          <TAB_DATA_HEADER>
            <IVNUM>{1}</IVNUM>
            <IVDAT>{2}</IVDAT>
            <LIFNR>{3}</LIFNR>
            <MATNR>{4}</MATNR>
            <ZSHOP>{5}</ZSHOP>
            <EBELN>{6}</EBELN>
            <IVQTY>{7}</IVQTY>
            <ZAIVAMT>{8}</ZAIVAMT>
            <ZANETPR>{9}</ZANETPR>
            <ZANETWR>{10}</ZANETWR>
            <ZCGST>{11}</ZCGST>
            <ZSGST>{12}</ZSGST>
            <ZIGST>{13}</ZIGST>
            <ZUGST>{14}</ZUGST>
            <ZATCS>{TCS1}</ZATCS>
            <ZHSNSAC>{15}</ZHSNSAC>
            <ZGSTIN>{16}</ZGSTIN>
            <VEHNO>{17}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{1N1}</ZNUM1>
            <ZNUM2>{1N2}</ZNUM2>
            <ZNUM3>{1N3}</ZNUM3>
            <ZNUM4>{1N4}</ZNUM4>
            <ZNUM5>{1N5}</ZNUM5>
            <ZCHAR2>{1C2}</ZCHAR2>
            <ZCHAR3>{1C3}</ZCHAR3>
            <ZCHAR4>{1C4}</ZCHAR4>
            <ZCHAR5>{1C5}</ZCHAR5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
             <IVNUM>{18}</IVNUM>
            <IVDAT>{19}</IVDAT>
            <LIFNR>{20}</LIFNR>
            <MATNR>{21}</MATNR>
            <ZSHOP>{22}</ZSHOP>
            <EBELN>{23}</EBELN>
            <IVQTY>{24}</IVQTY>
            <ZAIVAMT>{25}</ZAIVAMT>
            <ZANETPR>{26}</ZANETPR>
            <ZANETWR>{27}</ZANETWR>
            <ZCGST>{28}</ZCGST>
            <ZSGST>{29}</ZSGST>
            <ZIGST>{30}</ZIGST>
            <ZUGST>{31}</ZUGST>
            <ZATCS>{TCS2}</ZATCS>
            <ZHSNSAC>{32}</ZHSNSAC>
            <ZGSTIN>{33}</ZGSTIN>
            <VEHNO>{34}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{2N1}</ZNUM1>
            <ZNUM2>{2N2}</ZNUM2>
            <ZNUM3>{2N3}</ZNUM3>
            <ZNUM4>{2N4}</ZNUM4>
            <ZNUM5>{2N5}</ZNUM5>
            <ZCHAR2>{2C2}</ZCHAR2>
            <ZCHAR3>{2C3}</ZCHAR3>
            <ZCHAR4>{2C4}</ZCHAR4>
            <ZCHAR5>{2C5}</ZCHAR5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
             <IVNUM>{35}</IVNUM>
            <IVDAT>{36}</IVDAT>
            <LIFNR>{37}</LIFNR>
            <MATNR>{38}</MATNR>
            <ZSHOP>{39}</ZSHOP>
            <EBELN>{40}</EBELN>
            <IVQTY>{41}</IVQTY>
            <ZAIVAMT>{42}</ZAIVAMT>
            <ZANETPR>{43}</ZANETPR>
            <ZANETWR>{44}</ZANETWR>
            <ZCGST>{45}</ZCGST>
            <ZSGST>{46}</ZSGST>
            <ZIGST>{47}</ZIGST>
            <ZUGST>{48}</ZUGST>
            <ZATCS>{TCS3}</ZATCS>
            <ZHSNSAC>{49}</ZHSNSAC>
            <ZGSTIN>{50}</ZGSTIN>
            <VEHNO>{51}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{3N1}</ZNUM1>
            <ZNUM2>{3N2}</ZNUM2>
            <ZNUM3>{3N3}</ZNUM3>
            <ZNUM4>{3N4}</ZNUM4>
            <ZNUM5>{3N5}</ZNUM5>
            <ZCHAR2>{3C2}</ZCHAR2>
            <ZCHAR3>{3C3}</ZCHAR3>
            <ZCHAR4>{3C4}</ZCHAR4>
            <ZCHAR5>{3C5}</ZCHAR5>
          </TAB_DATA_HEADER>
         </GET_DATA>
      </data01>
    </getData>
                              </soap:Body>
                            </soap:Envelope>";
                    }
                    else if (count_rec == 4)
                    {
                        soapStr =
                        @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                              <soap:Body>
                              <getData xmlns=""{APISOAP}"">
      <HEXADECIMAL>{0}</HEXADECIMAL>
      <data01>
        <GET_DATA>
        <TAB_DATA_HEADER>
            <IVNUM>{1}</IVNUM>
            <IVDAT>{2}</IVDAT>
            <LIFNR>{3}</LIFNR>
            <MATNR>{4}</MATNR>
            <ZSHOP>{5}</ZSHOP>
            <EBELN>{6}</EBELN>
            <IVQTY>{7}</IVQTY>
            <ZAIVAMT>{8}</ZAIVAMT>
            <ZANETPR>{9}</ZANETPR>
            <ZANETWR>{10}</ZANETWR>
            <ZCGST>{11}</ZCGST>
            <ZSGST>{12}</ZSGST>
            <ZIGST>{13}</ZIGST>
            <ZUGST>{14}</ZUGST>
            <ZATCS>{TCS1}</ZATCS>
            <ZHSNSAC>{15}</ZHSNSAC>
            <ZGSTIN>{16}</ZGSTIN>
            <VEHNO>{17}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{1N1}</ZNUM1>
            <ZNUM2>{1N2}</ZNUM2>
            <ZNUM3>{1N3}</ZNUM3>
            <ZNUM4>{1N4}</ZNUM4>
            <ZNUM5>{1N5}</ZNUM5>
            <ZCHAR2>{1C2}</ZCHAR2>
            <ZCHAR3>{1C3}</ZCHAR3>
            <ZCHAR4>{1C4}</ZCHAR4>
            <ZCHAR5>{1C5}</ZCHAR5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
             <IVNUM>{18}</IVNUM>
            <IVDAT>{19}</IVDAT>
            <LIFNR>{20}</LIFNR>
            <MATNR>{21}</MATNR>
            <ZSHOP>{22}</ZSHOP>
            <EBELN>{23}</EBELN>
            <IVQTY>{24}</IVQTY>
            <ZAIVAMT>{25}</ZAIVAMT>
            <ZANETPR>{26}</ZANETPR>
            <ZANETWR>{27}</ZANETWR>
            <ZCGST>{28}</ZCGST>
            <ZSGST>{29}</ZSGST>
            <ZIGST>{30}</ZIGST>
            <ZUGST>{31}</ZUGST>
            <ZATCS>{TCS2}</ZATCS>
            <ZHSNSAC>{32}</ZHSNSAC>
            <ZGSTIN>{33}</ZGSTIN>
            <VEHNO>{34}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{2N1}</ZNUM1>
            <ZNUM2>{2N2}</ZNUM2>
            <ZNUM3>{2N3}</ZNUM3>
            <ZNUM4>{2N4}</ZNUM4>
            <ZNUM5>{2N5}</ZNUM5>
            <ZCHAR2>{2C2}</ZCHAR2>
            <ZCHAR3>{2C3}</ZCHAR3>
            <ZCHAR4>{2C4}</ZCHAR4>
            <ZCHAR5>{2C5}</ZCHAR5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
             <IVNUM>{35}</IVNUM>
            <IVDAT>{36}</IVDAT>
            <LIFNR>{37}</LIFNR>
            <MATNR>{38}</MATNR>
            <ZSHOP>{39}</ZSHOP>
            <EBELN>{40}</EBELN>
            <IVQTY>{41}</IVQTY>
            <ZAIVAMT>{42}</ZAIVAMT>
            <ZANETPR>{43}</ZANETPR>
            <ZANETWR>{44}</ZANETWR>
            <ZCGST>{45}</ZCGST>
            <ZSGST>{46}</ZSGST>
            <ZIGST>{47}</ZIGST>
            <ZUGST>{48}</ZUGST>
            <ZATCS>{TCS3}</ZATCS>
            <ZHSNSAC>{49}</ZHSNSAC>
            <ZGSTIN>{50}</ZGSTIN>
            <VEHNO>{51}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{3N1}</ZNUM1>
            <ZNUM2>{3N2}</ZNUM2>
            <ZNUM3>{3N3}</ZNUM3>
            <ZNUM4>{3N4}</ZNUM4>
            <ZNUM5>{3N5}</ZNUM5>
            <ZCHAR2>{3C2}</ZCHAR2>
            <ZCHAR3>{3C3}</ZCHAR3>
            <ZCHAR4>{3C4}</ZCHAR4>
            <ZCHAR5>{3C5}</ZCHAR5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
             <IVNUM>{52}</IVNUM>
            <IVDAT>{53}</IVDAT>
            <LIFNR>{54}</LIFNR>
            <MATNR>{55}</MATNR>
            <ZSHOP>{56}</ZSHOP>
            <EBELN>{57}</EBELN>
            <IVQTY>{58}</IVQTY>
            <ZAIVAMT>{59}</ZAIVAMT>
            <ZANETPR>{60}</ZANETPR>
            <ZANETWR>{61}</ZANETWR>
            <ZCGST>{62}</ZCGST>
            <ZSGST>{63}</ZSGST>
            <ZIGST>{64}</ZIGST>
            <ZUGST>{65}</ZUGST>
            <ZATCS>{TCS4}</ZATCS>
            <ZHSNSAC>{66}</ZHSNSAC>
            <ZGSTIN>{67}</ZGSTIN>
            <VEHNO>{68}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{4N1}</ZNUM1>
            <ZNUM2>{4N2}</ZNUM2>
            <ZNUM3>{4N3}</ZNUM3>
            <ZNUM4>{4N4}</ZNUM4>
            <ZNUM5>{4N5}</ZNUM5>
            <ZCHAR2>{4C2}</ZCHAR2>
            <ZCHAR3>{4C3}</ZCHAR3>
            <ZCHAR4>{4C4}</ZCHAR4>
            <ZCHAR5>{4C5}</ZCHAR5>
          </TAB_DATA_HEADER>
         </GET_DATA>
      </data01>
    </getData>
                              </soap:Body>
                            </soap:Envelope>";
                    }
                    else if (count_rec == 5)
                    {
                        soapStr =
                        @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                              <soap:Body>
                              <getData xmlns=""{APISOAP}"">
      <HEXADECIMAL>{0}</HEXADECIMAL>
      <data01>
        <GET_DATA>
          <TAB_DATA_HEADER>
            <IVNUM>{1}</IVNUM>
            <IVDAT>{2}</IVDAT>
            <LIFNR>{3}</LIFNR>
            <MATNR>{4}</MATNR>
            <ZSHOP>{5}</ZSHOP>
            <EBELN>{6}</EBELN>
            <IVQTY>{7}</IVQTY>
            <ZAIVAMT>{8}</ZAIVAMT>
            <ZANETPR>{9}</ZANETPR>
            <ZANETWR>{10}</ZANETWR>
            <ZCGST>{11}</ZCGST>
            <ZSGST>{12}</ZSGST>
            <ZIGST>{13}</ZIGST>
            <ZUGST>{14}</ZUGST>
            <ZATCS>{TCS1}</ZATCS>
            <ZHSNSAC>{15}</ZHSNSAC>
            <ZGSTIN>{16}</ZGSTIN>
            <VEHNO>{17}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{1N1}</ZNUM1>
            <ZNUM2>{1N2}</ZNUM2>
            <ZNUM3>{1N3}</ZNUM3>
            <ZNUM4>{1N4}</ZNUM4>
            <ZNUM5>{1N5}</ZNUM5>
            <ZCHAR2>{1C2}</ZCHAR2>
            <ZCHAR3>{1C3}</ZCHAR3>
            <ZCHAR4>{1C4}</ZCHAR4>
            <ZCHAR5>{1C5}</ZCHAR5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
             <IVNUM>{18}</IVNUM>
            <IVDAT>{19}</IVDAT>
            <LIFNR>{20}</LIFNR>
            <MATNR>{21}</MATNR>
            <ZSHOP>{22}</ZSHOP>
            <EBELN>{23}</EBELN>
            <IVQTY>{24}</IVQTY>
            <ZAIVAMT>{25}</ZAIVAMT>
            <ZANETPR>{26}</ZANETPR>
            <ZANETWR>{27}</ZANETWR>
            <ZCGST>{28}</ZCGST>
            <ZSGST>{29}</ZSGST>
            <ZIGST>{30}</ZIGST>
            <ZUGST>{31}</ZUGST>
            <ZATCS>{TCS2}</ZATCS>
            <ZHSNSAC>{32}</ZHSNSAC>
            <ZGSTIN>{33}</ZGSTIN>
            <VEHNO>{34}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{2N1}</ZNUM1>
            <ZNUM2>{2N2}</ZNUM2>
            <ZNUM3>{2N3}</ZNUM3>
            <ZNUM4>{2N4}</ZNUM4>
            <ZNUM5>{2N5}</ZNUM5>
            <ZCHAR2>{2C2}</ZCHAR2>
            <ZCHAR3>{2C3}</ZCHAR3>
            <ZCHAR4>{2C4}</ZCHAR4>
            <ZCHAR5>{2C5}</ZCHAR5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
             <IVNUM>{35}</IVNUM>
            <IVDAT>{36}</IVDAT>
            <LIFNR>{37}</LIFNR>
            <MATNR>{38}</MATNR>
            <ZSHOP>{39}</ZSHOP>
            <EBELN>{40}</EBELN>
            <IVQTY>{41}</IVQTY>
            <ZAIVAMT>{42}</ZAIVAMT>
            <ZANETPR>{43}</ZANETPR>
            <ZANETWR>{44}</ZANETWR>
            <ZCGST>{45}</ZCGST>
            <ZSGST>{46}</ZSGST>
            <ZIGST>{47}</ZIGST>
            <ZUGST>{48}</ZUGST>
            <ZATCS>{TCS3}</ZATCS>
            <ZHSNSAC>{49}</ZHSNSAC>
            <ZGSTIN>{50}</ZGSTIN>
            <VEHNO>{51}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{3N1}</ZNUM1>
            <ZNUM2>{3N2}</ZNUM2>
            <ZNUM3>{3N3}</ZNUM3>
            <ZNUM4>{3N4}</ZNUM4>
            <ZNUM5>{3N5}</ZNUM5>
            <ZCHAR2>{3C2}</ZCHAR2>
            <ZCHAR3>{3C3}</ZCHAR3>
            <ZCHAR4>{3C4}</ZCHAR4>
            <ZCHAR5>{3C5}</ZCHAR5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
             <IVNUM>{52}</IVNUM>
            <IVDAT>{53}</IVDAT>
            <LIFNR>{54}</LIFNR>
            <MATNR>{55}</MATNR>
            <ZSHOP>{56}</ZSHOP>
            <EBELN>{57}</EBELN>
            <IVQTY>{58}</IVQTY>
            <ZAIVAMT>{59}</ZAIVAMT>
            <ZANETPR>{60}</ZANETPR>
            <ZANETWR>{61}</ZANETWR>
            <ZCGST>{62}</ZCGST>
            <ZSGST>{63}</ZSGST>
            <ZIGST>{64}</ZIGST>
            <ZUGST>{65}</ZUGST>
            <ZATCS>{TCS4}</ZATCS>
            <ZHSNSAC>{66}</ZHSNSAC>
            <ZGSTIN>{67}</ZGSTIN>
            <VEHNO>{68}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{4N1}</ZNUM1>
            <ZNUM2>{4N2}</ZNUM2>
            <ZNUM3>{4N3}</ZNUM3>
            <ZNUM4>{4N4}</ZNUM4>
            <ZNUM5>{4N5}</ZNUM5>
            <ZCHAR2>{4C2}</ZCHAR2>
            <ZCHAR3>{4C3}</ZCHAR3>
            <ZCHAR4>{4C4}</ZCHAR4>
            <ZCHAR5>{4C5}</ZCHAR5>
          </TAB_DATA_HEADER>
        <TAB_DATA_HEADER>
             <IVNUM>{69}</IVNUM>
            <IVDAT>{70}</IVDAT>
            <LIFNR>{71}</LIFNR>
            <MATNR>{72}</MATNR>
            <ZSHOP>{73}</ZSHOP>
            <EBELN>{74}</EBELN>
            <IVQTY>{75}</IVQTY>
            <ZAIVAMT>{76}</ZAIVAMT>
            <ZANETPR>{77}</ZANETPR>
            <ZANETWR>{78}</ZANETWR>
            <ZCGST>{79}</ZCGST>
            <ZSGST>{80}</ZSGST>
            <ZIGST>{81}</ZIGST>
            <ZUGST>{82}</ZUGST>
            <ZATCS>{TCS5}</ZATCS>
            <ZHSNSAC>{83}</ZHSNSAC>
            <ZGSTIN>{84}</ZGSTIN>
            <VEHNO>{85}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{5N1}</ZNUM1>
            <ZNUM2>{5N2}</ZNUM2>
            <ZNUM3>{5N3}</ZNUM3>
            <ZNUM4>{5N4}</ZNUM4>
            <ZNUM5>{5N5}</ZNUM5>
            <ZCHAR2>{5C2}</ZCHAR2>
            <ZCHAR3>{5C3}</ZCHAR3>
            <ZCHAR4>{5C4}</ZCHAR4>
            <ZCHAR5>{5C5}</ZCHAR5>
          </TAB_DATA_HEADER>
         </GET_DATA>
      </data01>
    </getData>
                              </soap:Body>
                            </soap:Envelope>";
                    }
                    else if (count_rec == 6)
                    {
                        soapStr =
                        @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                              <soap:Body>
                              <getData xmlns=""{APISOAP}"">
      <HEXADECIMAL>{0}</HEXADECIMAL>
      <data01>
        <GET_DATA>
         <TAB_DATA_HEADER>
            <IVNUM>{1}</IVNUM>
            <IVDAT>{2}</IVDAT>
            <LIFNR>{3}</LIFNR>
            <MATNR>{4}</MATNR>
            <ZSHOP>{5}</ZSHOP>
            <EBELN>{6}</EBELN>
            <IVQTY>{7}</IVQTY>
            <ZAIVAMT>{8}</ZAIVAMT>
            <ZANETPR>{9}</ZANETPR>
            <ZANETWR>{10}</ZANETWR>
            <ZCGST>{11}</ZCGST>
            <ZSGST>{12}</ZSGST>
            <ZIGST>{13}</ZIGST>
            <ZUGST>{14}</ZUGST>
            <ZATCS>{TCS1}</ZATCS>
            <ZHSNSAC>{15}</ZHSNSAC>
            <ZGSTIN>{16}</ZGSTIN>
            <VEHNO>{17}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{1N1}</ZNUM1>
            <ZNUM2>{1N2}</ZNUM2>
            <ZNUM3>{1N3}</ZNUM3>
            <ZNUM4>{1N4}</ZNUM4>
            <ZNUM5>{1N5}</ZNUM5>
            <ZCHAR2>{1C2}</ZCHAR2>
            <ZCHAR3>{1C3}</ZCHAR3>
            <ZCHAR4>{1C4}</ZCHAR4>
            <ZCHAR5>{1C5}</ZCHAR5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
             <IVNUM>{18}</IVNUM>
            <IVDAT>{19}</IVDAT>
            <LIFNR>{20}</LIFNR>
            <MATNR>{21}</MATNR>
            <ZSHOP>{22}</ZSHOP>
            <EBELN>{23}</EBELN>
            <IVQTY>{24}</IVQTY>
            <ZAIVAMT>{25}</ZAIVAMT>
            <ZANETPR>{26}</ZANETPR>
            <ZANETWR>{27}</ZANETWR>
            <ZCGST>{28}</ZCGST>
            <ZSGST>{29}</ZSGST>
            <ZIGST>{30}</ZIGST>
            <ZUGST>{31}</ZUGST>
            <ZATCS>{TCS2}</ZATCS>
            <ZHSNSAC>{32}</ZHSNSAC>
            <ZGSTIN>{33}</ZGSTIN>
            <VEHNO>{34}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{2N1}</ZNUM1>
            <ZNUM2>{2N2}</ZNUM2>
            <ZNUM3>{2N3}</ZNUM3>
            <ZNUM4>{2N4}</ZNUM4>
            <ZNUM5>{2N5}</ZNUM5>
            <ZCHAR2>{2C2}</ZCHAR2>
            <ZCHAR3>{2C3}</ZCHAR3>
            <ZCHAR4>{2C4}</ZCHAR4>
            <ZCHAR5>{2C5}</ZCHAR5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
             <IVNUM>{35}</IVNUM>
            <IVDAT>{36}</IVDAT>
            <LIFNR>{37}</LIFNR>
            <MATNR>{38}</MATNR>
            <ZSHOP>{39}</ZSHOP>
            <EBELN>{40}</EBELN>
            <IVQTY>{41}</IVQTY>
            <ZAIVAMT>{42}</ZAIVAMT>
            <ZANETPR>{43}</ZANETPR>
            <ZANETWR>{44}</ZANETWR>
            <ZCGST>{45}</ZCGST>
            <ZSGST>{46}</ZSGST>
            <ZIGST>{47}</ZIGST>
            <ZUGST>{48}</ZUGST>
            <ZATCS>{TCS3}</ZATCS>
            <ZHSNSAC>{49}</ZHSNSAC>
            <ZGSTIN>{50}</ZGSTIN>
            <VEHNO>{51}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{3N1}</ZNUM1>
            <ZNUM2>{3N2}</ZNUM2>
            <ZNUM3>{3N3}</ZNUM3>
            <ZNUM4>{3N4}</ZNUM4>
            <ZNUM5>{3N5}</ZNUM5>
            <ZCHAR2>{3C2}</ZCHAR2>
            <ZCHAR3>{3C3}</ZCHAR3>
            <ZCHAR4>{3C4}</ZCHAR4>
            <ZCHAR5>{3C5}</ZCHAR5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
             <IVNUM>{52}</IVNUM>
            <IVDAT>{53}</IVDAT>
            <LIFNR>{54}</LIFNR>
            <MATNR>{55}</MATNR>
            <ZSHOP>{56}</ZSHOP>
            <EBELN>{57}</EBELN>
            <IVQTY>{58}</IVQTY>
            <ZAIVAMT>{59}</ZAIVAMT>
            <ZANETPR>{60}</ZANETPR>
            <ZANETWR>{61}</ZANETWR>
            <ZCGST>{62}</ZCGST>
            <ZSGST>{63}</ZSGST>
            <ZIGST>{64}</ZIGST>
            <ZUGST>{65}</ZUGST>
            <ZATCS>{TCS4}</ZATCS>
            <ZHSNSAC>{66}</ZHSNSAC>
            <ZGSTIN>{67}</ZGSTIN>
            <VEHNO>{68}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{4N1}</ZNUM1>
            <ZNUM2>{4N2}</ZNUM2>
            <ZNUM3>{4N3}</ZNUM3>
            <ZNUM4>{4N4}</ZNUM4>
            <ZNUM5>{4N5}</ZNUM5>
            <ZCHAR2>{4C2}</ZCHAR2>
            <ZCHAR3>{4C3}</ZCHAR3>
            <ZCHAR4>{4C4}</ZCHAR4>
            <ZCHAR5>{4C5}</ZCHAR5>
          </TAB_DATA_HEADER>
        <TAB_DATA_HEADER>
             <IVNUM>{69}</IVNUM>
            <IVDAT>{70}</IVDAT>
            <LIFNR>{71}</LIFNR>
            <MATNR>{72}</MATNR>
            <ZSHOP>{73}</ZSHOP>
            <EBELN>{74}</EBELN>
            <IVQTY>{75}</IVQTY>
            <ZAIVAMT>{76}</ZAIVAMT>
            <ZANETPR>{77}</ZANETPR>
            <ZANETWR>{78}</ZANETWR>
            <ZCGST>{79}</ZCGST>
            <ZSGST>{80}</ZSGST>
            <ZIGST>{81}</ZIGST>
            <ZUGST>{82}</ZUGST>
            <ZATCS>{TCS5}</ZATCS>
            <ZHSNSAC>{83}</ZHSNSAC>
            <ZGSTIN>{84}</ZGSTIN>
            <VEHNO>{85}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{5N1}</ZNUM1>
            <ZNUM2>{5N2}</ZNUM2>
            <ZNUM3>{5N3}</ZNUM3>
            <ZNUM4>{5N4}</ZNUM4>
            <ZNUM5>{5N5}</ZNUM5>
            <ZCHAR2>{5C2}</ZCHAR2>
            <ZCHAR3>{5C3}</ZCHAR3>
            <ZCHAR4>{5C4}</ZCHAR4>
            <ZCHAR5>{5C5}</ZCHAR5>
          </TAB_DATA_HEADER>
<TAB_DATA_HEADER>
             <IVNUM>{69}</IVNUM>
            <IVDAT>{70}</IVDAT>
            <LIFNR>{71}</LIFNR>
            <MATNR>{72}</MATNR>
            <ZSHOP>{73}</ZSHOP>
            <EBELN>{74}</EBELN>
            <IVQTY>{75}</IVQTY>
            <ZAIVAMT>{76}</ZAIVAMT>
            <ZANETPR>{77}</ZANETPR>
            <ZANETWR>{78}</ZANETWR>
            <ZCGST>{79}</ZCGST>
            <ZSGST>{80}</ZSGST>
            <ZIGST>{81}</ZIGST>
            <ZUGST>{82}</ZUGST>
            <ZATCS>{TCS5}</ZATCS>
            <ZHSNSAC>{83}</ZHSNSAC>
            <ZGSTIN>{84}</ZGSTIN>
            <VEHNO>{85}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{5N1}</ZNUM1>
            <ZNUM2>{5N2}</ZNUM2>
            <ZNUM3>{5N3}</ZNUM3>
            <ZNUM4>{5N4}</ZNUM4>
            <ZNUM5>{5N5}</ZNUM5>
            <ZCHAR2>{5C2}</ZCHAR2>
            <ZCHAR3>{5C3}</ZCHAR3>
            <ZCHAR4>{5C4}</ZCHAR4>
            <ZCHAR5>{5C5}</ZCHAR5>
          </TAB_DATA_HEADER>
<TAB_DATA_HEADER>
             <IVNUM>{86}</IVNUM>
            <IVDAT>{87}</IVDAT>
            <LIFNR>{88}</LIFNR>
            <MATNR>{89}</MATNR>
            <ZSHOP>{90}</ZSHOP>
            <EBELN>{91}</EBELN>
            <IVQTY>{92}</IVQTY>
            <ZAIVAMT>{93}</ZAIVAMT>
            <ZANETPR>{94}</ZANETPR>
            <ZANETWR>{95}</ZANETWR>
            <ZCGST>{96}</ZCGST>
            <ZSGST>{97}</ZSGST>
            <ZIGST>{98}</ZIGST>
            <ZUGST>{99}</ZUGST>
            <ZATCS>{TCS6}</ZATCS>
            <ZHSNSAC>{100}</ZHSNSAC>
            <ZGSTIN>{101}</ZGSTIN>
            <VEHNO>{102}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{6N1}</ZNUM1>
            <ZNUM2>{6N2}</ZNUM2>
            <ZNUM3>{6N3}</ZNUM3>
            <ZNUM4>{6N4}</ZNUM4>
            <ZNUM5>{6N5}</ZNUM5>
            <ZCHAR2>{6C2}</ZCHAR2>
            <ZCHAR3>{6C3}</ZCHAR3>
            <ZCHAR4>{6C4}</ZCHAR4>
            <ZCHAR5>{6C5}</ZCHAR5>
          </TAB_DATA_HEADER>
         </GET_DATA>
      </data01>
    </getData>
                              </soap:Body>
                            </soap:Envelope>";
                    }
                    else if (count_rec == 7)
                    {
                        soapStr =
                        @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                              <soap:Body>
                              <getData xmlns=""{APISOAP}"">
      <HEXADECIMAL>{0}</HEXADECIMAL>
      <data01>
        <GET_DATA>
        <TAB_DATA_HEADER>
            <IVNUM>{1}</IVNUM>
            <IVDAT>{2}</IVDAT>
            <LIFNR>{3}</LIFNR>
            <MATNR>{4}</MATNR>
            <ZSHOP>{5}</ZSHOP>
            <EBELN>{6}</EBELN>
            <IVQTY>{7}</IVQTY>
            <ZAIVAMT>{8}</ZAIVAMT>
            <ZANETPR>{9}</ZANETPR>
            <ZANETWR>{10}</ZANETWR>
            <ZCGST>{11}</ZCGST>
            <ZSGST>{12}</ZSGST>
            <ZIGST>{13}</ZIGST>
            <ZUGST>{14}</ZUGST>
            <ZATCS>{TCS1}</ZATCS>
            <ZHSNSAC>{15}</ZHSNSAC>
            <ZGSTIN>{16}</ZGSTIN>
            <VEHNO>{17}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{1N1}</ZNUM1>
            <ZNUM2>{1N2}</ZNUM2>
            <ZNUM3>{1N3}</ZNUM3>
            <ZNUM4>{1N4}</ZNUM4>
            <ZNUM5>{1N5}</ZNUM5>
            <ZCHAR2>{1C2}</ZCHAR2>
            <ZCHAR3>{1C3}</ZCHAR3>
            <ZCHAR4>{1C4}</ZCHAR4>
            <ZCHAR5>{1C5}</ZCHAR5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
             <IVNUM>{18}</IVNUM>
            <IVDAT>{19}</IVDAT>
            <LIFNR>{20}</LIFNR>
            <MATNR>{21}</MATNR>
            <ZSHOP>{22}</ZSHOP>
            <EBELN>{23}</EBELN>
            <IVQTY>{24}</IVQTY>
            <ZAIVAMT>{25}</ZAIVAMT>
            <ZANETPR>{26}</ZANETPR>
            <ZANETWR>{27}</ZANETWR>
            <ZCGST>{28}</ZCGST>
            <ZSGST>{29}</ZSGST>
            <ZIGST>{30}</ZIGST>
            <ZUGST>{31}</ZUGST>
            <ZATCS>{TCS2}</ZATCS>
            <ZHSNSAC>{32}</ZHSNSAC>
            <ZGSTIN>{33}</ZGSTIN>
            <VEHNO>{34}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{2N1}</ZNUM1>
            <ZNUM2>{2N2}</ZNUM2>
            <ZNUM3>{2N3}</ZNUM3>
            <ZNUM4>{2N4}</ZNUM4>
            <ZNUM5>{2N5}</ZNUM5>
            <ZCHAR2>{2C2}</ZCHAR2>
            <ZCHAR3>{2C3}</ZCHAR3>
            <ZCHAR4>{2C4}</ZCHAR4>
            <ZCHAR5>{2C5}</ZCHAR5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
             <IVNUM>{35}</IVNUM>
            <IVDAT>{36}</IVDAT>
            <LIFNR>{37}</LIFNR>
            <MATNR>{38}</MATNR>
            <ZSHOP>{39}</ZSHOP>
            <EBELN>{40}</EBELN>
            <IVQTY>{41}</IVQTY>
            <ZAIVAMT>{42}</ZAIVAMT>
            <ZANETPR>{43}</ZANETPR>
            <ZANETWR>{44}</ZANETWR>
            <ZCGST>{45}</ZCGST>
            <ZSGST>{46}</ZSGST>
            <ZIGST>{47}</ZIGST>
            <ZUGST>{48}</ZUGST>
            <ZATCS>{TCS3}</ZATCS>
            <ZHSNSAC>{49}</ZHSNSAC>
            <ZGSTIN>{50}</ZGSTIN>
            <VEHNO>{51}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{3N1}</ZNUM1>
            <ZNUM2>{3N2}</ZNUM2>
            <ZNUM3>{3N3}</ZNUM3>
            <ZNUM4>{3N4}</ZNUM4>
            <ZNUM5>{3N5}</ZNUM5>
            <ZCHAR2>{3C2}</ZCHAR2>
            <ZCHAR3>{3C3}</ZCHAR3>
            <ZCHAR4>{3C4}</ZCHAR4>
            <ZCHAR5>{3C5}</ZCHAR5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
             <IVNUM>{52}</IVNUM>
            <IVDAT>{53}</IVDAT>
            <LIFNR>{54}</LIFNR>
            <MATNR>{55}</MATNR>
            <ZSHOP>{56}</ZSHOP>
            <EBELN>{57}</EBELN>
            <IVQTY>{58}</IVQTY>
            <ZAIVAMT>{59}</ZAIVAMT>
            <ZANETPR>{60}</ZANETPR>
            <ZANETWR>{61}</ZANETWR>
            <ZCGST>{62}</ZCGST>
            <ZSGST>{63}</ZSGST>
            <ZIGST>{64}</ZIGST>
            <ZUGST>{65}</ZUGST>
            <ZATCS>{TCS4}</ZATCS>
            <ZHSNSAC>{66}</ZHSNSAC>
            <ZGSTIN>{67}</ZGSTIN>
            <VEHNO>{68}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{4N1}</ZNUM1>
            <ZNUM2>{4N2}</ZNUM2>
            <ZNUM3>{4N3}</ZNUM3>
            <ZNUM4>{4N4}</ZNUM4>
            <ZNUM5>{4N5}</ZNUM5>
            <ZCHAR2>{4C2}</ZCHAR2>
            <ZCHAR3>{4C3}</ZCHAR3>
            <ZCHAR4>{4C4}</ZCHAR4>
            <ZCHAR5>{4C5}</ZCHAR5>
          </TAB_DATA_HEADER>
        <TAB_DATA_HEADER>
             <IVNUM>{69}</IVNUM>
            <IVDAT>{70}</IVDAT>
            <LIFNR>{71}</LIFNR>
            <MATNR>{72}</MATNR>
            <ZSHOP>{73}</ZSHOP>
            <EBELN>{74}</EBELN>
            <IVQTY>{75}</IVQTY>
            <ZAIVAMT>{76}</ZAIVAMT>
            <ZANETPR>{77}</ZANETPR>
            <ZANETWR>{78}</ZANETWR>
            <ZCGST>{79}</ZCGST>
            <ZSGST>{80}</ZSGST>
            <ZIGST>{81}</ZIGST>
            <ZUGST>{82}</ZUGST>
            <ZATCS>{TCS5}</ZATCS>
            <ZHSNSAC>{83}</ZHSNSAC>
            <ZGSTIN>{84}</ZGSTIN>
            <VEHNO>{85}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{5N1}</ZNUM1>
            <ZNUM2>{5N2}</ZNUM2>
            <ZNUM3>{5N3}</ZNUM3>
            <ZNUM4>{5N4}</ZNUM4>
            <ZNUM5>{5N5}</ZNUM5>
            <ZCHAR2>{5C2}</ZCHAR2>
            <ZCHAR3>{5C3}</ZCHAR3>
            <ZCHAR4>{5C4}</ZCHAR4>
            <ZCHAR5>{5C5}</ZCHAR5>
          </TAB_DATA_HEADER>
<TAB_DATA_HEADER>
             <IVNUM>{69}</IVNUM>
            <IVDAT>{70}</IVDAT>
            <LIFNR>{71}</LIFNR>
            <MATNR>{72}</MATNR>
            <ZSHOP>{73}</ZSHOP>
            <EBELN>{74}</EBELN>
            <IVQTY>{75}</IVQTY>
            <ZAIVAMT>{76}</ZAIVAMT>
            <ZANETPR>{77}</ZANETPR>
            <ZANETWR>{78}</ZANETWR>
            <ZCGST>{79}</ZCGST>
            <ZSGST>{80}</ZSGST>
            <ZIGST>{81}</ZIGST>
            <ZUGST>{82}</ZUGST>
            <ZATCS>{TCS5}</ZATCS>
            <ZHSNSAC>{83}</ZHSNSAC>
            <ZGSTIN>{84}</ZGSTIN>
            <VEHNO>{85}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{5N1}</ZNUM1>
            <ZNUM2>{5N2}</ZNUM2>
            <ZNUM3>{5N3}</ZNUM3>
            <ZNUM4>{5N4}</ZNUM4>
            <ZNUM5>{5N5}</ZNUM5>
            <ZCHAR2>{5C2}</ZCHAR2>
            <ZCHAR3>{5C3}</ZCHAR3>
            <ZCHAR4>{5C4}</ZCHAR4>
            <ZCHAR5>{5C5}</ZCHAR5>
          </TAB_DATA_HEADER>
<TAB_DATA_HEADER>
             <IVNUM>{86}</IVNUM>
            <IVDAT>{87}</IVDAT>
            <LIFNR>{88}</LIFNR>
            <MATNR>{89}</MATNR>
            <ZSHOP>{90}</ZSHOP>
            <EBELN>{91}</EBELN>
            <IVQTY>{92}</IVQTY>
            <ZAIVAMT>{93}</ZAIVAMT>
            <ZANETPR>{94}</ZANETPR>
            <ZANETWR>{95}</ZANETWR>
            <ZCGST>{96}</ZCGST>
            <ZSGST>{97}</ZSGST>
            <ZIGST>{98}</ZIGST>
            <ZUGST>{99}</ZUGST>
            <ZATCS>{TCS6}</ZATCS>
            <ZHSNSAC>{100}</ZHSNSAC>
            <ZGSTIN>{101}</ZGSTIN>
            <VEHNO>{102}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{6N1}</ZNUM1>
            <ZNUM2>{6N2}</ZNUM2>
            <ZNUM3>{6N3}</ZNUM3>
            <ZNUM4>{6N4}</ZNUM4>
            <ZNUM5>{6N5}</ZNUM5>
            <ZCHAR2>{6C2}</ZCHAR2>
            <ZCHAR3>{6C3}</ZCHAR3>
            <ZCHAR4>{6C4}</ZCHAR4>
            <ZCHAR5>{6C5}</ZCHAR5>
          </TAB_DATA_HEADER>
<TAB_DATA_HEADER>
             <IVNUM>{103}</IVNUM>
            <IVDAT>{104}</IVDAT>
            <LIFNR>{105}</LIFNR>
            <MATNR>{106}</MATNR>
            <ZSHOP>{107}</ZSHOP>
            <EBELN>{108}</EBELN>
            <IVQTY>{109}</IVQTY>
            <ZAIVAMT>{110}</ZAIVAMT>
            <ZANETPR>{111}</ZANETPR>
            <ZANETWR>{112}</ZANETWR>
            <ZCGST>{113}</ZCGST>
            <ZSGST>{114}</ZSGST>
            <ZIGST>{115}</ZIGST>
            <ZUGST>{116}</ZUGST>
            <ZATCS>{TCS7}</ZATCS>
            <ZHSNSAC>{117}</ZHSNSAC>
            <ZGSTIN>{118}</ZGSTIN>
            <VEHNO>{119}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{7N1}</ZNUM1>
            <ZNUM2>{7N2}</ZNUM2>
            <ZNUM3>{7N3}</ZNUM3>
            <ZNUM4>{7N4}</ZNUM4>
            <ZNUM5>{7N5}</ZNUM5>
            <ZCHAR2>{7C2}</ZCHAR2>
            <ZCHAR3>{7C3}</ZCHAR3>
            <ZCHAR4>{7C4}</ZCHAR4>
            <ZCHAR5>{7C5}</ZCHAR5>
          </TAB_DATA_HEADER>
         </GET_DATA>
      </data01>
    </getData>
                              </soap:Body>
                            </soap:Envelope>";
                    }
                    else if (count_rec == 8)
                    {
                        soapStr =
                        @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                              <soap:Body>
                              <getData xmlns=""{APISOAP}"">
      <HEXADECIMAL>{0}</HEXADECIMAL>
      <data01>
        <GET_DATA>
        <TAB_DATA_HEADER>
            <IVNUM>{1}</IVNUM>
            <IVDAT>{2}</IVDAT>
            <LIFNR>{3}</LIFNR>
            <MATNR>{4}</MATNR>
            <ZSHOP>{5}</ZSHOP>
            <EBELN>{6}</EBELN>
            <IVQTY>{7}</IVQTY>
            <ZAIVAMT>{8}</ZAIVAMT>
            <ZANETPR>{9}</ZANETPR>
            <ZANETWR>{10}</ZANETWR>
            <ZCGST>{11}</ZCGST>
            <ZSGST>{12}</ZSGST>
            <ZIGST>{13}</ZIGST>
            <ZUGST>{14}</ZUGST>
            <ZATCS>{TCS1}</ZATCS>
            <ZHSNSAC>{15}</ZHSNSAC>
            <ZGSTIN>{16}</ZGSTIN>
            <VEHNO>{17}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{1N1}</ZNUM1>
            <ZNUM2>{1N2}</ZNUM2>
            <ZNUM3>{1N3}</ZNUM3>
            <ZNUM4>{1N4}</ZNUM4>
            <ZNUM5>{1N5}</ZNUM5>
            <ZCHAR2>{1C2}</ZCHAR2>
            <ZCHAR3>{1C3}</ZCHAR3>
            <ZCHAR4>{1C4}</ZCHAR4>
            <ZCHAR5>{1C5}</ZCHAR5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
             <IVNUM>{18}</IVNUM>
            <IVDAT>{19}</IVDAT>
            <LIFNR>{20}</LIFNR>
            <MATNR>{21}</MATNR>
            <ZSHOP>{22}</ZSHOP>
            <EBELN>{23}</EBELN>
            <IVQTY>{24}</IVQTY>
            <ZAIVAMT>{25}</ZAIVAMT>
            <ZANETPR>{26}</ZANETPR>
            <ZANETWR>{27}</ZANETWR>
            <ZCGST>{28}</ZCGST>
            <ZSGST>{29}</ZSGST>
            <ZIGST>{30}</ZIGST>
            <ZUGST>{31}</ZUGST>
            <ZATCS>{TCS2}</ZATCS>
            <ZHSNSAC>{32}</ZHSNSAC>
            <ZGSTIN>{33}</ZGSTIN>
            <VEHNO>{34}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{2N1}</ZNUM1>
            <ZNUM2>{2N2}</ZNUM2>
            <ZNUM3>{2N3}</ZNUM3>
            <ZNUM4>{2N4}</ZNUM4>
            <ZNUM5>{2N5}</ZNUM5>
            <ZCHAR2>{2C2}</ZCHAR2>
            <ZCHAR3>{2C3}</ZCHAR3>
            <ZCHAR4>{2C4}</ZCHAR4>
            <ZCHAR5>{2C5}</ZCHAR5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
             <IVNUM>{35}</IVNUM>
            <IVDAT>{36}</IVDAT>
            <LIFNR>{37}</LIFNR>
            <MATNR>{38}</MATNR>
            <ZSHOP>{39}</ZSHOP>
            <EBELN>{40}</EBELN>
            <IVQTY>{41}</IVQTY>
            <ZAIVAMT>{42}</ZAIVAMT>
            <ZANETPR>{43}</ZANETPR>
            <ZANETWR>{44}</ZANETWR>
            <ZCGST>{45}</ZCGST>
            <ZSGST>{46}</ZSGST>
            <ZIGST>{47}</ZIGST>
            <ZUGST>{48}</ZUGST>
            <ZATCS>{TCS3}</ZATCS>
            <ZHSNSAC>{49}</ZHSNSAC>
            <ZGSTIN>{50}</ZGSTIN>
            <VEHNO>{51}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{3N1}</ZNUM1>
            <ZNUM2>{3N2}</ZNUM2>
            <ZNUM3>{3N3}</ZNUM3>
            <ZNUM4>{3N4}</ZNUM4>
            <ZNUM5>{3N5}</ZNUM5>
            <ZCHAR2>{3C2}</ZCHAR2>
            <ZCHAR3>{3C3}</ZCHAR3>
            <ZCHAR4>{3C4}</ZCHAR4>
            <ZCHAR5>{3C5}</ZCHAR5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
             <IVNUM>{52}</IVNUM>
            <IVDAT>{53}</IVDAT>
            <LIFNR>{54}</LIFNR>
            <MATNR>{55}</MATNR>
            <ZSHOP>{56}</ZSHOP>
            <EBELN>{57}</EBELN>
            <IVQTY>{58}</IVQTY>
            <ZAIVAMT>{59}</ZAIVAMT>
            <ZANETPR>{60}</ZANETPR>
            <ZANETWR>{61}</ZANETWR>
            <ZCGST>{62}</ZCGST>
            <ZSGST>{63}</ZSGST>
            <ZIGST>{64}</ZIGST>
            <ZUGST>{65}</ZUGST>
            <ZATCS>{TCS4}</ZATCS>
            <ZHSNSAC>{66}</ZHSNSAC>
            <ZGSTIN>{67}</ZGSTIN>
            <VEHNO>{68}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{4N1}</ZNUM1>
            <ZNUM2>{4N2}</ZNUM2>
            <ZNUM3>{4N3}</ZNUM3>
            <ZNUM4>{4N4}</ZNUM4>
            <ZNUM5>{4N5}</ZNUM5>
            <ZCHAR2>{4C2}</ZCHAR2>
            <ZCHAR3>{4C3}</ZCHAR3>
            <ZCHAR4>{4C4}</ZCHAR4>
            <ZCHAR5>{4C5}</ZCHAR5>
          </TAB_DATA_HEADER>
        <TAB_DATA_HEADER>
             <IVNUM>{69}</IVNUM>
            <IVDAT>{70}</IVDAT>
            <LIFNR>{71}</LIFNR>
            <MATNR>{72}</MATNR>
            <ZSHOP>{73}</ZSHOP>
            <EBELN>{74}</EBELN>
            <IVQTY>{75}</IVQTY>
            <ZAIVAMT>{76}</ZAIVAMT>
            <ZANETPR>{77}</ZANETPR>
            <ZANETWR>{78}</ZANETWR>
            <ZCGST>{79}</ZCGST>
            <ZSGST>{80}</ZSGST>
            <ZIGST>{81}</ZIGST>
            <ZUGST>{82}</ZUGST>
            <ZATCS>{TCS5}</ZATCS>
            <ZHSNSAC>{83}</ZHSNSAC>
            <ZGSTIN>{84}</ZGSTIN>
            <VEHNO>{85}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{5N1}</ZNUM1>
            <ZNUM2>{5N2}</ZNUM2>
            <ZNUM3>{5N3}</ZNUM3>
            <ZNUM4>{5N4}</ZNUM4>
            <ZNUM5>{5N5}</ZNUM5>
            <ZCHAR2>{5C2}</ZCHAR2>
            <ZCHAR3>{5C3}</ZCHAR3>
            <ZCHAR4>{5C4}</ZCHAR4>
            <ZCHAR5>{5C5}</ZCHAR5>
          </TAB_DATA_HEADER>
<TAB_DATA_HEADER>
             <IVNUM>{69}</IVNUM>
            <IVDAT>{70}</IVDAT>
            <LIFNR>{71}</LIFNR>
            <MATNR>{72}</MATNR>
            <ZSHOP>{73}</ZSHOP>
            <EBELN>{74}</EBELN>
            <IVQTY>{75}</IVQTY>
            <ZAIVAMT>{76}</ZAIVAMT>
            <ZANETPR>{77}</ZANETPR>
            <ZANETWR>{78}</ZANETWR>
            <ZCGST>{79}</ZCGST>
            <ZSGST>{80}</ZSGST>
            <ZIGST>{81}</ZIGST>
            <ZUGST>{82}</ZUGST>
            <ZATCS>{TCS5}</ZATCS>
            <ZHSNSAC>{83}</ZHSNSAC>
            <ZGSTIN>{84}</ZGSTIN>
            <VEHNO>{85}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{5N1}</ZNUM1>
            <ZNUM2>{5N2}</ZNUM2>
            <ZNUM3>{5N3}</ZNUM3>
            <ZNUM4>{5N4}</ZNUM4>
            <ZNUM5>{5N5}</ZNUM5>
            <ZCHAR2>{5C2}</ZCHAR2>
            <ZCHAR3>{5C3}</ZCHAR3>
            <ZCHAR4>{5C4}</ZCHAR4>
            <ZCHAR5>{5C5}</ZCHAR5>
          </TAB_DATA_HEADER>
<TAB_DATA_HEADER>
             <IVNUM>{86}</IVNUM>
            <IVDAT>{87}</IVDAT>
            <LIFNR>{88}</LIFNR>
            <MATNR>{89}</MATNR>
            <ZSHOP>{90}</ZSHOP>
            <EBELN>{91}</EBELN>
            <IVQTY>{92}</IVQTY>
            <ZAIVAMT>{93}</ZAIVAMT>
            <ZANETPR>{94}</ZANETPR>
            <ZANETWR>{95}</ZANETWR>
            <ZCGST>{96}</ZCGST>
            <ZSGST>{97}</ZSGST>
            <ZIGST>{98}</ZIGST>
            <ZUGST>{99}</ZUGST>
            <ZATCS>{TCS6}</ZATCS>
            <ZHSNSAC>{100}</ZHSNSAC>
            <ZGSTIN>{101}</ZGSTIN>
            <VEHNO>{102}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{6N1}</ZNUM1>
            <ZNUM2>{6N2}</ZNUM2>
            <ZNUM3>{6N3}</ZNUM3>
            <ZNUM4>{6N4}</ZNUM4>
            <ZNUM5>{6N5}</ZNUM5>
            <ZCHAR2>{6C2}</ZCHAR2>
            <ZCHAR3>{6C3}</ZCHAR3>
            <ZCHAR4>{6C4}</ZCHAR4>
            <ZCHAR5>{6C5}</ZCHAR5>
          </TAB_DATA_HEADER>
<TAB_DATA_HEADER>
             <IVNUM>{103}</IVNUM>
            <IVDAT>{104}</IVDAT>
            <LIFNR>{105}</LIFNR>
            <MATNR>{106}</MATNR>
            <ZSHOP>{107}</ZSHOP>
            <EBELN>{108}</EBELN>
            <IVQTY>{109}</IVQTY>
            <ZAIVAMT>{110}</ZAIVAMT>
            <ZANETPR>{111}</ZANETPR>
            <ZANETWR>{112}</ZANETWR>
            <ZCGST>{113}</ZCGST>
            <ZSGST>{114}</ZSGST>
            <ZIGST>{115}</ZIGST>
            <ZUGST>{116}</ZUGST>
            <ZATCS>{TCS7}</ZATCS>
            <ZHSNSAC>{117}</ZHSNSAC>
            <ZGSTIN>{118}</ZGSTIN>
            <VEHNO>{119}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{7N1}</ZNUM1>
            <ZNUM2>{7N2}</ZNUM2>
            <ZNUM3>{7N3}</ZNUM3>
            <ZNUM4>{7N4}</ZNUM4>
            <ZNUM5>{7N5}</ZNUM5>
            <ZCHAR2>{7C2}</ZCHAR2>
            <ZCHAR3>{7C3}</ZCHAR3>
            <ZCHAR4>{7C4}</ZCHAR4>
            <ZCHAR5>{7C5}</ZCHAR5>
          </TAB_DATA_HEADER>
<TAB_DATA_HEADER>
             <IVNUM>{120}</IVNUM>
            <IVDAT>{121}</IVDAT>
            <LIFNR>{122}</LIFNR>
            <MATNR>{123}</MATNR>
            <ZSHOP>{124}</ZSHOP>
            <EBELN>{125}</EBELN>
            <IVQTY>{126}</IVQTY>
            <ZAIVAMT>{127}</ZAIVAMT>
            <ZANETPR>{128}</ZANETPR>
            <ZANETWR>{129}</ZANETWR>
            <ZCGST>{130}</ZCGST>
            <ZSGST>{131}</ZSGST>
            <ZIGST>{132}</ZIGST>
            <ZUGST>{133}</ZUGST>
            <ZATCS>{TCS8}</ZATCS>
            <ZHSNSAC>{134}</ZHSNSAC>
            <ZGSTIN>{135}</ZGSTIN>
            <VEHNO>{136}</VEHNO>
            <IRN>{EINVNO}</IRN>
            <EWAYBILL>{EWAY}</EWAYBILL>
            <ZNUM1>{8N1}</ZNUM1>
            <ZNUM2>{8N2}</ZNUM2>
            <ZNUM3>{8N3}</ZNUM3>
            <ZNUM4>{8N4}</ZNUM4>
            <ZNUM5>{8N5}</ZNUM5>
            <ZCHAR2>{8C2}</ZCHAR2>
            <ZCHAR3>{8C3}</ZCHAR3>
            <ZCHAR4>{8C4}</ZCHAR4>
            <ZCHAR5>{8C5}</ZCHAR5>
          </TAB_DATA_HEADER>
         </GET_DATA>
      </data01>
    </getData>
                              </soap:Body>
                            </soap:Envelope>";
                    }

                }
                if (count_rec > 0)
                {
                    soapStr = soapStr.Replace("{0}", hexvalue.ToString().Trim());
                    soapStr = soapStr.Replace("{APISOAP}", Global.APIPosturl);
                    objDSMModelData = new DSMModelData();
                    var invData = objDSMModelData.GetInvoiceDetailsByInvoiceNumber(Global.APIInvoiceNumber);
                    string cgst = "0";
                    string sgst = "0";
                    string tcs = "0";
                    bool valid = Validate(invData);
                    if (valid)
                    {
                        int w_counter = 0;
                        for (int i = 0; i < invData.Count; i++)
                        {
                            //MessageBox.Show(SInvoiceNo.VendorCode.ToString().Trim());
                            w_counter++;
                            if (w_counter == 1)
                            {
                                double sgsttaxamt = (Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()) * 14) / 100;

                                double Totaltaxamt = sgsttaxamt * 2;

                                double NetValue = Totaltaxamt + Convert.ToDouble(invData[i].AssessableValue.ToString().Trim());

                                double cgsttaxamt = Math.Round(sgsttaxamt, 2);
                              

                                soapStr = soapStr.Replace("{1}", invData[i].InvNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{2}", invData[i].InvDate.Value.ToString("yyyyMMdd").Trim());
                                soapStr = soapStr.Replace("{3}", invData[i].VendorCode == null ? "" : invData[i].VendorCode);
                                soapStr = soapStr.Replace("{4}", invData[i].PartNumber.ToString().Replace("-", "").Trim());
                                soapStr = soapStr.Replace("{5}", invData[i].ShopCode.ToString().Trim());
                                soapStr = soapStr.Replace("{6}", invData[i].PONumber.ToString().Trim());

                                soapStr = soapStr.Replace("{7}", invData[i].InvQuantity.ToString().Trim());
                                soapStr = soapStr.Replace("{8}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].InvValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{9}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].UnitPrice.ToString().Trim()), 3)));

                                soapStr = soapStr.Replace("{10}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                if (invData[i].CGST.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    cgst = "0";
                                }
                                else
                                {
                                    cgst = invData[i].CGST.ToString().Trim();

                                }


                                soapStr = soapStr.Replace("{11}", String.Format("{0:0.00}",cgst.ToString().Trim()));
                                if (invData[i].SGST.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    sgst = "0";
                                }
                                else
                                {
                                    sgst = invData[i].SGST.ToString().Trim();

                                }

                                soapStr = soapStr.Replace("{12}", String.Format("{0:0.00}", sgst.ToString().Trim()));

                                soapStr = soapStr.Replace("{13}", String.Format("{0:0.00}", invData[i].IGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{14}", "0");
                                soapStr = soapStr.Replace("{15}", invData[i].TarrifNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{16}", invData[i].GSTN.ToString().Trim());
                                soapStr = soapStr.Replace("{17}", invData[i].VehicleNumber == null ? "" : invData[i].VehicleNumber.ToString().Trim());
                                if (invData[i].TCS.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    tcs = "0";
                                }
                                else
                                {
                                    tcs = invData[i].TCS.ToString().Trim();

                                }

                                soapStr = soapStr.Replace("{TCS1}", tcs.ToString().Trim());
                                soapStr = soapStr.Replace("{LOT}", invData[i].Lot_Code.ToString().Trim());
                                soapStr = soapStr.Replace("{EWAY}", invData[i].Ewaybill_no.ToString().Trim());
                                soapStr = soapStr.Replace("{EINVNO}", invData[i].E_InvNo.ToString().Trim());
                                soapStr = soapStr.Replace("{1N1}", invData[i].EXTRA_NUM_1.ToString().Trim());
                                soapStr = soapStr.Replace("{1N2}", invData[i].EXTRA_NUM_2.ToString().Trim());
                                soapStr = soapStr.Replace("{1N3}", invData[i].EXTRA_NUM_3.ToString().Trim());
                                soapStr = soapStr.Replace("{1N4}", invData[i].EXTRA_NUM_4.ToString().Trim());
                                soapStr = soapStr.Replace("{1N5}", invData[i].EXTRA_NUM_5.ToString().Trim());
                                soapStr = soapStr.Replace("{1N6}", invData[i].EXTRA_NUM_6.ToString().Trim());
                                soapStr = soapStr.Replace("{1N7}", invData[i].EXTRA_NUM_7.ToString().Trim());
                                soapStr = soapStr.Replace("{1C1}", invData[i].EXTRA_CHAR_1.ToString().Trim());
                                soapStr = soapStr.Replace("{1C2}", invData[i].EXTRA_CHAR_2.ToString().Trim());
                                soapStr = soapStr.Replace("{1C3}", invData[i].EXTRA_CHAR_3.ToString().Trim());
                                soapStr = soapStr.Replace("{1C4}", invData[i].EXTRA_CHAR_4.ToString().Trim());
                                soapStr = soapStr.Replace("{1C5}", invData[i].EXTRA_CHAR_5.ToString().Trim());
                            }

                            if (w_counter == 2)
                            {
                                double sgsttaxamt = (Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()) * 14) / 100;

                                double Totaltaxamt = sgsttaxamt * 2;

                                double NetValue = Totaltaxamt + Convert.ToDouble(invData[i].AssessableValue.ToString().Trim());

                                double cgsttaxamt = Math.Round(sgsttaxamt, 2);

                                soapStr = soapStr.Replace("{18}", invData[i].InvNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{19}", invData[i].InvDate.Value.ToString("yyyyMMdd").Trim());
                                soapStr = soapStr.Replace("{20}", invData[i].VendorCode == null ? "" : invData[i].VendorCode);
                                soapStr = soapStr.Replace("{21}", invData[i].PartNumber.ToString().Replace("-", "").Trim());
                                soapStr = soapStr.Replace("{22}", invData[i].ShopCode.ToString().Trim());
                                soapStr = soapStr.Replace("{23}", invData[i].PONumber.ToString().Trim());

                                soapStr = soapStr.Replace("{24}", invData[i].InvQuantity.ToString().Trim());
                                soapStr = soapStr.Replace("{25}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].InvValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{26}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].UnitPrice.ToString().Trim()), 3)));

                                soapStr = soapStr.Replace("{27}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                if (invData[i].CGST.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    cgst = "0";
                                }
                                else
                                {
                                    cgst = invData[i].CGST.ToString().Trim();

                                }





                                soapStr = soapStr.Replace("{28}", String.Format("{0:0.00}", cgst.ToString().Trim()));

                                //soapStr = soapStr.Replace("{28}", String.Format("{0:0.00}", invData[i].CGST.ToString().Trim()));
                                if (invData[i].SGST.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    sgst = "0";
                                }
                                else
                                {
                                    sgst = invData[i].SGST.ToString().Trim();

                                }

                                soapStr = soapStr.Replace("{29}", String.Format("{0:0.00}", sgst.ToString().Trim()));

                                //soapStr = soapStr.Replace("{29}", String.Format("{0:0.00}", invData[i].SGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{30}", String.Format("{0:0.00}", invData[i].IGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{31}", "0");
                                soapStr = soapStr.Replace("{32}", invData[i].TarrifNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{33}", invData[i].GSTN.ToString().Trim());
                                soapStr = soapStr.Replace("{34}", invData[i].VehicleNumber == null ? "" : invData[i].VehicleNumber.ToString().Trim());
                                if (invData[i].TCS.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    tcs = "0";
                                }
                                else
                                {
                                    tcs = invData[i].TCS.ToString().Trim();

                                }

                                soapStr = soapStr.Replace("{TCS2}", tcs.ToString().Trim());
                                //soapStr = soapStr.Replace("{TCS2}", invData[i].TCS.ToString().Trim());
                                soapStr = soapStr.Replace("{LOT}", invData[i].Lot_Code.ToString().Trim());
                                soapStr = soapStr.Replace("{EWAY}", invData[i].Ewaybill_no.ToString().Trim());
                                soapStr = soapStr.Replace("{EINVNO}", invData[i].E_InvNo.ToString().Trim());
                                soapStr = soapStr.Replace("{2N1}", invData[i].EXTRA_NUM_1.ToString().Trim());
                                soapStr = soapStr.Replace("{2N2}", invData[i].EXTRA_NUM_2.ToString().Trim());
                                soapStr = soapStr.Replace("{2N3}", invData[i].EXTRA_NUM_3.ToString().Trim());
                                soapStr = soapStr.Replace("{2N4}", invData[i].EXTRA_NUM_4.ToString().Trim());
                                soapStr = soapStr.Replace("{2N5}", invData[i].EXTRA_NUM_5.ToString().Trim());
                                soapStr = soapStr.Replace("{2N6}", invData[i].EXTRA_NUM_6.ToString().Trim());
                                soapStr = soapStr.Replace("{2N7}", invData[i].EXTRA_NUM_7.ToString().Trim());
                                soapStr = soapStr.Replace("{2C1}", invData[i].EXTRA_CHAR_1.ToString().Trim());
                                soapStr = soapStr.Replace("{2C2}", invData[i].EXTRA_CHAR_2.ToString().Trim());
                                soapStr = soapStr.Replace("{2C3}", invData[i].EXTRA_CHAR_3.ToString().Trim());
                                soapStr = soapStr.Replace("{2C4}", invData[i].EXTRA_CHAR_4.ToString().Trim());
                                soapStr = soapStr.Replace("{2C5}", invData[i].EXTRA_CHAR_5.ToString().Trim());
                            }

                            if (w_counter == 3)
                            {
                                double sgsttaxamt = (Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()) * 14) / 100;

                                double Totaltaxamt = sgsttaxamt * 2;

                                double NetValue = Totaltaxamt + Convert.ToDouble(invData[i].AssessableValue.ToString().Trim());

                                double cgsttaxamt = Math.Round(sgsttaxamt, 2);


                                soapStr = soapStr.Replace("{35}", invData[i].InvNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{36}", invData[i].InvDate.Value.ToString("yyyyMMdd").Trim());
                                soapStr = soapStr.Replace("{37}", invData[i].VendorCode == null ? "" : invData[i].VendorCode);
                                soapStr = soapStr.Replace("{38}", invData[i].PartNumber.ToString().Replace("-", "").Trim());
                                soapStr = soapStr.Replace("{39}", invData[i].ShopCode.ToString().Trim());
                                soapStr = soapStr.Replace("{40}", invData[i].PONumber.ToString().Trim());

                                soapStr = soapStr.Replace("{41}", invData[i].InvQuantity.ToString().Trim());
                                soapStr = soapStr.Replace("{42}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].InvValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{43}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].UnitPrice.ToString().Trim()), 3)));

                                soapStr = soapStr.Replace("{44}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                if (invData[i].CGST.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    cgst = "0";
                                }
                                else
                                {
                                    cgst = invData[i].CGST.ToString().Trim();

                                }





                                soapStr = soapStr.Replace("{45}", String.Format("{0:0.00}", cgst.ToString().Trim()));

                                //soapStr = soapStr.Replace("{45}", String.Format("{0:0.00}", invData[i].CGST.ToString().Trim()));
                                //soapStr = soapStr.Replace("{46}", String.Format("{0:0.00}", invData[i].SGST.ToString().Trim()));
                                if (invData[i].SGST.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    sgst = "0";
                                }
                                else
                                {
                                    sgst = invData[i].SGST.ToString().Trim();

                                }

                                soapStr = soapStr.Replace("{46}", String.Format("{0:0.00}", sgst.ToString().Trim()));

                                soapStr = soapStr.Replace("{47}", String.Format("{0:0.00}", invData[i].IGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{48}", "0");
                                soapStr = soapStr.Replace("{49}", invData[i].TarrifNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{50}", invData[i].GSTN.ToString().Trim());
                                soapStr = soapStr.Replace("{51}", invData[i].VehicleNumber == null ? "" : invData[i].VehicleNumber.ToString().Trim());
                                if (invData[i].TCS.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    tcs = "0";
                                }
                                else
                                {
                                    tcs = invData[i].TCS.ToString().Trim();

                                }

                                soapStr = soapStr.Replace("{TCS3}", tcs.ToString().Trim());
                                //soapStr = soapStr.Replace("{TCS3}", invData[i].TCS.ToString().Trim());
                                soapStr = soapStr.Replace("{LOT}", invData[i].Lot_Code.ToString().Trim());
                                soapStr = soapStr.Replace("{EWAY}", invData[i].Ewaybill_no.ToString().Trim());
                                soapStr = soapStr.Replace("{EINVNO}", invData[i].E_InvNo.ToString().Trim());
                                soapStr = soapStr.Replace("{3N1}", invData[i].EXTRA_NUM_1.ToString().Trim());
                                soapStr = soapStr.Replace("{3N2}", invData[i].EXTRA_NUM_2.ToString().Trim());
                                soapStr = soapStr.Replace("{3N3}", invData[i].EXTRA_NUM_3.ToString().Trim());
                                soapStr = soapStr.Replace("{3N4}", invData[i].EXTRA_NUM_4.ToString().Trim());
                                soapStr = soapStr.Replace("{3N5}", invData[i].EXTRA_NUM_5.ToString().Trim());
                                soapStr = soapStr.Replace("{3N6}", invData[i].EXTRA_NUM_6.ToString().Trim());
                                soapStr = soapStr.Replace("{3N7}", invData[i].EXTRA_NUM_7.ToString().Trim());
                                soapStr = soapStr.Replace("{3C1}", invData[i].EXTRA_CHAR_1.ToString().Trim());
                                soapStr = soapStr.Replace("{3C2}", invData[i].EXTRA_CHAR_2.ToString().Trim());
                                soapStr = soapStr.Replace("{3C3}", invData[i].EXTRA_CHAR_3.ToString().Trim());
                                soapStr = soapStr.Replace("{3C4}", invData[i].EXTRA_CHAR_4.ToString().Trim());
                                soapStr = soapStr.Replace("{3C5}", invData[i].EXTRA_CHAR_5.ToString().Trim());
                            }
                            if (w_counter == 4)
                            {
                                double sgsttaxamt = (Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()) * 14) / 100;

                                double Totaltaxamt = sgsttaxamt * 2;

                                double NetValue = Totaltaxamt + Convert.ToDouble(invData[i].AssessableValue.ToString().Trim());

                                double cgsttaxamt = Math.Round(sgsttaxamt, 2);


                                soapStr = soapStr.Replace("{52}", invData[i].InvNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{53}", invData[i].InvDate.Value.ToString("yyyyMMdd").Trim());
                                soapStr = soapStr.Replace("{54}", invData[i].VendorCode == null ? "" : invData[i].VendorCode);
                                soapStr = soapStr.Replace("{55}", invData[i].PartNumber.ToString().Replace("-", "").Trim());
                                soapStr = soapStr.Replace("{56}", invData[i].ShopCode.ToString().Trim());
                                soapStr = soapStr.Replace("{57}", invData[i].PONumber.ToString().Trim());

                                soapStr = soapStr.Replace("{58}", invData[i].InvQuantity.ToString().Trim());
                                soapStr = soapStr.Replace("{59}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].InvValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{60}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].UnitPrice.ToString().Trim()), 3)));

                                soapStr = soapStr.Replace("{61}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                if (invData[i].CGST.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    cgst = "0";
                                }
                                else
                                {
                                    cgst = invData[i].CGST.ToString().Trim();

                                }





                                soapStr = soapStr.Replace("{62}", String.Format("{0:0.00}", cgst.ToString().Trim()));
                                //soapStr = soapStr.Replace("{62}", String.Format("{0:0.00}", invData[i].CGST.ToString().Trim()));
                                if (invData[i].SGST.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    sgst = "0";
                                }
                                else
                                {
                                    sgst = invData[i].SGST.ToString().Trim();

                                }

                                soapStr = soapStr.Replace("{63}", String.Format("{0:0.00}", sgst.ToString().Trim()));

                                //soapStr = soapStr.Replace("{63}", String.Format("{0:0.00}", invData[i].SGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{64}", String.Format("{0:0.00}", invData[i].IGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{65}", "0");
                                soapStr = soapStr.Replace("{66}", invData[i].TarrifNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{67}", invData[i].GSTN.ToString().Trim());
                                soapStr = soapStr.Replace("{68}", invData[i].VehicleNumber == null ? "" : invData[i].VehicleNumber.ToString().Trim());
                                if (invData[i].TCS.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    tcs = "0";
                                }
                                else
                                {
                                    tcs = invData[i].TCS.ToString().Trim();

                                }

                                soapStr = soapStr.Replace("{TCS4}", tcs.ToString().Trim());
                               // soapStr = soapStr.Replace("{TCS4}", invData[i].TCS.ToString().Trim());
                                soapStr = soapStr.Replace("{LOT}", invData[i].Lot_Code.ToString().Trim());
                                soapStr = soapStr.Replace("{EWAY}", invData[i].Ewaybill_no.ToString().Trim());
                                soapStr = soapStr.Replace("{EINVNO}", invData[i].E_InvNo.ToString().Trim());
                                soapStr = soapStr.Replace("{4N1}", invData[i].EXTRA_NUM_1.ToString().Trim());
                                soapStr = soapStr.Replace("{4N2}", invData[i].EXTRA_NUM_2.ToString().Trim());
                                soapStr = soapStr.Replace("{4N3}", invData[i].EXTRA_NUM_3.ToString().Trim());
                                soapStr = soapStr.Replace("{4N4}", invData[i].EXTRA_NUM_4.ToString().Trim());
                                soapStr = soapStr.Replace("{4N5}", invData[i].EXTRA_NUM_5.ToString().Trim());
                                soapStr = soapStr.Replace("{4N6}", invData[i].EXTRA_NUM_6.ToString().Trim());
                                soapStr = soapStr.Replace("{4N7}", invData[i].EXTRA_NUM_7.ToString().Trim());
                                soapStr = soapStr.Replace("{4C1}", invData[i].EXTRA_CHAR_1.ToString().Trim());
                                soapStr = soapStr.Replace("{4C2}", invData[i].EXTRA_CHAR_2.ToString().Trim());
                                soapStr = soapStr.Replace("{4C3}", invData[i].EXTRA_CHAR_3.ToString().Trim());
                                soapStr = soapStr.Replace("{4C4}", invData[i].EXTRA_CHAR_4.ToString().Trim());
                                soapStr = soapStr.Replace("{4C5}", invData[i].EXTRA_CHAR_5.ToString().Trim());
                            }
                            if (w_counter == 5)
                            {
                                double sgsttaxamt = (Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()) * 14) / 100;

                                double Totaltaxamt = sgsttaxamt * 2;

                                double NetValue = Totaltaxamt + Convert.ToDouble(invData[i].AssessableValue.ToString().Trim());

                                double cgsttaxamt = Math.Round(sgsttaxamt, 2);

                                soapStr = soapStr.Replace("{69}", invData[i].InvNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{70}", invData[i].InvDate.Value.ToString("yyyyMMdd").Trim());
                                soapStr = soapStr.Replace("{71}", invData[i].VendorCode == null ? "" : invData[i].VendorCode);
                                soapStr = soapStr.Replace("{72}", invData[i].PartNumber.ToString().Replace("-", "").Trim());
                                soapStr = soapStr.Replace("{73}", invData[i].ShopCode.ToString().Trim());
                                soapStr = soapStr.Replace("{74}", invData[i].PONumber.ToString().Trim());

                                soapStr = soapStr.Replace("{75}", invData[i].InvQuantity.ToString().Trim());
                                soapStr = soapStr.Replace("{76}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].InvValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{77}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].UnitPrice.ToString().Trim()), 3)));

                                soapStr = soapStr.Replace("{78}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                if (invData[i].CGST.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    cgst = "0";
                                }
                                else
                                {
                                    cgst = invData[i].CGST.ToString().Trim();

                                }

                                soapStr = soapStr.Replace("{79}", String.Format("{0:0.00}", cgst.ToString().Trim()));

                                //soapStr = soapStr.Replace("{79}", String.Format("{0:0.00}", invData[i].CGST.ToString().Trim()));
                                if (invData[i].SGST.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    sgst = "0";
                                }
                                else
                                {
                                    sgst = invData[i].SGST.ToString().Trim();

                                }

                                soapStr = soapStr.Replace("{80}", String.Format("{0:0.00}", sgst.ToString().Trim()));

                                //soapStr = soapStr.Replace("{80}", String.Format("{0:0.00}", invData[i].SGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{81}", String.Format("{0:0.00}", invData[i].IGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{82}", "0");
                                soapStr = soapStr.Replace("{83}", invData[i].TarrifNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{84}", invData[i].GSTN.ToString().Trim());
                                soapStr = soapStr.Replace("{85}", invData[i].VehicleNumber == null ? "" : invData[i].VehicleNumber.ToString().Trim());
                                if (invData[i].TCS.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    tcs = "0";
                                }
                                else
                                {
                                    tcs = invData[i].TCS.ToString().Trim();

                                }

                                soapStr = soapStr.Replace("{TCS5}", tcs.ToString().Trim());
                                //soapStr = soapStr.Replace("{TCS5}", invData[i].TCS.ToString().Trim());
                                soapStr = soapStr.Replace("{LOT}", invData[i].Lot_Code.ToString().Trim());
                                soapStr = soapStr.Replace("{EWAY}", invData[i].Ewaybill_no.ToString().Trim());
                                soapStr = soapStr.Replace("{EINVNO}", invData[i].E_InvNo.ToString().Trim());
                                soapStr = soapStr.Replace("{5N1}", invData[i].EXTRA_NUM_1.ToString().Trim());
                                soapStr = soapStr.Replace("{5N2}", invData[i].EXTRA_NUM_2.ToString().Trim());
                                soapStr = soapStr.Replace("{5N3}", invData[i].EXTRA_NUM_3.ToString().Trim());
                                soapStr = soapStr.Replace("{5N4}", invData[i].EXTRA_NUM_4.ToString().Trim());
                                soapStr = soapStr.Replace("{5N5}", invData[i].EXTRA_NUM_5.ToString().Trim());
                                soapStr = soapStr.Replace("{5N6}", invData[i].EXTRA_NUM_6.ToString().Trim());
                                soapStr = soapStr.Replace("{5N7}", invData[i].EXTRA_NUM_7.ToString().Trim());
                                soapStr = soapStr.Replace("{5C1}", invData[i].EXTRA_CHAR_1.ToString().Trim());
                                soapStr = soapStr.Replace("{5C2}", invData[i].EXTRA_CHAR_2.ToString().Trim());
                                soapStr = soapStr.Replace("{5C3}", invData[i].EXTRA_CHAR_3.ToString().Trim());
                                soapStr = soapStr.Replace("{5C4}", invData[i].EXTRA_CHAR_4.ToString().Trim());
                                soapStr = soapStr.Replace("{5C5}", invData[i].EXTRA_CHAR_5.ToString().Trim());
                            }
                            if (w_counter == 6)
                            {
                                double sgsttaxamt = (Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()) * 14) / 100;

                                double Totaltaxamt = sgsttaxamt * 2;

                                double NetValue = Totaltaxamt + Convert.ToDouble(invData[i].AssessableValue.ToString().Trim());

                                double cgsttaxamt = Math.Round(sgsttaxamt, 2);

                                soapStr = soapStr.Replace("{86}", invData[i].InvNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{87}", invData[i].InvDate.Value.ToString("yyyyMMdd").Trim());
                                soapStr = soapStr.Replace("{88}", invData[i].VendorCode == null ? "" : invData[i].VendorCode);
                                soapStr = soapStr.Replace("{89}", invData[i].PartNumber.ToString().Replace("-", "").Trim());
                                soapStr = soapStr.Replace("{90}", invData[i].ShopCode.ToString().Trim());
                                soapStr = soapStr.Replace("{91}", invData[i].PONumber.ToString().Trim());

                                soapStr = soapStr.Replace("{92}", invData[i].InvQuantity.ToString().Trim());
                                soapStr = soapStr.Replace("{93}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].InvValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{94}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].UnitPrice.ToString().Trim()), 3)));

                                soapStr = soapStr.Replace("{95}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                if (invData[i].CGST.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    cgst = "0";
                                }
                                else
                                {
                                    cgst = invData[i].CGST.ToString().Trim();

                                }

                                soapStr = soapStr.Replace("{96}", String.Format("{0:0.00}", cgst.ToString().Trim()));
                                //soapStr = soapStr.Replace("{96}", String.Format("{0:0.00}", invData[i].CGST.ToString().Trim()));
                                //soapStr = soapStr.Replace("{97}", String.Format("{0:0.00}", invData[i].SGST.ToString().Trim()));
                                if (invData[i].SGST.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    sgst = "0";
                                }
                                else
                                {
                                    sgst = invData[i].SGST.ToString().Trim();

                                }

                                soapStr = soapStr.Replace("{97}", String.Format("{0:0.00}", sgst.ToString().Trim()));

                                soapStr = soapStr.Replace("{98}", String.Format("{0:0.00}", invData[i].IGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{99}", "0");
                                soapStr = soapStr.Replace("{100}", invData[i].TarrifNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{101}", invData[i].GSTN.ToString().Trim());
                                soapStr = soapStr.Replace("{102}", invData[i].VehicleNumber == null ? "" : invData[i].VehicleNumber.ToString().Trim());
                                if (invData[i].TCS.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    tcs = "0";
                                }
                                else
                                {
                                    tcs = invData[i].TCS.ToString().Trim();

                                }

                                soapStr = soapStr.Replace("{TCS6}", tcs.ToString().Trim());
                                //soapStr = soapStr.Replace("{TCS6}", invData[i].TCS.ToString().Trim());
                                soapStr = soapStr.Replace("{LOT}", invData[i].Lot_Code.ToString().Trim());
                                soapStr = soapStr.Replace("{EWAY}", invData[i].Ewaybill_no.ToString().Trim());
                                soapStr = soapStr.Replace("{EINVNO}", invData[i].E_InvNo.ToString().Trim());
                                soapStr = soapStr.Replace("{6N1}", invData[i].EXTRA_NUM_1.ToString().Trim());
                                soapStr = soapStr.Replace("{6N2}", invData[i].EXTRA_NUM_2.ToString().Trim());
                                soapStr = soapStr.Replace("{6N3}", invData[i].EXTRA_NUM_3.ToString().Trim());
                                soapStr = soapStr.Replace("{6N4}", invData[i].EXTRA_NUM_4.ToString().Trim());
                                soapStr = soapStr.Replace("{6N5}", invData[i].EXTRA_NUM_5.ToString().Trim());
                                soapStr = soapStr.Replace("{6N6}", invData[i].EXTRA_NUM_6.ToString().Trim());
                                soapStr = soapStr.Replace("{6N7}", invData[i].EXTRA_NUM_7.ToString().Trim());
                                soapStr = soapStr.Replace("{6C1}", invData[i].EXTRA_CHAR_1.ToString().Trim());
                                soapStr = soapStr.Replace("{6C2}", invData[i].EXTRA_CHAR_2.ToString().Trim());
                                soapStr = soapStr.Replace("{6C3}", invData[i].EXTRA_CHAR_3.ToString().Trim());
                                soapStr = soapStr.Replace("{6C4}", invData[i].EXTRA_CHAR_4.ToString().Trim());
                                soapStr = soapStr.Replace("{6C5}", invData[i].EXTRA_CHAR_5.ToString().Trim());
                            }
                            if (w_counter == 7)
                            {
                                double sgsttaxamt = (Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()) * 14) / 100;

                                double Totaltaxamt = sgsttaxamt * 2;

                                double NetValue = Totaltaxamt + Convert.ToDouble(invData[i].AssessableValue.ToString().Trim());

                                double cgsttaxamt = Math.Round(sgsttaxamt, 2);

                                soapStr = soapStr.Replace("{103}", invData[i].InvNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{104}", invData[i].InvDate.Value.ToString("yyyyMMdd").Trim());
                                soapStr = soapStr.Replace("{105}", invData[i].VendorCode == null ? "" : invData[i].VendorCode);
                                soapStr = soapStr.Replace("{106}", invData[i].PartNumber.ToString().Replace("-", "").Trim());
                                soapStr = soapStr.Replace("{107}", invData[i].ShopCode.ToString().Trim());
                                soapStr = soapStr.Replace("{108}", invData[i].PONumber.ToString().Trim());

                                soapStr = soapStr.Replace("{109}", invData[i].InvQuantity.ToString().Trim());
                                soapStr = soapStr.Replace("{110}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].InvValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{111}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].UnitPrice.ToString().Trim()), 3)));

                                soapStr = soapStr.Replace("{112}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                if (invData[i].CGST.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    cgst = "0";
                                }
                                else
                                {
                                    cgst = invData[i].CGST.ToString().Trim();

                                }

                                soapStr = soapStr.Replace("{113}", String.Format("{0:0.00}", cgst.ToString().Trim()));
                                //soapStr = soapStr.Replace("{113}", String.Format("{0:0.00}", invData[i].CGST.ToString().Trim()));
                                // soapStr = soapStr.Replace("{114}", String.Format("{0:0.00}", invData[i].SGST.ToString().Trim()));
                                if (invData[i].SGST.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    sgst = "0";
                                }
                                else
                                {
                                    sgst = invData[i].SGST.ToString().Trim();

                                }

                                soapStr = soapStr.Replace("{114}", String.Format("{0:0.00}", sgst.ToString().Trim()));

                                soapStr = soapStr.Replace("{115}", String.Format("{0:0.00}", invData[i].IGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{116}", "0");
                                soapStr = soapStr.Replace("{117}", invData[i].TarrifNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{118}", invData[i].GSTN.ToString().Trim());
                                soapStr = soapStr.Replace("{119}", invData[i].VehicleNumber == null ? "" : invData[i].VehicleNumber.ToString().Trim());
                                if (invData[i].TCS.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    tcs = "0";
                                }
                                else
                                {
                                    tcs = invData[i].TCS.ToString().Trim();

                                }

                                soapStr = soapStr.Replace("{TCS7}", tcs.ToString().Trim());
                                //soapStr = soapStr.Replace("{TCS7}", invData[i].TCS.ToString().Trim());
                                soapStr = soapStr.Replace("{LOT}", invData[i].Lot_Code.ToString().Trim());
                                soapStr = soapStr.Replace("{EWAY}", invData[i].Ewaybill_no.ToString().Trim());
                                soapStr = soapStr.Replace("{EINVNO}", invData[i].E_InvNo.ToString().Trim());
                                soapStr = soapStr.Replace("{7N1}", invData[i].EXTRA_NUM_1.ToString().Trim());
                                soapStr = soapStr.Replace("{7N2}", invData[i].EXTRA_NUM_2.ToString().Trim());
                                soapStr = soapStr.Replace("{7N3}", invData[i].EXTRA_NUM_3.ToString().Trim());
                                soapStr = soapStr.Replace("{7N4}", invData[i].EXTRA_NUM_4.ToString().Trim());
                                soapStr = soapStr.Replace("{7N5}", invData[i].EXTRA_NUM_5.ToString().Trim());
                                soapStr = soapStr.Replace("{7N6}", invData[i].EXTRA_NUM_6.ToString().Trim());
                                soapStr = soapStr.Replace("{7N7}", invData[i].EXTRA_NUM_7.ToString().Trim());
                                soapStr = soapStr.Replace("{7C1}", invData[i].EXTRA_CHAR_1.ToString().Trim());
                                soapStr = soapStr.Replace("{7C2}", invData[i].EXTRA_CHAR_2.ToString().Trim());
                                soapStr = soapStr.Replace("{7C3}", invData[i].EXTRA_CHAR_3.ToString().Trim());
                                soapStr = soapStr.Replace("{7C4}", invData[i].EXTRA_CHAR_4.ToString().Trim());
                                soapStr = soapStr.Replace("{7C5}", invData[i].EXTRA_CHAR_5.ToString().Trim());
                            }
                            if (w_counter == 8)
                            {
                                double sgsttaxamt = (Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()) * 14) / 100;

                                double Totaltaxamt = sgsttaxamt * 2;

                                double NetValue = Totaltaxamt + Convert.ToDouble(invData[i].AssessableValue.ToString().Trim());

                                double cgsttaxamt = Math.Round(sgsttaxamt, 2);

                                soapStr = soapStr.Replace("{120}", invData[i].InvNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{121}", invData[i].InvDate.Value.ToString("yyyyMMdd").Trim());
                                soapStr = soapStr.Replace("{122}", invData[i].VendorCode == null ? "" : invData[i].VendorCode);
                                soapStr = soapStr.Replace("{123}", invData[i].PartNumber.ToString().Replace("-", "").Trim());
                                soapStr = soapStr.Replace("{124}", invData[i].ShopCode.ToString().Trim());
                                soapStr = soapStr.Replace("{125}", invData[i].PONumber.ToString().Trim());

                                soapStr = soapStr.Replace("{126}", invData[i].InvQuantity.ToString().Trim());
                                soapStr = soapStr.Replace("{127}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].InvValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{128}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].UnitPrice.ToString().Trim()), 3)));

                                soapStr = soapStr.Replace("{129}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                if (invData[i].CGST.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    cgst = "0";
                                }
                                else
                                {
                                    cgst = invData[i].CGST.ToString().Trim();

                                }





                                soapStr = soapStr.Replace("{130}", String.Format("{0:0.00}", cgst.ToString().Trim()));
                                //soapStr = soapStr.Replace("{130}", String.Format("{0:0.00}", invData[i].CGST.ToString().Trim()));
                                //soapStr = soapStr.Replace("{131}", String.Format("{0:0.00}", invData[i].SGST.ToString().Trim()));
                                if (invData[i].SGST.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    sgst = "0";
                                }
                                else
                                {
                                    sgst = invData[i].SGST.ToString().Trim();

                                }

                                soapStr = soapStr.Replace("{131}", String.Format("{0:0.00}", sgst.ToString().Trim()));

                                soapStr = soapStr.Replace("{132}", String.Format("{0:0.00}", invData[i].IGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{133}", "0");
                                soapStr = soapStr.Replace("{134}", invData[i].TarrifNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{135}", invData[i].GSTN.ToString().Trim());
                                soapStr = soapStr.Replace("{136}", invData[i].VehicleNumber == null ? "" : invData[i].VehicleNumber.ToString().Trim());
                                if (invData[i].TCS.ToString().Trim() == DBNull.Value.ToString())
                                {
                                    tcs = "0";
                                }
                                else
                                {
                                    tcs = invData[i].TCS.ToString().Trim();

                                }

                                soapStr = soapStr.Replace("{TCS8}", tcs.ToString().Trim());
                                //soapStr = soapStr.Replace("{TCS8}", invData[i].TCS.ToString().Trim());
                                soapStr = soapStr.Replace("{LOT}", invData[i].Lot_Code.ToString().Trim());
                                soapStr = soapStr.Replace("{EWAY}", invData[i].Ewaybill_no.ToString().Trim());
                                soapStr = soapStr.Replace("{EINVNO}", invData[i].E_InvNo.ToString().Trim());
                                soapStr = soapStr.Replace("{8N1}", invData[i].EXTRA_NUM_1.ToString().Trim());
                                soapStr = soapStr.Replace("{8N2}", invData[i].EXTRA_NUM_2.ToString().Trim());
                                soapStr = soapStr.Replace("{8N3}", invData[i].EXTRA_NUM_3.ToString().Trim());
                                soapStr = soapStr.Replace("{8N4}", invData[i].EXTRA_NUM_4.ToString().Trim());
                                soapStr = soapStr.Replace("{8N5}", invData[i].EXTRA_NUM_5.ToString().Trim());
                                soapStr = soapStr.Replace("{8N6}", invData[i].EXTRA_NUM_6.ToString().Trim());
                                soapStr = soapStr.Replace("{8N7}", invData[i].EXTRA_NUM_7.ToString().Trim());
                                soapStr = soapStr.Replace("{8C1}", invData[i].EXTRA_CHAR_1.ToString().Trim());
                                soapStr = soapStr.Replace("{8C2}", invData[i].EXTRA_CHAR_2.ToString().Trim());
                                soapStr = soapStr.Replace("{8C3}", invData[i].EXTRA_CHAR_3.ToString().Trim());
                                soapStr = soapStr.Replace("{8C4}", invData[i].EXTRA_CHAR_4.ToString().Trim());
                                soapStr = soapStr.Replace("{8C5}", invData[i].EXTRA_CHAR_5.ToString().Trim());
                            }
                        }

                        soapStr = soapStr.Replace("{0}", hexvalue.ToString().Trim());
                        LogSoapFormat("Coming Soap Generation : ");
                        LogSoapFormat(soapStr);
                        // MessageBox.Show(soapStr);

                        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);
                        req.Headers.Add("SOAPAction", "http://Kmieai/DMDI/data/getData");
                        req.ProtocolVersion = HttpVersion.Version11;
                        req.ContentType = "text/xml;charset=\"utf-8\"";
                        req.Accept = "text/xml";
                        req.Host = "kininvoice.kiaindia.net";

                        req.KeepAlive = true;
                        req.Method = "POST";


                        //MessageBox.Show("URL : " + Url.ToString().Trim());
                        //MessageBox.Show("MethodName : " + methodName.ToString().Trim());
                        //LogSoapFormat(soapStr);

                        int hitCount = 0;
                    APIhit:
                        try
                        {
                            hitCount++;
                            using (Stream stm = req.GetRequestStream())
                            {
                                LogSoapFormat("Coming Request : ");
                                using (StreamWriter stmw = new StreamWriter(stm))
                                {
                                    stmw.Write(soapStr);
                                    stmw.Close();
                                }
                                //MessageBox.Show("After Getrequest");
                            }
                            using (StreamReader responseReader = new StreamReader(req.GetResponse().GetResponseStream()))
                            {
                                //MessageBox.Show("Coming Response");
                                string result = responseReader.ReadToEnd();
                                ResultXML = XDocument.Parse(result);
                                ResultString = result;

                                //MessageBox.Show("Result :" + ResultString.ToString().Trim());

                                XmlDocument xmlDoc = new XmlDocument();
                                //xmlDoc.LoadXml(strReturnStatus);
                                xmlDoc.LoadXml(ResultString);

                                XmlNamespaceManager xmlnsManager = new System.Xml.XmlNamespaceManager(xmlDoc.NameTable);

                                xmlnsManager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                                xmlnsManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                                xmlnsManager.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");
                                //xmlnsManager.AddNamespace("si", "http://example.com/SystemIntegration");
                                xmlnsManager.AddNamespace("si", Global.APIPosturl);


                                // You'd access the full path like this
                                //XmlNode node = xmlDoc.SelectSingleNode("/soap:Envelope/soap:Body/si:LoginResponse/si:FirstName", xmlnsManager);
                                //XmlNode node = xmlDoc.SelectSingleNode("/soap:Envelope/soap:Body/si:getDataResponse/si:getDataResult/si:GET_DATA_01/si:TAB_DATA/si:IFRESULT", xmlnsManager);

                                //string firstname = node.InnerText;
                                //string Result = node.InnerText;
                                // string Result = "";

                                int successcount = 0;
                                int errorcount = 0;
                                string failedMsg = "";

                                XmlNodeList xNodelst = xmlDoc.DocumentElement.SelectNodes("/soap:Envelope/soap:Body/si:getDataResponse/si:getDataResult/si:GET_DATA_01/si:TAB_DATA", xmlnsManager);
                                foreach (XmlNode xn in xNodelst)
                                {
                                    //string fbc = xn["ns1:FBC"].Attributes["Destination"].Value;
                                    string xmlResult = xn["IFRESULT"].InnerText;
                                    failedMsg = xn["IFFAILMSG"].InnerText;
                                    string InvoiceNo = xn["IVNUM"].InnerText;
                                    if (xmlResult.ToString().Trim() != "E")
                                    {
                                        successcount++;
                                        objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNo(InvoiceNo.ToString().Trim(), xmlResult.ToString().Trim(), failedMsg.ToString().Trim());
                                    }
                                    else
                                    {
                                        objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNo(InvoiceNo.ToString().Trim(), xmlResult.ToString().Trim(), failedMsg.ToString().Trim());

                                    }
                                    if (failedMsg.ToString().Trim() == "I/F Success, ASN Not cerated,Create Manual ASN")
                                    {
                                        successcount++;
                                        //MessageBox.Show("Invoice Successfully Posted");
                                    }
                                    else if (failedMsg.ToString().Trim() == "I/F Success, ASN Not created,Create Manual ASN")
                                    {
                                        successcount++;
                                        // MessageBox.Show("Invoice Successfully Posted");
                                    }
                                    else
                                    {
                                        errorcount++;
                                        //MessageBox.Show("Invoice Not Successfully Posted." + failedMsg.ToString().Trim());
                                    }

                                    break;
                                }


                                //MoveFile(Global.InvoicePath + Global.APIInvoiceNumber + ".pdf","PROCESSED");
                                if (Global.APIpostType == "MANUAL" || Global.UserType.ToUpper() == "ADMIN")
                                {
                                    if (errorcount > 0)
                                        MessageBox.Show("Invoice Not Successfully Posted." + failedMsg.ToString().Trim());
                                    else if (successcount > 0)
                                        MessageBox.Show("Invoice Successfully Posted");
                                }
                            }
                        }
                        catch (Exception exs)

                        {
                            LogAPIErrors(exs);
                            if (hitCount <= 3)
                            {
                                goto APIhit;
                            }
                            else
                            {
                                //objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNoFinYear(Global.APIInvoiceNumber, "MANUAL", exs.Message,Finyear);
                                objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNo(Global.APIInvoiceNumber, "MANUAL", exs.Message);

                                if (Global.APIpostType == "MANUAL" || Global.UserType == "admin")
                                {
                                    MessageBox.Show("API site cannot be reached");
                                    Global.APIInvoiceNumber = "Null";
                                }
                            }
                            if (Global.UserType.ToUpper() == "ADMIN")
                                MessageBox.Show(exs.Message);
                        }
                        //End Correct Code
                    }
                    else
                    {
                        //LogSoapFormat("API skipped due to wrong data: " + Global.APIInvoiceNumber);
                        DSMModelData objDSMModelData = new DSMModelData();
                        objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNo(Global.APIInvoiceNumber, "FALSE", msg);
                        if (Global.UserType.ToUpper() == "ADMIN")
                        {
                            MessageBox.Show(msg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogAPIErrors(ex);
                objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNo(Global.APIInvoiceNumber, "MANUAL", "Manual Posting");
                if (Global.APIpostType == "MANUAL")
                {
                    MessageBox.Show("API EXCEPTION: " + ex.ToString());
                }
            }
        }

        public bool Validate(List<Invoice> invData)
        {
            bool checkChar = true;
            string specialChar = @"\/|!#$%&/()=?`+»«@£§€{}-;'<>_,*^";
            foreach (var inv in invData)
            {

                foreach (var item in specialChar)
                {
                    if (inv.PartNumber != null)
                    {
                        if (inv.PartNumber.Contains(item))
                        {
                            checkChar = false;
                            msg = "Part No Required";
                            break;
                        }
                    }
                    if (inv.VehicleNumber == null || inv.VehicleNumber == "")
                    {
                        checkChar = false;
                        msg = "Vehicle No Required";
                        break;
                    }
                    else
                    {
                        if (inv.VehicleNumber.Trim() == "")
                        {
                            checkChar = false;
                            msg = "Invalid Vehicle No ";
                            break;
                        }
                    }
                    if (inv.InvNumber != null)
                    {
                        if (inv.InvNumber.Contains(item))
                        {
                            checkChar = false;
                            msg = "Invoice No Required";
                            break;
                        }
                    }
                    if (inv.Ewaybill_no != null)
                    {
                        if (inv.Ewaybill_no.Contains(item))
                        {
                            checkChar = false;
                            msg = "Ewaybill No Required";
                            break;
                        }
                    }
                    if (inv.PONumber != null)
                    {
                        if (inv.PONumber.Contains(item))
                        {
                            checkChar = false;
                            msg = "Invalid PO No";
                            break;
                        }
                    }
                    if (inv.CGST != null)
                    {
                        if (inv.CGST.Contains(item))
                        {
                            checkChar = false;
                            msg = "Invalid CGST";

                            break;
                        }
                    }
                    if (inv.IGST != null)
                    {
                        if (inv.IGST.Contains(item))
                        {
                            checkChar = false;
                            msg = "Invalid IGST";
                            break;
                        }
                    }
                    if (inv.InvQuantity != null)
                    {
                        if (inv.InvQuantity.Contains(item))
                        {
                            checkChar = false;
                            msg = "Invalid Invoice Qty";
                            break;
                        }
                    }
                    if (inv.InvValue != null)
                    {
                        if (inv.InvValue.Contains(item))
                        {
                            checkChar = false;
                            msg = "Invalid Invoice Value";
                            break;
                        }
                    }
                    if (inv.SGST != null)
                    {
                        if (inv.SGST.Contains(item))
                        {
                            checkChar = false;
                            msg = "Invalid SGST";
                            break;
                        }
                    }
                    if (inv.GSTN != null)
                    {
                        if (inv.GSTN.Contains(item))
                        {
                            checkChar = false;
                            msg = "Invalid GSTN";
                            break;
                        }
                    }
                    if (inv.UnitPrice != null)
                    {
                        if (inv.UnitPrice.Contains(item))
                        {
                            checkChar = false;
                            msg = "Invalid Unit Price";
                            break;
                        }
                    }
                    if (inv.AssessableValue != null)
                    {
                        if (inv.AssessableValue.Contains(item))
                        {
                            checkChar = false;
                            msg = "Invalid Assesable Value";
                            break;
                        }
                    }
                    if (inv.TarrifNumber != null)
                    {
                        if (inv.TarrifNumber.Contains(item))
                        {
                            checkChar = false;
                            msg = "Invalid Tarrif Number";
                            break;
                        }
                    }
                    if (inv.BedAmount != null)
                    {
                        if (inv.BedAmount.Contains(item))
                        {
                            checkChar = false;
                            msg = "Invalid Bed Amount";
                            break;
                        }
                    }
                    if (inv.VatAmount != null)
                    {

                        if (inv.VatAmount.Contains(item))
                        {
                            checkChar = false;
                            msg = "Invalid VAT Amount";
                            break;
                        }
                    }
                    if (inv.MaterialCost != null)
                    {
                        if (inv.MaterialCost.Contains(item))
                        {
                            checkChar = false;
                            msg = "Invalid Material Cost";
                            break;
                        }
                    }
                    if (inv.ConsigneeMatlCost != null)
                    {
                        if (inv.ConsigneeMatlCost.Contains(item))
                        {
                            checkChar = false;
                            msg = "Invalid Consignee Material Cost";
                            break;
                        }
                    }
                    if (inv.ConsigneePartCost != null)
                    {
                        if (inv.ConsigneePartCost.Contains(item))
                        {
                            checkChar = false;
                            msg = "Invalid Consignee Part Cost";
                            break;
                        }
                    }
                    if (inv.ExciseDutyCost != null)
                    {
                        if (inv.ExciseDutyCost.Contains(item))
                        {
                            checkChar = false;
                            msg = "Invalid Excise Duty Cost";
                            break;
                        }
                    }
                    if (inv.CSTAmount != null)
                    {
                        if (inv.CSTAmount.Contains(item))
                        {
                            checkChar = false;
                            msg = "Invalid CST Amount";
                            break;
                        }
                    }
                    if (inv.ToolCost != null)
                    {
                        if (inv.ToolCost.Contains(item))
                        {
                            checkChar = false;
                            msg = "Invalid Tool Cost";
                            break;
                        }
                    }
                }
            }
            return checkChar;
        }

        public void LogSoapFormat(string soapStr)
        {
            StringBuilder sb = new StringBuilder();
            string path = AppDomain.CurrentDomain.BaseDirectory + "LogSoapXML.txt";
            // flush every 20 seconds as you do it

            sb.AppendLine();
            sb.Append("Log Time: " + DateTime.Now);
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();
            if (soapStr != null)
            {
                sb.Append(soapStr);
            }
            sb.AppendLine();
            sb.Append("--------------------------------------------------");

            File.AppendAllText(path, sb.ToString());
            sb.Clear();
        }

        public void LogAPIErrors(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            string path = AppDomain.CurrentDomain.BaseDirectory + "LogError.txt";

            sb.AppendLine();
            sb.Append("Log Time: " + DateTime.Now);
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();
            if (ex != null)
            {
                sb.Append("Exception: " + ex.ToString());
                if (ex.InnerException != null)
                {
                    sb.Append("Inner Exception: " + ex.InnerException.ToString());
                }
            }
            sb.AppendLine();
            sb.Append("--------------------------------------------------");

            File.AppendAllText(path, sb.ToString());
            sb.Clear();
        }

        public void MoveFile(string source, string location)
        {

            System.IO.StreamReader fileMove = new System.IO.StreamReader(source);
            string _SourceFile = source;

            //check folder
            if (!System.IO.Directory.Exists(Global.InvoicePath + "\\" + location + "\\"))
            {
                System.IO.Directory.CreateDirectory(Global.InvoicePath + "\\" + location + "\\");
            }
            string _DestFile = Global.InvoicePath + "\\" + location + "\\" + System.IO.Path.GetFileName(source);
            fileMove.Close();
            fileMove.Dispose();

            //Check File
            if (!System.IO.File.Exists(_DestFile))
            {
                File.Move(_SourceFile, _DestFile);
                System.IO.Directory.CreateDirectory(Global.InvoicePath + "\\" + location + "\\");
            }
            else
            {
                System.IO.File.Delete(_DestFile);
                File.Move(_SourceFile, _DestFile);
            }
        }


        /// <summary>
        /// This method should be called before each Invoke().
        /// </summary>
        internal void PreInvoke()
        {
            CleanLastInvoke();
            InitialCursorState = Cursor.Current;
            //Cursor.Current = Cursor.WaitCursor;
            Cursor.Current = Cursors.WaitCursor;
            // feel free to add more instructions to this method
        }

        /// <summary>
        /// This method should be called after each (successful or unsuccessful) Invoke().
        /// </summary>
        internal void PosInvoke()
        {
            Cursor.Current = InitialCursorState;
            // feel free to add more instructions to this method
        }

        #endregion


    }
}

