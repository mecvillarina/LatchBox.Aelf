using System;

namespace Application.Common.Extensions
{
    public static class TryCatchExtensions
    {
        /// <summary>
        ///     Tries to execute the given action. If it fails, nothing happens.
        /// </summary>
        public static void Try(Action action)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
                //
            }
        }
    }
}