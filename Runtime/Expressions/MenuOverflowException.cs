using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if VRC_SDK_VRCSDK3
namespace Chocopoi.AvatarLib.Expressions
{
    public class MenuOverflowException : Exception
    {
        public MenuOverflowException()
        {

        }

        public MenuOverflowException(string controlName, int amount) : base(string.Format("Cannot add more controls ({0}) to the current menu with {1} controls already!", controlName, amount))
        {

        }
    }
}
#endif
