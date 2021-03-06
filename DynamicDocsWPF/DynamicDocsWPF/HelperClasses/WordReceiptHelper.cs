using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Shapes;
using Microsoft.Office.Interop.Word;
using Application = System.Windows.Application;

namespace DynamicDocsWPF.HelperClasses
{
    public class WordReceiptHelper
    {
        public static void OpenDocument(string documentPath, params KeyValuePair<string, string>[] replacements)
        {
            var wordApp = new ApplicationClass();
            // File Path
            var strFilePath = $"{System.AppDomain.CurrentDomain.BaseDirectory}\\{documentPath}";

            if (!File.Exists(strFilePath)) return;

            // Create obj filename to pass it as paremeter in open 
            object objFile = strFilePath;
            object objNull = Missing.Value;
            object objReadOnly = false;

            var doc = wordApp.Documents.Open(ref objFile,
                ref objNull,
                ref objReadOnly,
                ref objNull,
                ref objNull,
                ref objNull,
                ref objNull,
                ref objNull,
                ref objNull,
                ref objNull,
                ref objNull,
                ref objNull,
                ref objNull,
                ref objNull,
                ref objNull,
                ref objNull);
            foreach (var replacement in replacements)
                FindAndReplaceMethods(wordApp, replacement.Key, replacement.Value);
            // close document and Quit Word

            doc.Close(ref objNull, ref objNull, ref objNull);

            wordApp.Quit(ref objNull, ref objNull, ref objNull);
        }

        private static void FindAndReplaceMethods(_Application doc, object findText, object replaceWithText)
        {
            //Find and Replace Options
            object matchCase = false;
            object matchWholeWord = true;
            object matchWildCards = false;
            object matchSoundsLike = false;
            object matchAllWordForms = false;
            object forward = true;
            object format = false;
            object matchKashida = false;
            object matchDiacritics = false;
            object matchAlefHamza = false;
            object matchControl = false;
            object read_only = false;
            object visible = true;
            object replace = 2;
            object wrap = 1;

            doc.Selection.Find.ClearFormatting();
            //Find and Replace
            doc.Selection.Find.Execute(ref findText,
                ref matchCase,
                ref matchWholeWord,
                ref matchWildCards,
                ref matchSoundsLike,
                ref matchAllWordForms,
                ref forward,
                ref wrap,
                ref format,
                ref replaceWithText,
                ref replace,
                ref matchKashida,
                ref matchDiacritics,
                ref matchAlefHamza,
                ref matchControl);
        }
    }
}