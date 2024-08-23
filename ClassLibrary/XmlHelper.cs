using System;
using System.Xml;

namespace ClassLibrary
{
    public class XmlHelper
    {
        public static XmlAttributeCollection SelectSingleNode(string xmlData, string nodePath)
        {
            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                xmlDoc.LoadXml(xmlData);
            
                XmlNode settingNode = xmlDoc.SelectSingleNode(nodePath);

                if (settingNode != null && settingNode.Attributes != null)
                {
                    return settingNode.Attributes;
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Error SelectSingleNode loading XML: " + e.Message);
            }
            return null;
        }


        public static XmlNodeList SelectNodes(string filePath, string nodePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
        
            try
            {
                xmlDoc.Load(filePath);
        
                XmlNodeList nodeList = xmlDoc.SelectNodes(nodePath);

                return nodeList;
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Error SelectNodes loading XML: " + e.Message);
            }
            return null;
        }
    
        public static XmlNodeList SelectNodesByString(string str, string nodePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
        
            try
            {
                xmlDoc.LoadXml(str);
        
                XmlNodeList nodeList = xmlDoc.SelectNodes(nodePath);

                return nodeList;
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Error SelectNodes loading XML: " + e.Message);
            }
            return null;
        }
    }
}