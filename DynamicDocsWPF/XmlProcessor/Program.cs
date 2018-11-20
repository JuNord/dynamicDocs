using System;
using System.CodeDom;
using System.Runtime.InteropServices;
using System.Security.Permissions;
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
                while(reader.Read())
                {
                    

                        // Get element name and switch on it.
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                ///
                                /// Überprüft ob die Node (Zeile) Attribute außer dem Namen aufweißt
                                /// 
                                if (reader.HasAttributes)
                                {
                                    //Node Name wird rausgeschrieben z.B. <teacher-dropdown>
                                    Console.WriteLine("<" + reader.Name + ">");
                                    
                                    var name = reader.GetAttribute("name");
                                    if(name!=null)
                                        Console.WriteLine("\tname: " + name );
                                   
                                    
                                    var description = reader.GetAttribute("description");
                                    if(description!=null)
                                        Console.WriteLine("\tdescription: " + description );

                                    
                                    var target = reader.GetAttribute("target");
                                    if(target!=null)
                                        Console.WriteLine("\ttarget: "+target);
                                         
                                    var locks = reader.GetAttribute("locks");
                                    if(locks!=null)
                                        Console.WriteLine("\tlocks: "+locks);
                                    
                                    var vText = reader.GetAttribute("text");
                                    if(vText!=null)
                                        Console.WriteLine("\ttext: "+vText);
                                    
                                    var draftname = reader.GetAttribute("draftname");
                                    if(draftname!=null)
                                        Console.WriteLine("\tdraftname: "+draftname);
                                    
                                    var filepath = reader.GetAttribute("filepath");
                                    if(filepath!=null)
                                        Console.WriteLine("\tfilepath: "+filepath);
                                }
                                else
                                {
                                    Console.WriteLine("<" + reader.Name + ">");
                                }
                                // Move the reader back to the element node.
                                    //reader.MoveToElement(); 

                                break;
                            
                            case XmlNodeType.EndElement:
                                Console.WriteLine("</"+reader.Name+">");
                                break;
                            
                            
                            default:
                                
                                
                                break;
                        }

                    
                }
            }
            Console.ReadKey();

        }
    }
}