using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Profiles
{
    public class AutoMapperConfig
    {
        public static void Register()
        {
            Mapper.Initialize(
                map =>
                {
                    map.AddProfile<EntityToViewModel>();
                    map.AddProfile<ViewModelToEntity>();
                    map.AddProfile<ViewModelToViewModel>();
                });
        }
    }
}
