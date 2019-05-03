using System;

namespace AjuPrince.ORM
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AjuPrinceTableAttribute : AjuPrinceBaseMappingAttribute
    {
        public AjuPrinceTableAttribute(string name) : base(name)
        {
        }
    }
}
