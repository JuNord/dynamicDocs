using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Xml;
using DynamicDocsWPF.Model;
using RestService.Model.Input;
using RestService.Model.Process;

namespace RestService
{
    public class XmlHelper
    {
        public static ProcessObject ReadXmlFromString(string content)
        {
            var valid = TryReadXmlFromString(content, out var processObject);
            return valid == XmlState.VALID ? processObject : null;
        }
        
        public static XmlState TryReadXmlFromString(string content, out ProcessObject processObject)
        {
            try
            {
                using (var reader = XmlReader.Create(new StringReader(content)))
                {
                    processObject = null;
                    ProcessStep processStep = null;
                    Dialog dialog = null;
                    ValidationElement validation = null;
                    while (reader.Read())
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            //Auslesen der möglichen Attribute
                            var name = reader.GetAttribute("name");
                            var description = reader.GetAttribute("description");
                            var target = reader.GetAttribute("target");
                            var locks = reader.GetAttribute("locks");
                            var text = reader.GetAttribute("text");
                            var draftname = reader.GetAttribute("draftname");
                            var filepath = reader.GetAttribute("filepath");
                            var obligatory = reader.GetAttribute("obligatory");
                            var calculation = reader.GetAttribute("calculation");

                            //Vergleiche, erzeuge und weise Objekte zu

                            switch (reader.Name.ToLower())
                            {
                                #region SurroundingTags

                                case Wording.Process:
                                    if(AnyStringNullOrWhiteSpace(name, description)) return XmlState.MISSINGATTRIBUTE;
                                    processObject = new ProcessObject(name, description);
                                    break;
                                case Wording.ProcessStep:
                                    if (processObject == null) return XmlState.MISSINGPARENTTAG;
                                    if (processObject?.ProcessStepCount > 0)
                                    {
                                        if (AnyStringNullOrWhiteSpace(name, description, target))
                                            return XmlState.MISSINGATTRIBUTE;
                                    }
                                    else
                                    {
                                        if (AnyStringNullOrWhiteSpace(name, description))
                                            return XmlState.MISSINGATTRIBUTE;
                                    }                                 
                                    processStep = new ProcessStep(processObject, name, description, target);
                                    processObject.AddStep(processStep);
                                    break;
                                case Wording.Dialog:
                                    if (processStep == null || processObject == null) return XmlState.MISSINGPARENTTAG;
                                    dialog = new Dialog(processStep);
                                    processStep.AddDialog(dialog);
                                    break;

                                #endregion

                                #region InputTags

                                case Wording.TextInputBox:
                                    if (AnyStringNullOrWhiteSpace(name, description)) return XmlState.MISSINGATTRIBUTE;
                                    if (dialog == null || processStep == null || processObject == null) return XmlState.MISSINGPARENTTAG;
                                    var textInputBox = new TextInputBox(dialog, name, description, ToBool(obligatory));
                                    dialog.AddElement(textInputBox);
                                    break;

                                case Wording.NumberInputBox:
                                    if (AnyStringNullOrWhiteSpace(name, description)) return XmlState.MISSINGATTRIBUTE;
                                    if (dialog == null || processStep == null || processObject == null) return XmlState.MISSINGPARENTTAG;
                                    var numberInputBox =
                                        new NumberInputBox(dialog, name, description, ToBool(obligatory), calculation);
                                    dialog.AddElement(numberInputBox);
                                    break;

                                case Wording.TeacherDropdown:
                                    if (AnyStringNullOrWhiteSpace(name, description)) return XmlState.MISSINGATTRIBUTE;
                                    if (dialog == null || processStep == null || processObject == null) return XmlState.MISSINGPARENTTAG;
                                    var teacherDropdown =
                                        new TeacherDropdown(dialog, name, description, ToBool(obligatory));
                                    dialog.AddElement(teacherDropdown);
                                    break;

                                case Wording.StudentDropdown:
                                    if (AnyStringNullOrWhiteSpace(name, description)) return XmlState.MISSINGATTRIBUTE;
                                    if (dialog == null || processStep == null || processObject == null) return XmlState.MISSINGPARENTTAG;
                                    var studentDropdown =
                                        new StudentDropdown(dialog, name, description, ToBool(obligatory));
                                    dialog.AddElement(studentDropdown);
                                    break;
                                case Wording.DateDropdown:
                                    if (AnyStringNullOrWhiteSpace(name, description)) return XmlState.MISSINGATTRIBUTE;
                                    if (dialog == null || processStep == null || processObject == null) return XmlState.MISSINGPARENTTAG;
                                    var dateDropdown = new DateDropdown(dialog, name, description, ToBool(obligatory), calculation);
                                    dialog.AddElement(dateDropdown);
                                    break;
                                case
                                    Wording.ClassDropdown:
                                    if (AnyStringNullOrWhiteSpace(name, description)) return XmlState.MISSINGATTRIBUTE;
                                    if (dialog == null || processStep == null || processObject == null) return XmlState.MISSINGPARENTTAG;
                                    var classDropdown =
                                        new ClassDropDown(dialog, name, description, ToBool(obligatory));
                                    dialog.AddElement(classDropdown);
                                    break;

                                #endregion

                                #region ProcessTags              
                                case Wording.ArchivePermission:
                                    if (AnyStringNullOrWhiteSpace(target)) return XmlState.MISSINGATTRIBUTE;
                                    if (processObject == null) return XmlState.MISSINGPARENTTAG;
                                    var archivePermission = new ArchivePermissionElement(processObject, target);
                                    processObject.AddPermission(archivePermission);
                                    break;
                                case Wording.MailNotification:
                                    if (AnyStringNullOrWhiteSpace(target, text)) return XmlState.MISSINGATTRIBUTE;
                                    if (processStep == null || processObject == null) return XmlState.MISSINGPARENTTAG;
                                    var mailNotification = new MailNotificationElement(processStep, target, text);
                                    if (validation?.Accepted != null)
                                        validation.Accepted.AddNotification(mailNotification);
                                    else if (validation?.Declined != null)
                                        validation.Declined.AddNotification(mailNotification);
                                    else processStep.AddNotification(mailNotification);
                                    break;
                                case Wording.Receipt:
                                    if (AnyStringNullOrWhiteSpace(draftname, filepath)) return XmlState.MISSINGATTRIBUTE;
                                    if (processStep == null || processObject == null) return XmlState.MISSINGPARENTTAG;
                                    var receipt = new ReceiptElement(processStep, draftname, filepath,
                                        ReceiptType.WORD);
                                    if (validation?.Accepted != null)
                                        validation.Accepted.AddReceipt(receipt);
                                    else if (validation?.Declined != null)
                                        validation.Declined.AddReceipt(receipt);
                                    else processStep.AddReceipt(receipt);
                                    break;
                                case Wording.Validation:
                                    if (processStep == null || processObject == null) return XmlState.MISSINGPARENTTAG;
                                    validation = new ValidationElement(processStep, ToBool(locks));
                                    processStep.AddValidation(validation);
                                    break;
                                case Wording.ValidationAccepted:
                                    if (validation == null || processStep == null || processObject == null) return XmlState.MISSINGPARENTTAG;
                                    
                                    validation.Accepted = new ValidationAccepted(validation);
                                    
                                    break;
                                case Wording.ValidationDeclined:
                                    if (validation == null || processStep == null || processObject == null) return XmlState.MISSINGPARENTTAG;
                                    
                                    validation.Declined = new ValidationDeclined(validation);
                                    break;

                                #endregion

                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement)
                        {
                            switch (reader.Name.ToLower())
                            {
                                case Wording.ProcessStep:
                                    if (processStep == null) return XmlState.MISSINGPARENTTAG;
                                    processStep = null;
                                    break;
                                case Wording.Dialog:
                                    if (dialog == null) return XmlState.MISSINGPARENTTAG;
                                    dialog = null;
                                    break;
                                case Wording.Validation:
                                    if (validation == null) return XmlState.MISSINGPARENTTAG;
                                    validation = null;
                                    break;
                            }
                        }

                   
                }
            }
            catch (NullReferenceException)
            {
                processObject = null;
                return XmlState.INVALID;
            }

            return IsValidProcess(processObject) ? XmlState.VALID : XmlState.INVALID;
        }

        public static XmlState TryReadXmlFromPath(string path, out ProcessObject processObject)
        {
            return TryReadXmlFromString(File.ReadAllText(path), out processObject);
        }
        
        public static ProcessObject ReadXmlFromPath(string path)
        {
            return ReadXmlFromString(File.ReadAllText(path));
        }

        private static bool AnyStringNullOrWhiteSpace(params string[] values)
        {
            return values.All(string.IsNullOrWhiteSpace);
        }
        
        private static bool IsValidProcess(ProcessObject processObject)
        {
            foreach (var step in processObject.Steps)
            {
                if (step.ValidationCount > 1) return false;
                if (step.ValidationCount < 1 && step.DialogCount < 1) return false;
                foreach (var dialog in step.Dialogs)
                {
                    if (dialog.ElementCount < 1) return false;
                }
            }
            return true;
        }

        private static bool ToBool(string obligatory)
        {
            return obligatory?.Equals("true") ?? false;
        }
    }
}