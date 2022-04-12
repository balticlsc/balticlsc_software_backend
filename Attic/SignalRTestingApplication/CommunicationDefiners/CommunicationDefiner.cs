using System;
using System.Collections.Generic;

namespace SignalRTestingApplication.CommunicationDefiners
{
    public abstract class CommunicationDefiner
    {
        protected const int MinParametersNumber = 1;
        protected const int MaxParametersNumber = 8;

        public string Url { get; protected set; }
        public List<Type> ParametersTypes { get; }
        public string MethodName { get; protected set; }
        protected int ParametersNumber;

        protected CommunicationDefiner()
        {
            ParametersTypes = new List<Type>();
        }
    }
}