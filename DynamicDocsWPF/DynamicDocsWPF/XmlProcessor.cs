using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XML_Test
{
    class Program
    {
        static void Main(string[] args)
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
                                Console.WriteLine("Start Element {0}", reader.Name);
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
