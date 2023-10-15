using System;

namespace Chocopoi.AvatarLib.Animations
{
    public class ParameterMismatchException : Exception
    {
        public ParameterMismatchException() { }

        public ParameterMismatchException(string parameter, string existingType, string incomingType) :
            base(string.Format("Parameter {0} type mismatch! existing: {1} incoming: {2}", parameter, existingType, incomingType))
        { }
    }
}
