using System;

namespace AjuPrince.ORM
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AjuPrinceColumnAttribute : AjuPrinceBaseMappingAttribute
    {
        public AjuPrinceColumnAttribute(string name) : base(name)
        {

        }
    }
}
