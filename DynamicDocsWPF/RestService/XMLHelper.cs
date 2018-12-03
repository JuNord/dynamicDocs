using System;
using System.IO;
using System.Text;
using System.Xml;
using DynamicDocsWPF.Model;
using RestService.Model.Input;
using RestService.Model.Process;

namespace RestService
{
    public class XmlHelper
    {
        public static ProcessObject ReadXMLFromString(string content)
        {
            using (var reader = XmlReader.Create(new StringReader(content)))
            {
                ProcessObject processObject = null;
                ProcessStep processStep = null;
                Dialog dialog = null;
                while (reader.Read())
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        //Auslesen der möglichen Attribute
                        var name = reader.GetAttribute("name");
                        var description = reader.GetAttribute("description");
                        var target = reader.GetAttribute("target");
                        var locks = reader.GetAttribute("locks");
                        var vText = reader.GetAttribute("text");
                        var draftname = reader.GetAttribute("draftname");
                        var filepath = reader.GetAttribute("filepath");
                        var obligatory = reader.GetAttribute("obligatory");

                        //Vergleiche, erzeuge und weise Objekte zu
                        switch (reader.Name.ToLower())
                        {
                            #region SurroundingTags

                            case Wording.Process:
                                processObject = new ProcessObject(name, description);
                                break;
                            case Wording.ProcessStep:
                                processStep = new ProcessStep(processObject, name, description, target);
                                processObject?.AddStep(processStep);
                                break;
                            case Wording.Dialog:
                                dialog = new Dialog(processStep);
                                processStep.AddDialog(dialog);
                                break;

                            #endregion

                            #region InputTags

                            case Wording.TextInputBox:
                                var textInputBox = new TextInputBox(dialog, name, description, ToBool(obligatory));
                                dialog.AddElement(textInputBox);
                                break;

                            case Wording.NumberInputBox:
                                var numberInputBox = new NumberInputBox(dialog, name, description, ToBool(obligatory));
                                dialog.AddElement(numberInputBox);
                                break;

                            case Wording.TeacherDropdown:
                                var teacherDropdown =
                                    new TeacherDropdown(dialog, name, description, ToBool(obligatory));
                                dialog.AddElement(teacherDropdown);
                                break;

                            case Wording.StudentDropdown:
                                var studentDropdown =
                                    new StudentDropdown(dialog, name, description, ToBool(obligatory));
                                dialog.AddElement(studentDropdown);
                                break;
                            case Wording.DateDropdown:
                                var dateDropdown = new DateDropdown(dialog, name, description, ToBool(obligatory));
                                dialog.AddElement(dateDropdown);
                                break;
                            case
                                Wording.ClassDropdown:
                                var classDropdown = new ClassDropDown(dialog, name, description, ToBool(obligatory));
                                dialog.AddElement(classDropdown);
                                break;

                            #endregion

                            #region ProcessTags              

                            case Wording.ArchivePermission:
                                var archivePermission = new ArchivePermissionElement(processObject, target);
                                processObject.AddPermission(archivePermission);
                                break;
                            case Wording.MailNotification:
                                var mailNotification = new MailNotificationElement(processStep);
                                processStep.AddNotification(mailNotification);
                                break;
                            case Wording.Receipt:
                                var receipt = new ReceiptElement(processStep, draftname, filepath, ReceiptType.WORD);
                                processStep.AddReceipt(receipt);
                                break;
                            case Wording.Validation:
                                var validation = new ValidationElement(processStep, ToBool(locks));
                                processStep.AddValidation(validation);
                                break;

                            #endregion

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                return processObject;
            }
        }

        public static ProcessObject ReadXMLFromBytes(byte[] content, Encoding encoding)
        {
            return ReadXMLFromString(encoding.GetString(content));
        }

        public static ProcessObject ReadXMLFromPath(string path)
        {
            return ReadXMLFromString(File.ReadAllText(path));
        }

        private static bool ToBool(string obligatory)
        {
            return obligatory?.Equals("true") ?? false;
        }
    }
}