using System;
using System.Xml;

namespace XmlProcessor
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // Create an XML reader for this file.
            using (XmlReader reader = XmlReader.Create(@"C:\Users\Julius.Nordhues\source\repos\dynamicDocs\XML_Test\XMLFile1.xml"))
            {
                while (reader.Read())
                {
                    // Only detect start elements.
                    if (reader.IsStartElement())
                    {
                        // Get element name and switch on it.
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.HasAttributes)
                                {
                                    Console.WriteLine("Attributes of <" + reader.Name + ">");
                                    string tmp = reader.GetAttribute("name");
                                    Console.WriteLine("\tname: " + tmp );
                                   
                                    string tmp1 = reader.GetAttribute("description");
                                    Console.WriteLine("\tdescription: " + tmp1 );
     
                                    
                                }
                                // Move the reader back to the element node.
                                    //reader.MoveToElement(); 

                                break;
                            
                            case XmlNodeType.EndElement:
                                Console.WriteLine("End Element {0}", reader.Name);
                                break;
                            default:
                                Console.WriteLine("Other node {0} with value {1}",
                                    reader.NodeType, reader.Value);
                                break;
                        }

                    }
                }
            }
            Console.ReadKey();

        }
    }
}