using System.Linq;
using Microsoft.AspNetCore.Components.Forms;

namespace SimpleDevToolbox.Client.Support
{
    class BootstrapCssClassProvider : FieldCssClassProvider
    {
        public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
        {
            bool isInvalid = !editContext.GetValidationMessages(fieldIdentifier).Any();

            if (!isInvalid)
            {
                return "is-invalid";
            }

            if (editContext.IsModified(in fieldIdentifier))
            {
                return "is-valid";
            }

            return string.Empty;
        }
    }
}
