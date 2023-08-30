using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if VRC_SDK_VRCSDK3
namespace Chocopoi.AvatarLib.Expressions
{
    public class ParameterOverflowException : Exception
    {
        public ParameterOverflowException()
        {

        }

        public ParameterOverflowException(int original, int newAmount, int maxCapacity) : base(string.Format("Cannot add {3} bits of synced parameters! {0}/{2} -> {1}/{2} which exceeds {4} bits of data! Try removing some parameters before proceeding!", original, newAmount, maxCapacity, newAmount - original, newAmount - maxCapacity))
        {

        }
    }
}
#endif
