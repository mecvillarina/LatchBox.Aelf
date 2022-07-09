using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApi
{
    public static class Extensions
    {
        public static Task<IActionResult> ToTask(this IActionResult result)
        {
            return Task.FromResult(result);
        }
    }
}