using FraudReporting.Models;
using FraudReporting.Utilities;
using FraudReporting.ViewModels.Home;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FraudReporting.Mappings
{
    public class FormEntryProfile : Profile
    {
        public FormEntryProfile()
        {
            CreateMap<FormViewModel, SessionData>();
        }
    }
}
