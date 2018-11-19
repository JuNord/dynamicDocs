using System;
using System.CodeDom;
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
                                    string name = reader.GetAttribute("name");
                                    if(name!=null)
                                        Console.WriteLine("\tname: " + name );
                                   
                                    string description = reader.GetAttribute("description");
                                    if(description!=null)
                                        Console.WriteLine("\tdescription: " + description );

                                    
                                    string target = reader.GetAttribute("target");
                                    if(target!=null)
                                        Console.WriteLine("\ttarget: "+target);
                                         
                                    string locks = reader.GetAttribute("locks");
                                    if(locks!=null)
                                        Console.WriteLine("\tlocks: "+locks);
                                    
                                    string vText = reader.GetAttribute("text");
                                    if(vText!=null)
                                        Console.WriteLine("\ttext: "+vText);
                                    
                                    string draftname = reader.GetAttribute("draftname");
                                    if(draftname!=null)
                                        Console.WriteLine("\tdraftname: "+draftname);
                                    
                                    string filepath = reader.GetAttribute("filepath");
                                    if(filepath!=null)
                                        Console.WriteLine("\tfilepath: "+filepath);

                                }
                                else
                                {
                                    Console.WriteLine("Attributes of <" + reader.Name + ">");
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