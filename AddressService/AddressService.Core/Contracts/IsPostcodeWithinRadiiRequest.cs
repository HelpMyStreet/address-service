﻿using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AddressService.Core.Contracts
{
    public class IsPostcodeWithinRadiiRequest : IRequest<IsPostcodeWithinRadiiResponse>
    {
        [Required]
        public string Postcode { get; set; }

        [Required]
        public List<PostcodeWithRadius> PostcodeWithRadiuses { get; set; }

    }
}