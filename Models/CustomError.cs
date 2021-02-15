using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class CustomError
    {
        public CustomError()
        {

        }

        public CustomError(ModelStateDictionary state)
        {
            Message = "Invalid parameter(s) has / have been detected.";
            Detail = state
                .FirstOrDefault(e => e.Value.Errors.Any()).Value.Errors
                .FirstOrDefault().ErrorMessage;
        }

        public string Message { get; set; }
        public string Detail { get; set; }
    }
}
