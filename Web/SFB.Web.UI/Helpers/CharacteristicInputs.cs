using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFB.Web.UI.Helpers
{
    public static class CharacteristicInputs
    {

        public static string NumberOfPupils { get
            {

                var html = @"<div class='form-group'><div class='column-half'><label class='form-label' for='MinNoPupil'>Between</label><input type='number' name='MinNoPupil' id='MinNoPupil' class='form-control' required></div><div class='column-half'><label class='form-label' for='MaxNoPupil'>and</label><input type='number' name='MaxNoPupil' id='MaxNoPupil' class='form-control' required></div></div>";

                return html.Replace("'", "\"").Replace("/", @"\/");
            }
        }
    }
}