using System;

namespace AjuPrince.ORM
{
    public class AjuPrinceBaseMappingAttribute : Attribute
    {
        private string _Name = string.Empty;
        public AjuPrinceBaseMappingAttribute(string name) => this._Name = name;
        public virtual string GetName() => this._Name;
    }
}
