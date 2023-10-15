using System;

namespace Chocopoi.AvatarLib.Animations
{
    public class ParameterNotExistException : Exception
    {
        public ParameterNotExistException()
        {

        }

        public ParameterNotExistException(string parameter, Type type) : base(string.Format("Parameter {0} with type {1} not exist!", parameter, type.Name))
        {

        }
    }
}
