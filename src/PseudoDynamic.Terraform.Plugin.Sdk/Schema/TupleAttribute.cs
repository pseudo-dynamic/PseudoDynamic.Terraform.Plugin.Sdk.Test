﻿using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    public class TupleAttribute : BlockAttribute
    {
        public TupleAttribute()
            : base(TerraformType.Tuple)
        {
        }
    }
}
