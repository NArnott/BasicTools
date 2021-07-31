using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
