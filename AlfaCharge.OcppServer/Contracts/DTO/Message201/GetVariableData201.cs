using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlfaCharge.OcppServer.Contracts.DTO.Message201
{
    public class GetVariableData201
    {
        public Component201 Component { get; set; } = default!;
        public Variable201 Variable { get; set; } = default!;
        public AttributeEnumType201? AttributeType { get; set; }
    }
}