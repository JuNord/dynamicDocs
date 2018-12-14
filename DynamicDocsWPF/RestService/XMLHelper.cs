using System;
using System.IO;
using System.Linq;
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
            return valid == XmlState.Valid ? processObject : null;
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
                                    if (AnyStringNullOrWhiteSpace(name, description)) return XmlState.Missingattribute;
                                    processObject = new ProcessObject(name, description);
                                    break;
                                case Wording.ProcessStep:
                                    if (processObject == null) return XmlState.Missingparenttag;
                                    if (processObject?.StepCount > 0)
                                    {
                                        if (AnyStringNullOrWhiteSpace(name, description, target))
                                            return XmlState.Missingattribute;
                                    }
                                    else
                                    {
                                        if (AnyStringNullOrWhiteSpace(name, description))
                                            return XmlState.Missingattribute;
                                    }

                                    processStep = new ProcessStep(processObject, name, description, target);
                                    processObject.AddStep(processStep);
                                    break;
                                case Wording.Dialog:
                                    if (processStep == null || processObject == null) return XmlState.Missingparenttag;
                                    dialog = new Dialog(processStep);
                                    processStep.AddDialog(dialog);
                                    break;

                                #endregion

                                #region InputTags

                                case Wording.TextInputBox:
                                    if (AnyStringNullOrWhiteSpace(name, description)) return XmlState.Missingattribute;
                                    if (dialog == null || processStep == null || processObject == null)
                                        return XmlState.Missingparenttag;
                                    var textInputBox = new TextInputBox(dialog, name, description, ToBool(obligatory));
                                    dialog.AddElement(textInputBox);
                                    break;

                                case Wording.NumberInputBox:
                                    if (AnyStringNullOrWhiteSpace(name, description)) return XmlState.Missingattribute;
                                    if (dialog == null || processStep == null || processObject == null)
                                        return XmlState.Missingparenttag;
                                    var numberInputBox =
                                        new NumberInputBox(dialog, name, description, ToBool(obligatory), calculation);
                                    dialog.AddElement(numberInputBox);
                                    break;

                                case Wording.TeacherDropdown:
                                    if (AnyStringNullOrWhiteSpace(name, description)) return XmlState.Missingattribute;
                                    if (dialog == null || processStep == null || processObject == null)
                                        return XmlState.Missingparenttag;
                                    var teacherDropdown =
                                        new TeacherDropdown(dialog, name, description, ToBool(obligatory));
                                    dialog.AddElement(teacherDropdown);
                                    break;

                                case Wording.StudentDropdown:
                                    if (AnyStringNullOrWhiteSpace(name, description)) return XmlState.Missingattribute;
                                    if (dialog == null || processStep == null || processObject == null)
                                        return XmlState.Missingparenttag;
                                    var studentDropdown =
                                        new StudentDropdown(dialog, name, description, ToBool(obligatory));
                                    dialog.AddElement(studentDropdown);
                                    break;
                                case Wording.DateDropdown:
                                    if (AnyStringNullOrWhiteSpace(name, description)) return XmlState.Missingattribute;
                                    if (dialog == null || processStep == null || processObject == null)
                                        return XmlState.Missingparenttag;
                                    var dateDropdown = new DateDropdown(dialog, name, description, ToBool(obligatory),
                                        calculation);
                                    dialog.AddElement(dateDropdown);
                                    break;
                                case
                                    Wording.ClassDropdown:
                                    if (AnyStringNullOrWhiteSpace(name, description)) return XmlState.Missingattribute;
                                    if (dialog == null || processStep == null || processObject == null)
                                        return XmlState.Missingparenttag;
                                    var classDropdown =
                                        new ClassDropDown(dialog, name, description, ToBool(obligatory));
                                    dialog.AddElement(classDropdown);
                                    break;

                                #endregion

                                #region ProcessTags              

                                case Wording.ArchivePermission:
                                    if (AnyStringNullOrWhiteSpace(target)) return XmlState.Missingattribute;
                                    if (processObject == null) return XmlState.Missingparenttag;
                                    var archivePermission = new ArchivePermissionElement(processObject, target);
                                    processObject.AddPermission(archivePermission);
                                    break;
                                case Wording.MailNotification:
                                    if (AnyStringNullOrWhiteSpace(target, text)) return XmlState.Missingattribute;
                                    if (processStep == null || processObject == null) return XmlState.Missingparenttag;
                                    var mailNotification = new MailNotificationElement(processStep, target, text);
                                    if (validation?.Accepted != null)
                                        validation.Accepted.AddNotification(mailNotification);
                                    else if (validation?.Declined != null)
                                        validation.Declined.AddNotification(mailNotification);
                                    else processStep.AddNotification(mailNotification);
                                    break;
                                case Wording.Receipt:
                                    if (AnyStringNullOrWhiteSpace(draftname, filepath))
                                        return XmlState.Missingattribute;
                                    if (processStep == null || processObject == null) return XmlState.Missingparenttag;
                                    var receipt = new ReceiptElement(processStep, draftname, filepath,
                                        ReceiptType.Word);
                                    if (validation?.Accepted != null)
                                        validation.Accepted.AddReceipt(receipt);
                                    else if (validation?.Declined != null)
                                        validation.Declined.AddReceipt(receipt);
                                    else processStep.AddReceipt(receipt);
                                    break;
                                case Wording.Validation:
                                    if (processStep == null || processObject == null) return XmlState.Missingparenttag;
                                    validation = new ValidationElement(processStep, ToBool(locks));
                                    processStep.AddValidation(validation);
                                    break;
                                case Wording.ValidationAccepted:
                                    if (validation == null || processStep == null || processObject == null)
                                        return XmlState.Missingparenttag;

                                    validation.Accepted = new ValidationAccepted(validation);

                                    break;
                                case Wording.ValidationDeclined:
                                    if (validation == null || processStep == null || processObject == null)
                                        return XmlState.Missingparenttag;

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
                                    if (processStep == null) return XmlState.Missingparenttag;
                                    processStep = null;
                                    break;
                                case Wording.Dialog:
                                    if (dialog == null) return XmlState.Missingparenttag;
                                    dialog = null;
                                    break;
                                case Wording.Validation:
                                    if (validation == null) return XmlState.Missingparenttag;
                                    validation = null;
                                    break;
                            }
                        }
                }
            }
            catch (XmlException)
            {
                processObject = null;
                return XmlState.Invalid;
            }
            catch (NullReferenceException)
            {
                processObject = null;
                return XmlState.Invalid;
            }

            return IsValidProcess(processObject) ? XmlState.Valid : XmlState.Invalid;
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
            return values.Any(e => string.IsNullOrWhiteSpace(e));
        }

        private static bool IsValidProcess(ProcessObject processObject)
        {
            foreach (var step in processObject.Steps)
            {
                if (step.ValidationCount > 1) return false;
                if (step.ValidationCount < 1 && step.DialogCount < 1) return false;
                foreach (var dialog in step.Dialogs)
                    if (dialog.ElementCount < 1)
                        return false;
            }

            return true;
        }

        private static bool ToBool(string obligatory)
        {
            return obligatory?.Equals("true") ?? false;
        }
    }
}