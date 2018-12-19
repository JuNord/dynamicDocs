using System;

namespace RestService
{
    public class XmlFormatException : Exception
    {
        public string TagName { get; private set; }
        public XmlState State { get; private set; }
        public XmlFormatException(string tagName, XmlState state) : base()
        {
            TagName = tagName;
            State = state;
        }
    }
}