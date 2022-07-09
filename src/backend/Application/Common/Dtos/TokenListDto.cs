using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class TokenListDto
    {
        public string Message { get; set; }
        public TokenDataDto Data { get; set; }
        public int Total { get; set; }
    }

    public class TokenDataDto
    {
        public List<TokenDto> List { get; set; }
    }
}
