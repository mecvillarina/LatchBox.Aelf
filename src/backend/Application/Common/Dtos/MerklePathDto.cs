using System.Collections.Generic;

namespace Application.Common.Dtos
{
    public class MerklePathDto
    {
        public List<MerklePathNodeDto> MerklePathNodes { get; set; }
    }

    public class MerklePathNodeDto
    {
        public string Hash { get; set; }
        public bool IsLeftChildNode { get; set; }
    }
}
