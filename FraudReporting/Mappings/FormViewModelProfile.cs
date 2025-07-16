using FraudReporting.Models;
using FraudReporting.ViewModels.Home;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FraudReporting.Mappings
{
    public class FormViewModelProfile : Profile
    {
        public FormViewModelProfile()
        {
            CreateMap<SessionData, FormViewModel>();
        }
    }
}
